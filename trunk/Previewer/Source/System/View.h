#pragma once
/**
	@file View.h
	@brief 画面へ表示されるオブジェクトを表すクラス	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class IView
		@brief 画面へ表示されるオブジェクトを表すクラスの基底クラス
	*/
	class IView
	{
	public:
		/// デストラクタ
		virtual ~IView(){};

		/**
			@brief Win32のメッセージハンドラ
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam ) = 0;
		/// 更新
		virtual void Update( float i_fElapsedTime ) = 0;
		/// レンダリング
		virtual void Render() = 0;
		/// Viewが追加された際に呼ばれるメソッド
		virtual void Attach() = 0;
		/// 初期化時およびデバイスロスト時のリストア処理
		virtual void Restore() = 0;
	};

	typedef std::list<shared_ptr<IView> > ViewList;
} // end of namespace opk