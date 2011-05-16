// file information

// include headers

// uniform definitions


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
   return float4(0.0f, 0.0f, 0.0f, 1.0f);   
}