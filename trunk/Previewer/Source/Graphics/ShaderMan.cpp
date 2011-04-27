/**
	@file ShaderMan.cpp
	@brief シェーダ管理クラス
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Graphics/ShaderMan.h"
#include "Graphics/Shader.h"
#include "Graphics/ShaderParameter.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace
{	
	/// デフォルト頂点シェーダパス(@@@相対パス化)
	static const char* g_pszVSPath = "..\\..\\data\\shader\\simple_vs.fx";	
	/// デフォルトピクセルシェーダパス
	static const char* g_pszPSPath = "..\\..\\data\\shader\\simple_ps.fx";
}

namespace opk
{
	namespace shader
	{
		CShaderMan* CShaderMan::s_pInstance = NULL;

		// Function Definitions ----------------------------------------------------------------------

		//------------------------------------------------------------------------------------------
		CShaderMan::CShaderMan()
			: m_pCurrentShaders(NULL)
			, m_pShaders( NULL )
			, m_pDefaultShaders( NULL )
		{
			Initialize();
		}

		//------------------------------------------------------------------------------------------
		CShaderMan::~CShaderMan()
		{
			Destroy();
		}

		//------------------------------------------------------------------------------------------
		bool CShaderMan::CreateInstance()
		{
			if( !s_pInstance )
			{
				s_pInstance = new CShaderMan();
			}
			return ( s_pInstance != NULL);
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::DisposeInstance()
		{
			SAFE_DELETE( s_pInstance );
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::Initialize()
		{
			char psPath[MAX_PATH];
			char vsPath[MAX_PATH];
			// 頂点シェーダのパスを絶対パスへ変換
			sprintf_s( vsPath, MAX_PATH, "%s%s", CApp::GetInstance()->GetApplicationDirectory(), g_pszVSPath );
			// ピクセルシェーダのパスを絶対パスへ変換
			sprintf_s( psPath, MAX_PATH, "%s%s", CApp::GetInstance()->GetApplicationDirectory(), g_pszPSPath );

			// プロファイルの数だけシェーダを作成
			m_pShaders = new CShader[Profile_Max];
			m_pDefaultShaders = new CShader[Profile_Max];

			// 頂点シェーダはデフォルトを利用
			m_pShaders[Profile_Vertex].CreateFromFile( Profile_Vertex, vsPath);						

			// デフォルトの各シェーダを作成
			m_pDefaultShaders[Profile_Vertex].CreateFromFile( Profile_Vertex, vsPath);
			m_pDefaultShaders[Profile_Pixel ].CreateFromFile( Profile_Pixel , psPath);

			// デフォルトのシェーダを現在のシェーダに設定
			m_pCurrentShaders = m_pDefaultShaders;
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::Destroy()
		{
			// 現在ののシェーダをNULLに設定
			m_pCurrentShaders = NULL;

			// シェーダを破棄
			SAFE_DELETE_ARRAY( m_pShaders );
			SAFE_DELETE_ARRAY( m_pDefaultShaders );
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShaderMan::Activate()
		{			
			// 各シェーダを開始
			for(uint32 i = 0; i < Profile_Max; ++i)
			{
				if( FAILED(m_pCurrentShaders[i].Activate()) )
					return E_FAIL;
			}

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::Deactivate()
		{			
			// 各シェーダを終了
			for(uint32 i = 0; i < Profile_Max; ++i)
			{
				m_pCurrentShaders[i].Deactivate();
			}
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShaderMan::CreateShaderFromBuffer( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize )
		{
			HRESULT hr;

			CShader& rShader = m_pShaders[i_nProfile];

			// 作成前に破棄
			V_RETURN( rShader.Destroy() );

			// 作成
			V_RETURN( rShader.Create( i_nProfile, i_pBuffer, i_nSize ) ) ;

			// 使用するシェーダをデフォルトから変更
			m_pCurrentShaders = m_pShaders;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::SetFloatValue(Profile i_nProfile, const std::string& i_strName, float i_fValue )
		{
			CShader& rShader = m_pShaders[i_nProfile];
			TParameterPtr pParameter = rShader.FindParameter(i_strName);

			// 有効なパラメータが見つかった
			if( pParameter.get() != NULL )
			{
				CFloatParameter* pFloatParam = dynamic_cast<CFloatParameter*>(pParameter.get());
				MY_ASSERT( pFloatParam );
				if( pFloatParam )
					pFloatParam->SetValue( i_fValue );
			}
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::SetVector4Value(Profile i_nProfile, const std::string& i_strName, const D3DXVECTOR4& i_vValue )
		{
			CShader& rShader = m_pShaders[i_nProfile];
			TParameterPtr pParameter = rShader.FindParameter(i_strName);

			// 有効なパラメータが見つかった
			if( pParameter.get() != NULL )
			{
				CVector4Parameter* pVector4Param = dynamic_cast<CVector4Parameter*>(pParameter.get());
				MY_ASSERT( pVector4Param );
				if( pVector4Param )
					pVector4Param->SetValue( i_vValue );
			}
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::SetTexturePath(Profile i_nProfile, const std::string& i_strName, const char* i_pszPath )
		{
			CShader& rShader = m_pShaders[i_nProfile];
			TParameterPtr pParameter = rShader.FindParameter(i_strName);

			// 有効なパラメータが見つかった
			if( pParameter.get() != NULL )
			{
				CTextureParameter* pTexParam = dynamic_cast<CTextureParameter*>(pParameter.get());
				MY_ASSERT( pTexParam );
				if( pTexParam )
					pTexParam->SetPath( i_pszPath );
			}
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::SetSamplerState(Profile i_nProfile, const std::string& i_strName, const SSamplerState& i_samplerState )
		{
			CShader& rShader = m_pShaders[i_nProfile];
			TParameterPtr pParameter = rShader.FindParameter(i_strName);

			// 有効なパラメータが見つかった
			if( pParameter.get() != NULL )
			{
				CTextureParameter* pTexParam = dynamic_cast<CTextureParameter*>(pParameter.get());
				MY_ASSERT( pTexParam );
				if( pTexParam )
					pTexParam->SetSamplerState( i_samplerState );
			}
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::UseDefaultShader()
		{
			// デフォルトシェーダを設定
			m_pCurrentShaders = m_pDefaultShaders;
		}

	} // end of namespace shader
} // end of namespace opk