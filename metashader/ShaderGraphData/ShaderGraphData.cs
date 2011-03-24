﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// シェーダーグラフのデータ構造
    /// </summary>    
    [Serializable]
    public partial class ShaderGraphData
    {
#region variables
        /// <summary>
        /// シェーダノードのリスト
        /// </summary>         
        Dictionary<int, ShaderNodeDataBase> m_nodeList = new Dictionary<int, ShaderNodeDataBase>();

        /// <summary>
        /// シェーダノード用ファクトリ
        /// </summary>        
        ShaderNodeFactory m_shaderNodeFactory = new ShaderNodeFactory();       
#endregion

#region public methods                  

        /// <summary>
        /// シェーダノードの取得
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ShaderNodeDataBase GetNode( string name )
        {
            ShaderNodeDataBase node;
            m_nodeList.TryGetValue(ShaderNodeDataBase.CalcHashCode(name), out node);

            return node;
        }

        /// <summary>
        /// 新規のシェーダノードの追加
        /// </summary>
        /// <returns></returns>
        public bool AddNewNode(ShaderNodeType type, Point pos, UndoRedoBuffer undoredo, out ShaderNodeDataBase newNode )
        {
            // 新しくノードを作成
            ShaderNodeDataBase node = CreateNewNode(type, pos);

            // 追加
            if ( AddNode( node, null) == false )
            {                
                // 追加に失敗
                newNode = null;
                return false; // 処理的には何もしていないので、Redo/Undoは積まない
            }
  
            // 追加に成功したのでnodeを返す
            newNode = node;

            // 追加に成功したのでファクトリのカウンタをインクリメント                
            m_shaderNodeFactory.IncrementCounter(type);

            // Undo/Redoバッファがあれば処理を積む
            if( undoredo != null )
            {
                undoredo.Add( new UndoRedo_AddNewNode(this, type, pos, node.GetHashCode()) );
            }

            return true;
        }

        /// <summary>
        /// 既存のシェーダノードの追加
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="undoredo"></param>
        /// <returns></returns>
        public bool AddNode( ShaderNodeDataBase nodeData, UndoRedoBuffer undoredo )
        {
            // すでに追加済みか？
            ShaderNodeDataBase existData;
            if( m_nodeList.TryGetValue( nodeData.GetHashCode(), out existData ) )
            {
                // ハッシュの重複チェック
                if( existData.Name != nodeData.Name )
                {
                    // 名前が異なるにも関わらずハッシュが同じ！
                    throw new Exception(nodeData.Name + "と" + existData.Name + "のハッシュ値が重複しています");
                }                
                else
                {
                    // すでに同じ名前のノードが登録済み
                    // なにもしない
                    return false;
                }
            }

            // 追加処理
            m_nodeList.Add(nodeData.GetHashCode(), nodeData);

            // Undo/Redoバッファがあれば処理を積む
            if( undoredo != null )
            {
                undoredo.Add(new UndoRedo_AddNode(this, nodeData));
            }
            
            return true;
        }

        /// <summary>
        /// シェーダーノードの削除
        /// </summary>
        /// <param name="hashCode"></param>
        /// <param name="undoredo"></param>
        /// <returns></returns>
        public bool DelNode( int hashCode, UndoRedoBuffer undoredo )
        {
            // ノードの存在を確認
            if( m_nodeList.ContainsKey(hashCode) == false )
            {
                // 対応するノード無ければなにもしない
                return false;
            }

            // 削除対象のノードを取得
            ShaderNodeDataBase node;
            m_nodeList.TryGetValue(hashCode, out node);
                        
            /// リンクの削除            
            // 入力リンクの削除
            for (int i = 0; i < node.InputJointNum; ++i )
            {
                JointData inJoint = node.GetInputJoint(i);
                while( inJoint.JointList.Count != 0 )
                {
                    JointData outJoint = inJoint.JointList.First.Value;
                    DelLink(outJoint.ParentNode.GetHashCode(), outJoint.JointIndex
                        , inJoint.ParentNode.GetHashCode(), inJoint.JointIndex
                        , undoredo);
                }
            }

            // 出力リンクの削除
            for (int i = 0; i < node.OutputJointNum; ++i)
            {
                JointData outJoint = node.GetOutputJoint(i);
                while( outJoint.JointList.Count != 0 )
                {
                    JointData inJoint = outJoint.JointList.First.Value;                
                    DelLink(outJoint.ParentNode.GetHashCode(), outJoint.JointIndex
                        , inJoint.ParentNode.GetHashCode(), inJoint.JointIndex
                        , undoredo);
                }
            }

            /// ノードの削除
            {
                // 削除                
                m_nodeList.Remove(hashCode);

                // Undo/Redoバッファがあれば処理を積む
                if (undoredo != null)
                {
                    undoredo.Add(new UndoRedo_DelNode(this, node));
                }
            }

            return true;
        }

        /// <summary>
        /// 対象のリンクを追加可能か
        /// </summary>       
        /// <returns></returns>
        public bool CanAddLink( int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex )
        {
            // 追加対象のノード(Out/In双方)が存在するか
            ShaderNodeDataBase outNode;
            if( m_nodeList.TryGetValue(outNodeHashCode, out outNode) == false )
            {
                // 見つからなかったら追加不可能
                return false;
            }
            ShaderNodeDataBase inNode;
            if (m_nodeList.TryGetValue(inNodeHashCode, out inNode) == false)
            {
                // 見つからなかったら追加不可能
                return false;
            }

            // 追加対象のノードの各ジョイントに関して
            // 出力ジョイントの判定            
            if (outJointIndex >= outNode.OutputJointNum) // インデックスがオーバーしているか？
            {
                return false;
            }
            // 入力ジョイントの判定
            if (    inJointIndex >= inNode.InputJointNum　        // インデックスがオーバーしているか？
                ||  !inNode.GetInputJoint(inJointIndex).CanNewConnect() // 対象ジョイントに新たなリンクが可能か？
                ) 
            {
                return false;
            }

            //@@@ 追加した結果閉路になってしまわないか
            // とりあえず同一性のみ判定
            if (outNodeHashCode == inNodeHashCode)
                return false;

            // 全ての追加判定に成功
            return true;
        }

        /// <summary>
        /// リンクを追加
        /// </summary>
        /// <param name="outNodeHashCode"></param>
        /// <param name="outJointIndex"></param>
        /// <param name="inNodeHashCode"></param>
        /// <param name="inJointIndex"></param>
        /// <returns>追加に成功したか</returns>
        public bool AddLink(int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex, UndoRedoBuffer undoredo )
        {
            // 追加可能か判定
            if( CanAddLink(outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex ) == false )
            {
                // 追加不可能だった
                return false;
            }

            /// 追加処理 /// 

            // 追加対象のノード(Out/In双方)を取得（上記の追加判定をクリアしているので取得出来るはず）
            ShaderNodeDataBase outNode,  inNode;
            m_nodeList.TryGetValue(outNodeHashCode, out outNode);                        
            m_nodeList.TryGetValue(inNodeHashCode, out inNode);

            // 入出力ジョイントを取得
            JointData outJoint, inJoint;
            outJoint = outNode.GetOutputJoint(outJointIndex);
            inJoint = inNode.GetInputJoint(inJointIndex);
            
            // 接続
            // 出力ノード→入力ノード
            outJoint.Connect(inJoint);

            // 入力ノード←出力ノード
            inJoint.Connect(outJoint);

            // Undo/Redoバッファがあれば処理を積む
            if( undoredo != null )
            {
                undoredo.Add( new UndoRedo_AddLink(this, outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex));
            }

            return true;
        }

        /// <summary>
        /// 指定したリンクを削除出来るか
        /// </summary>
        /// <param name="outNodeHashCode"></param>
        /// <param name="outJointIndex"></param>
        /// <param name="inNodeHashCode"></param>
        /// <param name="inJointIndex"></param>
        /// <returns></returns>
        public bool CanDelLink(int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex)
        {
            // 追加対象のノード(Out/In双方)が存在するか
            ShaderNodeDataBase outNode;
            if (m_nodeList.TryGetValue(outNodeHashCode, out outNode) == false)
            {
                // 見つからなかったら削除不可能
                return false;
            }
            ShaderNodeDataBase inNode;
            if (m_nodeList.TryGetValue(inNodeHashCode, out inNode) == false)
            {
                // 見つからなかったら削除不可能
                return false;
            }

            // 削除対象のノードの各ジョイントに関して
            // 出力ジョイントの判定            
            if (outJointIndex >= outNode.OutputJointNum) // インデックスがオーバーしているか？
            {
                return false;
            }
            // 入力ジョイントの判定
            if (inJointIndex >= inNode.InputJointNum)　  // インデックスがオーバーしているか？                                
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 指定したリンクを削除する
        /// </summary>
        /// <param name="outNodeHashCode"></param>
        /// <param name="outJointIndex"></param>
        /// <param name="inNodeHashCode"></param>
        /// <param name="inJointIndex"></param>
        /// <param name="undoredo"></param>
        /// <returns></returns>
        public bool DelLink(int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex, UndoRedoBuffer undoredo )
        {
            // 削除可能か？
            if( CanDelLink( outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex) == false )
            {
                // 不可能
                return false;
            }

            /// 削除処理 ///
            // 入出力両ノードを取得
            ShaderNodeDataBase outNode, inNode;
            m_nodeList.TryGetValue(outNodeHashCode, out outNode);
            m_nodeList.TryGetValue(inNodeHashCode, out inNode);

            // 入出力の両ジョイントを取得
            JointData outJoint, inJoint;
            outJoint = outNode.GetOutputJoint(outJointIndex);
            inJoint  = inNode.GetInputJoint(inJointIndex);

            // 出力ノード内から入力ノードのジョイントを削除
            outJoint.Disconnect(inJoint);

            // 入力ノード内から出力ノードのジョイントを削除
            inJoint.Disconnect(outJoint);

            // Undo/Redoバッファがあれば処理を積む
            if (undoredo != null)
            {
                undoredo.Add(new UndoRedo_DelLink(this, outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex));
            }

            return true;
        }
#endregion

#region private methods
        /// <summary>
        /// シェーダーノードを作成する
        /// </summary>
        /// <param name="type">種類</param>
        /// <param name="pos">表示位置</param>                
        /// <returns></returns>       
        private ShaderNodeDataBase CreateNewNode( ShaderNodeType type, Point pos)
        {                        
            return m_shaderNodeFactory.Create( type, pos );
        }

        /// <summary>
        /// ファクトリのカウンタのインクリメント
        /// </summary>
        private void IncreamentShaderNodeCounter( ShaderNodeType type )
        {
            m_shaderNodeFactory.IncrementCounter( type );
        }

        /// <summary>
        /// ファクトリのカウンタのデクリメント
        /// </summary>
        private void DecrementShaderNodeCounter( ShaderNodeType type )
        {
            m_shaderNodeFactory.DecrementCounter( type );
        }
#endregion

#if DEBUG
        /// <summary>
        /// ノードのデバッグ出力
        /// </summary>
        public void DebugPrintNodeList()
        {
            // ノードリストの出力
            System.Console.WriteLine("< Node List >");            
            foreach( ShaderNodeDataBase node in m_nodeList.Values)
            {
                System.Console.WriteLine( node.Name + "(" + "{0}" + ")", node.GetHashCode());
            }
            System.Console.WriteLine("");
        }

        /// <summary>
        /// リンクリストのデバッグ出力
        /// </summary>
        public void DebugPrintLinkList()
        {
            // ノードごとに出力
            System.Console.WriteLine("< Link List >");
            foreach (ShaderNodeDataBase node in m_nodeList.Values)
            {
                System.Console.WriteLine(node.Name + "(" + "{0}" + ")", node.GetHashCode());

                for(int i = 0; i < node.OutputJointNum; ++i)
                {
                    System.Console.Write(" [{0}]", i);
                    JointData joint = node.GetOutputJoint(i);
                    foreach ( JointData itr in joint.JointList)
                    {
                        System.Console.Write( itr.ParentNode.Name + "({0}),", itr.JointIndex );
                    }
                    System.Console.WriteLine("");
                }
            }
            System.Console.WriteLine("");
        }
#endif // DEBUG
    }
}
