#pragma once
/**
	@file utility.h
	@brief �֗��֐��E�}�N���̒�`		
*/

// Includes ----------------------------------------------------------------------------------

// Macro Definitions -------------------------------------------------------------------------
#define SAFE_DELETE(p)			if( (p) != NULL ) { delete(p); (p) = NULL; }
#define SAFE_DELETE_ARRAY(p)	if( (p) != NULL ) { delete[] (p); (p) = NULL; }
#define SAFE_RELEASE(p)			if( (p) != NULL ) { (p)->Release(); (p) = NULL; }