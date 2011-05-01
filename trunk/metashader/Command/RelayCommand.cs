using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;

namespace metashader
{
    /// <summary>
    /// コンストラクタで受け取るデリゲート、ラムダ式で
    /// 新たなコマンドを定義するためのクラス    
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region variables

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region constructors

        /// <summary>
        /// コンストラクタ
        /// 実行確認のないバージョン。常に実行可能と仮定する。
        /// </summary>
        /// <param name="execute">実行処理</param>
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="execute">実行処理</param>
        /// <param name="canExecute">実行確認処理</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region override methods

        /// <summary>
        /// 実行可能か判定
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// コマンドの実行可否判定呼び出しを、フレームワークに任せる
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
}