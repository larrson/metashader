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
	@retval ���b�Z�[�W�ɑΉ�����߂�l
*/
extern "C" DLLEXPORT LRESULT WndProc(int *i_hWnd, int i_nMsg, int* i_wParam, int* i_lParam);

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

/**
	@brief �o�b�t�@���w�肵�ăs�N�Z���V�F�[�_���쐬����
	@param [in] i_pBuffer	�V�F�[�_�R�[�h���i�[����Ă���o�b�t�@
	@param [in] i_nSize		�o�b�t�@�̃T�C�Y
*/
extern "C" DLLEXPORT void CreatePixelShaderFromBuffer( const char* i_pBuffer, uint32 i_nSize );

/**
	@brief �V�F�[�_��4�����x�N�g���̃��j�t�H�[���p�����[�^�ɒl��ݒ肷��
	@param [in] i_pszName �p�����[�^��
	@param [in] x 4D�x�N�g����x
	@param [in] y 4D�x�N�g����y
	@param [in] z 4D�x�N�g����z
	@param [in] w 4D�x�N�g����w
*/
extern "C" DLLEXPORT void SetUniformVector4( const char* i_pszName, float x, float y, float z, float w );

/**
	@brief �V�F�[�_�̃e�N�X�`���p�����[�^�Ƀt�@�C���p�X��ݒ肷��
	@param [in] i_pszName �p�����[�^��
	@param [in] i_pszPath �t�@�C���p�X
*/
extern "C" DLLEXPORT void SetTexturePath( const char* i_pszName, const char* i_pszPath );

/**
	@brief �V�F�[�_�̃e�N�X�`���p�����[�^�ɃT���v���[�X�e�[�g��ݒ肷��
	@param [in] i_pszName �p�����[�^��
	@param [in] i_samplerState �T���v���X�e�[�g
*/
extern "C" DLLEXPORT void SetSamplerState( const char* i_pszName, const opk::shader::SSamplerState i_samplerState );