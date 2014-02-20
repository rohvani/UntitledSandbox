using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using UntitledSandbox.Common;
using UntitledSandbox.Player;

namespace UntitledSandbox
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		public static GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		float aspectRatio;

		double lastUpdate = 0;

		List<CModel> objectList = new List<CModel>();

		Player.Player player;

		public Game()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			// [ContentLogic] Graphic Devices & Settings
			spriteBatch = new SpriteBatch(GraphicsDevice);
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

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
			player = new Player.Player();
			
			player.camera.UpdateViewMatrix();
			player.camera.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.3f, 1000.0f);
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
			Console.WriteLine((string)gameTime.ElapsedGameTime.TotalMilliseconds.ToString());

			//lastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
			//if (lastUpdate >= 10)
			//{
				float timeDifference = (float)lastUpdate / 1000.0f;
				player.controls.ProcessInput(timeDifference);
				//lastUpdate = 0;
			//}

			// [GameLogic]
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Transparent);

			foreach (CModel gameObj in objectList)
			{
				// Draw the model. A model can have multiple meshes, so loop.
				foreach (ModelMesh mesh in gameObj.model.Meshes)
				{
					Matrix[] transforms = new Matrix[gameObj.model.Bones.Count];
					gameObj.model.CopyAbsoluteBoneTransformsTo(transforms);

					// This is where the mesh orientation is set, as well 
					// as our camera and projection.
					foreach (BasicEffect effect in mesh.Effects)
					{
						//effect.EnableDefaultLighting();

						effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(gameObj.rotation) * Matrix.CreateTranslation(gameObj.position);

						effect.View = player.camera.viewMatrix;

						effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 10000.0f);
					}
					// Draw the mesh, using the effects set above.
					mesh.Draw();
				}
			}
			base.Draw(gameTime);
		}
	}
}
