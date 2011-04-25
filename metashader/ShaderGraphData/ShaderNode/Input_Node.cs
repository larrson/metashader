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
    /// 入力ノードの基底クラス
    /// </summary>
    [Serializable]
    abstract class Input_NodeBase : ShaderNodeDataBase
    {
        #region variables
        /// <summary>
        /// セマンティックスに付随するインデックス
        /// </summary>
        uint m_index;
        #endregion

        #region constructors
        public Input_NodeBase(ShaderNodeType type, string name, Point pos)
            : base(type, name, pos)
        {
            // 入力ノードであることを確認
            Debug.Assert(type.IsInputNode());

            m_index = 0;
        }
        #endregion

        #region properties
        /// <summary>
        /// セマンティックに付随するインデックス
        /// </summary>
        public uint Index
        {
            get { return m_index; }
            set { m_index = value; }
        }
        #endregion
    }

    /// <summary>
    /// 入力UV座標のノード
    /// </summary>
    [Serializable]
    class Input_UVNode : Input_NodeBase
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
            get { return "In.Texcoord" + Index; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダの入力属性を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderInputCode(StringWriter stream)
        {
            stream.WriteLine("\tfloat2 Texcoord{0} : TEXCOORD{0};", Index);
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

    /// <summary>
    /// 入力法線ベクトルのノード
    /// </summary>
    [Serializable]
    class Input_NormalNode : Input_NodeBase
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
            get { return "In.Normal" + Index; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダの入力属性を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderInputCode(StringWriter stream)
        {
            stream.WriteLine("\tfloat3 Normal{0} : NORMAL{0};", Index);
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
            m_outputJointNum = 4;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);
            m_outputJoints[1] = new JointData(this, 1, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.X);
            m_outputJoints[2] = new JointData(this, 2, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Y);
            m_outputJoints[3] = new JointData(this, 3, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Z);
        }
        #endregion
    }    
}
