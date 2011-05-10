/**
	@file GraphicDevice.cpp
	@brief �O���t�B�b�N�f�o�C�X�N���X
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "GraphicDevice.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace 
{	
	/// ���b�Z�[�W�����֐�
	LRESULT WINAPI MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
	{
		return DefWindowProc(hWnd, msg, wParam, lParam);
	}

	// �E�B���h�E�쐬
	WNDCLASSEX wndclass =
	{
		sizeof(WNDCLASSEX),
		CS_CLASSDC,
		MsgProc,
		0L,
		0L,
		GetModuleHandle(NULL),
		NULL,
		NULL,
		NULL,
		NULL,
		L"Previewer",
		NULL
	};
}

namespace opk
{


	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CGraphicDevice::CGraphicDevice()
		: m_pd3d9		(NULL)
		, m_pd3dDevice9	(NULL)	
		, m_pd3dSurface9(NULL)
		, m_hWnd		(0)
		, m_bValid	(false)
		, m_bActive (false)		
		, m_nWidth (0)
		, m_nHeight(0)
		, m_viewport()
		, m_cameraInfo()
	{

	}

	//------------------------------------------------------------------------------------------
	CGraphicDevice::~CGraphicDevice()
	{
		Dispose();
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::Initialize(int i_nWidth, int i_nHeight)
	{
		HRESULT hr = S_OK;

		// D3D�I�u�W�F�N�g�̍쐬
		m_pd3d9 = Direct3DCreate9(D3D_SDK_VERSION);
		if( !m_pd3d9 )
		{
			return false;
		}

		// �_�~�[�E�B���h�E���쐬
		if( CreateDummyWindow() == false )
		{
			return false;
		}

		// �f�o�C�X���쐬
		if( CreateDevice(i_nWidth, i_nHeight) == false )
		{
			return false;
		}

		// �����_�[�^�[�Q�b�g�̍쐬
		if( CreateRenderTarget(i_nWidth, i_nHeight) == false )
		{
			return false;
		}

		/// �S�Ăɐ��� ///

		m_nWidth = i_nWidth;
		m_nHeight = i_nHeight;

		// �r���[�|�[�g�̐ݒ�
		m_viewport.X = 0;
		m_viewport.Y = 0;
		m_viewport.Width = i_nWidth;
		m_viewport.Height = i_nHeight;		
		m_viewport.MinZ = 0.0f;
		m_viewport.MaxZ = 1.0f;

		// �g�����X�t�H�[���̏�����(�P�ʍs��)
		for(int i = 0; i < TransformType_Max; ++i)
		{
			D3DXMatrixIdentity( &m_mTransform[i] );
		}

		// �V�F�[�_�Ǘ��̏�����
		shader::CShaderMan::CreateInstance();
		shader::CShaderMan::GetInstance()->Initialize();

		m_bValid = true;

		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::CreateDummyWindow()
	{				
		if (!RegisterClassEx(&wndclass))
		{
			return false;
		}

		m_hWnd = CreateWindow(			
			L"Previewer",
			L"Previewer",
			WS_OVERLAPPEDWINDOW,
			0,                   // Initial X
			0,                   // Initial Y
			0,                   // Width
			0,                   // Height
			NULL,
			NULL,
			wndclass.hInstance,
			NULL);			

		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::CreateDevice( int i_nWidth, int i_nHeight )
	{
		// Presentation Parameter �̏�����		
		ZeroMemory( &m_d3dpp, sizeof(m_d3dpp) );
		m_d3dpp.Windowed = TRUE;
		// �T�C�Y�ƃt�H�[�}�b�g		
		m_d3dpp.BackBufferWidth = i_nWidth;
		m_d3dpp.BackBufferHeight = i_nHeight;
		m_d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
		m_d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;
		m_d3dpp.EnableAutoDepthStencil = TRUE;
		m_d3dpp.AutoDepthStencilFormat = D3DFMT_D24X8;		

		// �f�o�C�X�쐬
		HRESULT hr = m_pd3d9->CreateDevice(
				D3DADAPTER_DEFAULT
				, D3DDEVTYPE_HAL
				, m_hWnd
				, D3DCREATE_SOFTWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE
				, &m_d3dpp				
				, &m_pd3dDevice9
			);		
		if( FAILED(hr) )
		{
			return false;
		}

		 return true;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::CreateRenderTarget(int i_nWidth, int i_nHeight)
	{
		HRESULT hr;		
		hr = m_pd3dDevice9->CreateRenderTarget(
				i_nWidth
				,	i_nHeight
				,	D3DFMT_A8R8G8B8
				,	D3DMULTISAMPLE_NONE
				,	0
				,	true
				,	&(m_pd3dSurface9)
				,	NULL
				);
		if( FAILED(hr) )
		{
			return false;
		}		

		if( FAILED( m_pd3dDevice9->SetRenderTarget(0, m_pd3dSurface9) ) )
		{
			return false;
		}

		return true;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Dispose()
	{
		// �V�F�[�_�Ǘ��̔j��		
		shader::CShaderMan::DisposeInstance();

		// �j������				
		SAFE_RELEASE( m_pd3dSurface9 );	
		SAFE_RELEASE( m_pd3dDevice9 );
		SAFE_RELEASE( m_pd3d9 );
		
		if( m_hWnd )
		{
			DestroyWindow(m_hWnd);
			UnregisterClass(L"Previewer", NULL);
		}
	}	

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::Reset(int i_nWidth, int i_nHeight )
	{
		HRESULT hr;

		// �T�[�t�F�[�X�̔j��		
		SAFE_RELEASE( m_pd3dSurface9 );		

		// �f�o�C�X�̃��Z�b�g
		// Presentation Parameter �̏�����		
		m_d3dpp.BackBufferWidth = i_nWidth;
		m_d3dpp.BackBufferHeight = i_nHeight;	
		hr = m_pd3dDevice9->Reset( &m_d3dpp );
		MY_ASSERT( SUCCEEDED(hr) );
		
		// �T�[�t�F�[�X��V�����T�C�Y�ōč쐬
		bool ret = CreateRenderTarget( i_nWidth, i_nHeight );
		MY_ASSERT( ret );

		m_nWidth = i_nWidth;
		m_nHeight = i_nHeight;

		// �r���[�|�[�g�̐ݒ�
		m_viewport.X = 0;
		m_viewport.Y = 0;
		m_viewport.Width = i_nWidth;
		m_viewport.Height = i_nHeight;		
		m_viewport.MinZ = 0.0f;
		m_viewport.MaxZ = 1.0f;

		return ret;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::Activate()
	{
		HRESULT hr;

		// ������
		if( m_bValid == false )
		{
			return E_FAIL;
		}	

		V_RETURN( m_pd3dDevice9->BeginScene() );		

		// ���s���t���O���Z�b�g
		m_bActive = true;
		
		// �r���[�|�[�g��ݒ�
		V_RETURN( m_pd3dDevice9->SetViewport(&m_viewport) );

		// �����_�[�X�e�[�g�̐ݒ�
		V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ZENABLE, TRUE ) );
		V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ZWRITEENABLE, TRUE ) );
		V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ZFUNC, D3DCMP_LESSEQUAL ) );

		// �u�����h���[�h�̐ݒ�
		V_RETURN( SetBlendMode( m_nBlendMode, true ) );
		
		return hr;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Deactivate()
	{		
		// �����_�����O�I��
		m_pd3dDevice9->EndScene();	

		// ���s���t���O�����낷
		m_bActive = false;		
	}
	
	//------------------------------------------------------------------------------------------
	IDirect3DSurface9* CGraphicDevice::GetBackBuffer()
	{
		// �o�b�N�o�b�t�@�̎擾				
		return m_pd3dSurface9;
	}
	
	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::SetTransform(TransformType i_nTransformType, D3DXMATRIX i_mMatrix)
	{		
#if !USE_SHADER
		HRESULT hr;
#endif // USE_SHADER

		// IDirect3DDevice9::SetTransform�p�ϊ��e�[�u��
		static const D3DTRANSFORMSTATETYPE tbStateType[TransformType_Max] =
		{
			D3DTS_WORLD,
			D3DTS_VIEW,
			D3DTS_PROJECTION,
		};

		// �V�����s���ێ�
		m_mTransform[ i_nTransformType ] = i_mMatrix;

		// ���s���Ȃ�Z�b�g
		if( m_bActive )
		{
#if !USE_SHADER				
			V_RETURN( m_pd3dDevice9->SetTransform( tbStateType[i_nTransformType], &(m_mTransform[i_nTransformType]) ) );
#endif // USE_SHADER
		}

		return S_OK;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::SetCameraInfo( const SCameraInfo& i_cameraInfo )
	{
		HRESULT hr;

		// �V�����J��������ێ�
		m_cameraInfo = i_cameraInfo;

		/// �V�����J�������Ńr���[�C�ˉe�s����v�Z /// 
		D3DXMATRIX mView, mProj;

		// �r���[�s����v�Z		
		D3DXMatrixLookAtRH( &mView
			, &m_cameraInfo.vEyePos
			, &m_cameraInfo.vInterestPos
			, &m_cameraInfo.vUpDir );

		// �ˉe�s����v�Z
		D3DXMatrixPerspectiveFovRH( &mProj
			, m_cameraInfo.fFov
			, ((float)m_nWidth) / m_nHeight
			, m_cameraInfo.fNear
			, m_cameraInfo.fFar
			);

		// �e�s���ݒ�
		V_RETURN( SetTransform(TransformType_View, mView) );
		V_RETURN( SetTransform(TransformType_Projection, mProj) );

		return S_OK;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::SetBlendMode( BlendMode i_nBlendMode, bool i_bForced )
	{
		HRESULT hr = S_OK;

		// �����ł���΂Ȃɂ����Ȃ�
		if( !i_bForced && m_nBlendMode == i_nBlendMode )
			return hr;
		
		// �V�����l��ێ�
		m_nBlendMode = i_nBlendMode;

		// ���s���Ȃ�Z�b�g
		if( m_bActive )
		{
			switch( i_nBlendMode )
			{
			case BlendMode_None:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE) );
				break;
			case BlendMode_Normal:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE));
				// RGB
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOP, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA ));
				// A
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOPALPHA, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLENDALPHA, D3DBLEND_ONE));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLENDALPHA, D3DBLEND_ONE ));
				break;
			case BlendMode_Add:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE));
				// RGB
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOP, D3DBLENDOP_ADD) );
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE ));
				// A
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOPALPHA, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLENDALPHA, D3DBLEND_ONE));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLENDALPHA, D3DBLEND_ONE ));
				break;
			case BlendMode_Sub:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE));
				// RGB
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOP, D3DBLENDOP_REVSUBTRACT ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE ));
				// A
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOPALPHA, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLENDALPHA, D3DBLEND_ONE));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLENDALPHA, D3DBLEND_ONE ))
				break;
			}			
		}

		return hr;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::SetDirLightEnable( int i_nIndex, bool i_bEnable )
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		m_dirLightInfo[ i_nIndex ].bEnable = i_bEnable;
	}

	//------------------------------------------------------------------------------------------
	D3DXVECTOR3 CGraphicDevice::GetDirLightColor( int i_nIndex )
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		return m_dirLightInfo[ i_nIndex ].vColor;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::SetDirLightColor( int i_nIndex, float i_fR, float i_fG, float i_fB)
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		m_dirLightInfo[ i_nIndex ].vColor.x = i_fR;
		m_dirLightInfo[ i_nIndex ].vColor.y = i_fG;
		m_dirLightInfo[ i_nIndex ].vColor.z = i_fB;
	}

	//------------------------------------------------------------------------------------------
	D3DXVECTOR3 CGraphicDevice::GetDirLightDir( int i_nIndex )
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		return m_dirLightInfo[ i_nIndex ].vDir;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::SetDirLightDir( int i_nIndex, float i_fX, float i_fY, float i_fZ)
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		m_dirLightInfo[ i_nIndex ].vDir.x = i_fX;
		m_dirLightInfo[ i_nIndex ].vDir.y = i_fY;
		m_dirLightInfo[ i_nIndex ].vDir.z = i_fZ;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::Clear(float i_fR, float i_fG, float i_fB, float i_fA)
	{
		HRESULT hr;
		V_RETURN( m_pd3dDevice9->Clear( 0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, D3DCOLOR_ARGB((int)(i_fA * 255), (int)(i_fR * 255), (int)(i_fG * 255), (int)(i_fB * 255)), 1.0f, 0 ) );
		return hr;
	}

} // end of namespace opk