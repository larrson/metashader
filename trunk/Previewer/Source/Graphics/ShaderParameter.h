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
		typedef CGeneralParameter<D3DXVECTOR4>	CVector4Parameter;

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
				@brief 値を取得するための関数ポインタを設定する
			*/
			void SetupGetValueFunc();

			/** 
				@brief 値を取得
				@note Applyメソッド用
			*/
			virtual const D3DXMATRIX* GetValue()
			{
				return (this->*m_pGetValueFunc)();
			}

		private:
			/// 自身が保持している値を取得する
			const D3DXMATRIX* GetThisValue(){ return &m_tValue; };
			
			/// ワールド行列を取得する
			const D3DXMATRIX* GetValue_mWorld();

			/// ビュー行列を取得する
			const D3DXMATRIX* GetValue_mView();

			/// 射影行列を取得する
			const D3DXMATRIX* GetValue_mProjection();
		};
	}	// end of namespace shader
} // end of namespace opk
