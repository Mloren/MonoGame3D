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
	public class DirectionalLight : GameObject
	{
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public DirectionalLight(Vector3 v3Direction, Color color, float fIntensity)
		{
			Light light = AddComponent<Light>();
			light.SetLightType(ELightType.Directional);
			light.SetColor(color);
			light.SetIntensity(fIntensity);
			SetForward(v3Direction);
		}
	}
}
