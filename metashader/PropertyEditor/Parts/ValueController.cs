using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace metashader.PropertyEditor.Parts
{
    /// <summary>
    /// パーツが内部で利用する値の取得・設定用クラス
    /// </summary>
    public class ValueController<ParameterType>
    {
#region variables
        /// <summary>
        /// ノードデータ
        /// </summary>
        ShaderGraphData.ShaderNodeDataBase m_nodeData = null;

        /// <summary>
        /// リフレクションを利用するためのPropetyInfoクラス
        /// </summary>
        PropertyInfo m_propertyInfo;

        /// <summary>
        /// プロパティ名
        /// </summary>
        string m_propertyName;
#endregion        

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>        
        /// <param name="propertyName"></param>
        public ValueController(string propertyName)
        {            
            m_propertyName = propertyName;            
        }
#endregion

#region properties
        /// <summary>
        /// ノードデータの値
        /// </summary>
        public ParameterType Value
        {
            get
            {
                return (ParameterType)(m_propertyInfo.GetValue(m_nodeData, null));
            }
            set
            {
                // 値の変更はUndoRedoを考慮

                // Undo/Redoバッファ
                ShaderGraphData.UndoRedoBuffer undoredo = new ShaderGraphData.UndoRedoBuffer();

                // 新しいプロパティを設定
                if (m_nodeData != null)
                {
                    App.CurrentApp.GraphData.ChangeNodeProperty<ParameterType>(m_nodeData, m_propertyName, value, undoredo);

                    if (undoredo.IsValid)
                    {
                        ShaderGraphData.UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                    }
                }    
            }
        }

        /// <summary>
        /// ノードデータ
        /// </summary>
        public ShaderGraphData.ShaderNodeDataBase NodeData
        {
            set 
            {
                m_nodeData = value;

                if( m_nodeData != null )
                {
                    // プロパティのメタ情報の取得
                    Type NodeType = m_nodeData.GetType();
                    m_propertyInfo = NodeType.GetProperty(m_propertyName);
                }                
            }
            get
            {
                return m_nodeData;
            }
        }
#endregion
    }
}
