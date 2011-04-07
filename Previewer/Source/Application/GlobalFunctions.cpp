/**
	@file GlobalFunctions.h
	@brief Dllから公開する関数の定義を記述
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

	// アプリケーションを作成
	if( opk::CApp::CreateInstance() == false )
		return false;	

	// アプリケーションを初期化		
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

	// アプリケーションを破棄
	opk::CApp::DisposeInstance();

	return TRUE;
}

//-------------------------------------------------------------------------------------------
LRESULT WndProc(int *i_hWnd, int i_nMsg, int* i_wParam, int* i_lParam)
{
	// アプリケーションに処理を移譲
	return opk::CApp::GetInstance()->MsgProc( (HWND)i_hWnd, i_nMsg, (WPARAM)i_wParam, (LPARAM)i_lParam );
}

//-------------------------------------------------------------------------------------------
void* GetNextSurface()
{
	void* ret = NULL;

	// 更新
	if( opk::CApp::GetInstance()->Update() == false )
	{
		return false;
	}

	// レンダリング
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
		pInstance->ResetDevice(i_nScreenWidth, i_nScreenHeight);
	}	
}

//-------------------------------------------------------------------------------------------
void CreatePixelShaderFromBuffer( const char* i_pBuffer, uint32 i_nSize )
{
	opk::shader::CShaderMan* pShaderMan = opk::shader::CShaderMan::GetInstance();	
	pShaderMan->CreateShaderFromBuffer( opk::shader::Profile_Pixel, i_pBuffer, i_nSize );
}

//-------------------------------------------------------------------------------------------
void SetUniformVector4( const char* i_pszName, float x, float y, float z, float w )
{
	opk::shader::CShaderMan* pShaderMan = opk::shader::CShaderMan::GetInstance();	
	pShaderMan->SetVector4Value(opk::shader::Profile_Pixel, std::string(i_pszName), D3DXVECTOR4( x, y, z, w ));		
}