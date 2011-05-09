#pragma once
/**
	@file CameraController.h
	@brief �J��������N���X�̐錾	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CCameraController
		@brief �J��������N���X
	*/
	class CCameraController : public MouseHandlerBase
	{
	private:		
		bool m_bLButtonDown; ///< ���{�^����������Ă��邩
		int m_nPrevX; ///< �ȑO��X���W
		int m_nPrevY; ///< �ȑO��Y���W
		
		float m_fYaw;		///< �J�����̃��[�p�x
		float m_fPitch;		///< �J�����̃s�b�`�p�x
		float m_fDistance;	///< �����_����̋���	
		float m_fFov;		///< ������p

		CGraphicDevice::SCameraInfo m_cameraInfo; ///< �J�������

	public:
		/// �R���X�g���N�^
		CCameraController();

		/// �f�X�g���N�^
		~CCameraController();

		/// �J���������擾
		const CGraphicDevice::SCameraInfo& GetCameraInfo(){ return m_cameraInfo; }

	protected:
		/// �}�E�X�J�[�\����������
		virtual bool OnMouseMove( int x, int y );
		/// ���{�^���������ꂽ
		virtual bool OnLButtonDown( int x, int y  );
		/// ���{�^���������ꂽ
		virtual bool OnLButtonUp( int x, int y );
		/// �E�{�^���������ꂽ
		virtual bool OnRButtonDown( int x, int y ){ return false; }
		/// �E�{�^���������ꂽ
		virtual bool OnRButtonUp( int x, int y ){ return false; }
		/// �z�C�[�����񂳂ꂽ
		virtual bool OnMouseWheel( int i_nDelta );

	private:
		/// �J�������̍X�V
		void UpdateCameraInfo();
	};
} // end of namespace opk