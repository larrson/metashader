float4x4 mWorld;
float4x4 mView;
float4x4 mProjection;

struct VS_INPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord0: TEXCOORD0;
};

struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float2 Texcoord0: TEXCOORD0;
};

VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;

   Output.Position	= mul(mul(mul( Input.Position , mWorld) , mView) , mProjection);
   Output.Texcoord0 = Input.Texcoord0;
   
   return( Output );
   
};