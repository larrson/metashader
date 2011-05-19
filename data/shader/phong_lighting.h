/**
	@file phong_lighting.h
	@brief Phongモデルに基づくライティング計算を定義したファイル
*/

/**
	@brief 拡散項を計算する
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
	@brief スペキュラ項を計算する
*/
float3 CalcSpecular( float3 specular, float3 power, float3 normal )
{
	return float3(0.0f, 0.0f, 0.0f);
}