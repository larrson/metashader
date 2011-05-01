using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;

namespace metashader.GraphEditor
{
    /// <summary>
    /// グラフエディタ用カメラ
    /// 初期の画角(最小画角)で、ワールド座標系でZ=0平面がDotByDotでスクリーン上で表示されるように調整されたカメラ
    /// </summary>
    class GECamera
    {
#region variables
        /// <summary>
        /// 行列カメラ
        /// </summary>
        MatrixCamera m_camera = new MatrixCamera();
        /// <summary>
        /// 視点位置
        /// </summary>
        Vector3D m_eyePos;
        /// <summary>
        /// カメラの上方向
        /// </summary>
        Vector3D m_upDir = new Vector3D(0, 1, 0);
        /// <summary>
        /// 注視点の位置
        /// </summary>
        Vector3D m_lookAt = new Vector3D(0, 0, 0);
        /// <summary>
        /// 近クリップ面までの距離
        /// </summary>
        double m_near = 1.0;
        /// <summary>
        /// 遠クリップ面までの距離
        /// </summary>
        double m_far = 3000;
        /// <summary>
        /// アスペクト比(横/縦)
        /// </summary>
        double m_aspectRatio = 60;

        /// <summary>
        /// 垂直画角の最大値(度単位)
        /// </summary>
        static readonly double m_maxFov = 120.0f;
        /// <summary>
        /// 垂直が書くの最小値(度単位)
        /// </summary>
        static readonly double m_minFov = 60.0f;
        /// <summary>
        /// 垂直画角(度単位、半分ではない)
        /// </summary>
        double m_fov = m_minFov;

        /// <summary>
        /// 現在のスクリーン幅
        /// </summary>
        double m_width;
        /// <summary>
        /// 現在のスクリーンの高さ
        /// </summary>
        double m_height;
        /// <summary>
        /// ワールド座標系でのスクリーンのZ座標
        /// </summary>
        double m_screenZAtWorld;

        /// <summary>
        /// カメラの逆変換行列
        /// </summary>
        Matrix3D m_inverseMatrix;
#endregion        
        
#region constructors
        public GECamera()
        {                                   
            UpdateCamera();
        }
#endregion

#region properties
        /// <summary>
        /// カメラを取得する
        /// </summary>
        public MatrixCamera Camera
        {
            get { return m_camera; }
        }        

        /// <summary>
        /// 視点位置
        /// </summary>
        public Vector3D Position
        {
            get { return m_eyePos; }
        }
#endregion

#region public methods
        public void Initialize(double width, double height)
        {
            // 幅・高さを保持
            m_width = width;
            m_height = height;
 
            // アスペクト比を求める
            m_aspectRatio = width / height;

            // Fovとアスペクト比から距離を計算
            double z = height * 0.5 / Math.Tan(m_fov * Math.PI / 360.0);

            // 位置を設定
            m_eyePos = new Vector3D(0, 0, z);

            UpdateCamera();
        }

        /// <summary>
        /// リサイズ
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void Resize(double width, double height)
        {
            // アスペクト比を更新
            m_aspectRatio = width / height;

            // 投影面からの距離を更新
            double z = (height / m_height) * m_eyePos.Z;

            // スクリーンの幅・高さを更新
            m_width = width;
            m_height = height;

            // 位置を設定
            m_eyePos = new Vector3D(m_eyePos.X, m_eyePos.Y, z);

            UpdateCamera();
        }

        /// <summary>
        /// ズームする
        /// </summary>
        /// <param name="delta">画角の変更差分</param>
        public void Zoom(double delta)
        {
            m_fov += delta;

            // クランプする
            m_fov = Math.Max( m_minFov, Math.Min(m_fov, m_maxFov));

            UpdateCamera();
        }

        /// <summary>
        /// 移動させる
        /// </summary>       
        /// <param name="delta">移動差分</param>
        public void Move(Vector3D delta)
        {            
            // 位置を移動
            m_eyePos += delta;

            // 注視点も合わせて移動する
            m_lookAt += delta;

            // 更新
            UpdateCamera();
        }

        /// <summary>
        /// 仮想スクリーン上の2D座標をワールド3次元空間の位置に変換する
        /// </summary>
        /// <returns></returns>
        public Vector3D ToWorldPosFromVirtualScreen(Point screenPos)
        {
            Vector3D ret = new Vector3D(
                       screenPos.X - m_width / 2
                    , -screenPos.Y + m_height / 2
                    , 0                    
                );
            return ret;
        }

        /// <summary>
        /// クライアント座標から、ワールド位置を求める
        /// </summary>
        public Vector3D ToWorldPosFromClient( Point screenPos )
        {
            double X =  screenPos.X / (m_width * 0.5) - 1;
            double Y = -screenPos.Y / (m_height * 0.5) + 1; 
            Point4D world = m_inverseMatrix.Transform( new Point4D(X, Y, m_screenZAtWorld, 1) );            
            Vector3D ret = new Vector3D( world.X / world.W, world.Y / world.W, world.Z / world.W);

            return ret;
        }

        /// <summary>
        /// クライアント座標を仮想スクリーンの左上隅を基準としたワールド座標へ変換する
        /// </summary>
        /// <returns></returns>
        public Point ToVirtualScreenPos( Point screenPos )
        {
            Vector3D worldPos = ToWorldPosFromClient(screenPos);

            // 左上隅のオフセットを加える
            Point ret = new Point( worldPos.X + m_width/2, -(worldPos.Y - m_height/2));
            return ret;
        }
#endregion

#region private methods
        /// <summary>
        /// 現在のパラメータに基づいてカメラを更新
        /// </summary>
        void UpdateCamera()
        {
            m_camera.ViewMatrix = LookAtRH();
            m_camera.ProjectionMatrix = PerspectiveFovRH();

            Matrix3D viewProj = Matrix3D.Multiply(m_camera.ViewMatrix, m_camera.ProjectionMatrix);

            // 逆行列を更新
            m_inverseMatrix = viewProj;
            m_inverseMatrix.Invert();

            // スクリーンの位置を更新
            Point4D screenPosAtWorld = viewProj.Transform(new Point4D(0, 0, 0, 1));
            m_screenZAtWorld = screenPosAtWorld.Z / screenPosAtWorld.W;
        }

        /// <summary>
        /// ビュー行列を計算する
        /// </summary>
        /// <returns></returns>
        Matrix3D LookAtRH()
        {
            Matrix3D ret = new Matrix3D();

            // 視線方向単位ベクトル(ビューのZ軸)
            Vector3D v = new Vector3D(m_eyePos.X - m_lookAt.X, m_eyePos.Y - m_lookAt.Y, m_eyePos.Z - m_lookAt.Z);
            v.Normalize();
            // 視線と、上方向に直交する単位ベクトル(ビューのY軸)
            Vector3D r = Vector3D.CrossProduct(m_upDir, v); r.Normalize();
            // 上方向単位ベクトル(ビューのX軸)
            Vector3D u = Vector3D.CrossProduct(v, r);

            // 回転成分
            ret.M11 = r.X; ret.M12 = u.X; ret.M13 = v.X; ret.M14 = 0.0f;
            ret.M21 = r.Y; ret.M22 = u.Y; ret.M23 = v.Y; ret.M24 = 0.0f;
            ret.M31 = r.Z; ret.M32 = u.Z; ret.M33 = v.Z; ret.M34 = 0.0f;

            // 移動成分
            ret.OffsetX = -Vector3D.DotProduct(r, m_eyePos);
            ret.OffsetY = -Vector3D.DotProduct(u, m_eyePos);
            ret.OffsetZ = -Vector3D.DotProduct(v, m_eyePos);
            ret.M44 = 1.0f;

            return ret;
        }

        /// <summary>
        /// 射影行列を計算する
        /// </summary>
        /// <returns></returns>
        Matrix3D PerspectiveFovRH()
        {
            double xmin, xmax, ymin, ymax;
            ymax = m_near * Math.Tan(m_fov * Math.PI/ 360.0);
            ymin = -ymax;
            xmin = ymin * m_aspectRatio;
            xmax = ymax * m_aspectRatio;

            return PerspectiveFovRH(xmax - xmin, ymax - ymin, m_near, m_far);
        }

        /// <summary>
        /// 射影行列を計算する
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        /// <returns></returns>
        Matrix3D PerspectiveFovRH( double width, double height, double near, double far)
        {
            Matrix3D ret = new Matrix3D();

            double Q = far / (far - near);

            ret.M11 = 2 * near / width;
            ret.M22 = 2 * near / height;
            ret.M33 = -2 * Q + 1;

            ret.M34 = -1;
            ret.M44 = 0;

            ret.OffsetZ = -2 * Q * near;

            return ret;
        }
#endregion
    }
}
