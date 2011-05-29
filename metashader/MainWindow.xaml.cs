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

namespace metashader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
#region variables
        /// <summary>
        /// 各ウィンドウのLoadedイベントが呼ばれたかを保持するフラグ
        /// </summary>
        [Flags]
        private enum WindowLoadedFrag
        {
            None       = 0x00,
            MainWindow = 0x01,
            PreviewWindow = 0x02,
            ConsoleWindow = 0x04,
            AllWindow = MainWindow | PreviewWindow | ConsoleWindow,
        }

        /// <summary>
        /// 各ウィンドウのLoadedイベントが呼ばれたかを保持するフラグ
        /// </summary>
        WindowLoadedFrag m_windowLoaded;
#endregion        

        public MainWindow()
        {
            InitializeComponent();

            // 依存するデータの設定
            // コマンドベースなので、コマンドマネージャを渡す
            this.DataContext = App.CurrentApp.UICommandManager;

            // ショートカットキーの設定
            InitializeShortCutKeys();            
        }        
        
#region event handlers
        /// <summary>
        /// UI初期化完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //@@@ サブウィンドウ作成
            Previewer.PreviewWindow previewerWindow = new Previewer.PreviewWindow();
            previewerWindow.Owner = this;
            previewerWindow.Loaded += new RoutedEventHandler(previewerWindow_Loaded);
            previewerWindow.Show();

            // コンソールウィンドウの作成
            Console.ConsoleWindow consoleWindow = new Console.ConsoleWindow();
            consoleWindow.Owner = this;
            consoleWindow.Loaded += new RoutedEventHandler(consoleWindow_Loaded);
            consoleWindow.Show();

            // メインウィンドウのLoadedフラグをON
            SetLoaded(WindowLoadedFrag.MainWindow);
        }
       

        /// <summary>
        /// PreviewWindowのLoadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void previewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // プレビューウィンドウのLoadedフラグをON
            SetLoaded(WindowLoadedFrag.PreviewWindow);
        }

        /// <summary>
        /// ConsoleWindowのLoadedイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void consoleWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // コンソールウィンドウのLoadedフラグをON
            SetLoaded(WindowLoadedFrag.ConsoleWindow);
        }
#endregion      
  
#region private methods
        /// <summary>
        /// ショートカットキーの初期化
        /// </summary>
        private void InitializeShortCutKeys()
        {
            this.InputBindings.Add(
                new KeyBinding(App.CurrentApp.UICommandManager.CreateNewCommand, new KeyGesture(Key.N, ModifierKeys.Control))
            );
            this.InputBindings.Add(
                    new KeyBinding(App.CurrentApp.UICommandManager.LoadCommand, new KeyGesture(Key.O, ModifierKeys.Control))
                );
            this.InputBindings.Add(
                    new KeyBinding(App.CurrentApp.UICommandManager.SaveAsCommand, new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift))
                );
            this.InputBindings.Add(
                    new KeyBinding(App.CurrentApp.UICommandManager.UndoCommand, new KeyGesture(Key.Z, ModifierKeys.Control))
                );
            this.InputBindings.Add(
                    new KeyBinding(App.CurrentApp.UICommandManager.RedoCommand, new KeyGesture(Key.Y, ModifierKeys.Control))
                );
            this.InputBindings.Add(
                new KeyBinding(App.CurrentApp.UICommandManager.DeleteCommand, new KeyGesture(Key.Delete))
            );
            this.InputBindings.Add(
                new KeyBinding(App.CurrentApp.UICommandManager.ExecuteShaderCommand, new KeyGesture(Key.F5))
            );
        }

        /// <summary>
        /// 他のウィンドウが呼ばれたかフラグを設定し、
        /// 全てのウィンドウのLoadedが呼ばれたら、ドキュメントを初期化する
        /// 初期化されたドキュメントをイベント経由でUIへ反映するための措置
        /// </summary>
        /// <param name="flag"></param>
        private void SetLoaded( WindowLoadedFrag flag )
        {
            m_windowLoaded |= flag;

            // 全てのウィンドウのLoadedが呼ばれた
            if( m_windowLoaded == WindowLoadedFrag.AllWindow )
            {
                // ドキュメントを初期化
                App.CurrentApp.CreateNew();

                // フラグをリセット
                m_windowLoaded = WindowLoadedFrag.None;
            }
        }
#endregion
    }
}
