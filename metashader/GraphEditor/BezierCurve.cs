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
            get { return m_startPos; }
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
            get { return m_endPos; }
        }

        /// <summary>
        /// 曲専用制御点1
        /// </summary>
        private Point ControlPoint1
        {
            get
            {
                double x = (m_endPos.X - m_startPos.X) * 0.25;
                double y = 0;

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
                double x = (m_endPos.X - m_startPos.X) * 0.75;
                double y = m_endPos.Y - m_startPos.Y;

                return new Point(x, y);
            }
        }   
        
        /// <summary>
        /// 幅
        /// </summary>
        public double Width
        {
            get { return Math.Abs(m_startPos.X - m_endPos.X); }
        }

        /// <summary>
        /// 高さ
        /// </summary>
        public double Height
        {
            get { return Math.Abs(m_startPos.Y - m_endPos.Y);  }
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
        /// Bezierの中間制御点1
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Point GetControlPoint1(Point start, Point end)
        {
            double x = (end.X - start.X) * 0.25;
            double y = 0;

            return new Point(x, y);
        }

        /// <summary>
        /// Bezierの中間制御点2
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Point GetControlPoint2(Point start, Point end)
        {
            double x = (end.X - start.X) * 0.75;
            double y = (end.Y - start.Y);
            
            return new Point(x, y);
        }

        /// <summary>
        /// パスの制御点を更新する
        /// </summary>
        private void UpdateControlPoints()
        {            
            Point start = m_startPos;
            Point end = m_endPos;

            // 常に左側にStartPosが来るようにSwapする
            if (start.X >= end.X)
            {
                Point temp = end;
                end = start;
                start = temp;
            }

            m_pathFigure.StartPoint = new Point(0,0);
            m_bezierSegment.Point1 = GetControlPoint1(start, end);
            m_bezierSegment.Point2 = GetControlPoint2(start, end);
            m_bezierSegment.Point3 = new Point(end.X - start.X, end.Y - start.Y);
        }
        #endregion            
    }
}
