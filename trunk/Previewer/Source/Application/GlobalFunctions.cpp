/**
	@file GlobalFunctions.h
	@brief Dll������J����֐��̒�`���L�q
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "GlobalFunctions.h"
#include "System/App.h"

// Function Definitions ----------------------------------------------------------------------

//-------------------------------------------------------------------------------------------
int PreviewerMain(int i_nScreenWidth, int i_nScreenHeight)
{
	// @@@@	

	// �A�v���P�[�V�������쐬
	if( opk::CApp::CreateInstance() == false )
		return false;	

	// �A�v���P�[�V������������		
	return opk::CApp::GetInstance()->Initialize(	
			NULL
			, i_nScreenWidth
			, i_nScreenHeight
		);

	return true;
}



//-------------------------------------------------------------------------------------------
int ShutDown()
{
	// @@@@

	// �A�v���P�[�V������j��
	opk::CApp::DisposeInstance();

	return TRUE;
}

//-------------------------------------------------------------------------------------------
bool WndProc(int *i_hWnd, int i_nMsg, int* i_wParam, int* i_lParam)
{
	// �A�v���P�[�V�����ɏ������ڏ�
	return opk::CApp::GetInstance()->MsgProc( (HWND)i_hWnd, i_nMsg, (WPARAM)i_wParam, (LPARAM)i_lParam );
}

//-------------------------------------------------------------------------------------------
void* GetNextSurface()
{
	void* ret = NULL;

	// �X�V
	if( opk::CApp::GetInstance()->Update() == false )
	{
		return false;
	}

	// �����_�����O
	if( opk::CApp::GetInstance()->Render() )
	{
		ret = opk::CApp::GetInstance()->GetGraphicDevice()->GetBackBuffer();
	}	
	return ret;
}

//-------------------------------------------------------------------------------------------
void Resize(int i_nScreenWidth, int i_nScreenHeight )
{
	opk::CApp* pInstance = opk::CApp::GetInstance();
	if( pInstance )
	{
		opk::CGraphicDevice *pGraphicDevice = pInstance->GetGraphicDevice();
		if( pGraphicDevice )
		{
			pGraphicDevice->Resize( i_nScreenWidth, i_nScreenHeight);
		}
	}	
}