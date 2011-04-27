// stdafx.h : �W���̃V�X�e�� �C���N���[�h �t�@�C���̃C���N���[�h �t�@�C���A�܂���
// �Q�Ɖ񐔂������A�����܂�ύX����Ȃ��A�v���W�F�N�g��p�̃C���N���[�h �t�@�C��
// ���L�q���܂��B
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Windows �w�b�_�[����g�p����Ă��Ȃ����������O���܂��B
// Windows �w�b�_�[ �t�@�C��:
#include <windows.h>

// TODO: �v���O�����ɕK�v�Ȓǉ��w�b�_�[�������ŎQ�Ƃ��Ă��������B

/// C �����^�C�� ///
#include <stdlib.h>
#include <cmath>
#include <string>
#include <map>

/// �O�����C�u���� ///
#include <boost\config.hpp>
#include <boost\shared_ptr.hpp>
#include <boost\utility.hpp>
using boost::shared_ptr;

/// ���색�C�u���� ///
#include <types.h>
// ���[�e�B���e�B
#include <Utility/Utility.h>
// �O���t�B�b�N�X
#include <Graphics/graphics.h>
#include <Graphics/ShaderMan.h>
// IO
#include <IO/Mouse.h>
// �V�X�e��
#include <System/App.h>