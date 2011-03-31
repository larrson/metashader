#pragma once

/**
	@file GlobalFunctions.h
	@brief Dll������J����֐��̐錾���L�q
*/

#define DLLEXPORT __declspec(dllexport)

/**
	@brief �v���r���[�A�̃G���g���|�C���g	
	@param [in] i_nScreenWidth	�X�N���[���̕�
	@param [in] i_nScreenHeight �X�N���[���̍���
	@retval �������ɐ���������
*/
extern "C" DLLEXPORT int PreviewerMain(int i_nScreenWidth, int i_nScreenHeight);

/**
	@brief ���݂̃t���[���������_�����O
*/
extern "C" DLLEXPORT void RenderFrame();

/**
	@brief �I������
	@retval �I�������ɐ���������
*/
extern "C" DLLEXPORT int ShutDown();

/**
	@brief �E�B���h�E���b�Z�[�W����
	@param [in] i_hWnd	�E�C���h�E�n���h��
	@param [in]	i_nMsg	���b�Z�[�W
	@param [in] i_wParam ���b�Z�[�W�p�����[�^
	@param [in] i_lParam ���b�Z�[�W�p�����[�^
*/
extern "C" DLLEXPORT void WndProc(int *i_hWnd, int i_nMsg, int i_wParam, int i_lParam);

/**
	@brief ���̃����_�����O���ꂽ�t���[���̃T�[�t�F�C�X���擾����
	@retval �T�[�t�F�C�X�̃|�C���^
*/
extern "C" DLLEXPORT void* GetNextSurface();

/**
	@brief �O���t�B�b�N�f�o�C�X�̗L�������m�F���A���X�g���Ă����炻�̑Ή����s��
*/
// extern "C" DLLEXPORT void CheckGraphicDevice();

/**
	@brief ��ʂ̃��T�C�Y����������
	@param [in] i_nScreenWidth	�X�N���[���̕�
	@param [in] i_nScreenHeight �X�N���[���̍���
*/
extern "C" DLLEXPORT void Resize(int i_nScreenWidth, int i_nScreenHeight );