using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace metashader.PropertyEditor.Parts
{
    public interface IPartsControl
    {
        void UpdateTarget();
    }

    public interface IParts
    {
        ShaderGraphData.ShaderNodeDataBase NodeData
        {
            set;
            get;
        }

        UserControl UserControl
        {
            get;
        }
    }

    /// <summary>
    /// プロパティエディタの部品
    /// </summary>
    public class Parts<ParameterType> : IParts
    {
        /// <summary>
        /// ノードデータへの値の設定、取得を行うオブジェクト
        /// </summary>
        ValueController<ParameterType> m_valueController;

        /// <summary>
        /// コントロール
        /// </summary>
        UserControl m_userControl;

#region constructors
        public Parts(string propertyName, UserControl control )
        {
            m_valueController = new ValueController<ParameterType>(propertyName);
            
            m_userControl = control;            
        }
#endregion

#region properties      
        public UserControl UserControl
        {
            get { return m_userControl; }
        }

        public ShaderGraphData.ShaderNodeDataBase NodeData
        {
            set
            {
                m_valueController.NodeData = value;
                if( m_userControl.DataContext == null )
                    m_userControl.DataContext = m_valueController;
            }
            get 
            {
                return m_valueController.NodeData;
            }
        }
#endregion
    }
}
