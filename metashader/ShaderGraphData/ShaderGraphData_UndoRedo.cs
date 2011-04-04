using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// ShaderGraphDataのUndoRedo処理サブクラス群の定義
    /// </summary>
    public partial class ShaderGraphData
    {
        #region undoredo classes
        /// <summary>
        /// ノードを新規作成して追加
        /// </summary>
        class UndoRedo_AddNewNode : IUndoRedo
        {
            ShaderGraphData _graph;
            ShaderNodeType _type;
            Point _pos;

            /// <summary>
            /// 削除用
            /// </summary>
            int _hashCode;

            public UndoRedo_AddNewNode(ShaderGraphData graph, ShaderNodeType type, Point pos, int hashCode)
            {
                _graph = graph;
                _type = type;
                _pos = pos;
                _hashCode = hashCode;
            }

            public void Undo()
            {
                _graph.DelNode(_hashCode, null);
                _hashCode = -1;

                //削除したのでファクトリのカウンタをデクリメント
                _graph.DecrementShaderNodeCounter(_type);
            }

            public void Redo()
            {
                ShaderNodeDataBase node;
                _graph.AddNewNode(_type, _pos, null, out node);
                _hashCode = node.GetHashCode();
            }
        }

        /// <summary>
        /// 既存のシェーダノードの追加処理のUndo/Redo
        /// </summary>
        class UndoRedo_AddNode : IUndoRedo
        {
            ShaderGraphData _graph;
            ShaderNodeDataBase _node;

            public UndoRedo_AddNode(ShaderGraphData graph, ShaderNodeDataBase node)
            {
                _graph = graph;
                _node = node;
            }

            public void Undo()
            {
                _graph.DelNode(_node.GetHashCode(), null);
            }

            public void Redo()
            {
                _graph.AddNode(_node, null);
            }
        }

        /// <summary>
        /// シェーダノード削除処理のUndo/Redo
        /// </summary>
        class UndoRedo_DelNode : IUndoRedo
        {
            ShaderGraphData _graph;
            ShaderNodeDataBase _node;

            public UndoRedo_DelNode(ShaderGraphData graph, ShaderNodeDataBase node)
            {
                _graph = graph;
                _node = node;
            }

            public void Undo()
            {
                _graph.AddNode(_node, null);
            }

            public void Redo()
            {
                _node = _graph.GetNode(_node.GetHashCode()); // 再作成によってインスタンスが異なっている可能性があるため
                _graph.DelNode(_node.GetHashCode(), null);
            }
        }

        /// <summary>
        /// シェーダノードのリンクの追加処理のUndo/Redo
        /// </summary>
        class UndoRedo_AddLink : IUndoRedo
        {
            ShaderGraphData _graph;
            int _outNodeHashCode;
            int _outJointIndex;
            int _inNodeHashCode;
            int _inJointIndex;            

            public UndoRedo_AddLink(ShaderGraphData graph, int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex )
            {
                _graph = graph;
                _outNodeHashCode = outNodeHashCode;
                _outJointIndex = outJointIndex;
                _inNodeHashCode = inNodeHashCode;
                _inJointIndex = inJointIndex;                
            }

            public void Undo()
            {
                _graph.DelLink(_outNodeHashCode, _outJointIndex, _inNodeHashCode, _inJointIndex, null);
            }

            public void Redo()
            {
                _graph.AddLink(_outNodeHashCode, _outJointIndex, _inNodeHashCode, _inJointIndex, null);
            }
        }

        /// <summary>
        /// シェーダノードのリンクの追加処理のUndo/Redo
        /// </summary>
        class UndoRedo_DelLink : IUndoRedo
        {
            ShaderGraphData _graph;
            int _outNodeHashCode;
            int _outJointIndex;
            int _inNodeHashCode;
            int _inJointIndex;

            public UndoRedo_DelLink(ShaderGraphData graph, int outNodeHashCode, int outJointIndex, int inNodeHashCode, int inJointIndex)
            {
                _graph = graph;
                _outNodeHashCode = outNodeHashCode;
                _outJointIndex = outJointIndex;
                _inNodeHashCode = inNodeHashCode;
                _inJointIndex = inJointIndex;
            }

            public void Undo()
            {
                _graph.AddLink(_outNodeHashCode, _outJointIndex, _inNodeHashCode, _inJointIndex, null);
            }

            public void Redo()
            {
                _graph.DelLink(_outNodeHashCode, _outJointIndex, _inNodeHashCode, _inJointIndex, null);
            }
        }
        #endregion
    }
}
