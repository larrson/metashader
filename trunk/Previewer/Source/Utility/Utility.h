#pragma once
/**
	@file utility.h
	@brief 便利関数・マクロの定義		
*/

// Includes ----------------------------------------------------------------------------------

// Macro Definitions -------------------------------------------------------------------------
#define SAFE_DELETE(p)			if( (p) != NULL ) { delete(p); (p) = NULL; }
#define SAFE_DELETE_ARRAY(p)	if( (p) != NULL ) { delete[] (p); (p) = NULL; }
#define SAFE_RELEASE(p)			if( (p) != NULL ) { (p)->Release(); (p) = NULL; }