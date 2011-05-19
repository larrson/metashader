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
            previewerWindow.Show();

            // コンソールウィンドウの作成
            Console.ConsoleWindow consoleWindow = new Console.ConsoleWindow();
            consoleWindow.Owner = this;
            consoleWindow.Show();
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
#endregion
    }
}
