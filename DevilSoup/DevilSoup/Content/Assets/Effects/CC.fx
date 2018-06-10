sampler TextureSampler : register(s0);
Texture2D <float4> myTex2D;

float timer;
float amp=0.004;

float3 colorMul = float3(1,1,1);
float3 colorAdd = float3(0,0,0);
 
float4 PixelShaderFunction(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 tex;
    float2 coo = texCoord;
    coo.x += sin(coo.y*4*2*3.14159+ timer)*amp;
   // coo.y += cos(timer+coo.x*4*2*3.14159)*0.05;

  // coo.x= texCoord.y;
   //coo.y = texCoord.x;
    tex = myTex2D.Sample(TextureSampler, coo.xy) ;
   
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