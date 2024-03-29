/**
	@file CameraController.cpp
	@brief カメラ制御クラスの定義
*/
// Includes ----------------------------------------------------------------------------------
#include "stdafx.h"
#include "CameraController.h"

// Global Variables Definitions ------------------------------------------------------------------
namespace
{
	static const float c_fMaxPitch = D3DXToRadian(85.0f);
	static const float c_fMinFov = D3DXToRadian(10.0f);
	static const float c_fMaxFov = D3DXToRadian(60.0f);
	static const float c_fDefaultFov = D3DXToRadian(30.0f);
}

namespace opk
{
	// Function Definitions ----------------------------------------------------------------------

	//-------------------------------------------------------------------------------------------
	CCameraController::CCameraController()
		: MouseHandlerBase()
		, m_bLButtonDown( false )
		, m_nPrevX(0)
		, m_nPrevY(0)
		, m_fYaw	( 0.0f )
		, m_fPitch	( 0.0f )
		, m_fDistance ( 200.0f )
		, m_fFov( c_fDefaultFov )
	{
		// カメラ情報の初期化
		m_cameraInfo.fNear = 1.0f;
		m_cameraInfo.fFar = 3000.0f;		
		m_cameraInfo.vInterestPos = D3DXVECTOR3( 0.0f, 0.0f, 0.0f );		
		m_cameraInfo.vUpDir = D3DXVECTOR3( 0.0f, 1.0f, 0.0f );
		// 初期パラメータに基づいて更新
		UpdateCameraInfo();
	}

	//-------------------------------------------------------------------------------------------
	CCameraController::~CCameraController()
	{

	}

	//-------------------------------------------------------------------------------------------
	bool CCameraController::OnMouseMove( int x, int y )
	{
		static float fScaleX = 0.03f;
		static float fScaleY = 0.03f;

		// 左クリック中のみカメラ回転
		if( m_bLButtonDown )
		{					
			m_fYaw -= (x - m_nPrevX) * fScaleX;			
			m_fPitch -= (y - m_nPrevY) * fScaleY;
			m_fPitch = (m_fPitch < 0.0f ? -1.0f : 1.0f ) * min( fabs(m_fPitch), c_fMaxPitch );

			// 位置を保存
			m_nPrevX = x; 
			m_nPrevY = y;

			// カメラ情報を更新
			UpdateCameraInfo();
		}

		return true;
	}

	//-------------------------------------------------------------------------------------------
	bool CCameraController::OnLButtonDown( int x, int y )
	{
		// 押下フラグをON
		m_bLButtonDown = true;
		// 位置を保存
		m_nPrevX = x;
		m_nPrevY = y;
		
		return true;
	}

	//-------------------------------------------------------------------------------------------
	bool CCameraController::OnLButtonUp( int x, int y )
	{
		// 押下フラグをOFF
		m_bLButtonDown = false;

		return true;
	}

	//-------------------------------------------------------------------------------------------
	bool CCameraController::OnMouseWheel( int i_nDelta )
	{
		// ズーム処理
		static float fScale = D3DXToRadian(1.0f);
		
		m_fFov += -i_nDelta * fScale;
		m_fFov = min( c_fMaxFov, max(m_fFov, c_fMinFov ));

		// カメラ情報を更新
		UpdateCameraInfo();

		return true;
	}

	//-------------------------------------------------------------------------------------------
	void CCameraController::UpdateCameraInfo()
	{
		// カメラ情報の更新

		// カメラの位置を求める
		D3DXVECTOR3 vPos( 0.0f, 0.0f, m_fDistance );
		D3DXMATRIX mRotX, mRotY, mRot;
		D3DXMatrixRotationX( &mRotX, m_fPitch );
		D3DXMatrixRotationY( &mRotY, m_fYaw );
		D3DXMatrixMultiply( &mRot, &mRotX, &mRotY );

		D3DXVECTOR4 vNewPos;
		D3DXVec3Transform( &vNewPos, &vPos, &mRot );

		m_cameraInfo.vEyePos.x = vNewPos.x;
		m_cameraInfo.vEyePos.y = vNewPos.y;
		m_cameraInfo.vEyePos.z = vNewPos.z;

		m_cameraInfo.fFov = m_fFov;
	}
} // end of namespace opk