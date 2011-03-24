using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace metashader.ShaderGraphData
{
#region uniform nodes
    /// <summary>
    /// 4Dベクトル
    /// RGBAカラーとしても利用
    /// </summary>   
    class Uniform_Vector4Node : ShaderNodeDataBase
    {
#region constructors
        public Uniform_Vector4Node(string name, Point pos)
            : base( ShaderNodeType.Uniform_Vector4, name, pos, 
            0,   // 入力ジョイント（このノードが入力なので0）
            4    // 出力ジョイント（x,y,z,w）
        )
        {

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
    }

#endregion    

}
