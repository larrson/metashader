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

#endif // _UTILITY_H_