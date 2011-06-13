/// 
/// 演算(オペレータ)の役割を担うシェーダノードを定義するファイル
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace metashader.ShaderGraphData
{
    [Serializable]
    abstract class Operator_NodeBase : ShaderNodeDataBase
    {
#region constructors
        public Operator_NodeBase(string type, string name, Point pos)
            : base(type, name, pos)
        {            
        }
#endregion

#region properties
        /// <summary>
        /// シェーダーコードに記述するオペレータの文字列
        /// </summary>
        protected abstract string Operator
        {
            get;
        }        
#endregion

#region public methods
        /// <summary>
        /// 入力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetInputJointVariableType(int index)
        {
            // 接続元ノードの出力ジョイントに依存する

            // 接続されていない場合は不定
            if (GetInputJoint(index).JointList.Count == 0)
            {
                return VariableType.INDEFINITE;
            }
            // 接続されていれば接続元に依存
            else
            {
                JointData outputJoint = GetInputJoint(index).JointList.First.Value;
                ShaderNodeDataBase node = outputJoint.ParentNode;

                return node.GetOutputJointVariableType(outputJoint.JointIndex);
            }
        }

        /// <summary>
        /// 出力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetOutputJointVariableType(int index)
        {
            // 自身の入力ジョイントに依存する            
            // デフォルトは、入力ジョイントの型＝出力ジョイントの型とみなす            

            return GetInputJointVariableType(0);
        }        

         /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderMainCode(StringWriter stream)
        {
            // 出力型
            VariableType outputType = GetOutputJointVariableType(0);

            // 入力変数1の名前
            string inputName1 = GetInputJoint(0).VariableName;
            // 入力変数2の名前
            string inputName2 = GetInputJoint(1).VariableName;

            stream.WriteLine("\t{0} {1} = {2} {3} {4};",
                outputType.ToStringExt()
                , Name
                , inputName1
                , Operator
                , inputName2
                );
        }
#endregion     

        #region protected methods
        /// <summary>
        /// ジョイントの初期化
        /// </summary>
        protected override void InitializeJoints()
        {
            // ジョイントの初期化
            // 入力         
            m_inputJointNum = 2;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None, "L");
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None, "R");
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.DEPENDENT, JointData.SuffixType.None);
        }
        #endregion
    }

    /// <summary>
    /// 加算ノード
    /// </summary>
    [Serializable]
    class Operator_AddNode : Operator_NodeBase
    {
        #region constructors
        public Operator_AddNode(string name, Point pos)
            : base("Operator_Add", name, pos)
        {

        }
        #endregion  

#region properties
        /// <summary>
        /// オペレータの文字列
        /// </summary>
        protected override string Operator
        {
            get { return "+"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Add"; }
        }
#endregion
      
#region public methods
         /// <summary>
        /// ノードの有効性を判定
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            // 入力リンクの有効性を確認する
            // 全ての入力が埋まっているか？
            foreach (JointData inputJoint in m_inputJoints)
            {
                if (inputJoint.JointList.Count != 1)
                    return false;
            }

            // 入力型が適当か(演算子に即しているか)
            // 2つの入力型が等しい かつ 未定義でなければOK
            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);
            if ( typeLeft != typeRight || typeLeft == VariableType.INDEFINITE )
                return false;

            return true;
        }        
#endregion         
    }
    
    /// <summary>
    /// 減算ノード
    /// </summary>
    [Serializable]
    class Operator_SubNode : Operator_NodeBase
    {
#region constructors
        public Operator_SubNode(string name, Point pos)
            : base("Operator_Sub", name, pos)
        {

        }
#endregion

#region properties
        /// <summary>
        /// オペレータの文字列
        /// </summary>
        protected override string Operator
        {
            get { return "-"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Subtract"; }
        }
#endregion

#region public methods
        /// <summary>
        /// ノードの有効性を判定
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            // 入力リンクの有効性を確認する
            // 全ての入力が埋まっているか？
            foreach (JointData inputJoint in m_inputJoints)
            {
                if (inputJoint.JointList.Count != 1)
                    return false;
            }

            // 入力型が適当か(演算子に即しているか)
            // 2つの入力型が等しい かつ 未定義でなければOK
            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);
            if (typeLeft != typeRight || typeLeft == VariableType.INDEFINITE)
                return false;

            return true;
        }
#endregion                 
    }

    /// <summary>
    /// 積算ノード
    /// </summary>
    [Serializable]
    class Operator_MulNode : Operator_NodeBase
    {
        #region constructors
        public Operator_MulNode(string name, Point pos)
            : base("Operator_Mul", name, pos)
        {

        }
#endregion

#region properties
        /// <summary>
        /// オペレータの文字列(使用禁止)
        /// </summary>
        protected override string Operator
        {
            get 
            {
                Debug.Fail("積算ノードのOpeartorプロパティは使用禁止");
                return ""; 
            }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Multiply"; }
        }
#endregion

#region public methods
        /// <summary>
        /// 出力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetOutputJointVariableType(int index)
        {
            // 有効なジョイントでなければ、未定義
            if( IsValid() == false )
                return VariableType.INDEFINITE;

            // 自身の入力ジョイントに依存する            
            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);

            VariableType ret = VariableType.INDEFINITE;

            // Scalar x Scalar -> Scalar
            if (typeLeft.IsScalar() && typeRight.IsScalar())
                ret = typeLeft;
            // Scalar x Vector -> Vector
            if (typeLeft.IsScalar() && typeRight.IsVector())
                ret = typeRight;
            // Vector x Scalar -> Vector
            if (typeLeft.IsVector() && typeRight.IsScalar())
                ret = typeLeft;
            // Vector x Vector -> Vector
            // 次元数も同じである必要がある
            if (typeLeft.IsVector() && typeLeft == typeRight)
                ret = typeLeft;

            return ret;
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderMainCode(StringWriter stream)
        {
            // 出力型
            VariableType outputType = GetOutputJointVariableType(0);

            // 入力型
            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);

            // 入力変数1の名前
            string inputName1 = GetInputJoint(0).VariableName;
            // 入力変数2の名前
            string inputName2 = GetInputJoint(1).VariableName;

            // 入力型が同じ場合「*」演算子を使用
            if( typeLeft == typeRight )
            {
                stream.WriteLine("\t{0} {1} = {2} * {3};",
                outputType.ToStringExt()
                , Name
                , inputName1
                , inputName2
                );
            }            
            // 入力型が異なる場合「mul」関数を使用
            else
            {
                stream.WriteLine("\t{0} {1} = mul( {2}, {3} );",
                outputType.ToStringExt()
                , Name
                , inputName1
                , inputName2
                );
            }            
        }
            

        /// <summary>
        /// ノードの有効性を判定
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            // 入力リンクの有効性を確認する
            // 全ての入力が埋まっているか？
            foreach (JointData inputJoint in m_inputJoints)
            {
                if (inputJoint.JointList.Count != 1)
                    return false;
            }

            // 入力型が適当か(演算子に即しているか)            

            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);
            
            // Scalar x Scalar
            if (typeLeft.IsScalar() && typeRight.IsScalar())
                return true;
            // Scalar x Vector
            if (typeLeft.IsScalar() && typeRight.IsVector())
                return true;
            // Vector x Scalar
            if (typeLeft.IsVector() && typeRight.IsScalar())
                return true;
            // Vector x Vector -> Vector
            // 次元数も同じである必要がある
            if (typeLeft.IsVector() && typeLeft == typeRight)
                return true;
            // Vector x Matrix -> Vector // @@ 未実装
            // Matrix x Vector -> Vector // @@ 未実装
            // Matrix x Matrix -> Matrix // @@ 未実装

            // 上記以外は有効でない
            return false;
        }
#endregion            
    }

    /// <summary>
    /// 除算ノード
    /// </summary>
    [Serializable]
    class Operator_DivNode : Operator_NodeBase
    {
#region constructors
        public Operator_DivNode(string name, Point pos)
            : base("Operator_Div", name, pos)
        {

        }
#endregion

        #region properties
        /// <summary>
        /// オペレータの文字列
        /// </summary>
        protected override string Operator
        {
            get { return "/"; }
        }

        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Divide"; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// 出力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override VariableType GetOutputJointVariableType(int index)
        {
            // 有効なジョイントでなければ、未定義
            if (IsValid() == false)
                return VariableType.INDEFINITE;

            // 自身の入力ジョイントに依存する
            // 除算なので、左辺の型が出力型
            return GetInputJointVariableType(0);                        
        }

        /// <summary>
        /// ノードの有効性を判定
        /// </summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            // 入力リンクの有効性を確認する
            // 全ての入力が埋まっているか？
            foreach (JointData inputJoint in m_inputJoints)
            {
                if (inputJoint.JointList.Count != 1)
                    return false;
            }

            // 入力型が適当か(演算子に即しているか)                        
            VariableType typeLeft = GetInputJointVariableType(0);
            VariableType typeRight = GetInputJointVariableType(1);

            // Scalar / Scalar
            if (typeLeft.IsScalar() && typeRight.IsScalar())
                return true;

            // Vector / Scalar            
            if (typeLeft.IsVector() && typeRight.IsScalar())
                return true;
            
            return false;
        }
        #endregion         
    }    
}
