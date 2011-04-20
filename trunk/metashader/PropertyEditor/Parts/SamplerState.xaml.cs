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
    public class WrapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ShaderGraphData.Uniform_Texture2DNode.WrapMode source = (ShaderGraphData.Uniform_Texture2DNode.WrapMode)value;
            return (int)source;            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotFiniteNumberException();
        }
    }

    public class FilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ShaderGraphData.Uniform_Texture2DNode.FilterMode source = (ShaderGraphData.Uniform_Texture2DNode.FilterMode)value;
            return (int)source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotFiniteNumberException();
        }
    }

    /// <summary>
    /// SamplerState.xaml の相互作用ロジック
    /// </summary>
    public partial class SamplerState : UserControl, IPartsControl
    {
        public SamplerState()
        {
            InitializeComponent();


            InitializeBinding();
             
            
            // イベントハンドラの登録
            _wrapUComboBox.SelectionChanged += new SelectionChangedEventHandler(_wrapComboBox_SelectionChanged);
            _wrapVComboBox.SelectionChanged += new SelectionChangedEventHandler(_wrapComboBox_SelectionChanged);
            _wrapWComboBox.SelectionChanged += new SelectionChangedEventHandler(_wrapComboBox_SelectionChanged);

            _magFilterComboBox.SelectionChanged += new SelectionChangedEventHandler(_filterComboBox_SelectionChanged);
            _minFilterComboBox.SelectionChanged += new SelectionChangedEventHandler(_filterComboBox_SelectionChanged);
            _mipFilterComboBox.SelectionChanged += new SelectionChangedEventHandler(_filterComboBox_SelectionChanged);

            _maxAnisotoropyTextBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(_maxAnisotoropyTextBox_TextDecided);

            _borderColorRTextBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(_borderColorTextBox_TextDecided);
            _borderColorGTextBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(_borderColorTextBox_TextDecided);
            _borderColorBTextBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(_borderColorTextBox_TextDecided);
            _borderColorATextBox.TextDecided += new CommonTextBox.TextDecidedEventHandler(_borderColorTextBox_TextDecided);            
        }                      

        private void InitializeBinding()
        {
            // WrapU
            Binding binding = new Binding()
            {
                Path = new PropertyPath("Value.WrapU"),
                Mode = BindingMode.OneWay,
                Converter = new WrapConverter()
            };
            _wrapUComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

            // WrapV
            binding = new Binding()
            {
                Path = new PropertyPath("Value.WrapV"),
                Mode = BindingMode.OneWay,
                Converter = new WrapConverter()
            };
            _wrapVComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

            // WrapW
            binding = new Binding()
            {
                Path = new PropertyPath("Value.WrapW"),
                Mode = BindingMode.OneWay,
                Converter = new WrapConverter()
            };
            _wrapWComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

            // Mag Filter
            binding = new Binding()
            {
                Path = new PropertyPath("Value.MagFilter"),
                Mode = BindingMode.OneWay,
                Converter = new FilterConverter()
            };
            _magFilterComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

            // Min Filter
            binding = new Binding()
            {
                Path = new PropertyPath("Value.MinFilter"),
                Mode = BindingMode.OneWay,
                Converter = new FilterConverter()
            };
            _minFilterComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);

            // Mip Filter
            binding = new Binding()
            {
                Path = new PropertyPath("Value.MipFilter"),
                Mode = BindingMode.OneWay,
                Converter = new FilterConverter()
            };
            _mipFilterComboBox.SetBinding(ComboBox.SelectedIndexProperty, binding);
            
            // Max Anisotropy
            if( _maxAnisotoropyTextBox.GetBindingExpression(TextBox.TextProperty) == null )
            {
                binding = new Binding()
                {
                    Path = new PropertyPath("Value.MaxAnisotoropy"),
                    Mode = BindingMode.OneWay                    
                };
                _maxAnisotoropyTextBox.SetBinding(TextBox.TextProperty, binding);
            }

            // Border Color R
            if (_borderColorRTextBox.GetBindingExpression(TextBox.TextProperty) == null)
            {
                binding = new Binding()
                {
                    Path = new PropertyPath("Value.BorderColorR"),
                    Mode = BindingMode.OneWay
                };
                _borderColorRTextBox.SetBinding(TextBox.TextProperty, binding);
            }

            // Border Color G
            if (_borderColorGTextBox.GetBindingExpression(TextBox.TextProperty) == null)
            {
                binding = new Binding()
                {
                    Path = new PropertyPath("Value.BorderColorG"),
                    Mode = BindingMode.OneWay
                };
                _borderColorGTextBox.SetBinding(TextBox.TextProperty, binding);
            }

            // Border Color B
            if (_borderColorBTextBox.GetBindingExpression(TextBox.TextProperty) == null)
            {
                binding = new Binding()
                {
                    Path = new PropertyPath("Value.BorderColorB"),
                    Mode = BindingMode.OneWay
                };
                _borderColorBTextBox.SetBinding(TextBox.TextProperty, binding);
            }

            // Border Color A
            if (_borderColorATextBox.GetBindingExpression(TextBox.TextProperty) == null)
            {
                binding = new Binding()
                {
                    Path = new PropertyPath("Value.BorderColorA"),
                    Mode = BindingMode.OneWay
                };
                _borderColorATextBox.SetBinding(TextBox.TextProperty, binding);
            }
        }        

        #region override methods
        /// <summary>
        /// ターゲットプロパティを強制的に更新する
        /// </summary>
        public void UpdateTarget()
        {
            // 何故かバインディングが外れるので。。。
            InitializeBinding();

            _wrapUComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateTarget();
            _wrapVComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateTarget();
            _wrapWComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateTarget();
            _magFilterComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateTarget();
            _minFilterComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateTarget();
            _mipFilterComboBox.GetBindingExpression(ComboBox.SelectedIndexProperty).UpdateTarget();
            _maxAnisotoropyTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            _borderColorRTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            _borderColorGTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            _borderColorBTextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            _borderColorATextBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
        }
        #endregion

#region event handlers
        /// <summary>
        /// Wrap系コンボボックスが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _wrapComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState> valueController = this.DataContext as ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState>;
            ShaderGraphData.Uniform_Texture2DNode.SamplerState value = valueController.Value;

            ComboBox comboBox = sender as ComboBox;
            
            // WrapU
            if( ReferenceEquals(comboBox, _wrapUComboBox ) )
            {
                value.WrapU = (ShaderGraphData.Uniform_Texture2DNode.WrapMode)comboBox.SelectedIndex;
            }
            // WrapV
            else if( ReferenceEquals(comboBox, _wrapVComboBox ) )
            {
                value.WrapV = (ShaderGraphData.Uniform_Texture2DNode.WrapMode)comboBox.SelectedIndex;
            }
            // WrapW
            else if( ReferenceEquals(comboBox, _wrapWComboBox ) )
            {
                value.WrapW = (ShaderGraphData.Uniform_Texture2DNode.WrapMode)comboBox.SelectedIndex;
            }

            // 変更後の値をセット
            valueController.Value = value;
        }

        /// <summary>
        /// Filter系コンボボックスが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _filterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState> valueController = this.DataContext as ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState>;
            ShaderGraphData.Uniform_Texture2DNode.SamplerState value = valueController.Value;

            ComboBox comboBox = sender as ComboBox;

            // MagFilter
            if (ReferenceEquals(comboBox, _magFilterComboBox))
            {
                value.MagFilter = (ShaderGraphData.Uniform_Texture2DNode.FilterMode)comboBox.SelectedIndex;
            }
            // MinFilter
            else if (ReferenceEquals(comboBox, _minFilterComboBox))
            {
                value.MinFilter = (ShaderGraphData.Uniform_Texture2DNode.FilterMode)comboBox.SelectedIndex;
            }
            // MipFilter
            else if (ReferenceEquals(comboBox, _mipFilterComboBox))
            {
                value.MipFilter = (ShaderGraphData.Uniform_Texture2DNode.FilterMode)comboBox.SelectedIndex;
            }

            // 変更後の値をセット
            valueController.Value = value;
        }

        /// <summary>
        /// 異方性の最大値テキストボックスが決定した
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void _maxAnisotoropyTextBox_TextDecided(object sender, EventArgs args)
        {
            TextBox textBox = sender as TextBox;
            ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState> valueController = this.DataContext as ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState>;
            ShaderGraphData.Uniform_Texture2DNode.SamplerState value = valueController.Value;

            try
            {
                uint maxAnisotoropy = uint.Parse(textBox.Text);   
                value.MaxAnisotoropy = maxAnisotoropy;
                valueController.Value = value;
            }            
            catch(Exception /*e*/)
            {
                UpdateTarget();
            }
        }

        /// <summary>
        /// 境界色のテキストボックスのテキストが決定した
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void _borderColorTextBox_TextDecided(object sender, EventArgs args)
        {
            TextBox textBox = sender as TextBox;
            ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState> valueController = this.DataContext as ValueController<ShaderGraphData.Uniform_Texture2DNode.SamplerState>;
            ShaderGraphData.Uniform_Texture2DNode.SamplerState value = valueController.Value;

            try
            {
                float borderColor = float.Parse(textBox.Text);

                // R
                if( ReferenceEquals(textBox, _borderColorRTextBox) )
                {
                    value.BorderColorR = borderColor;
                }
                // G
                else if( ReferenceEquals(textBox, _borderColorGTextBox) )
                {
                    value.BorderColorG = borderColor;
                }
                // B
                else if (ReferenceEquals(textBox, _borderColorBTextBox))
                {
                    value.BorderColorB = borderColor;
                }
                // A
                else if (ReferenceEquals(textBox, _borderColorATextBox))
                {
                    value.BorderColorA = borderColor;
                }

                valueController.Value = value;
            }
            catch (System.Exception /*ex*/)
            {
                UpdateTarget();
            }
        }  
#endregion
    }
}
