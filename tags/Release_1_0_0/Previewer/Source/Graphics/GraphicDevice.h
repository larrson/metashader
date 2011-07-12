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

		/**
			@brief ���u�����f�B���O�̎��
		*/
		enum BlendMode
		{
			BlendMode_None,		///< ����
			BlendMode_Normal,	///< �ʏ�
			BlendMode_Add,		///< ���Z
			BlendMode_Sub,		///< ���Z
		};

		/**
			@struct SDirLightInfo
			@brief ���s�������̍\����
		*/
		struct SDirLightInfo
		{
			D3DXVECTOR3 vColor;		///< �F
			D3DXVECTOR3 vDir;		///< ����
			bool		bEnable;	///< �L���t���O
		};

		/**
			@brief ���C�g�̍ő吔
		*/
		enum LightMax {
			DirLight_Max = 3, ///< ���s�����̍ő吔
		};

	private:
		IDirect3D9*			m_pd3d9;			///< DirectX�I�u�W�F�N�g	
		IDirect3DDevice9*	m_pd3dDevice9;		///< �O���t�B�b�N�f�o�C�X
		IDirect3DSurface9*	m_pd3dSurface9;		///< �T�[�t�F�[�X(�_�u���o�b�t�@)
		D3DPRESENT_PARAMETERS m_d3dpp;			///< �v���[���e�[�V�����p�����[�^

		HWND m_hWnd;		///< �_�~�[�̃E�B���h�E�n���h��

		bool m_bValid;		///< �L����
		bool m_bActive;		///< ���s����		

		int m_nWidth;  ///< �o�b�N�o�b�t�@�̕�
		int m_nHeight; ///< �o�b�N�o�b�t�@�̍���		

		D3DVIEWPORT9	m_viewport; ///< �r���[�|�[�g

		SCameraInfo		m_cameraInfo; ///< �J�������

		D3DXMATRIX		m_mTransform[TransformType_Max]; ///< �ϊ��s��

		BlendMode		m_nBlendMode; ///< �u�����h���[�h

		SDirLightInfo	m_dirLightInfo[DirLight_Max];	///< ���s����

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

		/// �ϊ��s��̐ݒ�
		HRESULT SetTransform( TransformType i_nTransformType, D3DXMATRIX i_mMatrix );

		/// �ϊ��s��̎擾
		const D3DXMATRIX& GetTransform( TransformType i_nTransformType ) const { return m_mTransform[i_nTransformType]; }

		/// �J�������̎擾
		const SCameraInfo& GetCameraInfo() const { return m_cameraInfo; }

		/// �J�������̐ݒ�
		HRESULT SetCameraInfo( const SCameraInfo& i_cameraInfo );	
		
		/** 
			@brief �u�����h���[�h�̐ݒ�
			@param [in] i_nBlendMode �u�����h���[�h
			@param [in] i_bForced �����ݒ���s�����itrue�Ȃ�Ό��݂̏�ԂɊ֌W�Ȃ��ݒ肷��j
		*/
		HRESULT SetBlendMode( BlendMode i_nBlendMode, bool i_bForced = false );

		/**
			@brief ���s�����̗L��/�����؂�ւ�
			@param [in] i_nIndex ���s�����̃C���f�b�N�X
			@param [in] i_bEnable �L��or����
			@note i_nIndex�͈̔͂�[0, DirLight_Max - 1]
		*/
		void SetDirLightEnable( int i_nIndex, bool i_bEnable );

		/**
			@brief ���s�����̐F���擾����
			@param [in] i_nIndex ���s�����̃C���f�b�N�X
			@note i_nIndex�͈̔͂�[0, DirLight_Max - 1]
		*/
		D3DXVECTOR3 GetDirLightColor( int i_nIndex );

		/**
			@brief ���s�����̐F��ݒ肷��
			@param [in] i_nIndex ���s�����̃C���f�b�N�X
			@param [in] i_fR �Ԑ���
			@param [in] i_fG �ΐ���
			@param [in] i_fB ����
			@note i_nIndex�͈̔͂�[0, DirLight_Max - 1]
		*/
		void SetDirLightColor( int i_nIndex, float i_fR, float i_fG, float i_fB);

		/**
			@brief ���s�����̕������擾����
			@param [in] i_nIndex ���s�����̃C���f�b�N�X
			@note i_nIndex�͈̔͂�[0, DirLight_Max - 1]
		*/
		D3DXVECTOR3 GetDirLightDir( int i_nIndex );

		/**
			@brief ���s�����̕�����ݒ肷��
			@param [in] i_nIndex ���s�����̃C���f�b�N�X
			@param [in] i_fX �����x�N�g����X����
			@param [in] i_fY �����x�N�g����Y����
			@param [in] i_fZ �����x�N�g����Z����
			@note i_nIndex�͈̔͂�[0, DirLight_Max - 1]
		*/
		void SetDirLightDir( int i_nIndex, float i_fX, float i_fY, float i_fZ);

		/** 
			@brief �o�b�t�@�̃N���A
			@attension �e�p�����[�^�͈̔͂�[0,1]
			@param [in] i_fR ���b�h
			@param [in] i_fG �O���[��
			@param [in] i_fB �u���[
			@param [in] i_fA �A���t�@	
			@return �G���[�R�[�h
		*/
		HRESULT Clear(float i_fR, float i_fG, float i_fB, float i_fA);

	private:
		/// �_�~�[�̃E�B���h�E���쐬����
		bool CreateDummyWindow();

		/// �O���t�B�b�N�f�o�C�X���쐬����
		bool CreateDevice( int i_nWidth, int i_nHeight );

		/// �����_�[�^�[�Q�b�g���쐬����
		bool CreateRenderTarget( int i_nWidth, int i_nHeight );		
	};
} // end of namespace opk