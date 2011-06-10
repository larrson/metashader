/**
	@file App.cpp
	@brief アプリケーションクラス	
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "System/Logic.h"
#include "Application/PreviewerLogic.h"
#include "Application/PreviewerView.h"

namespace opk
{
	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CPreviewerLogic::CPreviewerLogic()
	{

	}

	//------------------------------------------------------------------------------------------
	CPreviewerLogic::~CPreviewerLogic()
	{

	}		

	//------------------------------------------------------------------------------------------
	void CPreviewerLogic::Render()
	{		
		CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice(); MY_ASSERT( pDevice );
		
		// クリア
		pDevice->Clear( 0.5f, 0.5f, 0.5f, 1.0f );

		// 基底クラスの描画を呼び出し
		CLogicBase::Render();
	}

	//------------------------------------------------------------------------------------------
	void CPreviewerLogic::Initialize()
	{
		// Previewer用ビューを作成しアタッチ
		IView* pView = new CPreviewerView();
		AddView( shared_ptr<IView>( pView ) );		
	}
};