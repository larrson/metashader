/**
	@file Time.cpp
	@brief 時間計測クラス
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
		// 初期化
		Init();
	}

	//------------------------------------------------------------------------------------------
	CTime::~CTime()
	{		
	}

	//------------------------------------------------------------------------------------------
	void CTime::Init()
	{
		// 周波数の初期化
		QueryPerformanceFrequency( &m_nFrequency );						
	}

	//------------------------------------------------------------------------------------------
	void CTime::Start()
	{
		// 開始時刻の初期化
		QueryPerformanceCounter( &m_nStartedCounter );		

		// 更新時刻の初期化
		m_nPrevCounter = m_nStartedCounter;
		m_nCurrentCounter = m_nStartedCounter;
	}

	//------------------------------------------------------------------------------------------
	void CTime::Update()
	{
		// 一つ前の時刻を更新
		m_nPrevCounter = m_nCurrentCounter;

		// 現在時刻を更新する
		QueryPerformanceCounter( &m_nCurrentCounter );		
	}		
} // end of namespace opk