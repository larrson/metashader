using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using metashader.Common;

namespace metashader.GraphEditor.Thumnail
{
    /// <summary>
    /// ベクトル4成分を色として表示するサムネイルクラス
    /// </summary>
    public class ColorThumnail : ThumnailControl
    {
        #region variables

        #endregion

        public ColorThumnail(ShaderGraphData.ShaderNodeDataBase node)
            : base(node)
        {
            InitializeComponent();                              
 
            // サムネイル部分が正方形になるように幅を設定
            this.Width = this.Height;

            // 背景色変更            
            float r,g,b,a;
            r = g = b = a = 0.0f;
            if( NodeData.Type == "Uniform_Vector3" )
            {
                ShaderGraphData.Uniform_Vector3Node vector3Node = NodeData as ShaderGraphData.Uniform_Vector3Node;
                r = vector3Node.Values[0];
                g = vector3Node.Values[1];
                b = vector3Node.Values[2];
                a = 1.0f;
            }
            else if ( NodeData.Type == "Uniform_Vector4" )
            {
                ShaderGraphData.Uniform_Vector4Node vector4Node = NodeData as ShaderGraphData.Uniform_Vector4Node;
                r = vector4Node.Values[0];
                g = vector4Node.Values[1];
                b = vector4Node.Values[2];
                a = vector4Node.Values[3];
            }
            SetBackgroundColor(r, g, b, a);            
        }

        #region event handlers
        /// <summary>
        /// ダブルクリックイベントを処理するメソッド        
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        protected override void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
            dialog.AllowFullOpen = true;
            dialog.FullOpen = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // 4Dベクトルパラメータを変更
                // Undo/Redoバッファ
                UndoRedoBuffer undoredo = new UndoRedoBuffer();

                // ベクトルの4成分を取得
                float[] values = null;
                if (NodeData.Type == "Uniform_Vector4")
                {
                    values = new float[4];
                    values[0] = dialog.Color.R / 255f;
                    values[1] = dialog.Color.G / 255f;
                    values[2] = dialog.Color.B / 255f;
                    values[3] = dialog.Color.A / 255f;
                }
                else if (NodeData.Type == "Uniform_Vector3")
                {
                    values = new float[3];
                    values[0] = dialog.Color.R / 255f;
                    values[1] = dialog.Color.G / 255f;
                    values[2] = dialog.Color.B / 255f;
                }

                App.CurrentApp.GraphData.ChangeNodeProperty<float[]>(NodeData, "Values", values, undoredo);

                if (undoredo.IsValid)
                {
                    UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ノードのプロパティ変更時に親コントロールから呼ばれる処理
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="args">イベント引数</param>
        public override void OnNodePropertyChanged(object sender, Event.NodePropertyChangedEventArgs args)
        {
            // 背景を変更するか
            bool bChangeBackground = args.PropertyName == "Values"
                || args.PropertyName == "X"
                || args.PropertyName == "Y"
                || args.PropertyName == "Z"
                || args.PropertyName == "W";
            
            if( bChangeBackground )
            {
                // 背景色を変更 
                float r, g, b, a; r = g = b = a = 0.0f;

                if (NodeData.Type == "Uniform_Vector3")
                {
                    ShaderGraphData.Uniform_Vector3Node vector3Node = args.Node as ShaderGraphData.Uniform_Vector3Node;
                    r = vector3Node.Values[0];
                    g = vector3Node.Values[1];
                    b = vector3Node.Values[2];
                    a = 1.0f;
                }
                else if (NodeData.Type == "Uniform_Vector4")
                {
                    ShaderGraphData.Uniform_Vector4Node vector4Node = args.Node as ShaderGraphData.Uniform_Vector4Node;
                    r = vector4Node.Values[0];
                    g = vector4Node.Values[1];
                    b = vector4Node.Values[2];
                    a = vector4Node.Values[3];
                }
                SetBackgroundColor(r, g, b, a);      
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// 背景色を設定する
        /// </summary>        
        private void SetBackgroundColor(float r, float g, float b, float a)
        {
            Color color = new Color();
            color.R = (Byte)(r * 255);
            color.G = (Byte)(g * 255);
            color.B = (Byte)(b * 255);
            color.A = (Byte)(a * 255);
            base.Background = new SolidColorBrush(color);
        }
        #endregion
    }
}
