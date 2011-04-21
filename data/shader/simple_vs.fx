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
   float3 Normal   : NORMAL0;
   float2 Texcoord0: TEXCOORD0;
};

VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;
   
   float4x4 mWorldView = mul( mWorld, mView );

   Output.Position	= mul(mul( Input.Position , mWorldView ) , mProjection);
   Output.Texcoord0 = Input.Texcoord0;
   Output.Normal	= normalize( mul( float4(Input.Normal, 0.0f), mWorldView ).xyz );
   
   return( Output );
   
};