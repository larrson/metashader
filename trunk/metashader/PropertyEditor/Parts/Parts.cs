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
        /// <summary>
        /// パーツに関連付けられているコントロール
        /// </summary>
        Control Control
        {
            get;
        }       

        /// <summary>
        /// パーツから値が設定されるターゲットオブジェクト
        /// </summary>
        object Target
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
        /// 値の設定、取得を行うオブジェクト
        /// </summary>
        ValueController<ParameterType> m_valueController;        

        /// <summary>
        /// コントロール
        /// </summary>
        Control m_control;                

#region constructors
        /// <summary>
        /// シェーダーノード用パーツ
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="propertyName"></param>
        /// <param name="control"></param>
        public Parts(ShaderGraphData.ShaderNodeDataBase nodeData, string propertyName, Control control )
        {
            m_valueController = new ValueControllerSub<ShaderGraphData.ShaderNodeDataBase, ParameterType>(nodeData, propertyName);

            // 初期化処理
            Initialize(control);
        }

        /// <summary>
        /// グローバル設定用パーツ
        /// </summary>
        /// <param name="globalSettings"></param>
        /// <param name="propertyName"></param>
        /// <param name="control"></param>
        public Parts(Setting.GlobalSettings globalSettings, string propertyName, Control control)
        {
            m_valueController = new ValueControllerSub<Setting.GlobalSettings, ParameterType>(globalSettings, propertyName);

            // 初期化処理
            Initialize(control);
        }
#endregion

#region properties      
        /// <summary>
        /// パーツに関連付けられているユーザーコントロール
        /// </summary>
        public Control Control
        {
            get { return m_control; }
        }

        /// <summary>
        /// パーツから値が設定されるターゲットオブジェクト
        /// </summary>
        public object Target
        {
            get { return m_valueController.Target; }
        }
#endregion

#region private methods
        /// <summary>
        /// 共通初期化処理
        /// </summary>
        /// <param name="control"></param>
        private void Initialize( Control control )
        {
	        m_control = control;
            m_control.DataContext = m_valueController;
        }
#endregion
    }
}
