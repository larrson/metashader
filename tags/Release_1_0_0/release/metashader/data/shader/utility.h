#ifndef _UTILITY_H_
#define _UTILITY_H_

/**
	@file utility.h
	@brief ���ʂŎg�p����֗��֐���}�N�����`
*/

/**
	@brief ���[�J���̖@���F�����[���h��ԏ�̖@���x�N�g���֕ϊ�����
*/
float3 ToWorldNormal( float3 localNormal, float3 normal, float3 tangent, float3 binormal )
{					
	float3x3 mToWorld = {
			 tangent, binormal, normal
	};	
	return normalize( mul( localNormal, mToWorld ) );
}

#endif // _UTILITY_H_