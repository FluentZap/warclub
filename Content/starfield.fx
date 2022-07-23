#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float time;

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

float3 hash( float3 p ) // replace this by something better
{
	p = float3( dot(p,float3(127.1,311.7, 74.7)),
			    dot(p,float3(269.5,183.3,246.1)),
			    dot(p,float3(113.5,271.9,124.6)));

	return -1.0 + 2.0*frac(sin(p)*43758.5453123);
}

float noise( in float3 p )
{
    float3 i = floor( p );
    float3 f = frac( p );
	
	float3 u = f*f*(3.0-2.0*f);

    return lerp( lerp( lerp( dot( hash( i + float3(0.0,0.0,0.0) ), f - float3(0.0,0.0,0.0) ), 
                          dot( hash( i + float3(1.0,0.0,0.0) ), f - float3(1.0,0.0,0.0) ), u.x),
                     lerp( dot( hash( i + float3(0.0,1.0,0.0) ), f - float3(0.0,1.0,0.0) ), 
                          dot( hash( i + float3(1.0,1.0,0.0) ), f - float3(1.0,1.0,0.0) ), u.x), u.y),
                lerp( lerp( dot( hash( i + float3(0.0,0.0,1.0) ), f - float3(0.0,0.0,1.0) ), 
                          dot( hash( i + float3(1.0,0.0,1.0) ), f - float3(1.0,0.0,1.0) ), u.x),
                     lerp( dot( hash( i + float3(0.0,1.0,1.0) ), f - float3(0.0,1.0,1.0) ), 
                          dot( hash( i + float3(1.0,1.0,1.0) ), f - float3(1.0,1.0,1.0) ), u.x), u.y), u.z );
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	// Normalized pixel coordinates (from 0 to 1)
    float2 uv = input.TextureUV;
    
    // Stars computation:
    float3 stars_direction = normalize(float3(uv * 2.0f - 1.0f, 1.0f)); // could be view vector for example
	float stars_threshold = 8.0f; // modifies the number of stars that are visible
	float stars_exposure = 200.0f; // modifies the overall strength of the stars
	float stars = pow(clamp(noise(stars_direction * 200.0f), 0.0f, 1.0f), stars_threshold) * stars_exposure;
	stars *= lerp(0.4, 1.4, noise(stars_direction * 100.0f + float(time))); // time based flickering
	
    // Output to screen
    return float4(stars, stars, stars, 1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};