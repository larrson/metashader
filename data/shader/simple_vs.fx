float4x4 mWorld;
float4x4 mView;
float4x4 mProjection;

struct VS_INPUT 
{
   float4 Position : POSITION0;
   float3 Normal   : NORMAL0;
   float2 Texcoord0: TEXCOORD0;   
};

struct VS_OUTPUT 
{
   float4 Position : POSITION0;   
   float2 Texcoord0: TEXCOORD0;
   float3 Normal0  : TEXCOORD1;
   float3 Position0: TEXCOORD2;
};

VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;
      
   float4 worldPosition = mul( Input.Position, mWorld );

   Output.Position	= mul(mul( worldPosition , mView ) , mProjection);
   Output.Texcoord0 = Input.Texcoord0;
   Output.Normal0	= normalize( mul( float4(Input.Normal, 0.0f), mWorld ).xyz );
   Output.Position0 = worldPosition.xyz / worldPosition.w;
   
   return( Output );
   
};