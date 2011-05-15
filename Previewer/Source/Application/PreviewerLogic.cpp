/**
	@file App.cpp
	@brief �A�v���P�[�V�����N���X	
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
		
		// �N���A
		pDevice->Clear( 0.5f, 0.5f, 0.5f, 1.0f );

		// ���N���X�̕`����Ăяo��
		CLogicBase::Render();
	}

	//------------------------------------------------------------------------------------------
	void CPreviewerLogic::Initialize()
	{
		// Previewer�p�r���[���쐬���A�^�b�`
		IView* pView = new CPreviewerView();
		AddView( shared_ptr<IView>( pView ) );		
	}
};