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

namespace metashader.Console
{
    /// <summary>
    /// ConsoleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConsoleWindow : Window
    {
#region variables
        /// <summary>
        /// 文字列コマンドを解釈、実行するインタプリタ
        /// </summary>
        CommandInterpreter m_interpreter;
#endregion

        public ConsoleWindow()
        {
            InitializeComponent();

            // イベント登録
            _ritchTextBox.PreviewKeyDown += new KeyEventHandler(ritchTextBox_KeyDown);            
            _ritchTextBox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(_ritchTextBox_PreviewMouseLeftButtonDown);
            
            // インタプリタ作成
            m_interpreter = new CommandInterpreter();
        }

        /// <summary>
        /// マウスクリックイベントのハンドラ
        /// マウスクリックで行および列の移動を行わないように制限するため、あえてトンネル式      
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _ritchTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // フォーカスを設定するだけでなにもしない
            _ritchTextBox.Focus();

            // マウス操作の無効化           
            e.Handled = true;
        }        

        /// <summary>
        /// リッチテキストボックスのキーダウンイベント
        /// 「Returnキー」のキーダウンイベントを取るため、あえてトンネル式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ritchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // リターンキーが押されたら、直前のブロック（＝パラグラフ）のテキストを取得し、
            // コマンドインタプリタへ処理を移譲する
            if( e.Key == Key.Return )
            {
                FlowDocument document = _ritchTextBox.Document;
                TextRange range = new TextRange(document.Blocks.LastBlock.ContentStart, document.Blocks.LastBlock.ContentEnd);
#if DEBUG
                System.Console.WriteLine(range.Text);
#endif // DEBUG

                // インタプリタへ処理を移譲
                m_interpreter.Interpret(range.Text);
            }
        }
    }
}
