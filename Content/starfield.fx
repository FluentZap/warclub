#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureUV : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureUV : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
	output.TextureUV = input.TextureUV;
    output.Color = input.Color;

	return output;
}

float2x2 Rot(float a)
{
    float s=sin(a), c=cos(a);
    return float2x2(c, -s, s, c);
}

float Star(float2 uv, float flare)
{
    float d = length(uv);
    float m = .05 / d;
    
    float rays = max(0, 1.-abs(uv.x*uv.y*1000));
    m += rays*flare;
    
    uv = mul(uv,Rot(3.1415 / 4));
    rays = max(0, 1.-abs(uv.x*uv.y*1000));
    m += rays*.3*flare;
    
    return m;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.TextureUV-.5;
	// float2 uv = input.Position.xy-.5;
    uv *= 3;
    float3 col = 0;

    float2 gv = frac(uv)-.5;

    // col.rg = gv;


    col += Star(gv, 0.2);
    if(gv.x>.48 || gv.y>.48) col.r=1;
    return float4(col, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};