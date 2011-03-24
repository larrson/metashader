using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// リンクの接続点のデータ構造
    /// </summary>
    class JointData
    {       
#region variables
        /// <summary>
        /// シェーダノードのハッシュ値
        /// </summary>
        int m_nodeHashCode;

        /// <summary>
        /// シェーダノード内のジョイントのインデックス
        /// </summary>
        int m_jointIndex;   
#endregion

#region constructors
        public JointData( int nodeHashCode, int jointIndex )
        {
            m_nodeHashCode = nodeHashCode;
            m_jointIndex = jointIndex;
        }
#endregion

#region properties
        /// <suimary>
        /// シェーダノードのハッシュ値
        /// </summary>
        public int NodeHashCode
        {
            get { return m_nodeHashCode; }
        }

        /// <summary>
        /// シェーダノード内のジョイントのインデックス
        /// </summary>
        public int JointIndex
        {
            get { return m_jointIndex; }
        }
#endregion
    }

    /// <summary>
    /// シェーダノード間の単一のリンクを表すデータ構造   
    /// データは、出力元ノード（out）から入力先ノード(in)へ流れる
    /// グラフ構造を隣接リストで表現するため、侵入型のリスト構造を採用
    /// </summary>
    class LinkData
    {        
#region variables
        /// <summary>
        /// 出力元ノードのジョイント
        /// </summary>
        JointData m_out;

        /// <summary>
        /// 入力先ノードのジョイント
        /// </summary>
        JointData m_in;
#endregion

#region constructors
        public LinkData( JointData outJoint, JointData inJoint )
        {
            m_out = outJoint;
            m_in = inJoint;
        }
#endregion

#region properties
        /// <summary>
        /// 出力元ジョイント
        /// </summary>
        public Joint OutJoint
        {
            get { return m_out; }
        }

        /// <summary>
        /// 入力先ジョイント
        /// </summary>
        public Joint InJoint
        {
            get { return m_in;  }
        }
        
        /// <summary>
        /// 同じ出力元ノードに接続された別のリンク（前）
        /// </summary>
        public LinkData PrevLink
        {
            get;
            set;
        }

        /// <summary>
        /// 同じ出力元ノードに接続された別のリンク（後）
        /// </summary>
        public LinkData NextLink
        {
            get;
            set;
        }
#endregion
    }
}
