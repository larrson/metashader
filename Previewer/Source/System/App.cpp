/**
	@file App.cpp
	@brief �A�v���P�[�V�����N���X	
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "App.h"


namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------
	CApp* CApp::s_pInstance = NULL;

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CApp::CApp()
		: m_pGraphicDevice(NULL)
		, m_model(L"C:\\projects\\metashader\\data\\model\\Sphere.x") ///< ���f�� @@@�폜
	{

	}

	//------------------------------------------------------------------------------------------
	CApp::~CApp()
	{
		SAFE_DELETE( m_pGraphicDevice );
	}

	//------------------------------------------------------------------------------------------
	bool CApp::CreateInstance()
	{
		if( !s_pInstance )
		{
			s_pInstance = new CApp();
		}
		return (s_pInstance != NULL );
	}

	//------------------------------------------------------------------------------------------
	void CApp::DisposeInstance()
	{				
		SAFE_DELETE( s_pInstance );
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Initialize(LPWSTR i_lpCmdLine, int i_nScreenWidth, int i_nScreenHeight)
	{						
		// �A�v���P�[�V�����̃f�B���N�g���̃p�X�̏�����
		InitializeApplicationDirectoryPath();

		// �f�o�C�X�̏�����
		m_pGraphicDevice = new CGraphicDevice();
		m_pGraphicDevice->Initialize(i_nScreenWidth, i_nScreenHeight);

		m_model.Restore(); ///< ���f�� @@@�폜		

		return true;
	}

	//------------------------------------------------------------------------------------------
	void CApp::InitializeApplicationDirectoryPath()
	{
		char drive[MAX_PATH];
		char dir[MAX_PATH];
		char fname[MAX_PATH];
		char ext[MAX_PATH];

		const HMODULE handle = GetModuleHandle(NULL);
		GetModuleFileNameA( handle, m_appDirectoryPath, MAX_PATH);	
		_splitpath_s( m_appDirectoryPath, drive, dir, fname, ext);

		// �A�v���P�[�V�����̃f�B���N�g�����̃p�X��ێ�
		sprintf_s(m_appDirectoryPath, MAX_PATH, "%s%s", drive, dir);
	}

	//------------------------------------------------------------------------------------------
	bool CApp::ResetDevice(int i_nScreenWidth, int i_nScreenHeight )	
	{
		// ���f���̔j��
		m_model.Destroy();

		// �f�o�C�X�̃��Z�b�g
		if( m_pGraphicDevice && m_pGraphicDevice->Reset( i_nScreenWidth, i_nScreenHeight) == false )
		{
			return false;
		}

		// ���f���̃��Z�b�g
		if( FAILED( m_model.Restore() ) )
		{
			return false;
		}

		// @@@ �e�N�X�`�����\�[�X�̃��Z�b�g
		// @@@ �V�F�[�_�̃��Z�b�g

		return true;
	}	

	//------------------------------------------------------------------------------------------
	LRESULT CApp::MsgProc( HWND i_hWnd, int i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
	{
		// �n���h�����Ȃ�
		bool result = false;

		// ���b�Z�[�W����
		switch( i_nMsg )
		{
		case WM_KEYDOWN:
		case WM_KEYUP:
		case WM_CHAR:
		case WM_MOUSEMOVE:
		case WM_LBUTTONDOWN:
		case WM_LBUTTONUP:
		case WM_RBUTTONDOWN:
		case WM_RBUTTONUP:
		case WM_MOUSEWHEEL:
			// @@@���ėp�I�ȏ�����
			m_cameraController.OnMsgProc( i_hWnd, i_nMsg, i_wParam, i_lParam );
			break;
		}
		
		// �f�t�H���g����
		return ::DefWindowProc( i_hWnd, i_nMsg, i_wParam, i_lParam);
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Update()
	{
		/// @@@@		

		return true;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Render()
	{		
		// �����_�����O�J�n
		if( FAILED(m_pGraphicDevice->Activate()) )
		{	
			return false;
		}

		m_pGraphicDevice->Clear( 0.0f, 0.0f, 0.0f, 0.0f );				
		
		m_pGraphicDevice->SetCameraInfo( m_cameraController.GetCameraInfo() );

		m_model.Render(); ///< ���f�� @@@�폜

		// �����_�����O�I��
		m_pGraphicDevice->Deactivate();

		return true;
	}
} // end of namespace opk