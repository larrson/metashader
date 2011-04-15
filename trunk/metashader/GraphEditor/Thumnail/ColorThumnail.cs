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

            // 背景色変更
            ShaderGraphData.Uniform_Vector4Node vector4Node = NodeData as ShaderGraphData.Uniform_Vector4Node;
            SetBackgroundColor(vector4Node.Values);            
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
                ShaderGraphData.UndoRedoBuffer undoredo = new ShaderGraphData.UndoRedoBuffer();

                // ベクトルの4成分を取得
                float[] values = new float[4];
                values[0] = dialog.Color.R / 255f;
                values[1] = dialog.Color.G / 255f;
                values[2] = dialog.Color.B / 255f;
                values[3] = dialog.Color.A / 255f;

                App.CurrentApp.GraphData.ChangeNodeProperty<float[]>(NodeData, "Values", values, undoredo);

                if (undoredo.IsValid)
                {
                    ShaderGraphData.UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
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
            switch (args.PropertyName)
            {
                case "Values":
                    {
                        // 背景色を変更
                        float[] values = args.NewValue as float[];
                        SetBackgroundColor(values);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// 背景色を設定する
        /// </summary>
        /// <param name="values"></param>
        private void SetBackgroundColor(float[] values)
        {
            Color color = new Color();
            color.R = (Byte)(values[0] * 255);
            color.G = (Byte)(values[1] * 255);
            color.B = (Byte)(values[2] * 255);
            color.A = (Byte)(values[3] * 255);
            base.Background = new SolidColorBrush(color);
        }
        #endregion
    }
}
