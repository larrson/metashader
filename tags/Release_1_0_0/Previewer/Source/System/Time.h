#pragma once
/**
	@file Time.h
	@brief 時間計測クラス	
*/

// Includes ----------------------------------------------------------------------------------

namespace opk
{	
	// Data Type Definitions ---------------------------------------------------------------------
	/**
		@class CTime
		@brief 時間計測クラス
	*/
	class CTime
	{
	private:
		LARGE_INTEGER m_nFrequency;			///< 周波数
		LARGE_INTEGER m_nStartedCounter;	///< 開始時刻を表すカウンタ
		LARGE_INTEGER m_nCurrentCounter;	///< 現在時刻を表すカウンタ	
		LARGE_INTEGER m_nPrevCounter;		///< 一つ前の更新時のカウンタ
		
	public:		
		/// コンストラクタ
		CTime();

		/// デストラクタ
		~CTime();

		/// 開始
		void Start();
		/// 更新
		void Update();

		/**
			@brife 経過時刻の取得(秒)
			@note 最後にStart()からUpdate()を最後に呼び出した時間までの経過時間
		*/
		inline double GetTotalTime()
		{
			return ((double)(m_nCurrentCounter.QuadPart - m_nStartedCounter.QuadPart)) / ((double)m_nFrequency.QuadPart);
		}

		/**
			@brief 一つ前の更新時からの経過時間(秒)
			@note 最後にUpdate()を呼び出した時刻と、その一つ前のUpdate()を呼び出した時刻の差
		*/
		inline double GetElapsedDeltaTime()
		{
			return ((double)(m_nCurrentCounter.QuadPart - m_nPrevCounter.QuadPart)) / ((double)m_nFrequency.QuadPart);
		}

	private:
		/// 初期化
		void Init();
	};
} // end of namespace opk