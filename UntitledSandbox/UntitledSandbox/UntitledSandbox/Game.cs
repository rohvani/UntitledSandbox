using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using UntitledSandbox.Common;
using UntitledSandbox.PlayerData;

namespace UntitledSandbox
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		private static Game _instance;

		public static Game Instance
		{
			get { return _instance; }
		}

		public GraphicsDeviceManager graphics;

		private SpriteBatch spriteBatch;
		private float aspectRatio;
		private double lastUpdate = 0;
		private List<CModel> objectList = new List<CModel>();
		private Player player;

		public Game()
		{
			_instance = this;

			this.graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			// [ContentLogic] Graphic Devices & Settings
			this.spriteBatch = new SpriteBatch(GraphicsDevice);
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			this.aspectRatio = this.graphics.GraphicsDevice.Viewport.AspectRatio;

			// [ContentLogic] Load 3D Content
			Model cubeModel = Content.Load<Model>("cube");
			
			// [WorldLogic] Create a 5x5 map of cubes
			for (int x = 0; x < (5 * 2); x += 2)
			{
				for (int z = 0; z < (5 * 2); z += 2)
				{
					objectList.Add(new CModel(cubeModel, new Vector3(x, 0, z)));
				}
			}

			// [StartupLogic] Create Player & Update
			Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
			this.player = new Player();

			this.player.Camera.UpdateViewMatrix();
			this.player.Camera.ProjectionMatrix 
				= Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
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
			this.lastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
			if (this.lastUpdate >= 10)
			{
				float timeDifference = (float) this.lastUpdate / 1000.0f;
				this.player.Controls.ProcessInput(timeDifference);
				this.lastUpdate = 0;
			}

			// [GameLogic]
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Transparent);

			foreach (CModel gameObj in this.objectList)
			{
				// Draw the model. A model can have multiple meshes, so loop.
				foreach (ModelMesh mesh in gameObj.Model.Meshes)
				{
					Matrix[] transforms = new Matrix[gameObj.Model.Bones.Count];
					gameObj.Model.CopyAbsoluteBoneTransformsTo(transforms);

					// This is where the mesh orientation is set, as well 
					// as our camera and projection.
					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.EnableDefaultLighting();

						effect.World = transforms[mesh.ParentBone.Index] 
							* Matrix.CreateRotationY(gameObj.Rotation) 
							* Matrix.CreateTranslation(gameObj.Position);

						effect.View = this.player.Camera.ViewMatrix;

						effect.Projection 
							= Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), this.aspectRatio, 1.0f, 10000.0f);
					}
					// Draw the mesh, using the effects set above.
					mesh.Draw();
				}
			}
			base.Draw(gameTime);
		}
	}
}
