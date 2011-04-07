/**
	@file Shader.cpp
	@brief �V�F�[�_�N���X
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Graphics/ShaderMan.h"
#include "Graphics/Shader.h"
#include "Graphics/ShaderParameter.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace opk
{
	namespace shader
	{
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
			// �j��
			Destroy();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Create( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize )
		{
			HRESULT hr;

			// �j��
			Destroy();

			// �쐬���@��ݒ�
			m_nCreateType = CreateType_FromBuffer;

			// �v���t�@�C����ݒ�
			m_nProfile = i_nProfile;

			// �o�b�t�@���R�s�[
			m_pBuffer = new uint8[i_nSize + 1];
			memcpy( m_pBuffer, i_pBuffer, i_nSize );		

			// �I�[�������ǉ�
			m_pBuffer[i_nSize] = '\0';

			// �T�C�Y(+1�͏I�[������)
			m_nSize = i_nSize + 1;

			// �쐬
			V_RETURN( Create_Sub() );
			
			// �L���t���O�����Ă�
			m_bValid = true;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Create_Sub()
		{
			HRESULT hr;
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// �R���p�C����̃o�b�t�@
			LPD3DXBUFFER pShaderBuffer = NULL;
			// �G���[���b�Z�[�W�i�[�p�o�b�t�@
			LPD3DXBUFFER pErrorBuffer = NULL;

			// ���_�V�F�[�_
			if( m_nProfile == Profile_Vertex )
			{
				// �R���p�C��
				LPCSTR pProfile = D3DXGetVertexShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ �œK��
				hr = D3DXCompileShader( (LPCSTR)m_pBuffer, m_nSize, NULL, NULL, "vs_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );
				MY_ASSERT( SUCCEEDED(hr) );				

				// �쐬
				hr = pD3DDevice->CreateVertexShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pVertex );
			}
			// �s�N�Z���V�F�[�_
			else if( m_nProfile == Profile_Pixel )
			{
				// �R���p�C��
				LPCSTR pProfile = D3DXGetPixelShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ �œK��

				hr = D3DXCompileShader( (LPCSTR)m_pBuffer, m_nSize, NULL, NULL, "ps_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );
				MY_ASSERT( SUCCEEDED(hr) );				

				// �쐬
				hr = pD3DDevice->CreatePixelShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pPixel );
			}
			// ����`
			else
			{
				MY_ASSERT( false );
				return S_FALSE;
			}			
			// �G���[������΃_�C�A���O�ŕ\��
			if( pErrorBuffer )
			{
				MessageBoxA( NULL, (LPCSTR)pErrorBuffer->GetBufferPointer(), "Shader Compile Error", MB_OK);
				MY_TRACE( "%s\n", pErrorBuffer->GetBufferPointer() );
			}					

			MY_ASSERT( SUCCEEDED(hr) );
			
			//�o�b�t�@���
			SAFE_RELEASE( pShaderBuffer );
			SAFE_RELEASE( pErrorBuffer );

			// �p�����[�^�̏�����
			return SetupParameters();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::CreateFromFile( Profile i_nProfile, const char* i_pszFllePath )
		{
			HRESULT hr;

			// �j��
			Destroy();

			// �v���t�@�C����ݒ�
			m_nProfile = i_nProfile;

			// �p�X���R�s�[
			strncpy_s( m_szFileName, MAX_PATH, i_pszFllePath, _TRUNCATE );

			// �쐬
			V_RETURN( CreateFromFile_Sub() );

			// �L���t���O�����Ă�
			m_bValid = true;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::CreateFromFile_Sub()
		{
			HRESULT hr;
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// �R���p�C����̃o�b�t�@
			LPD3DXBUFFER pShaderBuffer = NULL;
			// �G���[���b�Z�[�W�i�[�p�o�b�t�@
			LPD3DXBUFFER pErrorBuffer = NULL;

			// ���_�V�F�[�_
			if( m_nProfile == Profile_Vertex )
			{
				// �R���p�C��
				LPCSTR pProfile = D3DXGetVertexShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ �œK��
				hr = D3DXCompileShaderFromFileA( m_szFileName, NULL, NULL, "vs_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );
				MY_ASSERT( SUCCEEDED(hr) );				

				// �쐬
				hr = pD3DDevice->CreateVertexShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pVertex );
			}
			// �s�N�Z���V�F�[�_
			else if( m_nProfile == Profile_Pixel )
			{
				// �R���p�C��
				LPCSTR pProfile = D3DXGetPixelShaderProfile( pD3DDevice );
				DWORD dwFlags = 0; // @@ �œK��
				hr = D3DXCompileShaderFromFileA( m_szFileName, NULL, NULL, "ps_main",  pProfile, dwFlags, &pShaderBuffer, &pErrorBuffer, &m_pD3DConstantTable );
				MY_ASSERT( SUCCEEDED(hr) );				

				// �쐬
				hr = pD3DDevice->CreatePixelShader( (const DWORD*)pShaderBuffer->GetBufferPointer(), &m_d3dShader.pPixel );
			}
			// ����`
			else
			{
				MY_ASSERT( false );
				return S_FALSE;
			}			
			MY_ASSERT( SUCCEEDED(hr) );

			// �G���[������΃_�C�A���O�ŕ\��
			if( pErrorBuffer )
			{
				MessageBoxA( NULL, (LPCSTR)pErrorBuffer->GetBufferPointer(), "Shader Compile Error", MB_OK);
				MY_TRACE( "%s\n", pErrorBuffer->GetBufferPointer() );
			}					

			//�o�b�t�@���
			SAFE_RELEASE( pShaderBuffer );
			SAFE_RELEASE( pErrorBuffer );

			// �p�����[�^�̏�����
			return SetupParameters();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::SetupParameters()
		{
			// �萔�S�̂̋L�q
			D3DXCONSTANTTABLE_DESC tableDesc;
			m_pD3DConstantTable->GetDesc( &tableDesc );

			// �e�萔�̋L�q
			D3DXCONSTANT_DESC desc;

			// �萔�̐������p�����[�^���쐬
			for(uint32 i = 0; i < tableDesc.Constants; ++i )
			{
				uint32 count = 0xFFFFFFFF;

				// �n���h�����擾
				D3DXHANDLE handle = m_pD3DConstantTable->GetConstant( NULL, i );

				// �L�q���擾
				m_pD3DConstantTable->GetConstantDesc( handle, &desc, &count );

				// �p�����[�^��
				std::string strName( desc.Name );

				// �p�����[�^���쐬
				CParameterBase* pParam = NULL;				

				switch( desc.Type )
				{
				case D3DXPT_FLOAT:
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
					break;
				default:
					MY_ASSERT_MESS( false, "Invalid Shader Parameter Type");
					break;
				}

				// �}�b�v�֒ǉ�
				m_parameterMap[strName] = shared_ptr<CParameterBase>( pParam );
			}

			return S_OK;
		}
	

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Destroy()
		{
			HRESULT hr;

			// �L���t���O�����낷
			m_bValid = false;

			V_RETURN( Destroy_Sub() );		

			// �V�F�[�_�R�[�h�̃o�b�t�@�����
			SAFE_DELETE_ARRAY( m_pBuffer );
			m_nSize = 0;

			// �쐬���@���u���쐬�v��
			m_nCreateType = CreateType_NotCreated;

			// �v���t�@�C�����u�����v��
			m_nProfile = Profile_Max;

			return S_OK;
		}		

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Destroy_Sub()
		{			
			// �p�����[�^�̔j�� @@@ �p�����[�^�N���X�̃f�X�g���N�^�Ăяo����v�m�F
			m_parameterMap.clear();

			// �R���X�^���g�e�[�u���̉��
			SAFE_RELEASE( m_pD3DConstantTable );

			// �e�V�F�[�_�̔j��
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
			
			// �j�������̃T�u���[�`���͋���
			V_RETURN( Destroy_Sub() );

			// �o�b�t�@����쐬�����ꍇ
			if( m_nCreateType == CreateType_FromBuffer )
			{
				V_RETURN( Create_Sub() );							
			}
			// �t�@�C�������珉���������ꍇ
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

			// �����Ȃ�J�n���Ȃ�
			if( m_bValid == false )
				return E_FAIL;

			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// �V�F�[�_���Z�b�g
			if( m_nProfile == Profile_Vertex ) 
			{
				V_RETURN( pD3DDevice->SetVertexShader( m_d3dShader.pVertex ) );
			}
			else if( m_nProfile == Profile_Pixel )
			{
				V_RETURN( pD3DDevice->SetPixelShader( m_d3dShader.pPixel ) );
			}

			// �p�����[�^��K�p
			TParameterMap::iterator itr = m_parameterMap.begin();
			for( ; itr != m_parameterMap.end(); ++itr )
			{
				V_RETURN( itr->second->Apply( this ) );
			}

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CShader::Deactivate()
		{
			HRESULT hr;

			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pD3DDevice = pDevice->GetD3DDevice();	

			// �V�F�[�_���O��
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
			// ���O�ɑΉ�����p�����[�^��T��
			TParameterMap::iterator itr = m_parameterMap.find(i_strName);

			// ������Ȃ�����
			if( itr == m_parameterMap.end() )
			{
				return TParameterPtr();
			}
			// ��������
			else
			{
				return itr->second;
			}
		}

	} // end of namespace shader
} // end of namespace opk