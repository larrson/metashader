#pragma once
/**
	@file Shader.h
	@brief �V�F�[�_�N���X
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{	
	namespace shader
	{		
		class CParameterBase;	
		typedef shared_ptr<CParameterBase> TParameterPtr;
		class CShader;
		typedef shared_ptr<CShader> TShaderPtr;		

		/**
			@class CShader
			@brief �V�F�[�_�N���X
			@note ���Z�b�g�ɔ����ăV�F�[�_�R�[�h��ێ����Ă���
		*/
		class CShader
		{
		public:			

		private:
			bool		m_bValid;	///< �L����
			Profile		m_nProfile; ///< ���̃V�F�[�_�̃v���t�@�C��			
			uint8*		m_pBuffer;  ///< �V�F�[�_�R�[�h�ێ�
			uint32		m_nSize;	///< �V�F�[�_�R�[�h�̃T�C�Y�i�o�C�g�P�ʁj
			
			union D3DShader
			{
				IDirect3DVertexShader9* pVertex;
				IDirect3DPixelShader9* pPixel;
			};
			D3DShader	m_d3dShader;

			LPD3DXCONSTANTTABLE m_pD3DConstantTable; ///< �R���X�^���g�e�[�u��
			
			char m_szFileName[MAX_PATH]; ///< �t�@�C�����i�t�@�C������쐬�����ꍇ�̂��߁j

			/// �쐬���̎��
			enum CreateType
			{
				CreateType_FromBuffer, ///< �o�b�t�@����쐬
				CreateType_FromFile,   ///< �t�@�C������쐬
				CreateType_NotCreated, ///< ���쐬
			};			
			CreateType m_nCreateType; ///< �쐬���̎��

			typedef std::map<std::string, TParameterPtr > TParameterMap;
			TParameterMap m_parameterMap; ///< �p�����[�^�i�[�p�̃}�b�v

		public:
			/// �R���X�g���N�^
			CShader();

			/// �f�X�g���N�^
			~CShader();

			/**
				@brief �o�b�t�@���w�肵�č쐬
				@param [in] i_nProfile	�V�F�[�_�̃v���t�@�C��
				@param [in] i_pBuffer	�V�F�[�_�R�[�h�̃o�b�t�@
				@param [in] i_nSize		�o�b�t�@�̃T�C�Y
			*/
			HRESULT Create( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize );

			/**
				@brief �t�@�C������V�F�[�_���쐬
				@param [in] 
			*/
			HRESULT CreateFromFile( Profile i_nProfile, const char* i_pszFllePath );
			
			/**
				@brief �j��
			*/
			HRESULT Destroy();

			/**
				@brief ���X�g�A����
				@note �f�o�C�X���X�g�ւ̑Ή��p
			*/
			HRESULT Restore();

			/**
				@brief �V�F�[�_�J�n
			*/
			HRESULT Activate();

			/**
				@brief �V�F�[�_�I��
			*/
			HRESULT Deactivate();

			/**
				@brief �p�����[�^�̌���
			*/
			TParameterPtr FindParameter( const std::string& i_strName );	

			/**
				@brief �R���X�^���g�e�[�u���̎擾
			*/
			LPD3DXCONSTANTTABLE GetD3DConstantTable(){ return m_pD3DConstantTable; }

		private:
			/** 
				@brief Create���\�b�h�̃T�u���[�`��		
				@note Restore���̏����Ƃ̋��ʉ��̂���
			*/
			HRESULT Create_Sub();

			/**
				@brief CreateFromFile���\�b�h�̃T�u���[�`��
				@note Restore���̏����Ƃ̋��ʉ��̂���
			*/
			HRESULT CreateFromFile_Sub();

			/// �p�����[�^�̃Z�b�g�A�b�v
			HRESULT SetupParameters();

			/** 
				@brief Destroy���\�b�h�̃T�u���[�`��		
				@note Restore���̏����Ƃ̋��ʉ��̂���
			*/
			HRESULT Destroy_Sub();			
		};			

	} // end of namespace shader
} // end of namespace opk