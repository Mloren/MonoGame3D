using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class PointLight : GameObject
	{
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public PointLight(Vector3 v3Pos, Color color, float fIntensity)
		{
			Light light = AddComponent<Light>();
			light.SetLightType(ELightType.Point);
			light.SetColor(color);
			light.SetIntensity(fIntensity);
			SetLocalPosition(v3Pos);
		}
	}
}
