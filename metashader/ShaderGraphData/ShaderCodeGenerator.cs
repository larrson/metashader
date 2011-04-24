using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using metashader.Common;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// グラフからシェーダコードを生成するジェネレータ
    /// </summary>
    public class ShaderCodeGenerator
    {
#region variables
        /// <summary>
        /// シェーダーテンプレートファイルの相対パス
        /// </summary>
        static readonly string m_shaderTemplatePath = @"\..\..\data\shader\template\custom_material.msh";

        /// <summary>
        /// 有効なノードのキュー
        /// ノードが依存性の低い順に並ぶ（入力変数が最も最初となる）
        /// </summary>
        List<ShaderNodeDataBase> m_validNodeQue = new List<ShaderNodeDataBase>();

        /// <summary>
        /// シェーダコードを保持するメモリーストリーム
        /// </summary>
        MemoryStream             m_memoryStream = new MemoryStream();

        /// <summary>
        /// 入力ノードのインデックス記録用のマップ
        /// 入力ノードは
        /// </summary>        
        MultiMap<ShaderNodeType, uint> m_inputNodeMap = new MultiMap<ShaderNodeType, uint>();
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
        /// シェーダコードをファイルへ書きだす
        /// </summary>
        /// <param name="path">出力ファイルのパス</param>
       public void ExportToFile( string path )
       {
           // ストリームを作成
           Generate();

           // ストリームをファイルへ書き出し
           File.WriteAllBytes(path, m_memoryStream.ToArray());
       }

       /// <summary>
       /// シェーダコードをバイト列に書きだす
       /// </summary>
       /// <returns></returns>
       public byte[] ExportToBuffer()
       {
           // ストリームを作成
           Generate();

           return m_memoryStream.GetBuffer();
       }
#endregion

#region private methods        
        /// <summary>
        /// メモリーストリームへシェーダコードを作成する
        /// </summary>
        private void Generate()
        {
            /// テンプレート内の置き換えマーカに合わせて
            /// 生成した文字列で置き換える

            // テンプレートファイル
            string templatePath = FileSettings.ApplicationFolderPath + m_shaderTemplatePath;
            using (StreamReader templateStream = new StreamReader( templatePath 
                , Encoding.GetEncoding("shift_jis")))
            {
                using (StreamWriter outputStream = new StreamWriter(m_memoryStream, Encoding.ASCII) )
                {
                    // 終端まで読み込む
                    while (templateStream.EndOfStream == false)
                    {
                        string line = templateStream.ReadLine();
                        string replace = null;

                        switch (line)
                        {
                            case "%HEADER%":
                                replace = GetShaderHeaderString().ToString();
                                break;
                            case "%INCLUDES%": //@@ 要実装
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
                if( node.Type.IsOutputNode() )
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

                        // 含まれるか判定をしているのは、接続先が有効なノードではなく、nodeListに入っていない可能性があるため
                        // 「有効なノードではない」とは、最終出力ノードへたどれないノード
                        if( dependencyCounter.ContainsKey(node.GetHashCode()))
                            dependencyCounter[node.GetHashCode()]--;
                    }                    
                }

                // nodeListから該当するnodeを削除する                                
                nodeList.Remove(removedNode);
            }
        }        

        /// <summary>
        /// シェーダのヘッダ文字列を取得する
        /// </summary>
        /// <returns></returns>
        private StringBuilder GetShaderHeaderString()
        {
            StringWriter stream = new StringWriter();
            stream.WriteLine("// ======================================================================");
            stream.WriteLine("// Exported from {0}", App.CurrentApp.FileSettings.CurrentFilePath);
            stream.WriteLine("// Exported Time : {0}", DateTime.Now.ToString());            
            stream.WriteLine("// Metashader Version : {0}", Assembly.GetExecutingAssembly().GetName().Version);            
            stream.WriteLine("// ======================================================================");

            return stream.GetStringBuilder();
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
        /// 指定した入力ノードがすでに記録されたものか判定する
        /// </summary>
        /// <param name="inputNode"></param>
        /// <returns></returns>
        private bool IsAlreadyWitten( Input_NodeBase inputNode )
        {
            return m_inputNodeMap.Contains( new KeyValuePair<ShaderNodeType, uint>(inputNode.Type, inputNode.Index) );
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
                // すでに書き込み済みの入力属性はスキップする
                Input_NodeBase inputNode = node as Input_NodeBase;
                if( inputNode != null )                
                {             
                    // すでに書きこまれた入力ノードならスキップ
                    if( IsAlreadyWitten( inputNode) )
                    {
                        continue;
                    }
                    // 未書き込みなら、マップに記録しておく
                    else
                    {
                        m_inputNodeMap.Add(inputNode.Type, inputNode.Index);
                    }
                }

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
