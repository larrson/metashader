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
using System.Windows.Controls.Primitives;
using metashader.ShaderGraphData;
using metashader.Common;

namespace metashader.GraphEditor
{
    /// <summary>
    /// JointControl.xaml の相互作用ロジック
    /// </summary>
    public partial class JointControl : UserControl
    {

#region member classes
        public class JointDragData
        {
            public static readonly string Format = "metashader.JointDragData";

            JointControl _jointControl;

            public JointControl JointControl
            {
                get { return _jointControl;  }
            }

            public JointDragData( JointControl jointControl )
            {
                _jointControl = jointControl;
            }
        }
#endregion

#region variables
        /// <summary>
        /// 対応するジョイントデータ
        /// </summary>
        JointData m_jointData;

        /// <summary>
        /// このジョイントをもつ親コントロール
        /// </summary>
        ShaderNodeControl m_parentControl;
#endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="joint"></param>
        public JointControl(JointData jointData, ShaderNodeControl parentControl)
        {
            InitializeComponent();

            // メンバ変数初期化
            m_jointData = jointData;
            m_parentControl = parentControl;

            // JointDataに基づいて外観を変更
            // サフィックスに合わせて色変更
            SolidColorBrush[] brushes =
            {
                Brushes.Black,
                Brushes.Red,
                Brushes.Green,
                Brushes.Blue,
                Brushes.White,
            };
            _thumb.Background = brushes[(int)jointData.Suffix];

            // イベントハンドラ登録            
            // ドラッグ開始
            _thumb.DragStarted += new DragStartedEventHandler(_thumb_DragStarted);
            // ドロップ処理
            _thumb.Drop += new DragEventHandler(_thumb_Drop);
        }        

#region properties
        /// <summary>
        /// 対応するジョイントデータ
        /// </summary>
        public JointData JointData
        {
            get { return m_jointData; }
        }

        /// <summary>
        /// キャンバス上の位置(リンクの発生位置なので、コントロールの左隅ではなく中心位置)
        /// </summary>
        public Point Position
        {
            get 
            {
                return (JointData.SideType == JointData.Side.In)
                    ? m_parentControl.GetInputJointPos(JointData.JointIndex)
                    : m_parentControl.GetOutputJointPos(JointData.JointIndex);

            }
        }
#endregion

#region event handlers
        void _thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            // ドラッグ開始
            DragDrop.DoDragDrop(this
                , new DataObject(JointDragData.Format, new JointDragData(this))
                , DragDropEffects.Link);
        }

        void _thumb_Drop(object sender, DragEventArgs e)
        {            
            // ジョイントのドロップ
            if( e.Data.GetDataPresent(JointControl.JointDragData.Format) )
            {
                JointControl.JointDragData dragData = e.Data.GetData(JointControl.JointDragData.Format) as JointControl.JointDragData;

                // 接続を試みる                

                /// 接続 ///
                // 入出力ジョイントを取得
                JointData inputJointData = (this.JointData.SideType == JointData.Side.In ) 
                    ? this.JointData : dragData.JointControl.JointData;
                JointData outputJointData = (this.JointData.SideType == JointData.Side.Out )
                    ? this.JointData : dragData.JointControl.JointData;

                // 接続可否のチェック                             
                bool isValidJoint = inputJointData.SideType == JointData.Side.In && outputJointData.SideType == JointData.Side.Out;
                if ( isValidJoint == false )
                    return;
                
                // 接続コマンドを実行
                Command.AddLinkCommand command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.AddLink) as Command.AddLinkCommand;
                command.Execute( new Command.AddLinkCommand.Paramter(outputJointData.ParentNode.GetHashCode(), outputJointData.JointIndex, inputJointData.ParentNode.GetHashCode(), inputJointData.JointIndex) );
            }
        }

        /// <summary>
        /// コンテキストメニューの「リンクの解除」をクリックした
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_DeleteLink_Click(object sender, RoutedEventArgs e)
        {
            JointData inputJoint, outputJoint;
            inputJoint = outputJoint = null;
            if( this.JointData.SideType == JointData.Side.In )
            {
                inputJoint = this.JointData;
            }
            else
            {
                outputJoint = this.JointData;
            }

            // undo/redoバッファ
            UndoRedoBuffer undoredo = new UndoRedoBuffer();

            // このジョイントに接続されている全てのリンクを解除する            
            while( this.JointData.JointList.Count != 0 )
            {
                JointData joint = this.JointData.JointList.First.Value;
                if (joint.SideType == JointData.Side.In)
                {
                    inputJoint = joint;
                }
                else
                {
                    outputJoint = joint;
                }

                // データ構造に対してリンクの解除を行う
                App.CurrentApp.GraphData.DelLink(
                      outputJoint.ParentNode.GetHashCode()
                    , outputJoint.JointIndex
                    , inputJoint.ParentNode.GetHashCode()
                    , inputJoint.JointIndex
                    , undoredo );

                // undo/redoマネージャへ登録
                if( undoredo.IsValid )
                {
                    UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }
            }
        }

#endregion                
    }
}
