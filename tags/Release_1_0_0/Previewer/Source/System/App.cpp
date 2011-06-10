/**
	@file App.cpp
	@brief アプリケーションクラス	
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "System/Logic.h"

namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------
	CApp* CApp::s_pInstance = NULL;

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CApp::CApp()
		: m_pGraphicDevice(NULL)
		, m_pTime ( NULL )
		, m_pLogic( NULL )		
	{
	}

	//------------------------------------------------------------------------------------------
	CApp::~CApp()
	{
		SAFE_DELETE( m_pGraphicDevice );
		SAFE_DELETE( m_pTime );
		SAFE_DELETE( m_pLogic );
	}

	//------------------------------------------------------------------------------------------
	bool CApp::CreateInstance( CLogicBase *i_pLogic )
	{
		if( !s_pInstance )
		{
			s_pInstance = new CApp();
			s_pInstance->m_pLogic = i_pLogic;
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

		// グラフィックデバイスの初期化
		m_pGraphicDevice = new CGraphicDevice();
		m_pGraphicDevice->Initialize(i_nScreenWidth, i_nScreenHeight);

		// ロジックの初期化
		MY_ASSERT( m_pLogic );
		m_pLogic->Initialize();		

		// タイマーの初期化
		MY_ASSERT( m_pTime == NULL );
		m_pTime = new CTime();		
		m_pTime->Start();

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
		// デバイスのリセット
		if( m_pGraphicDevice && m_pGraphicDevice->Reset( i_nScreenWidth, i_nScreenHeight) == false )
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
		HRESULT ret = 0;

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
			// ロジックに処理を移譲		
			ret = m_pLogic->MsgProc( i_hWnd, i_nMsg, i_wParam, i_lParam );			
			break;
		default:
			// デフォルト処理
			ret = ::DefWindowProc( i_hWnd, i_nMsg, i_wParam, i_lParam);
		}		
		
		return ret;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Update()
	{
		// タイマーを更新
		m_pTime->Update();

		// ロジックに処理を移譲		
		m_pLogic->Update( (float)m_pTime->GetTotalTime(), (float)m_pTime->GetElapsedDeltaTime() );

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

		// ロジックに処理を移譲
		m_pLogic->Render();					

		// レンダリング終了
		m_pGraphicDevice->Deactivate();

		return true;
	}
} // end of namespace opk