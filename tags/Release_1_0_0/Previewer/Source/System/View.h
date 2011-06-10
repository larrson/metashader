#pragma once
/**
	@file View.h
	@brief ��ʂ֕\�������I�u�W�F�N�g��\���N���X	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class IView
		@brief ��ʂ֕\�������I�u�W�F�N�g��\���N���X�̊��N���X
	*/
	class IView
	{
	public:
		/// �f�X�g���N�^
		virtual ~IView(){};

		/**
			@brief Win32�̃��b�Z�[�W�n���h��
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam ) = 0;
		/// �X�V
		virtual void Update( float i_fElapsedTime ) = 0;
		/// �����_�����O
		virtual void Render() = 0;
		/// View���ǉ����ꂽ�ۂɌĂ΂�郁�\�b�h
		virtual void Attach() = 0;
		/// ������������уf�o�C�X���X�g���̃��X�g�A����
		virtual void Restore() = 0;
	};

	typedef std::list<shared_ptr<IView> > ViewList;
} // end of namespace opk