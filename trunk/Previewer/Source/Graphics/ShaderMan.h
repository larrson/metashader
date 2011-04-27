#pragma once
/**
	@file ShaderMan.h
	@brief シェーダ管理クラス
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{	
	namespace shader
	{
		/// プロファイル
		enum Profile
		{
			Profile_Vertex, ///< 頂点シェーダ
			Profile_Pixel, ///< ピクセルシェーダ
			Profile_Max,
		};

		/**
		@brief テクスチャの種類
		*/
		enum TextureType
		{
			TextureType_2D,		///< 2Dテクスチャ				
			TextureType_3D,		///< 3Dテクスチャ
			TextureType_Cube,	///< キューブマップ
			TextureType_Max,
		};		

		/**
		@brife ラッピングモード
		@note http://www.t-pot.com/books/DirectX_f.htmを参照
		*/
		enum WrapMode
		{
			WrapMode_Wrap = 0,	///< ループ
			WrapMode_Mirror,	///< 反転ループ
			WrapMode_Clamp,		///< 再外色でクランプ
			WrapMode_Border,	///< 指定した境界色を使用
			WrapMode_Mirroronce,///< 1回だけミラーリングし、外周は境界色
			WrapMode_Max,		///< 最大数
		};

		/**
		@brief フィルタリングモード
		*/
		enum FilterMode
		{				
			FilterMode_Point = 0,	///< 最近点サンプリング
			FilterMode_Linear,		///< 線形補間
			FilterMode_Ansotropic,	///< 異方性サンプリング
			FilterMode_Max,			///< 最大数
		};

		/**
		@brief サンプラーステート
		*/
		struct SSamplerState
		{
			WrapMode	m_nWrapU; ///< u座標のラッピングモード
			WrapMode	m_nWrapV; ///< v座標のラッピングモード
			WrapMode	m_nWrapW; ///< w座標のラッピングモード
			FilterMode	m_nMagFilter; ///< 拡大フィルタ
			FilterMode	m_nMinFilter; ///< 縮小フィルタ
			FilterMode	m_nMipFilter; ///< ミップマップフィルター				
			uint32		m_nMaxAnisotoropy;	///< 異方性の最大値
			float		m_fBorderColorR;	///< 境界色のR成分
			float		m_fBorderColorG;	///< 境界色のG成分
			float		m_fBorderColorB;	///< 境界色のB成分
			float		m_fBorderColorA;	///< 境界色のA成分


			/**
			@brife コンストラクタ
			@note デフォルト値用
			*/
			SSamplerState()
				: m_nWrapU( WrapMode_Wrap )
				, m_nWrapV( WrapMode_Wrap )
				, m_nWrapW( WrapMode_Wrap )
				, m_nMagFilter( FilterMode_Linear )
				, m_nMinFilter( FilterMode_Linear )
				, m_nMipFilter( FilterMode_Linear )
				, m_nMaxAnisotoropy( 1 )
				, m_fBorderColorR ( 0.0f )
				, m_fBorderColorG ( 0.0f )
				, m_fBorderColorB ( 0.0f )
				, m_fBorderColorA ( 0.0f )
			{}
		};

		class CShader;

		/**
			@class CShaderMan
			@brief シェーダ管理クラス
			@note シングルトン
		*/
		class CShaderMan
		{
		private:
			static CShaderMan* s_pInstance; ///< シングルトンのインスタンス			

			CShader*		   m_pCurrentShaders;	///< 現在使用中のシェーダの配列
			CShader*		   m_pShaders;			///< シェーダ
			CShader*		   m_pDefaultShaders;	///< メタシェーダによって作成されたシェーダが使用できない場合用

		public:
			/// インスタンスの取得
			static CShaderMan* GetInstance(){ return s_pInstance; }

			/// インスタンスの生成
			static bool CreateInstance();

			/// インスタンスの破棄
			static void DisposeInstance();

			/**
				@brief 初期化
			*/
			void Initialize();

			/**
				@brief シェーダを開始
			*/
			HRESULT Activate();

			/**
				@brief シェーダを終了
			*/
			void Deactivate();

			/**
				@brief バッファを指定してシェーダを作成する
				@param [in] i_nProfile	プロファイル
				@param [in] i_pBuffer	シェーダコードが格納されているバッファ
				@param [in] i_nSize		バッファのサイズ
				@retval エラーコード
			*/
			HRESULT CreateShaderFromBuffer( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize );

			/**
				@brief float型のパラメータへ値を設定する
				@note 存在しないパラメータ名が指定された場合は、何も行わない
			*/
			void SetFloatValue(Profile i_nProfile, const std::string& i_strname, float i_fValue );

			/**
				@brief ベクトル型のパラメータへ値を設定する
				@note 存在しないパラメータ名が指定された場合は、何も行わない
			*/			
			void SetVector4Value(Profile i_nProfile, const std::string& i_strName, const D3DXVECTOR4& i_vValue );

			/**
				@brief テクスチャ型のパラメータへテクスチャファイルのパスを設定する
				@note 存在しないパラメータ名が指定された場合は、何も行わない
			*/
			void SetTexturePath(Profile i_nProfile, const std::string& i_strName, const char* i_pszPath );

			/**
				@brief テクスチャ型のパラメータへサンプラーステートを設定する
				@note 存在しないパラメータ名が指定された場合は、何も行わない
			*/
			void SetSamplerState(Profile i_nProfile, const std::string& i_strName, const SSamplerState& i_samplerState );

			/**
				@brief デフォルトシェーダを利用する
			*/
			void UseDefaultShader();

		private:
			/**
				@brief コンストラクタ
				@note シングルトンのため外部から隠蔽
			*/
			CShaderMan();

			/**
				@brief デストラクタ
				@note シングルトンのため外部から隠蔽
			*/
			~CShaderMan();

			/**
				@brief コピーコンストラクタの禁止
			*/
			CShaderMan( const CShaderMan& );

			/**
				@brief 代入演算子の禁止
			*/
			CShaderMan& operator=( const CShaderMan& );			

			/**
				@brief 破棄
			*/
			void Destroy();			
		};
	} // end of namespace shader
} // end of namespace opk