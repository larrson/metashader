#pragma once
/**
	@file GraphicDevice.h
	@brief グラフィックデバイスクラス	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CGraphicDevice
		@brief グラフィックデバイスを管理するクラス
	*/
	class CGraphicDevice
	{
	private:
		IDirect3D9*			m_pd3d9;		///< DirectXオブジェクト	
		IDirect3DDevice9*	m_pd3dDevice9;  ///< グラフィックデバイス
		IDirect3DSurface9*	m_pd3dSurface9;	///< サーフェース

		HWND m_hWnd;		///< ダミーのウィンドウハンドル

		int m_nWidth;  ///< バックバッファの幅
		int m_nHeight; ///< バックバッファの高さ		

	public:
		/// コンストラクタ
		CGraphicDevice();

		/// デストラクタ
		~CGraphicDevice();

		/// グラフィックデバイスの取得
		IDirect3DDevice9* GetD3DDevice(){ return m_pd3dDevice9; }

		/// スクリーン幅の取得
		int GetWidth(){ return m_nWidth; }

		/// スクリーン高さの取得
		int GetHeight(){ return m_nHeight; }

		/// 初期化
		bool Initialize(int i_nWidth, int i_nHeight);

		/// 破棄.
		void Dispose();

		/// デバイスのリセット
		bool Reset(int i_nWidht, int i_nHeight);	

		/// 3Dシーンのレンダリング開始
		bool Activate();

		/// 3Dシーンのレンダリング終了
		void Deactivate();

		/// バックバッファを取得
		IDirect3DSurface9* GetBackBuffer();

		/// 画面のリサイズ
		void Resize(int i_nScreenWidth, int i_nScreenHeight );

	private:
		/// ダミーのウィンドウを作成する
		bool CreateDummyWindow();

		/// グラフィックデバイスを作成する
		bool CreateDevice( int i_nWidth, int i_nHeight );

		/// レンダーターゲットを作成する
		bool CreateRenderTarget( int i_nWidth, int i_nHeight );
	};
} // end of namespace opk