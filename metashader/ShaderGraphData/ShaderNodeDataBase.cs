using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace metashader.ShaderGraphData
{
#region Shader Node Type
    /// <summary>
    /// シェーダーノードの種類
    /// </summary>
    public enum ShaderNodeType : int
    {
        Uniform_Vector4, // 4Dベクトル

        Output_Color, // 出力色
        Max, // 最大数
    };

    /// <summary>
    /// ShaderNodeTypeを拡張するクラス
    /// </summary>
    public static class ShaderNodeTypeExt
    {
        public static string ToStringExt( this ShaderNodeType e )
        {
            switch( e )
            {
                case ShaderNodeType.Uniform_Vector4: return "Uniform_Vector4";
                case ShaderNodeType.Output_Color: return "Output_Color";
                default: throw new ArgumentOutOfRangeException("e");
            }
        }
    }
#endregion      
  
    /// <summary>
    /// リンクの接続点のデータ構造
    /// </summary>    
    public class JointData
    {
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
        #endregion

        #region constructors
        public JointData(ShaderNodeDataBase parentNode, int jointIndex, Side side)
        {
            m_parentNode = parentNode;
            m_jointIndex = jointIndex;
            m_side = side;
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
            if( this.SideType == joint.SideType )
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
            if( CanDisconnect(joint) )
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
        // 内部用対象ジョイントの検索関数。パラメータは、リンク先のハッシュコードと、ジョイントインデックス→リンク先のジョイントを引っ張ってきて参照を直接比較でも行ける気がする
#endregion
    }

    /// <summary>
    /// シェーダグラフを構成するノードのデータ構造の基本クラス  
    /// </summary>
    [Serializable]
    public class ShaderNodeDataBase
    {
#region variables
        /// <summary>
        /// シェーダノードの種類
        /// </summary>
        protected ShaderNodeType m_type;

        /// <summary>
        /// ノード名
        /// </summary>
        protected string m_name;

        /// <summary>
        /// UI上の表示位置
        /// </summary>
        protected Point m_pos;

        /// <summary>
        /// 入力ジョイント
        /// </summary>
        protected JointData[] m_inputJoints;

        /// <summary>
        /// 出力ジョイント
        /// </summary>
        protected JointData[] m_outputJoints;
#endregion        

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public ShaderNodeDataBase( ShaderNodeType type, string name, Point pos, int inJointNum,  int outJointNum)
        {
            m_type = type;
            m_name = name;
            m_pos = pos;

            // ジョイントの初期化
            // 入力
            m_inputJoints = new JointData[inJointNum];
            for(int i = 0; i < inJointNum; ++i)
            {
                m_inputJoints[i] = new JointData(this, i, JointData.Side.In);
            }
            // 出力
            m_outputJoints = new JointData[outJointNum];
            for(int i = 0; i < outJointNum; ++i)
            {
                m_outputJoints[i] = new JointData(this, i, JointData.Side.Out);
            }
        }
#endregion

#region properties     
        /// <summary>
        /// 種類
        /// </summary>
        public ShaderNodeType Type
        {
            get { return m_type; }
        }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Point Position
        {
            get { return m_pos; }
        }        

        /// <summary>
        /// 入力ジョイント数
        /// </summary>
        public int InputJointNum
        {
            get { return m_inputJoints.Length; }
        }        

        /// <summary>
        /// 出力ジョイント数
        /// </summary>
        public int OutputJointNum
        {
            get { return m_outputJoints.Length; }
        }
#endregion

#region public methods
        /// <summary>
        /// 名前からハッシュ値を計算する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public int CalcHashCode( string name )
        {
            return name.GetHashCode();
        }

        /// <summary>
        /// ハッシュコードの取得（GUID用）
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // 名前からハッシュコードを取得する
            return CalcHashCode(m_name);
        }

        /// <summary>
        /// 入力側のジョイントを取得する
        /// </summary>
        /// <param name="index"></param>
        public JointData GetInputJoint( int index )
        {
            return m_inputJoints[index];
        }

        /// <summary>
        /// 出力側のジョイントを取得する
        /// </summary>
        /// <param name="index"></param>
        public JointData GetOutputJoint( int index )
        {
            return m_outputJoints[index];
        }        
#endregion
    }
}
