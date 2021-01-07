using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
public class TestScene : Scene
{
	private Camera m_Camera = null;

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public TestScene()
	{
		CreatePBRTest();
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	private void CreatePBRTest()
	{
		Engine engine = Engine.GetInstance();
		RenderManager renderManager = engine.GetRenderManager();

		//Create camera
		m_Camera = new Camera();
		m_Camera.SetLocalPosition(new Vector3(0, 5, 10.0f));
		m_Camera.SetLocalRotation(Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), -3.14f / 10.0f));
		m_Camera.UpdateMatricies();
		renderManager.SetCamera(m_Camera);

		//Ambient light
		renderManager.SetAmbientLight(new Color(0.05f, 0.05f, 0.05f));

		//Create objects
		PBRTestObject floor = new PBRTestObject("Cube", "PBR/Wood/Wood", new Vector3(0, -1.5f, 0));
		floor.SetLocalScale(new Vector3(20, 0.2f, 20));

		new PBRTestObject("Sphere", "PBR/CircleTexture/CircleTexture", new Vector3(-7.5f, 0.0f, 0.0f));
		new PBRTestObject("Sphere", "PBR/Dirt/Dirt", new Vector3(-4.5f, 0, 0));
		new PBRTestObject("Sphere", "PBR/Plastic/Plastic", new Vector3(-1.5f, 0, 0));
		new PBRTestObject("Sphere", "PBR/RoughMetal/RoughMetal", new Vector3(1.5f, 0, 0));
		new PBRTestObject("Sphere", "PBR/ShinyMetal/ShinyMetal", new Vector3(4.5f, 0, 0));
		new PBRTestObject("Sphere", "PBR/Wood/Wood", new Vector3(7.5f, 0, 0));

		PBRTestObject cube1 = new PBRTestObject("Cube", "PBR/RoughMetal/RoughMetal", new Vector3(1, 1, -6.0f));
		cube1.SetLocalScale(new Vector3(2.0f, 5.0f, 2.0f));

		//Create directional lights
		new DirectionalLight(new Vector3(-0.3f, -1.0f, 0.5f), Color.White, 1.0f);
		new DirectionalLight(new Vector3(-1.0f, -1.0f, -1.0f), Color.Red, 0.1f);

		//Create point lights
		new PointLight(new Vector3(-3.0f, 7.0f, -3.0f), Color.White, 0.2f);
		new PointLight(new Vector3(5.0f, 5.0f, -3), Color.Blue, 0.3f);
		new PointLight(new Vector3(-5.0f, 5.0f, 5), Color.Red, 1.0f);
		new PointLight(new Vector3(-5.0f, 5.0f, -5), Color.Green, 1.0f);

	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public override void Update(float fDeltaTime)
	{
	}
}

