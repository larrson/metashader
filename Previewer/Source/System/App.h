#pragma once
/**
	@file App.h
	@brief アプリケーションクラス
	@note シングルトン
	@attension インスタンスの作成と破棄は手動で管理する
*/

// Includes ----------------------------------------------------------------------------------
#include "Application/CameraController.h" //@@@削除予定

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CApp
		@brief アプリケーションクラス
	*/
	class CApp
	{
	private:
		static CApp*	s_pInstance; ///< シングルトンの唯一のインスタンス				

		CGraphicDevice*	m_pGraphicDevice; ///< グラフィックデバイス

		CModel			m_model; ///< モデル @@@削除
		CCameraController m_cameraController; ///< カメラ
		

	public:
		IDirect3DTexture9* m_pd3dTexture;

		/// インスタンスの作成
		static bool CreateInstance();

		/// インスタンスの破棄
		static void DisposeInstance();

		/// インスタンスの取得
		static CApp* GetInstance(){ return s_pInstance; }

		/// グラフィックデバイスの取得
		CGraphicDevice* GetGraphicDevice(){ return m_pGraphicDevice; }

		/** 
			@brief 初期化			
			@param [in] i_lpCmdLine コマンドラインパラメータ			
			@param [in] i_nScreenWidth	スクリーンの幅
			@param [in] i_nScreenHeight	スクリーンの高さ
			@retval 初期化に成功したか			
		*/
		bool Initialize(LPWSTR i_lpCmdLine, int i_nScreenWidth, int i_nScreenHeight);
		
		/**
			@brief グラフィックデバイスのリセット
			@param [in] i_nScreenWidth	スクリーンの幅
			@param [in] i_nScreenHeight	スクリーンの高さ
			@retval リセットに成功したか
		*/
		bool ResetDevice(int i_nScreenWidth, int i_nScreenHeight );		

		/**
			@brief Win32のメッセージハンドラ
		*/
		bool MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );

		/// 更新
		bool Update();

		/// レンダリング
		bool Render();

	private:
		/// コンストラクタ
		CApp();

		/// デストラクタ
		~CApp();

		/// コピーコンストラクタの禁止
		CApp(const CApp& app);

		/// 代入演算の禁止
		CApp& operator=(const CApp& app);
	};
} // end of namespace opk