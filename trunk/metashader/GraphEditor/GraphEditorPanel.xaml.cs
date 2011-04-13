﻿using System;
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
using metashader.ShaderGraphData;

namespace metashader.GraphEditor
{
    /// <summary>
    /// GraphEditorPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class GraphEditorPanel : UserControl
    {
#region member classes
        /// <summary>
        /// グラフエディタ用コンテキストメニュー
        /// </summary>
        public class GraphEditorContextMenu : ContextMenu
        {
            public Point OpendPos{ get; set; }

            public GraphEditorContextMenu()
                : base()
            {
                // 「ノードの追加」メニュー 
                MenuItem menuItemAddNode = new MenuItem();
                menuItemAddNode.Header = "ノードの追加";
                for(int i = 0; i < (int)ShaderGraphData.ShaderNodeType.Max; ++i)
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

                    menuItemAddNode.Items.Add( menuItem );
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
        /// <summary>
        /// シェーダノードコントロールのリスト
        /// </summary>
        Dictionary<int, ShaderNodeControl> m_nodeList = new Dictionary<int, ShaderNodeControl>();

        /// <summary>
        /// リンクのパスのリスト
        /// </summary>
        SortedDictionary<LinkData, LinkPath> m_linkList = new SortedDictionary<LinkData, LinkPath>();

        /// <summary>
        /// 専用のコンテキストメニュー
        /// </summary>
        GraphEditorContextMenu m_contextMenu;
#endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GraphEditorPanel()
        {
            InitializeComponent();

            // イベントハンドラの初期化
            InitializeEventHandlers();

            // コンテキストメニューの初期化
            m_contextMenu = new GraphEditorContextMenu();
            _grid.ContextMenu = m_contextMenu;
        }        

#region event handlers
        /// <summary>
        /// ノードが追加された際に呼ばれるハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void EventManager_NodeAddedEvent(object sender, metashader.Event.NodeAddedEventArgs args)
        {
            // 対応するコントロールを追加する
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

            switch( args.PropertyName )
            {
                case "Position":
                    nodeControl.Position = (Point)args.NewValue;
                    break;
                default:
                    throw new NotImplementedException();                  
            }
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
        /// ドラッグオーバー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GraphEditorPanel_DragOver(object sender, DragEventArgs e)
        {
            Point pos = e.GetPosition(_grid);
            /*
            // ノードのドラッグ
            if( e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format) )
            {
                ShaderNodeControl.NodeDragData nodeDragData = e.Data.GetData(ShaderNodeControl.NodeDragData.Format) as ShaderNodeControl.NodeDragData;
                Point newPos = new Point(
                            pos.X - nodeDragData.StartedMousePos.X + nodeDragData.StartedPos.X
                        ,   pos.Y - nodeDragData.StartedMousePos.Y + nodeDragData.StartedPos.Y
                    );                
                // nodeのデータには反映させず、見た目だけ変更する
                nodeDragData.ShaderNodeControl.Position = newPos;
            }            
             */
        }

        /// <summary>
        /// ドラッグリーブ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GraphEditorPanel_DragLeave(object sender, DragEventArgs e)
        {            
            Point pos = e.GetPosition(_grid);                       
            
            /*
            // ノードのドラッグ
            if (e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format) && Object.ReferenceEquals(e.Source,_grid))
            {
                // 範囲外か判定
                bool isOutOfArea = !(( 0 <= pos.X && pos.X <= _grid.ActualWidth) && ( 0 <= pos.Y && pos.Y <= _grid.ActualHeight));
                if( isOutOfArea )
                {
                    // 範囲外であれば元の位置に戻す
                    ShaderNodeControl.NodeDragData nodeDragData = e.Data.GetData(ShaderNodeControl.NodeDragData.Format) as ShaderNodeControl.NodeDragData;
                    nodeDragData.ShaderNodeControl.Position = nodeDragData.StartedPos;
                }                
            }
             */
        }        

        /// <summary>
        /// ドロップ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GraphEditorPanel_Drop(object sender, DragEventArgs e)
        {
            Point pos = e.GetPosition(_grid);

            // ノードのドロップ
            if( e.Data.GetDataPresent(ShaderNodeControl.NodeDragData.Format) )
            {
                // 位置変更を確定する
                ShaderNodeControl.NodeDragData nodeDragData = e.Data.GetData(ShaderNodeControl.NodeDragData.Format) as ShaderNodeControl.NodeDragData;
                Point newPos = new Point(
                          pos.X - nodeDragData.StartedMousePos.X + nodeDragData.StartedPos.X
                        , pos.Y - nodeDragData.StartedMousePos.Y + nodeDragData.StartedPos.Y
                    );
                ChangeNodePosition(nodeDragData.ShaderNodeControl, newPos);
            }
        }

        /// <summary>
        /// コンテキストメニューを開く直前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GraphEditorPanel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // グリッド上の位置を記録
            m_contextMenu.OpendPos = new Point(e.CursorLeft, e.CursorTop);
        }
#endregion

#region private methods
        /// <summary>
        /// イベントハンドラの初期化
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

            // ドラッグオーバー
            _grid.DragOver += new DragEventHandler(GraphEditorPanel_DragOver);
            // ドラッグリーブ
            _grid.DragLeave += new DragEventHandler(GraphEditorPanel_DragLeave);
            // ドロップ処理
            _grid.Drop += new DragEventHandler(GraphEditorPanel_Drop);
            // コンテキストメニューを開いた
            _grid.ContextMenuOpening +=new ContextMenuEventHandler(GraphEditorPanel_ContextMenuOpening);
        }

        /// <summary>
        /// シェーダノードに対応するコントロールを探す
        /// </summary>        
        private ShaderNodeControl FindNodeControl(int hashCode)
        {
            return m_nodeList[hashCode];
        }

        /// <summary>
        /// 新しいシェーダノードを追加する
        /// </summary>
        /// <param name="node">追加対象のノード</param>
        private void AddNewNode(ShaderGraphData.ShaderNodeDataBase node)
        {
            // 新しいシェーダノードのコントロールを作成
            ShaderNodeControl newNodeControl =  new ShaderNodeControl(node);

            // リストに追加
            m_nodeList.Add(node.GetHashCode(), newNodeControl);

            /// 表示用のキャンバスへ追加 ///
            _nodeCanvas.Children.Add(newNodeControl);
            // 位置を合わせる
            newNodeControl.Position = node.Position;
        }

        /// <summary>
        /// 既存のノードを削除する
        /// </summary>
        /// <param name="hashCode">5</param>
        private void DeleteNode(int hashCode)
        {
            // 対応するコントロールを探す
            ShaderNodeControl nodeControl = FindNodeControl(hashCode);

            // キャンバスから削除
            _nodeCanvas.Children.Remove(nodeControl);

            // リストから削除
            m_nodeList.Remove(hashCode);
        }

        /// <summary>
        /// ノードを移動させる
        /// </summary>
        private void ChangeNodePosition(ShaderNodeControl nodeControl, Point pos)
        {
            // Undo/Redoバッファ
            ShaderGraphData.UndoRedoBuffer undoredo = new ShaderGraphData.UndoRedoBuffer();

            // 移動用のUndoRedoインスタンスを生成
            App.CurrentApp.GraphData.ChangeNodeProperty<Point>(nodeControl.Node, "Position", pos, undoredo);

            // Undo/Redoを登録
            if (undoredo.IsValid)
            {
                ShaderGraphData.UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
            }
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
            //LinkPath linkPath = new LinkPath( inputPos, outputPos );
            LinkPath linkPath = new LinkPath(inputNode, link._inJointIndex, outputNode, link._outJointIndex);

            // パスをキャンバスへ追加
            _linkCanvas.Children.Add(linkPath.Path);            

            // パスを保持用のリストへ追加
            m_linkList.Add(link, linkPath);
        }

        /// <summary>
        /// 既存のリンクを削除する
        /// </summary>
        /// <param name="link"></param>
        private void DeleteLink(LinkData link)
        {
            // 対応するパスを探す
            LinkPath path = m_linkList[link];

            // 削除時の処理を呼び出し
            path.OnDeleted();

            // パスのリストから削除
            m_linkList.Remove(link);

            // キャンバスから削除
            _linkCanvas.Children.Remove(path.Path);
        }
#endregion
    }
}
