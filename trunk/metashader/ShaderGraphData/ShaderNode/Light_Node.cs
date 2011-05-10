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
    class Light_DirLightDirNode : ShaderNodeDataBase, IAppliableParameter
    {
        #region variables
        /// <summary>
        /// 並行光源のインデックス
        /// </summary>
        int m_index = 0;
        #endregion

        #region constructors
        public Light_DirLightDirNode(string name, Point pos)
            : base(ShaderNodeType.Light_DirLightDir, name, pos)
        {
        }
        #endregion

        #region properties 
        /// <summary>
        /// 平行光源のインデックス
        /// </summary>
        public int Index
        {
            get { return m_index; }
            set { m_index = value;  }
        }

        /// <summary>
        /// 変数名
        /// </summary>
        public override string VariableName
        {
            get {
                return "Local_DirLightDir_" + Index; 
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
            // uniformを宣言する
            stream.WriteLine("uniform float3 \tUniform_DirLightDir_{0};", Index); // 方向            
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WritingShaderMainCode(StringWriter stream) 
        {            
            // シェーダ本文
            // 計算の都合上あえて反転させる
            stream.WriteLine("\tfloat3 \t{0} = -Uniform_DirLightDir_{1};", VariableName, Index);        
        }

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {
            // @@@@ previewer側のライトの有効化
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
    class Light_DirLightColorNode : ShaderNodeDataBase, IAppliableParameter
    {
        #region variables
        /// <summary>
        /// 並行光源のインデックス
        /// </summary>
        int m_index = 0;
        #endregion

        #region constructors
        public Light_DirLightColorNode(string name, Point pos)
            : base(ShaderNodeType.Light_DirLightColor, name, pos)
        {
        }
        #endregion

        #region properties
        /// <summary>
        /// 平行光源のインデックス
        /// </summary>
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }

        /// <summary>
        /// 変数名
        /// </summary>
        public override string VariableName
        {
            get
            {
                return "Uniform_DirLightColor_" + Index;
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
            // uniformを宣言する
            stream.WriteLine("uniform float3 \t{0};", VariableName); // 方向            
        }        

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {
            // @@@@ previewer側のライトの有効化
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
