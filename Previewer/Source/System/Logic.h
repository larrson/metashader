#pragma once
/**
	@file Logic.h
	@brief �A�v���P�[�V�����̃��W�b�N���L�q����N���X
*/

// Includes ----------------------------------------------------------------------------------
#include "View.h"

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{		
	/**
		@class CLogicBase 
		@brief ���W�b�N���L�q������N���X
	*/
	class CLogicBase
	{
	private:		
		ViewList m_viewList; ///< �r���[�̃��X�g

	public:
		/// �R���X�g���N�^
		CLogicBase();

		/// �f�X�g���N�^
		virtual ~CLogicBase();

		/// View��ǉ�
		virtual void AddView( shared_ptr<IView> i_pView );

		/// View���폜
		virtual void RemoveView( shared_ptr<IView> i_pView );

		/**
			@brief Win32�̃��b�Z�[�W�n���h��
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );
		/// �X�V
		virtual void Update(float i_fTime, float i_fElapsedTime);
		/// �����_�����O
		virtual void Render();
		/// ������
		virtual void Initialize() = 0;		
	};
} // end of namespace opk