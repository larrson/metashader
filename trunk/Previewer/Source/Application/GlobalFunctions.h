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
	@brief �o�b�N�o�b�t�@���擾����
	@retval �T�[�t�F�C�X�̃|�C���^
*/
extern "C" DLLEXPORT void* GetBackBuffer();

/**
	@brief �����_�����O����	
*/
extern "C" DLLEXPORT void Render();

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
	@brief �V�F�[�_�̕��������_(�X�J���[)�̃��j�t�H�[���p�����[�^�ɒl��ݒ肷��
	@param [in] i_pszName �p�����[�^��
	@param [in] i_fValue �ݒ肷��l
*/
extern "C" DLLEXPORT void SetUniformFloat( const char* i_pszName, float i_fValue );

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

extern "C" DLLEXPORT void SetBlendMode( opk::CGraphicDevice::BlendMode i_nBlendMode );

/**
	@brief �V�F�[�_���f�t�H���g�֐؂�ւ���
*/
extern "C" DLLEXPORT void UseDefaultShader();

/**
	@brief �w�肵���o�b�t�@�։摜�t�@�C���̃T���l�C���f�[�^���擾����
	@param [in] i_pszPath �T���l�C�����쐬���錳�摜�̃t�@�C���p�X
	@param [in] i_nWidth �T���l�C���̕�
	@param [in] i_nHeight �T���l�C���̍���
	@param [in] o_pBuffer �T���l�C���f�[�^�̊i�[��o�b�t�@
*/
extern "C" DLLEXPORT void GetImagePixelData( const char* i_pszPath, int i_nWidth, int i_nHeight, uint8* o_pBuffer);