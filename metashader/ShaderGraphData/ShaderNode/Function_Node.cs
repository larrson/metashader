///
/// 関数に関連したノードの定義
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace metashader.ShaderGraphData
{    
    /// <summary>
    /// 正規化ノード
    /// </summary>
    [Serializable]
    class Func_Normalize : ShaderNodeDataBase
    {
#region constructors
        public Func_Normalize(string name, Point pos)
            : base(ShaderNodeType.Func_Normalize, name, pos)
        {

        }
#endregion

#region properties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Normalize"; }
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
            // 出力型は入力ベクトル型
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

            // 入力変数の名前
            string inputName = GetInputJoint(0).VariableName;            

            stream.WriteLine("\t{0} {1} = normalize({2});",
                outputType.ToStringExt()
                , Name
                , inputName                
                );
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
            // 型がベクトル型か否か
            return GetInputJointVariableType(0).IsVector();
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
            m_inputJointNum = 1;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None);            
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.DEPENDENT, JointData.SuffixType.None);
        }
#endregion
    }

    /// <summary>
    /// 内積ノード
    /// </summary>
    [Serializable]
    class Func_Dot : ShaderNodeDataBase
    {
        #region constructors
        public Func_Dot(string name, Point pos)
            : base(ShaderNodeType.Func_Dot, name, pos)
        {

        }
#endregion

#region propeties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Dot Product"; }
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
            // 出力型はスカラー

            return VariableType.FLOAT;
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>        
        public override void WriteShaderMainCode(StringWriter stream)
        {
            // 出力型
            VariableType outputType = GetOutputJointVariableType(0);

            // 入力変数の名前
            string inputName1 = GetInputJoint(0).VariableName;
            string inputName2 = GetInputJoint(1).VariableName;            

            stream.WriteLine("\t{0} {1} = dot( {2}, {3} );",
                outputType.ToStringExt()
                , Name
                , inputName1
                , inputName2
                );
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
            // 型がベクトル型で一致している必要がある  
            VariableType inputType1 = GetInputJointVariableType(0);
            VariableType inputType2 = GetInputJointVariableType(1);

            return inputType1.IsVector() && (inputType1 == inputType2);
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
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None);
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None);            
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.None);
        }
#endregion
    }

    /// <summary>
    /// 反射ベクトルノード
    /// </summary>
    [Serializable]
    class Func_Reflect : ShaderNodeDataBase
    {
        #region constructors
        public Func_Reflect(string name, Point pos)
            : base(ShaderNodeType.Func_Reflect, name, pos)
        {

        }
        #endregion

#region properties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Reflect"; }
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
            // 出力型は入力ベクトル型
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

            // 入力変数の名前
            string inputName1 = GetInputJoint(0).VariableName;
            string inputName2 = GetInputJoint(1).VariableName;

            stream.WriteLine("\t{0} {1} = Reflect( {2}, {3} );",
                outputType.ToStringExt()
                , Name
                , inputName1
                , inputName2
                );
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
            // 型がベクトル型で一致している必要がある  
            VariableType inputType1 = GetInputJointVariableType(0);
            VariableType inputType2 = GetInputJointVariableType(1);

            return inputType1.IsVector() && (inputType1 == inputType2);
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
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None, "In");
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None, "Normal");
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.DEPENDENT, JointData.SuffixType.None);
        }
        #endregion
    }

    [Serializable]
    class Func_Pow : ShaderNodeDataBase
    {
        #region constructors
        public Func_Pow(string name, Point pos)
            : base(ShaderNodeType.Func_Pow, name, pos)
        {

        }
        #endregion

        #region properties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Pow"; }
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
            // 出力型は入力ベクトル型
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

            // 入力変数の名前
            string inputName1 = GetInputJoint(0).VariableName;
            string inputName2 = GetInputJoint(1).VariableName;

            stream.WriteLine("\t{0} {1} = pow( {2}, {3} );",
                outputType.ToStringExt()
                , Name
                , inputName1
                , inputName2
                );
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
            // 型が不定ではなく、一致している必要がある
            VariableType inputType1 = GetInputJointVariableType(0);
            VariableType inputType2 = GetInputJointVariableType(1);

            return inputType1 != VariableType.INDEFINITE && (inputType1 == inputType2);
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
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None, "Base");
            m_inputJoints[1] = new JointData(this, 1, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None, "Exp");
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.DEPENDENT, JointData.SuffixType.None);
        }
        #endregion
    }

    /// <summary>
    /// [0,1]クランプノード
    /// </summary>
    [Serializable]
    class Func_Saturate : ShaderNodeDataBase
    {
        #region constructors
        public Func_Saturate(string name, Point pos)
            : base(ShaderNodeType.Func_Saturate, name, pos)
        {

        }
        #endregion

        #region properties
        /// <summary>
        /// UI上に表示する表示名
        /// </summary>
        public override string Label
        {
            get { return "Saturate"; }
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
            // 出力型は入力ベクトル型
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

            // 入力変数の名前
            string inputName = GetInputJoint(0).VariableName;

            stream.WriteLine("\t{0} {1} = saturate({2});",
                outputType.ToStringExt()
                , Name
                , inputName
                );
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

            // 入力型が適当か          
            // 型がベクトル型orスカラー型ならば有効
            VariableType inputType = GetInputJointVariableType(0);
            return inputType.IsVector() || inputType.IsScalar();
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
            m_inputJointNum = 1;
            m_inputJoints = new JointData[m_inputJointNum];
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.DEPENDENT, JointData.SuffixType.None);
            // 出力            
            m_outputJointNum = 1;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.DEPENDENT, JointData.SuffixType.None);
        }
        #endregion
    }
}
