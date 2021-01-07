using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class Model
	{
		private Microsoft.Xna.Framework.Graphics.Model m_Model = null;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Model(string szModelName)
		{
			Engine engine = Engine.GetInstance();
			AssetManager assetManager = engine.GetAssetManager();

			m_Model = assetManager.GetAsset<Microsoft.Xna.Framework.Graphics.Model>(szModelName);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Microsoft.Xna.Framework.Graphics.Model GetModelData()
		{
			return m_Model;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetMaterial(Material material)
		{
			foreach(var mesh in m_Model.Meshes)
			{
				foreach(var part in mesh.MeshParts)
				{
					part.Effect = material.GetEffect();
				}
			}
		}
	}
}
