#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;
float iTime;
#define NUM_LAYERS 6

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

float Hash21(float2 p)
{
    p = frac(p * float2(123.34, 456.21));
    p += dot(p, p + 45.32);
    return frac(p.x*p.y);
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
    
    m *= smoothstep(1., .2, d);
    return m;
}

float3 StarLayer(float2 uv) {
	float3 col = 0;
	
    float2 gv = frac(uv)-.5;
    float2 id = floor(uv);
    
    for(int y=-1;y<=1;y++) {
    	for(int x=-1;x<=1;x++) {
            float2 offs = x, y;
            
    		float n = Hash21(id+offs); // random between 0 and 1
            float size = frac(n*345.32);
            
    		float star = Star(gv-offs-float2(n, frac(n*34.))+.5, smoothstep(.9, 1., size)*.6);
            
            float3 color = sin(float3(.2, .3, .9)*frac(n*2345.2)*123.2)*.5+.5;
            color = color*float3(1,.25,1.+size)+float3(.2, .2, .1)*2.;
            
            star *= sin(iTime*3.+n*6.2831)*.5+1.;
            col += star*size*color;
        }
    }
    return col;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.TextureUV-.5;

    float t = iTime*.02;
    float M = 1;
    uv += M*4.;
    
    uv = mul(uv, Rot(t));
    float3 col = 0;
    
    for(float i=0.; i<1.; i+=1./NUM_LAYERS) {
    	float depth = frac(i+t);
        
        float scale = lerp(20., .5, depth);
        float fade = depth*smoothstep(1., .9, depth);
        col += StarLayer(uv*scale+i*453.2-M)*fade;
    }
    
    col = pow(col, float(.4545));	// gamma correction

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