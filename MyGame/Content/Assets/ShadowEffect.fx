//----------------------------------------------------------------------------------
// Shadow mapping based on https://learnopengl.com/Advanced-Lighting/Shadows/Shadow-Mapping
// and even more based on https://community.monogame.net/t/shadow-mapping-on-monogame/8212/2
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
//----------------------------------------------------------------------------------
float4x4 _World;
float4x4 _LightViewProjection;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 Depth : TEXCOORD0;
};

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
VertexShaderOutput MainVS(float4 Position : POSITION0)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	float4 worldPos = mul(Position, _World);
	output.Position = mul(worldPos, _LightViewProjection);
	output.Depth = output.Position.zw;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return float4(input.Depth.x / input.Depth.y, 0, 0, 0);
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
};
