using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace metashader.GraphEditor
{
    /// <summary>
    /// ShaderNodeControl.xaml の相互作用ロジック
    /// </summary>
    public partial class ShaderNodeControl : UserControl, INotifyPropertyChanged
    {
#region member classes
        public class NodeDragData
        {
            public static readonly string Format = "metashader.NodeDragData";

            /// <summary>
            /// ドラッグ対象コントロール
            /// </summary>
            ShaderNodeControl   _nodeControl;

            /// <summary>
            /// ドラッグ開始時のコントロールの位置
            /// </summary>
            Point _startedPos;

            /// <summary>
            /// ドラッグ開始時のマウス位置
            /// </summary>
            Point _StartedMousePos;

            /// <summary>
            /// ドラッグ対象コントロール
            /// </summary>
            public ShaderNodeControl ShaderNodeControl
            {
                get { return _nodeControl; }                
            }

            /// <summary>
            /// ドラッグ開始時のコントロールの位置
            /// </summary>
            public Point StartedPos 
            {
                get { return _startedPos;  }
            }

            /// <summary>
            /// ドラッグ開始時のマウス位置
            /// </summary>
            public Point StartedMousePos
            {
                get { return _StartedMousePos;  }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="nodeControl"></param>
            /// <param name="startedMousePos"></param>
            public NodeDragData( ShaderNodeControl nodeControl, Point startedMousePos )
            {
                _nodeControl = nodeControl;
                _startedPos = _nodeControl.Position;
                _StartedMousePos = startedMousePos;       
            }
        }
#endregion

#region variables
        /// <summary>
        /// 対応するシェーダーノードデータ
        /// </summary>
        ShaderGraphData.ShaderNodeDataBase m_node;
        
        /// <summary>
        /// 最後にキャプチャされたマウス位置
        /// ドラッグ処理用
        /// </summary>
        Point m_lastCapturedPos;

        /// <summary>
        /// マウスの左ボタンが押下されているか
        /// ドラッグ処理用
        /// </summary>
        bool m_isMouseLeftButtonDown;        

        /// <summary>
        /// プロパティ変更イベント
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// サムネイルのコントロール
        /// </summary>
        Thumnail.ThumnailControl m_thumnailControl;
#endregion

#region properties
        /// <summary>
        /// 対応するシェーダノード
        /// </summary>
        public ShaderGraphData.ShaderNodeDataBase Node
        {
            get { return m_node; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Point Position
        {
            get { return new Point(Canvas.GetLeft(this), Canvas.GetTop(this));  }
            set 
            {                
                Canvas.SetLeft(this, value.X); Canvas.SetTop(this, value.Y); 
                
                NotifyPropertyChanged("Position");                
            }
        }        
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="node"></param>
        public ShaderNodeControl(ShaderGraphData.ShaderNodeDataBase node)
        {
            InitializeComponent();

            // 変数初期化
            m_node = node;
            m_isMouseLeftButtonDown = false;            

            // DataContextの設定
            DataContext = m_node;

            // ジョイント作成
            // 入力ジョイント
            for (int i = 0; i < node.InputJointNum; ++i)
            {
                _inputJointGrid.Children.Add(new JointControl(m_node.GetInputJoint(i), this));                    
            }
            // 出力ジョイント
            for (int i = 0; i < node.OutputJointNum; ++i)
            {
                _outputJointGrid.Children.Add(new JointControl(m_node.GetOutputJoint(i), this));
            }            

            // 中央のサムネイルを作成
            switch( node.Type )
            {
                case metashader.ShaderGraphData.ShaderNodeType.Uniform_Vector4:
                    m_thumnailControl = new Thumnail.ColorThumnail(m_node);
                    break;
                case metashader.ShaderGraphData.ShaderNodeType.Uniform_Texture2D:
                    m_thumnailControl = new Thumnail.TextureThumnail(m_node);
                    break;
                default:
                    m_thumnailControl = new Thumnail.DefaultThumnail(m_node);
                    break;
            }            
            _thumnailGrid.Children.Add(m_thumnailControl);

            // イベントハンドラ登録
            _nameTextBlock.MouseLeftButtonDown += new MouseButtonEventHandler(Node_MouseLeftButtonDown);
            _nameTextBlock.MouseUp += new MouseButtonEventHandler(Node_MouseUp);
            _nameTextBlock.MouseMove += new MouseEventHandler(Node_MouseMove);
        } 
#endregion        
      
#region public methods
        /// <summary>
        /// 入力ジョイントの中心位置を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point GetInputJointPos(int index)
        {
            // 左上基準位置
            Point basicPos = Position;

            // ActualXXXが0である場合、強制的にレイアウトを呼び出し、有効な値を設定させる
            if( _outputJointGrid.ActualWidth == 0 )
            {
                UpdateLayout();
            }

            // X座標オフセット
            double offsetX = _outputJointGrid.ActualWidth + _nameTextBlock.ActualWidth;
            double offsetY = _nameTextBlock.ActualHeight + (_thumnailGrid.ActualHeight / Node.InputJointNum) * ((double)index + 0.5);

            return new Point(basicPos.X + offsetX, basicPos.Y + offsetY);
        }

        /// <summary>
        /// 出力ジョイントの中心位置を取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Point GetOutputJointPos(int index)
        {
            // 左上基準位置
            Point basicPos = Position;

            // ActualXXXがNaNである場合、強制的にレイアウトを呼び出し、有効な値を設定させる
            if ( _outputJointGrid.ActualWidth == 0 )
            {
                UpdateLayout();
            }

            // X座標オフセット
            double offsetX = _outputJointGrid.ActualWidth / 2;
            double offsetY = _nameTextBlock.ActualHeight + (_thumnailGrid.ActualHeight / Node.OutputJointNum) * ((double)index + 0.5);

            return new Point(basicPos.X + offsetX, basicPos.Y + offsetY);
        }

        /// <summary>
        /// ノードのプロパティ変更時に親コントロールから呼ばれる処理
        /// データ構造の変更を反映する
        /// </summary>
        /// <param name="args"></param>
        public void OnNodePropertyChanged(object sender, Event.NodePropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case "Position":
                    Position = (Point)args.NewValue;
                    break;
                default:
                    // throw new NotImplementedException();                  
                    break;
            }

            // サムネイル側の処理を呼ぶ(@イベントドリブンにすべきか)
            m_thumnailControl.OnNodePropertyChanged(sender, args);
        }
#endregion

#region event handlers
        /// <summary>
        /// ノードがマウス左クリックされた際に呼ばれるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Node_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // ノードを選択する
            App.CurrentApp.SelectManager.Select(m_node);

            // キャプチャする                        
            m_lastCapturedPos = GetPositionInCanvas(e.GetPosition(this));

            m_isMouseLeftButtonDown = true; 
        }

        void Node_MouseUp(object sender, MouseButtonEventArgs e)
        {            
            if( e.LeftButton == MouseButtonState.Released )
            {
                m_isMouseLeftButtonDown = false;
            }            
        }

        void Node_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = GetPositionInCanvas(e.GetPosition(this));

            // キャプチャされている場合の処理
            if(  m_isMouseLeftButtonDown && e.LeftButton == MouseButtonState.Pressed )
            {
                // ドラッグを開始出来るか
                bool isDragStart = (Math.Abs(pos.X - m_lastCapturedPos.X) > SystemParameters.MinimumHorizontalDragDistance )
                                    || (Math.Abs(pos.Y - m_lastCapturedPos.Y) > SystemParameters.MinimumVerticalDragDistance );
                if( isDragStart )
                {                                       
                    m_isMouseLeftButtonDown = false;
                    
                    // ドラッグ開始
                    DragDrop.DoDragDrop(this
                        , new DataObject(NodeDragData.Format, new NodeDragData(this, pos))
                        , DragDropEffects.Move);
                }
            }            
            else
            {
                m_isMouseLeftButtonDown = false;
            }
        }        
#endregion        

#region private methods
        /// <summary>
        /// キャンバスに対する位置を取得する
        /// </summary>
        /// <param name="localPos"></param>
        /// <returns></returns>
        Point GetPositionInCanvas(Point localPos)
        {
            double X = Position.X + localPos.X;
            double Y = Position.Y + localPos.Y;
            return new Point(X, Y);
        }

        /// <summary>
        /// プロパティ変更イベントの通知
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
#endregion
    }
}
