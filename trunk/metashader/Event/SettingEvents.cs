///
/// 各種設定項目が変更された際に呼ばれるイベントの定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.Event
{
    /// <summary>
    /// グローバル設定のプロパティが変更された際に起動するイベントの引数
    /// </summary>
    public class GlobalSettingPropertyChangedEventArgs : EventArgs
    {
#region variables
        /// <summary>
        /// プロパティ名
        /// </summary>
        string m_propertyName;

        /// <summary>
        /// 変更後の値
        /// </summary>
        object m_newValue;
#endregion

        #region properties        
        /// <summary>
        /// 変更されたプロパティ名
        /// </summary>
        public string PropertyName
        {
            get { return m_propertyName; }
        }

        /// <summary>
        /// 新しい値を取得する
        /// </summary>
        public object NewValue
        {
            get
            {
                return m_newValue;
            }
        }
        #endregion

        #region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>                
        /// <param name="propertyName">変更対象のプロパティ名</param>
        /// <param name="newValue">変更後の値</param>
        public GlobalSettingPropertyChangedEventArgs(string propertyName, object newValue)
        {            
            m_propertyName = propertyName;
            m_newValue = newValue;
        }
        #endregion
    }

    /// <summary>
    /// グローバル設定プロパティ変更デリゲータの宣言
    /// </summary>
    /// <param name="sender">送信元</param>
    /// <param name="args">イベント引数</param>
    public delegate void GlobalSettingPropertyChangedEventHandler(object sender, GlobalSettingPropertyChangedEventArgs args);
}
