#pragma once
/**
	@file PreviewerLogic.h
	@brief プレビューアのアプリケーションロジックを表すクラス
	@note 生成された後のインスタンスはCAppが管理
*/
// Includes ----------------------------------------------------------------------------------


// Data Type Definitions ---------------------------------------------------------------------
namespace opk
{
	/**
		@class CPreviwerLogic
		@brief プレビューア用のロジッククラス
	*/
	class CPreviewerLogic : public CLogicBase
	{
	private:

	public:
		/// コンストラクタ
		CPreviewerLogic();

		/// デストラクタ
		virtual ~CPreviewerLogic();		
				
		/// 初期化
		virtual void Initialize();		

		/// 描画
		virtual void Render();
	};
} // end of namespace opk