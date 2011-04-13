using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace metashader.GraphEditor
{
    /// <summary>
    /// リンクを表すPathを保持するオブジェクト
    /// </summary>
    public class LinkPath
    {
#region variable
        /// <summary>
        /// パス
        /// </summary>
        Path m_path;
        /// <summary>
        /// パス内の図形
        /// </summary>
        PathFigure m_pathFigure;
        /// <summary>
        /// 図形内のPathセグメント
        /// </summary>
        BezierSegment m_bezierSegment;

        /// <summary>
        /// 接続先の入力ジョイントをもつコントロール
        /// </summary>
        ShaderNodeControl m_inputNode;
        /// <summary>
        /// 入力ジョイントのインデックス
        /// </summary>
        int m_inputJointIndex;

        /// <summary>
        /// 接続先の出力ジョイントをもつコントロール
        /// </summary>
        ShaderNodeControl m_outputNode;
        /// <summary>
        /// 出力ジョイントのインデックス
        /// </summary>
        int m_outputJointIndex;

        /// <summary>
        /// 曲線の開始位置
        /// </summary>
        Point m_startPos;

        /// <summary>
        /// 曲線の終了位置
        /// </summary>
        Point m_endPos;
#endregion

#region properties
        /// <summary>
        /// パス
        /// </summary>
        public Path Path
        {
            get { return m_path; }
        }        

        /// <summary>
        /// 曲専用制御点1
        /// </summary>
        private Point ControlPoint1
        {
            get 
            {
                double x = (m_endPos.X - m_startPos.X) * 0.25 + m_startPos.X;
                double y = m_startPos.Y;

                return new Point(x, y);
            }
        }

        /// <summary>
        /// 曲専用制御点2
        /// </summary>
        private Point ControlPoint2
        {
            get
            {
                double x = (m_endPos.X - m_startPos.X) * 0.75 + m_startPos.X;
                double y = m_endPos.Y;

                return new Point(x, y);
            }
        }
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public LinkPath(ShaderNodeControl inputNode, int inputJointIndex, ShaderNodeControl outputNode, int outputJointIndex )            
        {            
            // 初期化      

            // パスの作成
            m_path = new Path();
            m_path.Stroke = Brushes.Black;
            m_path.StrokeThickness = 1;

            m_pathFigure = new PathFigure();           
            m_bezierSegment = new BezierSegment();
            m_pathFigure.Segments = new PathSegmentCollection();
            m_pathFigure.Segments.Add( m_bezierSegment );

            m_path.Data = new PathGeometry();
            (m_path.Data as PathGeometry).Figures = new PathFigureCollection();
            (m_path.Data as PathGeometry).Figures.Add(m_pathFigure);

            // メンバ変数初期化
            m_inputNode = inputNode;            
            m_inputJointIndex = inputJointIndex;
            m_startPos = inputNode.GetInputJointPos(m_inputJointIndex);

            m_outputNode = outputNode;
            m_outputJointIndex = outputJointIndex;
            m_endPos = outputNode.GetOutputJointPos(m_outputJointIndex);
            
            // 制御点の設定
            UpdateControlPoints();

            // イベント登録(@@@削除時イベント登録解除)
            m_inputNode.PropertyChanged += new PropertyChangedEventHandler(inputNode_PropertyChanged);
            m_outputNode.PropertyChanged += new PropertyChangedEventHandler(outputNode_PropertyChanged);
        }        
#endregion

#region public methods
        /// <summary>
        /// 削除時に呼ばれるメソッド
        /// </summary>
        public void OnDeleted()
        {
            // イベントの登録解除
            m_inputNode.PropertyChanged -= new PropertyChangedEventHandler(inputNode_PropertyChanged);
            m_outputNode.PropertyChanged -= new PropertyChangedEventHandler(outputNode_PropertyChanged);
        }
#endregion

#region event handlers
        /// <summary>
        /// 入力ノードのプロパティが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void inputNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 接続先の入力ノードの位置のプロパティが変更された
            if( e.PropertyName == "Position")
            {
                m_startPos = (sender as ShaderNodeControl).GetInputJointPos(m_inputJointIndex);

                // 制御点の更新
                UpdateControlPoints();
            }
        }

        /// <summary>
        /// 出力ノードのプロパティが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void outputNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 接続先の出力ノードの位置のプロパティが変更された
            if (e.PropertyName == "Position")
            {
                m_endPos = (sender as ShaderNodeControl).GetOutputJointPos(m_outputJointIndex);

                // 制御点の更新
                UpdateControlPoints();
            }
        }   
#endregion

#region private methods
        /// <summary>
        /// パスの制御点を更新する
        /// </summary>
        private void UpdateControlPoints()
        {
            m_pathFigure.StartPoint = m_startPos;
            m_bezierSegment.Point1 = ControlPoint1;
            m_bezierSegment.Point2 = ControlPoint2;
            m_bezierSegment.Point3 = m_endPos;
        }
#endregion
    }
}
