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
using System.Windows.Interop;

namespace metashader.Previewer
{
    /// <summary>
    /// PreviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PreviewWindow : Window
    {
#region variables
        TimeSpan m_lastRenderTime;

        /// <summary>
        /// Win32のフックで使用するウィンドウを閉じるメッセージ
        /// </summary>
        const int WM_CLOSE = 0x0010;
#endregion
        public PreviewWindow()
        {
            InitializeComponent();            
        
            /// イベントハンドラの登録 /// 
            // ウィンドウの描画準備が完了した
            this.Loaded += new RoutedEventHandler(PreviewWindow_Loaded);
            /// ウィンドウが閉じる前            
            this.Closing += new System.ComponentModel.CancelEventHandler(PreviewWindow_Closing);            
            // フロントバッファの有効性が変更された
            _d3dimg.IsFrontBufferAvailableChanged += new DependencyPropertyChangedEventHandler(_d3dimg_IsFrontBufferAvailableChanged);
            // サイズが変更された
            this.SizeChanged += new SizeChangedEventHandler(PreviewWindow_SizeChanged);            
            // シェーダノードの値が変更された
            App.CurrentApp.EventManager.NodePropertyChangedEvent += new metashader.Event.NodePropertyChangedEventHandler(EventManager_NodePropertyChangedEvent);
            // グラフ構成でエラーが発生した
            App.CurrentApp.EventManager.GraphErrorEvent += new metashader.Event.GraphErrorEventHandler(EventManager_GraphErrorEvent);
            // グローバル設定が変更された
            App.CurrentApp.EventManager.GlobalSettingPropertyChangedEvent += new metashader.Event.GlobalSettingPropertyChangedEventHandler(EventManager_GlobalSettingPropertyChangedEvent);
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

            // Win32メッセージの処理を移譲
            WindowInteropHelper winInteropHelper = new WindowInteropHelper(this);
            HwndSource hwndSource = HwndSource.FromHwnd(winInteropHelper.Handle);
            hwndSource.AddHook(WinProc);

            // バックバッファを設定
            _d3dimg.Lock();
            _d3dimg.SetBackBuffer(System.Windows.Interop.D3DResourceType.IDirect3DSurface9
                , NativeMethods.GetBackBuffer());
            _d3dimg.Unlock();     
       
            // 定期的な描画コールバック
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);          
        }

        /// <summary>
        /// ウィンドウが閉じる前に呼ばれるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 閉じさせない
            // @@ 本来なら、閉じるボタンを無効化したウィンドウを用意すべきだが、
            // 標準のままでは作れないので
            e.Cancel = true;
        }        

        /// <summary>
        /// D3DImageのバッファの有効性変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _d3dimg_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {                                                   
            // @@@@
            // 有効から無効へ変化した場合は、デバイスロストをチェックする
            // http://msdn.microsoft.com/ja-jp/library/cc324253.aspx
        }

        /// <summary>
        /// サイズ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PreviewWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            _d3dimg.Lock();
            _d3dimg.SetBackBuffer(D3DResourceType.IDirect3DSurface9, IntPtr.Zero);
            _d3dimg.Unlock();
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
                // D3DImageの更新
                _d3dimg.Lock();
                NativeMethods.Render();
                _d3dimg.AddDirtyRect(new Int32Rect(0, 0, _d3dimg.PixelWidth, _d3dimg.PixelHeight));
                _d3dimg.Unlock();
                
                // 最後にレンダリングされた時刻を記録
                m_lastRenderTime = args.RenderingTime;             
                 
            }
        }
        
        /// <summary>
        /// メッセージ処理
        /// メッセージを処理した場合は、handledにtrueを設定する
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {  
            IntPtr ret = IntPtr.Zero;

            // Closingは別途処理するので、ここでは呼ばれないようにする
            if( msg != WM_CLOSE )
            {
                ret =  NativeMethods.WndProc(hwnd, msg, wParam, lParam);
                handled = true;
            }             
            else
            {
                handled = false;
            }            

            return ret;
        }

        /// <summary>
        /// シェーダノードのプロパティ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodePropertyChangedEvent(object sender, metashader.Event.NodePropertyChangedEventArgs args)
        {
            // プロパティ変更イベント
            //@@ ApplyParameterでは処理が重すぎる場合は、プロパティ名ごとに場合分けする
            ShaderGraphData.IAppliableParameter appliableParam = args.Node as ShaderGraphData.IAppliableParameter;
            if( appliableParam != null )
            {
                // 変更されたプロパティに関わらずパラメータを適用
                appliableParam.ApplyParameter();
            }
        }

        /// <summary>
        /// グラフ構成にエラーが発生した
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_GraphErrorEvent(object sender, metashader.Event.GraphErrorEventArgs args)
        {
            // 自動実行が有効なら、成功の場合シェーダを実行する
            if( Properties.Settings.Default.AutoExecution && args.Type == metashader.Event.GraphErrorType.NoError )
            {
                App.CurrentApp.UICommandManager.ExecuteShaderCommand.Execute(null);
            }
            else
            {
                // エラーに関わらず、デフォルトへ切り替え
                NativeMethods.UseDefaultShader();            
            }            
        }

        /// <summary>
        /// グローバル設定が変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_GlobalSettingPropertyChangedEvent(object sender, metashader.Event.GlobalSettingPropertyChangedEventArgs args)
        {
            // プロパティごとに行う処理を変更
            switch( args.PropertyName )
            {
                case "BlendMode":
                    NativeMethods.SetBlendMode((int)args.NewValue);
                    break;
                default:
                    break;
            }
        }
#endregion        
    }
}
