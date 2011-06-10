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
    /// StringTextbox.xaml の相互作用ロジック
    /// </summary>
    public partial class StringTextbox : UserControl, IPartsControl
    {
#region constructors
        public StringTextbox()
            : base()
        {
            InitializeComponent();

            // バインディングの初期化
            InitializeBinding();

            // イベントハンドラの登録
            _textBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(TextBox_TextDecided);
        }
#endregion        

#region event handlers
        /// <summary>
        /// テキストボックスのテキストが決定した
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void TextBox_TextDecided(object sender, EventArgs args)
        {
            TextBox textBox = sender as TextBox;

            // 決定した値を設定
            ValueController<string> valueController = this.DataContext as ValueController<string>;
            valueController.Value = textBox.Text;
        }
#endregion

        #region override methods
        /// <summary>
        /// UI側ターゲットの更新
        /// </summary>
        public void UpdateTarget()
        {
            // バインディングが外れることがあるため
            InitializeBinding();

            _textBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
        #endregion

        #region private methods
        /// <summary>
        /// バインディングを初期化する
        /// </summary>
        void InitializeBinding()
        {
            if (this.GetBindingExpression(TextBox.TextProperty) == null)
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.OneWay
                };
                _textBox.SetBinding(TextBox.TextProperty, binding);
            }
        }
        #endregion
    }
}
