﻿//========================================================================
// グラフデータを操作するためのコマンド群の定義
//========================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using metashader.ShaderGraphData;
using System.Windows;
using System.Collections.ObjectModel;
using metashader.Common;

namespace metashader.Command
{   
    /// <summary>
    /// 新規シェーダノードを追加するコマンド
    /// </summary>
   public class AddNewNodeCommand : CommandBase
   {
#region member class
       /// <summary>
       /// コマンド実行時パラメータ
       /// </summary>
       public class Paramter
       {
           /// <summary>
           /// 種類
           /// </summary>
           public string Type { get; set; }

           /// <summary>
           /// エディター上の表示位置
           /// </summary>
           public Point Pos{ get; set; }           

           public Paramter( string type, Point pos )
           {
               Type = type;
               Pos = pos;               
           }
       }
#endregion

#region constructor
       public AddNewNodeCommand()
           : base( "ノードを追加" )
       {       
       }
#endregion

#region override methods
       public override bool CanExecute(object parameter)
       {
           return true;
       }

       public override void Execute(object parameter)
       {
           Paramter param = parameter as Paramter;
           if( param == null )
           {
               throw new ArgumentException("parameterの型がAddNodeCommand.Parameterと一致しません", "parameter");
           }

           // undo/redo用バッファ
           UndoRedoBuffer undoredo = new UndoRedoBuffer();

           // 新規ノードの追加処理
           ShaderNodeDataBase node;
           App.CurrentApp.GraphData.AddNewNode(param.Type, param.Pos, undoredo, out node );
           
           // undo/redo用バッファの登録
           if( undoredo.IsValid)
           {
               UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
           }
       }         
#endregion       
   }

    public class AddLinkCommand : CommandBase
    {
#region member class
       /// <summary>
       /// コマンド実行時パラメータ
       /// </summary>
       public class Paramter
       {           
           public int OutNodeHashCode { get; set; }
           public int OutJointIndex { get; set; }
           public int InNodeHashCode { get;set; }
           public int InJointIndex { get; set; }

           public Paramter(int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex)
           {
               OutNodeHashCode = outNodeHashCode;
               OutJointIndex = outJointIndex;
               InNodeHashCode = inNodeHashCode;
               InJointIndex = inJointIndex;
           }
       }
#endregion

#region constructor
       public AddLinkCommand()
           : base( "リンクを追加" )
       {       
       }
#endregion

#region override methods
       public override bool CanExecute(object parameter)
       {
           return true;
       }

       public override void Execute(object parameter)
       {
           Paramter param = parameter as Paramter;
           if( param == null )
           {
               throw new ArgumentException("parameterの型がAddLinkCommand.Parameterと一致しません", "parameter");
           }

           // undo/redo用バッファ
           UndoRedoBuffer undoredo = new UndoRedoBuffer();

           // 入力ジョイントへすでに接続済みの場合、リンクを外して新たに接続する
           JointData inputJoint = App.CurrentApp.GraphData.GetNode(param.InNodeHashCode).GetInputJoint(param.InJointIndex);
           while( inputJoint.JointList.Count > 0 )
           {
               JointData outputJoint = inputJoint.JointList.Last.Value;
               App.CurrentApp.GraphData.DelLink(outputJoint.ParentNode.GetHashCode(), outputJoint.JointIndex, param.InNodeHashCode, param.InJointIndex, undoredo);
           }

           // リンクの追加処理           
           App.CurrentApp.GraphData.AddLink(param.OutNodeHashCode, param.OutJointIndex, param.InNodeHashCode, param.InJointIndex, undoredo);
           
           // undo/redo用バッファの登録
           if( undoredo.IsValid)
           {
               UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
           }
       }         
#endregion       
    }    

    /// <summary>
    /// 削除コマンド
    /// </summary>
    public class DeleteCommand : CommandBase
    {
#region constructor
        public DeleteCommand()
            : base("削除")
        {            
        }
#endregion

#region override methods
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            // 対象のグラフデータ
            ShaderGraphData.ShaderGraphData graphData = App.CurrentApp.GraphData;

            // undo/redo用バッファ
            UndoRedoBuffer undoredo = new UndoRedoBuffer();            

            /// リンクの削除 ///
            ReadOnlyCollection<ShaderGraphData.LinkData> links = App.CurrentApp.SelectManager.SelectedLinkList;
            foreach( LinkData link in links )
            {
                graphData.DelLink(link._outNodeHash, link._outJointIndex, link._inNodeHash, link._inJointIndex, undoredo);
            }

            /// ノードの削除 ///
            ReadOnlyCollection<ShaderGraphData.ShaderNodeDataBase> nodes = App.CurrentApp.SelectManager.SelectedNodeList;
            foreach( ShaderNodeDataBase node in nodes )
            {
                graphData.DelNode( node.GetHashCode(), undoredo );
            }           

           /// undo/redo用バッファの登録 ///
           if( undoredo.IsValid)
           {
               UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
           }           

           /// 選択の解除 ///
           App.CurrentApp.SelectManager.Clear();
        }
#endregion       
    }

    /// <summary>
    /// 元に戻すコマンド
    /// </summary>
    public class UndoCommand : CommandBase
    {
#region constructor
        public UndoCommand()
            : base ("元に戻す")
        {}
#endregion

#region override methods
        public override bool CanExecute(object parameter)
        {
            return UndoRedoManager.Instance.IsEnableUndo;
        }

        public override void Execute(object parameter)
        {
            // 選択の解除
            App.CurrentApp.SelectManager.Clear();

            // Undo処理
            UndoRedoManager.Instance.Undo();            
        }
#endregion
    }

    /// <summary>
    /// やり直しコマンド
    /// </summary>
    public class RedoCommand : CommandBase
    {
#region constructor
        public RedoCommand()
            : base ("やり直し")
        {}
#endregion

#region override methods
        public override bool CanExecute(object parameter)
        {
            return UndoRedoManager.Instance.IsEnableRedo;
        }

        public override void Execute(object parameter)
        {
            // 選択の解除
            App.CurrentApp.SelectManager.Clear();

            // Redo処理
            UndoRedoManager.Instance.Redo();            
        }
#endregion
    }

    /// <summary>
    /// 選択解除コマンド
    /// （全ての選択を解除する）
    /// </summary>
    public class UnselectCommand : CommandBase
    {
#region constructor
        public UnselectCommand()
            : base ("選択の解除")
        {            
        }        
#endregion

#region override methods
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            // 選択の解除
            App.CurrentApp.SelectManager.Clear();                    
        }
#endregion

    }
}
