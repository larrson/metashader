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

namespace metashader.PropertyEditor
{
    /// <summary>
    /// PropertyStackPanel.xaml の相互作用ロジック
    /// シェーダノードや全体の設定を行うための編集用パネル
    /// 各クラスごとにパネル内のスタックに異なる部品を積んで使用する
    /// </summary>
    public partial class PropertyStackPanel : UserControl
    {
#region variables
        List<Parts.IParts> m_partsList = new List<Parts.IParts>();        
#endregion

        public PropertyStackPanel()
        {
            InitializeComponent();                                                                                
        }        

#region properties
        /// <summary>
        /// パラメータ設定先のオブジェクト
        /// 各子コントロールにBindingされているValueControllerではなく、
        /// ValueControllerが設定するオブジェクト
        /// </summary>
        public object Target
        {            
            get 
            {
                return m_partsList[0].Target;
            }
        }
#endregion

#region public methods
        /// <summary>
        /// 部品をスタックへ追加する
        /// </summary>
        public void AddParts(string labelName, Parts.IParts parts)
        {
            // リストに追加
            m_partsList.Add(parts);

            // 左側のラベル
            TextBlock label = new TextBlock();
            label.Text = labelName;
            label.TextAlignment = TextAlignment.Left;
            
            // 右のコントロールの高さを左へバインドする
            Binding binding = new Binding()
            {
                Source = parts.Control,
                Path = new PropertyPath("ActualHeight"),
                Mode = BindingMode.OneWay
            };
            label.SetBinding(TextBlock.HeightProperty, binding);
            _leftStack.Children.Add(label);

            // 右側のコントロール                        
            parts.Control.BorderBrush = Brushes.White;
            parts.Control.BorderThickness = new System.Windows.Thickness(0, 0, 0, 1);            
            
            _rightStack.Children.Add(parts.Control);            
        }

        /// <summary>
        /// 全てのコントロールのターゲットプロパティを更新する
        /// </summary>
        public void UpdateAllTargets()
        {
            foreach (Parts.IParts parts in m_partsList)
            {               
                // UserControlの表示を更新
                (parts.Control as Parts.IPartsControl).UpdateTarget();
            }   
        }
#endregion
    }
}
