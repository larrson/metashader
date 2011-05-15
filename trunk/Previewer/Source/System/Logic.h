#pragma once
/**
	@file Logic.h
	@brief アプリケーションのロジックを記述するクラス
*/

// Includes ----------------------------------------------------------------------------------
#include "View.h"

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{		
	/**
		@class CLogicBase 
		@brief ロジックを記述する基底クラス
	*/
	class CLogicBase
	{
	private:		
		ViewList m_viewList; ///< ビューのリスト

	public:
		/// コンストラクタ
		CLogicBase();

		/// デストラクタ
		virtual ~CLogicBase();

		/// Viewを追加
		virtual void AddView( shared_ptr<IView> i_pView );

		/// Viewを削除
		virtual void RemoveView( shared_ptr<IView> i_pView );

		/**
			@brief Win32のメッセージハンドラ
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );
		/// 更新
		virtual void Update(float i_fTime, float i_fElapsedTime);
		/// レンダリング
		virtual void Render();
		/// 初期化
		virtual void Initialize() = 0;		
	};
} // end of namespace opk