#pragma once
/**
	@file GraphicDevice.h
	@brief �O���t�B�b�N�f�o�C�X�N���X	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CGraphicDevice
		@brief �O���t�B�b�N�f�o�C�X���Ǘ�����N���X
	*/
	class CGraphicDevice
	{
	public:
		/**
			@brief ���W�ϊ��̎��
			@note CGraphicDevice���ێ�����s��̃C���f�b�N�X�ɑΉ�����
		*/
		enum TransformType
		{
			TransformType_World,		///< ���[���h�ϊ�
			TransformType_View,			///< �r���[�ϊ�
			TransformType_Projection,	///< �ˉe�ϊ�
			TransformType_Max,			///< �ő吔
		};

		/**
			@struct SCameraInfo
			@brief �J�������\����
		*/
		struct SCameraInfo
		{
			D3DXVECTOR3 vEyePos;		///< ���_�ʒu
			D3DXVECTOR3 vInterestPos;	///< �����_�ʒu
			D3DXVECTOR3 vUpDir;			///< �J����������x�N�g��
			float		fFov;			///< ������p(���W�A���P��)
			float		fNear;			///< �߃N���b�v���ʂ܂ł̋���
			float		fFar;			///< ���N���b�v���ʂ܂ł̋���
		};

	private:
		IDirect3D9*			m_pd3d9;		///< DirectX�I�u�W�F�N�g	
		IDirect3DDevice9*	m_pd3dDevice9;  ///< �O���t�B�b�N�f�o�C�X
		IDirect3DSurface9*	m_pd3dSurface9;	///< �T�[�t�F�[�X

		HWND m_hWnd;		///< �_�~�[�̃E�B���h�E�n���h��

		bool m_bActive;		///< ���s����

		int m_nWidth;  ///< �o�b�N�o�b�t�@�̕�
		int m_nHeight; ///< �o�b�N�o�b�t�@�̍���		

		D3DVIEWPORT9	m_viewport; ///< �r���[�|�[�g

		SCameraInfo		m_cameraInfo; ///< �J�������

		D3DXMATRIX		m_mTransform[TransformType_Max]; ///< �ϊ��s��

	public:
		/// �R���X�g���N�^
		CGraphicDevice();

		/// �f�X�g���N�^
		~CGraphicDevice();

		/// �O���t�B�b�N�f�o�C�X�̎擾
		IDirect3DDevice9* GetD3DDevice(){ return m_pd3dDevice9; }

		/// ���s����
		bool ISActive(){ return m_bActive; }

		/// �X�N���[�����̎擾
		int GetWidth(){ return m_nWidth; }

		/// �X�N���[�������̎擾
		int GetHeight(){ return m_nHeight; }

		/// ������
		bool Initialize(int i_nWidth, int i_nHeight);

		/// �j��.
		void Dispose();

		/// �f�o�C�X�̃��Z�b�g
		bool Reset(int i_nWidht, int i_nHeight);	

		/// 3D�V�[���̃����_�����O�J�n
		HRESULT Activate();

		/// 3D�V�[���̃����_�����O�I��
		void Deactivate();

		/// �o�b�N�o�b�t�@���擾
		IDirect3DSurface9* GetBackBuffer();

		/// ��ʂ̃��T�C�Y
		void Resize(int i_nScreenWidth, int i_nScreenHeight );	

		/// �ϊ��s��̐ݒ�
		HRESULT SetTransform( TransformType i_nTransformType, D3DXMATRIX i_mMatrix );

		/// �J�������̎擾
		const SCameraInfo& GetCameraInfo() const { return m_cameraInfo; }

		/// �J�������̐ݒ�
		HRESULT SetCameraInfo( const SCameraInfo& i_cameraInfo );		

	private:
		/// �_�~�[�̃E�B���h�E���쐬����
		bool CreateDummyWindow();

		/// �O���t�B�b�N�f�o�C�X���쐬����
		bool CreateDevice( int i_nWidth, int i_nHeight );

		/// �����_�[�^�[�Q�b�g���쐬����
		bool CreateRenderTarget( int i_nWidth, int i_nHeight );		
	};
} // end of namespace opk