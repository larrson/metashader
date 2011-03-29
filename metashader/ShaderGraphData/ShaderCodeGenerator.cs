using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// グラフからシェーダコードを生成するジェネレータ
    /// </summary>
    public class ShaderCodeGenerator
    {
#region variables
        /// <summary>
        /// 有効なノードのキュー
        /// ノードが依存性の低い順に並ぶ（入力変数が最も最初となる）
        /// </summary>
        List<ShaderNodeDataBase> m_validNodeQue = new List<ShaderNodeDataBase>();
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>        
        public ShaderCodeGenerator( ReadOnlyCollection<ShaderNodeDataBase> nodeList )
        {
            // 有効なノードのみを取り出す
            List<ShaderNodeDataBase> validNodes =
                extractValidNodes(nodeList);

            // 有効なノードを依存関係に合わせて並び替える
            Sort( validNodes );
        }
#endregion        

#region public methods
        /// <summary>
        /// シェーダコードを生成する
        /// </summary>
        /// <param name="path">出力ファイルのパス</param>
       public void Generate( string path )
       {
            /// テンプレート内の置き換えマーカに合わせて
            /// 生成した文字列で置き換える
            
            // テンプレートファイル
           using(StreamReader templateStream = new StreamReader(@"C:\projects\metashader\data\custom_material.msh"
               , Encoding.GetEncoding("shift_jis")))
           {
               using ( StreamWriter outputStream = new StreamWriter(path, false, Encoding.GetEncoding("shift_jis")) )
               {
                   // 終端まで読み込む
                   while (templateStream.EndOfStream == false )
                   {
                       string line = templateStream.ReadLine();
                       string replace = null;

                       switch( line )
                       {
                           case "%INCLUDES%": //@@ 要実装
                               replace = "";
                               break;
                           case "%HEADER%": //@@ 要実装
                               replace = "";
                               break;                               
                           case "%UNIFORMS%":
                               replace = GetShaderUniformString().ToString();
                               break;
                           case "%PS_INPUT%":
                               replace = GetShaderInputString().ToString();
                               break;
                           case "%PS_MAIN%":
                               replace = GetShaderMainString().ToString();
                               break;
                           default:
                               replace = line;
                               break;
                       }

                       outputStream.WriteLine(replace);
                   }               
               }               
           }
       }
#endregion

#region private methods
        /// <summary>
        /// 有効なノードを抽出する
        /// ここで、「有効である」とは、対象とするノードが最終的に出力ノードへのパスをもつこと
        /// </summary>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        private List<ShaderNodeDataBase> extractValidNodes(  ReadOnlyCollection<ShaderNodeDataBase> nodeList )
        {
            // 最終出力ノードを探す
            ShaderNodeDataBase outputNode = null;
            foreach( ShaderNodeDataBase node in nodeList)
            {
                if( IsOutputNode(node) )
                {
                    outputNode = node;
                    break;
                }
            }
            if( outputNode == null )
            {
                throw new ArgumentException("渡されているシェーダノードが不正です");
            }

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
                    stack.Push(node.GetInputJoint(i).JointList.First.Value.ParentNode);
                }
            }

            // 抽出された有効なノードのリストを返す
            List<ShaderNodeDataBase> ret = new List<ShaderNodeDataBase>( validNodes.Values );

            return ret;
        }

        /// <summary>
        /// 出力ノードかの判定
        /// @ ノード自体に、判定関数を用意すべきかも
        /// </summary>
        /// <param name="node"></param>
        private bool IsOutputNode(ShaderNodeDataBase node)
        {
            return node.OutputJointNum == 0;
        }

        /// <summary>
        /// ノード間の依存関係に合わせてソートする
        /// </summary>
        private void Sort(List<ShaderNodeDataBase> nodeList )
        {
            // 依存性カウンタ
            // 各ノード毎の入力リンク数を保存する
            Dictionary<int,int> dependencyCounter = new Dictionary<int,int>(); 
            foreach ( ShaderNodeDataBase node in nodeList)
            {
                dependencyCounter.Add( node.GetHashCode(), node.InputJointNum);
            }

            // 全てのノードが取り除かれるまで、
            // 依存度の低い順にnodeListから取り出していく
            while( nodeList.Count > 0 )
            {
                // 取り除くべき、依存性0のノード
                ShaderNodeDataBase removedNode = null;

                foreach( ShaderNodeDataBase node in nodeList )
                {
                    int dependency = dependencyCounter[node.GetHashCode()];
                    
                    // 依存度0ならば取り出す
                    if (dependency == 0)
                    {
                        removedNode = node;
                        break;
                    }   
                }
                if( removedNode == null )
                {
                    throw new ArgumentException("依存度0のノードが見つかりませんでした。nodeListのトポロジが不正です");
                }

                // removeNodeをこのクラスのキューに積む
                m_validNodeQue.Add(removedNode);

                // removeNodeからのリンクを入力とするノードの依存度を減らす
                for(int i = 0; i < removedNode.OutputJointNum; ++i )
                {
                    // 各出力ジョイントの接続先の依存度を減らす
                    foreach( JointData joint in removedNode.GetOutputJoint(i).JointList )
                    {
                        ShaderNodeDataBase node = joint.ParentNode;
                        dependencyCounter[node.GetHashCode()]--;
                    }                    
                }

                // nodeListから該当するnodeを削除する                                
                nodeList.Remove(removedNode);
            }
        }        

        /// <summary>
        /// シェーダのuniform宣言の文字列を取得する
        /// </summary>       
        private StringBuilder GetShaderUniformString()
        {
            StringWriter stream = new StringWriter();
            foreach( ShaderNodeDataBase node in m_validNodeQue)
            {
                node.WritingShaderUniformCode(stream);
            }
            return stream.GetStringBuilder();
        }

        /// <summary>
        /// シェーダの入力属性の文字列を取得する
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetShaderInputString()
        {
            StringWriter stream = new StringWriter();
            foreach (ShaderNodeDataBase node in m_validNodeQue)
            {
                node.WritingShaderInputCode(stream);
            }
            return stream.GetStringBuilder();
        }

        /// <summary>
        /// シェーダの本文の文字列を取得する
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetShaderMainString()
        {
            StringWriter stream = new StringWriter();            
            foreach (ShaderNodeDataBase node in m_validNodeQue)
            {
                node.WritingShaderMainCode(stream);
            }
            return stream.GetStringBuilder();
        }
#endregion
    }
}
