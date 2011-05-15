#pragma once
/**
	@file PreviewerView.h
	@brief プレビューア用Viewクラス
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{	
	class CModel;
	class MouseHandlerBase;

	/**
		@class CPreviewerView
		@brief プレビューア用Viewクラス
	*/
	class CPreviewerView : public IView
	{
	private:
		CModel*				m_pModel;
		MouseHandlerBase*	m_pMouseHandler;

	public:
		/// コンストラクタ
		CPreviewerView();

		/// デストラクタ
		virtual ~CPreviewerView();

		/**
			@brief Win32のメッセージハンドラ
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );
		/// 更新
		virtual void Update( float i_fElapsedTime );
		/// レンダリング
		virtual void Render();
		/// Viewが追加された際に呼ばれるメソッド
		virtual void Attach();
		/// 初期化時およびデバイスロスト時のリストア処理
		virtual void Restore();
	};
}