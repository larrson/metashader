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
using metashader.Common;

namespace metashader.PropertyEditor
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
    /// GlobalSettingsPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class GlobalSettingsPanel : UserControl
    {
        public GlobalSettingsPanel()
        {
            InitializeComponent();

            // DataContextを設定
            this.DataContext = App.CurrentApp.GlobalSettings;

            // バインディングの初期化
            // BlendMode
            Binding binding = new Binding()
            {
                Path = new PropertyPath("BlendMode"),
                Mode = BindingMode.OneWay,
                Converter = new BlendModeConverter()
            };
            _blendModeComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

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
            ComboBox comboBox = sender as ComboBox;

            Setting.BlendMode blendMode = (Setting.BlendMode)comboBox.SelectedIndex;

            // GlobalSettingsのBlendModeプロパティを変更
            UndoRedoBuffer undoredo = new UndoRedoBuffer();

            App.CurrentApp.GlobalSettings.ChangeProperty<Setting.BlendMode>("BlendMode", blendMode, undoredo);

            if( undoredo.IsValid )
            {
                UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
            }
        }
#endregion        
    }
}
