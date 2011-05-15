/**
	@file App.cpp
	@brief �A�v���P�[�V�����N���X	
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "System/Logic.h"

namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------
	CApp* CApp::s_pInstance = NULL;

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CApp::CApp()
		: m_pGraphicDevice(NULL)
		, m_pTime ( NULL )
		, m_pLogic( NULL )		
	{
	}

	//------------------------------------------------------------------------------------------
	CApp::~CApp()
	{
		SAFE_DELETE( m_pGraphicDevice );
		SAFE_DELETE( m_pTime );
		SAFE_DELETE( m_pLogic );
	}

	//------------------------------------------------------------------------------------------
	bool CApp::CreateInstance( CLogicBase *i_pLogic )
	{
		if( !s_pInstance )
		{
			s_pInstance = new CApp();
			s_pInstance->m_pLogic = i_pLogic;
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

		// �O���t�B�b�N�f�o�C�X�̏�����
		m_pGraphicDevice = new CGraphicDevice();
		m_pGraphicDevice->Initialize(i_nScreenWidth, i_nScreenHeight);

		// ���W�b�N�̏�����
		MY_ASSERT( m_pLogic );
		m_pLogic->Initialize();		

		// �^�C�}�[�̏�����
		MY_ASSERT( m_pTime == NULL );
		m_pTime = new CTime();		
		m_pTime->Start();

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
		// �f�o�C�X�̃��Z�b�g
		if( m_pGraphicDevice && m_pGraphicDevice->Reset( i_nScreenWidth, i_nScreenHeight) == false )
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
		HRESULT ret = 0;

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
			// ���W�b�N�ɏ������ڏ�		
			ret = m_pLogic->MsgProc( i_hWnd, i_nMsg, i_wParam, i_lParam );			
			break;
		default:
			// �f�t�H���g����
			ret = ::DefWindowProc( i_hWnd, i_nMsg, i_wParam, i_lParam);
		}		
		
		return ret;
	}

	//------------------------------------------------------------------------------------------
	bool CApp::Update()
	{
		// �^�C�}�[���X�V
		m_pTime->Update();

		// ���W�b�N�ɏ������ڏ�		
		m_pLogic->Update( (float)m_pTime->GetTotalTime(), (float)m_pTime->GetElapsedDeltaTime() );

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

		// ���W�b�N�ɏ������ڏ�
		m_pLogic->Render();					

		// �����_�����O�I��
		m_pGraphicDevice->Deactivate();

		return true;
	}
} // end of namespace opk