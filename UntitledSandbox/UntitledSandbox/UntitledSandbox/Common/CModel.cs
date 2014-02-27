using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common
{
	public class CModel
	{
		public Model Model { get; private set; }
		public Vector3 Position { get; private set; }
		public float Rotation { get; private set; }
		public Matrix[] Transforms { get; private set; }

		public CModel(Model model, Vector3 position, float rotation)
		{
			this.Model = model;
			this.Position = position;
			this.Rotation = rotation;

			this.Transforms = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(this.Transforms);
		}

		public CModel(Model model, Vector3 position) : this(model, position, 0)
		{
		}
	}
}
