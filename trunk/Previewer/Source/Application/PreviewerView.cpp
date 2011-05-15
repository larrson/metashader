/**
	@file PreviewerView.cpp
	@brief プレビューア用Viewクラス
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "CameraController.h"
#include "Graphics/Model.h"
#include "PreviewerView.h"

namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------	
	namespace
	{
		const char* c_pszModelFilePath = "..\\..\\data\\model\\Sphere.x";
	}

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CPreviewerView::CPreviewerView()		
	{
		m_pModel = new CModel();
		m_pMouseHandler = new CCameraController();
	}

	//------------------------------------------------------------------------------------------
	CPreviewerView::~CPreviewerView()
	{
		SAFE_DELETE( m_pModel );
		SAFE_DELETE( m_pMouseHandler );
	}

	//------------------------------------------------------------------------------------------
	LRESULT CPreviewerView::MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
	{
		// マウスハンドラへ処理を移譲
		return m_pMouseHandler->OnMsgProc( i_hWnd, i_nMsg, i_wParam, i_lParam );
	}

	//------------------------------------------------------------------------------------------
	void CPreviewerView::Update( float i_fElapsedTime )
	{

	}

	//------------------------------------------------------------------------------------------
	void CPreviewerView::Render()
	{
		CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );

		// カメラ設定
		CCameraController *pCameraController = dynamic_cast<CCameraController*>( m_pMouseHandler );
		pDevice->SetCameraInfo( pCameraController->GetCameraInfo() );

		// ライト設定				
		// 0 Fill
		D3DXVECTOR3 vLightDir0(-1, -1, -1); D3DXVec3Normalize( &vLightDir0, &vLightDir0);		
		pDevice->SetDirLightDir( 0, vLightDir0.x, vLightDir0.y, vLightDir0.z );
		pDevice->SetDirLightColor( 0, 1.0f, 1.0f, 1.0f );		
		// 1 Back
		D3DXVECTOR3 vLightDir1( 0, 0, -1); D3DXVec3Normalize( &vLightDir1, &vLightDir1);		
		pDevice->SetDirLightDir( 1, vLightDir1.x, vLightDir1.y, vLightDir1.z );
		pDevice->SetDirLightColor( 1, 0.2f, 0.2f, 1.0f );		
		// 2 Key
		D3DXVECTOR3 vLightDir2( -1, -1, 0); D3DXVec3Normalize( &vLightDir2, &vLightDir2);		
		pDevice->SetDirLightDir( 2, vLightDir2.x, vLightDir2.y, vLightDir2.z );
		pDevice->SetDirLightColor( 2, 1.0f, 1.0f, 1.0f );		

		// モデルをレンダリング		
		m_pModel->Render();
	}

	//------------------------------------------------------------------------------------------
	void CPreviewerView::Attach()
	{				
	}

	//------------------------------------------------------------------------------------------
	void CPreviewerView::Restore()
	{
		// モデル初期化
		std::string strModelFilePath = std::string(CApp::GetInstance()->GetApplicationDirectory());
		strModelFilePath.append(c_pszModelFilePath);
		m_pModel->LoadFromFile( strModelFilePath );
	}

} // end of namespace opk
