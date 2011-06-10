#pragma once
/**
	@file ShaderMan.h
	@brief �V�F�[�_�Ǘ��N���X
*/

// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{	
	namespace shader
	{
		/// �v���t�@�C��
		enum Profile
		{
			Profile_Vertex, ///< ���_�V�F�[�_
			Profile_Pixel, ///< �s�N�Z���V�F�[�_
			Profile_Max,
		};

		/**
		@brief �e�N�X�`���̎��
		*/
		enum TextureType
		{
			TextureType_2D,		///< 2D�e�N�X�`��				
			TextureType_3D,		///< 3D�e�N�X�`��
			TextureType_Cube,	///< �L���[�u�}�b�v
			TextureType_Max,
		};		

		/**
		@brife ���b�s���O���[�h
		@note http://www.t-pot.com/books/DirectX_f.htm���Q��
		*/
		enum WrapMode
		{
			WrapMode_Wrap = 0,	///< ���[�v
			WrapMode_Mirror,	///< ���]���[�v
			WrapMode_Clamp,		///< �ĊO�F�ŃN�����v
			WrapMode_Border,	///< �w�肵�����E�F���g�p
			WrapMode_Mirroronce,///< 1�񂾂��~���[�����O���A�O���͋��E�F
			WrapMode_Max,		///< �ő吔
		};

		/**
		@brief �t�B���^�����O���[�h
		*/
		enum FilterMode
		{				
			FilterMode_Point = 0,	///< �ŋߓ_�T���v�����O
			FilterMode_Linear,		///< ���`���
			FilterMode_Ansotropic,	///< �ٕ����T���v�����O
			FilterMode_Max,			///< �ő吔
		};

		/**
		@brief �T���v���[�X�e�[�g
		*/
		struct SSamplerState
		{
			WrapMode	m_nWrapU; ///< u���W�̃��b�s���O���[�h
			WrapMode	m_nWrapV; ///< v���W�̃��b�s���O���[�h
			WrapMode	m_nWrapW; ///< w���W�̃��b�s���O���[�h
			FilterMode	m_nMagFilter; ///< �g��t�B���^
			FilterMode	m_nMinFilter; ///< �k���t�B���^
			FilterMode	m_nMipFilter; ///< �~�b�v�}�b�v�t�B���^�[				
			uint32		m_nMaxAnisotoropy;	///< �ٕ����̍ő�l
			float		m_fBorderColorR;	///< ���E�F��R����
			float		m_fBorderColorG;	///< ���E�F��G����
			float		m_fBorderColorB;	///< ���E�F��B����
			float		m_fBorderColorA;	///< ���E�F��A����


			/**
			@brife �R���X�g���N�^
			@note �f�t�H���g�l�p
			*/
			SSamplerState()
				: m_nWrapU( WrapMode_Wrap )
				, m_nWrapV( WrapMode_Wrap )
				, m_nWrapW( WrapMode_Wrap )
				, m_nMagFilter( FilterMode_Linear )
				, m_nMinFilter( FilterMode_Linear )
				, m_nMipFilter( FilterMode_Linear )
				, m_nMaxAnisotoropy( 1 )
				, m_fBorderColorR ( 0.0f )
				, m_fBorderColorG ( 0.0f )
				, m_fBorderColorB ( 0.0f )
				, m_fBorderColorA ( 0.0f )
			{}
		};

		class CShader;

		/**
			@class CShaderMan
			@brief �V�F�[�_�Ǘ��N���X
			@note �V���O���g��
		*/
		class CShaderMan
		{
		private:
			static CShaderMan* s_pInstance; ///< �V���O���g���̃C���X�^���X			

			CShader*		   m_pCurrentShaders;	///< ���ݎg�p���̃V�F�[�_�̔z��
			CShader*		   m_pShaders;			///< �V�F�[�_
			CShader*		   m_pDefaultShaders;	///< ���^�V�F�[�_�ɂ���č쐬���ꂽ�V�F�[�_���g�p�ł��Ȃ��ꍇ�p

		public:
			/// �C���X�^���X�̎擾
			static CShaderMan* GetInstance(){ return s_pInstance; }

			/// �C���X�^���X�̐���
			static bool CreateInstance();

			/// �C���X�^���X�̔j��
			static void DisposeInstance();

			/**
				@brief ������
			*/
			void Initialize();

			/**
				@brief �V�F�[�_���J�n
			*/
			HRESULT Activate();

			/**
				@brief �V�F�[�_���I��
			*/
			void Deactivate();

			/**
				@brief �o�b�t�@���w�肵�ăV�F�[�_���쐬����
				@param [in] i_nProfile	�v���t�@�C��
				@param [in] i_pBuffer	�V�F�[�_�R�[�h���i�[����Ă���o�b�t�@
				@param [in] i_nSize		�o�b�t�@�̃T�C�Y
				@retval �G���[�R�[�h
			*/
			HRESULT CreateShaderFromBuffer( Profile i_nProfile, const char* i_pBuffer, uint32 i_nSize );

			/**
				@brief float�^�̃p�����[�^�֒l��ݒ肷��
				@note ���݂��Ȃ��p�����[�^�����w�肳�ꂽ�ꍇ�́A�����s��Ȃ�
			*/
			void SetFloatValue(Profile i_nProfile, const std::string& i_strname, float i_fValue );

			/**
				@brief �x�N�g���^�̃p�����[�^�֒l��ݒ肷��
				@note ���݂��Ȃ��p�����[�^�����w�肳�ꂽ�ꍇ�́A�����s��Ȃ�
			*/			
			void SetVector4Value(Profile i_nProfile, const std::string& i_strName, const D3DXVECTOR4& i_vValue );

			/**
				@brief �e�N�X�`���^�̃p�����[�^�փe�N�X�`���t�@�C���̃p�X��ݒ肷��
				@note ���݂��Ȃ��p�����[�^�����w�肳�ꂽ�ꍇ�́A�����s��Ȃ�
			*/
			void SetTexturePath(Profile i_nProfile, const std::string& i_strName, const char* i_pszPath );

			/**
				@brief �e�N�X�`���^�̃p�����[�^�փT���v���[�X�e�[�g��ݒ肷��
				@note ���݂��Ȃ��p�����[�^�����w�肳�ꂽ�ꍇ�́A�����s��Ȃ�
			*/
			void SetSamplerState(Profile i_nProfile, const std::string& i_strName, const SSamplerState& i_samplerState );

			/**
				@brief �f�t�H���g�V�F�[�_�𗘗p����
			*/
			void UseDefaultShader();

		private:
			/**
				@brief �R���X�g���N�^
				@note �V���O���g���̂��ߊO������B��
			*/
			CShaderMan();

			/**
				@brief �f�X�g���N�^
				@note �V���O���g���̂��ߊO������B��
			*/
			~CShaderMan();

			/**
				@brief �R�s�[�R���X�g���N�^�̋֎~
			*/
			CShaderMan( const CShaderMan& );

			/**
				@brief ������Z�q�̋֎~
			*/
			CShaderMan& operator=( const CShaderMan& );			

			/**
				@brief �j��
			*/
			void Destroy();			
		};
	} // end of namespace shader
} // end of namespace opk