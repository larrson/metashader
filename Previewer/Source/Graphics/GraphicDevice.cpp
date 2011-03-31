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
		, m_nWidth (0)
		, m_nHeight(0)
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

		// �S�Ăɐ���

		m_nWidth = i_nWidth;
		m_nHeight = i_nHeight;

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
		D3DPRESENT_PARAMETERS d3dpp;
		ZeroMemory( &d3dpp, sizeof(d3dpp) );
		d3dpp.Windowed = TRUE;
		// �T�C�Y�ƃt�H�[�}�b�g		
		d3dpp.BackBufferWidth = i_nWidth;
		d3dpp.BackBufferHeight = i_nHeight;
		d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
		d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;
		d3dpp.EnableAutoDepthStencil = TRUE;
		d3dpp.AutoDepthStencilFormat = D3DFMT_D24X8;		

		// �f�o�C�X�쐬
		HRESULT hr = m_pd3d9->CreateDevice(
				D3DADAPTER_DEFAULT
				, D3DDEVTYPE_HAL
				, m_hWnd
				, D3DCREATE_SOFTWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE
				, &d3dpp				
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
		HRESULT hr = m_pd3dDevice9->CreateRenderTarget(
					i_nWidth
				,	i_nHeight
				,	D3DFMT_A8R8G8B8
				,	D3DMULTISAMPLE_NONE
				,	0
				,	true
				,	&m_pd3dSurface9
				,	NULL
			);
		return SUCCEEDED(hr);
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Dispose()
	{
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
		// �f�o�C�X�̃��Z�b�g
		return true;;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::Activate()
	{
		// �����_�����O�J�n
		if( m_pd3dDevice9 == NULL )
			return false;
		HRESULT hr = m_pd3dDevice9->BeginScene();
		if( FAILED(hr) )
		{
			return false;
		}

		// �����_�[�^�[�Q�b�g�̐ݒ�
		hr = m_pd3dDevice9->SetRenderTarget(0, m_pd3dSurface9);
		
		return SUCCEEDED( hr );
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Deactivate()
	{
		// �����_�[�^�[�Q�b�g�̉���
		HRESULT hr = m_pd3dDevice9->SetRenderTarget(0, NULL);

		// �����_�����O�I��
		m_pd3dDevice9->EndScene();

		// �X���b�v
		// m_pd3dDevice9->Present(NULL, NULL, NULL, NULL);
	}
	
	//------------------------------------------------------------------------------------------
	IDirect3DSurface9* CGraphicDevice::GetBackBuffer()
	{
		// �o�b�N�o�b�t�@�̎擾		
		return m_pd3dSurface9;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Resize(int i_nScreenWidth, int i_nScreenHeight )
	{
		// �T�[�t�F�[�X�̔j��
		SAFE_RELEASE( m_pd3dSurface9 );
		// �T�[�t�F�[�X��V�����T�C�Y�ōč쐬
		CreateRenderTarget( i_nScreenWidth, i_nScreenHeight );
	}
	

} // end of namespace opk