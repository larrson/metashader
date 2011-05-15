#pragma  once
/**
	@file Model.h
	@brief ���f���N���X
*/
// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	class CModel
	{
	private:
		std::string	m_strFilePath;	///< ���f���̃t�@�C���p�X
		ID3DXMesh*		m_pMesh;		///< ���b�V��

	public:		 
		/// �R���X�g���N�^											
		CModel();			

		/// �f�X�g���N�^
		~CModel();

		/**
			@brief �t�@�C���p�X���烂�f�������[�h����
			@attension i_strFilePath�̗L�����͌Ăяo�������ӔC��������
			@param[in] i_strFilePath X�t�@�C���̃p�X
		*/
		HRESULT LoadFromFile( const std::string& i_strFilePath );

		/// ���X�g�A
		HRESULT Restore();

		/// �j��
		void Destroy();

		/// �`��
		HRESULT Render();
	};
} // end of namespace opk