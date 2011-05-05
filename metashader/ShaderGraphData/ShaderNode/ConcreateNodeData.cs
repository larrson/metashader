using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;

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

    [Serializable]
    class Uniform_FloatNode : ShaderNodeDataBase, IAppliableParameter
    {
#region variables
        float m_value = 0.0f;
#endregion

#region constructors
        public Uniform_FloatNode(string name, Point pos)
            : base( ShaderNodeType.Uniform_Float, name, pos )
        {        
        }
#endregion

#region properties
        /// <summary>
        /// 浮動小数点値
        /// </summary>
        public float Value
        {
            get { return m_value; }
            set { m_value = value;}
        }
#endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderUniformCode(StringWriter stream)
        {
            // このスカラー値のuniformを宣言する
            stream.WriteLine("uniform float \t{0};", Name);
        }

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {            
            NativeMethods.SetUniformFloat(Name, m_value);
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
            m_inputJointNum = 0;
            m_inputJoints = new JointData[m_inputJointNum];
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.None);                 
        }
        #endregion
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
            m_values[3] = 1.0f; // w成分を1に設定
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
