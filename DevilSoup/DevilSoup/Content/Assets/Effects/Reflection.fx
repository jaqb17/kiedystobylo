float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 WorldInverseTranspose;
 
float3 addColor = float3(0,0,0);
float3 CameraPosition;
 
Texture SkyboxTexture; 
samplerCUBE SkyboxSampler = sampler_state 
{ 
   texture = <SkyboxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
  AddressU = Mirror; 
   AddressV = Mirror; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Reflection : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    float3 ViewDirection = CameraPosition - VertexPosition;
 
    float3 Normal = normalize(mul(input.Normal, WorldInverseTranspose));
    output.Reflection = refract(-normalize(ViewDirection), normalize(Normal),.1);
 
    return output;
}
 
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 coloreg =  texCUBE(SkyboxSampler, normalize(input.Reflection)) + float4(addColor,0);
    coloreg.a = .25;
	return coloreg;
}
 
technique Reflection
{
    pass Pass1
    {
     	AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}