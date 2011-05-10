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

		/**
			@brief αブレンディングの種類
		*/
		enum BlendMode
		{
			BlendMode_None,		///< 無し
			BlendMode_Normal,	///< 通常
			BlendMode_Add,		///< 加算
			BlendMode_Sub,		///< 減算
		};

		/**
			@struct SDirLightInfo
			@brief 並行光源情報の構造体
		*/
		struct SDirLightInfo
		{
			D3DXVECTOR3 vColor;		///< 色
			D3DXVECTOR3 vDir;		///< 方向
			bool		bEnable;	///< 有効フラグ
		};

		/**
			@brief ライトの最大数
		*/
		enum LightMax {
			DirLight_Max = 3, ///< 並行光源の最大数
		};

	private:
		IDirect3D9*			m_pd3d9;			///< DirectXオブジェクト	
		IDirect3DDevice9*	m_pd3dDevice9;		///< グラフィックデバイス
		IDirect3DSurface9*	m_pd3dSurface9;		///< サーフェース(ダブルバッファ)
		D3DPRESENT_PARAMETERS m_d3dpp;			///< プレゼンテーションパラメータ

		HWND m_hWnd;		///< ダミーのウィンドウハンドル

		bool m_bValid;		///< 有効か
		bool m_bActive;		///< 実行中か		

		int m_nWidth;  ///< バックバッファの幅
		int m_nHeight; ///< バックバッファの高さ		

		D3DVIEWPORT9	m_viewport; ///< ビューポート

		SCameraInfo		m_cameraInfo; ///< カメラ情報

		D3DXMATRIX		m_mTransform[TransformType_Max]; ///< 変換行列

		BlendMode		m_nBlendMode; ///< ブレンドモード

		SDirLightInfo	m_dirLightInfo[DirLight_Max];	///< 並行光源

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

		/// 変換行列の設定
		HRESULT SetTransform( TransformType i_nTransformType, D3DXMATRIX i_mMatrix );

		/// 変換行列の取得
		const D3DXMATRIX& GetTransform( TransformType i_nTransformType ) const { return m_mTransform[i_nTransformType]; }

		/// カメラ情報の取得
		const SCameraInfo& GetCameraInfo() const { return m_cameraInfo; }

		/// カメラ情報の設定
		HRESULT SetCameraInfo( const SCameraInfo& i_cameraInfo );	
		
		/** 
			@brief ブレンドモードの設定
			@param [in] i_nBlendMode ブレンドモード
			@param [in] i_bForced 強制設定を行うか（trueならば現在の状態に関係なく設定する）
		*/
		HRESULT SetBlendMode( BlendMode i_nBlendMode, bool i_bForced = false );

		/**
			@brief 並行光源の有効/無効切り替え
			@param [in] i_nIndex 並行光源のインデックス
			@param [in] i_bEnable 有効or無効
			@note i_nIndexの範囲は[0, DirLight_Max - 1]
		*/
		void SetDirLightEnable( int i_nIndex, bool i_bEnable );

		/**
			@brief 並行光源の色を取得する
			@param [in] i_nIndex 並行光源のインデックス
			@note i_nIndexの範囲は[0, DirLight_Max - 1]
		*/
		D3DXVECTOR3 GetDirLightColor( int i_nIndex );

		/**
			@brief 並行光源の色を設定する
			@param [in] i_nIndex 並行光源のインデックス
			@param [in] i_fR 赤成分
			@param [in] i_fG 緑成分
			@param [in] i_fB 青成分
			@note i_nIndexの範囲は[0, DirLight_Max - 1]
		*/
		void SetDirLightColor( int i_nIndex, float i_fR, float i_fG, float i_fB);

		/**
			@brief 並行光源の方向を取得する
			@param [in] i_nIndex 並行光源のインデックス
			@note i_nIndexの範囲は[0, DirLight_Max - 1]
		*/
		D3DXVECTOR3 GetDirLightDir( int i_nIndex );

		/**
			@brief 並行光源の方向を設定する
			@param [in] i_nIndex 並行光源のインデックス
			@param [in] i_fX 方向ベクトルのX成分
			@param [in] i_fY 方向ベクトルのY成分
			@param [in] i_fZ 方向ベクトルのZ成分
			@note i_nIndexの範囲は[0, DirLight_Max - 1]
		*/
		void SetDirLightDir( int i_nIndex, float i_fX, float i_fY, float i_fZ);

		/** 
			@brief バッファのクリア
			@attension 各パラメータの範囲は[0,1]
			@param [in] i_fR レッド
			@param [in] i_fG グリーン
			@param [in] i_fB ブルー
			@param [in] i_fA アルファ	
			@return エラーコード
		*/
		HRESULT Clear(float i_fR, float i_fG, float i_fB, float i_fA);

	private:
		/// ダミーのウィンドウを作成する
		bool CreateDummyWindow();

		/// グラフィックデバイスを作成する
		bool CreateDevice( int i_nWidth, int i_nHeight );

		/// レンダーターゲットを作成する
		bool CreateRenderTarget( int i_nWidth, int i_nHeight );		
	};
} // end of namespace opk