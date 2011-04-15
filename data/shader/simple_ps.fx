// file information


// include headers


// uniform definitions
uniform float4    Uniform_Vector4_1;
uniform float4    Uniform_Vector4_0;


// pixel shader inputs
struct PS_INPUT
{
   float4 Position : POSITION0;

};

float4 ps_main
(
 PS_INPUT In
) : COLOR0
{
   // body of pixel shader   
float4 Operator_Add_0 = Uniform_Vector4_0 + Uniform_Vector4_1;
   return Operator_Add_0;

}