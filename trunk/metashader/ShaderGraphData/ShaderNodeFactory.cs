using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// シェーダノードのファクトリ
    /// </summary>    
    class ShaderNodeFactory
    {
#region variables
        /// <summary>
        /// ノードに一意の名前を割り当てるための種類ごとのカウンタ
        /// カウンタの値を生成したノード名のサフィックスとして利用する
        /// </summary>
        uint[] m_typeCounter = new uint[(int)ShaderNodeType.Max];
#endregion                

#region public methods
        /// <summary>
        /// シェーダノードの種類ごとのカウンタをインクリメント
        /// </summary>
        /// <param name="type"></param>
        public void IncrementCounter( ShaderNodeType type )
        {
            ++m_typeCounter[(int)type];
        }

        /// <summary>
        /// シェーダノードの種類ごとのカウンタをデクリメント
        /// </summary>
        /// <param name="type"></param>
        public void DecrementCounter( ShaderNodeType type )
        {
            --m_typeCounter[(int)type];
        }

        /// <summary>
        /// シェーダノードの具象クラスを作成する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public ShaderNodeDataBase Create( ShaderNodeType type, Point pos )
        {
            // ノード名を決定する
            string name = type.ToStringExt() + "_" + m_typeCounter[(int)type];

            ShaderNodeDataBase ret = null;

            // @@@ 具象クラスと作成
            switch(type)
            {
                case ShaderNodeType.Uniform_Vector4:
                    ret = new Uniform_Vector4Node(name, pos);
                    break;
                case ShaderNodeType.Output_Color:
                    ret = new Output_ColorNode(name, pos);
                    break;
                default:
                    throw new ArgumentException("適合するタイプのコンストラクタが有りません", "type");
            }
            return ret;
        }
#endregion
    }
}
