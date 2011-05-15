#pragma once
/**
	@file PreviewerLogic.h
	@brief �v���r���[�A�̃A�v���P�[�V�������W�b�N��\���N���X
	@note �������ꂽ��̃C���X�^���X��CApp���Ǘ�
*/
// Includes ----------------------------------------------------------------------------------


// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CPreviwerLogic
		@brief �v���r���[�A�p�̃��W�b�N�N���X
	*/
	class CPreviewerLogic : public CLogicBase
	{
	private:

	public:
		/// �R���X�g���N�^
		CPreviewerLogic();

		/// �f�X�g���N�^
		virtual ~CPreviewerLogic();		
				
		/// ������
		virtual void Initialize();		

		/// �`��
		virtual void Render();
	};
} // end of namespace opk