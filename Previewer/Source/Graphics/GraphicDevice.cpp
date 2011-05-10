/**
	@file GraphicDevice.cpp
	@brief グラフィックデバイスクラス
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "GraphicDevice.h"

// Global Variable Definitions ---------------------------------------------------------------	
namespace 
{	
	/// メッセージ処理関数
	LRESULT WINAPI MsgProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
	{
		return DefWindowProc(hWnd, msg, wParam, lParam);
	}

	// ウィンドウ作成
	WNDCLASSEX wndclass =
	{
		sizeof(WNDCLASSEX),
		CS_CLASSDC,
		MsgProc,
		0L,
		0L,
		GetModuleHandle(NULL),
		NULL,
		NULL,
		NULL,
		NULL,
		L"Previewer",
		NULL
	};
}

namespace opk
{


	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CGraphicDevice::CGraphicDevice()
		: m_pd3d9		(NULL)
		, m_pd3dDevice9	(NULL)	
		, m_pd3dSurface9(NULL)
		, m_hWnd		(0)
		, m_bValid	(false)
		, m_bActive (false)		
		, m_nWidth (0)
		, m_nHeight(0)
		, m_viewport()
		, m_cameraInfo()
	{

	}

	//------------------------------------------------------------------------------------------
	CGraphicDevice::~CGraphicDevice()
	{
		Dispose();
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::Initialize(int i_nWidth, int i_nHeight)
	{
		HRESULT hr = S_OK;

		// D3Dオブジェクトの作成
		m_pd3d9 = Direct3DCreate9(D3D_SDK_VERSION);
		if( !m_pd3d9 )
		{
			return false;
		}

		// ダミーウィンドウを作成
		if( CreateDummyWindow() == false )
		{
			return false;
		}

		// デバイスを作成
		if( CreateDevice(i_nWidth, i_nHeight) == false )
		{
			return false;
		}

		// レンダーターゲットの作成
		if( CreateRenderTarget(i_nWidth, i_nHeight) == false )
		{
			return false;
		}

		/// 全てに成功 ///

		m_nWidth = i_nWidth;
		m_nHeight = i_nHeight;

		// ビューポートの設定
		m_viewport.X = 0;
		m_viewport.Y = 0;
		m_viewport.Width = i_nWidth;
		m_viewport.Height = i_nHeight;		
		m_viewport.MinZ = 0.0f;
		m_viewport.MaxZ = 1.0f;

		// トランスフォームの初期化(単位行列化)
		for(int i = 0; i < TransformType_Max; ++i)
		{
			D3DXMatrixIdentity( &m_mTransform[i] );
		}

		// シェーダ管理の初期化
		shader::CShaderMan::CreateInstance();
		shader::CShaderMan::GetInstance()->Initialize();

		m_bValid = true;

		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::CreateDummyWindow()
	{				
		if (!RegisterClassEx(&wndclass))
		{
			return false;
		}

		m_hWnd = CreateWindow(			
			L"Previewer",
			L"Previewer",
			WS_OVERLAPPEDWINDOW,
			0,                   // Initial X
			0,                   // Initial Y
			0,                   // Width
			0,                   // Height
			NULL,
			NULL,
			wndclass.hInstance,
			NULL);			

		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::CreateDevice( int i_nWidth, int i_nHeight )
	{
		// Presentation Parameter の初期化		
		ZeroMemory( &m_d3dpp, sizeof(m_d3dpp) );
		m_d3dpp.Windowed = TRUE;
		// サイズとフォーマット		
		m_d3dpp.BackBufferWidth = i_nWidth;
		m_d3dpp.BackBufferHeight = i_nHeight;
		m_d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
		m_d3dpp.BackBufferFormat = D3DFMT_UNKNOWN;
		m_d3dpp.EnableAutoDepthStencil = TRUE;
		m_d3dpp.AutoDepthStencilFormat = D3DFMT_D24X8;		

		// デバイス作成
		HRESULT hr = m_pd3d9->CreateDevice(
				D3DADAPTER_DEFAULT
				, D3DDEVTYPE_HAL
				, m_hWnd
				, D3DCREATE_SOFTWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE
				, &m_d3dpp				
				, &m_pd3dDevice9
			);		
		if( FAILED(hr) )
		{
			return false;
		}

		 return true;
	}

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::CreateRenderTarget(int i_nWidth, int i_nHeight)
	{
		HRESULT hr;		
		hr = m_pd3dDevice9->CreateRenderTarget(
				i_nWidth
				,	i_nHeight
				,	D3DFMT_A8R8G8B8
				,	D3DMULTISAMPLE_NONE
				,	0
				,	true
				,	&(m_pd3dSurface9)
				,	NULL
				);
		if( FAILED(hr) )
		{
			return false;
		}		

		if( FAILED( m_pd3dDevice9->SetRenderTarget(0, m_pd3dSurface9) ) )
		{
			return false;
		}

		return true;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Dispose()
	{
		// シェーダ管理の破棄		
		shader::CShaderMan::DisposeInstance();

		// 破棄処理				
		SAFE_RELEASE( m_pd3dSurface9 );	
		SAFE_RELEASE( m_pd3dDevice9 );
		SAFE_RELEASE( m_pd3d9 );
		
		if( m_hWnd )
		{
			DestroyWindow(m_hWnd);
			UnregisterClass(L"Previewer", NULL);
		}
	}	

	//------------------------------------------------------------------------------------------
	bool CGraphicDevice::Reset(int i_nWidth, int i_nHeight )
	{
		HRESULT hr;

		// サーフェースの破棄		
		SAFE_RELEASE( m_pd3dSurface9 );		

		// デバイスのリセット
		// Presentation Parameter の初期化		
		m_d3dpp.BackBufferWidth = i_nWidth;
		m_d3dpp.BackBufferHeight = i_nHeight;	
		hr = m_pd3dDevice9->Reset( &m_d3dpp );
		MY_ASSERT( SUCCEEDED(hr) );
		
		// サーフェースを新しいサイズで再作成
		bool ret = CreateRenderTarget( i_nWidth, i_nHeight );
		MY_ASSERT( ret );

		m_nWidth = i_nWidth;
		m_nHeight = i_nHeight;

		// ビューポートの設定
		m_viewport.X = 0;
		m_viewport.Y = 0;
		m_viewport.Width = i_nWidth;
		m_viewport.Height = i_nHeight;		
		m_viewport.MinZ = 0.0f;
		m_viewport.MaxZ = 1.0f;

		return ret;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::Activate()
	{
		HRESULT hr;

		// 準備中
		if( m_bValid == false )
		{
			return E_FAIL;
		}	

		V_RETURN( m_pd3dDevice9->BeginScene() );		

		// 実行中フラグをセット
		m_bActive = true;
		
		// ビューポートを設定
		V_RETURN( m_pd3dDevice9->SetViewport(&m_viewport) );

		// レンダーステートの設定
		V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ZENABLE, TRUE ) );
		V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ZWRITEENABLE, TRUE ) );
		V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ZFUNC, D3DCMP_LESSEQUAL ) );

		// ブレンドモードの設定
		V_RETURN( SetBlendMode( m_nBlendMode, true ) );
		
		return hr;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::Deactivate()
	{		
		// レンダリング終了
		m_pd3dDevice9->EndScene();	

		// 実行中フラグを下ろす
		m_bActive = false;		
	}
	
	//------------------------------------------------------------------------------------------
	IDirect3DSurface9* CGraphicDevice::GetBackBuffer()
	{
		// バックバッファの取得				
		return m_pd3dSurface9;
	}
	
	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::SetTransform(TransformType i_nTransformType, D3DXMATRIX i_mMatrix)
	{		
#if !USE_SHADER
		HRESULT hr;
#endif // USE_SHADER

		// IDirect3DDevice9::SetTransform用変換テーブル
		static const D3DTRANSFORMSTATETYPE tbStateType[TransformType_Max] =
		{
			D3DTS_WORLD,
			D3DTS_VIEW,
			D3DTS_PROJECTION,
		};

		// 新しい行列を保持
		m_mTransform[ i_nTransformType ] = i_mMatrix;

		// 実行時ならセット
		if( m_bActive )
		{
#if !USE_SHADER				
			V_RETURN( m_pd3dDevice9->SetTransform( tbStateType[i_nTransformType], &(m_mTransform[i_nTransformType]) ) );
#endif // USE_SHADER
		}

		return S_OK;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::SetCameraInfo( const SCameraInfo& i_cameraInfo )
	{
		HRESULT hr;

		// 新しいカメラ情報を保持
		m_cameraInfo = i_cameraInfo;

		/// 新しいカメラ情報でビュー，射影行列を計算 /// 
		D3DXMATRIX mView, mProj;

		// ビュー行列を計算		
		D3DXMatrixLookAtRH( &mView
			, &m_cameraInfo.vEyePos
			, &m_cameraInfo.vInterestPos
			, &m_cameraInfo.vUpDir );

		// 射影行列を計算
		D3DXMatrixPerspectiveFovRH( &mProj
			, m_cameraInfo.fFov
			, ((float)m_nWidth) / m_nHeight
			, m_cameraInfo.fNear
			, m_cameraInfo.fFar
			);

		// 各行列を設定
		V_RETURN( SetTransform(TransformType_View, mView) );
		V_RETURN( SetTransform(TransformType_Projection, mProj) );

		return S_OK;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::SetBlendMode( BlendMode i_nBlendMode, bool i_bForced )
	{
		HRESULT hr = S_OK;

		// 同じであればなにもしない
		if( !i_bForced && m_nBlendMode == i_nBlendMode )
			return hr;
		
		// 新しい値を保持
		m_nBlendMode = i_nBlendMode;

		// 実行時ならセット
		if( m_bActive )
		{
			switch( i_nBlendMode )
			{
			case BlendMode_None:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, FALSE) );
				break;
			case BlendMode_Normal:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE));
				// RGB
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOP, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_INVSRCALPHA ));
				// A
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOPALPHA, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLENDALPHA, D3DBLEND_ONE));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLENDALPHA, D3DBLEND_ONE ));
				break;
			case BlendMode_Add:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE));
				// RGB
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOP, D3DBLENDOP_ADD) );
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE ));
				// A
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOPALPHA, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLENDALPHA, D3DBLEND_ONE));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLENDALPHA, D3DBLEND_ONE ));
				break;
			case BlendMode_Sub:
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_ALPHABLENDENABLE, TRUE));
				// RGB
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOP, D3DBLENDOP_REVSUBTRACT ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLEND, D3DBLEND_SRCALPHA));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLEND, D3DBLEND_ONE ));
				// A
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_BLENDOPALPHA, D3DBLENDOP_ADD ));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_SRCBLENDALPHA, D3DBLEND_ONE));
				V_RETURN( m_pd3dDevice9->SetRenderState(D3DRS_DESTBLENDALPHA, D3DBLEND_ONE ))
				break;
			}			
		}

		return hr;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::SetDirLightEnable( int i_nIndex, bool i_bEnable )
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		m_dirLightInfo[ i_nIndex ].bEnable = i_bEnable;
	}

	//------------------------------------------------------------------------------------------
	D3DXVECTOR3 CGraphicDevice::GetDirLightColor( int i_nIndex )
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		return m_dirLightInfo[ i_nIndex ].vColor;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::SetDirLightColor( int i_nIndex, float i_fR, float i_fG, float i_fB)
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		m_dirLightInfo[ i_nIndex ].vColor.x = i_fR;
		m_dirLightInfo[ i_nIndex ].vColor.y = i_fG;
		m_dirLightInfo[ i_nIndex ].vColor.z = i_fB;
	}

	//------------------------------------------------------------------------------------------
	D3DXVECTOR3 CGraphicDevice::GetDirLightDir( int i_nIndex )
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		return m_dirLightInfo[ i_nIndex ].vDir;
	}

	//------------------------------------------------------------------------------------------
	void CGraphicDevice::SetDirLightDir( int i_nIndex, float i_fX, float i_fY, float i_fZ)
	{
		MY_ASSERT( i_nIndex < DirLight_Max );
		m_dirLightInfo[ i_nIndex ].vDir.x = i_fX;
		m_dirLightInfo[ i_nIndex ].vDir.y = i_fY;
		m_dirLightInfo[ i_nIndex ].vDir.z = i_fZ;
	}

	//------------------------------------------------------------------------------------------
	HRESULT CGraphicDevice::Clear(float i_fR, float i_fG, float i_fB, float i_fA)
	{
		HRESULT hr;
		V_RETURN( m_pd3dDevice9->Clear( 0, NULL, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, D3DCOLOR_ARGB((int)(i_fA * 255), (int)(i_fR * 255), (int)(i_fG * 255), (int)(i_fB * 255)), 1.0f, 0 ) );
		return hr;
	}

} // end of namespace opk