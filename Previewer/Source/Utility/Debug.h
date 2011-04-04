#pragma once
/**
	@file Debug.h
	@brief �f�o�b�O�p�}�N�����֐��̒�`
*/

// Includes ----------------------------------------------------------------------------------
#include <Windows.h>

// Macro Definitions -------------------------------------------------------------------------

#ifdef _DEBUG
#define MY_BREAK() ::DebugBreak()
#define MY_TRACE(...) opk::CDebugTrace::Trace(__VA_ARGS__)

/// �A�T�[�g
#define MY_ASSERT(expr) \
if( !(expr) ) \
{ \
	MY_TRACE(			\
		"\n-Assertion-------------------------------------\n"	\
		"Expr: ( %s )\n"	\
		"At: %s(%d) \n"		\
		"In Function: %s\n"	\
		"\n-----------------------------------------------\n",	\
		#expr, __FILE__, __LINE__, __FUNCTION__); \
	MY_BREAK(); \
}


/// ���b�Z�[�W�t���A�T�[�g
#define MY_ASSERT_MESS(expr, mess) \
if( !( expr ) ) \
{ \
	MY_TRACE(			\
	"\n-Assertion-------------------------------------\n"	\
	"%s\n"				\
	"Expr: ( %s )\n"	\
	"At: %s(%d) \n"		\
	"In Function: %s\n"	\
	"\n-----------------------------------------------\n",	\
	mess, #expr, __FILE__, __LINE__, __FUNCTION__); \
	MY_BREAK(); \
} 

#else 
 // _DEBUG����`��
#define MY_BREAK()
#define MY_TRACE(...)
#define MY_ASSERT(expr)
#define MY_ASSERT_MESS(expr, mess)

#endif // _DEBUG

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CDebugTrace
		@brief TTY�ւ̃f�o�b�O�o�̓N���X
	*/
	class CDebugTrace
	{
	public:
		/**
			@brief TTY�֏o��
			@param [in] i_pszFormat �����w��̕�����
		*/
		static void Trace(const char* i_pszFormat, ...);						
	};
} // end of namespace opk