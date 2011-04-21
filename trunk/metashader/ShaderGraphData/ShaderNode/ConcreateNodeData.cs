using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{
#region uniform nodes
    /// <summary>
    /// Previewerへのパラメータ適用メソッドを実装するためのインターフェース
    /// </summary>    
    public interface IAppliableParameter
    {
        void ApplyParameter();
    }

    /// <summary>
    /// 4Dベクトル
    /// RGBAカラーとしても利用
    /// </summary>   
    [Serializable]
    class Uniform_Vector4Node : ShaderNodeDataBase, IAppliableParameter
    {
#region variables
        float[] m_values = new float[4];
#endregion

#region constructors
        public Uniform_Vector4Node(string name, Point pos)
            : base( ShaderNodeType.Uniform_Vector4, name, pos)
        {
            m_values[0] = 0.0f;
            m_values[1] = 0.0f;
            m_values[2] = 0.0f;
            m_values[3] = 0.0f;
        }
#endregion

#region properties
        /// <summary>
        /// 4Dベクトル値
        /// </summary>
        public float[] Values
        {
            get { return m_values;  }
            set 
            { 
                if( value.Length != 4 )
                {
                    throw new ArgumentException("Valueへ設定する配列のサイズは4で無ければなりません");
                }
                m_values = value;                                 
            }
        }

        /// <summary>
        /// X成分
        /// </summary>
        public float X
        {
            get { return m_values[0]; }
            set
            {
                m_values[0] = value;
            }
        }

        /// <summary>
        /// Y成分
        /// </summary>
        public float Y
        {
            get { return m_values[1]; }
            set
            {
                m_values[1] = value;
            }
        }

        /// <summary>
        /// Z成分
        /// </summary>
        public float Z
        {
            get { return m_values[2]; }
            set
            {
                m_values[2] = value;
            }
        }

        /// <summary>
        /// W成分
        /// </summary>
        public float W
        {
            get { return m_values[3]; }
            set
            {
                m_values[3] = value;
            }
        }
#endregion

#region public methods        
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderUniformCode(StringWriter stream)         
        {
            // この4Dベクトルのuniformを宣言する
            stream.WriteLine("uniform float4 \t{0};", Name);
        }

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {
            NativeMethods.SetUniformVector4(Name, m_values[0], m_values[1], m_values[2], m_values[3]);
        }
#if DEBUG   
        /// <summary>
        /// デバッグ用のコンソールへの情報表示
        /// </summary>
        public override void DebugPrint()
        {
            // 基底クラスの同メソッドを呼び出す
            base.DebugPrint();

            System.Console.WriteLine("\tvalues:{0}, {1}, {2}, {3}", m_values[0].ToString(), m_values[1].ToString(), m_values[2].ToString(), m_values[3].ToString());
        }
#endif // DEBUG

#endregion

#region protected methods
        /// <summary>
        /// ジョイントの初期化
        /// </summary>
        protected override void InitializeJoints()
        {
            // ジョイントの初期化
            // 入力         
            m_inputJointNum = 0;
            m_inputJoints = new JointData[m_inputJointNum];            
            // 出力            
            m_outputJointNum = 5;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT4, JointData.SuffixType.None);
            m_outputJoints[1] = new JointData(this, 1, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.X);
            m_outputJoints[2] = new JointData(this, 2, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Y);
            m_outputJoints[3] = new JointData(this, 3, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Z);
            m_outputJoints[4] = new JointData(this, 4, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.W);            
        }
#endregion
    }        
#endregion

#region input nodes
    /// <summary>
    /// 入力UV座標のノード
    /// </summary>
    [Serializable]
    class Input_UVNode : ShaderNodeDataBase
    {
#region variables
        /// <summary>
        /// セマンティックに付随するインデックス
        /// </summary>
        uint m_index;
#endregion

#region constructors
        public Input_UVNode(string name, Point pos)
            : base( ShaderNodeType.Input_UV, name, pos )            
        {
            m_index = 0;
        }
#endregion

#region properties
        /// <summary>
        /// セマンティックに付随するインデックス
        /// </summary>
        public uint Index
        {
            get { return m_index;  }
            set { m_index = value; }
        }

        /// <summary>
        /// ノードの変数名
        /// </summary>
        public override string VariableName
        {
            get { return "In.Texcoord" + m_index; }
        }
#endregion

#region public methods        
        /// <summary>
        /// ストリームへシェーダの入力属性を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderInputCode(StringWriter stream)
        {
            stream.WriteLine("\tfloat2 Texcoord{0} : TEXCOORD{0};", m_index);
        }
#endregion

#region protected methods
        /// <summary>
        /// ジョイントの初期化
        /// </summary>
        protected override void InitializeJoints()
        {
            // 入力
            m_inputJointNum = 0;
            m_inputJoints = new JointData[m_inputJointNum];

            // 出力
            m_outputJointNum = 3;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT2, JointData.SuffixType.None);
            m_outputJoints[1] = new JointData(this, 1, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.X);
            m_outputJoints[2] = new JointData(this, 2, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Y);
        }
#endregion
    }    
#endregion

#region operator nodes
    /// <summary>
    /// 加算ノード
    /// </summary>
    [Serializable]
    class Operator_AddNode : ShaderNodeDataBase
    {
#region constructors
        public Operator_AddNode(string name, Point pos)
            : base(ShaderNodeType.Operator_Add, name, pos)
        {

        }
#endregion

#region public methods
        /// <summary>
        /// 入力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetInputJointVariableType(int index)
        {
            // 接続元ノードの出力ジョイントに依存する
            JointData outputJoint = GetInputJoint( index ).JointList.First.Value;
            ShaderNodeDataBase node = outputJoint.ParentNode;

            return node.GetOutputJointVariableType(outputJoint.JointIndex);
        }

        /// <summary>
        /// 出力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetOutputJointVariableType(int index)
        {
            // 自身の入力ジョイントに依存する            
            // 「加算」なので、入力ジョイントの型＝出力ジョイントの型
            // インデックスはどれでも良い

            return GetInputJointVariableType(0);
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="localCount">ローカル変数のカウンタ</param>
        public override void WritingShaderMainCode(StringWriter stream)
        {
            // 出力型
            VariableType outputType = GetOutputJointVariableType( 0 );         

            // 入力変数1の名前
            string inputName1 = GetInputJoint(0).VariableName;
            // 入力変数2の名前
            string inputName2 = GetInputJoint(1).VariableName;

            stream.WriteLine("\t{0} {1} = {2} + {3};", 
                outputType.ToStringExt()
                , Name
                , inputName1
                , inputName2
                );
        }        
#endregion

#region protected methods
        /// <summary>
        /// ジョイントの初期化
        /// </summary>
        protected override void InitializeJoints()
        {
            // ジョイントの初期化
            // 入力         
            m_inputJointNum = 2;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None);
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None);
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.DEPENDENT, JointData.SuffixType.None);            
        }
#endregion
    }
#endregion

#region output nodes
    /// <summary>
    /// 出力色ノード
    /// </summary>
    [Serializable]
    class Output_ColorNode : ShaderNodeDataBase
    {
#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        public Output_ColorNode(string name, Point pos)
            : base( ShaderNodeType.Output_Color, name, pos)
        {

        }
#endregion

#region public methods       
        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderMainCode(StringWriter stream)
        {
#if false // 4入力バージョン
            stream.WriteLine("\treturn float4({0},{1},{2},{3});"
                , GetInputJoint(0).VariableName
                , GetInputJoint(1).VariableName
                , GetInputJoint(2).VariableName
                , GetInputJoint(3).VariableName);
#else
            stream.WriteLine("\treturn {0};", GetInputJoint(0).VariableName);
#endif 
        } 
#endregion

#region protected methods
        /// <summary>
        /// ジョイントの初期化
        /// </summary>
        protected override void InitializeJoints()
        {
            // ジョイントの初期化
            // 入力   
#if false // 4入力バージョン
            m_inputJointNum = 4;
            m_inputJoints = new JointData[m_inputJointNum];           
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.X);
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.Y);
            m_inputJoints[2] = new JointData(this, 2, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.Z);
            m_inputJoints[3] = new JointData(this, 3, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.W);
#else
            m_inputJointNum = 1;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.FLOAT4, JointData.SuffixType.None);            
#endif 

            // 出力            
            m_outputJointNum = 0;
            m_outputJoints = new JointData[m_outputJointNum];            
        }
#endregion
    }

#endregion    

}
