using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace metashader.PropertyEditor.Parts
{
    /// <summary>
    /// Enumのコンバータ
    /// </summary>
    public class EnumConverter<T> : IValueConverter
        where T : IConvertible
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {            
            IConvertible source = (T)value as IConvertible;
            return (int)(source);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (T)(value);
        }
    }


    /// <summary>
    /// Enum内の1要素の選択に特化したコンボボックス   
    /// </summary>
    /// <typeparam name="T">対象のEnum型。Enum内の最後に最大数を表すMaxが定義されている事を前提としている</typeparam>
    class EnumComboBox<T> : ComboBox, IPartsControl 
        where T : IConvertible
    {        
#region constructors
        public EnumComboBox()
            : base()
        {
            // コンボボックスの要素を初期化            
            List<T> list = new List<T>( Enum.GetValues(typeof(T)) as IEnumerable<T> );
            for(int i = 0; i < list.Count - 1 ; ++i ) // -1はEnum内のMax定義
            {
                this.AddChild(new ComboBoxItem() { Content = list[i] });
            }                        
            
            // バインディング設定
            InitializeBinding();
        }
#endregion

#region override methods
        /// <summary>
        /// ターゲットプロパティを強制的に更新する
        /// </summary>
        public void UpdateTarget()
        {
            // バインディングが外れることがあるため再設定
            InitializeBinding();

            BindingExpression bindingExpression = this.GetBindingExpression(ComboBox.SelectedIndexProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
        }
#endregion

#region private methods
        /// <summary>
        /// バインディングの初期化
        /// </summary>
        private void InitializeBinding()
        {
            BindingExpression bindingExpression = this.GetBindingExpression(ComboBox.SelectedIndexProperty);
            if (bindingExpression == null)
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.TwoWay,                    
                    Converter = new EnumConverter<T>(),
                };
                this.SetBinding(ComboBox.SelectedIndexProperty, binding);
            }          
        }
#endregion
    }     
}
