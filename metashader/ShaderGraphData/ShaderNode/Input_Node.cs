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
    /// 入力UV座標のノード
    /// </summary>
    [Serializable]
    class Input_UVNode : Indexed_NodeBase
    {
        #region constructors
        public Input_UVNode(string name, Point pos)
            : base(ShaderNodeType.Input_UV, name, pos)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// ノードの変数名
        /// </summary>
        public override string VariableName
        {
            get { return "In.Texcoord0"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "UV" + Index; }
        }
        #endregion

        #region public methods                
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

    /// <summary>
    /// 入力法線ベクトルのノード
    /// </summary>
    [Serializable]
    class Input_NormalNode : Indexed_NodeBase
    {
        #region constructors
        public Input_NormalNode(string name, Point pos)
            : base(ShaderNodeType.Input_Normal, name, pos)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// ノードの変数名
        /// </summary>
        public override string VariableName
        {
            get { return "In.Normal0"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Normal" + Index; }
        }
        #endregion

        #region public methods        
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
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);            
        }
        #endregion
    }

    /// <summary>
    /// 入力ワールド位置ノード
    /// </summary>
    [Serializable]
    class Input_PositionNode : Indexed_NodeBase
    {
        #region constructors
        public Input_PositionNode(string name, Point pos)
            : base(ShaderNodeType.Input_Position, name, pos)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// インデックス
        /// </summary>
        public override uint Index
        {
            get { return 0; }
            set { m_index = value; }
        }

        /// <summary>
        /// ノードの変数名
        /// </summary>
        public override string VariableName
        {
            get { return "In.Position0"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Position" + Index; }
        }
        #endregion

        #region public methods        
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
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);            
        }
        #endregion
    }

    /// <summary>
    /// 入力反射ベクトルノード
    /// 反射の計算は、入力法線とカメラ位置からシェーダ内の初期化処理で求めるので明示的な計算用シェーダコードをここでは指定しない
    /// そのかわり、必要なカメラ位置をUniformとして指定する
    /// </summary>
    [Serializable]
    class Input_ReflectionNode : ShaderNodeDataBase
    {
        #region constructors
        public Input_ReflectionNode(string name, Point pos)
            : base(ShaderNodeType.Input_Reflection, name, pos)
        {
        }
        #endregion

        #region properties        
        /// <summary>
        /// ノードの変数名
        /// </summary>
        public override string VariableName
        {
            get { return "In.Reflection0"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Reflection Vector"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのマクロを書きこむ
        /// </summary>
        public override void WriteShaderMacroCode(StringWriter stream)
        {
            stream.WriteLine("#define UNIFORM_CameraPosition");
        }

        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            stream.WriteLine("uniform float3 \tUniform_CameraPosition;");
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
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);
        }
        #endregion
    }    
}
