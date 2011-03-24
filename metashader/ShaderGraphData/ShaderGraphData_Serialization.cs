/// 
/// ShaderGraphDataのシリアライズ/デシリアライズ用データ構造とメソッド群の定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace metashader.ShaderGraphData
{
    partial class ShaderGraphData
    {
#region member classes
        /// <summary>
        /// リンクデータシリアライズ用
        /// </summary>
        [Serializable]
        struct Serialize_LinkData
        {
            public int _outNodeHash;
            public int _outJointIndex;
            public int _inNodeHash;
            public int _inJointIndex;

            public Serialize_LinkData(int outHash, int outIndex, int inHash, int inIndex)
            {
                _outNodeHash = outHash;
                _outJointIndex = outIndex;
                _inNodeHash = inHash;
                _inJointIndex = inIndex;
            }
        };
#endregion

#region public methods
        /// <summary>
        /// ファイルへ保存
        /// </summary>        
        public void Save(FileStream fileStream, BinaryFormatter formatter)
        {
            // ノードを保存
            formatter.Serialize(fileStream, this);
            
            /// リンクのシリアライズ用データを作成 ///            
            LinkedList<Serialize_LinkData> linkList = new LinkedList<Serialize_LinkData>();

            // 各リンクを格納
            foreach( ShaderNodeDataBase node in m_nodeList.Values )
            {
                for(int i = 0; i < node.OutputJointNum; ++i)
                {
                    JointData joint = node.GetOutputJoint(i);
                    foreach( JointData itr in joint.JointList )
                    {
                        linkList.AddLast(
                            new Serialize_LinkData(
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

            /// リンクのデシリアライズ ///            
            LinkedList<Serialize_LinkData> linkList =
                formatter.Deserialize(fileStream) as LinkedList<Serialize_LinkData>;

            // グラフ内のリンクを再構築
            foreach( Serialize_LinkData itr in linkList)
            {
                graph.AddLink(itr._outNodeHash, itr._outJointIndex, itr._inNodeHash, itr._inJointIndex, null);
            }

            return graph;
        }
#endregion        
    }
}
