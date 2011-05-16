using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// データ選択管理クラス
    /// </summary>
    public class SelectManager
    {
#region member classes
        public class SelectionChangedEventArgs : EventArgs
        {
            /// <summary>
            /// 新たに選択されたノード
            /// </summary>
            ShaderNodeDataBase[] m_selectedNodes;
            /// <summary>
            /// 新たに選択解除されたノード
            /// </summary>
            ShaderNodeDataBase[] m_unselectedNodes;
            /// <summary>
            /// 新たに選択されたリンク
            /// </summary>
            LinkData[] m_selectedLinks;
            /// <summary>
            /// 新たに選択解除されたリンク
            /// </summary>
            LinkData[] m_unselectedLinks;

            public SelectionChangedEventArgs(List<ShaderNodeDataBase> selectedNodes, List<ShaderNodeDataBase> unselectedNodes, List<LinkData> selectedLinks, List<LinkData> unselectedLinks )
            {
                if (selectedNodes != null)
                    m_selectedNodes = selectedNodes.ToArray();
                else
                    m_selectedNodes = new ShaderNodeDataBase[0];

                if (unselectedNodes != null)
                    m_unselectedNodes = unselectedNodes.ToArray();
                else
                    m_unselectedNodes = new ShaderNodeDataBase[0];

                if (selectedLinks != null)
                    m_selectedLinks = selectedLinks.ToArray();
                else
                    m_selectedLinks = new LinkData[0];

                if (unselectedLinks != null)
                    m_unselectedLinks = unselectedLinks.ToArray();
                else
                    m_unselectedLinks = new LinkData[0];
            }

#region properties
            public ShaderNodeDataBase[] SelectedNodes
            {
                get { return m_selectedNodes; }
            }

            public ShaderNodeDataBase[] UnselectedNodes
            {
                get { return m_unselectedNodes; }
            }

            public LinkData[] SelectedLinks
            {
                get { return m_selectedLinks;  }
            }

            public LinkData[] UnselectedLinks
            {
                get { return m_unselectedLinks; }
            }
#endregion

#region public methods
            /// <summary>
            /// 有効なパラメータか
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                bool ret = m_selectedNodes.Length > 0 || m_unselectedNodes.Length > 0 || m_selectedLinks.Length > 0 || m_unselectedLinks.Length > 0;
                return ret;
            }
#endregion            
        }

        /// <summary>
        /// 選択変更デリゲータの宣言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs args);
#endregion

#region variables
        /// <summary>
        /// 選択されているシェーダノードのリスト
        /// </summary>
        List<ShaderNodeDataBase> m_shaderNodes = new List<ShaderNodeDataBase>();

        /// <summary>
        /// 選択されているリンクのリスト
        /// </summary>
        List<LinkData> m_shaderLinks = new List<LinkData>();

        /// <summary>
        /// 選択変更イベント
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;
#endregion

#region properties
        /// <summary>
        /// 選択中のシェーダノードのリスト
        /// </summary>
        public ReadOnlyCollection<ShaderNodeDataBase> SelectedNodeList
        {
            get { return m_shaderNodes.AsReadOnly();  }
        }

        /// <summary>
        /// 選択中のシェーダリンクのリスト
        /// </summary>
        public ReadOnlyCollection<LinkData> SelectedLinkList
        {
            get { return m_shaderLinks.AsReadOnly();  }
        }
#endregion

#region public methods
        /// <summary>
        /// シェーダノードが選択中か
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsSelected( ShaderNodeDataBase node )
        {
            bool ret = false;

            // 選択中リストから探す
            foreach ( ShaderNodeDataBase it in m_shaderNodes )
            {
                if( Object.ReferenceEquals(it, node) )
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }        

        /// <summary>
        /// シェーダリンクが選択中か
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public bool IsSelected( LinkData link )
        {
            bool ret = false;

            // 選択中リストから探す
            foreach( LinkData itr in m_shaderLinks )
            {
                if( itr.CompareTo(link) == 0 )
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// 対象オブジェクトの単一選択        
        /// </summary>
        /// <param name="node"></param>
        public void Select( ShaderNodeDataBase node )
        {         
            List<ShaderNodeDataBase> selectedNodes = new List<ShaderNodeDataBase>();
            List<ShaderNodeDataBase> unselectedNodes = new List<ShaderNodeDataBase>();
            List<LinkData> unselectedLinks = new List<LinkData>();            

            // すでに選択済みの場合
            if( IsSelected(node) )
            {                
                // 他の選択を解除
                // ノードの選択解除
                foreach( ShaderNodeDataBase itr in m_shaderNodes)
                {
                    if( ReferenceEquals(node, itr) == false )
                    {
                        unselectedNodes.Add(itr);
                    }
                }                
            }   
            // 選択されていなかった場合
            else
            {
                // 全ノードの選択を解除
                foreach (ShaderNodeDataBase itr in m_shaderNodes)
                {
                    unselectedNodes.Add(itr);
                }  
                // このノードの選択を追加
                // ただし、出力ノードは選択不可
                if( node.Type.IsOutputNode() == false )
                {
                    selectedNodes.Add(node);                
                }                
            }
            // リンクの全選択解除
            foreach( LinkData itr in m_shaderLinks)
            {
                unselectedLinks.Add(itr);
            }

            // メンバの整合性をとる           
            Inner_Clear();            
            m_shaderNodes.Add(node);            

            // 変更イベントの起動
            SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                    selectedNodes, unselectedNodes, null, unselectedLinks
                );
            if( args.IsValid() )
            {
                if (SelectionChanged != null)
                    SelectionChanged(this, args);
            }
        }
        
        /// <summary>
        /// 対象オブジェクトの単一選択
        /// </summary>
        /// <param name="link"></param>
        public void Select( LinkData link )
        {
            // 他の選択を解除
            Inner_Clear();

            // 選択追加
            m_shaderLinks.Add(link);

            //@@ 必要ならイベント処理            
            List<ShaderNodeDataBase> unselectedNodes = new List<ShaderNodeDataBase>();
            List<LinkData> selectedLinks = new List<LinkData>();
            List<LinkData> unselectedLinks = new List<LinkData>();
 
            // すでに選択済みの場合
            if (IsSelected(link))
            {
                // 他の選択を解除
                // リンクの選択解除
                foreach (LinkData itr in m_shaderLinks)
                {
                    if (itr.CompareTo(link) != 0)
                    {
                        unselectedLinks.Add(itr);
                    }
                }
            }
            // 選択されていなかった場合
            else
            {
                // 全リンクの選択を解除
                foreach (LinkData itr in m_shaderLinks)
                {
                    unselectedLinks.Add(itr);
                }
                // このリンクの選択を追加
                selectedLinks.Add(link);
            }
            // ノードの全選択解除
            foreach (ShaderNodeDataBase itr in m_shaderNodes)
            {
                unselectedNodes.Add(itr);
            }

            // メンバの整合性をとる           
            Inner_Clear();
            m_shaderLinks.Add(link);

            // 変更イベントの起動
            SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                    null, unselectedNodes, selectedLinks, unselectedLinks
                );
            if (args.IsValid())
            {
                if (SelectionChanged != null)
                    SelectionChanged(this, args);
            }
        }

        /// <summary>
        /// 全選択の解除
        /// </summary>
        public void Clear()
        {
            // 全選択の解除
            List<ShaderNodeDataBase> unselectedNodes = new List<ShaderNodeDataBase>();
            List<LinkData> unselectedLinks = new List<LinkData>();  
            foreach ( ShaderNodeDataBase itr in m_shaderNodes)
            {
                unselectedNodes.Add(itr);
            }
            foreach ( LinkData itr in m_shaderLinks )
            {
                unselectedLinks.Add(itr);
            }

            // メンバ変数の整合性を取る
            Inner_Clear();

            // 変更イベントの起動
            SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                    null, unselectedNodes, null, unselectedLinks
                );
            if (args.IsValid())
            {
                if (SelectionChanged != null)
                    SelectionChanged(this, args);
            }
        }        
#endregion

#region private methods
        /// <summary>
        /// 全選択の解除
        /// </summary>
        public void Inner_Clear()
        {
            // ノードの選択解除
            m_shaderNodes.Clear();

            // リンクの選択解除
            m_shaderLinks.Clear();
        }
#endregion
    }    
}
