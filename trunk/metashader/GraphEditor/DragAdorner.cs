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
        protected UIElement _child;

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
            // ゴーストの描画要素
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
                var adorner = this.Parent as AdornerLayer;
                if (adorner != null)
                {
                    adorner.Update(this.AdornedElement);
                }
            }
        }
#endregion

#region protected methods
        protected override Visual GetVisualChild(int index)
        {
            return _child;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size finalSize)
        {
            _child.Measure(finalSize);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(_child.DesiredSize));
            return finalSize;
        }

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
