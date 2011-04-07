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

			CShader*		   m_pShaders;	///< �V�F�[�_

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
				@brief �x�N�g���^�̃p�����[�^�֒l��ݒ肷��
				@note ���݂��Ȃ��p�����[�^�����w�肳�ꂽ�ꍇ�́A�����s��Ȃ�
			*/			
			void SetVector4Value(Profile i_nProfile, const std::string& i_strName, const D3DXVECTOR4& i_vValue );

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