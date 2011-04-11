/**
	@file ShaderMan.cpp
	@brief �V�F�[�_�Ǘ��N���X
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Graphics/ShaderMan.h"
#include "Graphics/Shader.h"
#include "Graphics/ShaderParameter.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace
{
	/// ���_�V�F�[�_�p�X(@@@���΃p�X��)
	static const char* g_pszVSPath = "C:\\projects\\metashader\\data\\shader\\simple_vs.fx";	
}

namespace opk
{
	namespace shader
	{
		CShaderMan* CShaderMan::s_pInstance = NULL;

		// Function Definitions ----------------------------------------------------------------------

		//------------------------------------------------------------------------------------------
		CShaderMan::CShaderMan()
			: m_pShaders( NULL )
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
			// �v���t�@�C���̐������V�F�[�_���쐬
			m_pShaders = new CShader[Profile_Max];

			// ���_�V�F�[�_�̂݃t�@�C������쐬						 
			m_pShaders[Profile_Vertex].CreateFromFile( Profile_Vertex, g_pszVSPath);						
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::Destroy()
		{
			// �V�F�[�_��j��
			SAFE_DELETE_ARRAY( m_pShaders );
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShaderMan::Activate()
		{			
			// �e�V�F�[�_���J�n
			for(uint32 i = 0; i < Profile_Max; ++i)
			{
				if( FAILED(m_pShaders[i].Activate()) )
					return E_FAIL;
			}

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::Deactivate()
		{			
			// �e�V�F�[�_���I��
			for(uint32 i = 0; i < Profile_Max; ++i)
			{
				m_pShaders[i].Deactivate();
			}
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShaderMan::CreateShaderFromBuffer( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize )
		{
			HRESULT hr;

			CShader& rShader = m_pShaders[i_nProfile];

			// �쐬�O�ɔj��
			V_RETURN( rShader.Destroy() );

			// �쐬
			V_RETURN( rShader.Create( i_nProfile, i_pBuffer, i_nSize ) ) ;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		void CShaderMan::SetVector4Value(Profile i_nProfile, const std::string& i_strName, const D3DXVECTOR4& i_vValue )
		{
			CShader& rShader = m_pShaders[i_nProfile];
			TParameterPtr pParameter = rShader.FindParameter(i_strName);

			// �L���ȃp�����[�^����������
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

			// �L���ȃp�����[�^����������
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

			// �L���ȃp�����[�^����������
			if( pParameter.get() != NULL )
			{
				CTextureParameter* pTexParam = dynamic_cast<CTextureParameter*>(pParameter.get());
				MY_ASSERT( pTexParam );
				if( pTexParam )
					pTexParam->SetSamplerState( i_samplerState );
			}
		}

	} // end of namespace shader
} // end of namespace opk