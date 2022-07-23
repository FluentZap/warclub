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
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TextureUV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, WorldViewProjection);
    output.TextureUV = input.TextureUV;
	// output.Color = input.Color;

	return output;
}

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};


struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

float time;

Texture2D NoiseTexture;

SamplerState _MainTex
{
    Texture = <NoiseTexture>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
	AddressU = Wrap;
	AddressV = Wrap;
};

// Texture2D _MainTex;
// sampler2D _MainTex;
float4 _MainTex_ST;

float2 r2D(float2 p)
{
    return float2(frac(sin(dot(p, float2(92.51, 65.19)))*4981.32),
                frac(sin(dot(p, float2(23.34, 15.28)))*6981.32));
}

#define PI 3.141592

// #define _Time float2(1, 2)

float polygon(float2 p, float s)
{
    float a = ceil(s*(atan2(-p.y, -p.x)/PI+1.)*.5);
    float n = 2.*PI/s;
    float t = n*a-n*.5;
    return lerp(dot(p, float2(cos(t), sin(t))), length(p), .3);
}

float voronoi(float2 p, float s)
{
    float2 i = floor(p*s);
    float2 current = i + frac(p*s);
    float min_dist = 1.;
    for (int y = -1; y <= 1; y++)
    {
        for (int x = -1; x <= 1; x++)
        {
            float2 neighbor = i + float2(x, y);
            float2 vPoint = r2D(neighbor);
            vPoint = 0.5 + 0.5*sin(time*.5 + 6.*vPoint);
            float dist = polygon(neighbor+vPoint - current, 3.);
            min_dist = min(min_dist, dist);
        }
    }
    return min_dist;
}

float4 MainPS(v2f i) : SV_Target
{
    float2 uv = i.uv*2.-1.;
    float2 e = float2(.01, .0);
    
    float s = 2.;
    float vor = 1.-voronoi(uv, s);
    float dx = 1.-voronoi(uv-e.xy, s);
    float dy = 1.-voronoi(uv-e.yx, s);
    dx = (dx-vor)/e.x;
    dy = (dy-vor)/e.x;
    
    float t = time;
    float3 n = normalize(float3(dx, dy, 1.));
    float3 lp = float3(cos(t), sin(t), .5)*2.;
    float3 ld = normalize(lp-float3(uv, 0.));
    float3 ed = normalize(float3(0., .0, 1.)-float3(uv, 0.));
    float3 hd = normalize(ld + ed);
    float sl = pow(max(dot(hd,n), 0.),4.);
    float oc = clamp(pow((vor), 2.), 0., 1.);
    float amb = (1.-vor)*.5;
    float diff = max(dot(n, ld), 0.)*.75;
    float l = oc*diff+amb+sl;
    
    float3 col = float3(0,0,0);
    
    
    col += l*tex2D(_MainTex, normalize(reflect(float3(0., .0, 1.), n))).rgb;

    return float4(col,1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};