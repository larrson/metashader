//-----------------------------------------------------------------------------------------
// ShaderNodeに関連するイベント
//-----------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.Event
{
    /// <summary>
    /// シェーダノードのプロパティが変更された際に起動するイベント
    /// </summary>
    public class NodePropertyChangedEventArgs : EventArgs
    {
        #region variables
        /// <summary>
        /// 変更対象のノード
        /// </summary>
        ShaderGraphData.ShaderNodeDataBase m_node;

        /// <summary>
        /// プロパティ名
        /// </summary>
        string m_propertyName;

        /// <summary>
        /// 変更後の値
        /// </summary>
        object m_newValue;
        #endregion

        #region properties
        /// <summary>
        /// 変更対象のノード
        /// </summary>
        public ShaderGraphData.ShaderNodeDataBase Node
        {
            get { return m_node; }
        }

        /// <summary>
        /// ノードのハッシュ値
        /// </summary>            
        public int NodeHashCode
        {
            get { return m_node.GetHashCode(); }
        }        

        /// <summary>
        /// 変更されたプロパティ名
        /// </summary>
        public string PropertyName
        {
            get { return m_propertyName; }
        }

        /// <summary>
        /// 新しい値を取得する
        /// </summary>
        public object NewValue
        {
            get
            {
                return m_newValue;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node">変更対象ノード</param>
        /// <param name="propertyType">変更対象のプロパティの型</param>
        /// <param name="propertyName">変更対象のプロパティ名</param>
        /// <param name="newValue">変更後の値</param>
        public NodePropertyChangedEventArgs(ShaderGraphData.ShaderNodeDataBase node, string propertyName, object newValue)
        {
            m_node = node;            
            m_propertyName = propertyName;
            m_newValue = newValue;
        }
        #endregion
    }

    /// <summary>
    /// ノードプロパティ変更デリゲータの宣言
    /// </summary>
    /// <param name="sender">送信元</param>
    /// <param name="args">イベント引数</param>
    public delegate void NodePropertyChangedEventHandler( object sender, NodePropertyChangedEventArgs args);
}
