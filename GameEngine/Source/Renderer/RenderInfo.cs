using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine
{
	public class RenderInfo
	{
		public const int m_nMaxDirectionalLights = 8;
		public const int m_nMaxPointLights = 8;
		public const int m_nMaxPointShadowMaps = m_nMaxPointLights;
		public const int m_nCubeSides = 6;

		//Camera
		public Matrix m_m44View;
		public Matrix m_m44Projection;
		public Vector3 m_v3CameraPos;

		//Lights
		public Vector3 m_v3AmbientLight = new Vector3();

		public Vector3[] m_av3DirectionalLightDirections = new Vector3[m_nMaxDirectionalLights];
		public Vector3[] m_av3DirectionalLightColours = new Vector3[m_nMaxDirectionalLights];
		public float[] m_afDirectionalLightIntensity = new float[m_nMaxDirectionalLights];

		public Vector3[] m_av3PointLightPositions = new Vector3[m_nMaxPointLights];
		public Vector3[] m_av3PointLightColours = new Vector3[m_nMaxPointLights];
		public float[] m_afPointLightIntensity = new float[m_nMaxPointLights];
		public float[] m_afPointLightRange = new float[m_nMaxPointLights];

		//Shadows
		public RenderTarget2D m_DirectionalShadowMap = null;
		public RenderTarget2D m_PointShadowMaps = null;
		public Matrix m_m44LightViewProjection;
		public Matrix[] m_m44PointLightViewProjection = new Matrix[m_nCubeSides * m_nMaxPointShadowMaps];
		public float m_fShadowDepthBias = 0.2f;
		public int m_nShadowMapSize = 2048;
		public int m_nDirecitonalShadowIndex = -1;
	}
}
