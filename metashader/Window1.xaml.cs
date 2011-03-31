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
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
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
    }
}
