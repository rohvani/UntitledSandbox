using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UntitledSandbox.Common;
using UntitledSandbox.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Terrain.Renderers
{
	public class BlockTerrainRenderer : Renderer
	{
		public const string BLOCK_MODEL = "models/cube";
		public const int MAP_SIZE = 50 * 2;

		private List<CModel> CModels { get; set; }

		public BlockTerrainRenderer()
		{
			this.CModels = new List<CModel>();
		}

		public override void Load()
		{
			Model cubeModel = this.ContentManager.Load<Model>(BLOCK_MODEL);

			// Create a 5x5 map of cubes
			for (int x = -MAP_SIZE; x < MAP_SIZE; x += 2)
			{
				for (int z = -MAP_SIZE; z < MAP_SIZE; z += 2)
				{
					this.CModels.Add(new CModel(cubeModel, new Vector3(x, -5, z)));
				}
			}
		}

		public override void Draw()
		{
			this.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

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
						effect.AmbientLightColor = new Vector3(0.6f, 0.6f, 0.6f);
						effect.DirectionalLight0.Direction = new Vector3(0f, -1f, 0f);
					}

					if (this.Player.Camera.Frustum.Contains(sphere) != ContainmentType.Disjoint)
						mesh.Draw();
				}
			}
		}
	}
}
