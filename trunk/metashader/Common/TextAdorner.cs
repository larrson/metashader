using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace metashader.Common
{
    /// <summary>
    /// 装飾としてテキストを表示するアドーナー
    /// </summary>
    class TextAdorner : Adorner
    {
        #region variables       
        string m_text = "";
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="adornedElement"></param>
        public TextAdorner(UIElement adornedElement)
            : base(adornedElement)
        {

        }

        #region properties      
        /// <summary>
        /// 表示するテキスト
        /// </summary>
        public string Text 
        {
            set 
            { 
                if( m_text != value )
                {
                    m_text = value;
                    InvalidateVisual();
                }                
            }
            get { return m_text; }
        }
        #endregion

        #region override methods
        /// <summary>
        /// 描画のオーバーライド
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.DrawText(
                new FormattedText(
                    Text
                    , Thread.CurrentThread.CurrentCulture
                    , FlowDirection.LeftToRight
                    , new Typeface("メイリオ")
                    , 14
                    , Brushes.Black
                    )
                    , new Point(6, 6)
                );
            drawingContext.DrawText(
                new FormattedText(
                    Text
                    , Thread.CurrentThread.CurrentCulture
                    , FlowDirection.LeftToRight
                    , new Typeface("メイリオ")
                    , 14
                    , Brushes.Red
                    )
                    , new Point( 5, 5 )
                );
        }
        #endregion
    }
}
