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
	class CLogicBase;
	class CTime;

	/**
		@class CApp
		@brief アプリケーションクラス
	*/
	class CApp
	{		
	private:
		static CApp*	s_pInstance; ///< シングルトンの唯一のインスタンス	

		char m_appDirectoryPath[MAX_PATH];	///< アプリケーションのディレクトリのパス

		CGraphicDevice*	m_pGraphicDevice;	///< グラフィックデバイス		

		CTime*				m_pTime;				///< タイマー
		
		CLogicBase*			m_pLogic;				///< アプリケーションロジック				
	public:		
		/// インスタンスの作成
		static bool CreateInstance( CLogicBase* i_pLogic );

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
		LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );

		/// 更新
		bool Update();

		/// レンダリング
		bool Render();

		/**
			@brief アプリケーションの実行ファイルの置かれているディレクトリのパスを取得する			
			@note 取得される文字列の最後には、パス区切り文字「\」が付加されている
		*/
		const char*	GetApplicationDirectory(){ return m_appDirectoryPath; };

	private:
		/// コンストラクタ
		CApp();

		/// デストラクタ
		~CApp();

		/// コピーコンストラクタの禁止
		CApp(const CApp& app);

		/// 代入演算の禁止
		CApp& operator=(const CApp& app);

		/**
			@brief アプリケーションディレクトリの初期化
		*/
		void InitializeApplicationDirectoryPath();		
	};

	/**
		@class ILogic
		@brief アプリケーションのロジックを記述する基底クラス
	*/
	class ILogic
	{
	public:
		/// デストラクタ
		virtual ~ILogic(){};

		/**
			@brief Win32のメッセージハンドラ
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam ) = 0;
		/// 更新
		virtual void Update(float i_fTime, float i_fElapsedTime) = 0;
		/// レンダリング
		virtual void Render() = 0;
		/// 初期化
		virtual void Initialize() = 0;		
	};
} // end of namespace opk