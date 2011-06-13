using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// 並行光源の方向ノード   
    /// </summary>   
    [Serializable]
    class Light_DirLightDirNode : Indexed_NodeBase, IAppliableParameter
    {
        #region variables        
        #endregion

        #region constructors
        public Light_DirLightDirNode(string name, Point pos)
            : base("Light_DirLightDir", name, pos)
        {
        }
        #endregion

        #region properties         
        /// <summary>
        /// 変数名
        /// </summary>
        public override string VariableName
        {
            get {
                return "Uniform_DirLightDir[" + Index + "]";
            }
        }

        /// <summary>
        /// インデックスの最大値
        /// @@ Previewer側へ問い合わせるべき
        /// </summary>
        public override uint MaximumIndex
        {
            get { return 2; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Dir Light Dir "+ Index; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへマクロを書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderMacroCode(StringWriter stream)
        {
            // ライト数を宣言する
            //@ 数を動的に変更
            stream.WriteLine("#define DIR_LIGHT_NUM 3");
        }

        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // uniformを宣言する
            stream.WriteLine("uniform float3 \tUniform_DirLightDir[DIR_LIGHT_NUM];"); // 方向            
        }        

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {            
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
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);            
        }
        #endregion
    }

    /// <summary>
    /// 並行光源の色ノード   
    /// </summary>   
    [Serializable]
    class Light_DirLightColorNode : Indexed_NodeBase, IAppliableParameter
    {
        #region variables        
        #endregion

        #region constructors
        public Light_DirLightColorNode(string name, Point pos)
            : base("Light_DirLightColor", name, pos)
        {
        }
        #endregion

        #region properties        
        /// <summary>
        /// 変数名
        /// </summary>
        public override string VariableName
        {
            get
            {
                return "Uniform_DirLightCol[" + Index + "]";
            }
        }

        /// <summary>
        /// インデックスの最大値
        /// @@ Previewer側へ問い合わせるべき
        /// </summary>
        public override uint MaximumIndex
        {
            get { return 2; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Dir Light Color " + Index; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへマクロを書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderMacroCode(StringWriter stream)
        {
            // ライト数を宣言する
            //@ 数を動的に変更
            stream.WriteLine("#define DIR_LIGHT_NUM 3");
        }

        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // uniformを宣言する
            stream.WriteLine("uniform float3 \tUniform_DirLightCol[DIR_LIGHT_NUM];"); // 色
        }        

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {            
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
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT3, JointData.SuffixType.None);
        }
        #endregion
    }    
}
