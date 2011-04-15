/// 
/// ShaderGraphDataのシリアライズ/デシリアライズ用データ構造とメソッド群の定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

namespace metashader.ShaderGraphData
{
    partial class ShaderGraphData : IDeserializationCallback
    {
#region public methods
        /// <summary>
        /// ファイルへ保存
        /// </summary>        
        public void Save(FileStream fileStream, BinaryFormatter formatter)
        {
            // クラス中に含まれるシリアライズ可能なオブジェクトを
            // デフォルトのシリアライザーでシリアライズ
            formatter.Serialize(fileStream, this);

            /// ノードのシリアライズ用データを作成 ///
            LinkedList<ShaderNodeDataBase> nodeList = new LinkedList<ShaderNodeDataBase>();

            // 各ノードを格納
            foreach( ShaderNodeDataBase node in m_nodeList.Values )
            {
                nodeList.AddLast(node);
            }
            formatter.Serialize(fileStream, nodeList);
            
            /// リンクのシリアライズ用データを作成 ///            
            LinkedList<LinkData> linkList = new LinkedList<LinkData>();

            // 各リンクを格納
            foreach( ShaderNodeDataBase node in m_nodeList.Values )
            {
                for(int i = 0; i < node.OutputJointNum; ++i)
                {
                    JointData joint = node.GetOutputJoint(i);
                    foreach( JointData itr in joint.JointList )
                    {
                        linkList.AddLast(
                            new LinkData(
                                joint.ParentNode.GetHashCode()
                                , joint.JointIndex
                                , itr.ParentNode.GetHashCode()
                                , itr.JointIndex
                                )
                            );
                    }
                }
            }
            formatter.Serialize(fileStream, linkList);
        }

        /// <summary>
        /// ファイルからロード
        /// </summary>        
        public static ShaderGraphData Load(FileStream fileStream, BinaryFormatter formatter)
        {
            ShaderGraphData graph = formatter.Deserialize(fileStream) as ShaderGraphData;

            /// ノードのデシリアライズ ///            
            LinkedList<ShaderNodeDataBase> nodeList = 
                formatter.Deserialize(fileStream) as LinkedList<ShaderNodeDataBase>;
            // ノードを再構築
            foreach( ShaderNodeDataBase itr in nodeList )
            {
                graph.AddNode(itr, null);
            }

            /// リンクのデシリアライズ ///            
            LinkedList<LinkData> linkList =
                formatter.Deserialize(fileStream) as LinkedList<LinkData>;

            // グラフ内のリンクを再構築
            foreach( LinkData itr in linkList)
            {
                graph.AddLink(itr._outNodeHash, itr._outJointIndex, itr._inNodeHash, itr._inJointIndex, null);
            }

            return graph;
        }
#endregion        

#region override methods
        /// <summary>
        /// デシリアライズ時のコールバック
        /// </summary>
        /// <param name="sender"></param>
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            //　空のノードリストを作成
            this.m_nodeList = new Dictionary<int, ShaderNodeDataBase>();
        }        
#endregion
    }
}
