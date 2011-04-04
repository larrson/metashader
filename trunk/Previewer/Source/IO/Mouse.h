#pragma once
/**
	@file Mouse.h
	@brief �}�E�X�̃n���h�����O���s���N���X�̐錾
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class MouseHandlerBase
		@brief �}�E�X���n���h�����O���邽�߂̊��N���X
	*/
	class MouseHandlerBase
	{
	public:
		/// �R���X�g���N�^
		MouseHandlerBase(){}

		/// �f�X�g���N�^
		virtual ~MouseHandlerBase(){}

	protected:
		/// �}�E�X�J�[�\����������
		virtual bool OnMouseMove( int x, int y ) = 0;
		/// ���{�^���������ꂽ
		virtual bool OnLButtonDown( int x, int y  ) = 0;
		/// ���{�^���������ꂽ
		virtual bool OnLButtonUp( int x, int y ) = 0;
		/// �E�{�^���������ꂽ
		virtual bool OnRButtonDown( int x, int y ) = 0;
		/// �E�{�^���������ꂽ
		virtual bool OnRButtonUp( int x, int y ) = 0;
		/// �z�C�[�����񂳂ꂽ
		virtual bool OnMouseWheel( int i_nDelta ) = 0;
	public:
		/// ���b�Z�[�W����
		bool OnMsgProc( HWND i_hWnd, UINT i_nMsg, WPARAM i_wParam, LPARAM i_lParam );	
	};
} // end of namespace opk