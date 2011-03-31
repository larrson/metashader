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
	bool CApp::Update()
	{
		/// @@@@
		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Render()
	{
		if( !GetGraphicDevice()->Activate() )
		{	
			return false;
		}

		GetGraphicDevice()->GetD3DDevice()->Clear( 0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, D3DCOLOR_ARGB(255, 255, 0, 255), 0.0f, 0 );

		GetGraphicDevice()->Deactivate();

		return true;
	}
} // end of namespace opk