#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float iTime;
Texture2D g_texture;

sampler TextureSampler = 
sampler_state
{
    Texture = <g_texture>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
};

float2 iResolution = float2(4096, 2160);

static const float sphsize=.7; // planet size
static const float dist=.27; // distance for glow and distortion
static const float perturb=.3; // distortion amount of the flow around the planet
static const float displacement=.015; // hot air effect
static const float windspeed=.4; // speed of wind flow
static const float steps=110.; // number of steps for the volumetric rendering
static const float stepsize=.025; 
static const float brightness=.43;
static const float3 planetcolor=float3(0.55,0.4,0.3);
static const float fade=.005; //fade by distance
static const float glow=3.5; // glow amount, mainly on hit side

// fractal params
static const int iterations=13; 
static const float fractparam=.7;
static const float3 offset=float3(1.5,2.,-1.5);

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



float wind(float3 p) {
	float d=max(0.,dist-max(0.,length(p)-sphsize)/sphsize)/dist; // for distortion and glow area
	float x=max(0.2,p.x*2.); // to increase glow on left side
	p.y*=1.+max(0.,-p.x-sphsize*.25)*1.5; // left side distortion (cheesy)
	p-=d*normalize(p)*perturb; // spheric distortion of flow
	p+=float3(iTime*windspeed,0.,0.); // flow movement
	p=abs(frac((p+offset)*.1)-.5); // tile folding 
	for (int i=0; i<iterations; i++) {  
		p=abs(p)/dot(p,p)-fractparam; // the magic formula for the hot flow
	}
	return length(p)*(1.+d*glow*x)+d*glow*x; // return the result with glow applied
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 uv = input.TextureUV;
	float3 dir=float3(uv,1.);
	dir.x*=iResolution.x/iResolution.y;
	float3 from=float3(0.,0.,-2.+tex2D(TextureSampler,uv*.5+iTime).x*stepsize); //from+dither

	// volumetric rendering
	float v=0., l=-0.0001, t=iTime*windspeed*.2;
	for (float r=10.;r<steps;r++) {
		float3 p=from+r*dir*stepsize;
		float tx=tex2D(TextureSampler,uv*.2+float2(t,0.)).x*displacement; // hot air effect
		if (length(p)-sphsize-tx>0.)
		// outside planet, accumulate values as ray goes, applying distance fading
			v+=min(50.,wind(p))*max(0.,1.-r*fade); 
		else if (l<0.) 
		//inside planet, get planet shading if not already 
		//loop continues because of previous problems with breaks and not always optimizes much
			l=pow(max(.53,dot(normalize(p),normalize(float3(-1.,.5,-0.3)))),4.)
			*(.5+tex2D(TextureSampler,uv*float2(2.,1.)*(1.+p.z*.5)+float2(tx+t*.5,0.)).x*2.);
		}
	v/=steps; v*=brightness; // average values and apply bright factor
	float3 col=float3(v*1.25,v*v,v*v*v)+l*planetcolor; // set color
	col*=1.-length(pow(abs(uv),float2(5., 5.)))*14.; // vignette (kind of)
    
    return float4(col,1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};