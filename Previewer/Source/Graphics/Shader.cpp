/**
	@file Shader.cpp
	@brief シェーダクラス
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "shlwapi.h" // パス用
#include "Graphics/ShaderMan.h"
#include "Graphics/Shader.h"
#include "Graphics/ShaderParameter.h"

namespace opk
{
	namespace shader
	{	
		// Data Type Definitions ---------------------------------------------------------------------
		namespace
		{
			/** 
				@brief インクルードファイルの
			*/			
			class FileD3DXInclude : public ID3DXInclude
			{
			public:
				/// インクルードファイルのOpen時のコールバック				
				virtual COM_DECLSPEC_NOTHROW HRESULT STDMETHODCALLTYPE Open(
					D3DXINCLUDE_TYPE IncludeType,	///< ファイルの場所
					LPCSTR pFileName,				///< ファイルパス
					LPCVOID pParentData,			///< ファイルを格納しているコンテナへのポインタ
					LPCVOID * ppData,				///< 取得バッファへのポインタ
					UINT * pBytes					///< ppDataで返すバッファのサイズ
					)			
				{					
					char path[MAX_PATH];
					// ファイル名を絶対パスへ変換
					if( ::PathIsRelativeA( pFileName ) )
					{
						sprintf_s( path, MAX_PATH, "%s..\\..\\data\\shader\\%s", CApp::GetInstance()->GetApplicationDirectory(), pFileName);						
					}	
					// 絶対パスならそのまま使用
					else
					{
						sprintf_s( path, MAX_PATH, "%s", pFileName );
					}

					// ファイルを開く
					HANDLE handle = ::CreateFileA( path, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL  );

					// サイズ分のバッファを確保
					DWORD dwFileSize = ::GetFileSize( handle, NULL );
					LPVOID pBuffer  = new byte[dwFileSize];

					// ファイルロード
					DWORD dwLoaded;
					::ReadFile( handle, pBuffer, dwFileSize, &dwLoaded, NULL );
					MY_ASSERT( dwFileSize == dwLoaded );

					// ファイルを閉じる
					::CloseHandle( handle );

					// 返す値
					// バッファを返す
					*ppData = pBuffer;
					// サイズを返す
					*pBytes = dwFileSize;

					return S_OK;
				}

				/// インクルードファイルのClose時のコールバック
				virtual COM_DECLSPEC_NOTHROW HRESULT STDMETHODCALLTYPE Close(
						LPCVOID pData		///< OpenコールバックでppDataに格納したポインタ
					)
				{
					// 格納したバッファを削除
					SAFE_DELETE( pData );

					return S_OK;
				}
			};
		} // end of nameless namespace 

		// Global Variable Definitions ---------------------------------------------------------------	
		FileD3DXInclude g_d3dxInclude;

		// Function Definitions ----------------------------------------------------------------------

		//------------------------------------------------------------------------------------------
		CShader::CShader()
			: m_bValid		( false )
			, m_nProfile	( Profile_Max )
			, m_pBuffer		( NULL )
			, m_nSize		( 0 )			
			, m_pD3DConstantTable ( NULL )	
			, m_nCreateType	( CreateType_NotCreated )
		{

		}

		//------------------------------------------------------------------------------------------
		CShader::~CShader()
		{
			// 破棄
			Destroy();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Create( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize )
		{
			HRESULT hr;

			// 破棄
			Destroy();

			// 作成方法を設定
			m_nCreateType = CreateType_FromBuffer;

			// プロファイルを設定
			m_nProfile = i_nProfile;

			// バッファをコピー
			m_pBuffer = new uint8[i_nSize + 1];
			memcpy( m_pBuffer, i_pBuffer, i_nSize );		

			// 終端文字列を追加
			m_pBuffer[i_nSize] = '\0';

			// サイズ(+1は終端文字列分)
			m_nSize = i_nSize + 1;

			// 作成
			V_RETURN( Create_Sub() );
			
			// 有効フラグをたてる
			m_bValid = true;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Create_Sub()
		{
			HRESULT hr;
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// コンパイル後のバッファ
			LPD3DXBUFFER pShaderBuffer = NULL;
			// エラーメッセージ格納用バッファ
			LPD3DXBUFFER pErrorBuffer = NULL;

			// 頂点シェーダ
			if( m_nProfile == Profile_Vertex )
			{
				// コンパイル
				LPCSTR pProfile = D3DXGetVertexShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ 最適化
				hr = D3DXCompileShader( (LPCSTR)m_pBuffer, m_nSize, NULL, &g_d3dxInclude, "vs_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );				

				// 作成
				if( SUCCEEDED(hr) )
				{
					hr = pD3DDevice->CreateVertexShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pVertex );
				}
			}
			// ピクセルシェーダ
			else if( m_nProfile == Profile_Pixel )
			{
				// コンパイル
				LPCSTR pProfile = D3DXGetPixelShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ 最適化

				hr = D3DXCompileShader( (LPCSTR)m_pBuffer, m_nSize, NULL, &g_d3dxInclude, "ps_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );						

				// 作成
				if( SUCCEEDED(hr) )
				{
					hr = pD3DDevice->CreatePixelShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pPixel );
				}				
			}
			// 未定義
			else
			{
				MY_ASSERT( false );
				return S_FALSE;
			}			
			// エラーがあればダイアログで表示
			if( pErrorBuffer )
			{
				MessageBoxA( NULL, (LPCSTR)pErrorBuffer->GetBufferPointer(), "Shader Compile Error", MB_OK);
				MY_TRACE( "%s\n", pErrorBuffer->GetBufferPointer() );
			}					

			MY_ASSERT( SUCCEEDED(hr) );
			
			//バッファ解放
			SAFE_RELEASE( pShaderBuffer );
			SAFE_RELEASE( pErrorBuffer );

			// パラメータの初期化
			return SetupParameters();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::CreateFromFile( Profile i_nProfile, const char* i_pszFllePath )
		{
			HRESULT hr;

			// 破棄
			Destroy();

			// プロファイルを設定
			m_nProfile = i_nProfile;

			// パスをコピー
			strncpy_s( m_szFileName, MAX_PATH, i_pszFllePath, _TRUNCATE );

			// 作成
			V_RETURN( CreateFromFile_Sub() );

			// 有効フラグをたてる
			m_bValid = true;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::CreateFromFile_Sub()
		{
			HRESULT hr;
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// コンパイル後のバッファ
			LPD3DXBUFFER pShaderBuffer = NULL;
			// エラーメッセージ格納用バッファ
			LPD3DXBUFFER pErrorBuffer = NULL;

			// 頂点シェーダ
			if( m_nProfile == Profile_Vertex )
			{
				// コンパイル
				LPCSTR pProfile = D3DXGetVertexShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ 最適化
				hr = D3DXCompileShaderFromFileA( m_szFileName, NULL, NULL, "vs_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );				

				// 作成
				if( SUCCEEDED(hr) )
					hr = pD3DDevice->CreateVertexShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pVertex );
			}
			// ピクセルシェーダ
			else if( m_nProfile == Profile_Pixel )
			{
				// コンパイル
				LPCSTR pProfile = D3DXGetPixelShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ 最適化
				hr = D3DXCompileShaderFromFileA( m_szFileName, NULL, NULL, "ps_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );				

				// 作成
				if( SUCCEEDED(hr) )
					hr = pD3DDevice->CreatePixelShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pPixel );
			}
			// 未定義
			else
			{
				MY_ASSERT( false );
				return S_FALSE;
			}			
			MY_ASSERT( SUCCEEDED(hr) );

			// エラーがあればダイアログで表示
			if( pErrorBuffer )
			{
				MessageBoxA( NULL, (LPCSTR)pErrorBuffer->GetBufferPointer(), "Shader Compile Error", MB_OK);
				MY_TRACE( "%s\n", pErrorBuffer->GetBufferPointer() );
			}					

			//バッファ解放
			SAFE_RELEASE( pShaderBuffer );
			SAFE_RELEASE( pErrorBuffer );

			// パラメータの初期化
			return SetupParameters();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::SetupParameters()
		{
			// 定数全体の記述
			D3DXCONSTANTTABLE_DESC tableDesc;
			m_pD3DConstantTable->GetDesc( &tableDesc );

			// 各定数の記述
			D3DXCONSTANT_DESC desc;

			// 定数の数だけパラメータを作成
			for(uint32 i = 0; i < tableDesc.Constants; ++i )
			{
				uint32 count = 0xFFFFFFFF;

				// ハンドルを取得
				D3DXHANDLE handle = m_pD3DConstantTable->GetConstant( NULL, i );

				// 記述を取得
				m_pD3DConstantTable->GetConstantDesc( handle, &desc, &count );

				// パラメータ名
				std::string strName( desc.Name );

				// パラメータを作成
				CParameterBase* pParam = NULL;				

				switch( desc.Type )
				{
				case D3DXPT_FLOAT: // float値(スカラー・ベクトル・行列)
					{
						// 配列
						if( desc.Elements > 1 )
						{
							// 配列は一括して１次元配列として扱う
							pParam = new CFloatArrayParameter( strName, handle, desc.Bytes / sizeof(float));
						}
						// 非配列
						else
						{
							switch ( desc.Class )
							{
							case D3DXPC_SCALAR:
								pParam = new CFloatParameter( strName, handle );
								break;
							case D3DXPC_VECTOR:							
								pParam = new CVector4Parameter( strName, handle );
								break;
							case D3DXPC_MATRIX_COLUMNS:
								pParam = new CMatrixParameter( strName, handle );
								break;
							default:
								MY_ASSERT_MESS( false, "Invalid Shader Parameter Class");
								break;
							}
						}						
					}
					break;				
				// サンプラー各種
				case D3DXPT_SAMPLER2D:
					pParam = new CTextureParameter( strName, handle, TextureType_2D );
					break;
				case D3DXPT_TEXTURE3D:
					pParam = new CTextureParameter( strName, handle, TextureType_3D );
					break;
				case D3DXPT_SAMPLERCUBE:
					pParam = new CTextureParameter( strName, handle, TextureType_Cube );
					break;
				default:
					MY_ASSERT_MESS( false, "Invalid Shader Parameter Type");
					break;
				}

				// マップへ追加
				m_parameterMap[strName] = shared_ptr<CParameterBase>( pParam );
			}

			return S_OK;
		}
	

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Destroy()
		{
			HRESULT hr;

			// 有効フラグを下ろす
			m_bValid = false;

			V_RETURN( Destroy_Sub() );		

			// シェーダコードのバッファを解放
			SAFE_DELETE_ARRAY( m_pBuffer );
			m_nSize = 0;

			// 作成方法を「未作成」に
			m_nCreateType = CreateType_NotCreated;

			// プロファイルを「無効」に
			m_nProfile = Profile_Max;

			return S_OK;
		}		

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Destroy_Sub()
		{			
			// パラメータの破棄
			m_parameterMap.clear();

			// コンスタントテーブルの解放
			SAFE_RELEASE( m_pD3DConstantTable );

			// 各シェーダの破棄
			if( m_nProfile == Profile_Vertex )
			{
				SAFE_RELEASE( m_d3dShader.pVertex );
			}
			else if( m_nProfile == Profile_Pixel )
			{
				SAFE_RELEASE( m_d3dShader.pPixel );
			}			

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Restore()
		{
			HRESULT hr; 
			
			// 破棄処理のサブルーチンは共通
			V_RETURN( Destroy_Sub() );

			// バッファから作成した場合
			if( m_nCreateType == CreateType_FromBuffer )
			{
				V_RETURN( Create_Sub() );							
			}
			// ファイル名から初期化した場合
			else if( m_nCreateType == CreateType_FromFile )
			{
				V_RETURN( CreateFromFile_Sub() );
			}

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Activate()
		{
			HRESULT hr;

			// 無効なら開始しない
			if( m_bValid == false )
				return E_FAIL;

			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// シェーダをセット
			if( m_nProfile == Profile_Vertex ) 
			{
				V_RETURN( pD3DDevice->SetVertexShader( m_d3dShader.pVertex ) );
			}
			else if( m_nProfile == Profile_Pixel )
			{
				V_RETURN( pD3DDevice->SetPixelShader( m_d3dShader.pPixel ) );
			}

			// パラメータを適用
			TParameterMap::iterator itr = m_parameterMap.begin();
			for( ; itr != m_parameterMap.end(); ++itr )
			{
				if( FAILED( itr->second->Apply( this ) ) )
					return E_FAIL;
			}

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Deactivate()
		{
			HRESULT hr;

			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// シェーダを外す
			if( m_nProfile == Profile_Vertex ) 
			{
				V_RETURN( pD3DDevice->SetVertexShader( NULL ) );
			}
			else if( m_nProfile == Profile_Pixel )
			{
				V_RETURN( pD3DDevice->SetPixelShader( NULL ) );
			}

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		TParameterPtr CShader::FindParameter( const std::string& i_strName )
		{
			// 名前に対応するパラメータを探す
			TParameterMap::iterator itr = m_parameterMap.find(i_strName);

			// 見つからなかった
			if( itr == m_parameterMap.end() )
			{
				return TParameterPtr();
			}
			// 見つかった
			else
			{
				return itr->second;
			}
		}

	} // end of namespace shader
} // end of namespace opk