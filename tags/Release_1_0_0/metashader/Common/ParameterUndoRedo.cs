using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace metashader.Common
{
    /// <summary>
    /// パラメータのUndoRedoクラス
    /// プロパティを設定するノードと、設定したいプロパティ名から
    /// リフレクションを使用して、実行時に指定したプロパティへアクセスする
    /// </summary>
    public class ParameterUndoRedo<ParameterType, TargetType>: IUndoRedo     
    {
        /// <summary>
        /// 設定対象
        /// </summary>
        TargetType m_target;

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

        /// <summary>
        /// プロパティ名
        /// </summary>
        string m_propertyName;

        public ParameterUndoRedo(TargetType target, string propertyName,  ParameterType newValue)
        {
            m_target = target;
            m_newValue = newValue;
            m_propertyName = propertyName;

            // プロパティのメタ情報の取得
            Type TargetType = m_target.GetType();
            m_propertyInfo = TargetType.GetProperty(propertyName);

            // プロパティを利用して変更前の値を取得する
            m_oldValue = (ParameterType)(m_propertyInfo.GetValue(m_target, null));                        
        }

#region override methods
        /// <summary>
        /// もとに戻す
        /// </summary>
        public void Undo()
        {
            // プロパティを利用して古い値を設定する
            m_propertyInfo.SetValue(m_target, m_oldValue, null);

            // ノードのプロパティ変更イベントを起動
            RaisePropertyChangeEvent(m_oldValue);            
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
        /// 保持している値と異なる場合のみ設定する
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
            m_propertyInfo.SetValue(m_target, m_newValue, null);

            // ノードのプロパティ変更イベントを起動
            RaisePropertyChangeEvent(m_newValue);            

            return true;
        }        
#endregion

#region private methods
        /// <summary>
        /// プロパティ変更イベントを起こす
        /// </summary>
        private void RaisePropertyChangeEvent( ParameterType value )
        {
            // C#は部分的な特殊化ができないので強引に場合分け
            Type TargetType = m_target.GetType();

            // シェーダグラフのノード
            if( TargetType.IsSubclassOf(Type.GetType("metashader.ShaderGraphData.ShaderNodeDataBase")) )
            {
                ShaderGraphData.ShaderNodeDataBase node = m_target as ShaderGraphData.ShaderNodeDataBase;
                App.CurrentApp.EventManager.RaiseNodePropertyChanged(this, new Event.NodePropertyChangedEventArgs(node, m_propertyName, value));
            }
            // グローバル設定
            if( TargetType.IsInstanceOfType(App.CurrentApp.GlobalSettings))
            {
                Setting.GlobalSettings globalSettings = m_target as Setting.GlobalSettings;
                App.CurrentApp.EventManager.RaiseGlobalSettingPropertyChanged(this, new Event.GlobalSettingPropertyChangedEventArgs(m_propertyName, value));
            }
        }
#endregion
    }
}
