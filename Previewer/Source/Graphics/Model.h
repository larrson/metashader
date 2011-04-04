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
		std::wstring	m_strFilePath;	///< ���f���̃t�@�C���p�X
		ID3DXMesh*		m_pMesh;		///< ���b�V��

	public:
		/** 
			@brief �R���X�g���N�^						
			@attension i_strFilePath�̗L�����͌Ăяo�������ӔC��������
			@param[in] i_strFilePath X�t�@�C���̃p�X
		*/
		CModel( std::wstring i_strFilePath );			

		/// �f�X�g���N�^
		~CModel();

		/// ���X�g�A
		HRESULT Restore();

		/// �j��
		void Destroy();

		/// �`��
		HRESULT Render();
	};
} // end of namespace opk