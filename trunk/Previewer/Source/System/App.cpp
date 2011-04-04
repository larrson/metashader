/**
	@file App.cpp
	@brief アプリケーションクラス	
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "App.h"


namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------
	CApp* CApp::s_pInstance = NULL;

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CApp::CApp()
		: m_pd3dTexture(NULL)
		, m_pGraphicDevice(NULL)
		, m_model(L"C:\\projects\\metashader\\data\\model\\teapot.x") ///< モデル @@@削除
	{

	}

	//------------------------------------------------------------------------------------------
	CApp::~CApp()
	{
		SAFE_DELETE( m_pGraphicDevice );
	}

	//------------------------------------------------------------------------------------------
	bool CApp::CreateInstance()
	{
		if( !s_pInstance )
		{
			s_pInstance = new CApp();
		}
		return (s_pInstance != NULL );
	}

	//------------------------------------------------------------------------------------------
	void CApp::DisposeInstance()
	{				
		SAFE_DELETE( s_pInstance );
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Initialize(LPWSTR i_lpCmdLine, int i_nScreenWidth, int i_nScreenHeight)
	{						
		// デバイスの初期化
		m_pGraphicDevice = new CGraphicDevice();
		m_pGraphicDevice->Initialize(i_nScreenWidth, i_nScreenHeight);

		m_model.Restore(); ///< モデル @@@削除		

		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::ResetDevice(int i_nScreenWidth, int i_nScreenHeight )	
	{
		// デバイスのリセット
		if( m_pGraphicDevice->Reset( i_nScreenWidth, i_nScreenHeight) == false )
		{
			return false;
		}

		return true;
	}	

	//------------------------------------------------------------------------------------------
	bool CApp::MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
	{
		// ハンドルしない
		bool result = false;

		// メッセージ処理
		switch( i_nMsg )
		{
		case WM_KEYDOWN:
		case WM_KEYUP:
		case WM_CHAR:
		case WM_MOUSEMOVE:
		case WM_LBUTTONDOWN:
		case WM_LBUTTONUP:
		case WM_RBUTTONDOWN:
		case WM_RBUTTONUP:
		case WM_MOUSEWHEEL:
			// @@@より汎用的な処理に
			m_cameraController.OnMsgProc( i_hWnd, i_nMsg, i_wParam, i_lParam );
			break;
		}

		return result;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Update()
	{
		/// @@@@
		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Render()
	{
		if( FAILED(GetGraphicDevice()->Activate()) )
		{	
			return false;
		}

		GetGraphicDevice()->GetD3DDevice()->Clear( 0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, D3DCOLOR_ARGB(255, 255, 0, 255), 1.0f, 0 );

		
		IDirect3DDevice9* pd3dDevice = CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice();		

		//@@@ カメラ設定
		/*
		CGraphicDevice::SCameraInfo camera;
		camera.fNear = 1.0f;
		camera.fFar = 5000.0f;
		camera.fFov = 3.14f / 6.0f;
		camera.vEyePos.x = 0.0f; camera.vEyePos.y = 10.0f; camera.vEyePos.z = 20.0f;
		camera.vInterestPos.x = camera.vInterestPos.y = camera.vInterestPos.z = 0.0f; // 原点
		camera.vUpDir.x = 0.0f; camera.vUpDir.y = 1.0f, camera.vUpDir.z = 0.0f; // 上方向
		GetGraphicDevice()->SetCameraInfo( camera );
		*/
		GetGraphicDevice()->SetCameraInfo( m_cameraController.GetCameraInfo() );

		m_model.Render(); ///< モデル @@@削除

		GetGraphicDevice()->Deactivate();

		return true;
	}
} // end of namespace opk