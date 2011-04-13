using System;
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

                // 表示位置
                double x, y;
                x = y = 0;
                if (options.Length >= 4) // 種類以外もふくんでいる場合
                {
                    x = double.Parse(options[2]);
                    y = double.Parse(options[3]);
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
    /// コンソールベースのシェーダノードのプロパティ変更コマンド(4Dベクトル版)
    /// </summary>
    public class ChangeVector4Command : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if( options.Length < 7)
            {
                return;
            }

            // ノード名前
            string name = options[1];

            // 対応するノードを取得
            ShaderGraphData.ShaderNodeDataBase node = App.CurrentApp.GraphData.GetNode(name);

            // プロパティ名を取得
            string propertyName = options[2];

            // ベクトルの4成分を取得
            float[] values = new float[4];
            values[0] = float.Parse( options[3] );
            values[1] = float.Parse( options[4] );
            values[2] = float.Parse( options[5] );
            values[3] = float.Parse( options[6] );

            // Undo/Redoバッファ
            ShaderGraphData.UndoRedoBuffer undoredo = new ShaderGraphData.UndoRedoBuffer();
            
            // 新しいプロパティを設定
            if( node != null )            
            {
                App.CurrentApp.GraphData.ChangeNodeProperty<float[]>( node, propertyName, values, undoredo );

                if( undoredo.IsValid )
                {
                    ShaderGraphData.UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }
            }            
        }
    }

    /// <summary>
    /// コンソールベースの文字列変更コマンド
    /// </summary>
    public class ChangeStringCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if( options.Length < 4 )
            {
                return;
            }

            // ノード名
            string name = options[1];

            // 対応するノードを取得
            ShaderGraphData.ShaderNodeDataBase node = App.CurrentApp.GraphData.GetNode(name);

            // プロパティ名を取得
            string propertyName = options[2];

            // 変更後の文字列を取得
            string newStr = options[3];

            // Undo/Redoバッファ
            ShaderGraphData.UndoRedoBuffer undoredo = new ShaderGraphData.UndoRedoBuffer();

            // 新しいプロパティを設定
            if (node != null)
            {
                App.CurrentApp.GraphData.ChangeNodeProperty<string>(node, propertyName, newStr, undoredo);

                if (undoredo.IsValid)
                {
                    ShaderGraphData.UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }
            }            
        }
    }

    /// <summary>
    /// コンソールベースの位置変更コマンド
    /// </summary>
    public class ChangePositionCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if (options.Length < 4)
            {
                return;
            }

            // ノード名
            string name = options[1];

            // 対応するノードを取得
            ShaderGraphData.ShaderNodeDataBase node = App.CurrentApp.GraphData.GetNode(name);            

            // 変更後の位置を取得
            double x = double.Parse(options[2]);
            double y = double.Parse(options[3]);
            Point pos = new Point(x, y);

            // Undo/Redoバッファ
            ShaderGraphData.UndoRedoBuffer undoredo = new ShaderGraphData.UndoRedoBuffer();

            // 新しいプロパティを設定
            if (node != null)
            {
                App.CurrentApp.GraphData.ChangeNodeProperty<Point>(node, "Position", pos, undoredo);

                if (undoredo.IsValid)
                {
                    ShaderGraphData.UndoRedoManager.Instance.RegistUndoRedoBuffer(undoredo);
                }
            }
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
    /// コンソールベースのシェーダ実行コマンド
    /// </summary>
    public class ExecuteShaderCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {                        

            // 実行するコマンドを呼び出し
            // データを操作するコマンドを呼び出し
            Command.CommandBase command = App.CurrentApp.UICommandManager.GetCommand(Command.CommandType.ExecuteShader);
            command.Execute(
                 null
                );
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
    /// コンソールベースのノード情報出力コマンド
    /// </summary>
    public class PrintNodeCommand : IConsoleCommand
    {
        /// <summary>
        /// コンソールベースのコマンドを実行する
        /// </summary>
        /// <param name="options">コマンド引数（コマンド名自体を含む）</param>
        public void Execute(string[] options)
        {
            if( options.Length < 2 )
            {
                return;
            }

            // 名前
            string name = options[1];

            // 対応するノードを取得
            ShaderGraphData.ShaderNodeDataBase node = App.CurrentApp.GraphData.GetNode(name);

            // 表示
            if( node != null )
            {
                node.DebugPrint();
            }
        }
    }

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
