#pragma once
/**
	@file ShaderParameter.h
	@brief �V�F�[�_�p�����[�^�N���X
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	namespace shader
	{
		/**
			@class CParameterBase
			@brief �V�F�[�_�p�����[�^�̊��N���X
		*/
		class CParameterBase
		{
		protected:

			std::string m_strName; ///< �p�����[�^��
			D3DXHANDLE	m_nHandle; ///< �p�����[�^�̃n���h��			

		public:
			/**
				@brife �R���X�g���N�^
			*/
			CParameterBase(std::string i_strName, D3DXHANDLE i_nHandle);

			/**
				@brief �f�X�g���N�^
			*/
			virtual ~CParameterBase();

			/**
				@brief �p�����[�^���̎擾
			*/
			const std::string& GetName() const { return m_strName; }

			/**
				@brief �p�����[�^���V�F�[�_�֓K�p����
				@retval �G���[�R�[�h
			*/
			virtual HRESULT Apply(CShader* i_pShader) = 0;

		protected:
			/**
				@brief �l���V�F�[�_�֐ݒ肷��
				@param [in] i_pShader �ݒ��̃V�F�[�_
				@param [in] i_nHandle �ݒ��̃p�����[�^�̃n���h��
				@param [in] i_pValue  �ݒ肷��l
				@retval �G���[�R�[�h
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
			@brief �l�^�i�e�N�X�`���ȊO�j�������ėp�p�����[�^�N���X
		*/
		template <typename T>
		class CGeneralParameter : public CParameterBase
		{
		protected:
			T m_tValue; ///< �l			
			
		public:
			/// �R���X�g���N�^
			CGeneralParameter(std::string i_strName, D3DXHANDLE i_nHandle )
				: CParameterBase( i_strName, i_nHandle )				
			{

			}

			/// �f�X�g���N�^
			virtual ~CGeneralParameter(){};

			/// �l�̐ݒ�
			void SetValue(T i_tValue )
			{
				m_tValue = i_tValue;
			}

			/**
				@brief �p�����[�^���V�F�[�_�֓K�p����
				@retval �G���[�R�[�h
			*/
			virtual HRESULT Apply( CShader* i_pShader )
			{
				return CParameterBase::ApplyValue<T>( i_pShader, m_nHandle, GetValue() );
			}

		private:

			/** 
				@brief �l���擾
				@note Apply���\�b�h�p
			*/
			virtual const T* GetValue()
			{
				return &m_tValue;
			}
		};		

		/// �^�p�����[�^�ɍ��킹���^����typedef

		typedef CGeneralParameter<float>		CFloatParameter;
		typedef CGeneralParameter<D3DXVECTOR4>	CVector4Parameter;

		/**
			@class CMatrixParameter
			@brief �s��p�������[�^�N���X
		*/
		class CMatrixParameter : public CGeneralParameter<D3DXMATRIX>
		{
		private:
			typedef const D3DXMATRIX* (CMatrixParameter::*TGetValueFuncPtr)();
			TGetValueFuncPtr m_pGetValueFunc; ///< �l���擾���邽�߂̃����o�֐��|�C���^

		public: 		
			/// �R���X�g���N�^
			CMatrixParameter(std::string i_strName, D3DXHANDLE i_nHandle );

			/// �f�X�g���N�^
			virtual ~CMatrixParameter();

			/**
				@brief �l���擾���邽�߂̊֐��|�C���^��ݒ肷��
			*/
			void SetupGetValueFunc();

			/** 
				@brief �l���擾
				@note Apply���\�b�h�p
			*/
			virtual const D3DXMATRIX* GetValue()
			{
				return (this->*m_pGetValueFunc)();
			}

		private:
			/// ���g���ێ����Ă���l���擾����
			const D3DXMATRIX* GetThisValue(){ return &m_tValue; };
			
			/// ���[���h�s����擾����
			const D3DXMATRIX* GetValue_mWorld();

			/// �r���[�s����擾����
			const D3DXMATRIX* GetValue_mView();

			/// �ˉe�s����擾����
			const D3DXMATRIX* GetValue_mProjection();
		};
	}	// end of namespace shader
} // end of namespace opk
