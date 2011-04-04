#pragma once
/**
	@file Mouse.h
	@brief マウスのハンドリングを行うクラスの宣言
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class MouseHandlerBase
		@brief マウスをハンドリングするための基底クラス
	*/
	class MouseHandlerBase
	{
	public:
		/// コンストラクタ
		MouseHandlerBase(){}

		/// デストラクタ
		virtual ~MouseHandlerBase(){}

	protected:
		/// マウスカーソルが動いた
		virtual bool OnMouseMove( int x, int y ) = 0;
		/// 左ボタンが押された
		virtual bool OnLButtonDown( int x, int y  ) = 0;
		/// 左ボタンが離された
		virtual bool OnLButtonUp( int x, int y ) = 0;
		/// 右ボタンが押された
		virtual bool OnRButtonDown( int x, int y ) = 0;
		/// 右ボタンが離された
		virtual bool OnRButtonUp( int x, int y ) = 0;
		/// ホイールが回された
		virtual bool OnMouseWheel( int i_nDelta ) = 0;
	public:
		/// メッセージ処理
		bool OnMsgProc( HWND i_hWnd, UINT i_nMsg, WPARAM i_wParam, LPARAM i_lParam );	
	};
} // end of namespace opk