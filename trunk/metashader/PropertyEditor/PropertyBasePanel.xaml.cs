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
    /// PropertyBasePanel.xaml の相互作用ロジック
    /// プロパティエディタを構成する背景パネル
    /// この上に、各種のノードに適したパネルを表示する
    /// </summary>
    public partial class PropertyBasePanel : UserControl
    {
#region variables
        PropertyStackPanel m_globalSettingPanel;

        PropertyStackPanel[] m_nodePanels;
#endregion

        public PropertyBasePanel()
        {
            InitializeComponent();

            // メンバ変数の初期化
            m_globalSettingPanel = new PropertyStackPanel();
            m_nodePanels = new PropertyStackPanel[(int)ShaderGraphData.ShaderNodeType.Max];
            for(int i = 0; i < (int)ShaderGraphData.ShaderNodeType.Max; ++i)
            {
                m_nodePanels[i] = CreateNodePropertyPanel((metashader.ShaderGraphData.ShaderNodeType)i);
            }
            _scrollViewer.Content = m_globalSettingPanel;

            // イベント登録
            App.CurrentApp.SelectManager.SelectionChanged += new metashader.ShaderGraphData.SelectManager.SelectionChangedEventHandler(SelectManager_SelectionChanged);
            App.CurrentApp.EventManager.NodePropertyChangedEvent += new metashader.Event.NodePropertyChangedEventHandler(EventManager_NodePropertyChangedEvent);
        }

#region event handlers
        /// <summary>
        /// データ構造の選択変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void SelectManager_SelectionChanged(object sender, metashader.ShaderGraphData.SelectManager.SelectionChangedEventArgs args)
        {
            if( args.SelectedNodes.Length == 1 )
            {
                ShaderGraphData.ShaderNodeDataBase node = args.SelectedNodes[0];

                PropertyStackPanel panel = m_nodePanels[(int)node.Type];                
                _scrollViewer.Content = panel;
                panel.NodeData = node;                
            }
            // ノード以外の選択
            else
            {
                _scrollViewer.Content = m_globalSettingPanel;
            }
        }

        /// <summary>
        /// シェーダノードのプロパティが変更された際のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodePropertyChangedEvent(object sender, metashader.Event.NodePropertyChangedEventArgs args)
        {
            ShaderGraphData.ShaderNodeDataBase node = args.Node;

            // 現在表示されているパネルを取得
            PropertyStackPanel panel = _scrollViewer.Content as PropertyStackPanel;

            // 表示中のノードと変更されたノードが等しければ、表示を更新する
            if( ReferenceEquals(panel.NodeData, node) )
            {
                panel.UpdateAllTargets();
            }
        }        
#endregion

#region private methods
        /// <summary>
        /// シェーダーノードのタイプに応じたプロパティパネルを作成する
        /// </summary>
        /// <param name="type"></param>
        private PropertyStackPanel CreateNodePropertyPanel(ShaderGraphData.ShaderNodeType type)
        {
            PropertyStackPanel panel = new PropertyStackPanel();

            // ノードの種類ごとにスタックへUIを積む

            // Float値用
            if( type == metashader.ShaderGraphData.ShaderNodeType.Uniform_Float )
            {
                panel.AddParts("値", new Parts.Parts<float>("Value", new Parts.FloatTextBox()));
            }
            // Vector4用
            else if (type == metashader.ShaderGraphData.ShaderNodeType.Uniform_Vector4)
            {
                panel.AddParts("X", new Parts.Parts<float>("X", new Parts.FloatTextBox()));
                panel.AddParts("Y", new Parts.Parts<float>("Y", new Parts.FloatTextBox()));
                panel.AddParts("Z", new Parts.Parts<float>("Z", new Parts.FloatTextBox()));
                panel.AddParts("W", new Parts.Parts<float>("W", new Parts.FloatTextBox()));
            }            
            // テクスチャ用
            else if (type == ShaderGraphData.ShaderNodeType.Uniform_Texture2D
                        || type == ShaderGraphData.ShaderNodeType.Uniform_TextureCube)
            {
                panel.AddParts("テクスチャファイル", new Parts.Parts<string>("Path", new Parts.FilePath()));
                panel.AddParts("サンプラーステート", new Parts.Parts<ShaderGraphData.SamplerState>("TextureSamplerState", new Parts.SamplerState()));
            }                                                    

            return panel;
        }
#endregion 
    }
}
