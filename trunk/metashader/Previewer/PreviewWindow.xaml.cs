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
using System.Windows.Shapes;

namespace metashader.Previewer
{
    /// <summary>
    /// PreviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PreviewWindow : Window
    {
#region variables
        TimeSpan m_lastRenderTime;
#endregion
        public PreviewWindow()
        {
            InitializeComponent();            
        
            /// イベントハンドラの登録 /// 
            // ウィンドウの描画準備が完了した
            this.Loaded += new RoutedEventHandler(PreviewWindow_Loaded);
            // フロントバッファの有効性が変更された
            _d3dimg.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(_d3dimg_IsFrontBufferAvailableChanged);
            // サイズが変更された
            this.SizeChanged += new SizeChangedEventHandler(PreviewWindow_SizeChanged);
            // 定期的な描画コールバック
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }        
        

#region event handlers
        /// <summary>
        /// 描画準備完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Previewerのエントリポイントを呼び出し
            NativeMethods.PreviewerMain((int)this.Width, (int)this.Height);
        }

        /// <summary>
        /// D3DImageのバッファの有効性変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _d3dimg_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // 有効から無効へ変化した場合は、デバイスロストをチェックする
            if( (bool)e.NewValue == false && (bool)e.OldValue == true )
            {
                // @@@@
                // http://msdn.microsoft.com/ja-jp/library/cc324253.aspx
            }
        }

        /// <summary>
        /// サイズ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            NativeMethods.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
        }

        /// <summary>
        /// 定期的な描画コールバック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs args = (RenderingEventArgs)e;

            // 同じフレームで2回呼ばれる可能性があるため、
            // 2回目のレンダリングを避ける
            if( _d3dimg.IsFrontBufferAvailable && m_lastRenderTime != args.RenderingTime )
            {
                IntPtr pSurface = NativeMethods.GetNextSurface();
                if( pSurface != IntPtr.Zero )
                {
                    _d3dimg.Lock();
                    _d3dimg.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9, pSurface);
                    _d3dimg.AddDirtyRect(new Int32Rect(0, 0, _d3dimg.PixelWidth, _d3dimg.PixelHeight));
                    _d3dimg.Unlock();

                    m_lastRenderTime = args.RenderingTime;
                }
            }
        }
        
#endregion        
    }
}
