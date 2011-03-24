using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace metashader.Command
{
    /// <summary>
    /// コマンドの種類
    /// </summary>
    public enum CommandType : int
    {
        AddShaderNode,  // シェーダーノードの追加
        AddLink,        // リンクの追加

        Delete,         // 選択中要素の削除
        Undo,           // 元に戻すコマンド
        Redo,           // やり直しコマンド

        Max,            // コマンドの数
    };

    /// <summary>
    /// UI上から利用するコマンドの基本クラス
    /// </summary>
    public abstract class CommandBase : ICommand
    {

#region variables
        /// <summary>
        /// コマンドの名前
        /// </summary>
        string m_name;
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public CommandBase(string name) : base()
        {
            m_name = name;
        }
#endregion

#region properties
        /// <summary>
        /// コマンドの名前
        /// </summary>
        public string Name 
        {
            get { return m_name; }
        }
#endregion

#region public methods
        /// <summary>
        /// コマンドの実行可否を判定する
        /// ICommandから継承したメソッド        
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract bool CanExecute(object parameter);
        /// <summary>
        /// コマンドを実行する
        /// ICommandから継承したメソッド
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(object parameter);
#endregion

#region events
        /// <summary>       
        /// コマンドマネージャに、CanExecuteイベントの要求をする
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }
#endregion  
    }

    /// <summary>
    /// UI用コマンド管理クラス
    /// </summary>
    public class CommandManager
    {      
#region variables
        CommandBase[] m_command;
#endregion    

#region constructors
        public CommandManager()
        {
            // 初期化
            Initialize();
        }
#endregion

#region public methods
        /// <summary>
        /// 指定した種類のコマンドを取得する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CommandBase GetCommand(CommandType type)
        {
            return m_command[(int)type];
        }
#endregion

#region private methods
        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize()
        {
            // コマンドの登録
            m_command = new CommandBase[(int)CommandType.Max];
            m_command[(int)CommandType.AddShaderNode] = new AddNewNodeCommand();
            m_command[(int)CommandType.AddLink] = new AddLinkCommand();
            m_command[(int)CommandType.Delete] = new DeleteCommand();
            m_command[(int)CommandType.Undo] = new UndoCommand();
            m_command[(int)CommandType.Redo] = new RedoCommand();
        }
#endregion

    }
}
