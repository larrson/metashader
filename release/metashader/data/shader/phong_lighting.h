#ifndef _PHONG_LIGHTING_H_
#define _PHONG_LIGHTING_H_

/**
	@file phong_lighting.h
	@brief Phong���f���Ɋ�Â����C�e�B���O�v�Z���`�����t�@�C��
*/

/**
	@brief �g�U�����v�Z����
*/
float3 CalcDiffuse( float3 diffuse, float3 normal)
{
	float3 ret =  float3(0.0f, 0.0f, 0.0f);
	for(int i = 0; i < DIR_LIGHT_NUM; ++i )
	{
		float k = saturate( dot( normal, Uniform_DirLightDir[i]));
		ret += k * diffuse * Uniform_DirLightCol[i];
	}
	return ret;
}

/**
	@brief �X�y�L���������v�Z����
*/
float3 CalcSpecular( float3 specular, float power, float3 reflection )
{
	float3 ret = float3( 0.0f, 0.0f, 0.0f );
	for(int i = 0; i < DIR_LIGHT_NUM; ++i )
	{		
		// �X�y�L�����̋������v�Z
		float k = saturate( dot( reflection, Uniform_DirLightDir[i] ) );
		k = pow( k, power );
		ret += k * specular * Uniform_DirLightCol[i];
	}
	return ret;
}

#endif // _PHONG_LIGHTING_H_