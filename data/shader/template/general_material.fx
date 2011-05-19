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
%PS_INPUT%
};

// include headers
%INCLUDES%

// get diffuse parameter
float3 GetDiffuse( PS_INPUT In )
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
float3 GetSpecular( PS_INPUT In )
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
float GetSpecularPower( PS_INPUT In )
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
float GetOpacity( PS_INPUT In )
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
float3 GetNormal( PS_INPUT In )
{
#ifdef FUNC_Normal
	float3 ret;
%Normal%
	return ret;
#else
	// return default value
	return In.Normal0;
#endif
}

float4 ps_main
(
 PS_INPUT In
) : COLOR0
{
	// body of pixel shader	
	// get parameters for lighting
	float3	diffuse			= GetDiffuse(In);
	float3	specular		= GetSpecular(In);
	float	specularPower	= GetSpecularPower(In);
	float	opacity			= GetOpacity(In);
	float3	normal			= GetNormal(In);
	
	// calc lighting
	float3 color = float3(0.0f, 0.0f, 0.0f);
	color += CalcDiffuse( diffuse, normal );
	color += CalcSpecular( specular, specularPower, In.Position0, normal );
	
	// return
	return float4( color.xyz, opacity );						
}