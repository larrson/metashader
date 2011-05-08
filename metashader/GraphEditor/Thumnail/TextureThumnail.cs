using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using metashader.Common;

namespace metashader.GraphEditor.Thumnail
{
    public class TextureThumnail : ThumnailControl
    {
        #region private methods
        /// <summary>
        /// イメージ
        /// </summary>
        Image m_image;
        #endregion

        public TextureThumnail(ShaderGraphData.ShaderNodeDataBase node)
            : base(node)
        {
            // サムネイル表示用のイメージを作成して追加
            // 正方形のため、高さに合わせる(幅が不定なので)
            m_image = new Image() { Width = base.Height, Height = base.Height };
            base._grid.Children.Add(m_image);

            // データにパスが設定されていれば、サムネイル画像を作成する
            ShaderGraphData.Uniform_TextureNodeBase texNode = node as ShaderGraphData.Uniform_TextureNodeBase;
            if( texNode.Path != null )
            {
                SetPath(texNode.Path);
            }            
        }

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
                case "Path":
                    {
                        // 背景新しいテクスチャに変更                      
                        SetPath(args.NewValue as string);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region protected methods
        /// <summary>
        /// マウスダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // テクスチャファイルの選択

            // ファイル選択ダイアログを開く
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Texture Files (*.DDS;*.BMP;*.JPG;*.PNG)|*.DDS;*.BMP;*.JPG;*.PNG|All files (*.*)|*.*";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // データ側のテクスチャパスを変更
                // Undo/Redoバッファ
                UndoRedoBuffer undoredo = new UndoRedoBuffer();

                // 新しいプロパティを設定                
                App.CurrentApp.GraphData.ChangeNodeProperty<string>(NodeData, "Path", dlg.FileName, undoredo);

                if (undoredo.IsValid)
                {
                    UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }

            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// テクスチャのパスを設定する
        /// 内部でテクスチャファイルのデコードを行いサムネイルを作成する
        /// </summary>
        /// <param name="path"></param>
        private void SetPath(string path)
        {
#if false // BitmapImageクラスを使用したデコード
            BitmapImage bitmapImage = new BitmapImage();
            // イメージのデコード
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new System.Uri(path, UriKind.Absolute);
            bitmapImage.DecodePixelWidth = (int)m_image.Width;
            bitmapImage.DecodePixelHeight = (int)m_image.Height;
            bitmapImage.EndInit();

            // イメージのセット
            m_image.Source = bitmapImage;
#else 
            // プラットフォーム呼び出しによるDirectXAPIを使用したデコード

            int bytePerPixel = 4; // 1ピクセル当たりのバイト数(Brga32フォーマットを作成するため4)            
            int size = bytePerPixel * (int)m_image.Width * (int)m_image.Height;
                        
            // P/Invokeでサムネイルデータを取得            
            // サーフェース格納用にアンマネージメモリを確保する
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            NativeMethods.GetImagePixelData(path, (int)m_image.Width, (int)m_image.Height, bufferPtr);

            // 格納用バッファを作成            
            byte[] pixelData = new byte[size];
            Marshal.Copy(bufferPtr, pixelData, 0, size);
                        
            // バッファからBitmapSourceを作成
            m_image.Source = BitmapSource.Create((int)m_image.Width, (int)m_image.Height, 96, 96, PixelFormats.Bgra32, null, pixelData, (int)m_image.Width * bytePerPixel);

            // アンマネージメモリを解放
            Marshal.FreeHGlobal(bufferPtr);
#endif 
        }
        #endregion
    }
}
