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

        /// <summary>
        /// ノードが追加された際に起動するイベント
        /// </summary>
        public event NodeAddedEventHandler NodeAddedEvent;

        /// <summary>
        /// ノードが削除された際に起動するイベント
        /// </summary>
        public event NodeDeletedEventHandler NodeDeletedEvent;

        /// <summary>
        /// リンクが追加された際に起動するイベント
        /// </summary>
        public event LinkAddedEventHandler LinkAddedEvent;

        /// <summary>
        /// リンクが削除された際に起動するイベント
        /// </summary>
        public event LinkDeletedEventHandler LinkDeletedEvent;

        /// <summary>
        /// グラフ構造からのエラー報告に使用するイベント
        /// エラーがない場合もその旨を報告する
        /// </summary>
        public event GraphErrorEventHandler GraphErrorEvent;
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
        /// ノードのプロパティ変更イベントの起動
        /// </summary>
        public void RaiseNodePropertyChanged(object sender, NodePropertyChangedEventArgs args)
        {           
            if( NodePropertyChangedEvent != null )
            {
                NodePropertyChangedEvent(sender, args);
            }
        }
        
        /// <summary>
        /// ノード追加イベントの起動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RaiseNodeAdded(object sender, NodeAddedEventArgs args)
        {
            if( NodeAddedEvent != null )
            {
                NodeAddedEvent(sender, args);
            }
        }

        /// <summary>
        /// ノード削除イベントの起動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RaiseNodeDeleted(object sender, NodeDeletedEventArgs args)
        {
            if (NodeDeletedEvent != null)
            {
                NodeDeletedEvent(sender, args);
            }
        }

        /// <summary>
        /// リンク追加イベントの起動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RaiseLinkAdded(object sender, LinkAddedEventArgs args)
        {
            if(LinkAddedEvent != null)
            {
                LinkAddedEvent(sender, args);
            }
        }

        /// <summary>
        /// リンク削除イベントの起動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RaiseLinkDeleted(object sender, LinkDeletedEventArgs args)
        {
            if (LinkDeletedEvent != null)
            {
                LinkDeletedEvent(sender, args);
            }
        }

        /// <summary>
        /// グラフエラー報告イベントの起動
        /// </summary>
        public void RaiseGraphError(object sender, GraphErrorEventArgs args)
        {
            if( GraphErrorEvent != null)
            {
                GraphErrorEvent(sender, args);
            }
        }
#endregion

#region event handlers        
#endregion 
    }
}
