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
	public class MeshRenderer : Component
	{
		private Model m_Model = null;
		private Material m_Material = null;
		private ERenderQueue m_eRenderQueue = ERenderQueue.Opaque;
		private int m_nSortOrder = -1;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public MeshRenderer()
		{
			Engine engine = Engine.GetInstance();
			RenderManager renderManager = engine.GetRenderManager();

			renderManager.AddRenderer(this);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetModel(Model model)
		{
			m_Model = model;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Model GetModel()
		{
			return m_Model;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetMaterial(Material material)
		{
			m_Material = material;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Material GetMaterial()
		{
			return m_Material;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetSortOrder(int nOrder)
		{
			m_nSortOrder = nOrder;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public int GetSortOrder()
		{
			return m_nSortOrder;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetRenderQueue(ERenderQueue eRenderQueue)
		{
			Engine engine = Engine.GetInstance();
			RenderManager renderManager = engine.GetRenderManager();

			renderManager.RemoveRenderer(this);

			m_eRenderQueue = eRenderQueue;

			renderManager.AddRenderer(this);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public ERenderQueue GetRenderQueue()
		{
			return m_eRenderQueue;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void Draw(RenderInfo info)
		{
			GameObject oObject = GetGameObject();
			Matrix m44World = oObject.GetLocalMatrix();

			//MVP
			m_Material.SetProperty("_World", m44World);
			m_Material.SetProperty("_View", info.m_m44View);
			m_Material.SetProperty("_Projection", info.m_m44Projection);

			m_Material.SetProperty("_CameraPos", info.m_v3CameraPos);

			//Lights
			m_Material.SetProperty("_AmbientColor", info.m_v3AmbientLight);

			m_Material.SetProperty("_DirectionalLights", info.m_av3DirectionalLightDirections);
			m_Material.SetProperty("_DirectionalColors", info.m_av3DirectionalLightColours);
			m_Material.SetProperty("_DirectionalIntensity", info.m_afDirectionalLightIntensity);

			m_Material.SetProperty("_PointLightPos", info.m_av3PointLightPositions);
			m_Material.SetProperty("_PointLightColors", info.m_av3PointLightColours);
			m_Material.SetProperty("_PointLightIntensity", info.m_afPointLightIntensity);
			m_Material.SetProperty("_PointLightRange", info.m_afPointLightRange);

			//Shadows
			m_Material.SetProperty("_DirectionalShadowMap", info.m_DirectionalShadowMap);
			m_Material.SetProperty("_LightSpaceMatrix", info.m_m44LightViewProjection);
			m_Material.SetProperty("_ShadowMapSize", info.m_nShadowMapSize);
			m_Material.SetProperty("_DepthBias", info.m_fShadowDepthBias);
			m_Material.SetProperty("_DirectionalShadowIndex", info.m_nDirecitonalShadowIndex);
			m_Material.SetProperty("_PointLightSpaceMatrix", info.m_m44PointLightViewProjection);
			m_Material.SetProperty("_PointShadowMap", info.m_PointShadowMaps);

			m_Model.SetMaterial(m_Material);
			var modelData = m_Model.GetModelData();

			foreach(ModelMesh mesh in modelData.Meshes)
			{
				mesh.Draw();
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void DrawShadows(Material shadowMaterial)
		{
			GameObject oObject = GetGameObject();
			Matrix m44World = oObject.GetLocalMatrix();

			shadowMaterial.SetProperty("_World", m44World);

			m_Model.SetMaterial(shadowMaterial);
			var modelData = m_Model.GetModelData();

			foreach(ModelMesh mesh in modelData.Meshes)
			{
				mesh.Draw();
			}
		}
	}
}
