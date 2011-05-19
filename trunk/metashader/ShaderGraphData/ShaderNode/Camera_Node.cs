///
/// カメラ関連のノード定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// カメラ位置ノード
    /// </summary>
    [Serializable]
    class Camera_PositionNode : ShaderNodeDataBase
    {
        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        public Camera_PositionNode(string name, Point pos)
            : base(ShaderNodeType.Camera_Position, name, pos)
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
                return "Uniform_Camera_Position";
            }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Camera Position"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // uniformを宣言する
            stream.WriteLine("uniform float3 \t{0};", VariableName); // 方向            
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
