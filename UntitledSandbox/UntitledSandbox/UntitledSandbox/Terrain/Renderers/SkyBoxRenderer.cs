using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UntitledSandbox.PlayerData;

namespace UntitledSandbox.Terrain.Renderers
{
	public class SkyBoxRenderer : Renderer
	{
		//public const string SKY_BOX_TEXTURE = "textures/Sunset";
		public const string SKY_BOX_TEXTURE = "textures/SkyBox";
		public const string SKY_BOX_EFFECT = "effects/SkyBox";
		public const string SKY_BOX_MODEL = "models/cube";

		private const float SIZE = 100f;

		public override void Load()
		{
			TextureCube skyBoxTexture = this.ContentManager.Load<TextureCube>(SKY_BOX_TEXTURE);

			Effect skyBoxEffect = this.ContentManager.Load<Effect>(SKY_BOX_EFFECT);
			skyBoxEffect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);

			Model skyBox = this.ContentManager.Load<Model>(SKY_BOX_MODEL);
			foreach (ModelMesh mesh in skyBox.Meshes)
			{
				foreach (ModelMeshPart part in mesh.MeshParts)
				{
					part.Effect = skyBoxEffect;
				}
			}
		}

		public override void Draw()
		{
			this.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

			Model skyBox = this.ContentManager.Get<Model>(SKY_BOX_MODEL);
			Effect skyBoxEffect = this.ContentManager.Get<Effect>(SKY_BOX_EFFECT);

			// Go through each pass in the effect, but we know there is only one...
			foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
			{
				// Draw all of the components of the mesh, but we know the cube really
				// only has one mesh
				foreach (ModelMesh mesh in skyBox.Meshes)
				{
					// Assign the appropriate values to each of the parameters
					foreach (ModelMeshPart part in mesh.MeshParts)
					{
						part.Effect.Parameters["World"].SetValue(Matrix.CreateScale(SIZE) * Matrix.CreateTranslation(this.Player.Camera.Position));
						part.Effect.Parameters["View"].SetValue(this.Player.Camera.ViewMatrix);
						part.Effect.Parameters["Projection"].SetValue(this.Player.Camera.ProjectionMatrix);
						part.Effect.Parameters["CameraPosition"].SetValue(this.Player.Camera.Position);
					}

					// Draw the mesh with the skybox effect
					mesh.Draw();
				}
			}

			this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}
	}
}
