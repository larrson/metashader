using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.CodeDom.Compiler;
using System.Reflection;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// シェーダノードのファクトリ
    /// </summary>   
    [Serializable]
    class ShaderNodeFactory
    {
#region memeber classes
        /// <summary>
        /// シリアライズ用データ構造
        /// </summary>
        [Serializable]
        class SaveData
        {
            public List<KeyValuePair<string, uint>> m_IDCounterList = new List<KeyValuePair<string, uint>>();
            public List<KeyValuePair<string, uint>> m_instanceCounterList = new List<KeyValuePair<string, uint>>();            
        };                
#endregion

#region variables
        /// <summary>
        /// ノードに一意の名前を割り当てるための種類ごとのID
        /// この値を生成したノード名のサフィックスとして利用する
        /// </summary>        
        [NonSerialized]
        Dictionary<string, uint> m_IDCounter = new Dictionary<string, uint>();

        /// <summary>
        /// 各ノードの種類ごとのインスタンス数を保持するカウンタ
        /// グラフにノードが追加されると増え、消されると減る
        /// </summary>
        [NonSerialized]
        Dictionary<string, uint> m_instanceCounter = new Dictionary<string, uint>();

        /// <summary>
        /// シェーダノードの有効なタイプのリスト
        /// </summary>        
        [NonSerialized]
        Dictionary<string, Type> m_shaderNodeTypeList;
#endregion                

#region constructors
        public ShaderNodeFactory()
        {
            // 有効なノードのタイプを初期化
            InitializeValidTypes();
        }
#endregion

#region prpoerties
        public ReadOnlyCollection<string> ValidNodeTypeList
        {
            get {
                List<string> nodeNameList = new List<string>( m_shaderNodeTypeList.Keys );
                return nodeNameList.AsReadOnly();
            } 
        }
#endregion

#region public methods
        /// <summary>
        /// シェーダノードの種類ごとのIDをカウント
        /// </summary>
        /// <param name="type"></param>
        public void IncrementID( string type )
        {                                    
            ++m_IDCounter[type];                      
        }

        /// <summary>
        /// シェーダノードの種類ごとのIDをカウント
        /// </summary>
        /// <param name="type"></param>
        public void DecrementID( string type )
        {
            --m_IDCounter[type];
        }

        /// <summary>
        /// 種類毎のインスタンス数をインクリメント
        /// </summary>
        /// <param name="?"></param>
        public void IncrementInstance( string type )
        {
            ++m_instanceCounter[type];
        }

        /// <summary>
        /// 種類毎のインスタンス数をデクリメント
        /// </summary>
        /// <param name="?"></param>
        public void DecrementInstance(string type)
        {
            --m_instanceCounter[type];
        }

        /// <summary>
        /// 指定した種類の新しいノードを作成できるか
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanCreate( string type )
        {
            // 有効な名前か判定
            if( m_shaderNodeTypeList.ContainsKey(type) == false )
            {
                return false;
            }

            // 生成できる最大数以下ならば、作成可能            
            // @@ タイプをstringにしたため、別途処理する必要がある
            bool ret = !(type == "Output_Material" && m_instanceCounter[type] >= 1);
            return ret;
        }

        /// <summary>
        /// シェーダノードの具象クラスを作成する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <returns>作成された具象クラス。作成不可能な場合は、nullを返す</returns>
        public ShaderNodeDataBase Create( string type, Point pos )
        {
            // 作成不可能な場合は、nullを返す
            if (CanCreate(type) == false)
                return null;

            // ノード名を決定する
            string name = type + "_" + m_IDCounter[type];

            ShaderNodeDataBase ret = null;

            // ノードの種類に応じて具象クラスを作成
            Type t = m_shaderNodeTypeList[type];
            Object[] args = { name, pos };  
            ret = Activator.CreateInstance(t, args) as ShaderNodeDataBase;
            
            Debug.Assert(ret != null);
            // 指定したタイプと、生成されたノードのタイプは等しい
            Debug.Assert(type == ret.Type);

            return ret;
        }

        /// <summary>
        /// 外部からの初期化用
        /// </summary>
        public void Reset()
        {
            // メンバ変数の初期化
            foreach (string type in m_shaderNodeTypeList.Keys)
            {
                m_IDCounter[type] = 0;
            }
            foreach (string type in m_shaderNodeTypeList.Keys)
            {
                m_instanceCounter[type] = 0;
            }                    
        }

        /// <summary>
        /// ファイルへの保存
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public bool Save(FileStream fileStream, BinaryFormatter formatter)
        {
            // 保存用データ作成
            SaveData data = new SaveData();
            
            // IDのカウンタを保存
            foreach( KeyValuePair<string, uint> pair in m_IDCounter )
            {
                data.m_IDCounterList.Add(pair);
            }

            // インスタンスのカウンタを保存
            foreach ( KeyValuePair<string, uint> pair in m_instanceCounter )
            {
                data.m_instanceCounterList.Add(pair);
            }            

            // 保存用データをシリアライズ
            formatter.Serialize(fileStream, data);

            return true;
        }

        /// <summary>
        /// ファイルからロード
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public bool Load(FileStream fileStream, BinaryFormatter formatter)
        {
            // ロード前にリセット
            Reset();

            // セーブデータ読み込み
            SaveData data = formatter.Deserialize(fileStream) as SaveData;

            /// クラス内のデータを復元 ///

            // IDのカウンタを復元
            foreach (KeyValuePair<string, uint> pair in data.m_IDCounterList)
            {
                m_IDCounter[pair.Key] = pair.Value;
            }

            // インスタンスのカウンタを復元
            foreach (KeyValuePair<string, uint> pair in data.m_instanceCounterList)
            {
                m_instanceCounter[pair.Key] = pair.Value;
            }

            return true;
        }
#endregion

#region private methods
        /// <summary>
        /// タイプの初期化処理
        /// </summary>
        public void InitializeValidTypes()
        {                        
            m_shaderNodeTypeList = new Dictionary<string, Type>();

            // サポート済みのノード            
            AddValidType("Uniform_Float", Type.GetType("metashader.ShaderGraphData.Uniform_FloatNode"));                   
            AddValidType("Uniform_Vector2", Type.GetType("metashader.ShaderGraphData.Uniform_Vector2Node"));                    
            AddValidType("Uniform_Vector3", Type.GetType("metashader.ShaderGraphData.Uniform_Vector3Node"));                    
            AddValidType("Uniform_Vector4", Type.GetType("metashader.ShaderGraphData.Uniform_Vector4Node"));                    
            AddValidType("Uniform_Texture2D", Type.GetType("metashader.ShaderGraphData.Uniform_Texture2DNode"));                    
            AddValidType("Uniform_TextureCube", Type.GetType("metashader.ShaderGraphData.Uniform_TextureCubeNode"));                    
            AddValidType("Input_UV", Type.GetType("metashader.ShaderGraphData.Input_UVNode"));                    
            AddValidType("Input_Normal", Type.GetType("metashader.ShaderGraphData.Input_NormalNode"));                    
            AddValidType("Input_Position", Type.GetType("metashader.ShaderGraphData.Input_PositionNode"));                    
            AddValidType("Input_Reflection", Type.GetType("metashader.ShaderGraphData.Input_ReflectionNode"));                    
            AddValidType("Operator_Add", Type.GetType("metashader.ShaderGraphData.Operator_AddNode"));                    
            AddValidType("Operator_Sub", Type.GetType("metashader.ShaderGraphData.Operator_SubNode"));                    
            AddValidType("Operator_Mul", Type.GetType("metashader.ShaderGraphData.Operator_MulNode"));                    
            AddValidType("Operator_Div", Type.GetType("metashader.ShaderGraphData.Operator_DivNode"));                    
            AddValidType("Func_Normalize", Type.GetType("metashader.ShaderGraphData.Func_Normalize"));                    
            AddValidType("Func_Dot", Type.GetType("metashader.ShaderGraphData.Func_Dot"));                    
            AddValidType("Func_Reflect", Type.GetType("metashader.ShaderGraphData.Func_Reflect"));                    
            AddValidType("Func_Pow", Type.GetType("metashader.ShaderGraphData.Func_Pow"));                    
            AddValidType("Func_Saturate", Type.GetType("metashader.ShaderGraphData.Func_Saturate"));                    
            AddValidType("Light_DirLightDir", Type.GetType("metashader.ShaderGraphData.Light_DirLightDirNode"));                    
            AddValidType("Light_DirLightColor", Type.GetType("metashader.ShaderGraphData.Light_DirLightColorNode"));                    
            AddValidType("Camera_Position", Type.GetType("metashader.ShaderGraphData.Camera_PositionNode"));                    
            AddValidType("Utility_Append", Type.GetType("metashader.ShaderGraphData.Utility_AppendNode"));                    
            AddValidType("Output_Material", Type.GetType("metashader.ShaderGraphData.Output_MaterialNode"));


            // 外部ファイルに定義されている型を取得
            //コンパイルするための準備
            CodeDomProvider cp = new Microsoft.CSharp.CSharpCodeProvider(
                    new Dictionary<string, string>
                    {
                        { "CompilerVersion", "v3.5" }
                    }
                );
            CompilerParameters cps = new CompilerParameters();
            CompilerResults cres;
            //メモリ内で出力を生成する
            cps.GenerateInMemory = true;
            // 参照の追加
            // using
            cps.ReferencedAssemblies.Add("System.dll");
            cps.ReferencedAssemblies.Add("System.Core.dll");
            cps.ReferencedAssemblies.Add("WindowsBase.dll");

            // 自分自身 
            cps.ReferencedAssemblies.Add(Path.Combine(Setting.FileSettings.ApplicationFolderPath, "metashader.exe"));
            //コンパイルする
            cres = cp.CompileAssemblyFromFile(cps
                , Setting.FileSettings.ApplicationFolderPath + @"\..\..\data\script\shadernode\userdefined.cs");

            if (cres.Errors.Count > 0)
            {
                StringBuilder stringbuilder = new StringBuilder();
                foreach (CompilerError error in cres.Errors)
                {
                    stringbuilder.AppendLine(error.FileName + "(" + error.Line + ")" + " : " + error.ErrorText);
                }
                MessageBox.Show(stringbuilder.ToString());
                throw new Exception("Compile error in userdefined Shader Node.");
            }

            //コンパイルしたアセンブリを取得
            Assembly asm = cres.CompiledAssembly;
            // タイプを取得            
            foreach (Type type in asm.GetTypes())
            {
                // 種類の名前を取得
                object[] attributes = type.GetCustomAttributes(typeof(ShaderNodeTypeNameAttribute), false);
                Debug.Assert(attributes.Length == 1);
                string typeName = (attributes[0] as ShaderNodeTypeNameAttribute).TypeName;

                // ノードのリストに追加
                AddValidType(typeName, type);
            }
        }

        /// <summary>
        /// 有効なノードタイプを追加する
        /// タイプの初期化時にのみ呼ぶメソッド
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="type"></param>
        public void AddValidType( string typeName, Type type )
        {            
            // タイプのリストに追加
            m_shaderNodeTypeList.Add(typeName, type);

            // IDカウンターに含まれていなければ追加
            if( m_IDCounter.ContainsKey(typeName) == false )
            {
                m_IDCounter.Add(typeName, 0);
            }

            // インスタンスカウンターに含まれていなければ追加
            if (m_instanceCounter.ContainsKey(typeName) == false)
            {
                m_instanceCounter.Add(typeName, 0);
            }
        }
#endregion
    }
}
