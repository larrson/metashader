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
float3 CalcSpecular( float3 specular, float power, float3 position, float3 normal )
{
	float3 ret = float3( 0.0f, 0.0f, 0.0f );
	for(int i = 0; i < DIR_LIGHT_NUM; ++i )
	{
		// ��������
		float3 eyeDir = normalize( Uniform_CameraPosition - position );
		// �n�[�t�x�N�g��
		float3 half = normalize( eyeDir + Uniform_DirLightDir[i] );
		// �X�y�L�����̋������v�Z
		float k = saturate( dot( normal, half ) );
		k = pow( k, power );
		ret += k * specular * Uniform_DirLightCol[i];
	}
	return ret;
}