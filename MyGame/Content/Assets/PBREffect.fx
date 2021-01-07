//----------------------------------------------------------------------------------
// PBR shader based on https://learnopengl.com/PBR/Lighting
// Shadow maps based on https://community.monogame.net/t/shadow-mapping-on-monogame/8212/2
//----------------------------------------------------------------------------------

//----------------------------------------------------------------------------------
// Standard defines
//----------------------------------------------------------------------------------
#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0 
	#define PS_SHADERMODEL ps_5_0
#endif

//----------------------------------------------------------------------------------
// Properties
//----------------------------------------------------------------------------------
float4x4 _World;
float4x4 _View;
float4x4 _Projection;
float3 _CameraPos;

float4 _Color;
Texture2D _Albedo;
Texture2D _Metalness;
Texture2D _Displacement;
Texture2D _Normal;
Texture2D _Roughness;
Texture2D _AO;

//Lights
static const int MaxDirectionalLights = 8;
static const int MaxPointLights = 8;
static const int CubeSides = 6;

static const float3 FaceDirectons[CubeSides] = {
    float3(1, 0, 0),
    float3(-1, 0, 0),
    float3(0, 1, 0),
    float3(0, -1, 0),
    float3(0, 0, 1),
    float3(0, 0, -1),
};

float3 _AmbientColor;

float3 _DirectionalLights[MaxDirectionalLights];
float3 _DirectionalColors[MaxDirectionalLights];
float _DirectionalIntensity[MaxDirectionalLights];

float3 _PointLightPos[MaxPointLights];
float3 _PointLightColors[MaxPointLights];
float _PointLightIntensity[MaxPointLights];
float _PointLightRange[MaxPointLights];

//Shadows
Texture2D _DirectionalShadowMap;
SamplerState DirectionalShadowMapSampler
{
    Texture = (_DirectionalShadowMap);
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2DArray _PointShadowMap;
SamplerState PointShadowMapSampler
{
    Texture = (_PointShadowMap);
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    AddressU = Wrap;
    AddressV = Wrap;
};

float4x4 _LightSpaceMatrix;
float4x4 _PointLightSpaceMatrix[MaxPointLights * CubeSides];
int _ShadowMapSize;
float _DepthBias;
int _DirectionalShadowIndex;


//----------------------------------------------------------------------------------
static const float PI = 3.14159265359;

//----------------------------------------------------------------------------------
// Required attributes of the input vertices
//----------------------------------------------------------------------------------
struct VertexShaderInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL;
    //float3 Tangent : TANGENT;
    //float3 Binormal : BINORMAL;
    float2 TextureUV : TEXCOORD0;
};

// Semantics for output of vertex shader / input of pixel shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TextureUV : TEXCOORD0;
    float3 Normal : TEXCOORD1;
    //float3 Tangent : TEXCOORD2;
    //float3 Binormal : TEXCOORD3;
    float3 WorldPosition : TEXCOORD2;
};

SamplerState MeshTextureSampler
{
    Filter = Anisotropic;
    AddressU = Wrap;
    AddressV = Wrap;
};

//----------------------------------------------------------------------------------
// PBR equations
//----------------------------------------------------------------------------------
float3 fresnelSchlick(float cosTheta, float3 F0)
{
    cosTheta = min(cosTheta, 1.0);
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

//Generating tangents and binormals based on https://github.com/JoeyDeVries/LearnOpenGL/blob/master/src/6.pbr/1.2.lighting_textured/1.2.pbr.fs
float3 getNormalFromMap(VertexShaderOutput input)
{
    float3 tangentNormal = _Normal.Sample(MeshTextureSampler, input.TextureUV).rgb;
    tangentNormal = normalize(tangentNormal * 2.0 - 1.0);

    float3 Q1 = ddx(input.WorldPosition);
    float3 Q2 = ddy(input.WorldPosition);
    float2 st1 = ddx(input.TextureUV);
    float2 st2 = ddy(input.TextureUV);

    float3 N = normalize(input.Normal);
    float3 T = -normalize(Q1 * st2.y - Q2 * st1.y);
    float3 B = normalize(cross(N, T));

    //float3 N = normalize(input.Normal);
    //float3 T = normalize(input.Tangent);
    //float3 B = normalize(input.Binormal);

    float3x3 TBN = float3x3(T, B, N);

    return normalize(mul(tangentNormal, TBN));
}

//----------------------------------------------------------------------------------
// Directional lights: Calculates the shadow term using PCF
//----------------------------------------------------------------------------------
float CalcDirectionalShadowsPCF(float light_space_depth, float ndotl, float2 shadow_coord)
{
    float shadow_term = 0;

    float variableBias = clamp(0.001 * tan(acos(ndotl)), 0, _DepthBias);

    //safe to assume it's a square
    float size = 1.0 / _ShadowMapSize;
    	
    float samples[4];
    samples[0] = (light_space_depth - variableBias < _DirectionalShadowMap.Sample(DirectionalShadowMapSampler, shadow_coord).r);
    samples[1] = (light_space_depth - variableBias < _DirectionalShadowMap.Sample(DirectionalShadowMapSampler, shadow_coord + float2(size, 0)).r);
    samples[2] = (light_space_depth - variableBias < _DirectionalShadowMap.Sample(DirectionalShadowMapSampler, shadow_coord + float2(0, size)).r);
    samples[3] = (light_space_depth - variableBias < _DirectionalShadowMap.Sample(DirectionalShadowMapSampler, shadow_coord + float2(size, size)).r);

    shadow_term = (samples[0] + samples[1] + samples[2] + samples[3]) / 4.0;

    return shadow_term;
}

//----------------------------------------------------------------------------------
// Point Lights: Calculates the shadow term using PCF
//----------------------------------------------------------------------------------
float CalcPointShadowsPCF(float light_space_depth, float ndotl, float3 shadow_coord)
{
	float shadow_term = 0;
    float variableBias = clamp(0.001 * tan(acos(ndotl)), 0, _DepthBias);
    float size = 1.0 / _ShadowMapSize;
    	
    float samples[4];
    samples[0] = (light_space_depth - variableBias < _PointShadowMap.Sample(PointShadowMapSampler, shadow_coord).r);
    samples[1] = (light_space_depth - variableBias < _PointShadowMap.Sample(PointShadowMapSampler, shadow_coord + float3(size, 0, 0)).r);
    samples[2] = (light_space_depth - variableBias < _PointShadowMap.Sample(PointShadowMapSampler, shadow_coord + float3(0, size, 0)).r);
    samples[3] = (light_space_depth - variableBias < _PointShadowMap.Sample(PointShadowMapSampler, shadow_coord + float3(size, size, 0)).r);

    shadow_term = (samples[0] + samples[1] + samples[2] + samples[3]) / 4.0;

    return shadow_term;
}

//----------------------------------------------------------------------------------
// Lights
//----------------------------------------------------------------------------------
float3 CalculatePointLights(float3 worldPosition, float3 N, float3 albedo, float metallic, float roughness)
{
    float3 V = normalize(_CameraPos - worldPosition);

    //Calculate surface reflection at zero incidence, default to 0.04 but adjust for metallic surfaces.
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, albedo, metallic);

    //Reflection equation
    float3 Lo = float3(0.0, 0.0, 0.0);
    for(int i = 0; i < MaxPointLights; ++i)
	{
        // calculate per-light radiance
        float3 L = normalize(_PointLightPos[i] - worldPosition);
        float3 H = normalize(V + L);
        float distance = length(_PointLightPos[i] - worldPosition);
        float attenuation = _PointLightRange[i] / (distance * distance);
        float3 radiance = _PointLightColors[i] * attenuation * _PointLightIntensity[i];

        // cook-torrance brdf
        float NDF = DistributionGGX(N, H, roughness);
        float G = GeometrySmith(N, V, L, roughness);
        float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);

        float3 kS = F;
        float3 kD = float3(1.0, 1.0, 1.0) - kS;
        kD *= 1.0 - metallic;

        float3 numerator = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        float3 specular = numerator / max(denominator, 0.001);

        // add to outgoing radiance Lo
        float NdotL = max(dot(N, L), 0.0);

        //Shadows
        float shadowContribution = 1.0;

		//Work out which face of the shadow cube
		float3 directionToFragment = normalize(worldPosition - _PointLightPos[i]);
		float closestDirection = -1;
		int faceIndex = 0;
		for(int face = 0; face < CubeSides; ++face)
		{
			float3 forward = FaceDirectons[face];
			float result = dot(directionToFragment, forward);
			if(result > closestDirection)
			{
				closestDirection = result;
				faceIndex = face;
			}
		}

        int arrayIndex = i * CubeSides + faceIndex;
        float4 lightingPosition = mul(float4(worldPosition, 1), _PointLightSpaceMatrix[arrayIndex]);
		float2 shadowTexCoord = mad(0.5, lightingPosition.xy / lightingPosition.w, float2(0.5, 0.5));
		shadowTexCoord.y = 1.0f - shadowTexCoord.y;

		float ourDepth = (lightingPosition.z / lightingPosition.w);
        shadowContribution = CalcPointShadowsPCF(ourDepth, NdotL, float3(shadowTexCoord, arrayIndex));

		Lo += (kD * albedo / PI + specular) * radiance * NdotL * shadowContribution;
    }

    return Lo;
}

float3 CalculateDirectionalLights(float3 worldPosition, float3 N, float3 albedo, float metallic, float roughness)
{
    float3 V = normalize(_CameraPos - worldPosition);

    //Calculate surface reflection at zero incidence, default to 0.04 but adjust for metallic surfaces.
    float3 F0 = float3(0.04, 0.04, 0.04);
    F0 = lerp(F0, albedo, metallic);

    //Reflection equation
    float3 Lo = float3(0.0, 0.0, 0.0);
    for(int i = 0; i < MaxDirectionalLights; ++i)
	{
        // calculate per-light radiance
        float3 L = -_DirectionalLights[i];
        float3 H = normalize(V + L);
        float3 radiance = _DirectionalColors[i] * _DirectionalIntensity[i];

        // cook-torrance brdf
        float NDF = DistributionGGX(N, H, roughness);
        float G = GeometrySmith(N, V, L, roughness);
        float3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);

        float3 kS = F;
        float3 kD = float3(1.0, 1.0, 1.0) - kS;
        kD *= 1.0 - metallic;

        float3 numerator = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        float3 specular = numerator / max(denominator, 0.001);

        float NdotL = max(dot(N, L), 0.0);

        //Shadows
        float shadowContribution = 1.0;

		if(i == _DirectionalShadowIndex)
		{
			float4 lightingPosition = mul(float4(worldPosition, 1), _LightSpaceMatrix);
			float2 shadowTexCoord = mad(0.5, lightingPosition.xy / lightingPosition.w, float2(0.5, 0.5));
			shadowTexCoord.y = 1.0f - shadowTexCoord.y;
			float ourDepth = (lightingPosition.z / lightingPosition.w);
			shadowContribution = CalcDirectionalShadowsPCF(ourDepth, NdotL, shadowTexCoord);
		}

		// add to outgoing radiance Lo
		Lo += (kD * albedo / PI + specular) * radiance * NdotL * shadowContribution;
    }
    return Lo;
}

//----------------------------------------------------------------------------------
// Actual shaders
//----------------------------------------------------------------------------------
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    //Position
    float4 worldPosition = mul(float4(input.Position.xyz, 1), _World);
    float4 viewPosition = mul(worldPosition, _View);
    output.Position = mul(viewPosition, _Projection);
    output.WorldPosition = worldPosition.xyz;

    //Normals
    output.Normal = mul(input.Normal, (float3x3)_World);
    output.Normal = normalize(output.Normal);

    //UVs
    output.TextureUV = input.TextureUV;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	//Read textures
	float3 albedo = _Albedo.Sample(MeshTextureSampler, input.TextureUV).xyz;
	float metallic = _Metalness.Sample(MeshTextureSampler, input.TextureUV).r;
	float roughness = _Roughness.Sample(MeshTextureSampler, input.TextureUV).r;
	float ao = _AO.Sample(MeshTextureSampler, input.TextureUV).r;
	float3 normal = getNormalFromMap(input);

	//Convert albedo to linear space
	albedo = pow(abs(albedo), 2.2);

	//Point lights
	float3 Lo = CalculatePointLights(input.WorldPosition, normal, albedo, metallic, roughness);
	Lo += CalculateDirectionalLights(input.WorldPosition, normal, albedo, metallic, roughness);
	
    //Final colours
	float3 ambient = _AmbientColor * albedo * ao;
	float3 color = ambient + Lo;

	color = color / (color + float3(1.0, 1.0, 1.0));
	color = pow(abs(color), float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));

	return float4(color, 1.0);
}

//----------------------------------------------------------------------------------
// Technique and passes within the technique
//----------------------------------------------------------------------------------
technique MainEffect
{
    pass Pass0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}