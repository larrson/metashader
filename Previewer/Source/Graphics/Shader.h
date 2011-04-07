#pragma once
/**
	@file Shader.h
	@brief シェーダクラス
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{	
	namespace shader
	{		
		class CParameterBase;	
		typedef shared_ptr<CParameterBase> TParameterPtr;
		class CShader;
		typedef shared_ptr<CShader> TShaderPtr;		

		/**
			@class CShader
			@brief シェーダクラス
			@note リセットに備えてシェーダコードを保持しておく
		*/
		class CShader
		{
		public:			

		private:
			bool		m_bValid;	///< 有効か
			Profile		m_nProfile; ///< このシェーダのプロファイル			
			uint8*		m_pBuffer;  ///< シェーダコード保持
			uint32		m_nSize;	///< シェーダコードのサイズ（バイト単位）
			
			union D3DShader
			{
				IDirect3DVertexShader9* pVertex;
				IDirect3DPixelShader9* pPixel;
			};
			D3DShader	m_d3dShader;

			LPD3DXCONSTANTTABLE m_pD3DConstantTable; ///< コンスタントテーブル
			
			char m_szFileName[MAX_PATH]; ///< ファイル名（ファイルから作成した場合のため）

			/// 作成時の種類
			enum CreateType
			{
				CreateType_FromBuffer, ///< バッファから作成
				CreateType_FromFile,   ///< ファイルから作成
				CreateType_NotCreated, ///< 未作成
			};			
			CreateType m_nCreateType; ///< 作成時の種類

			typedef std::map<std::string, TParameterPtr > TParameterMap;
			TParameterMap m_parameterMap; ///< パラメータ格納用のマップ

		public:
			/// コンストラクタ
			CShader();

			/// デストラクタ
			~CShader();

			/**
				@brief バッファを指定して作成
				@param [in] i_nProfile	シェーダのプロファイル
				@param [in] i_pBuffer	シェーダコードのバッファ
				@param [in] i_nSize		バッファのサイズ
			*/
			HRESULT Create( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize );

			/**
				@brief ファイルからシェーダを作成
				@param [in] 
			*/
			HRESULT CreateFromFile( Profile i_nProfile, const char* i_pszFllePath );
			
			/**
				@brief 破棄
			*/
			HRESULT Destroy();

			/**
				@brief リストアする
				@note デバイスロストへの対応用
			*/
			HRESULT Restore();

			/**
				@brief シェーダ開始
			*/
			HRESULT Activate();

			/**
				@brief シェーダ終了
			*/
			HRESULT Deactivate();

			/**
				@brief パラメータの検索
			*/
			TParameterPtr FindParameter( const std::string& i_strName );	

			/**
				@brief コンスタントテーブルの取得
			*/
			LPD3DXCONSTANTTABLE GetD3DConstantTable(){ return m_pD3DConstantTable; }

		private:
			/** 
				@brief Createメソッドのサブルーチン		
				@note Restore時の処理との共通化のため
			*/
			HRESULT Create_Sub();

			/**
				@brief CreateFromFileメソッドのサブルーチン
				@note Restore時の処理との共通化のため
			*/
			HRESULT CreateFromFile_Sub();

			/// パラメータのセットアップ
			HRESULT SetupParameters();

			/** 
				@brief Destroyメソッドのサブルーチン		
				@note Restore時の処理との共通化のため
			*/
			HRESULT Destroy_Sub();			
		};			

	} // end of namespace shader
} // end of namespace opk