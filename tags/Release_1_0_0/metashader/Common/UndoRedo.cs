using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.Common
{
    /// <summary>
    /// 単一のUndoRedo処理を行うインターフェース
    /// </summary>
    public interface IUndoRedo
    {
        /// <summary>
        /// もとに戻す
        /// </summary>
        void Undo();

        /// <summary>
        /// やり直し
        /// </summary>
        void Redo();
    };

    /// <summary>
    /// 複数のUndoRedo処理を記録し、単一の操作として実行するバッファ
    /// </summary>
    public class UndoRedoBuffer : IUndoRedo
    {
#region variables
        /// <summary>
        /// 一連のUndo/Redoオブジェクトを保持するコンテナ
        /// </summary>
        List<IUndoRedo> m_histroy = new List<IUndoRedo>();
#endregion

#region properties
        /// <summary>
        /// 保持されているUndo/Redoオブジェクトの数
        /// </summary>
        public int Count
        {
            get { return m_histroy.Count; }
        }

        /// <summary>
        /// 有効か
        /// </summary>
        public bool IsValid
        {
            get { return (m_histroy.Count > 0); }
        }
#endregion

#region override methods
        /// <summary>
        /// もとに戻す
        /// </summary>
        public void Undo()
        {
            int i, num = m_histroy.Count;
            for(i = num-1; i >= 0; --i )
            {
                m_histroy[i].Undo();
            }
        }

        /// <summary>
        /// やり直し
        /// </summary>
        public void Redo()
        {
            foreach( IUndoRedo it in m_histroy )
            {
                it.Redo();
            }
        }
#endregion

#region public methods
        /// <summary>
        /// バッファにUndo/Redoオブジェクトを積む
        /// </summary>
        /// <param name="undoredo"></param>
        public void Add( IUndoRedo undoredo )
        {
            m_histroy.Add(undoredo);
        }
#endregion
    };

    /// <summary>
    /// Undo/Redoの管理クラス
    /// </summary>
    public class UndoRedoManager
    {
#region variables
        /// <summary>
        /// もとに戻す用スタック
        /// </summary>
        Stack<UndoRedoBuffer> m_undoStack = new Stack<UndoRedoBuffer>();
        /// <summary>
        /// やり直し用スタック
        /// </summary>
        Stack<UndoRedoBuffer> m_redoStack = new Stack<UndoRedoBuffer>();
#endregion

#region singleton
        /// <summary>
        /// 唯一のインスタンス
        /// </summary>
        private static UndoRedoManager instance;

        /// <summary>
        /// コンストラクタの禁止
        /// </summary>
        private UndoRedoManager(){}

        /// <summary>
        /// インスタンスアクセス用プロパティ
        /// </summary>
        public static UndoRedoManager Instance
        {
            get
            {
                if( instance == null )
                {
                    instance = new UndoRedoManager();
                }
                return instance;
            }
        }
#endregion

#region properties
        /// <summary>
        /// Undoが可能か
        /// </summary>
        public bool IsEnableUndo
        {
            get { return (m_undoStack.Count > 0); }
        }

        /// <summary>
        /// Redoが可能か
        /// </summary>
        public bool IsEnableRedo
        {
            get { return (m_redoStack.Count > 0); }
        }
#endregion

#region public methods
        /// <summary>
        /// もとに戻す
        /// </summary>
        public void Undo()
        {
            if ( IsEnableUndo )                
            {
                // undo用スタックから取り出し、Undoを実行
                UndoRedoBuffer undoredo = m_undoStack.Pop();
                undoredo.Undo();

                // redoスタックへ積む
                m_redoStack.Push(undoredo);
            }
        }

        /// <summary>
        /// やり直し
        /// </summary>
        public void Redo()
        {
            if ( IsEnableRedo )
            {
                // redo用スタックから取り出し、Redoを実行
                UndoRedoBuffer undoredo = m_redoStack.Pop();
                undoredo.Redo();

                // undoスタックへ積む
                m_undoStack.Push(undoredo);
            }
        }

        /// <summary>
        /// Undo/Redoバッファの登録       
        /// 新たに操作を行った場合に呼び出す
        /// </summary>
        public void RegistUndoRedoBuffer( UndoRedoBuffer buffer )
        {
            // 新規操作をundoスタックへ積む
            m_undoStack.Push(buffer);

            // 新規操作なのでredoスタックをクリア
            m_redoStack.Clear();
        }

        /// <summary>
        /// クリアする
        /// ファイル読み込み時など、Undo/Redoが無効になる場合に呼び出す
        /// </summary>
        public void Clear()
        {
            m_undoStack.Clear();
            m_redoStack.Clear();
        }
#endregion
    };
}
