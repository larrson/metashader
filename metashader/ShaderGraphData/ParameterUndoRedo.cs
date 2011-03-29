using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// パラメータのUndoRedoクラス
    /// プロパティを設定するノードと、設定したいプロパティ名から
    /// リフレクションを使用して、実行時に指定したプロパティへアクセスする
    /// </summary>
    public class ParameterUndoRedo<ParameterType>: IUndoRedo     
    {
        /// <summary>
        /// パラメータを設定する対象のノード
        /// </summary>
        ShaderNodeDataBase m_node;

        /// <summary>
        /// 設定前の古いパラメータ
        /// </summary>
        ParameterType m_oldValue;

        /// <summary>
        /// 設定後の新しいパラメータ
        /// </summary>
        ParameterType m_newValue;

        /// <summary>
        /// リフレクションを利用するためのPropetyInfoクラス
        /// </summary>
        PropertyInfo m_propertyInfo;

        public ParameterUndoRedo(ShaderNodeDataBase node, string propertyName,  ParameterType newValue)
        {
            m_node = node;
            m_newValue = newValue;

            // プロパティのメタ情報の取得
            Type NodeType = node.GetType();
            m_propertyInfo = NodeType.GetProperty(propertyName);

            // プロパティを利用して変更前の値を取得する
            m_oldValue = (ParameterType)(m_propertyInfo.GetValue(node, null));                        
        }

#region override methods
        /// <summary>
        /// もとに戻す
        /// </summary>
        public void Undo()
        {
            // プロパティを利用して古い値を設定する
            m_propertyInfo.SetValue(m_node, m_oldValue, null);
        }

        /// <summary>
        /// やり直し
        /// </summary>
        public void Redo()
        {
            Do();
        }
#endregion

#region public methods
        /// <summary>
        /// 値変更を実行する        
        /// </summary>
        /// <returns>値の変更が実行されたか</returns>
        public bool Do()
        {
            // 変更前と後の値が変わらなければ実行しない
            if( m_newValue.Equals( m_oldValue ) )
            {
                return false;
            }

            // プロパティを利用して新しい値を設定する            
            m_propertyInfo.SetValue(m_node, m_newValue, null);            

            return true;
        }
#endregion
    }
}
