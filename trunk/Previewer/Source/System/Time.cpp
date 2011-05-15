/**
	@file Time.cpp
	@brief ���Ԍv���N���X
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"

namespace opk
{
	// Global Variable Definitions ---------------------------------------------------------------	

	// Function Definitions ----------------------------------------------------------------------

	//------------------------------------------------------------------------------------------
	CTime::CTime()
	{		
		// ������
		Init();
	}

	//------------------------------------------------------------------------------------------
	CTime::~CTime()
	{		
	}

	//------------------------------------------------------------------------------------------
	void CTime::Init()
	{
		// ���g���̏�����
		QueryPerformanceFrequency( &m_nFrequency );						
	}

	//------------------------------------------------------------------------------------------
	void CTime::Start()
	{
		// �J�n�����̏�����
		QueryPerformanceCounter( &m_nStartedCounter );		

		// �X�V�����̏�����
		m_nPrevCounter = m_nStartedCounter;
		m_nCurrentCounter = m_nStartedCounter;
	}

	//------------------------------------------------------------------------------------------
	void CTime::Update()
	{
		// ��O�̎������X�V
		m_nPrevCounter = m_nCurrentCounter;

		// ���ݎ������X�V����
		QueryPerformanceCounter( &m_nCurrentCounter );		
	}		
} // end of namespace opk