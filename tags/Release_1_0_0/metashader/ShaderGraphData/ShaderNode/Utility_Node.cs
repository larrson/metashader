///
/// 便利系ノード群の定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// ベクトルを合成するノード
    /// </summary>
    [Serializable]
    class Utility_AppendNode : ShaderNodeDataBase
    {
        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pos"></param>
        public Utility_AppendNode(string name, Point pos)
            : base(ShaderNodeType.Utility_Append, name, pos)
        {

        }
        #endregion

        #region properties        
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

            // 接続されていない場合は不定
            if (GetInputJoint(index).JointList.Count == 0)
            {
                return VariableType.INDEFINITE;
            }
            // 接続されていれば接続元に依存
            else
            {
                JointData outputJoint = GetInputJoint(index).JointList.First.Value;
                ShaderNodeDataBase node = outputJoint.ParentNode;

                return node.GetOutputJointVariableType(outputJoint.JointIndex);
            }
        }

        /// <summary>
        /// 出力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetOutputJointVariableType(int index)
        {
            // 自身の入力ジョイントに依存する

            // 無効な状態なら未定義を返す
            if( IsValid() == false )
            {                
                return VariableType.INDEFINITE;
            }

            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);
            
            // 有効なら次元数に応じて
            VariableType ret = 
                (VariableType)( VariableType.FLOAT + typeLeft.GetDimension() + typeRight.GetDimension() - 1);

            // ベクトル型で次元数が一致している事を確認
            Debug.Assert( ret.IsVector() && (ret.GetDimension() == typeLeft.GetDimension() + typeRight.GetDimension()));

            return ret;
        }

        /// <summary>
        /// ノードの有効性を判定
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            // 入力リンクの有効性を確認する
            // 全ての入力が埋まっているか？
            foreach (JointData inputJoint in m_inputJoints)
            {
                if (inputJoint.JointList.Count != 1)
                    return false;
            }

            // 入力型が適当か
            // 2つの入力型がベクトルないし、スカラーで、
            // 足しあわせた次元数が4を超えない

            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);
            bool isValid = 
                (typeLeft.IsScalar() || typeLeft.IsVector())
                && (typeRight.IsScalar() || typeRight.IsVector())
                && (typeLeft.GetDimension() + typeRight.GetDimension() <= 4);            

            return isValid;
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderMainCode(StringWriter stream)
        {
            // 2つの数値型を合成して新たなベクトルを作成する
            VariableType outputType = GetOutputJointVariableType(0);
            stream.WriteLine("\t{0} {1} = {0}( {2}, {3} );"
                , outputType.ToStringExt()
                , VariableName
                , GetInputJoint(0).VariableName
                , GetInputJoint(1).VariableName
                );        
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Append"; }
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
}
