//////////////////////////////////////////////////////////////////////////
// ユーザー定義のノード
// スクリプト言語のように、編集後のアプリケーション本体のコンパイルが不要
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// フレネルノード
    /// </summary>
    [Serializable]   
    [ShaderNodeTypeName("User_Fresnel")]
    class User_FresnelNode : ShaderNodeDataBase
    {   
#region constructors
        public User_FresnelNode(string name, Point pos)
            : base("User_Fresnel", name, pos)
        {

        }
#endregion

#region properties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Fresnel"; }
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
            // カメラ位置の宣言を有効化
            stream.WriteLine("#define UNIFORM_CameraPosition");
        }

        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WriteShaderUniformCode(StringWriter stream)
        {
            // uniformを宣言する
            stream.WriteLine("uniform float3 \tUniform_DirLightDir[DIR_LIGHT_NUM];"); // ライト方向            
            stream.WriteLine("uniform float3 \tUniform_CameraPosition;"); // カメラ方向        
        } 

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderMainCode(StringWriter stream)
        {
            // 出力型
            VariableType outputType = GetOutputJointVariableType(0);

            // 入力変数の名前
            string n = GetInputJoint(0).VariableName;
            string k = GetInputJoint(1).VariableName;

            stream.WriteLine("\t{0} {1} = CalcFresnel({2}, {3}, In.CameraDir0, Uniform_DirLightDir[0] ) / dot(In.Normal0, In.CameraDir0);",                
                outputType.ToStringExt()
                , Name                              
                , n
                , k
                );
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

            // 入力型が適当か(演算子に即しているか)            
            // 型がスカラー型か否か
            return GetInputJointVariableType(0).IsScalar() && GetInputJointVariableType(1).IsScalar();
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
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.None, "n");
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.FLOAT, JointData.SuffixType.None, "k");            
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.None);
        }
#endregion
    }
}