#ifndef _UTILITY_H_
#define _UTILITY_H_

/**
	@file utility.h
	@brief 共通で使用する便利関数やマクロを定義
*/

/**
	@brief ローカルの法線色をワールド空間上の法線ベクトルへ変換する
*/
float3 ToWorldNormal( float3 localNormal, float3 normal, float3 tangent, float3 binormal )
{					
	float3x3 mToWorld = {
			 tangent, binormal, normal
	};	
	return normalize( mul( localNormal, mToWorld ) );
}

/**
	@brief フレネル項を計算する
	@param [in] n 屈折率
	@param [in] k 消失係数
	@param [in] v ビュー方向ベクトル(視点へ向かうベクトル)
	@param [in] l ライト方向ベクトル(光源へ向かうベクトル)
	@retval フレネル項
*/
float CalcFresnel( float n, float k, float3 v, float3 l)
{
	// ハーフベクトル
	float3 h = normalize(v + l);
	h = normalize( h );

	float cosTheta = saturate( dot( h, l ) );

	return ((n-1.0f)*(n-1.0f) + 4*n*pow(1.0f-cosTheta, 5) + k*k)
		/ ((n+1)*(n+1) + k*k);
}
#endif // _UTILITY_H_