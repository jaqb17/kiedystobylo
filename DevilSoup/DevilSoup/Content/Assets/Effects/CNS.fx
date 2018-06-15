#define SKINNED_EFFECT_MAX_BONES   72
float4x3 Bones[SKINNED_EFFECT_MAX_BONES];

float4x4 ViewProj;
float3x3 WorldIT;
float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CamPosition = float3(0,0,100);

float Shine = 5;

float4 AmbientColor = float4(1,1,1,1);
float AmbientIntensity = 0.1;
float3 LightDirection = float3(0,0,-1);

float3 addColor = float3(0,0,0);

float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 0.9;

float4 SpecularColor = float4(1,1,1,1);


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : NORMAL0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};

struct Skinned_VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal   : NORMAL0;
	float2 TexCoord : TEXCOORD0;
	uint4  Indices  : BLENDINDICES0;
	float4 Weights  : BLENDWEIGHT0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 View : TEXCOORD1;
    float3x3 WorldToTangentSpace : TEXCOORD2;
};

texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
    Texture = <ColorMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};
 
texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

texture2D SpecMap;
sampler2D SpecMapSampler = sampler_state
{
    Texture = <SpecMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

void SkinNormal(inout Skinned_VertexShaderInput vin, uniform int boneCount)
{
	float4x3 skinning = 0;

	[unroll]
	for (int i = 0; i < boneCount; i++)
	{
		skinning += Bones[vin.Indices[i]] * vin.Weights[i];
	}

	vin.Position.xyz = mul(vin.Position, skinning);
	vin.Normal = mul(vin.Normal, (float3x3)skinning);
	vin.Binormal = mul(vin.Binormal, (float3x3)skinning);
	vin.Tangent = mul(vin.Tangent, (float3x3)skinning);
}

//4 weights per vertex
VertexShaderOutput Skinned_VertexShaderFunction(Skinned_VertexShaderInput input)
{
	VertexShaderOutput output;
	
	SkinNormal(input, 4);

	float4 worldPosition = mul(input.Position, World);	
	output.Position = mul(worldPosition, ViewProj);
	output.WorldToTangentSpace[0] = mul(input.Tangent, WorldIT);
	output.WorldToTangentSpace[1] = mul(input.Binormal, WorldIT);
	output.WorldToTangentSpace[2] = mul(input.Normal, WorldIT);
	output.TexCoord = input.TexCoord;
	output.View = normalize(float4(CamPosition,1.0) - worldPosition);
 
	return output;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input, float3 Normal : NORMAL)
{
    VertexShaderOutput output;
 
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
   
    output.TexCoord = input.TexCoord;

    output.WorldToTangentSpace[0] = mul(normalize(input.Tangent), World);
    output.WorldToTangentSpace[1] = mul(normalize(input.Binormal), World);
    output.WorldToTangentSpace[2] = mul(normalize(input.Normal), World);
     

    //float3 normal = normalize(mul(Normal, World));
    //output.Normal = normal;

    output.View = normalize(float4(CamPosition,1.0) - worldPosition);
 
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{

    float4 color = tex2D(ColorMapSampler, input.TexCoord);

    
    float3 normalMap = 2.0 *(tex2D(NormalMapSampler, input.TexCoord)) - 1.0;
    normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));
    float4 normal = float4(normalMap,1.0);
 
    float4 SpecMapColor = tex2D(SpecMapSampler, input.TexCoord);

    float4 diffuse = saturate(dot(-LightDirection,normal));
    float4 reflect = normalize(2*diffuse*normal-float4(LightDirection,1.0));
    float4 specular = pow(saturate(dot(reflect,input.View)),Shine);
   

    specular = specular*SpecMapColor.x;

    float4 final =   color * AmbientColor * AmbientIntensity + 
            color * DiffuseIntensity * DiffuseColor * diffuse + 
            color * SpecularColor*specular;

    float4 outputColor = final;

   //Sepia
  //  outputColor.r = (final.r * 0.393) + (final.g * 0.769) + (final.b * 0.189);
  //  outputColor.g = (final.r * 0.349) + (final.g * 0.686) + (final.b * 0.168);    
  //  outputColor.b = (final.r * 0.272) + (final.g * 0.534) + (final.b * 0.131);

  //Negative
  // outputColor.r = 1.0-final.r;
  // outputColor.g = 1.0-final.g;
  // outputColor.b = 1.0-final.b;
 
    return outputColor + float4(addColor,0);

}

technique NotSkinned
{
    pass Pass1
    {
    AlphaBlendEnable = FALSE;
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}

technique Skinned
{
    pass Pass1
    {   
    AlphaBlendEnable = FALSE;
        VertexShader = compile vs_4_0 Skinned_VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}