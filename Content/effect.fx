#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.1;

Texture2D NoiseTexture;

SamplerState NoiseSampler
{
    Texture = <NoiseTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureUV : TEXCOORD0;   // vertex texture coords
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    // float4 Diffuse : COLOR0;      // vertex diffuse color (note that COLOR0 is clamped from 0..1)
    float2 TextureUV : TEXCOORD0;   // vertex texture coords
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.TextureUV = input.TextureUV;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // return tex2D(NoiseSampler, input.TextureUV) * AmbientColor * AmbientIntensity;
    // return 255 - tex2D(NoiseSampler, input.TextureUV);
    // return tex2D(NoiseSampler, input.TextureUV) * AmbientColor;
    // return tex2D(NoiseSampler, input.TextureUV) + AmbientColor;
    return AmbientColor * AmbientIntensity;
}

technique Ambient
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
