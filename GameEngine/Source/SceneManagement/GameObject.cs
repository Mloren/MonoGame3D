using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public class GameObject : GraphNode
	{
		private Dictionary<Type, Component> m_ComponentDictionary = new Dictionary<Type, Component>();

		private Matrix m_m44LocalTransform = Matrix.Identity;

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public GameObject()
		{
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public T AddComponent<T>() where T : Component, new()
		{
			T component = new T();
			Type type = typeof(T);

			if(Assert.True(!m_ComponentDictionary.ContainsKey(type), "GameObject already contains component of type", type.ToString()))
			{
				component.SetGameObject(this);
				m_ComponentDictionary.Add(type, component);

				return component;
			}

			return null;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void RemoveComponent<T>() where T : Component
		{
			Type type = typeof(T);

			if(Assert.True(m_ComponentDictionary.ContainsKey(type), "GameObject doesn't contain component of type", type.ToString()))
			{
				m_ComponentDictionary.Remove(type);
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public T GetComponent<T>() where T : Component
		{
			Type type = typeof(T);

			if(m_ComponentDictionary.ContainsKey(type))
				return (T)m_ComponentDictionary[type];

			return null;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Matrix GetLocalMatrix()
		{
			return m_m44LocalTransform;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Vector3 GetLocalPosition()
		{
			return m_m44LocalTransform.Translation;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetLocalPosition(Vector3 v3Position)
		{
			m_m44LocalTransform.Translation = v3Position;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Quaternion GetLocalRotation()
		{
			if(m_m44LocalTransform.Decompose(out Vector3 v3Scale, out Quaternion qRotation, out Vector3 v3Translation))
				return qRotation;

			return Quaternion.Identity;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetLocalRotation(Quaternion qRotation)
		{
			if(m_m44LocalTransform.Decompose(out Vector3 v3Scale, out Quaternion qOldRotation, out Vector3 v3Translation))
			{
				Matrix m44Scale = Matrix.CreateScale(v3Scale);
				Matrix m44Rotation = Matrix.CreateFromQuaternion(qRotation);
				Matrix m44Translation = Matrix.CreateTranslation(v3Translation);

				m_m44LocalTransform = m44Scale * m44Rotation * m44Translation;
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Vector3 GetLocalScale()
		{
			if(m_m44LocalTransform.Decompose(out Vector3 v3Scale, out Quaternion qRotation, out Vector3 v3Translation))
				return v3Scale;

			return Vector3.One;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetLocalScale(Vector3 v3Scale)
		{
			if(m_m44LocalTransform.Decompose(out Vector3 v3OldScale, out Quaternion qRotation, out Vector3 v3Translation))
			{
				Matrix m44Scale = Matrix.CreateScale(v3Scale);
				Matrix m44Rotation = Matrix.CreateFromQuaternion(qRotation);
				Matrix m44Translation = Matrix.CreateTranslation(v3Translation);

				m_m44LocalTransform = m44Scale * m44Rotation * m44Translation;
			}
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public Vector3 GetForward()
		{
			return m_m44LocalTransform.Forward;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public void SetForward(Vector3 v3Direction)
		{
			v3Direction.Normalize();
			m_m44LocalTransform.Forward = v3Direction;
		}
	}
}
