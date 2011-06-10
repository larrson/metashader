#pragma once
/**
	@file Debug.h
	@brief デバッグ用マクロ＆関数の定義
*/

// Includes ----------------------------------------------------------------------------------
#include <Windows.h>

// Macro Definitions -------------------------------------------------------------------------

#ifdef _DEBUG
#define MY_BREAK() ::DebugBreak()
#define MY_TRACE(...) opk::CDebugTrace::Trace(__VA_ARGS__)

/// アサート
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


/// メッセージ付きアサート
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
 // _DEBUG未定義時
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
		@brief TTYへのデバッグ出力クラス
	*/
	class CDebugTrace
	{
	public:
		/**
			@brief TTYへ出力
			@param [in] i_pszFormat 書式指定の文字列
		*/
		static void Trace(const char* i_pszFormat, ...);						
	};
} // end of namespace opk