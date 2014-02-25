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
		private List<CModel> objectList = new List<CModel>();
		public Player player;

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
			//GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			this.aspectRatio = this.graphics.GraphicsDevice.Viewport.AspectRatio;

			// [ContentLogic] Load 3D Content
			Model cubeModel = Content.Load<Model>("cube");
			
			// [WorldLogic] Create a 5x5 map of cubes
			for (int x = 0; x < (100 * 2); x += 2)
			{
				for (int z = 0; z < (100 * 2); z += 2)
				{
					objectList.Add(new CModel(cubeModel, new Vector3(x, 0, z)));
				}
			}

			// [StartupLogic] Create Player & Update
			Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
			this.player = new Player();

			this.player.Camera.ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -100), Vector3.Zero, Vector3.Up);
			this.player.Camera.ProjectionMatrix 
				= Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, this.aspectRatio, 0.3f, 1000.0f);
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
			this.player.Controls.ProcessInput((float) gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);
			
			// [GameLogic]
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Transparent);

			Matrix world;
			BoundingSphere sphere;

			foreach (CModel gameObj in this.objectList)
			{
				foreach (ModelMesh mesh in gameObj.Model.Meshes)
				{
					world = gameObj.Transforms[mesh.ParentBone.Index]
							* Matrix.CreateRotationY(gameObj.Rotation)
							* Matrix.CreateTranslation(gameObj.Position);

					sphere = gameObj.Model.Meshes[0].BoundingSphere.Transform(world);
					
					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.EnableDefaultLighting();
						
						effect.World = world;

						effect.View = this.player.Camera.ViewMatrix;

						effect.Projection = this.player.Camera.ProjectionMatrix;
					}

					if (this.player.Camera.Frustum.Contains(sphere) != ContainmentType.Disjoint) 
						mesh.Draw();
				}
			}
			base.Draw(gameTime);
		}
	}
}
