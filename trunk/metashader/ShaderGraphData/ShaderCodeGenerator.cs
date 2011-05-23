using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using metashader.Common;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// 全体のコードジェネレータ(ShaderCodeGeneratorBase)
    /// と部分コードジェネレータ（CoCodeGenerator）
    /// の共通基底クラス
    /// </summary>
    public abstract class CommonCodeGeneratorBase
    {
        #region variables
        /// <summary>
        /// マクロの唯一性を保証する連想コンテナ
        /// </summary>
        protected HashSet<string> m_macroSet = new HashSet<string>();

        /// <summary>
        /// インクルードするファイルの唯一性を保証するコンテナ
        /// </summary>
        protected HashSet<string> m_includeSet = new HashSet<string>();

        /// <summary>
        /// uniform変数の唯一性を保証する連想コンテナ
        /// </summary>
        protected HashSet<string> m_uniformSet = new HashSet<string>();

        /// <summary>
        /// 入力変数の唯一性を保証する連想コンテナ
        /// </summary>
        protected HashSet<string> m_inputSet = new HashSet<string>();
        #endregion

        #region protected Methods
        /// <summary>
        /// マクロを追加する
        /// 例) AddMacro("TEST") ⇒ #define TEST
        /// </summary>
        /// <param name="macro">追加するマクロ</param>
        protected void AddMacro(string macro)
        {
            m_macroSet.Add("#define " + macro + "\r\n");
        }

        /// <summary>
        /// インクルードするファイルのパスを追加する        
        /// </summary>
        /// <param name="filePath">インクルードファイルの相対ファイルパス</param>
        protected void AddIncludeFilePath(string filePath)
        {
            m_includeSet.Add("#include " + "\"" + filePath + "\"\r\n");
        }

        /// <summary>
        /// Uniform変数の定義を追加する
        /// 例)AddUniform("uniform float param;") ⇒ uniform float param;
        /// </summary>
        /// <param name="uniform"></param>
        protected void AddUniform(string uniform)
        {
            m_uniformSet.Add(uniform + "\r\n");
        }

        /// <summary>
        /// 入力属性の追加        
        /// @@ 共通の文字列変換関数を作成すべき。セマンティックの種類とインデックスから生成するように。
        /// </summary>
        /// <param name="attribute">セミコロン込みの宣言文</param>        
        protected void AddInputAttribute(string attribute)
        {
            m_inputSet.Add("\t" + attribute + "\r\n");
        }
        #endregion
    }

    /// <summary>
    /// シェーダをコード生成するジェネレータクラスの基底クラス
    /// </summary>
    public abstract class ShaderCodeGeneratorBase : CommonCodeGeneratorBase
    {
        #region variables
        /// <summary>
        /// 生成対象のグラフ
        /// </summary>
        protected ShaderGraphData m_graphData;

        /// <summary>
        /// ジョイントごとのコード生成
        /// </summary>
        protected Dictionary<string, CoCodeGenerator> m_cogeneratorDict = new Dictionary<string, CoCodeGenerator>();

        /// <summary>
        /// シェーダコードを保持するメモリーストリーム
        /// </summary>
        MemoryStream m_memoryStream = new MemoryStream();
        #endregion

        #region constructors
        public ShaderCodeGeneratorBase(ShaderGraphData graphData)
        {
            // メンバ変数の初期化
            m_graphData = graphData;
        }
        #endregion

        #region properties
        /// <summary>
        /// シェーダのテンプレートファイルのパス
        /// </summary>
        protected abstract string TemplateFilePath
        {
            get;
        }
        #endregion

        #region public methods
        /// <summary>
        /// シェーダコードをファイルへ書きだす
        /// </summary>
        /// <param name="path">出力ファイルのパス</param>
        public void ExportToFile(string path)
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
            // 出力ノード
            ShaderNodeDataBase outputNode = m_graphData.OutputNode;

            // 出力ノードの入力ジョイントごとにコードを生成            
            for (int i = 0; i < outputNode.InputJointNum; ++i)
            {
                CoCodeGenerator cogenerator = new CoCodeGenerator(outputNode.GetInputJoint(i));
                cogenerator.Generate();
                m_cogeneratorDict.Add(outputNode.GetInputJointLabel(i), cogenerator);
            }

            // ジョイントごとの変数宣言コードをマージ
            foreach (CoCodeGenerator cogenerator in m_cogeneratorDict.Values)
            {
                // Macro
                m_macroSet.UnionWith(cogenerator.MacroSet);
                // Include
                m_includeSet.UnionWith(cogenerator.IncludeSet);
                // Uniform
                m_uniformSet.UnionWith(cogenerator.UniformSet);
                // Input
                m_inputSet.UnionWith(cogenerator.InputSet);
            }

            // テンプレートコードの各箇所を置換
            /// テンプレート内の置き換えマーカに合わせて
            /// 生成した文字列で置き換える

            // テンプレートファイル
            string templatePath = Setting.FileSettings.ApplicationFolderPath + TemplateFilePath;
            using (StreamReader templateStream = new StreamReader(templatePath
                , Encoding.GetEncoding("shift_jis")))
            {
                using (StreamWriter outputStream = new StreamWriter(m_memoryStream, Encoding.ASCII))
                {
                    // 置換対象文字の判定用正規表現
                    Regex replaceRegex = new Regex(@"%(\w+)%");

                    // 終端まで読み込む
                    while (templateStream.EndOfStream == false)
                    {
                        string line = templateStream.ReadLine();
                        string replace = "";

                        // 置換対象文字か判定
                        Match match = replaceRegex.Match(line);
                        if (match.Success)
                        {
                            string replaceWord = match.Result("$1");

                            // 置換
                            switch (replaceWord)
                            {
                                case "HEADER":
                                    replace = GetShaderHeaderString();
                                    break;
                                case "MACROS":
                                    replace = GetShaderMacroString();
                                    break;
                                case "INCLUDES":
                                    replace = GetShaderIncludeString();
                                    break;
                                case "UNIFORMS":
                                    replace = GetShaderUniformString();
                                    break;
                                case "PS_INPUT":
                                    replace = GetShaderInputString();
                                    break;
                                default:
                                    // 入力ジョイントに基づく置き換え処理
                                    {
                                        CoCodeGenerator cogenerator = m_cogeneratorDict[replaceWord];
                                        if (cogenerator.ContainCode)
                                        {
                                            replace = cogenerator.MainCode;
                                        }
                                    }
                                    break;
                            }
                        }
                        // 置換対象でなければそのまま出力
                        else
                        {
                            replace = line;
                        }

                        outputStream.WriteLine(replace);
                    }
                }
            }
        }

        /// <summary>
        /// シェーダのヘッダ文字列を取得する
        /// </summary>
        /// <returns></returns>
        private string GetShaderHeaderString()
        {
            StringWriter stream = new StringWriter();
            stream.WriteLine("// ======================================================================");
            stream.WriteLine("// Exported from {0}", App.CurrentApp.FileSettings.CurrentFilePath);
            stream.WriteLine("// Exported Time : {0}", DateTime.Now.ToString());
            stream.WriteLine("// Metashader Version : {0}", Assembly.GetExecutingAssembly().GetName().Version);
            stream.WriteLine("// ======================================================================");

            return stream.ToString();
        }

        /// <summary>
        /// シェーダのマクロ文字列を取得する
        /// </summary>
        /// <returns></returns>
        private string GetShaderMacroString()
        {
            StringWriter stream = new StringWriter();
            foreach (string str in m_macroSet)
            {
                stream.Write(str);
            }

            return stream.ToString();
        }

        /// <summary>
        /// シェーダのインクルード文字列を取得する
        /// </summary>
        /// <returns></returns>
        private string GetShaderIncludeString()
        {
            StringWriter stream = new StringWriter();
            foreach (string str in m_includeSet)
            {
                stream.Write(str);
            }

            return stream.ToString();
        }

        /// <summary>
        /// シェーダのUniform宣言文字列を取得する
        /// </summary>
        private string GetShaderUniformString()
        {
            StringWriter stream = new StringWriter();
            foreach (string str in m_uniformSet)
            {
                stream.Write(str);
            }

            return stream.ToString();
        }

        /// <summary>
        /// シェーダの入力属性宣言文字列を取得する
        /// </summary>
        private string GetShaderInputString()
        {
            StringWriter stream = new StringWriter();
            foreach (string str in m_inputSet)
            {
                stream.Write(str);
            }

            return stream.ToString();
        }
        #endregion
    }

    /// <summary>
    /// 出力ノードのジョイントに関連するコードを出力するクラス
    /// </summary>
    public class CoCodeGenerator : CommonCodeGeneratorBase
    {
        #region variables
        /// <summary>
        /// 最終入力ジョイント
        /// </summary>
        private JointData m_joint;

        /// <summary>
        /// 有効なノードのキュー
        /// ノードが依存性の低い順に並ぶ（入力変数が最も最初となる）
        /// </summary>
        List<ShaderNodeDataBase> m_validNodeQue = new List<ShaderNodeDataBase>();

        /// <summary>
        /// 変数宣言を除く、メインの手続きコードを記述するライター
        /// </summary>
        StringWriter m_mainCodeWriter = new StringWriter();
        #endregion

        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="joint"></param>
        public CoCodeGenerator(JointData joint)
        {
            m_joint = joint;
        }
        #endregion

        #region properties
        /// <summary>
        /// コードを含んでいるか
        /// </summary>
        /// <returns></returns>
        public bool ContainCode
        {
            get { return m_joint.JointList.Count > 0; }
        }

        /// <summary>
        /// マクロ宣言コードのセット
        /// </summary>
        public HashSet<string> MacroSet
        {
            get { return m_macroSet; }
        }

        /// <summary>
        /// Include宣言コードのセット
        /// </summary>
        public HashSet<string> IncludeSet
        {
            get { return m_includeSet; }
        }

        /// <summary>
        /// Uniform宣言コードのセット
        /// </summary>
        public HashSet<string> UniformSet
        {
            get { return m_uniformSet; }
        }

        /// <summary>
        /// 入力属性宣言コードのセット
        /// </summary>
        public HashSet<string> InputSet
        {
            get { return m_inputSet; }
        }

        /// <summary>
        /// 変数宣言を除く、メインの手続きコード
        /// </summary>
        public string MainCode
        {
            get { return m_mainCodeWriter.ToString(); }
        }
        #endregion

        #region public methods
        /// <summary>
        /// コードを生成
        /// </summary>
        public void Generate()
        {
            if (ContainCode == false)
                return;

            // 最終出力ジョイントに接続されている最終ノード
            ShaderNodeDataBase finalNode = m_joint.JointList.First.Value.ParentNode;

            // ジョイントに対応するコードが生成されたことを表すマクロを宣言
            AddMacro("FUNC_" + m_joint.Label);

            // ノードをたどって依存関係に合わせて並び替える
            SortNodes(finalNode);

            // コードを生成
            foreach (ShaderNodeDataBase node in m_validNodeQue)
            {
                // Uniform
                {
                    StringWriter writer = new StringWriter();
                    node.WriteShaderUniformCode(writer);
                    string code = writer.ToString();

                    // 長さがある場合のみ書き込み
                    if (code.Length > 0)
                    {
                        // 書き込み済みならスキップ
                        if (m_uniformSet.Contains(code))
                            continue;

                        // 書き込み
                        m_uniformSet.Add(code);
                    }
                }

                // Input(入力属性）
                {
                    StringWriter writer = new StringWriter();
                    node.WriteShaderInputCode(writer);
                    string code = writer.ToString();

                    // 長さがある場合のみ書き込み
                    if (code.Length > 0)
                    {
                        // 書き込み済みならスキップ
                        if (m_inputSet.Contains(code))
                            continue;

                        // 書き込み
                        m_inputSet.Add(code);
                    }
                }

                // Macro
                {
                    StringWriter writer = new StringWriter();
                    node.WriteShaderMacroCode(writer);
                    string code = writer.ToString();

                    // 長さがある場合のみ書き込み
                    if (code.Length > 0)
                    {
                        // 書き込み済みなら書き込まないが、
                        // スキップはしない
                        if (m_macroSet.Contains(code) == false)
                        {
                            // 書き込み
                            m_macroSet.Add(code);
                        }
                    }
                }

                // メインコード
                {
                    node.WriteShaderMainCode(m_mainCodeWriter);
                }
            }

            // メインコードの最後のreturn用変数への代入
            m_mainCodeWriter.WriteLine("\tret = {0};\r\n", m_joint.VariableName);
        }
        #endregion

        #region private methods
        /// <summary>
        /// ノードを依存関係に合わせて並び替え
        /// </summary>
        private void SortNodes(ShaderNodeDataBase finalNode)
        {
            // 最終ノードが依存するノードを抽出
            List<ShaderNodeDataBase> nodeList = ExtractNodes(finalNode);

            // ノードを依存する順序に従って並び替え
            SortByDependency(nodeList);
        }

        /// <summary>
        /// 最終出力ノードに関連するノード
        /// </summary>
        private List<ShaderNodeDataBase> ExtractNodes(ShaderNodeDataBase finalNode)
        {
            // 上記ノードが依存している全てのノードをさかのぼって列挙する
            Stack<ShaderNodeDataBase> stack = new Stack<ShaderNodeDataBase>();
            stack.Push(finalNode);
            Dictionary<int, ShaderNodeDataBase> nodeMap = new Dictionary<int, ShaderNodeDataBase>(); // すでに格納したか判定するためのもの
            while (stack.Count > 0)
            {
                ShaderNodeDataBase node = stack.Pop();

                // すでに格納済みなら次のノードへ
                if (nodeMap.ContainsKey(node.GetHashCode()))
                {
                    continue;
                }

                // 未格納なのでMapへ格納
                nodeMap.Add(node.GetHashCode(), node);

                // ノードに接続されている入力ノードをスタックへ積む
                for (int i = 0; i < node.InputJointNum; ++i)
                {
                    JointData inputJoint = node.GetInputJoint(i);
                    ShaderNodeDataBase outputNode = inputJoint.JointList.First.Value.ParentNode;
                    stack.Push(outputNode);
                }
            }

            return new List<ShaderNodeDataBase>(nodeMap.Values);
        }

        /// <summary>
        /// 依存度に応じてソートする
        /// </summary>
        /// <param name="nodeList"></param>
        private void SortByDependency(List<ShaderNodeDataBase> nodeList)
        {
            // 依存性カウンタ
            // 各ノード毎の入力リンク数を保存する
            Dictionary<int, int> dependencyCounter = new Dictionary<int, int>();
            foreach (ShaderNodeDataBase node in nodeList)
            {
                dependencyCounter.Add(node.GetHashCode(), node.InputJointNum);
            }

            // 全てのノードが取り除かれるまで、
            // 依存度の低い順にnodeListから取り出していく
            while (nodeList.Count > 0)
            {
                // 取り除くべき、依存性0のノード
                ShaderNodeDataBase removedNode = null;

                foreach (ShaderNodeDataBase node in nodeList)
                {
                    int dependency = dependencyCounter[node.GetHashCode()];

                    // 依存度0ならば取り出す
                    if (dependency == 0)
                    {
                        removedNode = node;
                        break;
                    }
                }
                if (removedNode == null)
                {
                    throw new ArgumentException("依存度0のノードが見つかりませんでした。nodeListのトポロジが不正です");
                }

                // removeNodeをこのクラスのキューに積む
                m_validNodeQue.Add(removedNode);

                // removeNodeからのリンクを入力とするノードの依存度を減らす
                for (int i = 0; i < removedNode.OutputJointNum; ++i)
                {
                    // 各出力ジョイントの接続先の依存度を減らす
                    foreach (JointData joint in removedNode.GetOutputJoint(i).JointList)
                    {
                        ShaderNodeDataBase node = joint.ParentNode;

                        // 含まれるか判定をしているのは、接続先が有効なノードではなく、nodeListに入っていない可能性があるため
                        // 「有効なノードではない」とは、最終出力ノードへたどれないノード
                        if (dependencyCounter.ContainsKey(node.GetHashCode()))
                            dependencyCounter[node.GetHashCode()]--;
                    }
                }

                // nodeListから該当するnodeを削除する                                
                nodeList.Remove(removedNode);
            }
        }
        #endregion
    };

    /// <summary>
    /// マテリアル指定のシェーダを生成するクラス
    /// </summary>
    public class MaterialShaderGenerator : ShaderCodeGeneratorBase
    {
        #region variables
        Setting.MaterialType m_materialType;
        #endregion

        #region constructors
        public MaterialShaderGenerator(Setting.MaterialType materialType, ShaderGraphData graph)
            : base(graph)
        {
            m_materialType = materialType;

            // マクロの初期化
            InitializeMacros();

            // インクルードファイルの初期化
            InitializeIncludeFiles();

            // Uniformの初期化
            InitializeUniforms();

            // 入力属性の初期化
            InitializeInputAttributes();
        }
        #endregion

        #region properties
        /// <summary>
        /// テンプレートファイルを取得する
        /// </summary>
        protected override string TemplateFilePath
        {
            get
            {
                string[] filePathTable = 
                {
                    @"\..\..\data\shader\template\general_material.fx",
                    @"\..\..\data\shader\template\custom_material.fx",                    
                };
                return filePathTable[(int)m_materialType];
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// マクロの初期化
        /// </summary>
        private void InitializeMacros()
        {
            switch (m_materialType)
            {
                case Setting.MaterialType.Phong:
                    // フォン用に方向ライト数を指定
                    AddMacro("DIR_LIGHT_NUM 3");
                    break;
                case Setting.MaterialType.Custom:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// インクルードファイルの初期化
        /// </summary>
        private void InitializeIncludeFiles()
        {
            switch (m_materialType)
            {
                case Setting.MaterialType.Phong:
                    // フォンマテリアル用インクルードファイルの追加                    
                    AddIncludeFilePath("phong_lighting.h");
                    break;
                case Setting.MaterialType.Custom:
                    // カスタムは特にマテリアル必須のインクルードはない                   
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Uniformの初期化
        /// </summary>         
        private void InitializeUniforms()
        {
            switch (m_materialType)
            {
                case Setting.MaterialType.Phong:
                    // フォン用に方向ライトを追加
                    AddUniform("uniform float3 \tUniform_DirLightDir[DIR_LIGHT_NUM];");
                    AddUniform("uniform float3 \tUniform_DirLightCol[DIR_LIGHT_NUM];");
                    // フォン用にカメラ位置を追加
                    AddUniform("uniform float3 \tUniform_CameraPosition;");
                    break;
                case Setting.MaterialType.Custom:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 入力属性の初期化
        /// </summary>
        private void InitializeInputAttributes()
        {
            switch (m_materialType)
            {
                case Setting.MaterialType.Phong:
                    // フォン用に位置・法線を追加
                    AddInputAttribute("float3 Position0 : TEXCOORD0;");
                    AddInputAttribute("float3 Normal0 : TEXCOORD1;");
                    AddInputAttribute("float3 Tangent0 : TEXCOORD3;");
                    AddInputAttribute("float3 BiNormal0 : TEXCOORD4;");
                    break;
                case Setting.MaterialType.Custom:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    };
}
