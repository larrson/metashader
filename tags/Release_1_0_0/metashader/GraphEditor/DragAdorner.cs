using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace metashader.GraphEditor
{
    /// <summary>
    /// 指定したUIElementのドラッグ用ゴーストを表示させるアドーナー
    /// </summary>
    public class DragAdorner : Adorner
    {
        /// <summary>
        /// 装飾用エレメント
        /// </summary>
        protected UIElement _child;

        /// <summary>
        /// 表示位置
        /// </summary>
        Point _pos;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">オーナー</param>
        /// <param name="adornElement">装飾対象エレメント</param>
        /// <param name="opacity">ゴーストの透明度</param>
        public DragAdorner(UIElement owner, UIElement adornElement, double opacity)
            : base( owner )
        {            
            // ゴーストの描画要素を初期化
            var brush = new VisualBrush(adornElement) { Opacity = opacity };
            var Bound = VisualTreeHelper.GetDescendantBounds(adornElement);
            var Rect = new Rectangle() { Width = Bound.Width, Height = Bound.Height };

            Rect.Fill = brush;
            _child = Rect;
        }

#region properties
        /// <summary>
        /// 表示位置
        /// </summary>
        public Point Position
        {
            set
            { 
                _pos = value;

                // 表示位置を反映
                var adorner = this.Parent as AdornerLayer;
                if (adorner != null)
                {
                    adorner.Update(this.AdornedElement);
                }
            }
        }
#endregion

#region protected methods
        /// <summary>
        /// 描画要素を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            // 装飾用UIElementを返す
            return _child;
        }

        /// <summary>
        /// 描画要素数を返す
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        /// <summary>
        /// 望ましいサイズを返す
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size finalSize)
        {
            // 描画要素の望ましいサイズを利用
            _child.Measure(finalSize);
            return _child.DesiredSize;
        }

        /// <summary>
        /// レイアウト処理
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // 描画要素のレイアウト処理をそのまま利用
            _child.Arrange(new Rect(_child.DesiredSize));
            return finalSize;
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_pos.X, _pos.Y));
            return result;
        }
#endregion
    }
}
