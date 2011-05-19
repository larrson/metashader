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

// get custom color parameter
float3 GetCustomColor( PS_INPUT In )
{
#ifdef FUNC_CustomColor
	float3 ret;
%CustomColor%
	return ret;
#else
	return float3( 0.0f, 0.0f, 0.0f );
#endif 
}

float4 ps_main
(
 PS_INPUT In
) : COLOR0
{
	// body of pixel shader		
	float3	color	= GetCustomColor( In );
	float	opacity = GetOpacity( In );
	return float4( color, opacity);	
}