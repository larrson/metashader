using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using metashader.Common;

namespace metashader.PropertyEditor.Parts
{
    /// <summary>
    /// パーツが内部で利用する値の取得・設定用クラス
    /// </summary>
    public abstract class ValueController<ParameterType>
    {
#region variables
        /*
        /// <summary>
        /// ノードデータ
        /// </summary>
        ShaderGraphData.ShaderNodeDataBase m_nodeData = null;
         */

        /// <summary>
        /// プロパティ名
        /// </summary>
        protected string m_propertyName;

        /// <summary>
        /// リフレクションを利用するためのPropetyInfoクラス
        /// </summary>
        protected PropertyInfo m_propertyInfo;        
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
        /// プロパティ値
        /// </summary>
        public abstract ParameterType Value
        {
            get; set;
        }

        /// <summary>
        /// プロパティの設定先ターゲット
        /// </summary>
        public abstract object Target
        {
            get;
        }       

        /// <summary>
        /// プロパティ名
        /// </summary>
        public string PropertyName
        {
            get { return m_propertyName; }
        }
#endregion
    }

    public class ValueControllerSub<TargetType, ParameterType> : ValueController<ParameterType>
        where TargetType : class
    {
#region variables
        TargetType m_target;

        delegate void ChangePropertyDelegate(ParameterType value, UndoRedoBuffer undoredo);

        ChangePropertyDelegate m_changeProperty;
#endregion

#region constructors
        public ValueControllerSub(TargetType target, string propertyName)
            : base(propertyName)
        {
            // 値設定先のデータを設定
            m_target = target;

            // プロパティ情報を初期化
            Type targetType = m_target.GetType();
            m_propertyInfo = targetType.GetProperty(m_propertyName);

            // ターゲット型によってデリゲートを変更            
            // シェーダノードの場合
            if( targetType.IsSubclassOf(Type.GetType("metashader.ShaderGraphData.ShaderNodeDataBase")) )
            {
                m_changeProperty = (value, undoredo) =>
                {
                    ShaderGraphData.ShaderNodeDataBase node = m_target as ShaderGraphData.ShaderNodeDataBase;
                    App.CurrentApp.GraphData.ChangeNodeProperty<ParameterType>(node, m_propertyName, value, undoredo);
                };
            }
            // グローバル設定の場合
            else if( Type.GetType("metashader.Setting.GlobalSettings").IsInstanceOfType(m_target) )
            {
                m_changeProperty = (value, undoredo) =>
                {
                    App.CurrentApp.GlobalSettings.ChangeProperty<ParameterType>(m_propertyName, value, undoredo);
                };
            }
        }
#endregion

#region properties
        public override ParameterType Value
        {
            get
            {
                return (ParameterType)(m_propertyInfo.GetValue(m_target, null));
            }
            set
            {
                // 値の変更はUndoRedoを考慮

                // Undo/Redoバッファ
                UndoRedoBuffer undoredo = new UndoRedoBuffer();

                // 新しいプロパティを設定                
                m_changeProperty(value, undoredo);

                if (undoredo.IsValid)
                {
                    UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }                
            }
        }

        /// <summary>
        /// パーツから値が設定されるターゲットオブジェクト
        /// </summary>
        public override object Target
        {
            get
            {
                return m_target;
            }
        }
#endregion
    }
}
