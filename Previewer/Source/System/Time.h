#pragma once
/**
	@file Time.h
	@brief ���Ԍv���N���X	
*/

// Includes ----------------------------------------------------------------------------------

namespace opk
{	
	// Data Type Definitions ---------------------------------------------------------------------
	/**
		@class CTime
		@brief ���Ԍv���N���X
	*/
	class CTime
	{
	private:
		LARGE_INTEGER m_nFrequency;			///< ���g��
		LARGE_INTEGER m_nStartedCounter;	///< �J�n������\���J�E���^
		LARGE_INTEGER m_nCurrentCounter;	///< ���ݎ�����\���J�E���^	
		LARGE_INTEGER m_nPrevCounter;		///< ��O�̍X�V���̃J�E���^
		
	public:		
		/// �R���X�g���N�^
		CTime();

		/// �f�X�g���N�^
		~CTime();

		/// �J�n
		void Start();
		/// �X�V
		void Update();

		/**
			@brife �o�ߎ����̎擾(�b)
			@note �Ō��Start()����Update()���Ō�ɌĂяo�������Ԃ܂ł̌o�ߎ���
		*/
		inline double GetTotalTime()
		{
			return ((double)(m_nCurrentCounter.QuadPart - m_nStartedCounter.QuadPart)) / ((double)m_nFrequency.QuadPart);
		}

		/**
			@brief ��O�̍X�V������̌o�ߎ���(�b)
			@note �Ō��Update()���Ăяo���������ƁA���̈�O��Update()���Ăяo���������̍�
		*/
		inline double GetElapsedDeltaTime()
		{
			return ((double)(m_nCurrentCounter.QuadPart - m_nPrevCounter.QuadPart)) / ((double)m_nFrequency.QuadPart);
		}

	private:
		/// ������
		void Init();
	};
} // end of namespace opk