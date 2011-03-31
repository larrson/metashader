/**
	@file App.h
	@brief �A�v���P�[�V�����N���X
	@note �V���O���g��
	@attension �C���X�^���X�̍쐬�Ɣj���͎蓮�ŊǗ�����
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CApp
		@brief �A�v���P�[�V�����N���X
	*/
	class CApp
	{
	private:
		static CApp*	s_pInstance; ///< �V���O���g���̗B��̃C���X�^���X				

		CGraphicDevice*	m_pGraphicDevice; ///< �O���t�B�b�N�f�o�C�X

	public:
		IDirect3DTexture9* m_pd3dTexture;

		/// �C���X�^���X�̍쐬
		static bool CreateInstance();

		/// �C���X�^���X�̔j��
		static void DisposeInstance();

		/// �C���X�^���X�̎擾
		static CApp* GetInstance(){ return s_pInstance; }

		/// �O���t�B�b�N�f�o�C�X�̎擾
		CGraphicDevice* GetGraphicDevice(){ return m_pGraphicDevice; }

		/** 
			@brief ������			
			@param [in] i_lpCmdLine �R�}���h���C���p�����[�^			
			@param [in] i_nScreenWidth	�X�N���[���̕�
			@param [in] i_nScreenHeight	�X�N���[���̍���
			@retval �������ɐ���������			
		*/
		bool Initialize(LPWSTR i_lpCmdLine, int i_nScreenWidth, int i_nScreenHeight);
		
		/**
			@brief �O���t�B�b�N�f�o�C�X�̃��Z�b�g
			@param [in] i_nScreenWidth	�X�N���[���̕�
			@param [in] i_nScreenHeight	�X�N���[���̍���
			@retval ���Z�b�g�ɐ���������
		*/
		bool ResetDevice(int i_nScreenWidth, int i_nScreenHeight );		

		/// �X�V
		bool Update();

		/// �����_�����O
		bool Render();

	private:
		/// �R���X�g���N�^
		CApp();

		/// �f�X�g���N�^
		~CApp();

		/// �R�s�[�R���X�g���N�^�̋֎~
		CApp(const CApp& app);

		/// ������Z�̋֎~
		CApp& operator=(const CApp& app);
	};
} // end of namespace opk