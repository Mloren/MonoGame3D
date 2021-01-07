using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class Texture
	{
		private Microsoft.Xna.Framework.Graphics.Texture2D m_Texture = null;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Texture(string szTextureName)
		{
			Engine engine = Engine.GetInstance();
			AssetManager assetManager = engine.GetAssetManager();

			m_Texture = assetManager.GetAsset<Microsoft.Xna.Framework.Graphics.Texture2D>(szTextureName);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Microsoft.Xna.Framework.Graphics.Texture2D GetTexture()
		{
			return m_Texture;
		}
	}
}
