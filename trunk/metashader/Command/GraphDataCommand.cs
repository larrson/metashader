using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using metashader.ShaderGraphData;
using System.Windows;
using System.Collections.ObjectModel;

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
           public ShaderGraphData.ShaderNodeType Type { get; set; }

           /// <summary>
           /// エディター上の表示位置
           /// </summary>
           public Point Pos{ get; set; }           

           public Paramter( ShaderGraphData.ShaderNodeType type, Point pos )
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

            /// ノードの削除 ///
            ReadOnlyCollection<ShaderGraphData.ShaderNodeDataBase> list = App.CurrentApp.SelectManager.SelectedNodeList;
            foreach( ShaderNodeDataBase node in list )
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
}
