using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.Event
{
    /// <summary>
    /// イベント管理クラス
    /// </summary>
    public class EventManager
    {
#region variables 
        /// <summary>
        /// ノードのプロパティが変更された際に起動するイベント        
        /// </summary>
        public event NodePropertyChangedEventHandler NodePropertyChangedEvent;
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EventManager()
        {            
        }        
#endregion        

#region public methods
        /// <summary>
        /// プロパティ変更イベントの起動
        /// </summary>
        public void RaiseNodePropertyChanged(object sender, NodePropertyChangedEventArgs args)
        {           
            if( NodePropertyChangedEvent != null )
            {
                NodePropertyChangedEvent(sender, args);
            }
        }
#endregion

#region event handlers        
#endregion 
    }
}
