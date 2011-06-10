/**
	@file Mouse.cpp
	@brief �}�E�X���n���h�����O���s���N���X�̒�`
*/

// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"

namespace opk
{
	// Function Definitions ----------------------------------------------------------------------

	//--------------------------------------------------------------------------------------------
	bool MouseHandlerBase::OnMsgProc( HWND i_hWnd, UINT i_nMsg, WPARAM i_wParam, LPARAM i_lParam )
	{
		bool ret = false;

		// �}�E�X�Ɋ֌W���郁�b�Z�[�W���n���h�����O����
		switch( i_nMsg )
		{
		case WM_MOUSEMOVE:
			ret = OnMouseMove(LOWORD(i_lParam), HIWORD(i_lParam));
			break;
		case WM_LBUTTONDOWN:
			ret = OnLButtonDown(LOWORD(i_lParam), HIWORD(i_lParam));
			break;
		case WM_LBUTTONUP:
			ret = OnLButtonUp(LOWORD(i_lParam), HIWORD(i_lParam));
			break;
		case WM_RBUTTONDOWN:
			ret = OnRButtonDown(LOWORD(i_lParam), HIWORD(i_lParam));
			break;
		case WM_RBUTTONUP:
			ret = OnRButtonUp(LOWORD(i_lParam), HIWORD(i_lParam));
			break;
		case WM_MOUSEWHEEL:
			ret = OnMouseWheel( GET_WHEEL_DELTA_WPARAM(i_wParam) / WHEEL_DELTA );
			break;
		}

		return ret;
	}

} // end of namespace opk