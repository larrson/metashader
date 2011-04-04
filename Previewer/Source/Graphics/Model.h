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
		std::wstring	m_strFilePath;	///< モデルのファイルパス
		ID3DXMesh*		m_pMesh;		///< メッシュ

	public:
		/** 
			@brief コンストラクタ						
			@attension i_strFilePathの有効性は呼び出し側が責任をもつこと
			@param[in] i_strFilePath Xファイルのパス
		*/
		CModel( std::wstring i_strFilePath );			

		/// デストラクタ
		~CModel();

		/// リストア
		HRESULT Restore();

		/// 破棄
		void Destroy();

		/// 描画
		HRESULT Render();
	};
} // end of namespace opk