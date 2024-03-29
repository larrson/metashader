﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using metashader.Common;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// シェーダーグラフのデータ構造
    /// グラフデータの保持とその変更通知を行うが、
    /// イベントのハンドリングは行わない（データの独立性を高めるため）。
    /// </summary>    
    [Serializable]
    public partial class ShaderGraphData
    {
#region variables        
        /// <summary>
        /// 出力ノードのリスト
        /// グラフの処理の高速化やエラー処理用
        /// </summary>
        [NonSerialized]
        List<ShaderNodeDataBase> m_outputNodeList = new List<ShaderNodeDataBase>();

        /// <summary>
        /// シェーダノードのリスト
        /// </summary>     
        [NonSerialized]
        Dictionary<int, ShaderNodeDataBase> m_nodeList = new Dictionary<int, ShaderNodeDataBase>();

        /// <summary>
        /// シェーダノード用ファクトリ
        /// </summary>        
        [NonSerialized]
        ShaderNodeFactory m_shaderNodeFactory = new ShaderNodeFactory();       
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShaderGraphData()
        {                    
        }        
#endregion

#region properties
        /// <summary>
        /// 格納されているノードのリストを取得する
        /// </summary>
        private ReadOnlyCollection<ShaderNodeDataBase> NodeList
        {
            get 
            {
                List<ShaderNodeDataBase> list = new List<ShaderNodeDataBase>(m_nodeList.Values);
                return list.AsReadOnly();
            }
        }

        /// <summary>
        /// 有効なノードのリストを返す
        /// ここでの有効なノードとは、出力ノードへのパスに含まれるノードのこと
        /// </summary>
        /// <returns></returns>
        public List<ShaderNodeDataBase> ValidNodes
        {
            get 
            {
                // 出力ノードが確定しない
                if( m_outputNodeList.Count != 1 )
                {
                    throw new InvalidOperationException("出力ノードが確定していません");
                }

                // 出力ノード
                ShaderNodeDataBase outputNode = m_outputNodeList[0];

                // 有効なノードを入れる連想コンテナ
                Dictionary<int, ShaderNodeDataBase> validNodes = new Dictionary<int, ShaderNodeDataBase>();

                // 深さ優先探索で有効なノードを抽出
                Stack<ShaderNodeDataBase> stack = new Stack<ShaderNodeDataBase>(); // 深さ優先探索用コンテナ
                stack.Push(outputNode);
                while( stack.Count > 0 )
                {
                    ShaderNodeDataBase node = stack.Pop();
                    
                    // すでに連想コンテナに入っていれば、探索せずに、スタックの次の要素へ
                    if( validNodes.ContainsKey(node.GetHashCode()) )
                    {
                        continue;
                    }
                    
                    // 新しく探索するノードなので、連想コンテナへ積む
                    validNodes.Add(node.GetHashCode(), node);

                    // このノードの入力ジョイントへ接続されているノードをスタックへ積む
                    for(int i = 0; i < node.InputJointNum; ++i)
                    {
                        foreach( JointData outputJoint in node.GetInputJoint(i).JointList )
                        {
                            stack.Push(outputJoint.ParentNode);
                        }                        
                    }
                }

                // 抽出された有効なノードのリストを返す
                return new List<ShaderNodeDataBase>( validNodes.Values );
            }
        }

        /// <summary>
        /// 出力ノード
        /// </summary>
        public ShaderNodeDataBase OutputNode
        {
            get { return m_outputNodeList[0]; }
        }        

        /// <summary>
        /// 有効なノードのタイプのリスト
        /// </summary>
        public ReadOnlyCollection<string> ValidNodeTypeList
        {
            get { return m_shaderNodeFactory.ValidNodeTypeList; }
        }
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
        /// シェーダノードの取得
        /// </summary>
        /// <param name="hashCode"></param>
        /// <returns></returns>
        public ShaderNodeDataBase GetNode( int hashCode )
        {
            return m_nodeList[hashCode];
        }

        /// <summary>
        /// 新規のシェーダノードの追加
        /// </summary>
        /// <returns></returns>
        public bool AddNewNode(string type, Point pos, UndoRedoBuffer undoredo, out ShaderNodeDataBase newNode )
        {
            // 新しくノードを作成
            ShaderNodeDataBase node = CreateNewNode(type, pos);
            
            // 追加
            if ( node == null || AddNode( node, null) == false )
            {                
                // 追加に失敗
                newNode = null;
                return false; // 処理的には何もしていないので、Redo/Undoは積まない
            }
  
            // 追加に成功したのでnodeを返す
            newNode = node;

            // 追加に成功したのでファクトリのカウンタをインクリメント                
            m_shaderNodeFactory.IncrementID(type);

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
            if( nodeData == null )
            {
                throw new ArgumentNullException("nodeData");
            }

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

            // 出力ノードならリストに追加
            if (nodeData.Type == "Output_Material")
                m_outputNodeList.Add(nodeData);

            // Undo/Redoバッファがあれば処理を積む
            if( undoredo != null )
            {
                undoredo.Add(new UndoRedo_AddNode(this, nodeData));
            }

            // 種類のインスタンスカウンタを増やす
            m_shaderNodeFactory.IncrementInstance(nodeData.Type);

            // イベントを通知
            App.CurrentApp.EventManager.RaiseNodeAdded(this, new metashader.Event.NodeAddedEventArgs(nodeData));

            // エラー検出
            DetectError();
            
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

                // 出力ノードならリストから削除
                if( node.Type == "Output_Material" )
                    m_outputNodeList.Remove(node);
            }

            // 種類のインスタンスカウンタを減らす
            m_shaderNodeFactory.DecrementInstance(node.Type);

            // ノード削除時のイベントを起動する
            App.CurrentApp.EventManager.RaiseNodeDeleted(this, new Event.NodeDeletedEventArgs(hashCode));

            // エラー検出
            DetectError();

            return true;
        }

        /// <summary>
        /// 指定したノードの指定したプロパティへ新しい値を設定する
        /// </summary>
        /// <typeparam name="ParameterType">設定する値の型</typeparam>
        /// <param name="node">対象ノード</param>
        /// <param name="propertyName">対象ノードがもつプロパティ</param>
        /// <param name="newValue">新しい値</param>
        /// <param name="undoredo">Undo/Redo用コマンドバッファ</param>
        /// <returns>新しい値の設定が成功したか</returns>
        public bool ChangeNodeProperty<ParameterType>(ShaderNodeDataBase node, string propertyName, ParameterType newValue, UndoRedoBuffer undoredo)
        {
            // パラメータ設定用のIUndoRedoクラスのインタンス
            ParameterUndoRedo<ParameterType, ShaderNodeDataBase> parameterUndoRedo = new ParameterUndoRedo<ParameterType, ShaderNodeDataBase>(node, propertyName, newValue);

            // 値の設定を試みる
            if( parameterUndoRedo.Do() == false )
            {
                // 失敗
                return false;
            }
            
            // 成功
            // undoredoコマンドバッファへ積む
            if( undoredo != null )
            {
                undoredo.Add(parameterUndoRedo);
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
            
            // 同一性のみ判定
            // 閉路の判定はシェーダ実行時に判定する
            // 厳密に判定し、エッジの操作を禁止してしまうと、編集操作がしずらいため
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

            // リンク追加イベントを起動
            App.CurrentApp.EventManager.RaiseLinkAdded(this, new Event.LinkAddedEventArgs(new LinkData(outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex)));

            // エラー検出
            DetectError();

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

            // リンク削除時のイベントを起動する
            App.CurrentApp.EventManager.RaiseLinkDeleted(this, new Event.LinkDeletedEventArgs(new LinkData(outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex)));

            // エラー検出
            DetectError();

            return true;
        }

        /// <summary>
        /// シェーダコードをバイト列へ書きだす
        /// </summary>        
        public bool ExportShaderCodeToBuffer(out byte[] buffer)
        {
            buffer = null;

            // 書き出し可能か判定
            if (NoError == false)
                return false;
            
            // ジェネレータに処理を移譲
            ShaderCodeGeneratorBase generator = new MaterialShaderGenerator( App.CurrentApp.GlobalSettings.MaterialType, this);
            buffer = generator.ExportToBuffer();

            return true;
        }
        
        /// <summary>
        /// シェーダコードをファイルへ書きだす
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ExportShaderCode(string path)
        {
            // 書き出し可能か判定
            if (NoError == false)
                return false;

            // ジェネレータに処理を移譲
            ShaderCodeGeneratorBase generator = new MaterialShaderGenerator(App.CurrentApp.GlobalSettings.MaterialType, this);
            generator.ExportToFile(path);

            return true;
        }

        /// <summary>
        /// シェーダノードの各パラメータをPreviwerへと適用する
        /// </summary>
        public void ApplyAllParameters()
        {
            foreach (ShaderNodeDataBase node in NodeList)
            {
                IAppliableParameter param = node as IAppliableParameter;
                if( param != null )
                {
                    param.ApplyParameter();
                }
            }            
        }

        /// <summary>
        /// 外部からの初期化用
        /// データ構造を初期状態に戻す
        /// </summary>
        public void Reset()
        {
            // 全てのノードを消す
            // 通常の操作と同じメソッドを使用して、
            // イベントに依存している処理(主にUI)側も
            int[] hashCodes = m_nodeList.Keys.ToArray();
            foreach( int hashCode in hashCodes )
            {
                DelNode(hashCode, null);
            }

            // ファクトリの初期化
            m_shaderNodeFactory.Reset();
        }
#endregion

#region private methods
        /// <summary>
        /// シェーダーノードを作成する
        /// </summary>
        /// <param name="type">種類</param>
        /// <param name="pos">表示位置</param>                
        /// <returns></returns>       
        private ShaderNodeDataBase CreateNewNode( string type, Point pos)
        {                        
            return m_shaderNodeFactory.Create( type, pos );
        }

        /// <summary>
        /// ファクトリのカウンタのインクリメント
        /// </summary>
        private void IncreamentShaderNodeCounter( string type )
        {
            m_shaderNodeFactory.IncrementID( type );
        }

        /// <summary>
        /// ファクトリのカウンタのデクリメント
        /// </summary>
        private void DecrementShaderNodeCounter( string type )
        {
            m_shaderNodeFactory.DecrementID( type );
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
                node.DebugPrint();
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
