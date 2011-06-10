/**
	@file ShaderParameter.cpp
	@brief �V�F�[�_�p�����[�^�N���X
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Graphics/ShaderMan.h"
#include "Graphics/Shader.h"
#include "Graphics/ShaderParameter.h"

namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------	

	namespace shader
	{
		// Data Type Definitions ---------------------------------------------------------------------		

		// Function Definitions ----------------------------------------------------------------------		

		//------------------------------------------------------------------------------------------
		CParameterBase::CParameterBase( std::string i_strName, D3DXHANDLE i_nHandle )
			: m_strName	( i_strName )
			, m_nHandle ( i_nHandle )			
		{			
		}

		//------------------------------------------------------------------------------------------
		CParameterBase::~CParameterBase()
		{
		}		

		//------------------------------------------------------------------------------------------
		CFloatArrayParameter::CFloatArrayParameter(std::string i_strName, D3DXHANDLE i_nHandle, int i_nElementNum )
			: CParameterBase( i_strName, i_nHandle )
			, m_nElementNum( i_nElementNum )
			, m_pArray( NULL )
			, m_pGetValueFunc( NULL )
		{
			// �z����m��
			m_pArray = new float[m_nElementNum];
			memset( m_pArray, 0, sizeof(float) * m_nElementNum );

			// �p�����[�^�̒l��ݒ肷�郁���o�֐��|�C���^�̏�����
			SetupGetValueFunc();
		}

		//------------------------------------------------------------------------------------------
		CFloatArrayParameter::~CFloatArrayParameter()
		{
			// �p�����[�^�ێ��p�̔z������
			SAFE_DELETE( m_pArray );
		}

		//------------------------------------------------------------------------------------------
		void CFloatArrayParameter::SetupGetValueFunc()
		{
			MY_ASSERT( m_pGetValueFunc == NULL );

			const std::string& strName = GetName();

#define ELSE_IF_GET_VALUE_FUNC(name) \
		} else if( strcmp( strName.c_str(), #name ) == 0 ) { \
		m_pGetValueFunc = &CFloatArrayParameter::GetValue_##name;

			// �_�~�[
			if( false ) 
			{				

				/// ����Ȗ��O�̏ꍇ
				ELSE_IF_GET_VALUE_FUNC( Uniform_DirLightDir )
				ELSE_IF_GET_VALUE_FUNC( Uniform_DirLightCol )

			} else {				
				/// �ʏ�
				m_pGetValueFunc = NULL;
			}	

#undef ELSE_IF_GET_VALUE_FUNC
		}

		//------------------------------------------------------------------------------------------
		HRESULT CFloatArrayParameter::Apply(CShader* i_pShader)
		{
			HRESULT hr;

			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9 *pd3dDevice = pDevice->GetD3DDevice(); MY_ASSERT( pd3dDevice );

			// �L���ȃ����o�֐��|�C���^������΂���Œl��ݒ�
			if( m_pGetValueFunc )
			{
				(this->*m_pGetValueFunc)();
			}

			ID3DXConstantTable* pConstantTable = i_pShader->GetD3DConstantTable();
			V_RETURN( pConstantTable->SetFloatArray( pd3dDevice, m_nHandle, m_pArray, m_nElementNum ) );

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		void CFloatArrayParameter::GetValue_Uniform_DirLightDir()
		{
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			for(int i = 0; i < CGraphicDevice::DirLight_Max; ++i )
			{
				// �V�F�[�_���ł̌v�Z���Ȃ����߂ɁA���]���đ���
				D3DXVECTOR3 v = pDevice->GetDirLightDir(i);
				m_pArray[ i * 3 + 0 ] = -v.x;
				m_pArray[ i * 3 + 1 ] = -v.y;
				m_pArray[ i * 3 + 2 ] = -v.z;
			}
		}

		//------------------------------------------------------------------------------------------
		void CFloatArrayParameter::GetValue_Uniform_DirLightCol()
		{
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			for(int i = 0; i < CGraphicDevice::DirLight_Max; ++i )
			{
				D3DXVECTOR3 v = pDevice->GetDirLightColor(i);
				m_pArray[ i * 3 + 0 ] = v.x;
				m_pArray[ i * 3 + 1 ] = v.y;
				m_pArray[ i * 3 + 2 ] = v.z;
			}
		}

		//------------------------------------------------------------------------------------------
		CVector4Parameter::CVector4Parameter(std::string i_strName, D3DXHANDLE i_nHandle )
			: CGeneralParameter<D3DXVECTOR4>( i_strName, i_nHandle )
			, m_pGetValueFunc(NULL)
		{
			// �p�����[�^�̒l��ݒ肷�郁���o�֐��|�C���^�̏�����
			SetupGetValueFunc();
		}

		//------------------------------------------------------------------------------------------
		CVector4Parameter::~CVector4Parameter()
		{						
		}

		//------------------------------------------------------------------------------------------
		void CVector4Parameter::SetupGetValueFunc()
		{
			MY_ASSERT( m_pGetValueFunc == NULL );

			const std::string& strName = GetName();
			
#define ELSE_IF_GET_VALUE_FUNC(name) \
		} else if( strcmp( strName.c_str(), #name ) == 0 ) { \
		m_pGetValueFunc = &CVector4Parameter::GetValue_##name;

			// �_�~�[
			if( false ) 
			{				

			/// ����Ȗ��O�̏ꍇ
			ELSE_IF_GET_VALUE_FUNC( Uniform_CameraPosition )

			} else {				
				/// �ʏ�
				m_pGetValueFunc = NULL;
			}	

#undef ELSE_IF_GET_VALUE_FUNC
		}

		//------------------------------------------------------------------------------------------
		void CVector4Parameter::GetValue_Uniform_CameraPosition()
		{
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			const CGraphicDevice::SCameraInfo cameraInfo = pDevice->GetCameraInfo();
			m_tValue.x = cameraInfo.vEyePos.x;
			m_tValue.y = cameraInfo.vEyePos.y;
			m_tValue.z = cameraInfo.vEyePos.z;
			m_tValue.w = 0.0f;
		}

		//------------------------------------------------------------------------------------------
		const D3DXVECTOR4* CVector4Parameter::GetValue()
		{		
			// �֐��|�C���^���ݒ肳��Ă���΁A������g�p���A�����o�ϐ����X�V
			if( m_pGetValueFunc )
			{
				(this->*m_pGetValueFunc)();
			}			
			
			return &m_tValue;
		}		

		//------------------------------------------------------------------------------------------
		CMatrixParameter::CMatrixParameter(std::string i_strName, D3DXHANDLE i_nHandle )
			: CGeneralParameter<D3DXMATRIX>( i_strName, i_nHandle)
			, m_pGetValueFunc(NULL)
		{
			// �p�����[�^�̒l��ݒ肷��֐��̃|�C���^��������
			SetupGetValueFunc();
		}

		//------------------------------------------------------------------------------------------
		CMatrixParameter::~CMatrixParameter()
		{			
		}

		//------------------------------------------------------------------------------------------
		void CMatrixParameter::SetupGetValueFunc()
		{		
			const std::string& strName = GetName();

#define ELSE_IF_GET_VALUE_FUNC(name) \
		} else if( strcmp( strName.c_str(), #name ) == 0 ) { \
			m_pGetValueFunc = &CMatrixParameter::GetValue_##name;
			
			// �_�~�[
			if( false ) 
			{				

			/// ����Ȗ��O�̏ꍇ
			ELSE_IF_GET_VALUE_FUNC( mWorld )
			ELSE_IF_GET_VALUE_FUNC( mView )
			ELSE_IF_GET_VALUE_FUNC( mProjection )

			} else {				
			/// �ʏ�
				m_pGetValueFunc = &CMatrixParameter::GetThisValue;
			}

#undef ELSE_IF_GET_VALUE_FUNC
		}

		//------------------------------------------------------------------------------------------
		const D3DXMATRIX* CMatrixParameter::GetValue_mWorld()
		{
			CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();

			return &(pDevice->GetTransform(CGraphicDevice::TransformType_World));
		}

		//------------------------------------------------------------------------------------------
		const D3DXMATRIX* CMatrixParameter::GetValue_mView()
		{
			CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();

			return &(pDevice->GetTransform(CGraphicDevice::TransformType_View));
		}

		//------------------------------------------------------------------------------------------
		const D3DXMATRIX* CMatrixParameter::GetValue_mProjection()
		{
			CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();

			return &(pDevice->GetTransform(CGraphicDevice::TransformType_Projection));
		}

		//------------------------------------------------------------------------------------------
		CTextureParameter::CTextureParameter( std::string i_strName, D3DXHANDLE i_nHandle, TextureType i_nTextureType )
			: CParameterBase( i_strName, i_nHandle )
			, m_nTextureType( i_nTextureType )
			, m_strPath()
			, m_pd3dBaseTexture ( NULL )
			, m_bValid( false )
			, m_samplerState()
		{			
		}

		//------------------------------------------------------------------------------------------
		CTextureParameter::~CTextureParameter()
		{
			// �j��
			Destroy();
		}

		//------------------------------------------------------------------------------------------
		HRESULT CTextureParameter::Destroy()
		{
			HRESULT hr;

			V_RETURN( Destroy_Sub() );			

			// �e�N�X�`���̎�ނ�������
			m_nTextureType = TextureType_Max;

			// �T���v���[�X�e�[�g��������
			m_samplerState = SSamplerState();

			// �e�N�X�`���p�X�����
			m_strPath.clear();							

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CTextureParameter::Destroy_Sub()
		{
			// ������
			m_bValid = false;

			// DirectX�̃e�N�X�`�������
			SAFE_RELEASE( m_pd3dBaseTexture );

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CTextureParameter::Apply(CShader* i_pShader)
		{
			HRESULT hr;

			// ���b�s���O���[�h�̕ϊ��e�[�u��
			static int wrapTable[] =
			{
				D3DTADDRESS_WRAP,
				D3DTADDRESS_MIRROR,
				D3DTADDRESS_CLAMP,
				D3DTADDRESS_BORDER,
				D3DTADDRESS_MIRROR,
			};

			// �t�B���^�����O���[�h�̕ϊ��e�[�u��
			static int filterTable[] =
			{
				D3DTEXF_POINT,
				D3DTEXF_LINEAR,
				D3DTEXF_ANISOTROPIC,
			};

			if( m_bValid == false )
				return E_FAIL;

			// �f�o�C�X
			CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
			IDirect3DDevice9* pD3DDevice = pDevice->GetD3DDevice();

			// �萔�e�[�u��
			LPD3DXCONSTANTTABLE pD3DConstantTable = i_pShader->GetD3DConstantTable();

			// �T���v���C���f�b�N�X
			uint32 nSamplerIndex = pD3DConstantTable->GetSamplerIndex( m_nHandle );

			// �T���v���[�X�e�[�g�̐ݒ�			
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_ADDRESSU, wrapTable[m_samplerState.m_nWrapU] ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_ADDRESSV, wrapTable[m_samplerState.m_nWrapV] ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_ADDRESSW,	wrapTable[m_samplerState.m_nWrapW] ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_MAGFILTER, filterTable[m_samplerState.m_nMagFilter] ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_MINFILTER, filterTable[m_samplerState.m_nMinFilter] ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_MIPFILTER, filterTable[m_samplerState.m_nMipFilter] ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_MAXANISOTROPY, m_samplerState.m_nMaxAnisotoropy ));
			V_RETURN( pD3DDevice->SetSamplerState( nSamplerIndex, D3DSAMP_BORDERCOLOR, D3DCOLOR_ARGB((uint32)(m_samplerState.m_fBorderColorA * 255), (uint32)(m_samplerState.m_fBorderColorR * 255), (uint32)(m_samplerState.m_fBorderColorG * 255), (uint32)(m_samplerState.m_fBorderColorB * 255))));

			// �e�N�X�`���̓K�p
			V_RETURN( pD3DDevice->SetTexture( nSamplerIndex, m_pd3dBaseTexture ) );

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CTextureParameter::SetPath( const char* i_pszPath )
		{
			// ������������Z�b�g���Ȃ�
			if( strcmp( m_strPath.c_str(), i_pszPath ) == 0 )
				return S_OK;

			// ���ݕێ����Ă���e�N�X�`����j��
			Destroy_Sub();

			// �V�����p�X��ݒ�
			m_strPath = std::string(i_pszPath);

			// �V�����p�X�Ńe�N�X�`�����쐬			
			if( FAILED(CreateTexture()) )
			{
				// ���s�����疳���Ɛݒ肷��
				m_bValid = false;
				return E_FAIL;
			}

			// �쐬�����̂Ńp�����[�^�Ƃ��ėL��
			m_bValid = true;

			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CTextureParameter::SetSamplerState( const SSamplerState& samplerState )
		{
			// �V�����T���v���[�X�e�[�g��ݒ�
			m_samplerState = samplerState;
			return S_OK;
		}

		//------------------------------------------------------------------------------------------
		bool CTextureParameter::CanLoadTexture( const char* i_pszPath, TextureType i_nTextureType )
		{
			// �摜�t�@�C���̏����擾
			D3DXIMAGE_INFO info;
			if( FAILED(D3DXGetImageInfoFromFileA( i_pszPath, &info ) ) )
			{
				return false; // �����擾�ł��Ȃ������ˑΉ�����摜�t�@�C���ł͂Ȃ�
			}
			
			// ��ނ𔻒�
			bool bTypeValidate = false;
			switch( info.ResourceType )
			{
			case D3DRTYPE_TEXTURE:
				bTypeValidate = (i_nTextureType == TextureType_2D );
				break;
			case D3DRTYPE_VOLUMETEXTURE:
				bTypeValidate = (i_nTextureType == TextureType_3D );
				break;
			case D3DRTYPE_CUBETEXTURE:
				bTypeValidate = (i_nTextureType == TextureType_Cube );
				break;
			}

			return bTypeValidate;
		}

		//------------------------------------------------------------------------------------------
		HRESULT CTextureParameter::CreateTexture()
		{
			HRESULT hr;			

			// �摜�t�@�C���ƃ^�C�v����v���Ă��鎖���m�F
			if( CanLoadTexture( m_strPath.c_str(), m_nTextureType ) == false )
			{				
				return E_FAIL;
			}

			// �O���t�B�b�N�f�o�C�X
			CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT(pDevice);
			IDirect3DDevice9* pD3DDevice = pDevice->GetD3DDevice();

			// ���\�[�X�̎�ނɍ��킹�ďꍇ����
			switch( m_nTextureType )
			{
			case TextureType_2D:
				{
					IDirect3DTexture9* pD3DTexture;					
					V_RETURN( D3DXCreateTextureFromFileA( pD3DDevice, m_strPath.c_str(), &pD3DTexture ));
					m_pd3dBaseTexture = pD3DTexture;
				}								
				break;
			case TextureType_3D:
				{
					IDirect3DVolumeTexture9* pD3DTexture;					
					V_RETURN( D3DXCreateVolumeTextureFromFileA( pD3DDevice, m_strPath.c_str(), &pD3DTexture ) );
					m_pd3dBaseTexture = pD3DTexture;
				}				
				break;
			case TextureType_Cube:
				{
					IDirect3DCubeTexture9* pD3DTexture;					
					V_RETURN( D3DXCreateCubeTextureFromFileA( pD3DDevice, m_strPath.c_str(), &pD3DTexture ) );
					m_pd3dBaseTexture = pD3DTexture;
				}				
				break;
			}

			return S_OK;
		}
	} // end of namespace shader
} // end of namespace opk