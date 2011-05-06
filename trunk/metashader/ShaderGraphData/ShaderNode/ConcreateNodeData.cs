﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace metashader.ShaderGraphData
{   
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