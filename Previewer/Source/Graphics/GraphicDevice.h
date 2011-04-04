#pragma once
/**
	@file GraphicDevice.h
	@brief グラフィックデバイスクラス	
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CGraphicDevice
		@brief グラフィックデバイスを管理するクラス
	*/
	class CGraphicDevice
	{
	public:
		/**
			@brief 座標変換の種類
			@note CGraphicDeviceが保持する行列のインデックスに対応する
		*/
		enum TransformType
		{
			TransformType_World,		///< ワールド変換
			TransformType_View,			///< ビュー変換
			TransformType_Projection,	///< 射影変換
			TransformType_Max,			///< 最大数
		};

		/**
			@struct SCameraInfo
			@brief カメラ情報構造体
		*/
		struct SCameraInfo
		{
			D3DXVECTOR3 vEyePos;		///< 視点位置
			D3DXVECTOR3 vInterestPos;	///< 注視点位置
			D3DXVECTOR3 vUpDir;			///< カメラ上方向ベクトル
			float		fFov;			///< 垂直画角(ラジアン単位)
			float		fNear;			///< 近クリップ平面までの距離
			float		fFar;			///< 遠クリップ平面までの距離
		};

	private:
		IDirect3D9*			m_pd3d9;		///< DirectXオブジェクト	
		IDirect3DDevice9*	m_pd3dDevice9;  ///< グラフィックデバイス
		IDirect3DSurface9*	m_pd3dSurface9;	///< サーフェース

		HWND m_hWnd;		///< ダミーのウィンドウハンドル

		bool m_bActive;		///< 実行中か

		int m_nWidth;  ///< バックバッファの幅
		int m_nHeight; ///< バックバッファの高さ		

		D3DVIEWPORT9	m_viewport; ///< ビューポート

		SCameraInfo		m_cameraInfo; ///< カメラ情報

		D3DXMATRIX		m_mTransform[TransformType_Max]; ///< 変換行列

	public:
		/// コンストラクタ
		CGraphicDevice();

		/// デストラクタ
		~CGraphicDevice();

		/// グラフィックデバイスの取得
		IDirect3DDevice9* GetD3DDevice(){ return m_pd3dDevice9; }

		/// 実行中か
		bool ISActive(){ return m_bActive; }

		/// スクリーン幅の取得
		int GetWidth(){ return m_nWidth; }

		/// スクリーン高さの取得
		int GetHeight(){ return m_nHeight; }

		/// 初期化
		bool Initialize(int i_nWidth, int i_nHeight);

		/// 破棄.
		void Dispose();

		/// デバイスのリセット
		bool Reset(int i_nWidht, int i_nHeight);	

		/// 3Dシーンのレンダリング開始
		HRESULT Activate();

		/// 3Dシーンのレンダリング終了
		void Deactivate();

		/// バックバッファを取得
		IDirect3DSurface9* GetBackBuffer();

		/// 画面のリサイズ
		void Resize(int i_nScreenWidth, int i_nScreenHeight );	

		/// 変換行列の設定
		HRESULT SetTransform( TransformType i_nTransformType, D3DXMATRIX i_mMatrix );

		/// カメラ情報の取得
		const SCameraInfo& GetCameraInfo() const { return m_cameraInfo; }

		/// カメラ情報の設定
		HRESULT SetCameraInfo( const SCameraInfo& i_cameraInfo );		

	private:
		/// ダミーのウィンドウを作成する
		bool CreateDummyWindow();

		/// グラフィックデバイスを作成する
		bool CreateDevice( int i_nWidth, int i_nHeight );

		/// レンダーターゲットを作成する
		bool CreateRenderTarget( int i_nWidth, int i_nHeight );		
	};
} // end of namespace opk