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
    /// BlendModeのEnumのコンバータ
    /// </summary>
    public class BlendModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Setting.BlendMode source = (Setting.BlendMode)value;
            return (int)source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotFiniteNumberException();
        }
    }

    /// <summary>
    /// BlendModeComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class BlendModeComboBox : UserControl, IPartsControl
    {
        public BlendModeComboBox()
        {
            InitializeComponent();

            // バインディングの初期化
            InitializeBinding();

            // イベントハンドラの登録
            _blendModeComboBox.SelectionChanged += new SelectionChangedEventHandler(_blendModeComboBox_SelectionChanged);
        }

        #region event handlers
        /// <summary>
        /// ブレンドモードイベントハンドラが選択された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _blendModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 新たに選択されたBlendModeを設定する
            ComboBox comboBox = sender as ComboBox;            

            Setting.BlendMode blendMode = (Setting.BlendMode)comboBox.SelectedIndex;

            ValueController<Setting.BlendMode> valueController = this.DataContext as ValueController<Setting.BlendMode>;
            if (valueController.Value != blendMode)
                valueController.Value = blendMode;
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

            BindingExpression bindingExpression = _blendModeComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty);
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
            BindingExpression bindingExpression = _blendModeComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty);
            if( bindingExpression == null )
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.OneWay,
                    Converter = new BlendModeConverter()
                };
                _blendModeComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);
            }            
        }
#endregion
    }
}
