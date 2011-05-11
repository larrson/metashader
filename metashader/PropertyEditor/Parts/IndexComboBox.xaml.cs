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
    /// IndexComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class IndexComboBox : UserControl, IPartsControl
    {
        public IndexComboBox(uint maximumIndex)
        {
            InitializeComponent();

            // コントロールの初期化
            // maximumIndexまでのインデックスをコンボボックスへ追加
            for (uint i = 0; i <= maximumIndex; ++i )
            {
                _comboBox.Items.Add(
                        new ComboBoxItem(){ Content=i }
                    );
            }

            // バインディングの初期化
            InitializeBinding();

            // イベント登録
            _comboBox.SelectionChanged += new SelectionChangedEventHandler(_comboBox_SelectionChanged);
        }

        #region override methods
        /// <summary>
        /// ターゲットプロパティを強制的に更新する
        /// </summary>
        public void UpdateTarget()
        {
            // バインディングが外れることがあるため再設定
            InitializeBinding();

            BindingExpression bindingExpression = _comboBox.GetBindingExpression(ComboBox.SelectedIndexProperty);
            if (bindingExpression != null)
            {
                bindingExpression.UpdateTarget();
            }
        }
        #endregion

#region event handlers
        /// <summary>
        /// コンボボックスのインデックスが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 新たに選択されたBlendModeを設定する
            ComboBox comboBox = sender as ComboBox;

            uint index = (uint)comboBox.SelectedIndex;

            ValueController<uint> valueController = this.DataContext as ValueController<uint>;
            if (valueController.Value != index)
                valueController.Value = index;
        }
#endregion

        #region private methods
        /// <summary>
        /// バインディングの初期化
        /// </summary>
        private void InitializeBinding()
        {
            BindingExpression bindingExpression = _comboBox.GetBindingExpression(ComboBox.SelectedIndexProperty);
            if (bindingExpression == null)
            {
                Binding binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Mode = BindingMode.OneWay                    
                };
                _comboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);
            }
        }
        #endregion
    }
}
