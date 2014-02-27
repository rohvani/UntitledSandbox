using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using UntitledSandbox.Common;
using UntitledSandbox.PlayerData;
using UntitledSandbox.Managers;

namespace UntitledSandbox
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		public static Game Instance { get; private set; }

		public SpriteBatch SpriteBatch { get; private set; }
		public ContentManager ContentManager { get; private set; }
		public UIManager UIManager { get; private set; }

		public GraphicsDeviceManager Graphics { get; set; }
		public Player Player { get; set; }

		private List<CModel> CModels { get; set; }
		
		public Game()
		{
			Instance = this;

			this.CModels = new List<CModel>();
			this.Graphics = new GraphicsDeviceManager(this);

			Content.RootDirectory = "Content";
		}

		// TODO Look into GameServiceContainer
		protected override void Initialize()
		{
			// [StartupLogic] Enable Window Resizing
			this.Window.AllowUserResizing = true;
			this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

			// [StartupLogic] Initialize Managers
			this.ContentManager = new ContentManager();
			this.UIManager = new UIManager();

			// [StartupLogic] Graphic Devices & Settings
			this.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
			this.SpriteBatch = new SpriteBatch(GraphicsDevice);

			// [StartupLogic] Create Player & Update
			Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
			this.Player = new Player();

			this.Player.Camera.ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -100), Vector3.Zero, Vector3.Up);
			this.Player.Camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.3f, 1000.0f);

			// [XNA]
			base.Initialize();
		}

		protected override void LoadContent()
		{
			// [ContentLogic] Load 3D Content
			Model cubeModel = ContentManager.GetModel("models/cube"); // String literals for constants leave a bad taste in my mouth
			
			// [WorldLogic] Create a 5x5 map of cubes
			for (int x = 0; x < (50 * 2); x += 2)
			{
				for (int z = 0; z < (50 * 2); z += 2)
				{
					CModels.Add(new CModel(cubeModel, new Vector3(x, 0, z)));
				}
			}
		}

		protected override void UnloadContent()
		{
			// [[ExitLogic] Could save here in the future
		}

		protected override void Update(GameTime gameTime)
		{
			// [GameLogic] Global Controls
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) this.Exit();
			if (Keyboard.GetState().IsKeyDown(Keys.Space)) this.Exit();

			// [GameLogic] Player Controls
			this.Player.Controls.ProcessInput((float) gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);
			
			// [GameLogic]
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Transparent);

			Matrix world;
			BoundingSphere sphere;

			foreach (CModel gameObj in this.CModels)
			{
				foreach (ModelMesh mesh in gameObj.Model.Meshes)
				{
					world = gameObj.Transforms[mesh.ParentBone.Index]
							* Matrix.CreateRotationY(gameObj.Rotation)
							* Matrix.CreateTranslation(gameObj.Position);

					sphere = mesh.BoundingSphere.Transform(world);
					
					foreach (BasicEffect effect in mesh.Effects)
					{	
						effect.World = world;

						effect.View = this.Player.Camera.ViewMatrix;

						effect.Projection = this.Player.Camera.ProjectionMatrix;

						// Placeholder lighting
						effect.LightingEnabled = true;
						effect.AmbientLightColor = new Vector3(0.25f, 0.25f, 0.25f);
						effect.DirectionalLight0.Direction = new Vector3(0f, -1f, 0f);
					}

					if (this.Player.Camera.Frustum.Contains(sphere) != ContainmentType.Disjoint) 
						mesh.Draw();
				}
			}
			base.Draw(gameTime);
		}

		void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			this.Player.UpdateMouseState();
		}

		public static void CenterMouse()
		{
			Mouse.SetPosition(Instance.GraphicsDevice.Viewport.Width / 2, Instance.GraphicsDevice.Viewport.Height / 2);
		}
	}
}
