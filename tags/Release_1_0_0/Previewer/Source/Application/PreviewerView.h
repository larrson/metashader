#pragma once
/**
	@file PreviewerView.h
	@brief �v���r���[�A�pView�N���X
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{	
	class CModel;
	class MouseHandlerBase;

	/**
		@class CPreviewerView
		@brief �v���r���[�A�pView�N���X
	*/
	class CPreviewerView : public IView
	{
	private:
		CModel*				m_pModel;
		MouseHandlerBase*	m_pMouseHandler;

	public:
		/// �R���X�g���N�^
		CPreviewerView();

		/// �f�X�g���N�^
		virtual ~CPreviewerView();

		/**
			@brief Win32�̃��b�Z�[�W�n���h��
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );
		/// �X�V
		virtual void Update( float i_fElapsedTime );
		/// �����_�����O
		virtual void Render();
		/// View���ǉ����ꂽ�ۂɌĂ΂�郁�\�b�h
		virtual void Attach();
		/// ������������уf�o�C�X���X�g���̃��X�g�A����
		virtual void Restore();
	};
}