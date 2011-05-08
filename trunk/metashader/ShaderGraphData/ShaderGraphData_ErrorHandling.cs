using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// シェーダグラフデータクラスのエラー関連処理
    /// </summary>
    public partial class ShaderGraphData
    {
#region private variables
        /// <summary>
        /// グラフのトポロジによるエラーが存在しないフラグ
        /// デシリアライズ時にエラー検出イベントを通じて自動的に設定されるため、シリアライズしない
        /// </summary>
        [NonSerialized]
        bool m_noError = false;
#endregion

#region properties
        /// <summary>
        /// エラーがないか
        /// </summary>
        public bool NoError
        {
            get { return m_noError; }
        }
#endregion

#region private methods
        /// <summary>
        /// エラー検出
        /// @@ 検出するエラーのマスクにフラグを用いる        
        /// </summary>
        private void DetectError()
        {            
            // エラーの有無を記録
            // エラーが無ければ、最終return前にtrueに設定
            m_noError = false;

            // 出力ノードの存在に関して
            if( m_outputNodeList.Count == 0 )
            {
                // 出力ノードがない
                App.CurrentApp.EventManager.RaiseGraphError(this, Event.GraphErrorEventArgs.NoOutput());

                return;
            }

            // 出力ノードを取得
            ShaderNodeDataBase outputNode = m_outputNodeList[0];

            // 出力ノードに接続されているリストを取得
            List<ShaderNodeDataBase> nodeList = this.ValidNodes;

            // 無限ループの検出
            ShaderNodeDataBase loopedNode;
            if( DetectInfiniteLoop(outputNode, out loopedNode) )
            {
                // 無限ループが見つかった
                App.CurrentApp.EventManager.RaiseGraphError(this, Event.GraphErrorEventArgs.InfiniteLoop(loopedNode));

                return;
            }

            // 無効なノードの検出
            foreach( ShaderNodeDataBase node in nodeList )
            {
                if (node.IsValid() == false)
                {
                    App.CurrentApp.EventManager.RaiseGraphError(this, Event.GraphErrorEventArgs.InvalidNode(node));
                    return;
                }                    
            }

            // エラーが無いことを記録しておく
            m_noError = true;

            // エラー無し
            App.CurrentApp.EventManager.RaiseGraphError(this, Event.GraphErrorEventArgs.NoError());
        }

        /// <summary>
        /// 無限ループの検出
        /// </summary>       
        /// <param name="outputNode"></param>
        /// <param name="loopedNode"></param>
        /// <returns></returns>
        private bool DetectInfiniteLoop(ShaderNodeDataBase outputNode, out ShaderNodeDataBase loopedNode )
        {
            Dictionary<int, ShaderNodeDataBase> finishedList = new Dictionary<int, ShaderNodeDataBase>();
            Dictionary<int,ShaderNodeDataBase> currentList  = new Dictionary<int, ShaderNodeDataBase>();

            // サブルーチンへ処理を移譲
            return DetectInfiniteLoopSub(outputNode, out loopedNode, finishedList, currentList);

        }

        /// <summary>
        /// 無限ループ検出のサブルーチン
        /// 深さ優先でループを検出する
        /// </summary>
        /// <param name="node">検索対象となる部分グラフのルートノード</param>
        /// <param name="loopedNode">見つかったループ中の1ノード</param>
        /// <param name="finishedList">検索が終了したノードのリスト</param>
        /// <param name="currentList">現在検索中のノードのリスト</param>
        /// <returns></returns>
        private bool DetectInfiniteLoopSub(ShaderNodeDataBase node, out ShaderNodeDataBase loopedNode, Dictionary<int,ShaderNodeDataBase> finishedList, Dictionary<int,ShaderNodeDataBase> currentList)
        {
            // 出力の初期化
            loopedNode = null;

            // すでに検索済みならリターン
            if (finishedList.ContainsKey(node.GetHashCode()))
                return false;
            // 調査中ノードに含まれるなら、ループしている
            if( currentList.ContainsKey(node.GetHashCode()) )
            {
                loopedNode = node;
                return true;
            }

            // 部分グラフを検索開始
            currentList.Add(node.GetHashCode(), node); // 検索中リストに追加

            for (int i = 0; i < node.InputJointNum; ++i )
            {
                // 入力ジョイントを取得
                JointData inputJoint = node.GetInputJoint(i);

                // それに接続されている出力ジョイント
                foreach( JointData outputJoint in inputJoint.JointList)
                {
                    // 出力ジョイントをもつノード
                    ShaderNodeDataBase outputNode = outputJoint.ParentNode;

                    // 再帰呼び出し
                    if( DetectInfiniteLoopSub(outputNode, out loopedNode, finishedList, currentList) )
                    {
                        // 見つかったら終了
                        return true;
                    }
                }
            }

            // 部分グラフの検索終了
            currentList.Remove(node.GetHashCode()); // 検索中リストから削除
            finishedList.Add(node.GetHashCode(), node); // 検索終了リストに追加

            return false;
        }
#endregion
    }
}
