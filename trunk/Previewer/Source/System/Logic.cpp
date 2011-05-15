/**
	@file Logic.cpp
	@brief �A�v���P�[�V�����̃��W�b�N���L�q����N���X
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
		// View�̍폜
		while( !m_viewList.empty() )
		{
			m_viewList.pop_front();
		}
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::AddView( shared_ptr<IView> i_pView )
	{	
		// ���X�g�ɒǉ�
		m_viewList.push_back( i_pView );

		// �ǉ�������
		i_pView->Attach();
		// ������
		i_pView->Restore();
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::RemoveView( shared_ptr<IView> i_pView )
	{
		// ���X�g����폜
		m_viewList.remove( i_pView );
	}

	//------------------------------------------------------------------------------------------
	LRESULT CLogicBase::MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
	{
		// �r���[�֏������ڏ�
		// �d�Ȃ��Ă���㑤���珈������
		for( ViewList::reverse_iterator itr = m_viewList.rbegin(); itr != m_viewList.rend(); ++itr )
		{
			// �n���h�����ꂽ��A�����ŏ�������������
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
		// �r���[�֏������ڏ�
		for( ViewList::iterator itr = m_viewList.begin(); itr != m_viewList.end(); ++itr )
		{
			(*itr)->Update( i_fElapsedTime );
		}
	}

	//------------------------------------------------------------------------------------------
	void CLogicBase::Render()
	{
		// �r���[�֏������ڏ�
		// �r���[�̉������珇�ɕ`��
		for( ViewList::iterator itr = m_viewList.begin(); itr != m_viewList.end(); ++itr )
		{
			(*itr)->Render();
		}
	}
} // end of namespace opk