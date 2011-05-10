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
	namespace
	{
		const char* c_pszUniformDirLightDir = "Uniform_DirLightDir_";
		const char* c_pszUniformDirLightColor = "Uniform_DirLightColor_";
		const char* c_pszUniformCameraPosition = "Uniform_Camera_Position";
	}

	namespace shader
	{
		// Data Type Definitions ---------------------------------------------------------------------
		namespace vector4param
		{
			/**
			@brief ���s�����̕����x�N�g���擾�p�t�@���N�^
			*/
			class CDirLightDirFunc : public CVector4Parameter::IFunctor
			{
			private:
				int m_nIndex; ///< �C���f�b�N�X			

			public:
				/// �R���X�g���N�^
				CDirLightDirFunc(int i_nIndex)
					: m_nIndex( i_nIndex )
				{
				}

				/// �f�X�g���N�^
				virtual ~CDirLightDirFunc(){}

				/// �l�擾���\�b�h
				virtual D3DXVECTOR4 GetValue()
				{
					CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
					D3DXVECTOR3 vDir = pDevice->GetDirLightDir(m_nIndex);
					return D3DXVECTOR4( vDir.x, vDir.y, vDir.z, 0.0f );
				}
			};

			/**
			@brief ���s�����̐F�擾�p�t�@���N�^
			*/
			class CDirLightColorFunc : public CVector4Parameter::IFunctor
			{
			private:
				int m_nIndex; ///< �C���f�b�N�X

			public:
				/// �R���X�g���N�^
				CDirLightColorFunc(int i_nIndex)
					: m_nIndex( i_nIndex )
				{
				}

				/// �f�X�g���N�^
				virtual ~CDirLightColorFunc(){}

				/// �l�擾���\�b�h
				virtual D3DXVECTOR4 GetValue()
				{
					CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
					D3DXVECTOR3 vColor = pDevice->GetDirLightColor(m_nIndex);
					return D3DXVECTOR4( vColor.x, vColor.y, vColor.z, 0.0f );
				}
			};				

			/**
				@brief �J�����ʒu�擾�p�t�@���N�^
			*/
			class CCameraPositionFunc : public CVector4Parameter::IFunctor
			{
			public:
				/// �R���X�g���N�^
				CCameraPositionFunc()					
				{
				}

				/// �f�X�g���N�^
				virtual ~CCameraPositionFunc(){}

				/// �l�擾���\�b�h
				virtual D3DXVECTOR4 GetValue()
				{
					CGraphicDevice *pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
					const CGraphicDevice::SCameraInfo cameraInfo = pDevice->GetCameraInfo();
					return D3DXVECTOR4( cameraInfo.vEyePos.x, cameraInfo.vEyePos.y, cameraInfo.vEyePos.z, 0.0f );
				}
			};
		} // end of namespace vector4param

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
		CVector4Parameter::CVector4Parameter(std::string i_strName, D3DXHANDLE i_nHandle )
			: CGeneralParameter<D3DXVECTOR4>( i_strName, i_nHandle )
			, m_pGetValueFunc(NULL)
		{
			// �p�����[�^�̒l��ݒ肷��t�@���N�^�̏�����
			SetupGetValueFunc();
		}

		//------------------------------------------------------------------------------------------
		CVector4Parameter::~CVector4Parameter()
		{
			// �t�@���N�^�̉��
			SAFE_DELETE( m_pGetValueFunc );
		}

		//------------------------------------------------------------------------------------------
		void CVector4Parameter::SetupGetValueFunc()
		{
			MY_ASSERT( m_pGetValueFunc == NULL );

			const std::string& strName = GetName();
			IFunctor *pFunc = NULL;

			// ���s�����̕���
			if( strncmp( strName.c_str(), c_pszUniformDirLightDir, strlen(c_pszUniformDirLightDir) ) == 0)
			{
				int nIndex = atoi( strName.c_str() + strlen(c_pszUniformDirLightDir));
				pFunc = new vector4param::CDirLightDirFunc(nIndex);
			}
			// ���s�����̐F
			else if( strncmp( strName.c_str(), c_pszUniformDirLightColor, strlen(c_pszUniformDirLightColor) ) == 0)
			{
				int nIndex = atoi( strName.c_str() + strlen(c_pszUniformDirLightColor));
				pFunc = new vector4param::CDirLightColorFunc(nIndex);
			}
			else if( strcmp( strName.c_str(), c_pszUniformCameraPosition ) == 0)
			{
				pFunc = new vector4param::CCameraPositionFunc();
			}
			// ���̑�
			else
			{
				// �t�@���N�^���g�p���Ȃ�
			}

			m_pGetValueFunc = pFunc;
		}

		//------------------------------------------------------------------------------------------
		const D3DXVECTOR4* CVector4Parameter::GetValue()
		{		
			// �t�@���N�^���쐬����Ă���΁A������g�p
			if( m_pGetValueFunc )
			{
				m_tValue = m_pGetValueFunc->GetValue();
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