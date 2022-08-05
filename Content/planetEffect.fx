#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Noise functions and most of the implementation based on
// https://www.shadertoy.com/view/4dS3Wd by Morgan McGuire @morgan3d!

// see also
// https://iquilezles.org/articles/warp
// https://thebookofshaders.com/13/
// for informations on fbm, noise, ...

// please check out stuff like: https://www.shadertoy.com/view/lsGGDd
// for more advanced planet lighting/clouds/...

// Looking for a blue planet? Colors:
// vec3 col_top = vec3(0.0, 0.5, 0.0);
// vec3 col_bot = vec3(0.0, 1.0, 1.0);
// vec3 col_mid1 = vec3(0.0, 1.0, 0.0);
// vec3 col_mid2 = vec3(0.0, 0.0, 1.0);
// vec3 col_mid3 = vec3(0.0, 0.0, 1.0);


// number of octaves of fbm
#define NUM_NOISE_OCTAVES 10
// size of the planet
#define PLANET_SIZE		0.75
// uncomment to use a simple sharpen filter
// #define SHARPEN
// simple and fast smoothing of outside border
#define SMOOTH

// float2 iResolution = float2(4096, 2160);
float2 iResolution = float2(512, 512);

float iTime;

struct PixelShaderInput
{
	float4 Position : SV_POSITION;
	float2 TextureUV : TEXCOORD0;
    float4 Color : COLOR0;
};



//////////////////////////////////////////////////////////////////////////////////////
// Noise functions:
//////////////////////////////////////////////////////////////////////////////////////

// Precision-adjusted variations of https://www.shadertoy.com/view/4djSRW
float hash(float p) { p = frac(p * 0.011); p *= p + 7.5; p *= p + p; return frac(p); }

float noise(float3 x) {
    const float3 step = float3(110, 241, 171);
    float3 i = floor(x);
    float3 f = frac(x);
    float n = dot(i, step);
    float3 u = f * f * (3.0 - 2.0 * f);
    return lerp(lerp(lerp( hash(n + dot(step, float3(0, 0, 0))), hash(n + dot(step, float3(1, 0, 0))), u.x),
                   lerp( hash(n + dot(step, float3(0, 1, 0))), hash(n + dot(step, float3(1, 1, 0))), u.x), u.y),
               lerp(lerp( hash(n + dot(step, float3(0, 0, 1))), hash(n + dot(step, float3(1, 0, 1))), u.x),
                   lerp( hash(n + dot(step, float3(0, 1, 1))), hash(n + dot(step, float3(1, 1, 1))), u.x), u.y), u.z);
}

float fbm(float3 x) {
	float v = 0.0;
	float a = 0.5;
	float3 shift = 100;
	for (int i = 0; i < NUM_NOISE_OCTAVES; ++i) {
		v += a * noise(x);
		x = x * 2.0 + shift;
		a *= 0.5;
	}
	return v;
}

// //////////////////////////////////////////////////////////////////////////////////////
// // Visualization:
// //////////////////////////////////////////////////////////////////////////////////////

static const float pi          = 3.1415926535;
static const float inf         = 9999999.9;
float square(float x) { return x * x; }
float infIfNegative(float x) { return (x >= 0.0) ? x : inf; }

// C = sphere center, r = sphere radius, P = ray origin, w = ray direction
float intersectSphere(float3 C, float r, float3 P, float3 w) {	
	float3 v = P - C;
	float b = -dot(w, v);
	float c = dot(v, v) - square(r);
	float d = (square(b) - c);
	if (d < 0.0) { return inf; }	
	float dsqrt = sqrt(d);
	
	// Choose the first positive intersection
	return min(infIfNegative((b - dsqrt)), infIfNegative((b + dsqrt)));
}

// returns max of a single float3
float max3 (float3 v) {
  return max (max (v.x, v.y), v.z);
}

float3 getColorForCoord(float2 fragCoord) {
    // (intermediate) results of fbm
    float3 q = 0.0;
    float3 r = 0.0;
	float v = 0.0;
    float3 color = 0.0;

    // planet rotation
    float theta = iTime * 0.15;  
    float3x3 rot = float3x3(
        cos(theta), 0, sin(theta),	// column 1
        0, 1, 0,	                // column 2
        -sin(theta), 0, cos(theta)	// column 3
    );

    // Ray-sphere
    const float verticalFieldOfView = 25.0 * pi / 180.0;

    // position of viewpoint (P) and ray of vision (w)
    float3 P = float3(0.0, 0.0, 5.0);
    // float3 w = normalize(float3(fragCoord.xy - iResolution.xy * 0.5, (iResolution.y) / (-2.0 * tan(verticalFieldOfView / 2.0))));
    float3 w = normalize(float3(fragCoord.xy - 0.5, 1.0 / (-2.0 * tan(verticalFieldOfView / 2.0))));

    // calculate intersect with sphere (along the "line" of w from P)
    float t = intersectSphere(float3(0, 0, 0), PLANET_SIZE, P, w);
    
    // calculate color for sphere/background
    if (t < inf) {
        // calculate point of intersection on the sphere
        float3 X = P + w*t;

        // apply rotation matrix
        X = mul(rot, X);

        // calculate fbm noise (3 steps)
        q = float3(fbm(X + 0.025*iTime), fbm(X), fbm(X));
        r = float3(fbm(X + 1.0*q + 0.01*iTime), fbm(X + q), fbm(X + q));
        v = fbm(X + 5.0*r + iTime*0.005);
    } else {
        // ray missed the sphere
        return 0.0;
    }
    
    // convert noise value into color
    // three colors: top - mid - bottom (mid being constructed by three colors)
    float3 col_top = float3(1.0, 1.0, 1.0);
    float3 col_bot = float3(0.0, 0.0, 0.0);
    float3 col_mid1 = float3(0.1, 0.2, 0.0);
    float3 col_mid2 = float3(0.7, 0.4, 0.3);
    float3 col_mid3 = float3(1.0, 0.4, 0.2);

    // mix mid color based on intermediate results
    float3 col_mid = lerp(col_mid1, col_mid2, clamp(r, 0.0, 1.0));
    col_mid = lerp(col_mid, col_mid3, clamp(q, 0.0, 1.0));
    col_mid = col_mid;

    // calculate pos (scaling betwen top and bot color) from v
    float pos = v * 2.0 - 1.0;
    color = lerp(col_mid, col_top, clamp(pos, 0.0, 1.0));
    color = lerp(color, col_bot, clamp(-pos, 0.0, 1.0));

    // clamp color to scale the highest r/g/b to 1.0
    color = color / max3(color);
      
    // create output color, increase light > 0.5 (and add a bit to dark areas)
    color = (clamp((0.4 * pow(v,3.) + pow(v,2.) + 0.5*v), 0.0, 1.0) * 0.9 + 0.1) * color;
    
    // apply diffuse lighting  
    float diffuse = max(0.0, dot(P + w*t, float3(1.0, sqrt(0.5), 1.0)));
    float ambient = 0.1;
    color *= clamp((diffuse + ambient), 0.0, 1.0);
    
#ifdef SMOOTH
    // apply a smoothing to the outside
    color *= (P + w*t).z * 2.0;
#endif    
    
    return color;
}


float4 MainPS(PixelShaderInput input) : COLOR
{
	float2 uv = input.TextureUV;
    float3 fragColor = float3(0, 0, 0);
// #ifdef SHARPEN
//     // use a simple sharpen filter (you could improve that immensely!
//     fragColor.rgb =
//         getColorForCoord(fragCoord) * 3. -
//         getColorForCoord(fragCoord + vec2(1.0, 0.0)) * 0.5 -
//         getColorForCoord(fragCoord + vec2(0.0, 1.0)) * 0.5 -
//         getColorForCoord(fragCoord - vec2(1.0, 0.0)) * 0.5 -
//         getColorForCoord(fragCoord - vec2(0.0, 1.0)) * 0.5;
// #else
//     // just use a single pass
    fragColor.rgb = getColorForCoord(uv);
    // fragColor.rgb = getColorForCoord(float2(256, 256));
// #endif
    return float4(fragColor,1.0);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};