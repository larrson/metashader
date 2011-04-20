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
    /// 変更不可の文字列タグクラス
    /// StringTag.xaml の相互作用ロジック
    /// </summary>
    public partial class StringTag : UserControl, IPartsControl
    {
#region variables                
#endregion
        /// <summary>
        /// コンストラクタ
        /// </summary>        
        /// <param name="propertyName"></param>
        public StringTag()            
        {
            InitializeComponent();

            _textBlock.SetBinding(TextBlock.TextProperty, new Binding("Value"));            
        }       

#region properties
        
#endregion

#region override methods
        /// <summary>
        /// ターゲットプロパティを強制的に更新する
        /// </summary>
        public void UpdateTarget()
        {
            BindingExpression bindingExpression = _textBlock.GetBindingExpression(TextBlock.TextProperty);
            if( bindingExpression != null )
            {
                bindingExpression.UpdateTarget();
            }
        }
#endregion
    }
}
