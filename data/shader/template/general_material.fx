// file information
%HEADER%

// macro definitions
%MACROS%

// uniform definitions
%UNIFORMS%

// pixel shader inputs
struct PS_INPUT
{
	float4 Position : POSITION0;	
	float3 Position0: TEXCOORD0;      
	float3 Normal0  : TEXCOORD1;   
	float2 Texcoord0: TEXCOORD2;   
	float3 Tangent0 : TEXCOORD3;
	float3 BiNormal0: TEXCOORD4;
};

// parameters used by GetXXX functions.
struct PARAMETERS
{
	float3 Position0;
	float2 Texcoord0;
	float3 Normal0;
	float3 Tangent0;
	float3 BiNormal0; 
	float3 Reflection0;
};

// include headers
#include "utility.h"
%INCLUDES%

// get diffuse parameter
float3 GetDiffuse( PARAMETERS In )
{
#ifdef FUNC_Diffuse
	float3 ret;
%Diffuse%
	return ret;
#else
	// return default value
	return float3( 0.0f, 0.0f, 0.0f );
#endif 
}

// get specular color
float3 GetSpecular( PARAMETERS In )
{
#ifdef FUNC_Specular
	float3 ret;
%Specular%
	return ret;
#else
	// return default value
	return float3( 1.0f, 1.0f, 1.0f );
#endif 
}

// get specular power
float GetSpecularPower( PARAMETERS In )
{
#ifdef FUNC_SpecularPower
	float ret;
%SpecularPower%
	return ret;
#else
	// return default value
	return 30.0f;
#endif 
}

// get opacity parameter
float GetOpacity( PARAMETERS In )
{
#ifdef FUNC_Opacity
	float ret;
%Opacity%
	return ret;
#else
	// return default value
	return 1.0f;
#endif
}

// get normal parameter
float3 GetNormal( PARAMETERS In )
{
#ifdef FUNC_Normal
	float3 ret;
%Normal%
	return ToWorldNormal( ret, In.Normal0, In.Tangent0, In.BiNormal0 );
#else
	// return default value	
	return In.Normal0;	
#endif
}

// get custom color parameter
float3 GetCustomColor( PARAMETERS In )
{
#ifdef FUNC_CustomColor
	float3 ret;
%CustomColor%
	return ret;
#else
	return float3( 0.0f, 0.0f, 0.0f );
#endif 
}

// initialize parameters
PARAMETERS InitializeParams( PS_INPUT In )
{
	PARAMETERS Params;
	
	Params.Position0 = In.Position0;
	Params.Texcoord0 = In.Texcoord0;
	Params.Normal0   = normalize( In.Normal0 );
	Params.Tangent0  = normalize( In.Tangent0 );
	Params.BiNormal0 = normalize( In.BiNormal0 );
	
	// normal vector is calculated by user defined graph. 
	Params.Normal0 = GetNormal( Params );
	
	// calculate refrection vector
#ifdef UNIFORM_CameraPosition
	Params.Reflection0 = normalize( reflect( Params.Position0 - Uniform_CameraPosition, Params.Normal0 ) );
#endif

	return Params;
}

float4 ps_main
(
 PS_INPUT In
) : COLOR0
{
	// initialize parameters
	PARAMETERS Params = InitializeParams( In );
	
#if defined (MATERIALTYPE_Phong)

	// get parameters for lighting
	float3	diffuse			= GetDiffuse( Params );
	float3	specular		= GetSpecular( Params );
	float	specularPower	= GetSpecularPower( Params );
	float	opacity			= GetOpacity( Params );	
	
	// calc lighting
	float3 color = float3(0.0f, 0.0f, 0.0f);
	color += CalcDiffuse( diffuse, Params.Normal0 );
	color += CalcSpecular( specular, specularPower, Params.Reflection0 );
	
#elif defined (MATERIALTYPE_Custom)
	
	// body of pixel shader		
	float3	color	= GetCustomColor( Params );
	float	opacity = GetOpacity( Params );
	
#else
	#error undefined material type
#endif 
	
	// return
	return float4( color.xyz, opacity );						
}