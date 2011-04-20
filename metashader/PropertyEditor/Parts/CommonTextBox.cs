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


namespace metashader.PropertyEditor.Parts
{
    /// <summary>
    /// 共用するテキストボックス
    /// 通常のテキストボックスとは異なり、「フォーカスのロスト」と「リターンキーの押下」を
    /// 「決定」イベントとして合わせて扱えるようにする
    /// </summary>
    public class CommonTextBox : TextBox
    {
#region variables
        public delegate void TextDecidedEventHandler(object sender, EventArgs args);

        /// <summary>
        /// テキスト決定イベント
        /// </summary>
        public event TextDecidedEventHandler TextDecided;
#endregion

#region constructors
        public CommonTextBox()
            : base()
        {
            // イベントハンドラの登録
            this.LostFocus += new RoutedEventHandler(CommonTextBox_LostFocus);
            this.KeyDown += new KeyEventHandler(CommonTextBox_KeyDown);
        }
#endregion

#region event handlers
        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommonTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Returnが押されたら決定されたとみなす
            if (e.Key == Key.Return)
            {
                if( TextDecided != null )
                    TextDecided(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// フォーカスロストイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommonTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if( TextDecided != null )
                TextDecided(this, EventArgs.Empty);
        }
#endregion        
    }
}
