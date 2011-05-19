///
/// 数値型のUniformノード定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace metashader.ShaderGraphData
{   
    /// <summary>
    /// float型のスカラーノード
    /// </summary>
    [Serializable]
    class Uniform_FloatNode : ShaderNodeDataBase, IAppliableParameter
    {
        #region variables
        float m_value = 0.0f;
        #endregion

        #region constructors
        public Uniform_FloatNode(string name, Point pos)
            : base(ShaderNodeType.Uniform_Float, name, pos)
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
            set { m_value = value; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "( " + Value.ToString("F") + " )"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
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
    /// 2Dベクトルノード
    /// </summary>
    [Serializable]
    class Uniform_Vector2Node : ShaderNodeDataBase, IAppliableParameter
    {
        #region variables
        float[] m_values = new float[2];
        #endregion

        #region constructors
        public Uniform_Vector2Node(string name, Point pos)
            : base(ShaderNodeType.Uniform_Vector2, name, pos)
        {
            m_values[0] = m_values[1] = 0.0f;
        }
        #endregion

        #region properties        
        /// <summary>
        /// 2Dベクトル値
        /// </summary>
        public float[] Values
        {
            get { return m_values; }
            set
            {
                if (value.Length != 2)
                {
                    throw new ArgumentException("Valueへ設定する配列のサイズは2で無ければなりません");
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
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "( " + X.ToString("F") + ", " + Y.ToString("F") + " )"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // このスカラー値のuniformを宣言する
            stream.WriteLine("uniform float2 \t{0};", Name);
        }

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {            
            NativeMethods.SetUniformVector4(Name, Values[0], Values[1], 0.0f, 0.0f);
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
            m_outputJointNum = 3;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT2, JointData.SuffixType.None);
            m_outputJoints[1] = new JointData(this, 1, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.X);
            m_outputJoints[2] = new JointData(this, 2, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Y);
        }
        #endregion
    }

    /// <summary>
    /// 3Dベクトルノード
    /// </summary>
    [Serializable]
    class Uniform_Vector3Node : ShaderNodeDataBase, IAppliableParameter
    {
        #region variables
        float[] m_values = new float[3];
        #endregion

        #region constructors
        public Uniform_Vector3Node(string name, Point pos)
            : base(ShaderNodeType.Uniform_Vector3, name, pos)
        {
            m_values[0] = m_values[1] = m_values[2] = 0.0f;
        }
        #endregion

        #region properties
        /// <summary>
        /// 3Dベクトル値
        /// </summary>
        public float[] Values
        {
            get { return m_values; }
            set
            {
                if (value.Length != 3)
                {
                    throw new ArgumentException("Valueへ設定する配列のサイズは3で無ければなりません");
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
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "( " + X.ToString("F") + ", " + Y.ToString("F") + ", " + Z.ToString("F") + " )"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // このスカラー値のuniformを宣言する
            stream.WriteLine("uniform float3 \t{0};", Name);
        }

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {            
            NativeMethods.SetUniformVector4(Name, Values[0], Values[1], Values[2], 0.0f);
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
            m_outputJointNum = 4;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);
            m_outputJoints[1] = new JointData(this, 1, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.X);
            m_outputJoints[2] = new JointData(this, 2, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Y);
            m_outputJoints[3] = new JointData(this, 3, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Z);
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
            : base(ShaderNodeType.Uniform_Vector4, name, pos)
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
            get { return m_values; }
            set
            {
                if (value.Length != 4)
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

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "( " + X.ToString("F") + ", " + Y.ToString("F") + ", " + Z.ToString("F") + ", " + W.ToString("F") + " )"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
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
}
