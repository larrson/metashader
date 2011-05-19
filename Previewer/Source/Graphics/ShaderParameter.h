#pragma once
/**
	@file ShaderParameter.h
	@brief シェーダパラメータクラス
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	namespace shader
	{
		/**
			@class CParameterBase
			@brief シェーダパラメータの基底クラス
		*/
		class CParameterBase
		{
		protected:

			std::string m_strName; ///< パラメータ名
			D3DXHANDLE	m_nHandle; ///< パラメータのハンドル			

		public:
			/**
				@brife コンストラクタ
			*/
			CParameterBase(std::string i_strName, D3DXHANDLE i_nHandle);

			/**
				@brief デストラクタ
			*/
			virtual ~CParameterBase();

			/**
				@brief パラメータ名の取得
			*/
			const std::string& GetName() const { return m_strName; }

			/**
				@brief パラメータをシェーダへ適用する
				@retval エラーコード
			*/
			virtual HRESULT Apply(CShader* i_pShader) = 0;

		protected:
			/**
				@brief 値をシェーダへ設定する
				@param [in] i_pShader 設定先のシェーダ
				@param [in] i_nHandle 設定先のパラメータのハンドル
				@param [in] i_pValue  設定する値
				@retval エラーコード
			*/
			template <typename T>
			static HRESULT ApplyValue(CShader* i_pShader, D3DXHANDLE i_nHandle, const T* i_pValue)
			{
				HRESULT hr;
				CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();
				IDirect3DDevice9* pD3DDevice = pDevice->GetD3DDevice();
				V_RETURN( i_pShader->GetD3DConstantTable()->SetValue( pD3DDevice, i_nHandle, (void)i_pValue, sizeof(T)) );

				return S_OK;
			}

			template <>
			static HRESULT ApplyValue<float>(CShader* i_pShader, D3DXHANDLE i_nHandle, const float* i_pValue)
			{
				HRESULT hr;
				CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();
				IDirect3DDevice9* pD3DDevice = pDevice->GetD3DDevice();
				V_RETURN( i_pShader->GetD3DConstantTable()->SetFloat( pD3DDevice, i_nHandle, *i_pValue) );

				return S_OK;
			}			

			template <>
			static HRESULT ApplyValue<D3DXVECTOR4>(CShader* i_pShader, D3DXHANDLE i_nHandle, const D3DXVECTOR4* i_pValue)
			{
				HRESULT hr;
				CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();
				IDirect3DDevice9* pD3DDevice = pDevice->GetD3DDevice();
				V_RETURN( i_pShader->GetD3DConstantTable()->SetVector( pD3DDevice, i_nHandle, i_pValue) );

				return S_OK;
			}

			template<>
			static HRESULT ApplyValue<D3DXMATRIX>(CShader* i_pShader, D3DXHANDLE i_nHandle, const D3DXMATRIX* i_pValue)
			{
				HRESULT hr;
				CGraphicDevice* pDevice = CApp::GetInstance()->GetGraphicDevice();
				IDirect3DDevice9* pD3DDevice = pDevice->GetD3DDevice();
				V_RETURN( i_pShader->GetD3DConstantTable()->SetMatrix( pD3DDevice, i_nHandle, i_pValue) );

				return S_OK;
			}
		};	

		/**
			@class CGeneralParameter
			@brief 値型（テクスチャ以外）を扱う汎用パラメータクラス
		*/
		template <typename T>
		class CGeneralParameter : public CParameterBase
		{
		protected:
			T m_tValue; ///< 値			
			
		public:
			/// コンストラクタ
			CGeneralParameter(std::string i_strName, D3DXHANDLE i_nHandle )
				: CParameterBase( i_strName, i_nHandle )				
			{

			}

			/// デストラクタ
			virtual ~CGeneralParameter(){};

			/// 値の設定
			void SetValue(T i_tValue )
			{
				m_tValue = i_tValue;
			}

			/**
				@brief パラメータをシェーダへ適用する
				@retval エラーコード
			*/
			virtual HRESULT Apply( CShader* i_pShader )
			{
				return CParameterBase::ApplyValue<T>( i_pShader, m_nHandle, GetValue() );
			}

		private:

			/** 
				@brief 値を取得
				@note Applyメソッド用
			*/
			virtual const T* GetValue()
			{
				return &m_tValue;
			}
		};		

		/// 型パラメータに合わせた型名をtypedef

		typedef CGeneralParameter<float>		CFloatParameter;

		/**
			@class CFloatArrayParameter
			@brief 浮動小数点配列のシェーダパラメータ
		*/
		class CFloatArrayParameter : public CParameterBase
		{
		private:
			int m_nElementNum;	///< 配列の要素数
			float* m_pArray;	///< 配列データ

			typedef void (CFloatArrayParameter::*TApplyFuncPtr)();
			TApplyFuncPtr m_pGetValueFunc; ///< 値を取得するためのメンバ関数ポインタ
		public:
			/**
				@brief コンストラクタ
				@param [in] i_nElementNum 配列の要素数
			*/
			CFloatArrayParameter(std::string i_strName, D3DXHANDLE i_nHandle, int i_nElementNum );

			/// デストラクタ
			virtual ~CFloatArrayParameter();

			/**
				@brief パラメータをシェーダへ適用する
				@retval エラーコード
			*/
			virtual HRESULT Apply(CShader* i_pShader);

		private:
			/**
				@brief 値を取得するための関数ポインタを設定する
			*/
			void SetupGetValueFunc();

			/// 並行光源の方向を設定する
			void GetValue_Uniform_DirLightDir();

			/// 並行光源の色を設定する
			void GetValue_Uniform_DirLightCol();
		};

		class CVector4Parameter : public CGeneralParameter<D3DXVECTOR4>
		{
		public:									
			typedef void (CVector4Parameter::*TGetValueFuncPtr)();
			TGetValueFuncPtr m_pGetValueFunc; ///< 値を取得しメンバ変数へ設定するためのメンバ関数ポインタ

		public:
			/// コンストラクタ
			CVector4Parameter(std::string i_strName, D3DXHANDLE i_nHandle );

			/// デストラクタ
			virtual ~CVector4Parameter();			

			/** 
				@brief 値を取得
				@note Applyメソッド用
			*/
			virtual const D3DXVECTOR4* GetValue();		

		private:
			/**
				@brief 値を取得するためのファンクタを設定する
			*/
			void SetupGetValueFunc();

			/// カメラ位置を取得する
			void GetValue_Uniform_Camera_Position();
		};

		/**
			@class CMatrixParameter
			@brief 行列パラメメータクラス
		*/
		class CMatrixParameter : public CGeneralParameter<D3DXMATRIX>
		{
		private:
			typedef const D3DXMATRIX* (CMatrixParameter::*TGetValueFuncPtr)();
			TGetValueFuncPtr m_pGetValueFunc; ///< 値を取得するためのメンバ関数ポインタ

		public: 		
			/// コンストラクタ
			CMatrixParameter(std::string i_strName, D3DXHANDLE i_nHandle );

			/// デストラクタ
			virtual ~CMatrixParameter();			

			/** 
				@brief 値を取得
				@note Applyメソッド用
			*/
			virtual const D3DXMATRIX* GetValue()
			{
				return (this->*m_pGetValueFunc)();
			}

		private:
			/**
				@brief 値を取得するための関数ポインタを設定する
			*/
			void SetupGetValueFunc();

			/// 自身が保持している値を取得する
			const D3DXMATRIX* GetThisValue(){ return &m_tValue; };
			
			/// ワールド行列を取得する
			const D3DXMATRIX* GetValue_mWorld();

			/// ビュー行列を取得する
			const D3DXMATRIX* GetValue_mView();

			/// 射影行列を取得する
			const D3DXMATRIX* GetValue_mProjection();
		};

		/**
			@class CTextureParameter
			@brief テクスチャのパラメータクラス
			@note 2D,3D,Cubeを共通に扱い、内部で一部処理を場合分けする
		*/
		class CTextureParameter : public CParameterBase
		{			
		private:				
			TextureType		m_nTextureType;		///< テクスチャの種類			

			std::string		m_strPath;			///< テクスチャのファイルパス			
			
			IDirect3DBaseTexture9*	m_pd3dBaseTexture; ///< DirectX9のテクスチャ基底クラス

			bool					m_bValid; ///< 有効か

			SSamplerState			m_samplerState;	///< サンプラーステート

		public:
			/**
				@brife コンストラクタ				
				@param [in] i_strName		パラメータ名
				@param [in] i_nHandle		パラメータのハンドル
				@param [in] i_nTextureType	テクスチャの種類				
			*/
			CTextureParameter( std::string i_strName, D3DXHANDLE i_nHandle, TextureType i_nTextureType );

			/**
				@デストラクタ
			*/
			~CTextureParameter();

			/**
				@brief パラメータをシェーダへ適用する
				@retval エラーコード
			*/
			HRESULT Apply(CShader* i_pShader);

			/**
				@brief テクスチャパスを設定する
				@note 保持しているパスと異なる場合のみ、パスを設定すると同時に、テクスチャの再作成を行う
			*/
			HRESULT SetPath( const char* i_pszPath );			

			/**
				@brief サンプラーステートを設定する
			*/
			HRESULT SetSamplerState( const SSamplerState& i_samplerState );

			/**
				@brief ロード可能なテクスチャか判定する
				@note 対象ファイルがテクスチャとして読み込み可能な画像ファイルかだけでなく、種類の一致も判定する
				@param [in] i_pszPath 判定対象のテクスチャのパス
				@param [in] i_nTexturetype テクスチャの種類
				@retval 読み込み可能か
			*/			
			static bool CanLoadTexture( const char* i_pszPath, TextureType i_nTextureType );

		private:
			/**
				@brief 破棄
			*/
			HRESULT Destroy();

			/**
				@brief 破棄メソッドのサブルーチン
				@note 再作成時の処理と共通化するため
			*/
			HRESULT Destroy_Sub();

			/**
				@brief 現在保持しているパスに基づいてテクスチャを作成する
			*/
			HRESULT CreateTexture();
		};
	}	// end of namespace shader
} // end of namespace opk
