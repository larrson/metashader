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

			CShader*		   m_pShaders;	///< シェーダ

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
				@brief ベクトル型のパラメータへ値を設定する
				@note 存在しないパラメータ名が指定された場合は、何も行わない
			*/			
			void SetVector4Value(Profile i_nProfile, const std::string& i_strName, const D3DXVECTOR4& i_vValue );

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