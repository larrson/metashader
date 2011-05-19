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

		/**
			@class CFloatArrayParameter
			@brief ���������_�z��̃V�F�[�_�p�����[�^
		*/
		class CFloatArrayParameter : public CParameterBase
		{
		private:
			int m_nElementNum;	///< �z��̗v�f��
			float* m_pArray;	///< �z��f�[�^

			typedef void (CFloatArrayParameter::*TApplyFuncPtr)();
			TApplyFuncPtr m_pGetValueFunc; ///< �l���擾���邽�߂̃����o�֐��|�C���^
		public:
			/**
				@brief �R���X�g���N�^
				@param [in] i_nElementNum �z��̗v�f��
			*/
			CFloatArrayParameter(std::string i_strName, D3DXHANDLE i_nHandle, int i_nElementNum );

			/// �f�X�g���N�^
			virtual ~CFloatArrayParameter();

			/**
				@brief �p�����[�^���V�F�[�_�֓K�p����
				@retval �G���[�R�[�h
			*/
			virtual HRESULT Apply(CShader* i_pShader);

		private:
			/**
				@brief �l���擾���邽�߂̊֐��|�C���^��ݒ肷��
			*/
			void SetupGetValueFunc();

			/// ���s�����̕�����ݒ肷��
			void GetValue_Uniform_DirLightDir();

			/// ���s�����̐F��ݒ肷��
			void GetValue_Uniform_DirLightCol();
		};

		class CVector4Parameter : public CGeneralParameter<D3DXVECTOR4>
		{
		public:									
			typedef void (CVector4Parameter::*TGetValueFuncPtr)();
			TGetValueFuncPtr m_pGetValueFunc; ///< �l���擾�������o�ϐ��֐ݒ肷�邽�߂̃����o�֐��|�C���^

		public:
			/// �R���X�g���N�^
			CVector4Parameter(std::string i_strName, D3DXHANDLE i_nHandle );

			/// �f�X�g���N�^
			virtual ~CVector4Parameter();			

			/** 
				@brief �l���擾
				@note Apply���\�b�h�p
			*/
			virtual const D3DXVECTOR4* GetValue();		

		private:
			/**
				@brief �l���擾���邽�߂̃t�@���N�^��ݒ肷��
			*/
			void SetupGetValueFunc();

			/// �J�����ʒu���擾����
			void GetValue_Uniform_Camera_Position();
		};

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
				@brief �l���擾
				@note Apply���\�b�h�p
			*/
			virtual const D3DXMATRIX* GetValue()
			{
				return (this->*m_pGetValueFunc)();
			}

		private:
			/**
				@brief �l���擾���邽�߂̊֐��|�C���^��ݒ肷��
			*/
			void SetupGetValueFunc();

			/// ���g���ێ����Ă���l���擾����
			const D3DXMATRIX* GetThisValue(){ return &m_tValue; };
			
			/// ���[���h�s����擾����
			const D3DXMATRIX* GetValue_mWorld();

			/// �r���[�s����擾����
			const D3DXMATRIX* GetValue_mView();

			/// �ˉe�s����擾����
			const D3DXMATRIX* GetValue_mProjection();
		};

		/**
			@class CTextureParameter
			@brief �e�N�X�`���̃p�����[�^�N���X
			@note 2D,3D,Cube�����ʂɈ����A�����ňꕔ�������ꍇ��������
		*/
		class CTextureParameter : public CParameterBase
		{			
		private:				
			TextureType		m_nTextureType;		///< �e�N�X�`���̎��			

			std::string		m_strPath;			///< �e�N�X�`���̃t�@�C���p�X			
			
			IDirect3DBaseTexture9*	m_pd3dBaseTexture; ///< DirectX9�̃e�N�X�`�����N���X

			bool					m_bValid; ///< �L����

			SSamplerState			m_samplerState;	///< �T���v���[�X�e�[�g

		public:
			/**
				@brife �R���X�g���N�^				
				@param [in] i_strName		�p�����[�^��
				@param [in] i_nHandle		�p�����[�^�̃n���h��
				@param [in] i_nTextureType	�e�N�X�`���̎��				
			*/
			CTextureParameter( std::string i_strName, D3DXHANDLE i_nHandle, TextureType i_nTextureType );

			/**
				@�f�X�g���N�^
			*/
			~CTextureParameter();

			/**
				@brief �p�����[�^���V�F�[�_�֓K�p����
				@retval �G���[�R�[�h
			*/
			HRESULT Apply(CShader* i_pShader);

			/**
				@brief �e�N�X�`���p�X��ݒ肷��
				@note �ێ����Ă���p�X�ƈقȂ�ꍇ�̂݁A�p�X��ݒ肷��Ɠ����ɁA�e�N�X�`���̍č쐬���s��
			*/
			HRESULT SetPath( const char* i_pszPath );			

			/**
				@brief �T���v���[�X�e�[�g��ݒ肷��
			*/
			HRESULT SetSamplerState( const SSamplerState& i_samplerState );

			/**
				@brief ���[�h�\�ȃe�N�X�`�������肷��
				@note �Ώۃt�@�C�����e�N�X�`���Ƃ��ēǂݍ��݉\�ȉ摜�t�@�C���������łȂ��A��ނ̈�v�����肷��
				@param [in] i_pszPath ����Ώۂ̃e�N�X�`���̃p�X
				@param [in] i_nTexturetype �e�N�X�`���̎��
				@retval �ǂݍ��݉\��
			*/			
			static bool CanLoadTexture( const char* i_pszPath, TextureType i_nTextureType );

		private:
			/**
				@brief �j��
			*/
			HRESULT Destroy();

			/**
				@brief �j�����\�b�h�̃T�u���[�`��
				@note �č쐬���̏����Ƌ��ʉ����邽��
			*/
			HRESULT Destroy_Sub();

			/**
				@brief ���ݕێ����Ă���p�X�Ɋ�Â��ăe�N�X�`�����쐬����
			*/
			HRESULT CreateTexture();
		};
	}	// end of namespace shader
} // end of namespace opk
