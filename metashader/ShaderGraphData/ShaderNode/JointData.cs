using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.ShaderGraphData
{
    #region Variable Type
    /// <summary>
    /// シェーダ変数の型
    /// </summary>
    public enum VariableType
    {
        INDEFINITE, // 不定(関数の戻り値用)
        DEPENDENT,  // 依存型（ノードに依存する）

        FLOAT,  // スカラー
        FLOAT2, // 2次元ベクトル
        FLOAT3, // 3次元ベクトル
        FLOAT4, // 4次元ベクトル
    }

    /// <summary>
    /// VariableTypeを拡張するクラス
    /// </summary>
    public static class VariableTypeExt
    {
        /// <summary>
        /// 文字列化
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToStringExt(this VariableType e)
        {
            switch (e)
            {
                case VariableType.FLOAT: return "float";
                case VariableType.FLOAT2: return "float2";
                case VariableType.FLOAT3: return "float3";
                case VariableType.FLOAT4: return "float4";
                default: throw new ArgumentOutOfRangeException("e");
            }
        }
    }
    #endregion
   
    /// <summary>
    /// 単一のリンクデータを表すデータ構造
    /// </summary>
    [Serializable]
    public struct LinkData : IComparable<LinkData>
    {
        public int _outNodeHash;
        public int _outJointIndex;
        public int _inNodeHash;
        public int _inJointIndex;

        public LinkData(int outHash, int outIndex, int inHash, int inIndex)
        {
            _outNodeHash = outHash;
            _outJointIndex = outIndex;
            _inNodeHash = inHash;
            _inJointIndex = inIndex;
        }

#region IComparable<T> interfaces
        public int CompareTo(LinkData other)
        {
            // 以下の順に比較
            // 出力ノードハッシュ
            // 入力ノードハッシュ
            // 出力ジョイントインデックス
            // 入力ジョイントインデックス

            if( _outNodeHash != other._outNodeHash )
            {
               return _outNodeHash.CompareTo(other._outNodeHash);
            }
            else if( _inNodeHash != other._inNodeHash )
            {
                return _inNodeHash.CompareTo(other._inNodeHash);
            }
            else if( _outJointIndex != other._outJointIndex )
            {
                return _outJointIndex.CompareTo(other._outJointIndex);
            }
            else if( _inJointIndex != other._inJointIndex )
            {
                return _inJointIndex.CompareTo(other._inJointIndex);
            }

            // 全て一致しているので等しい
            return 0;
        }
#endregion        
    };    

    /// <summary>
    /// リンクの接続点のデータ構造
    /// </summary>    
    public class JointData
    {
        /// <summary>
        /// サフィックスの種類
        /// </summary>
        public enum SuffixType : int
        {
            None, // サフィックス無し
            X,    // ".x"
            Y,    // ".y"
            Z,    // ".z"
            W     // ".w"
        }

        /// <summary>
        /// ジョイントの種類
        /// </summary>
        public enum Side
        {
            In, // 入力
            Out,// 出力
        }

        #region variables
        /// <summary>
        /// このジョイントを保持しているシェーダノード
        /// </summary>
        ShaderNodeDataBase m_parentNode;

        /// <summary>
        /// 接続先/元のジョイントのリスト
        /// </summary>
        LinkedList<JointData> m_jointList = new LinkedList<JointData>();

        /// <summary>
        /// シェーダノード内のこのジョイントのインデックス
        /// </summary>
        int m_jointIndex;

        /// <summary>
        /// ジョイントの種類（入力か出力）
        /// </summary>
        Side m_side;

        /// <summary>
        /// シェーダ変数の初期型       
        /// </summary>
        VariableType m_defaultVariableType;

        /// <summary>
        /// 変数につくサフィックスの種類
        /// </summary>
        SuffixType m_suffixType;
        #endregion

        #region constructors
        public JointData(ShaderNodeDataBase parentNode, int jointIndex, Side side, VariableType variableType, SuffixType suffixType)
        {
            m_parentNode = parentNode;
            m_jointIndex = jointIndex;
            m_side = side;
            m_defaultVariableType = variableType;
            m_suffixType = suffixType;
        }
        #endregion

        #region properties
        public ShaderNodeDataBase ParentNode
        {
            get { return m_parentNode; }
        }

        /// <summary>
        /// シェーダノード内のジョイントのインデックス
        /// </summary>
        public int JointIndex
        {
            get { return m_jointIndex; }
        }

        /// <summary>
        /// ジョイントの種類
        /// </summary>
        public Side SideType
        {
            get { return m_side; }
        }

        /// <summary>
        /// 接続先のジョイントのリストを取得
        /// </summary>
        public LinkedList<JointData> JointList
        {
            get { return m_jointList; }
        }

        /// <summary>
        /// 初期の変数型
        /// </summary>
        public VariableType DefaultVariableType
        {
            get { return m_defaultVariableType; }
        }

        /// <summary>
        /// 変数名を取得する
        /// </summary>
        public string VariableName
        {
            get
            {
                // 入力に対しては、接続されている出力ジョイントの変数名を取得する
                if (SideType == Side.In)
                {
                    // 入力は2つ以上の出力ジョイントと接続できないため、接続先のジョイントが一意に求まる
                    return m_jointList.First.Value.VariableName;
                }
                else if (SideType == Side.Out)
                {
                    // 出力に関しては、自身のノード名＋サフィックス
                    string[] suffixTable =
                    {
                        "", // 変数全体はサフィックス必要なし
                        ".x",
                        ".y",
                        ".z",
                        ".w",
                    };
                    return ParentNode.VariableName + suffixTable[(int)m_suffixType];
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// サフィックスの種類
        /// </summary>
        public SuffixType Suffix
        {
            get { return m_suffixType; }
        }
        #endregion

        #region public method
        /// <summary>
        /// 接続可能か
        /// </summary>
        public bool CanNewConnect()
        {
            // 入力タイプのジョイントは、1つだけリンク可能           
            return !(SideType == Side.In && m_jointList.Count > 0);
        }

        /// <summary>
        /// ジョイントに接続する
        /// </summary>        
        /// <param name="joint">接続先ジョイント</param>
        public void Connect(JointData joint)
        {
            // 自身と接続先の種類が出力と入力で異なる事を確認
            if (this.SideType == joint.SideType)
            {
                // 同じだったら例外
                throw new ArgumentException("ジョイントのSideTypeが同じです", "joint");
            }

            // 接続先ジョイントを追加
            m_jointList.AddLast(joint);
        }

        /// <summary>
        /// 接続解除が可能か        
        /// </summary>
        /// <returns></returns>
        public bool CanDisconnect(JointData joint)
        {
            // 解除対象と同じジョイントが見つからなければ解除できない
            return (m_jointList.Find(joint) != null);
        }

        /// <summary>
        /// 対象のジョイントへの接続を解除する
        /// </summary>
        /// <param name="joint"></param>
        /// <returns></returns>
        public bool Disconnect(JointData joint)
        {
            // 接続を解除可能か
            if (CanDisconnect(joint) == false)
            {
                // 解除不可能
                return false;
            }

            /// 接続解除 ///
            // リストから要素を削除
            m_jointList.Remove(joint);

            return true;
        }
        #endregion

        #region private method        
        #endregion
    }
}
