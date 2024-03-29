﻿///
/// 最終出力ノードの定義
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
    /// 定義済みマテリアル出力ノード
    /// Phong等の定義済みマテリアルで使用
    /// WritingShaderXXXCodeが無いのは、入力ジョイントとその接続先に対してコードを生成するため
    /// （このノード自体が生成するシェーダコードは無い）。
    /// </summary>
    [Serializable]
    class Output_MaterialNode : ShaderNodeDataBase
    {
        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        public Output_MaterialNode(string name, Point pos)
            : base("Output_Material", name, pos)
        {

        }
#endregion

#region properties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Output Material"; }
        }
#endregion

#region public methods      
        /// <summary>
        /// 有効なノードか判定する
        /// @@ エラーメッセージの付加       
        /// </summary>
        public override bool IsValid()
        {
            // 入力リンクが埋まっていなくとも構わない

            // 入力リンクの型が繋がっている出力の型と一致しているか
            foreach (JointData inputJoint in m_inputJoints)
            {
                // つながっていなければスキップ
                if (inputJoint.JointList.Count == 0)
                    continue;

                // 入力の型を求める
                VariableType inputType = GetInputJointVariableType(inputJoint.JointIndex);

                // 出力の型を求める
                JointData outputJoint = inputJoint.JointList.First.Value;
                ShaderNodeDataBase outputNode = outputJoint.ParentNode;
                VariableType outputType = outputNode.GetOutputJointVariableType(outputJoint.JointIndex);

                if (inputType != outputType)
                {
                    return false;
                }
            }

            return true;
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
            m_inputJointNum = 6;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.FLOAT3, JointData.SuffixType.None, "Diffuse");            
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.FLOAT3, JointData.SuffixType.None, "Specular");            
            m_inputJoints[2] = new JointData(this, 2, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.None, "SpecularPower");            
            m_inputJoints[3] = new JointData(this, 3, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.None, "Opacity");            
            m_inputJoints[4] = new JointData(this, 4, JointData.Side.In, VariableType.FLOAT3, JointData.SuffixType.None, "Normal");
            m_inputJoints[5] = new JointData(this, 5, JointData.Side.In, VariableType.FLOAT3, JointData.SuffixType.None, "CustomColor");            

            // 出力            
            m_outputJointNum = 0;
            m_outputJoints = new JointData[m_outputJointNum];            
        }
#endregion
    }
}
