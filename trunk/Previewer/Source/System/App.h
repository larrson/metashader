#pragma once
/**
	@file App.h
	@brief �A�v���P�[�V�����N���X
	@note �V���O���g��
	@attension �C���X�^���X�̍쐬�Ɣj���͎蓮�ŊǗ�����
*/

// Includes ----------------------------------------------------------------------------------
#include "Application/CameraController.h" //@@@�폜�\��

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	class CLogicBase;
	class CTime;

	/**
		@class CApp
		@brief �A�v���P�[�V�����N���X
	*/
	class CApp
	{		
	private:
		static CApp*	s_pInstance; ///< �V���O���g���̗B��̃C���X�^���X	

		char m_appDirectoryPath[MAX_PATH];	///< �A�v���P�[�V�����̃f�B���N�g���̃p�X

		CGraphicDevice*	m_pGraphicDevice;	///< �O���t�B�b�N�f�o�C�X		

		CTime*				m_pTime;				///< �^�C�}�[
		
		CLogicBase*			m_pLogic;				///< �A�v���P�[�V�������W�b�N				
	public:		
		/// �C���X�^���X�̍쐬
		static bool CreateInstance( CLogicBase* i_pLogic );

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

		/**
			@brief Win32�̃��b�Z�[�W�n���h��
		*/
		LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam );

		/// �X�V
		bool Update();

		/// �����_�����O
		bool Render();

		/**
			@brief �A�v���P�[�V�����̎��s�t�@�C���̒u����Ă���f�B���N�g���̃p�X���擾����			
			@note �擾����镶����̍Ō�ɂ́A�p�X��؂蕶���u\�v���t������Ă���
		*/
		const char*	GetApplicationDirectory(){ return m_appDirectoryPath; };

	private:
		/// �R���X�g���N�^
		CApp();

		/// �f�X�g���N�^
		~CApp();

		/// �R�s�[�R���X�g���N�^�̋֎~
		CApp(const CApp& app);

		/// ������Z�̋֎~
		CApp& operator=(const CApp& app);

		/**
			@brief �A�v���P�[�V�����f�B���N�g���̏�����
		*/
		void InitializeApplicationDirectoryPath();		
	};

	/**
		@class ILogic
		@brief �A�v���P�[�V�����̃��W�b�N���L�q������N���X
	*/
	class ILogic
	{
	public:
		/// �f�X�g���N�^
		virtual ~ILogic(){};

		/**
			@brief Win32�̃��b�Z�[�W�n���h��
		*/
		virtual LRESULT MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam ) = 0;
		/// �X�V
		virtual void Update(float i_fTime, float i_fElapsedTime) = 0;
		/// �����_�����O
		virtual void Render() = 0;
		/// ������
		virtual void Initialize() = 0;		
	};
} // end of namespace opk