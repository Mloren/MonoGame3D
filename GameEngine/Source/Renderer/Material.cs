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
	public class Material
	{
		private Effect m_Effect = null;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Material(string szMaterialName)
		{
			Engine engine = Engine.GetInstance();
			AssetManager assetManager = engine.GetAssetManager();

			m_Effect = assetManager.GetAsset<Effect>(szMaterialName);
			m_Effect = m_Effect.Clone();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Effect GetEffect()
		{
			return m_Effect;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, bool bValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(bValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, int nValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(nValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, int[] anValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(anValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, float fValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(fValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, float[] afValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(afValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Vector2 v2Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(v2Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Vector2[] av2Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(av2Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Vector3 v3Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(v3Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Vector3[] av3Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(av3Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Vector4 v4Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(v4Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Vector4[] av4Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(av4Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Quaternion qValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(qValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Matrix m44Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(m44Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Matrix[] am44Value)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(am44Value);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Texture tValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(tValue.GetTexture());
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, Texture2D tValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(tValue);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetProperty(string szPropertyName, RenderTargetCube tValue)
		{
			EffectParameter property = m_Effect.Parameters[szPropertyName];
			if(Assert.Valid(property, "Material property not found:", szPropertyName))
			{
				property.SetValue(tValue);
			}
		}
	}
}
