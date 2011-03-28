﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace metashader.Console
{          
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
                System.Console.WriteLine(e.Message + "(" + e.ParamName + ")");
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
            if (options.Length < 5) return;

            int outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex;
            outNodeHashCode = ShaderGraphData.ShaderNodeDataBase.CalcHashCode(options[1]);
            outJointIndex = int.Parse(options[2]);
            inNodeHashCode = ShaderGraphData.ShaderNodeDataBase.CalcHashCode(options[3]);
            inJointIndex = int.Parse(options[4]);

            // データを操作するコマンドを呼び出し
            Command.AddLinkCommand command = App.CurrentApp.UICommandManager.GetCommand(
                Command.CommandType.AddLink) as Command.AddLinkCommand;
            command.Execute(
                new Command.AddLinkCommand.Paramter(outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex)
                );
        }
    }        

    /// <summary>
    /// コンソールベースのリンク選択コマンド
    /// </summary>
    public class SelectLinkCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if (options.Length < 5) return;

            // パラメータをパース
            int outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex;
            outNodeHashCode = ShaderGraphData.ShaderNodeDataBase.CalcHashCode(options[1]);
            outJointIndex = int.Parse(options[2]);
            inNodeHashCode = ShaderGraphData.ShaderNodeDataBase.CalcHashCode(options[3]);
            inJointIndex = int.Parse(options[4]);

            // 選択
            App.CurrentApp.SelectManager.Select(new ShaderGraphData.LinkData(outNodeHashCode, outJointIndex, inNodeHashCode, inJointIndex));            
        }
    }

    /// <summary>
    /// コンソールベースの選択解除コマンド
    /// </summary>
    public class UnselectCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            // データを操作するコマンドを呼び出し
            Command.CommandBase command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.Unselect);
            command.Execute(
                 null
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
    /// コンソールベースのシェーダの書き出しコマンド
    /// </summary>
    public class ExportShaderCommand : IConsoleCommand
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

            App.CurrentApp.GraphData.ExportShaderCode(path);
        }
    }

    /// <summary>
    /// コンソールベースのコマンドが書かれたファイルを読み込み実行する
    /// </summary>
    public class ImportConsoleCommand : IConsoleCommand
    {      
#region variables
        /// <summary>
        /// コマンド実行用インタプリタ
        /// </summary>
        CommandInterpreter m_interpreter;
#endregion

#region constructors
        public ImportConsoleCommand(CommandInterpreter interpreter) : base()
        {
            m_interpreter = interpreter;
        }
#endregion

        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if (options.Length < 2)
                return;

            string path = options[1];

            using ( StreamReader reader = new StreamReader(path) )
            {
                while( reader.EndOfStream == false )
                {
                    // 1行読み込み
                    string line = reader.ReadLine();

                    // コマンドを実行
                    m_interpreter.Interpret(line);
                }                
            }
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
}