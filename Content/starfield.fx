#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float iTime;
// float2 iMouse = float2(0, 0);
// float2 iResolution = float2(1920, 1080);
float2 iResolution = float2(4096, 2160);

#define iterations 17
#define formuparam 0.53

#define volsteps 20
#define stepsize 0.1

#define zoom   0.800
#define tile   0.850
#define speed  0.010 

#define brightness 0.0015
#define darkmatter 0.300
#define distfading 0.730
#define saturation 0.850

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

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//get coords and direction
	// float2 uv=fragCoord.xy/iResolution.xy-.5;
	// uv.y*=iResolution.y/iResolution.x;
    float2 iMouse = float2(0, 0);
    // float2 iResolution = float2(1920, 1080);

	
    float2 uv=input.TextureUV;
	float3 dir=float3(uv*zoom, 1.);
	float time=iTime*speed+.25;

	//mouse rotation
	float a1=.5+iMouse.x/iResolution.x*2.;
	float a2=.8+iMouse.y/iResolution.y*2.;
	float2x2 rot1=float2x2(cos(a1),sin(a1),-sin(a1),cos(a1));
	float2x2 rot2=float2x2(cos(a2),sin(a2),-sin(a2),cos(a2));
	dir.xz = mul(dir.xz, rot1);
	dir.xy = mul(dir.xy, rot2);
	float3 from=float3(1.,.5,0.5);
	from+=float3(time*2.,time,-2.);
	from.xz = mul(from.xz, rot1);
	from.xy = mul(from.xy, rot2);
	
    float3 pw = sin(iTime/6.)*0.08 + 1.;
	//volumetric rendering
	float s=0.1,fade=1.;
	float3 v = 0.;
	for (int r=0; r<volsteps; r++) {
		float3 p=from+s*dir*.5;
		p = abs(tile-fmod(p,tile*2.)); // tiling fold
		float pa,a=pa=0.;
		for (int i=0; i<iterations; i++) { 
			// p=abs(p)/dot(p,p)-formuparam; // the magic formula
			p=pow(abs(p), pw)/max(dot(p,p), 0.001)-formuparam; // the magic formula
			a+=abs(length(p)-pa); // absolute sum of average change
			pa=length(p);
		}
		float dm=max(0.,darkmatter-a*a*.001); //dark matter
		a*=a*a; // add contrast
		if (r>6) fade*=1.-dm; // dark matter, don't render near
		//v+=float3(dm,dm*.5,0.);
		v+=fade;
		v+=float3(s,s*s,s*s*s*s)*a*brightness*fade; // coloring based on distance
		fade*=distfading; // distance fading
		s+=stepsize;
	}
	v=lerp(length(v),v,saturation); //color adjust

    // Output to screen
    return float4(v*.01,1.);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};