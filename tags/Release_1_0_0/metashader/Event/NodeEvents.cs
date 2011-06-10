//-----------------------------------------------------------------------------------------
// ShaderNodeに関連するイベント
//-----------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace metashader.Event
{
    /// <summary>
    /// シェーダノードのプロパティが変更された際に起動するイベントの引数
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

    /// <summary>
    /// シェーダノードが追加された際に起動するイベントの引数
    /// </summary>
    public class NodeAddedEventArgs : EventArgs
    {
#region variables
        /// <summary>
        /// 追加対象のノード
        /// </summary>
        ShaderGraphData.ShaderNodeDataBase m_node;
#endregion

        #region properties
        /// <summary>
        /// 追加対象のノード
        /// </summary>
        public ShaderGraphData.ShaderNodeDataBase Node
        {
            get { return m_node; }
        }
        #endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node">追加対象のノード</param>
        public NodeAddedEventArgs(ShaderGraphData.ShaderNodeDataBase node)
        {
            m_node = node;
        }
#endregion
    }

    /// <summary>
    /// ノード追加デリゲータの宣言
    /// </summary>
    /// <param name="sender">送信元</param>
    /// <param name="args">イベント引数</param>
    public delegate void NodeAddedEventHandler( object sender, NodeAddedEventArgs args );

    /// <summary>
    /// シェーダノードが削除された際に起動するイベントの引数
    /// </summary>
    public class NodeDeletedEventArgs : EventArgs
    {
        #region variables
        /// <summary>
        /// 削除対象のノードのハッシュ値
        /// </summary>
        int m_nodeHashCode;
        #endregion

        #region properties
        /// <summary>
        /// 削除対象のノードのハッシュ値
        /// </summary>
        public int NodeHashCode
        {
            get { return m_nodeHashCode; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>    
        /// <param name="nodeHashCode">削除対象のノードのハッシュ値</param>
        public NodeDeletedEventArgs(int nodeHashCode)
        {
            m_nodeHashCode = nodeHashCode;
        }
        #endregion
    }

    /// <summary>
    /// ノード削除デリゲータの宣言
    /// </summary>
    /// <param name="sender">送信元</param>
    /// <param name="args">イベント引数</param>
    public delegate void NodeDeletedEventHandler(object sender, NodeDeletedEventArgs args);

    /// <summary>
    /// リンクが追加された際に起動するイベントの引数
    /// </summary>
    public class LinkAddedEventArgs : EventArgs
    {
#region variables
        /// <summary>
        /// リンク定義
        /// </summary>
        ShaderGraphData.LinkData m_linkData;
#endregion

#region properties
        public ShaderGraphData.LinkData LinkData
        {
            get{ return m_linkData; }
        }
#endregion

#region constructors
        public LinkAddedEventArgs(ShaderGraphData.LinkData linkData)
        {  
            m_linkData = linkData;
        }        
#endregion       
    }

    /// <summary>
    /// リンク追加デリゲータの宣言
    /// </summary>
    /// <param name="sender">送信元</param>
    /// <param name="args">イベント引数</param>
    public delegate void LinkAddedEventHandler(object sender, LinkAddedEventArgs args);

    /// <summary>
    /// リンクが削除された際に呼ばれるイベントの引数
    /// </summary>
    public class LinkDeletedEventArgs : EventArgs
    {
        #region variables
        /// <summary>
        /// リンク定義
        /// </summary>
        ShaderGraphData.LinkData m_linkData;
        #endregion

        #region properties
        public ShaderGraphData.LinkData LinkData
        {
            get { return m_linkData; }
        }
        #endregion

        #region constructors
        public LinkDeletedEventArgs(ShaderGraphData.LinkData linkData)
        {
            m_linkData = linkData;
        }
        #endregion
    }

    /// <summary>
    /// リンク削除デリゲータの宣言
    /// </summary>
    /// <param name="sender">送信元</param>
    /// <param name="args">イベント引数</param>
    public delegate void LinkDeletedEventHandler(object sender, LinkDeletedEventArgs args);

    /// <summary>
    /// エラーの種類
    /// </summary>
    public enum GraphErrorType
    {
        NoError,        /// エラー無し
        InvalidNode,    /// 不正なノード
        InfiniteLoop,   /// 無限ループを含む
        NoOutput,       /// 出力ノードが存在しない
        Max,            /// 最大数
    }

    /// <summary>
    /// グラフ構造に起因するエラーイベント引数
    /// </summary>
    public class GraphErrorEventArgs : EventArgs
    {
        #region properties
        /// <summary>
        /// エラーの種類
        /// </summary>
        GraphErrorType _type;
        /// <summary>
        /// エラーの詳細を表す文字列
        /// </summary>
        string _message;
        #endregion

        #region constructors
        private GraphErrorEventArgs(GraphErrorType type, string message)
        {
            _type = type;
            _message = message;
        }
        #endregion

        #region properties
        /// <summary>
        /// エラーの種類
        /// </summary>
        public GraphErrorType Type { get { return _type; } }
        /// <summary>
        /// エラーの詳細を表す文字列
        /// </summary>
        public string Message { get { return _message; } }
        #endregion

        #region public methods
        /// <summary>
        /// 「エラー無し」用引数の作成
        /// </summary>
        static public GraphErrorEventArgs NoError()
        {
            return new GraphErrorEventArgs(GraphErrorType.NoError, "");
        }

        /// <summary>
        /// 「不正なノード」のエラー
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        static public GraphErrorEventArgs InvalidNode( ShaderGraphData.ShaderNodeDataBase node)
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("Error:Invalid Node");
            writer.WriteLine("{0} が不正です.", node.Name);

            // @@ どう不正なのか

            return new GraphErrorEventArgs(GraphErrorType.InvalidNode, writer.ToString());
        }

        /// <summary>
        /// 「無限ループ」のエラー
        /// </summary>
        /// <param name="node">無限ループ中に含まれるノード</param>
        /// <returns></returns>
        static public GraphErrorEventArgs InfiniteLoop(ShaderGraphData.ShaderNodeDataBase node)
        {
            StringWriter writer = new StringWriter();
            writer.WriteLine("Error:Infinite Loop");
            writer.WriteLine("{0} でループが見つかりました.", node.Name);

            return new GraphErrorEventArgs(GraphErrorType.InfiniteLoop, writer.ToString());
        }

        /// <summary>
        /// 出力ノードが存在しない
        /// </summary>
        /// <returns></returns>
        static public GraphErrorEventArgs NoOutput()
        {
            return new GraphErrorEventArgs(GraphErrorType.NoOutput, "Error:No Output Node");
        }
        #endregion
    }

    public delegate void GraphErrorEventHandler(object sender, GraphErrorEventArgs args);

}
