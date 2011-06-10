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

namespace metashader.GraphEditor.Thumnail
{   
    /// <summary>
    /// ThumnailControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ThumnailControl : UserControl
    {
#region variables
        /// <summary>
        /// シェーダノードのデータ
        /// </summary>
        private ShaderGraphData.ShaderNodeDataBase m_node;
#endregion

        public ThumnailControl(ShaderGraphData.ShaderNodeDataBase node)
        {
            InitializeComponent();

            // メンバ変数初期化
            m_node = node;

            // イベントハンドラの登録
            this.MouseDoubleClick += new MouseButtonEventHandler(ThumnailControl_MouseDoubleClick);
        }

#region properties
        /// <summary>
        /// シェーダノードのデータ
        /// </summary>
        protected ShaderGraphData.ShaderNodeDataBase NodeData
        {
            get { return m_node; }
        }
#endregion

#region event handlers
        /// <summary>
        /// ダブルクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ThumnailControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // 処理を派生クラスに移譲
            OnMouseDoubleClick(sender, e);
        }
#endregion

#region protected methods
        /// <summary>
        /// ダブルクリックイベントを処理するメソッド        
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベント引数</param>
        protected virtual void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// ノードのプロパティ変更時に親コントロールから呼ばれる処理
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="args">イベント引数</param>
        public virtual void OnNodePropertyChanged(object sender, Event.NodePropertyChangedEventArgs args)
        {

        }
#endregion        
    }

    /// <summary>
    /// デフォルトのサムネイルクラス
    /// </summary>
    public class DefaultThumnail : ThumnailControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node"></param>
        public DefaultThumnail(ShaderGraphData.ShaderNodeDataBase node)
            : base(node)
        {

        }
    }
}
