sampler TextureSampler : register(s0);
Texture2D <float4> myTex2D;

float3 colorMul = float3(1,1,1);
float3 colorAdd = float3(0,0,0);
 
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 tex;
    tex = myTex2D.Sample(TextureSampler, texCoord.xy) ;
   
    tex.r = tex.r * colorMul.r;
    tex.g = tex.g * colorMul.g;
    tex.b = tex.b * colorMul.b;

    
    return tex;
}


technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_3 PixelShaderFunction();  
    }
}