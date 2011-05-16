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
    [Serializable]
    class ShaderNodeFactory
    {
#region variables
        /// <summary>
        /// ノードに一意の名前を割り当てるための種類ごとのID
        /// この値を生成したノード名のサフィックスとして利用する
        /// </summary>        
        uint[] m_IDCounter = new uint[(int)ShaderNodeType.Max];

        /// <summary>
        /// 各ノードの種類ごとのインスタンス数を保持するカウンタ
        /// グラフにノードが追加されると増え、消されると減る
        /// </summary>
        uint[] m_instanceCounter = new uint[(int)ShaderNodeType.Max];
#endregion                

#region public methods
        /// <summary>
        /// シェーダノードの種類ごとのIDをカウント
        /// </summary>
        /// <param name="type"></param>
        public void IncrementID( ShaderNodeType type )
        {
            ++m_IDCounter[(int)type];
        }

        /// <summary>
        /// シェーダノードの種類ごとのIDをカウント
        /// </summary>
        /// <param name="type"></param>
        public void DecrementID( ShaderNodeType type )
        {
            --m_IDCounter[(int)type];
        }

        /// <summary>
        /// 種類毎のインスタンス数をインクリメント
        /// </summary>
        /// <param name="?"></param>
        public void IncrementInstance( ShaderNodeType type )
        {
            ++m_instanceCounter[(int)type];
        }

        /// <summary>
        /// 種類毎のインスタンス数をデクリメント
        /// </summary>
        /// <param name="?"></param>
        public void DecrementInstance(ShaderNodeType type)
        {
            --m_instanceCounter[(int)type];
        }

        /// <summary>
        /// 指定した種類の新しいノードを作成できるか
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanCreate( ShaderNodeType type )
        {
            // 生成できる最大数以下ならば、作成可能
            return (m_instanceCounter[(int)type] + 1) <= type.GetMaxNodeNum();
        }

        /// <summary>
        /// シェーダノードの具象クラスを作成する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pos"></param>
        /// <returns>作成された具象クラス。作成不可能な場合は、nullを返す</returns>
        public ShaderNodeDataBase Create( ShaderNodeType type, Point pos )
        {
            // 作成不可能な場合は、nullを返す
            if (CanCreate(type) == false)
                return null;

            // ノード名を決定する
            string name = type.ToStringExt() + "_" + m_IDCounter[(int)type];

            ShaderNodeDataBase ret = null;

            // @@@ 具象クラスを作成
            switch(type)
            {                    
                case ShaderNodeType.Uniform_Float:
                    ret = new Uniform_FloatNode(name, pos);
                    break;
                case ShaderNodeType.Uniform_Vector2:
                    ret = new Uniform_Vector2Node(name, pos);
                    break;
                case ShaderNodeType.Uniform_Vector3:
                    ret = new Uniform_Vector3Node(name, pos);
                    break;
                case ShaderNodeType.Uniform_Vector4:
                    ret = new Uniform_Vector4Node(name, pos);
                    break;
                case ShaderNodeType.Uniform_Texture2D:
                    ret = new Uniform_Texture2DNode(name, pos);
                    break;
                case ShaderNodeType.Uniform_TextureCube:
                    ret = new Uniform_TextureCubeNode(name, pos);
                    break;
                case ShaderNodeType.Input_UV:
                    ret = new Input_UVNode(name, pos);
                    break;
                case ShaderNodeType.Input_Normal:
                    ret = new Input_NormalNode(name, pos);
                    break;
                case ShaderNodeType.Input_Position:
                    ret = new Input_PositionNode(name, pos);
                    break;
                case ShaderNodeType.Operator_Add:
                    ret = new Operator_AddNode(name, pos);
                    break;
                case ShaderNodeType.Operator_Sub:
                    ret = new Operator_SubNode(name, pos);
                    break;
                case ShaderNodeType.Operator_Mul:
                    ret = new Operator_MulNode(name, pos);
                    break;
                case ShaderNodeType.Operator_Div:
                    ret = new Operator_DivNode(name, pos);
                    break;
                case ShaderNodeType.Func_Normalize:
                    ret = new Func_Normalize(name, pos);
                    break;
                case ShaderNodeType.Func_Dot:
                    ret = new Func_Dot(name, pos);
                    break;
                case ShaderNodeType.Func_Reflect:
                    ret = new Func_Reflect(name, pos);
                    break;
                case ShaderNodeType.Func_Pow:
                    ret = new Func_Pow(name, pos);
                    break;
                case ShaderNodeType.Func_Saturate:
                    ret = new Func_Saturate(name, pos);
                    break;
                case ShaderNodeType.Light_DirLightDir:
                    ret = new Light_DirLightDirNode(name, pos);
                    break;
                case ShaderNodeType.Light_DirLightColor:
                    ret = new Light_DirLightColorNode(name, pos);
                    break;
                case ShaderNodeType.Camera_Position:
                    ret = new Camera_PositionNode(name, pos);
                    break;
                case ShaderNodeType.Utility_Append:
                    ret = new Utility_AppendNode(name, pos);
                    break;
                case ShaderNodeType.Output_Color:
                    ret = new Output_ColorNode(name, pos);
                    break;                                    
                case ShaderNodeType.Output_Material:
                    ret = new Output_MaterialNode(name, pos);
                    break;
                default:
                    throw new ArgumentException("適合するタイプのコンストラクタが有りません", type.ToStringExt());
            }
            return ret;
        }

        /// <summary>
        /// 外部からの初期化用
        /// </summary>
        public void Reset()
        {
            // メンバ変数の初期化
            m_IDCounter = new uint[(int)ShaderNodeType.Max];            
            m_instanceCounter = new uint[(int)ShaderNodeType.Max];
        }
#endregion
    }
}
