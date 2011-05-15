/**
	@file Logic.cpp
	@brief アプリケーションのロジックを記述するクラス
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "Logic.h"

namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------	

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CLogicBase::CLogicBase()
	{

	}

	//------------------------------------------------------------------------------------------
	CLogicBase::~CLogicBase()
	{
		// Viewの削除
		while( !m_viewList.empty() )
		{
			m_viewList.pop_front();
		}
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::AddView( shared_ptr<IView> i_pView )
	{	
		// リストに追加
		m_viewList.push_back( i_pView );

		// 追加時処理
		i_pView->Attach();
		// 初期化
		i_pView->Restore();
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::RemoveView( shared_ptr<IView> i_pView )
	{
		// リストから削除
		m_viewList.remove( i_pView );
	}

	//------------------------------------------------------------------------------------------
	LRESULT CLogicBase::MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
	{
		// ビューへ処理を移譲
		// 重なっている上側から処理する
		for( ViewList::reverse_iterator itr = m_viewList.rbegin(); itr != m_viewList.rend(); ++itr )
		{
			// ハンドルされたら、そこで処理をうちきる
			if( (*itr)->MsgProc( i_hWnd, i_nMsg, i_wParam, i_lParam ) )
			{
				return true;
			}
		}
		return false;
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::Update(float i_fTime, float i_fElapsedTime)
	{
		// ビューへ処理を移譲
		for( ViewList::iterator itr = m_viewList.begin(); itr != m_viewList.end(); ++itr )
		{
			(*itr)->Update( i_fElapsedTime );
		}
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::Render()
	{
		// ビューへ処理を移譲
		// ビューの下側から順に描画
		for( ViewList::iterator itr = m_viewList.begin(); itr != m_viewList.end(); ++itr )
		{
			(*itr)->Render();
		}
	}
} // end of namespace opk