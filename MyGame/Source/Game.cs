using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
public class Game : Microsoft.Xna.Framework.Game
{
	private Engine m_Engine = null;
	private RenderManager m_RenderManager = null;

	private Scene m_CurrentScene = null;

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public Game()
	{
		m_Engine = Engine.Create(this);
		
		m_RenderManager = m_Engine.GetRenderManager();
		m_RenderManager.SetVSync(false);
		m_RenderManager.SetResolution(1440, 810);

		Content.RootDirectory = @"Content\Assets";
		IsMouseVisible = true;
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	protected override void Initialize()
	{
		// TODO: Add your initialization logic here
		m_RenderManager.InitGraphicsDevice();

		base.Initialize();
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	protected override void LoadContent()
	{
		//m_spriteBatch = new SpriteBatch(GraphicsDevice);

		// TODO: use this.Content to load your game content here
		m_CurrentScene = new TestScene();
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	protected override void Update(GameTime gameTime)
	{
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		//Early update: Update systems before the common game logic
		float fDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
		m_Engine.EarlyUpdate();

		//Update
		m_CurrentScene.Update(fDeltaTime);
		base.Update(gameTime);

		//Late update: Update things that need to be after all common game logic
		m_Engine.LateUpdate();
	}

	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	protected override void Draw(GameTime gameTime)
	{
		// TODO: Add your drawing code here

		m_RenderManager.Draw();

		base.Draw(gameTime);
	}
}

