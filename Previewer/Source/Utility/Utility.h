#pragma once
/**
	@file utility.h
	@brief 便利関数・マクロの定義		
*/

// Includes ----------------------------------------------------------------------------------
#include <Utility/Debug.h>

// Macro Definitions -------------------------------------------------------------------------
#define SAFE_DELETE(p)			if( (p) != NULL ) { delete(p); (p) = NULL; }
#define SAFE_DELETE_ARRAY(p)	if( (p) != NULL ) { delete[] (p); (p) = NULL; }
#define SAFE_RELEASE(p)			if( (p) != NULL ) { (p)->Release(); (p) = NULL; }

#ifdef _DEBUG
#define V_RETURN(x) { hr = (x); if( FAILED(hr) ) { \
	MY_TRACE(			\
	"\n-V_RETURN--------------------------------------\n"	\
	"hr  : %d\n"			\
	"Expr: ( %s )\n"	\
	"At: %s(%d) \n"		\
	"In Function: %s\n"	\
	"\n-----------------------------------------------\n",	\
	hr, #x, __FILE__, __LINE__, __FUNCTION__); \
	MY_BREAK(); \
	return hr; \
  } \
}
#else  // _DEBUG
#define V_RETURN(x) { hr = (x); if( FAILED(hr) ) { return hr; } }
#endif // _DEBUG

#ifdef _DEBUG
#define V_CALL(x) { hr = (x); if( FAILED(hr) ) { \
	MY_TRACE(			\
	"\n-V_CALL----------------------------------------\n"	\
	"hr  : %d\n"			\
	"Expr: ( %s )\n"	\
	"At: %s(%d) \n"		\
	"In Function: %s\n"	\
	"\n-----------------------------------------------\n",	\
	hr, #x, __FILE__, __LINE__, __FUNCTION__); \
	MY_BREAK(); \
	}\
}
#else  // _DEBUG
#define V_CALL(x) { hr = (x); }
#endif // _DEBUG