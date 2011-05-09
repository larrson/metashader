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
        /// <summary>
        /// グローバル設定の編集用パネル
        /// </summary>
        GlobalSettingsPanel m_globalSettingPanel = new GlobalSettingsPanel();

        /// <summary>
        /// 各ノードの編集用パネル
        /// </summary>
        Dictionary<int,PropertyStackPanel> m_nodePanels = new Dictionary<int,PropertyStackPanel>();
#endregion

        public PropertyBasePanel()
        {
            InitializeComponent();

            // メンバ変数の初期化
            
            // 初期表示をグローバル設定用に指定
            this._scrollViewer.Content = m_globalSettingPanel;

            // イベント登録            
            App.CurrentApp.SelectManager.SelectionChanged += new metashader.ShaderGraphData.SelectManager.SelectionChangedEventHandler(SelectManager_SelectionChanged);
            App.CurrentApp.EventManager.NodeAddedEvent += new metashader.Event.NodeAddedEventHandler(EventManager_NodeAddedEvent);
            App.CurrentApp.EventManager.NodePropertyChangedEvent += new metashader.Event.NodePropertyChangedEventHandler(EventManager_NodePropertyChangedEvent);
            App.CurrentApp.EventManager.NodeDeletedEvent += new metashader.Event.NodeDeletedEventHandler(EventManager_NodeDeletedEvent);
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

                PropertyStackPanel panel = m_nodePanels[node.GetHashCode()];                
                _scrollViewer.Content = panel;                
            }
            // ノード以外の選択
            else
            {
                _scrollViewer.Content = m_globalSettingPanel;
            }
        }

        /// <summary>
        /// シェーダノードが追加された際のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodeAddedEvent(object sender, metashader.Event.NodeAddedEventArgs args)
        {
            // 新しいノードに対応するパネルを追加
            PropertyStackPanel panel = CreateNodePropertyPanel(args.Node.Type);
            panel.NodeData = args.Node;
            m_nodePanels[args.Node.GetHashCode()] = panel;
        }

        /// <summary>
        /// ノードが削除された際のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodeDeletedEvent(object sender, metashader.Event.NodeDeletedEventArgs args)
        {
            // 削除対象が選択されていたら、グローバル設定に表示を切り替える
            PropertyStackPanel panel = _scrollViewer.Content as PropertyStackPanel;                       
                
            if( ReferenceEquals(panel, m_globalSettingPanel) == false
                && panel.NodeData.GetHashCode() == args.NodeHashCode )
            {
                _scrollViewer.Content = m_globalSettingPanel;
            }

            // 既存のノードを削除
            m_nodePanels.Remove(args.NodeHashCode);
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

            if ( ReferenceEquals(panel, m_globalSettingPanel) )
                return;

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

            // 共通項目
            // 題名
            panel._captionTextBlock.Text = "ノード設定";
            // 名前            
            panel.AddParts("ノード名", new Parts.Parts<string>("Name", new Parts.StringTag()));    
            // 説明文
            panel.AddParts("説明", new Parts.Parts<string>("Description", new Parts.StringTextbox()));

            // Float値用
            if( type == metashader.ShaderGraphData.ShaderNodeType.Uniform_Float )
            {
                panel.AddParts("値", new Parts.Parts<float>("Value", new Parts.FloatTextBox()));
            }
            // Vector2用
            else if (type == metashader.ShaderGraphData.ShaderNodeType.Uniform_Vector2)
            {
                panel.AddParts("X", new Parts.Parts<float>("X", new Parts.FloatTextBox()));
                panel.AddParts("Y", new Parts.Parts<float>("Y", new Parts.FloatTextBox()));                
            }            
            // Vector3用
            else if (type == metashader.ShaderGraphData.ShaderNodeType.Uniform_Vector3)
            {
                panel.AddParts("X", new Parts.Parts<float>("X", new Parts.FloatTextBox()));
                panel.AddParts("Y", new Parts.Parts<float>("Y", new Parts.FloatTextBox()));
                panel.AddParts("Z", new Parts.Parts<float>("Z", new Parts.FloatTextBox()));                
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
