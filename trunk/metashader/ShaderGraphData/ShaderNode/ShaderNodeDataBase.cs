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
        Uniform_Float,      // スカラー(浮動小数点値)
        Uniform_Vector2,    // 2Dベクトル
        Uniform_Vector3,    // 3Dベクトル
        Uniform_Vector4,    // 4Dベクトル       
        Uniform_Texture2D,  // 2Dテクスチャ
        Uniform_TextureCube,// Cubeテクスチャ
        
        Input_UV,           // 入力UVベクトル
        Input_Normal,       // 入力法線ベクトル

        Operator_Add,   // 加算
        Operator_Sub,   // 減算
        Operator_Mul,   // 乗算
        Operator_Div,   // 除算

        Func_Normalize, // 正規化
        Func_Dot,       // 内積
        Func_Reflect,   // 反射
        Func_Pow,       // べき乗

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
                case ShaderNodeType.Uniform_Float:   return "Uniform_Float";
                case ShaderNodeType.Uniform_Vector2: return "Uniform_Vector2";
                case ShaderNodeType.Uniform_Vector3: return "Uniform_Vector3";
                case ShaderNodeType.Uniform_Vector4: return "Uniform_Vector4";                    
                case ShaderNodeType.Uniform_Texture2D:  return "Uniform_Texture2D";
                case ShaderNodeType.Uniform_TextureCube: return "Uniform_TextureCube";

                case ShaderNodeType.Input_UV: return "Input_UV";
                case ShaderNodeType.Input_Normal: return "Input_Normal";
                
                case ShaderNodeType.Operator_Add: return "Operator_Add";
                case ShaderNodeType.Operator_Sub: return "Operator_Sub";
                case ShaderNodeType.Operator_Mul: return "Operator_Mul";
                case ShaderNodeType.Operator_Div: return "Operator_Div";

                case ShaderNodeType.Func_Normalize: return "Func_Normalize";
                case ShaderNodeType.Func_Dot: return "Func_Dot";
                case ShaderNodeType.Func_Reflect: return "Func_Reflect";
                case ShaderNodeType.Func_Pow: return "Func_Pow";

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
                case ShaderNodeType.Uniform_Float: return uint.MaxValue;
                case ShaderNodeType.Uniform_Vector2: return uint.MaxValue;
                case ShaderNodeType.Uniform_Vector3: return uint.MaxValue;
                case ShaderNodeType.Uniform_Vector4: return uint.MaxValue;
                case ShaderNodeType.Uniform_Texture2D: return uint.MaxValue;
                case ShaderNodeType.Uniform_TextureCube: return uint.MaxValue;

                case ShaderNodeType.Input_UV: return uint.MaxValue;
                case ShaderNodeType.Input_Normal: return uint.MaxValue;

                case ShaderNodeType.Operator_Add: return uint.MaxValue;
                case ShaderNodeType.Operator_Sub: return uint.MaxValue;
                case ShaderNodeType.Operator_Mul: return uint.MaxValue;
                case ShaderNodeType.Operator_Div: return uint.MaxValue;

                case ShaderNodeType.Func_Normalize: return uint.MaxValue;
                case ShaderNodeType.Func_Dot: return uint.MaxValue;
                case ShaderNodeType.Func_Reflect: return uint.MaxValue;
                case ShaderNodeType.Func_Pow: return uint.MaxValue;

                case ShaderNodeType.Output_Color: return 1;               
                default: throw new ArgumentOutOfRangeException("e");
            }
        }

        /// <summary>
        /// 入力ノードか判定する
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsInputNode( this ShaderNodeType e )
        {            
            return ShaderNodeType.Input_UV <= e && e <= ShaderNodeType.Input_Normal;
        }

        /// <summary>
        /// 演算ノードか判定する
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsOperatorNode( this ShaderNodeType e)
        {
            return ShaderNodeType.Operator_Add <= e && e <= ShaderNodeType.Operator_Div;
        }

        /// <summary>
        /// 出力ノードか判定する
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsOutputNode(this ShaderNodeType e)
        {            
            return ShaderNodeType.Output_Color == e;
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
        /// 説明文
        /// </summary>
        protected string m_description = "";

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
        /// ノードの名前
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }

        /// <summary>
        /// 説明文
        /// </summary>
        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        /// <summary>
        /// 変数名
        /// </summary>
        public virtual string VariableName
        {
            get { return m_name; }
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Point Position
        {
            get { return m_pos; }
            set { m_pos = value;}
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
        /// 入力ジョイントのラベルを取得する
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual string GetInputJointLabel( int index )
        {
            return "";
        }

        /// <summary>
        /// 有効なノードか判定する
        /// @@ エラーメッセージの付加       
        /// </summary>
        public virtual bool IsValid()
        {            
            // 入力リンクの有効性を確認する
            // 全ての入力が埋まっているか？
            foreach(JointData inputJoint in m_inputJoints)
            {
                if( inputJoint.JointList.Count != 1 )
                    return false;
            }
            // 入力リンクの型が繋がっている出力の型と一致しているか
            foreach(JointData inputJoint in m_inputJoints)
            {
                // 入力の型を求める
                VariableType inputType = GetInputJointVariableType(inputJoint.JointIndex);

                // 出力の型を求める
                JointData outputJoint = inputJoint.JointList.First.Value;               
                ShaderNodeDataBase outputNode = outputJoint.ParentNode;                
                VariableType outputType = outputNode.GetOutputJointVariableType(outputJoint.JointIndex);

                if( inputType != outputType )
                {
                    return false;
                }
            }

            return true;
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
        /// <summary>
        /// デシリアライズ時に呼ばれるコールバック
        /// </summary>
        /// <param name="sender"></param>
        void IDeserializationCallback.OnDeserialization(Object sender)
        {
            // ジョイントの初期化
            InitializeJoints();

            // その他のデシリアライズ処理
            OnDeserializationSub();
        }

        /// <summary>
        /// デシリアライズ処理のサブルーチン
        /// 派生クラスでカスタマイズすることを想定
        /// </summary>
        protected virtual void OnDeserializationSub()
        {}
#endregion

#region protected methods
        protected abstract void InitializeJoints();        
#endregion
    }

    /// <summary>
    /// Previewerへのパラメータ適用メソッドを実装するためのインターフェース
    /// 主にUniform型ノード用
    /// </summary>    
    public interface IAppliableParameter
    {
        void ApplyParameter();
    }
}