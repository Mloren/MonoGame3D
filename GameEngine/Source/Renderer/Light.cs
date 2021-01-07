using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	public enum ELightType
	{
		Directional,
		Point
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class Light : Component
	{
		private Color m_Color = Color.White;
		private float m_fIntensity = 5.0f;
		private float m_fRange = 100.0f;
		private ELightType m_eLightType = ELightType.Point;
		private bool m_bCastShadows = true;

		private Matrix m_m44Projection;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Light()
		{
			Engine engine = Engine.GetInstance();
			RenderManager renderManager = engine.GetRenderManager();

			renderManager.AddLight(this);

			CalculateProjection();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		private void CalculateProjection()
		{
			switch(m_eLightType)
			{
				case ELightType.Directional:
					m_m44Projection = Matrix.CreateOrthographic(50, 50, 1.0f, 100.0f);
					break;

				case ELightType.Point:
					m_m44Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90.0f), 1.0f, 1.0f, 100.0f);
					break;
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetColor(Color color)
		{
			m_Color = color;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Color GetColor()
		{
			return m_Color;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Vector3 GetColorV3()
		{
			return m_Color.ToVector3();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetIntensity(float fIntensity)
		{
			m_fIntensity = fIntensity;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public float GetIntensity()
		{
			return m_fIntensity;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetRange(float fRange)
		{
			m_fRange = fRange;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public float GetRange()
		{
			return m_fRange;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public bool GetCastShadows()
		{
			return m_bCastShadows;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetCastShadows(bool bCastShadows)
		{
			m_bCastShadows = bCastShadows;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public ELightType GetLightType()
		{
			return m_eLightType;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetLightType(ELightType eLightType)
		{
			m_eLightType = eLightType;
			
			CalculateProjection();
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Matrix GetViewMatrix(CubeMapFace eFace = 0)
		{
			GameObject obj = GetGameObject();

			switch(m_eLightType)
			{
				//Point light
				case ELightType.Point:
				{
					Vector3 v3Pos = obj.GetLocalPosition();

					switch(eFace)
					{
						case CubeMapFace.PositiveX:		return Matrix.CreateLookAt(v3Pos, v3Pos + Vector3.Right, Vector3.Up);
						case CubeMapFace.NegativeX:		return Matrix.CreateLookAt(v3Pos, v3Pos + Vector3.Left, Vector3.Up);
						case CubeMapFace.PositiveY:		return Matrix.CreateLookAt(v3Pos, v3Pos + Vector3.Up, Vector3.Backward);
						case CubeMapFace.NegativeY:		return Matrix.CreateLookAt(v3Pos, v3Pos + Vector3.Down, Vector3.Forward);
						case CubeMapFace.PositiveZ:		return Matrix.CreateLookAt(v3Pos, v3Pos + Vector3.Backward, Vector3.Down);
						case CubeMapFace.NegativeZ:		return Matrix.CreateLookAt(v3Pos, v3Pos + Vector3.Forward, Vector3.Up);
						default:						return Matrix.Identity;
					}
				}

				//Directional light
				default:
				{
					Vector3 v3Forward = obj.GetForward();
					Vector3 v3Up = Vector3.Up;
					Vector3 v3Pos = -obj.GetForward() * 30.0f;

					//Change Up vector if forward is straight up or down
					if(v3Forward.X == 0.0f && v3Forward.Z == 0.0f && (v3Forward.Y == 1.0f || v3Forward.Y == -1.0f))
						v3Up = Vector3.Forward;

					Matrix m44View = Matrix.CreateLookAt(v3Pos, v3Pos + obj.GetForward(), v3Up);

					return m44View;
				}
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Matrix GetProjectionMatrix()
		{
			return m_m44Projection;
		}
	}
}
