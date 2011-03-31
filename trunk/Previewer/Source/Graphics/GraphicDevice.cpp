/**
	@file GraphicDevice.cpp
	@brief グラフィックデバイスクラス
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "GraphicDevice.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace 
{	
	/// メッセージ処理関数
	LRESULT WINAPI MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
	{
		return DefWindowProc(hWnd, msg, wParam, lParam);
	}

	// ウィンドウ作成
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

		// D3Dオブジェクトの作成
		m_pd3d9 = Direct3DCreate9(D3D_SDK_VERSION);
		if( !m_pd3d9 )
		{
			return false;
		}

		// ダミーウィンドウを作成
		if( CreateDummyWindow() == false )
		{
			return false;
		}

		// デバイスを作成
		if( CreateDevice(i_nWidth, i_nHeight) == false )
		{
			return false;
		}

		// レンダーターゲットの作成
		if( CreateRenderTarget(i_nWidth, i_nHeight) == false )
		{
			return false;
		}

		// 全てに成功

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
		// Presentation Parameter の初期化
		D3DPRESENT_PARAMETERS d3dpp;
		ZeroMemory( &d3dpp, sizeof(d3dpp) );
		d3dpp.Windowed = TRUE;
		// サイズとフォーマット		
		d3dpp.BackBufferWidth = i_nWidth;
		d3dpp.BackBufferHeight = i_nHeight;
		d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
		d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;
		d3dpp.EnableAutoDepthStencil = TRUE;
		d3dpp.AutoDepthStencilFormat = D3DFMT_D24X8;		

		// デバイス作成
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
		// 破棄処理		
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
		// デバイスのリセット
		return true;;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::Activate()
	{
		// レンダリング開始
		if( m_pd3dDevice9 == NULL )
			return false;
		HRESULT hr = m_pd3dDevice9->BeginScene();
		if( FAILED(hr) )
		{
			return false;
		}

		// レンダーターゲットの設定
		hr = m_pd3dDevice9->SetRenderTarget(0, m_pd3dSurface9);
		
		return SUCCEEDED( hr );
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Deactivate()
	{
		// レンダーターゲットの解除
		HRESULT hr = m_pd3dDevice9->SetRenderTarget(0, NULL);

		// レンダリング終了
		m_pd3dDevice9->EndScene();

		// スワップ
		// m_pd3dDevice9->Present(NULL, NULL, NULL, NULL);
	}
	
	//------------------------------------------------------------------------------------------
	IDirect3DSurface9* CGraphicDevice::GetBackBuffer()
	{
		// バックバッファの取得		
		return m_pd3dSurface9;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Resize(int i_nScreenWidth, int i_nScreenHeight )
	{
		// サーフェースの破棄
		SAFE_RELEASE( m_pd3dSurface9 );
		// サーフェースを新しいサイズで再作成
		CreateRenderTarget( i_nScreenWidth, i_nScreenHeight );
	}
	

} // end of namespace opk