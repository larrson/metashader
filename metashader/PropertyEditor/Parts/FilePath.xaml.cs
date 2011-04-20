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
using Microsoft.Win32;

namespace metashader.PropertyEditor.Parts
{
    /// <summary>
    /// FilePath.xaml の相互作用ロジック
    /// </summary>
    public partial class FilePath : UserControl, IPartsControl
    {
        public FilePath()
        {
            InitializeComponent();

            // バインディングの初期化
            InitializeBinding();          

            // イベント登録
            _pathTextBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(_pathTextBox_TextDecided);
            _openButton.Click += new RoutedEventHandler(_openButton_Click);
        }

#region properties
        /// <summary>
        /// ファイルパス
        /// </summary>
        private string Path
        {
            set 
            {              
                if (IsValidPath(value))
                {
                    // 新しいパスを設定する
                    ValueController<string> valueController = this.DataContext as ValueController<string>;
                    if( valueController.Value != value )
                        valueController.Value = value;
                }
                else
                {
                    // 元の表示に戻す
                    UpdateTarget();
                }
            }
        }
#endregion

#region event handlers
        /// <summary>
        /// テキストが設定された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _pathTextBox_TextDecided(object sender, EventArgs e)
        {
            // パスを設定する
            Path = (sender as TextBox).Text;     
        }       

        /// <summary>
        /// ファイルを開くボタンが押された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _openButton_Click(object sender, RoutedEventArgs e)
        {
            // ファイル選択ダイアログを開く
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All files (*.*)|*.*";

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                // パスを変更
                Path = dlg.FileName;                
            }
        }        
#endregion

        #region override methods
        /// <summary>
        /// ターゲットプロパティを強制的に更新する
        /// </summary>
        public void UpdateTarget()
        {
            // バインディングを有効に
            InitializeBinding();

            BindingExpression bindingExpression = _pathTextBox.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
        }
        #endregion

#region private methods
        /// <summary>
        /// 有効なパスか判定する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsValidPath(string path)
        {
            return System.IO.File.Exists(path);
        }

        /// <summary>
        /// バインディングの初期化
        /// </summary>
        private void InitializeBinding()
        {
            if (_pathTextBox.GetBindingExpression(TextBox.TextProperty) == null)
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.OneWay
                };
                _pathTextBox.SetBinding(TextBox.TextProperty, binding);
            }
        }
#endregion
    }
}
