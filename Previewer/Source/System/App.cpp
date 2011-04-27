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
		: m_pGraphicDevice(NULL)
		, m_model(L"C:\\projects\\metashader\\data\\model\\Sphere.x") ///< モデル @@@削除
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
		// アプリケーションのディレクトリのパスの初期化
		InitializeApplicationDirectoryPath();

		// デバイスの初期化
		m_pGraphicDevice = new CGraphicDevice();
		m_pGraphicDevice->Initialize(i_nScreenWidth, i_nScreenHeight);

		m_model.Restore(); ///< モデル @@@削除		

		return true;
	}

	//------------------------------------------------------------------------------------------
	void CApp::InitializeApplicationDirectoryPath()
	{
		char drive[MAX_PATH];
		char dir[MAX_PATH];
		char fname[MAX_PATH];
		char ext[MAX_PATH];

		const HMODULE handle = GetModuleHandle(NULL);
		GetModuleFileNameA( handle, m_appDirectoryPath, MAX_PATH);	
		_splitpath_s( m_appDirectoryPath, drive, dir, fname, ext);

		// アプリケーションのディレクトリ迄のパスを保持
		sprintf_s(m_appDirectoryPath, MAX_PATH, "%s%s", drive, dir);
	}

	//------------------------------------------------------------------------------------------
	bool CApp::ResetDevice(int i_nScreenWidth, int i_nScreenHeight )	
	{
		// モデルの破棄
		m_model.Destroy();

		// デバイスのリセット
		if( m_pGraphicDevice && m_pGraphicDevice->Reset( i_nScreenWidth, i_nScreenHeight) == false )
		{
			return false;
		}

		// モデルのリセット
		if( FAILED( m_model.Restore() ) )
		{
			return false;
		}

		// @@@ テクスチャリソースのリセット
		// @@@ シェーダのリセット

		return true;
	}	

	//------------------------------------------------------------------------------------------
	LRESULT CApp::MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
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
		
		// デフォルト処理
		return ::DefWindowProc( i_hWnd, i_nMsg, i_wParam, i_lParam);
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
		// レンダリング開始
		if( FAILED(m_pGraphicDevice->Activate()) )
		{	
			return false;
		}

		m_pGraphicDevice->Clear( 0.0f, 0.0f, 0.0f, 0.0f );				
		
		m_pGraphicDevice->SetCameraInfo( m_cameraController.GetCameraInfo() );

		m_model.Render(); ///< モデル @@@削除

		// レンダリング終了
		m_pGraphicDevice->Deactivate();

		return true;
	}
} // end of namespace opk