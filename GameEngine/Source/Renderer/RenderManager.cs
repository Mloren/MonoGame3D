using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public enum ERenderQueue
	{
		Opaque,
		Transparent //Not implemented yet
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class RenderManager
	{
		private Game m_Game = null;
		private GraphicsDeviceManager m_GraphicsDeviceManager = null;
		private GraphicsDevice m_Graphics = null;
		private SpriteBatch m_SpriteBatch = null;

		private Camera m_Camera = null;
		private List<MeshRenderer> m_OpaqueList = new List<MeshRenderer>();
		private List<Light> m_LightList = new List<Light>();

		private RenderInfo m_RenderInfo = new RenderInfo();

		//Shadow mapping
		private Material m_ShadowMaterial = null;
		private RenderTarget2D m_DirectionalShadowMap = null; //Directional shadow map
		private RenderTarget2D m_PointShadowMap = null; //Omnidirectional shadow maps
		private float m_fShadowDepthBias = 0.2f;
		private int m_nShadowMapSize = 2048;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public RenderManager(Game game)
		{
			m_Game = game;
			m_GraphicsDeviceManager = new GraphicsDeviceManager(game);

			m_GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			m_GraphicsDeviceManager.PreferMultiSampling = true;
			m_GraphicsDeviceManager.PreparingDeviceSettings += SetMultiSampling;
			m_GraphicsDeviceManager.ApplyChanges();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void SetMultiSampling(object sender, PreparingDeviceSettingsEventArgs e)
		{
			var pp = e.GraphicsDeviceInformation.PresentationParameters;
			pp.MultiSampleCount = 8;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void InitGraphicsDevice()
		{
			m_Graphics = m_Game.GraphicsDevice;
			m_SpriteBatch = new SpriteBatch(m_Graphics);

			//Directional shadow map for one direcitonal light
			m_DirectionalShadowMap = new RenderTarget2D(
				m_Graphics,
				m_nShadowMapSize,
				m_nShadowMapSize,
				false,
				SurfaceFormat.Single,
				DepthFormat.Depth24,
				0,
				RenderTargetUsage.PlatformContents);

			//Omnidirecitonal shadow maps for a small number of point lights
			m_PointShadowMap = new RenderTarget2D(
				m_Graphics,
				m_nShadowMapSize,
				m_nShadowMapSize,
				false,
				SurfaceFormat.Single,
				DepthFormat.Depth24,
				0,
				RenderTargetUsage.PlatformContents,
				false,
				RenderInfo.m_nCubeSides * RenderInfo.m_nMaxPointShadowMaps);

			//Shadow map shader
			m_ShadowMaterial = new Material("ShadowEffect");
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetCamera(Camera camera)
		{
			m_Camera = camera;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Camera GetCamera()
		{
			return m_Camera;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void AddRenderer(MeshRenderer renderer)
		{
			ERenderQueue eRenderQueue = renderer.GetRenderQueue();
			switch(eRenderQueue)
			{
				case ERenderQueue.Opaque:
					m_OpaqueList.Add(renderer);
					break;
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void RemoveRenderer(MeshRenderer renderer)
		{
			ERenderQueue eRenderQueue = renderer.GetRenderQueue();
			switch(eRenderQueue)
			{
				case ERenderQueue.Opaque:
					m_OpaqueList.Remove(renderer);
					break;
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void AddLight(Light light)
		{
			m_LightList.Add(light);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void RemoveLight(Light light)
		{
			m_LightList.Remove(light);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetAmbientLight(Color color)
		{
			m_RenderInfo.m_v3AmbientLight = color.ToVector3();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void Draw()
		{
			if(!Assert.Valid(m_Graphics, "RenderManager hasn't been initialized."))
				return;

			if(m_Camera == null)
				return;

			m_RenderInfo.m_m44View = m_Camera.GetViewMatrix();
			m_RenderInfo.m_m44Projection = m_Camera.GetProjectionMatrix();
			m_RenderInfo.m_v3CameraPos = m_Camera.GetLocalPosition();
			m_RenderInfo.m_nShadowMapSize = m_nShadowMapSize;
			m_RenderInfo.m_fShadowDepthBias = m_fShadowDepthBias;

			//Prepare lights
			int nDirectionalLightCount = 0;
			int nPointLightCount = 0;
			for(int i = 0; i < m_LightList.Count; ++i)
			{
				Light light = m_LightList[i];
				switch(light.GetLightType())
				{
					case ELightType.Directional:
					{
						if(nDirectionalLightCount >= RenderInfo.m_nMaxDirectionalLights)
							break;

						GameObject obj = light.GetGameObject();
						m_RenderInfo.m_av3DirectionalLightDirections[nDirectionalLightCount] = obj.GetForward();
						m_RenderInfo.m_av3DirectionalLightColours[nDirectionalLightCount] = light.GetColorV3();
						m_RenderInfo.m_afDirectionalLightIntensity[nDirectionalLightCount] = light.GetIntensity();

						++nDirectionalLightCount;
					}
					break;

					case ELightType.Point:
					{
						if(nPointLightCount >= RenderInfo.m_nMaxPointLights)
							break;

						GameObject obj = light.GetGameObject();
						m_RenderInfo.m_av3PointLightPositions[nPointLightCount] = obj.GetLocalPosition();
						m_RenderInfo.m_av3PointLightColours[nPointLightCount] = light.GetColorV3();
						m_RenderInfo.m_afPointLightIntensity[nPointLightCount] = light.GetIntensity();
						m_RenderInfo.m_afPointLightRange[nPointLightCount] = light.GetRange();

						++nPointLightCount;
					}
					break;
				}


			}

			//Render the shadow maps
			DrawShadowMaps();

			//Render meshes
			DrawOpaquePass();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void DrawOpaquePass()
		{
			m_Graphics.Clear(Color.Black);
			foreach(MeshRenderer renderer in m_OpaqueList)
			{
				renderer.Draw(m_RenderInfo);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void DrawShadowMaps()
		{
			bool bDirectionalShadowsRendered = false;
			int nPointLightCount = 0;

			for(int i = 0; i < m_LightList.Count; ++i)
			{
				Light light = m_LightList[i];
				if(!light.GetCastShadows())
					continue;

				ELightType eLightType = light.GetLightType();
				switch(eLightType)
				{
					case ELightType.Directional:
					{
						if(!bDirectionalShadowsRendered)
						{
							DrawDirectionalShadowMap(light);
							m_RenderInfo.m_nDirecitonalShadowIndex = i;
							bDirectionalShadowsRendered = true;
						}
					}
					break;

					case ELightType.Point:
					{
						if(nPointLightCount < RenderInfo.m_nMaxPointShadowMaps)
						{
							DrawOmnidirectionalShadowMap(light, nPointLightCount);
							++nPointLightCount;
						}
						
					}
					break;
				}
			}
			m_RenderInfo.m_PointShadowMaps = m_PointShadowMap;
		}

		//----------------------------------------------------------------------------------
		// Draw the shadow map for the primary directional light
		// We only support shadows on one directional light.
		//----------------------------------------------------------------------------------
		private void DrawDirectionalShadowMap(Light light)
		{
			m_Graphics.SetRenderTarget(m_DirectionalShadowMap);
			m_Graphics.Clear(Color.Black);

			Matrix m44View = light.GetViewMatrix();
			Matrix m44Projection = light.GetProjectionMatrix();
			Matrix m44LightViewProjection = m44View * m44Projection;

			m_ShadowMaterial.SetProperty("_LightViewProjection", m44LightViewProjection);

			//Render meshes
			foreach(MeshRenderer renderer in m_OpaqueList)
			{
				renderer.DrawShadows(m_ShadowMaterial);
			}

			m_Graphics.SetRenderTarget(null);

			//Store shadow map details in the render info for the colour pass
			m_RenderInfo.m_DirectionalShadowMap = m_DirectionalShadowMap;
			m_RenderInfo.m_m44LightViewProjection = m44LightViewProjection;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void DrawOmnidirectionalShadowMap(Light light, int nLightIndex)
		{
			//Six faces of the cube map
			for(int i = 0; i < RenderInfo.m_nCubeSides; ++i)
			{
				int nTextureIndex = nLightIndex * RenderInfo.m_nCubeSides + i;

				m_Graphics.SetRenderTarget(m_PointShadowMap, nTextureIndex);
				m_Graphics.Clear(Color.Black);

				Matrix m44View = light.GetViewMatrix((CubeMapFace)i);
				Matrix m44Projection = light.GetProjectionMatrix();
				Matrix m44LightViewProjection = m44View * m44Projection;

				m_ShadowMaterial.SetProperty("_LightViewProjection", m44LightViewProjection);

				//Render meshes
				foreach(MeshRenderer renderer in m_OpaqueList)
				{
					renderer.DrawShadows(m_ShadowMaterial);
				}

				m_Graphics.SetRenderTarget(null);
				m_RenderInfo.m_m44PointLightViewProjection[nTextureIndex] = m44LightViewProjection;
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetVSync(bool bEnabled)
		{
			m_GraphicsDeviceManager.SynchronizeWithVerticalRetrace = bEnabled;
			m_Game.IsFixedTimeStep = bEnabled;
			m_GraphicsDeviceManager.ApplyChanges();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetResolution(int nWidth, int nHeight)
		{
			m_GraphicsDeviceManager.PreferredBackBufferWidth = nWidth;
			m_GraphicsDeviceManager.PreferredBackBufferHeight = nHeight;
			m_GraphicsDeviceManager.ApplyChanges();
		}
	}
}
