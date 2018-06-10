float4x4 World;
float4x4 View;
float4x4 Projection;
 
float3 CameraPosition = float3(0,0,100);
 
Texture SkyBoxTexture; 
samplerCUBE SkyBoxSampler = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   MinFilter = linear;
   MagFilter = linear;
   MipFilter = linear;
   AddressU = Mirror; 
   AddressV = Mirror; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
 
    float4 VertexPosition = mul(input.Position, World);
    output.TexCoord = VertexPosition - float4(CameraPosition,1.0);
 
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return texCUBE(SkyBoxSampler, normalize(input.TexCoord));
}

technique Skybox
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}