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

/**
	@brief �t���l�������v�Z����
	@param [in] n ���ܗ�
	@param [in] k �����W��
	@param [in] v �r���[�����x�N�g��(���_�֌������x�N�g��)
	@param [in] l ���C�g�����x�N�g��(�����֌������x�N�g��)
	@retval �t���l����
*/
float CalcFresnel( float n, float k, float3 v, float3 l)
{
	// �n�[�t�x�N�g��
	float3 h = normalize(v + l);
	h = normalize( h );

	float cosTheta = saturate( dot( h, l ) );

	return ((n-1.0f)*(n-1.0f) + 4*n*pow(1.0f-cosTheta, 5) + k*k)
		/ ((n+1)*(n+1) + k*k);
}
#endif // _UTILITY_H_