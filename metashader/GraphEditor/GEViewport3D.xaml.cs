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
using System.Windows.Media.Media3D;
using metashader.ShaderGraphData;
using metashader.Common;

namespace metashader.GraphEditor
{    
    /// <summary>
    /// GEViewport3D.xaml の相互作用ロジック
    /// </summary>
    public partial class GEViewport3D : UserControl
    {
        #region member classes
        /// <summary>
        /// グラフエディタ用コンテキストメニュー
        /// </summary>
        public class GraphEditorContextMenu : ContextMenu
        {
            public Point OpendPos { get; set; }

            public GraphEditorContextMenu()
                : base()
            {
                // 「ノードの追加」メニュー 
                MenuItem menuItemAddNode = new MenuItem();
                menuItemAddNode.Header = "ノードの追加";
                for (int i = 0; i < (int)ShaderGraphData.ShaderNodeType.Max; ++i)
                {
                    // インデックに対応するシェーダーノードの種類
                    ShaderGraphData.ShaderNodeType type = (ShaderGraphData.ShaderNodeType)i;

                    // 追加するメニュー
                    MenuItem menuItem = new MenuItem();
                    // 表記
                    menuItem.Header = type.ToStringExt();
                    // ハンドラ
                    menuItem.Click += (s, e) =>
                    {
                        Command.AddNewNodeCommand addNodeCommand = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.AddShaderNode) as Command.AddNewNodeCommand;
                        addNodeCommand.Execute(new Command.AddNewNodeCommand.Paramter(type, OpendPos));
                    };

                    menuItemAddNode.Items.Add(menuItem);
                }
                this.AddChild(menuItemAddNode);

                // 「削除」メニュー
                MenuItem menuItemDelete = new MenuItem();
                menuItemDelete.Header = "削除";
                menuItemDelete.Command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.Delete);
                this.AddChild(menuItemDelete);
            }
        }
        #endregion

#region variables
        static readonly double m_zoomBias = 10;

        Point m_prevMousePos;

        /// <summary>
        /// シェーダノードコントロールのリスト
        /// </summary>
        Dictionary<int, ShaderNodeControl> m_nodeList = new Dictionary<int, ShaderNodeControl>();

        /// <summary>
        /// リンクのパスのリスト
        /// </summary>
        SortedDictionary<LinkData, LinkPathControl> m_linkList = new SortedDictionary<LinkData, LinkPathControl>();

        /// <summary>
        /// viewport3D用カメラ
        /// </summary>
        GECamera m_camera = new GECamera();

        GraphEditorContextMenu m_contextMenu = new GraphEditorContextMenu();

        /// <summary>
        /// リンクドラッグ時に表示する曲線
        /// </summary>
        BezierCurve m_curveForLinkDrag;
        /// <summary>
        /// リンクドラッグ時に表示する3Dオブジェクト
        /// </summary>
        Viewport2DVisual3D m_visual3DForLinkDrag;

        /// <summary>
        /// ノードドラッグ時に表示する3Dオブジェクト
        /// </summary>
        Viewport2DVisual3D m_visual3DForNodeDrag;

        /// <summary>
        /// エラー表示用のテキストアドーナー
        /// </summary>
        Common.TextAdorner m_errorTextAdorner;
#endregion
        
#region constructors        
        public GEViewport3D()
        {
            InitializeComponent();

            // コンテキストメニュ初期化
            _grid.ContextMenu = m_contextMenu;

            // ビューポートを設定            
            _linkViewport.Camera = m_camera.Camera;            
            _nodeViewport.Camera = m_camera.Camera;
            _dragViewport.Camera = m_camera.Camera;
            _dragViewport.IsEnabled = false;

            // イベントハンドラの登録
            InitializeEventHandlers();

            // ドラッグ用オブジェクトを作成
            m_curveForLinkDrag = new BezierCurve();
            m_visual3DForLinkDrag = CreateViewport2DVisual3D(m_curveForLinkDrag.Path);
            _dragViewport.Children.Add(m_visual3DForLinkDrag);
        }        
#endregion        
    
#region public methods
        /// <summary>
        /// 指定したViewport2DVisual3Dオブジェクトの位置を設定する
        /// </summary>
        /// <param name="visual3D"></param>
        /// <param name="pos"></param>
        static public void SetPosition(Viewport2DVisual3D visual3D, Vector3D pos)
        {
            TranslateTransform3D translate = (visual3D.Transform as Transform3DGroup).Children[1] as TranslateTransform3D;
            translate.OffsetX = pos.X;
            translate.OffsetY = pos.Y;
            translate.OffsetZ = pos.Z;
        }

        /// <summary>
        /// 指定したViewport2DVisual3Dオブジェクトのスケールを設定する
        /// </summary>
        /// <param name="visual3D"></param>
        /// <param name="scale"></param>
        static public void SetScale(Viewport2DVisual3D visual3D, Vector3D scale)
        {
            ScaleTransform3D scaleTransform = (visual3D.Transform as Transform3DGroup).Children[0] as ScaleTransform3D;
            scaleTransform.ScaleX = scale.X;
            scaleTransform.ScaleY = scale.Y;
            scaleTransform.ScaleZ = scale.Z;            
        }
#endregion

#region event handlers
        /// <summary>
        /// ノード追加イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodeAddedEvent(object sender, metashader.Event.NodeAddedEventArgs args)
        {
            AddNewNode(args.Node);
        }

        /// <summary>
        /// ノードが削除された際に呼ばれるハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodeDeletedEvent(object sender, metashader.Event.NodeDeletedEventArgs args)
        {
            // 対応するコントロールを削除する
            DeleteNode(args.NodeHashCode);
        }

        /// <summary>
        /// ノードのプロパティが変更された際に呼ばれるハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodePropertyChangedEvent(object sender, metashader.Event.NodePropertyChangedEventArgs args)
        {
            // 対応するコントロールを探す
            ShaderNodeControl nodeControl = FindNodeControl(args.NodeHashCode);

            // 位置変更
            if (args.PropertyName == "Position" )
            {
                Point newPos = (Point)args.NewValue;
                Vector3D worldPos = m_camera.ToWorldPosFromVirtualScreen(newPos);
                SetPosition(nodeControl.Visual3D, worldPos);
            }

            // 処理を移譲
            nodeControl.OnNodePropertyChanged(sender, args);
        }

        /// <summary>
        /// リンクが追加された際に呼ばれるハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_LinkAddedEvent(object sender, metashader.Event.LinkAddedEventArgs args)
        {
            AddNewLink(args.LinkData);
        }

        /// <summary>
        /// リンクが削除された際に呼ばれるハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_LinkDeletedEvent(object sender, metashader.Event.LinkDeletedEventArgs args)
        {
            DeleteLink(args.LinkData);
        }

        /// <summary>
        /// グラフデータ構造のエラー報告時に呼ばれるイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_GraphErrorEvent(object sender, metashader.Event.GraphErrorEventArgs args)
        {
            // エラー表示を変更
            m_errorTextAdorner.Text = args.Message;
        }

        /// <summary>
        /// ノードやリンクの選択状態変更イベントのハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void SelectManager_SelectionChanged(object sender, SelectManager.SelectionChangedEventArgs args)
        {
            // 選択処理
            foreach (ShaderNodeDataBase node in args.SelectedNodes)
            {
                if (m_nodeList.ContainsKey(node.GetHashCode()))
                    m_nodeList[node.GetHashCode()].IsSelected = true;
            }

            // 選択が解除処理
            foreach (ShaderNodeDataBase node in args.UnselectedNodes)
            {
                if (m_nodeList.ContainsKey(node.GetHashCode()))
                    m_nodeList[node.GetHashCode()].IsSelected = false;
            }
        }        

        /// <summary>
        /// ビューポート内へマウスが入った
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _viewport_MouseEnter(object sender, MouseEventArgs e)
        {
            m_prevMousePos = e.GetPosition(this);
        }

        /// <summary>
        /// ビューポート内でマウスが移動した
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _viewport_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(_nodeViewport);
            
            // Controlを押していればマウス移動
            if (e.LeftButton == MouseButtonState.Pressed && (Keyboard.Modifiers == ModifierKeys.Control) )
            {
                // 画面に対して水平方向のカメラ制御
                GECamera camera = m_camera;

                // ワールド空間上の移動ベクトルを算出する
                Vector3D currentPos = camera.ToWorldPosFromClient(new Point(pos.X, pos.Y));
                Vector3D prevPos = camera.ToWorldPosFromClient(new Point(m_prevMousePos.X, m_prevMousePos.Y));
                Vector3D delta = currentPos - prevPos;
                delta.Z = 0;

                camera.Move(-delta);
            }
            m_prevMousePos = e.GetPosition(_nodeViewport);
        }

        /// <summary>
        /// ドラッグエンター
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GEViewport3D_DragEnter(object sender, DragEventArgs e)
        {
            if (ReferenceEquals(e.Source, _grid) == false && ReferenceEquals(e.Source, this) == false )
                return;

            // ノードのドラッグ
            if (e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format))
            {
                // ドラッグ用Visual3Dの作成   
                if( m_visual3DForNodeDrag == null )         
                {
                    ShaderNodeControl.NodeDragData nodeDragData = e.Data.GetData(ShaderNodeControl.NodeDragData.Format) as ShaderNodeControl.NodeDragData;
                    ShaderNodeControl nodeControl = nodeDragData.ShaderNodeControl;

                    var brush = new VisualBrush(nodeControl) { Opacity = 0.5 };
                    var Bound = VisualTreeHelper.GetDescendantBounds(nodeControl);
                    var Rect = new Rectangle() { Width = Bound.Width, Height = Bound.Height };
                    Rect.Fill = brush;

                    Rect.AllowDrop = false;
                    Rect.IsEnabled = false; // UIとしては無効(表示するだけ)に設定
                    m_visual3DForNodeDrag = CreateViewport2DVisual3D(Rect);
                    _dragViewport.Children.Add(m_visual3DForNodeDrag);  

                    // 仮想スクリーン上の位置を取得
                    Point screenPos = m_camera.ToVirtualScreenPos(e.GetPosition(_grid));
                    // ドラッグ開始位置で補正
                    screenPos.X -= nodeDragData.StartedMousePos.X;
                    screenPos.Y -= nodeDragData.StartedMousePos.Y;
                    
                    // viewport上の位置を調整
                    Vector3D worldPos = m_camera.ToWorldPosFromVirtualScreen(screenPos);
                    SetPosition(m_visual3DForNodeDrag, worldPos);
                }                
            }
            // ジョイントのドラッグ
            if (e.Data.GetDataPresent(JointControl.JointDragData.Format))
            {
                // リンクを表す曲線を表示
                m_curveForLinkDrag.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// ドラッグオーバー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GEViewport3D_DragOver(object sender, DragEventArgs e)
        {
            Point pos = e.GetPosition(_grid);

            // ノードのドラッグ
            if (e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format))
            {
                ShaderNodeControl.NodeDragData nodeDragData = e.Data.GetData(ShaderNodeControl.NodeDragData.Format) as ShaderNodeControl.NodeDragData;                

                // 仮想スクリーン上の位置を取得
                Point screenPos = m_camera.ToVirtualScreenPos(pos);
                // ドラッグ開始位置で補正
                screenPos.X -= nodeDragData.StartedMousePos.X;
                screenPos.Y -= nodeDragData.StartedMousePos.Y;

                // viewport上の位置を調整
                Vector3D worldPos = m_camera.ToWorldPosFromVirtualScreen(screenPos);
                SetPosition(m_visual3DForNodeDrag, worldPos);                
            }
            // ジョイントのドラッグ
            if (e.Data.GetDataPresent(JointControl.JointDragData.Format))
            {
                JointControl.JointDragData jointDragData = e.Data.GetData(JointControl.JointDragData.Format) as JointControl.JointDragData;

                // 曲線(コントロールの位置からマウス位置まで)を描画                                                
                m_curveForLinkDrag.StartPos = jointDragData.JointControl.Position;
                m_curveForLinkDrag.EndPos = m_camera.ToVirtualScreenPos( pos );
            }
        }

        /// <summary>
        /// ドラッグリーブ(バブル)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GEViewport3D_DragLeave(object sender, DragEventArgs e)
        {        
            Point pos = e.GetPosition(_grid);
            
            // ノードのドラッグは、PreviewDragLeaveで処理

            // ジョイントのドラッグ
            if (e.Data.GetDataPresent(JointControl.JointDragData.Format))
            {
                // リンクを表す曲線を非表示
                m_curveForLinkDrag.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// ドラッグリーブ(トンネル)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GEViewport3D_PreviewDragLeave(object sender, DragEventArgs e)
        {            
            // ノードのドラッグ
            if( e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format) )
            {
                // 自分の子供のオブジェクトからのLeaveであれば無視
                if (_nodeViewport.IsAncestorOf(e.OriginalSource as DependencyObject) && !ReferenceEquals(_grid, e.Source))
                    return;

                // ノードの3D表示を削除
                DeleteVisual3DForNodeDrag();
            }            

            // ジョイントのドラッグはDragLeave(バブル式)で処理
        }  

        /// <summary>
        /// ドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GEViewport3D_Drop(object sender, DragEventArgs e)
        {
            Point pos = e.GetPosition(_grid);

            // ノードのドロップ
            if (e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format))
            {
                // 位置変更を確定する               

                // ノードデータを取り出す
                ShaderNodeControl.NodeDragData nodeDragData = e.Data.GetData(ShaderNodeControl.NodeDragData.Format) as ShaderNodeControl.NodeDragData;
                ShaderGraphData.ShaderNodeDataBase nodeData = nodeDragData.ShaderNodeControl.Node;                

                // 仮想スクリーン上の位置を求める
                Point screenPos = m_camera.ToVirtualScreenPos(pos);
                // 位置を補正
                screenPos.X -= nodeDragData.StartedMousePos.X;
                screenPos.Y -= nodeDragData.StartedMousePos.Y;

                // Undo/Redoバッファ
                UndoRedoBuffer undoredo = new UndoRedoBuffer();                

                // 移動用のUndoRedoインスタンスを生成
                App.CurrentApp.GraphData.ChangeNodeProperty<Point>(nodeData, "Position", screenPos, undoredo);

                // Undo/Redoを登録
                if (undoredo.IsValid)
                {
                    UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }

                // ドラッグ用のVisual3Dを削除
                DeleteVisual3DForNodeDrag();
            }
            // ジョイントのドラッグ
            if (e.Data.GetDataPresent(JointControl.JointDragData.Format))
            {
                // リンクを表す曲線を非表示
                m_curveForLinkDrag.Visibility = Visibility.Hidden;

                // ハンドリングされた
                e.Handled = true;
            }
        }

        /// <summary>
        /// コンテキストメニューが開かれた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _grid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // グリッド上の位置を記録                      
            Point screenPos = new Point(e.CursorLeft, e.CursorTop);
            Point worldPos = m_camera.ToVirtualScreenPos(screenPos);

            m_contextMenu.OpendPos = worldPos;
        }

        /// <summary>
        /// 左マウスボタンが押された際のイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 選択解除
            App.CurrentApp.SelectManager.Clear();

            // イベントの伝播を止める
            e.Handled = true;
        }

        /// <summary>
        /// マウスホイールイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 画面に対して垂直方向のカメラ制御
            GECamera camera = m_camera;

            double delta = (-e.Delta / 120) * m_zoomBias;

            // カメラをズーム
            camera.Zoom(delta);
        }                

        /// <summary>
        /// 表示完了イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GEViewport3D_Loaded(object sender, RoutedEventArgs e)
        {
            // カメラの初期化
            m_camera.Initialize(_grid.ActualWidth, _grid.ActualHeight);

            // エラー表示用アドーナーの追加          
            m_errorTextAdorner = new Common.TextAdorner(this);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this);
            layer.Add(m_errorTextAdorner);
        }

        /// <summary>
        /// サイズ変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 新しいサイズをカメラに反映
            m_camera.Resize(e.NewSize.Width, e.NewSize.Height);
        }           
#endregion

#region private methods
        /// <summary>
        /// イベントハンドラの登録
        /// </summary>
        private void InitializeEventHandlers()
        {
            // ノードの追加
            App.CurrentApp.EventManager.NodeAddedEvent += new metashader.Event.NodeAddedEventHandler(EventManager_NodeAddedEvent);
            // ノードの削除
            App.CurrentApp.EventManager.NodeDeletedEvent += new metashader.Event.NodeDeletedEventHandler(EventManager_NodeDeletedEvent);
            // ノードのプロパティ変更
            App.CurrentApp.EventManager.NodePropertyChangedEvent += new metashader.Event.NodePropertyChangedEventHandler(EventManager_NodePropertyChangedEvent);
            // リンクの追加
            App.CurrentApp.EventManager.LinkAddedEvent += new metashader.Event.LinkAddedEventHandler(EventManager_LinkAddedEvent);
            // リンクの削除
            App.CurrentApp.EventManager.LinkDeletedEvent += new metashader.Event.LinkDeletedEventHandler(EventManager_LinkDeletedEvent);
            // エラー報告イベント
            App.CurrentApp.EventManager.GraphErrorEvent += new metashader.Event.GraphErrorEventHandler(EventManager_GraphErrorEvent);
            // 選択イベント
            App.CurrentApp.SelectManager.SelectionChanged += new SelectManager.SelectionChangedEventHandler(SelectManager_SelectionChanged);


            // ドラッグエンター
            this.DragEnter += new DragEventHandler(GEViewport3D_DragEnter);
            // ドラッグオーバー(バブル)
            this.DragOver += new DragEventHandler(GEViewport3D_DragOver);
            // ドラッグリーブ(トンネル)
            this.DragLeave += new DragEventHandler(GEViewport3D_DragLeave);
            // ドロップ処理
            this.Drop += new DragEventHandler(GEViewport3D_Drop);
            // ドラッグリーブ()
            this.PreviewDragLeave += new DragEventHandler(GEViewport3D_PreviewDragLeave);

            // コンテキストメニューが開かれた
            _grid.ContextMenuOpening += new ContextMenuEventHandler(_grid_ContextMenuOpening);

            // 描画準備完了イベント
            this.Loaded += new RoutedEventHandler(GEViewport3D_Loaded);            

            // マウス関連イベント            
            _grid.MouseLeftButtonDown += new MouseButtonEventHandler(_viewport_MouseLeftButtonDown);
            _grid.MouseWheel += new MouseWheelEventHandler(_viewport_MouseWheel);
            _grid.MouseMove += new MouseEventHandler(_viewport_MouseMove);
            _grid.MouseEnter += new MouseEventHandler(_viewport_MouseEnter);                    

            // リサイズ
            _grid.SizeChanged += new SizeChangedEventHandler(_grid_SizeChanged);            
        }                      

        /// <summary>
        /// ノードをドラッグする際に表示する3Dオブジェクトの削除
        /// </summary>
        private void DeleteVisual3DForNodeDrag()
        {            
            _dragViewport.Children.Remove(m_visual3DForNodeDrag);
            m_visual3DForNodeDrag = null;
        }

        /// <summary>
        /// シェーダノードに対応するコントロールを探す
        /// </summary>        
        private ShaderNodeControl FindNodeControl(int hashCode)
        {
            return m_nodeList[hashCode];
        }

        /// <summary>
        /// 指定したVisual要素からViewport2DVisual3Dオブジェクトを作成する
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public Viewport2DVisual3D CreateViewport2DVisual3D(FrameworkElement visual)
        {
            Viewport2DVisual3D viewport2DVisual3D = new Viewport2DVisual3D();     

            // ジオメトリを選択
            if( visual is ShaderNodeControl )
            {
                viewport2DVisual3D.Geometry = (MeshGeometry3D)(this.Resources["lefttopPlane"]);
            }            
            else if( visual is LinkPathControl)
            {
                viewport2DVisual3D.Geometry = (MeshGeometry3D)(this.Resources["centeredPlane"]);
            }
            else if( visual is Path)
            {
                viewport2DVisual3D.Geometry = (MeshGeometry3D)(this.Resources["centeredPlane"]);
            }
            else if( visual is Rectangle)
            {
                viewport2DVisual3D.Geometry = (MeshGeometry3D)(this.Resources["lefttopPlane"]);
            }
            else
            {
                throw new ArgumentException("visual");
            }

            viewport2DVisual3D.Material = (DiffuseMaterial)(this.Resources["materialWhite"]);

            // トランスフォーム
            Transform3DGroup transformGroup = new Transform3DGroup();             
            transformGroup.Children.Add(new ScaleTransform3D(1, 1, 1));
            transformGroup.Children.Add(new TranslateTransform3D(0, 0, 0));
            viewport2DVisual3D.Transform = transformGroup;
            viewport2DVisual3D.Visual = visual;
            // サイズ変更イベント
            visual.SizeChanged += (sender, e) =>
            {
                Viewport2DVisual3D v2dv3d = null;

                // スケーリングを変更する
                double actualHeight = 0;
                double actualWidth = 0;

                // ノード
                if( sender is ShaderNodeControl )
                {
                    ShaderNodeControl nodeControl = sender as ShaderNodeControl;
                    v2dv3d = nodeControl.Visual3D;

                    // 平面ポリゴンをスケーリング
                    actualWidth = nodeControl.ActualWidth;
                    actualHeight = nodeControl.ActualHeight;                    
                }
                // リンク
                else if( sender is LinkPathControl )
                {
                    LinkPathControl path = sender as LinkPathControl;
                    v2dv3d = path.Visual3D;
                    
                    // 平面ポリゴンをスケーリング
                    actualWidth = path.PathWidth;
                    actualHeight = path.PathHeight;   
                    
                    // 位置を移動
                    // 中央を基準にスケーリングしているので、中間地点に置けば良い
                    Vector3D worldPos = m_camera.ToWorldPosFromVirtualScreen(
                        new Point(
                            (path.StartPos.X + path.EndPos.X)/2
                            , (path.StartPos.Y + path.EndPos.Y)/2
                            ));
                    SetPosition(v2dv3d, worldPos);
                }                
                // ドラッグ用曲線
                else if( ReferenceEquals(sender, m_curveForLinkDrag.Path) )
                {
                    BezierCurve bezier = m_curveForLinkDrag;
                    v2dv3d = m_visual3DForLinkDrag;

                    // 平面ポリゴンをスケーリング
                    actualWidth = bezier.Width;
                    actualHeight = bezier.Height;
 
                    // 位置を移動
                    // 中央を基準にスケーリングしているので、中間地点に置けば良い
                    Vector3D worldPos = m_camera.ToWorldPosFromVirtualScreen(
                        new Point(
                            (bezier.StartPos.X + bezier.EndPos.X) / 2
                            , (bezier.StartPos.Y + bezier.EndPos.Y) / 2
                            ));
                    SetPosition(v2dv3d, worldPos);
                }
                // ドラッグ用ノード
                else if( sender is Rectangle)
                {
                    Rectangle rect = sender as Rectangle;

                    v2dv3d = m_visual3DForNodeDrag;

                    // 平面ポリゴンをスケーリング
                    actualWidth = rect.ActualWidth;
                    actualHeight = rect.ActualHeight;
                }
                else
                {
                    throw new ArgumentException("visual");
                }

                if( v2dv3d != null )
                {
                    SetScale(v2dv3d, new Vector3D(actualWidth, actualHeight, 1.0));                
                }                
            };
           
            return viewport2DVisual3D;
        }              

        /// <summary>
        /// 新しいシェーダノードを追加する
        /// </summary>
        /// <param name="node">追加対象のノード</param>
        private void AddNewNode(ShaderGraphData.ShaderNodeDataBase node)
        {            
            // 新しいシェーダノードのコントロールを作成
            ShaderNodeControl newNodeControl = new ShaderNodeControl(node);

            // コントロールからViewport2DVisual3Dを作成
            newNodeControl.Visual3D = CreateViewport2DVisual3D(newNodeControl);

            // リストに追加
            m_nodeList.Add(node.GetHashCode(), newNodeControl);

            // 表示用のビューポートへ追加
            _nodeViewport.Children.Add(newNodeControl.Visual3D);

            // 位置を合わせる
            Point screenPos = newNodeControl.Position;
            Vector3D worldPos = m_camera.ToWorldPosFromVirtualScreen(screenPos);
            SetPosition(newNodeControl.Visual3D, worldPos);
        }

        /// <summary>
        /// 既存のノードを削除する
        /// </summary>
        /// <param name="hashCode">5</param>
        private void DeleteNode(int hashCode)
        {
            // 対応するコントロールを探す
            ShaderNodeControl nodeControl = FindNodeControl(hashCode);

            // ビューポートから削除
            _nodeViewport.Children.Remove(nodeControl.Visual3D);

            // リストから削除
            m_nodeList.Remove(hashCode);
        }

        /// <summary>
        /// ノードを移動させる
        /// </summary>
        private void ChangeNodePosition(ShaderNodeControl nodeControl, Point pos)
        {            
        }

        /// <summary>
        /// 新しいリンクを追加する
        /// </summary>
        /// <param name="link"></param>
        private void AddNewLink(LinkData link)
        {
            // 入出力のコントロール
            ShaderNodeControl inputNode = m_nodeList[link._inNodeHash];
            ShaderNodeControl outputNode = m_nodeList[link._outNodeHash];

            // パスを作成            
            LinkPathControl linkPath = new LinkPathControl(inputNode, link._inJointIndex, outputNode, link._outJointIndex);

            // パスからViewport2DVisual3Dを作成
            linkPath.Visual3D = CreateViewport2DVisual3D(linkPath);            

            // パスを保持用のリストへ追加
            m_linkList.Add(link, linkPath);

            // 表示用のビューポートへ追加
            _linkViewport.Children.Add(linkPath.Visual3D);

            // 位置合わせは、SizeChangedイベント内で処理するので明示的な処理は必要ない

            // ドラッグ時に表示させていた曲線を非表示に
            m_curveForLinkDrag.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 既存のリンクを削除する
        /// </summary>
        /// <param name="link"></param>
        private void DeleteLink(LinkData link)
        {
            // 対応するパスを探す
            LinkPathControl path = m_linkList[link];

            // 削除時の処理を呼び出し
            path.OnDeleted();

            // ビューポートから削除
            _linkViewport.Children.Remove(path.Visual3D);

            // パスのリストから削除
            m_linkList.Remove(link);            
        }        
#endregion
    }
}
