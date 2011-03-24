using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace metashader.Console
{
    /// <summary>
    /// コンソールベースのコマンドのインターフェース
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        void Execute(string[] options);        
    }

    /// <summary>
    /// コンソールベースのセーブコマンド
    /// </summary>
    public class SaveCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if (options.Length < 2)
                return;

            string path = options[1];
            BinaryFormatter bf = new BinaryFormatter();
            App.CurrentApp.Save(path, bf);
        }
    }

    /// <summary>
    /// コンソールベースのロードコマンド
    /// </summary>
    public class LoadCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if (options.Length < 2)
                return;

            string path = options[1];
            BinaryFormatter bf = new BinaryFormatter();
            App.CurrentApp.Load(path, bf);
        }
    }

    /// <summary>
    /// コンソールベースのシェーダノード追加コマンド
    /// </summary>
    public class AddNodeCommand : IConsoleCommand
    {       
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
       public void Execute(string[] options)
       {
           // 種類
           string typeName = options[1];          
           try
           {
               ShaderGraphData.ShaderNodeType type = (ShaderGraphData.ShaderNodeType)Enum.Parse(Type.GetType("metashader.ShaderGraphData.ShaderNodeType"), typeName);
               // ジョイント数
               int inJointNum, outJointNum;
               if (options.Length > 2)
               {
                   inJointNum = int.Parse(options[2]);
                   outJointNum = int.Parse(options[3]);
               }

               // 表示位置
               double x, y;
               x = y = 0;
               if (options.Length > 4) // 種類以外もふくんでいる場合
               {
                   x = double.Parse(options[4]);
                   y = double.Parse(options[5]);
               }

               // データを操作するコマンドを呼び出し
               Command.AddNewNodeCommand command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.AddShaderNode) as Command.AddNewNodeCommand;
               command.Execute(
                    new Command.AddNewNodeCommand.Paramter(type, new Point(x, y))
                   );
           }
           catch (System.ArgumentException e)
           {
               System.Console.WriteLine(e.Message + "("+  e.ParamName + ")" );
           }                      
       }
    }

    /// <summary>
    /// コンソールベースのシェーダノード単一選択コマンド
    /// </summary>
    public class SelectNodeCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            // 名前
            string name = options[1];

            // 対応するノードを取得
            ShaderGraphData.ShaderNodeDataBase node = App.CurrentApp.GraphData.GetNode(name);

            // 選択処理
            App.CurrentApp.SelectManager.Select(node);
        }
    }

    /// <summary>
    /// コンソールベースのリンク追加コマンド
    /// </summary>
    public class AddLinkCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if( options.Length < 5 ) return;

            int outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex;
            outNodeHashCode = ShaderGraphData.ShaderNodeDataBase.CalcHashCode(options[1]);
            outJointIndex   = int.Parse(options[2]);
            inNodeHashCode = ShaderGraphData.ShaderNodeDataBase.CalcHashCode(options[3]);
            inJointIndex    = int.Parse(options[4]);

            // データを操作するコマンドを呼び出し
            Command.AddLinkCommand command = App.CurrentApp.UICommandManager.GetCommand(
                Command.CommandType.AddLink) as Command.AddLinkCommand;
            command.Execute(
                new Command.AddLinkCommand.Paramter( outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex )
                );
        }
    }

    /// <summary>
    /// 削除コマンド
    /// </summary>
    public class DeleteCommand : IConsoleCommand
    {
         /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            // データを操作するコマンドを呼び出し
            Command.CommandBase command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.Delete);
            command.Execute(
                 null
                );
        }
    }

    /// <summary>
    /// コンソールベースのUndoコマンド
    /// </summary>
    public class UndoCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            // データを操作するコマンドを呼び出し
            Command.CommandBase command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.Undo);
            command.Execute(
                 null
                );
        }
    }

    /// <summary>
    /// コンソールベースのRedoコマンド
    /// </summary>
    public class RedoCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            // データを操作するコマンドを呼び出し
            Command.CommandBase command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.Redo);
            command.Execute(
                 null
                );
        }
    }

#if DEBUG
    /// <summary>
    /// コンソールベースのリンク出力コマンド
    /// </summary>
    public class PrintLinkCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            App.CurrentApp.GraphData.DebugPrintLinkList();
        }
    }
#endif // DEBUG

    public class CommandInterpreter
    {
#region variables
        /// <summary>
        /// コマンド名とコマンド処理オブジェクトのマップ
        /// </summary>
        Dictionary<string, IConsoleCommand> m_commands = new Dictionary<string, IConsoleCommand>();
#endregion

#region constructors
        public CommandInterpreter()
        {
            /// コマンドの登録 ///           
            // ノード操作
            m_commands.Add("AddNode", new AddNodeCommand() );
            m_commands.Add("SelectNode", new SelectNodeCommand());
            // リンク操作
            m_commands.Add("AddLink", new AddLinkCommand());

            // 汎用操作
            m_commands.Add("Delete", new DeleteCommand());
            m_commands.Add("Undo", new UndoCommand());
            m_commands.Add("Redo", new RedoCommand());
            m_commands.Add("Load", new LoadCommand());
            m_commands.Add("Save", new SaveCommand());
#if DEBUG
            m_commands.Add("PrintLink", new PrintLinkCommand());
#endif // DEBUG
        }
#endregion

#region public methods
        /// <summary>
        /// コマンドラインのコマンドを解釈し、実行する
        /// </summary>
        /// <param name="line"></param>
        public void Interpret(string line)
        {
            // 文字列を空白文字で分割
            string[] result;
            char[] separator ={' '};
            result = line.Split(separator);

            // 先頭をコマンド名として取り出す
            string name = result[0];

            // 対応するコマンドを実行
            IConsoleCommand command;
            if( m_commands.TryGetValue(name, out command))
            {
                command.Execute(result);
            }
        }
#endregion
    }
}
