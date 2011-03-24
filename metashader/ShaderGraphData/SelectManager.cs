using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// データ選択管理クラス
    /// </summary>
    public class SelectManager
    {
#region variables
        /// <summary>
        /// 選択されているシェーダノードのリスト
        /// </summary>
        List<ShaderNodeDataBase> m_shaderNodes = new List<ShaderNodeDataBase>();
#endregion

#region properties
        public ReadOnlyCollection<ShaderNodeDataBase> SelectedNodeList
        {
            get { return m_shaderNodes.AsReadOnly();  }
        }
#endregion

#region public methods
        /// <summary>
        /// シェーダノードの
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsSelected( ShaderNodeDataBase node )
        {
            bool ret = false;

            // 選択中リストから探す
            foreach ( ShaderNodeDataBase it in m_shaderNodes )
            {
                if( Object.ReferenceEquals(it, node) )
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// 対象オブジェクトを選択
        /// </summary>
        /// <param name="node"></param>
        public void Select( ShaderNodeDataBase node )
        {
            // 他の選択を解除
            Inner_Clear();

            // 選択追加
            m_shaderNodes.Add(node);            

            //@@ 必要ならイベント処理
        }

        /// <summary>
        /// 全選択の解除
        /// </summary>
        public void Clear()
        {
            Inner_Clear();

            //@@ 必要ならイベント処理
        }
#endregion

#region private methods
        /// <summary>
        /// 全選択の解除
        /// </summary>
        public void Inner_Clear()
        {
            m_shaderNodes.Clear();
        }
#endregion
    }
}
