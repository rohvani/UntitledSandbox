using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UntitledSandbox.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Terrain.Renderers
{
	public class SkySphereRenderer : Renderer
	{
		public const string SKY_SPHERE_TEXTURE = "textures/uffizi_cross";
		public const string SKY_SPHERE_EFFECT = "effects/SkySphere";
		public const string SKY_SPHERE_MODEL = "models/SphereHighPoly";

		public override void Load()
		{
			TextureCube skySphereTexture = this.ContentManager.Load<TextureCube>(SKY_SPHERE_TEXTURE);

			Effect skySphereEffect = this.ContentManager.Load<Effect>(SKY_SPHERE_EFFECT);
			skySphereEffect.Parameters["ViewMatrix"].SetValue(this.Player.Camera.ViewMatrix);
			skySphereEffect.Parameters["ProjectionMatrix"].SetValue(this.Player.Camera.ProjectionMatrix);
			skySphereEffect.Parameters["SkyboxTexture"].SetValue(skySphereTexture);

			Model skySphere = this.ContentManager.Load<Model>(SKY_SPHERE_MODEL);
			foreach (ModelMesh mesh in skySphere.Meshes)
			{
				foreach (ModelMeshPart part in mesh.MeshParts)
				{
					part.Effect = skySphereEffect;
				}
			}
		}

		public override void Draw()
		{
			Model skySphere = this.ContentManager.Get<Model>(SKY_SPHERE_MODEL);
			Effect skySphereEffect = this.ContentManager.Get<Effect>(SKY_SPHERE_EFFECT);

			// Set the View and Projection matrix for the effect
			skySphereEffect.Parameters["ViewMatrix"].SetValue(this.Player.Camera.ViewMatrix);
			skySphereEffect.Parameters["ProjectionMatrix"].SetValue(this.Player.Camera.ProjectionMatrix);

			// Draw the sphere model that the effect projects onto
			foreach (ModelMesh mesh in skySphere.Meshes)
			{
				mesh.Draw();
			}

			// Undo the renderstate settings from the shader
			this.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}
	}
}
