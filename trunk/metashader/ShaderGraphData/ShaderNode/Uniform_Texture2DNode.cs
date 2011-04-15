using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Runtime.InteropServices;

namespace metashader.ShaderGraphData
{
    /// <summary>
    /// テクスチャ2Dノード
    /// </summary>
    [Serializable]
    class Uniform_Texture2DNode : ShaderNodeDataBase, IAppliableParameter
    {
#region member classes
        /// <summary>
        /// ラッピングモード
        /// </summary>
        public enum WrapMode : int
		{
			Wrap = 0,	///< ループ
			Mirror, 	///< 反転ループ
			Clamp,		///< 再外色でクランプ
			Border,	    ///< 指定した境界色を使用
			Mirroronce, ///< 1回だけミラーリングし、外周は境界色			
		};

        /// <summary>
        /// フィルタリングモード
        /// </summary>
        public enum FilterMode : int
		{				
			Point = 0,	///< 最近点サンプリング
			Linear,		///< 線形補間
			Ansotropic,	///< 異方性サンプリング			
		};

        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        public struct SamplerState
        {
            public WrapMode m_nWrapU; ///< u座標のラッピングモード
            public WrapMode m_nWrapV; ///< v座標のラッピングモード
            public WrapMode m_nWrapW; ///< w座標のラッピングモード
            public FilterMode m_nMagFilter; ///< 拡大フィルタ
            public FilterMode m_nMinFilter; ///< 縮小フィルタ
            public FilterMode m_nMipFilter; ///< ミップマップフィルター				
            public uint m_nMaxAnisotoropy;	///< 異方性の最大値
            public float m_fBorderColorR;	///< 境界色のR成分
            public float m_fBorderColorG;	///< 境界色のG成分
            public float m_fBorderColorB;	///< 境界色のB成分
            public float m_fBorderColorA;	///< 境界色のA成分                    
        };

#endregion

        #region variables
        /// <summary>
        /// テクスチャファイルのパス
        /// </summary>
        string m_path;
        /// <summary>
        /// サンプラーステート
        /// </summary>
        SamplerState m_samplerState;
        /// <summary>
        /// 変数名
        /// </summary>
        string m_variableName;
        #endregion

        #region constructors
        public Uniform_Texture2DNode(string name, Point pos)
            : base(ShaderNodeType.Uniform_Texture2D, name, pos)
        {
            // 変数名の初期化
            StringBuilder builder = new StringBuilder(name);
            builder.Replace("Uniform", "Color");
            m_variableName = builder.ToString();

            // サンプラステートの初期化
            m_samplerState.m_nWrapU = (WrapMode.Wrap);
            m_samplerState.m_nWrapV = (WrapMode.Wrap);
            m_samplerState.m_nWrapW = (WrapMode.Wrap);
            m_samplerState.m_nMagFilter = (FilterMode.Linear);
            m_samplerState.m_nMinFilter = (FilterMode.Linear);
            m_samplerState.m_nMipFilter = (FilterMode.Linear);
            m_samplerState.m_nMaxAnisotoropy = (1);
            m_samplerState.m_fBorderColorR = (0.0f);
            m_samplerState.m_fBorderColorG = (0.0f);
            m_samplerState.m_fBorderColorB = (0.0f);
            m_samplerState.m_fBorderColorA = (0.0f);
        }
        #endregion

        #region properties
        /// <summary>
        /// テクスチャのファイルパス
        /// </summary>
        public string Path
        {
            get { return m_path; }
            set
            {
                m_path = value;                               
            }
        }

        /// <summary>
        /// テクスチャのサンプラステート
        /// </summary>
        public SamplerState TextureSamplerState
        {
            get { return m_samplerState; }
            set 
            { 
                m_samplerState = value;                                 
            }
        }

        /// <summary>
        /// 出力変数名
        /// </summary>
        public override string VariableName
        {
            get { return m_variableName; }
        }
        #endregion

        #region public methods
        /// <summary>
        /// ストリームへシェーダのuniform宣言を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderUniformCode(StringWriter stream)
        {
            // テクスチャサンプラを宣言する
            stream.WriteLine("uniform sampler \t{0};", Name);
        }

        /// <summary>
        /// ストリームへシェーダの本文を書きこむ
        /// </summary>
        /// <param name="stream"></param>
        public override void WritingShaderMainCode(StringWriter stream)
        {
            // サンプラから色をサンプリングする
            stream.WriteLine("\tfloat4 {0} = tex2D( {1}, {2} );", VariableName, Name, GetInputJoint(0).VariableName);
        }

        /// <summary>
        /// パラメータのPreviewerへの適用
        /// </summary>
        public void ApplyParameter()
        {           
            // テクスチャパスの適用
            ApplyTexturePath();

            // サンプラステートの適用
            ApplySamplerState();
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
            m_inputJoints[0] = new JointData(this, 0, JointData.Side.In, VariableType.FLOAT2, JointData.SuffixType.None);

            // 出力            
            m_outputJointNum = 5;
            m_outputJoints = new JointData[m_outputJointNum];
            m_outputJoints[0] = new JointData(this, 0, JointData.Side.Out, VariableType.FLOAT4, JointData.SuffixType.None);
            m_outputJoints[1] = new JointData(this, 1, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.X);
            m_outputJoints[2] = new JointData(this, 2, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Y);
            m_outputJoints[3] = new JointData(this, 3, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.Z);
            m_outputJoints[4] = new JointData(this, 4, JointData.Side.Out, VariableType.FLOAT, JointData.SuffixType.W);
        }
        #endregion

        #region override methods
        /// <summary>
        /// デシリアライズ時の処理
        /// </summary>
        protected override void OnDeserializationSub()
        {
            // 基底クラスの処理を呼び出し
            base.OnDeserializationSub();

            // ファイルパスを現在の作業フォルダに基づいて絶対パス化
            m_path = App.CurrentApp.FileSettings.ToAbsolutePath(m_path);
        }
        #endregion

        #region private methods
        /// <summary>
        /// テクスチャパスの適用
        /// </summary>
        private void ApplyTexturePath()
        {            
            NativeMethods.SetTexturePath(Name, Path);
        }

        /// <summary>
        /// サンプラーステートの適用
        /// </summary>
        private void ApplySamplerState()
        {
            NativeMethods.SetSamplerState(Name, TextureSamplerState);
        }        
        #endregion
    }
}
