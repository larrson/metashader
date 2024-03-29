﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace metashader.ShaderGraphData
{
    #region Variable Type
    /// <summary>
    /// シェーダ変数の型
    /// </summary>
    public enum VariableType : uint
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

        /// <summary>
        /// スカラー型か判定
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsScalar(this VariableType e)
        {
            return VariableType.FLOAT == e;
        }

        /// <summary>
        /// ベクトル型か判定
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsVector(this VariableType e)
        {
            return VariableType.FLOAT2 <= e && e <= VariableType.FLOAT4;
        }        

        /// <summary>
        /// 次元を取得する
        /// 次元の無いものは返さない（このメソッドの呼び出し自体が不正）
        /// </summary>
        /// <returns></returns>
        public static uint GetDimension(this VariableType e)
        {
            switch( e )
            {
                case VariableType.FLOAT: return 1;
                case VariableType.FLOAT2: return 2;
                case VariableType.FLOAT3: return 3;
                case VariableType.FLOAT4: return 4;
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
            W,     // ".w"
            XYZ,   // ".xyz"
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

        /// <summary>
        /// ラベル
        /// </summary>
        string m_label;
        #endregion

        #region constructors
        /// <summary>
        /// コンストラクタ
        /// ラベル無しバージョン
        /// </summary>
        /// <param name="parentNode">ジョイントが付いているノード</param>
        /// <param name="jointIndex">ノード内のジョイントのインデックス</param>
        /// <param name="side">入力用ジョイント（左側）or 出力用ジョイント（右側）</param>
        /// <param name="variableType">変数の型</param>
        /// <param name="suffixType">シェーダコード生成時のサフィックスの型</param>
        /// <param name="label">UI表示用のラベル</param>
        public JointData(ShaderNodeDataBase parentNode, int jointIndex, Side side, VariableType variableType, SuffixType suffixType)
        {
            m_parentNode = parentNode;
            m_jointIndex = jointIndex;
            m_side = side;
            m_defaultVariableType = variableType;
            m_suffixType = suffixType;
            m_label = "";
        }

        /// <summary>
        /// コンストラクタ
        /// ラベル有りバージョン
        /// </summary>
        /// <param name="parentNode">ジョイントが付いているノード</param>
        /// <param name="jointIndex">ノード内のジョイントのインデックス</param>
        /// <param name="side">入力用ジョイント（左側）or 出力用ジョイント（右側）</param>
        /// <param name="variableType">変数の型</param>
        /// <param name="suffixType">シェーダコード生成時のサフィックスの型</param>
        /// <param name="label">UI表示用のラベル</param>
        public JointData(ShaderNodeDataBase parentNode, int jointIndex, Side side, VariableType variableType, SuffixType suffixType, string label)
        {
            m_parentNode = parentNode;
            m_jointIndex = jointIndex;
            m_side = side;
            m_defaultVariableType = variableType;
            m_suffixType = suffixType;
            m_label = label;
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
                    return ParentNode.VariableName + m_suffixType.ToStringExt();
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

        /// <summary>
        /// UI表示用のラベル
        /// </summary>
        public string Label
        {
            get { return m_label; }
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

    /// <summary>
    /// JointData.SuffixTypeを拡張するクラス
    /// </summary>
    public static class SuffixTypeExt
    {
        /// <summary>
        /// 文字列化
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToStringExt(this JointData.SuffixType e)
        {
            switch (e)
            {
                case JointData.SuffixType.None: return "";
                case JointData.SuffixType.X: return ".x";
                case JointData.SuffixType.Y: return ".y";
                case JointData.SuffixType.Z: return ".z";
                case JointData.SuffixType.W: return ".w";
                case JointData.SuffixType.XYZ: return ".xyz";
                default: throw new ArgumentOutOfRangeException("e");
            }
        }
    }
}
