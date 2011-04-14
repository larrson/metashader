using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace metashader.GraphEditor
{
    /// <summary>
    /// ベジエ曲線クラス
    /// </summary>
    public class BezierCurve
    {
        #region variable
        /// <summary>
        /// パス
        /// </summary>
        Path m_path;
        /// <summary>
        /// パス内の図形
        /// </summary>
        PathFigure m_pathFigure;
        /// <summary>
        /// 図形内のPathセグメント
        /// </summary>
        BezierSegment m_bezierSegment;

        /// <summary>
        /// 曲線の開始位置
        /// </summary>
        Point m_startPos;

        /// <summary>
        /// 曲線の終了位置
        /// </summary>
        Point m_endPos;
        #endregion

#region properties
        /// <summary>
        /// パス
        /// </summary>
        public Path Path
        {
            get { return m_path; }
        }

        /// <summary>
        /// 可視性
        /// </summary>
        public Visibility Visibility
        {
            set { Path.Visibility = value; }
        }

        /// <summary>
        /// 開始位置
        /// </summary>
        public Point StartPos
        {
            set 
            {
                m_startPos = value;
                UpdateControlPoints();
            }
        }

        /// <summary>
        /// 終了位置
        /// </summary>
        public Point EndPos
        {
            set
            {
                m_endPos = value;
                UpdateControlPoints();
            }
        }

        /// <summary>
        /// 曲専用制御点1
        /// </summary>
        private Point ControlPoint1
        {
            get
            {
                double x = (m_endPos.X - m_startPos.X) * 0.25 + m_startPos.X;
                double y = m_startPos.Y;

                return new Point(x, y);
            }
        }

        /// <summary>
        /// 曲専用制御点2
        /// </summary>
        private Point ControlPoint2
        {
            get
            {
                double x = (m_endPos.X - m_startPos.X) * 0.75 + m_startPos.X;
                double y = m_endPos.Y;

                return new Point(x, y);
            }
        }                
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BezierCurve()
        {
            // パスの作成
            m_path = new Path();
            m_path.Stroke = Brushes.Black;
            m_path.StrokeThickness = 1;

            m_pathFigure = new PathFigure();
            m_bezierSegment = new BezierSegment();
            m_pathFigure.Segments = new PathSegmentCollection();
            m_pathFigure.Segments.Add(m_bezierSegment);

            m_path.Data = new PathGeometry();
            (m_path.Data as PathGeometry).Figures = new PathFigureCollection();
            (m_path.Data as PathGeometry).Figures.Add(m_pathFigure);
        }
#endregion

        #region private methods
        /// <summary>
        /// パスの制御点を更新する
        /// </summary>
        private void UpdateControlPoints()
        {
            m_pathFigure.StartPoint = m_startPos;
            m_bezierSegment.Point1 = ControlPoint1;
            m_bezierSegment.Point2 = ControlPoint2;
            m_bezierSegment.Point3 = m_endPos;
        }
        #endregion
    }
}
