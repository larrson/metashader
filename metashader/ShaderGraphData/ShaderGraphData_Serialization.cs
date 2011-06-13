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
    partial class ShaderGraphData
    {
#region public methods
        /// <summary>
        /// ファイルへ保存
        /// </summary>        
        public void Save(FileStream fileStream, BinaryFormatter formatter)
        {
            /// ファクトリのシリアライズ ///
            m_shaderNodeFactory.Save(fileStream, formatter);

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
        public bool Load(FileStream fileStream, BinaryFormatter formatter)
        {
            // ロード前にリセット
            Reset();

            /// ファクトリのデシリアライズ ///
            m_shaderNodeFactory.Load(fileStream, formatter);

            /// ノードのデシリアライズ ///            
            LinkedList<ShaderNodeDataBase> nodeList =
                formatter.Deserialize(fileStream) as LinkedList<ShaderNodeDataBase>;
            // ノードを再構築
            foreach (ShaderNodeDataBase itr in nodeList)
            {
               AddNode(itr, null);
            }

            /// リンクのデシリアライズ ///            
            LinkedList<LinkData> linkList =
                formatter.Deserialize(fileStream) as LinkedList<LinkData>;

            // グラフ内のリンクを再構築
            foreach (LinkData itr in linkList)
            {
                AddLink(itr._outNodeHash, itr._outJointIndex, itr._inNodeHash, itr._inJointIndex, null);
            }

            return true;
        }
#endregion        
    }
}
