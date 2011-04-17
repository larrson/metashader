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

//-------------------------------------------------------------------------------------------
void SetTexturePath( const char* i_pszName, const char* i_pszPath )
{
	opk::shader::CShaderMan* pShaderMan = opk::shader::CShaderMan::GetInstance();
	pShaderMan->SetTexturePath( opk::shader::Profile_Pixel, std::string(i_pszName), i_pszPath );
}

//-------------------------------------------------------------------------------------------
void SetSamplerState( const char* i_pszName, const opk::shader::SSamplerState i_samplerState )
{	
	opk::shader::CShaderMan* pShaderMan = opk::shader::CShaderMan::GetInstance();
	pShaderMan->SetSamplerState( opk::shader::Profile_Pixel, std::string(i_pszName), i_samplerState );
}

//-------------------------------------------------------------------------------------------
void GetImagePixelData( const char* i_pszPath, int i_nWidth, int i_nHeight, uint8* o_pBuffer )
{
	HRESULT hr;	

	IDirect3DDevice9 *pD3DDevice = opk::CApp::GetInstance()->GetGraphicDevice()->GetD3DDevice();
	IDirect3DTexture9 *pD3DTexture;
	hr = D3DXCreateTexture( pD3DDevice, i_nWidth, i_nHeight, 1, 0, D3DFMT_A8R8G8B8, D3DPOOL_SYSTEMMEM, &pD3DTexture ); MY_ASSERT( SUCCEEDED(hr) );

	IDirect3DSurface9* pD3DSurface;
	hr = pD3DTexture->GetSurfaceLevel(0, &pD3DSurface);

	RECT destRect = { 0, 0, i_nWidth, i_nHeight };
	hr = D3DXLoadSurfaceFromFileA( pD3DSurface, NULL, &destRect, i_pszPath, NULL, D3DX_DEFAULT, 0, NULL );	 MY_ASSERT( SUCCEEDED(hr) );				

	// サーフェースのロック
	D3DLOCKED_RECT lockedRect;
	hr = pD3DSurface->LockRect( &lockedRect, NULL, D3DLOCK_READONLY); MY_ASSERT( SUCCEEDED(hr) );		

	for( int h = 0; h < i_nHeight; ++h)
	{		
		for(int w = 0; w < i_nWidth; ++w)
		{
			// ARGB⇒BGRAの順
			int pixel = h * lockedRect.Pitch + w * 4;
			o_pBuffer[pixel + 0] = (reinterpret_cast<uint8*>(lockedRect.pBits))[pixel + 0];
			o_pBuffer[pixel + 1] = (reinterpret_cast<uint8*>(lockedRect.pBits))[pixel + 1];
			o_pBuffer[pixel + 2] = (reinterpret_cast<uint8*>(lockedRect.pBits))[pixel + 2];
			o_pBuffer[pixel + 3] = (reinterpret_cast<uint8*>(lockedRect.pBits))[pixel + 3];
		}		
	}

	// サーフェースのアンロック
	hr = pD3DSurface->UnlockRect(); MY_ASSERT( SUCCEEDED(hr) );		

	SAFE_RELEASE( pD3DSurface );
	SAFE_RELEASE( pD3DTexture );
}