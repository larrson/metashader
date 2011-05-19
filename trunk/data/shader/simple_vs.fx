float4x4 mWorld;
float4x4 mView;
float4x4 mProjection;

struct VS_INPUT 
{
   float4 Position : POSITION0;
   float3 Normal   : NORMAL0;
   float3 Tangent  : TANGENT0;
   float3 BiNormal : BINORMAL0;
   float2 Texcoord0: TEXCOORD0;   
};

struct VS_OUTPUT 
{
   float4 Position : POSITION0;
   float3 Position0: TEXCOORD0;      
   float3 Normal0  : TEXCOORD1;   
   float2 Texcoord0: TEXCOORD2;   
   float3 Tangent0 : TEXCOORD3;
   float3 BiNormal0: TEXCOORD4;
};

VS_OUTPUT vs_main( VS_INPUT Input )
{
   VS_OUTPUT Output;
      
   float4 worldPosition = mul( Input.Position, mWorld );

   Output.Position	= mul(mul( worldPosition , mView ) , mProjection);
   Output.Texcoord0 = Input.Texcoord0;
   
   // ワールド空間上の各方向ベクトルを計算（ワールド行列にスケール成分が含まれていないことを前提）
   Output.Normal0	= normalize( mul( float4(Input.Normal, 0.0f), mWorld ).xyz );
   Output.Tangent0  = normalize( mul( float4(Input.Tangent, 0.0f), mWorld ).xyz );
   Output.BiNormal0 = normalize( mul( float4(Input.BiNormal, 0.0f), mWorld ).xyz );
   
   Output.Position0 = worldPosition.xyz / worldPosition.w;
   
   return( Output );
   
};