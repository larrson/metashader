/**
	@file Debug.cpp
	@brief �f�o�b�O�p�}�N�����֐��̒�`
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"

// Function Definitions ----------------------------------------------------------------------
namespace opk
{
	void CDebugTrace::Trace(const char* i_pszFormat, ...)
	{
		char buff[1024];

		va_list vaarg;
		va_start(vaarg, i_pszFormat);
		vsnprintf_s( buff, sizeof(buff), _TRUNCATE, i_pszFormat, vaarg );
		va_end(vaarg);

		::OutputDebugStringA(buff);
	}	
} // end of namespace opk
