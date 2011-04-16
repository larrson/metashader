using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace metashader.GraphEditor
{
    /// <summary>
    /// リンクを表すPathを保持するオブジェクト
    /// </summary>
    public class LinkPath
    {
#region variable        
        /// <summary>
        /// Pathをもつ曲線オブジェクト
        /// </summary>
        BezierCurve m_curve;

        /// <summary>
        /// 接続先の入力ジョイントをもつコントロール
        /// </summary>
        ShaderNodeControl m_inputNode;
        /// <summary>
        /// 入力ジョイントのインデックス
        /// </summary>
        int m_inputJointIndex;

        /// <summary>
        /// 接続先の出力ジョイントをもつコントロール
        /// </summary>
        ShaderNodeControl m_outputNode;
        /// <summary>
        /// 出力ジョイントのインデックス
        /// </summary>
        int m_outputJointIndex;        
#endregion

#region properties
        /// <summary>
        /// パス
        /// </summary>
        public Path Path
        {
            get { return m_curve.Path; }
        }                        
#endregion

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public LinkPath(ShaderNodeControl inputNode, int inputJointIndex, ShaderNodeControl outputNode, int outputJointIndex )            
        {            
            // 初期化      

            // 曲線作成
            m_curve = new BezierCurve();

            // メンバ変数初期化
            m_inputNode = inputNode;            
            m_inputJointIndex = inputJointIndex;
            m_curve.StartPos = inputNode.GetInputJointPos(m_inputJointIndex);

            m_outputNode = outputNode;
            m_outputJointIndex = outputJointIndex;
            m_curve.EndPos = outputNode.GetOutputJointPos(m_outputJointIndex);
                        
            // イベント登録
            m_inputNode.PropertyChanged += new PropertyChangedEventHandler(inputNode_PropertyChanged);
            m_outputNode.PropertyChanged += new PropertyChangedEventHandler(outputNode_PropertyChanged);
        }        
#endregion

#region public methods
        /// <summary>
        /// 削除時に呼ばれるメソッド
        /// </summary>
        public void OnDeleted()
        {
            // イベントの登録解除
            m_inputNode.PropertyChanged -= new PropertyChangedEventHandler(inputNode_PropertyChanged);
            m_outputNode.PropertyChanged -= new PropertyChangedEventHandler(outputNode_PropertyChanged);
        }
#endregion

#region event handlers
        /// <summary>
        /// 入力ノードのプロパティが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void inputNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 接続先の入力ノードの位置のプロパティが変更された
            if( e.PropertyName == "Position")
            {
                m_curve.StartPos = (sender as ShaderNodeControl).GetInputJointPos(m_inputJointIndex);
            }
        }

        /// <summary>
        /// 出力ノードのプロパティが変更された
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void outputNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 接続先の出力ノードの位置のプロパティが変更された
            if (e.PropertyName == "Position")
            {
                m_curve.EndPos = (sender as ShaderNodeControl).GetOutputJointPos(m_outputJointIndex);                
            }
        }   
#endregion
    }
}
