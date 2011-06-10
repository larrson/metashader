#pragma once
/**
	@file CameraController.h
	@brief カメラ制御クラスの宣言	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CCameraController
		@brief カメラ制御クラス
	*/
	class CCameraController : public MouseHandlerBase
	{
	private:		
		bool m_bLButtonDown; ///< 左ボタンが押されているか
		int m_nPrevX; ///< 以前のX座標
		int m_nPrevY; ///< 以前のY座標
		
		float m_fYaw;		///< カメラのヨー角度
		float m_fPitch;		///< カメラのピッチ角度
		float m_fDistance;	///< 注視点からの距離	
		float m_fFov;		///< 垂直画角

		CGraphicDevice::SCameraInfo m_cameraInfo; ///< カメラ情報

	public:
		/// コンストラクタ
		CCameraController();

		/// デストラクタ
		~CCameraController();

		/// カメラ情報を取得
		const CGraphicDevice::SCameraInfo& GetCameraInfo(){ return m_cameraInfo; }

	protected:
		/// マウスカーソルが動いた
		virtual bool OnMouseMove( int x, int y );
		/// 左ボタンが押された
		virtual bool OnLButtonDown( int x, int y  );
		/// 左ボタンが離された
		virtual bool OnLButtonUp( int x, int y );
		/// 右ボタンが押された
		virtual bool OnRButtonDown( int x, int y ){ return false; }
		/// 右ボタンが離された
		virtual bool OnRButtonUp( int x, int y ){ return false; }
		/// ホイールが回された
		virtual bool OnMouseWheel( int i_nDelta );

	private:
		/// カメラ情報の更新
		void UpdateCameraInfo();
	};
} // end of namespace opk