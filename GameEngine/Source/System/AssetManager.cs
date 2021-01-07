using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class AssetManager
	{
		private ContentManager m_ContentManager = null;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public AssetManager(ContentManager contentManager)
		{
			m_ContentManager = contentManager;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public T GetAsset<T>(string szAssetName) where T : class
		{
			if(!Assert.Valid(m_ContentManager, "ContentManager not ready."))
				return null;

			try
			{
				T asset = m_ContentManager.Load<T>(szAssetName);

				return asset;
			}
			catch(Exception)
			{
				return null;
			}
		}
	}
}
