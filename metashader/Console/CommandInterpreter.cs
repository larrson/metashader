using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

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
    /// 1行入力の文字列をコンソールベースのコマンドへ変換＆実行するインタプリタ
    /// </summary>
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
            m_commands.Add("ChangeVector4", new ChangeVector4Command());
            m_commands.Add("ChangeString", new ChangeStringCommand());
            m_commands.Add("ChangePos", new ChangePositionCommand());

            // リンク操作
            m_commands.Add("AddLink", new AddLinkCommand());
            m_commands.Add("SelectLink", new SelectLinkCommand());

            // 汎用操作
            m_commands.Add("Unselect", new UnselectCommand());
            m_commands.Add("Delete", new DeleteCommand());
            m_commands.Add("Undo", new UndoCommand());
            m_commands.Add("Redo", new RedoCommand());
            m_commands.Add("Load", new LoadCommand());
            m_commands.Add("Save", new SaveCommand());
            m_commands.Add("ExportShader", new ExportShaderCommand());
            m_commands.Add("ExecuteShader", new ExecuteShaderCommand());

            // コンソール自体の操作
            m_commands.Add("ImportConsole", new ImportConsoleCommand( this ));

            // デバッグ用
#if DEBUG
            m_commands.Add("PrintNode", new PrintNodeCommand());
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
