﻿using System;
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
                case ShaderNodeType.Uniform_Vector4:
                    ret = new Uniform_Vector4Node(name, pos);
                    break;
                case ShaderNodeType.Operator_Add:
                    ret = new Operator_AddNode(name, pos);
                    break;
                case ShaderNodeType.Output_Color:
                    ret = new Output_ColorNode(name, pos);
                    break;                
                default:
                    throw new ArgumentException("適合するタイプのコンストラクタが有りません", type.ToStringExt());
            }
            return ret;
        }
#endregion
    }
}
