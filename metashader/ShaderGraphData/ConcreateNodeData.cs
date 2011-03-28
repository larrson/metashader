using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{
#region uniform nodes
    /// <summary>
    /// 4Dベクトル
    /// RGBAカラーとしても利用
    /// </summary>   
    [Serializable]
    class Uniform_Vector4Node : ShaderNodeDataBase
    {
#region variables
        float[] values = new float[4];
#endregion

#region constructors
        public Uniform_Vector4Node(string name, Point pos)
            : base( ShaderNodeType.Uniform_Vector4, name, pos, 
            0,   // 入力ジョイント（このノードが入力なので0）
            4    // 出力ジョイント（x,y,z,w）
        )
        {
            values[0] = 1.0f;
            values[1] = 0.0f;
            values[2] = 0.0f;
            values[3] = 1.0f;
        }
#endregion

#region public methods
        /// <summary>
        /// 指定した入力ジョイントのパラメータ型を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ParameterType GetInputJointParameterType(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定した出力ジョイントのパラメータ型を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ParameterType GetOuputJointParameterType(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderUniformCode(StringWriter stream)         
        {
            // この4Dベクトルのuniformを宣言する
            stream.WriteLine("uniform float4 \t{0};", Name);
        }        
#endregion
    }

#endregion

#region input nodes
#endregion

#region operator nodes
#endregion

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
            : base( ShaderNodeType.Output_Color, name, pos, 
            4, // 入力ジョイント（r,g,b,a）
            0  // 出力ジョイント (このノードがピクセルシェーダの出力なので0)
            )
        {

        }
#endregion

#region public methods
        /// <summary>
        /// 指定した入力ジョイントのパラメータ型を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ParameterType GetInputJointParameterType(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定した出力ジョイントのパラメータ型を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ParameterType GetOuputJointParameterType(int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderMainCode(StringWriter stream)
        {
            stream.WriteLine("\treturn float4({0},{1},{2},{3});"
                , GetInputJoint(0).VariableName
                , GetInputJoint(1).VariableName
                , GetInputJoint(2).VariableName
                , GetInputJoint(3).VariableName);
        } 
#endregion
    }

#endregion    

}
