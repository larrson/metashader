#pragma  once
/**
	@file Model.h
	@brief モデルクラス
*/
// Includes ----------------------------------------------------------------------------------

// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	class CModel
	{
	private:
		std::string	m_strFilePath;	///< モデルのファイルパス
		ID3DXMesh*		m_pMesh;		///< メッシュ

	public:		 
		/// コンストラクタ											
		CModel();			

		/// デストラクタ
		~CModel();

		/**
			@brief ファイルパスからモデルをロードする
			@attension i_strFilePathの有効性は呼び出し側が責任をもつこと
			@param[in] i_strFilePath Xファイルのパス
		*/
		HRESULT LoadFromFile( const std::string& i_strFilePath );

		/// リストア
		HRESULT Restore();

		/// 破棄
		void Destroy();

		/// 描画
		HRESULT Render();
	};
} // end of namespace opk