﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO;

namespace metashader.ShaderGraphData
{
#region Shader Node Type
    /// <summary>
    /// シェーダーノードの種類
    /// </summary>
    public enum ShaderNodeType : int
    {
        Uniform_Vector4, // 4Dベクトル

        Operator_Add,   // 加算

        Output_Color,   // 出力色
        Max, // 最大数
    };

    /// <summary>
    /// ShaderNodeTypeを拡張するクラス
    /// </summary>
    public static class ShaderNodeTypeExt
    {
        /// <summary>
        /// 文字列化
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToStringExt( this ShaderNodeType e )
        {
            switch( e )
            {
                case ShaderNodeType.Uniform_Vector4: return "Uniform_Vector4";
                case ShaderNodeType.Operator_Add: return "Operator_Add";
                case ShaderNodeType.Output_Color: return "Output_Color";                
                default: throw new ArgumentOutOfRangeException("e");
            }
        }

        /// <summary>
        /// 各タイプごとのノードの最大数
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static uint GetMaxNodeNum( this ShaderNodeType e )
        {
            switch( e )
            {
                case ShaderNodeType.Uniform_Vector4: return uint.MaxValue;
                case ShaderNodeType.Operator_Add: return uint.MaxValue;
                case ShaderNodeType.Output_Color: return 1;
                default: throw new ArgumentOutOfRangeException("e");
            }
        }
    }
#endregion    

    /// <summary>
    /// シェーダグラフを構成するノードのデータ構造の基本クラス  
    /// </summary>
    [Serializable]
    public abstract class ShaderNodeDataBase : IDeserializationCallback
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
        /// 入力ジョイント数
        /// </summary>
        protected int m_inputJointNum;

        /// <summary>
        /// 入力ジョイント
        /// </summary>
        [NonSerialized]
        protected JointData[] m_inputJoints;

        /// <summary>
        /// 出力ジョイント数
        /// </summary>
        protected int m_outputJointNum;

        /// <summary>
        /// 出力ジョイント
        /// </summary>
        [NonSerialized]
        protected JointData[] m_outputJoints;
#endregion        

#region constructors
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name"></param>
        public ShaderNodeDataBase( ShaderNodeType type, string name, Point pos)
        {
            m_type = type;
            m_name = name;
            m_pos = pos;           

            // ジョイントの初期化
            InitializeJoints();
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
            get { return m_inputJointNum; }
        }        

        /// <summary>
        /// 出力ジョイント数
        /// </summary>
        public int OutputJointNum
        {
            get { return m_outputJointNum; }
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

        /// <summary>
        /// 入力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual VariableType GetInputJointVariableType( int index )
        {
            // 初期に設定した変数型をそのまま返す
         
            if (m_inputJoints[index].DefaultVariableType == VariableType.DEPENDENT)
            {
                // Dependentの場合、サブクラス内で独自に変数型を求めるはずなので、
                // この親クラスの呼び出しは不正
                throw new Exception("サブクラスのGetInputJointVariableTypeが未実装です");
            }

            return m_inputJoints[index].DefaultVariableType;
        }

        /// <summary>
        /// 出力ジョイントに対応する変数型を取得
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual VariableType GetOutputJointVariableType( int index )
        {
            // 初期に設定した変数型をそのまま返す

            if (m_outputJoints[index].DefaultVariableType == VariableType.DEPENDENT)
            {
                // Dependentの場合、サブクラス内で独自に変数型を求めるはずなので、
                // この親クラスの呼び出しは不正
                throw new Exception("サブクラスのGetOutputJointVariableTypeが未実装です");
            }

            return m_outputJoints[index].DefaultVariableType;
        }

        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public virtual void WritingShaderUniformCode(StringWriter stream){}

        /// <summary>
        /// ストリームへシェーダの入力属性を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public virtual void WritingShaderInputCode(StringWriter stream){}

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="localCount">ローカル変数のカウンタ</param>
        public virtual void WritingShaderMainCode(StringWriter stream){}

#if DEBUG
        /// <summary>
        /// デバッグ用のコンソールへの情報表示
        /// </summary>
        public virtual void DebugPrint()
        {
            System.Console.WriteLine("<Node {0}({1})>", Name, GetHashCode());
        }
#endif // DEBUG
#endregion

#region override methods
        void IDeserializationCallback.OnDeserialization(Object sender)
        {
            // ジョイントの初期化
            InitializeJoints();
        }
#endregion

#region protected methods
        protected abstract void InitializeJoints();        
#endregion
    }
}
