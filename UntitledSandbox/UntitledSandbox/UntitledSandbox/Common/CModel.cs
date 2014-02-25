using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common
{
	public class CModel
	{
		public Model Model { get { return this._model; } }
		public Vector3 Position { get { return this._position; } }
		public float Rotation { get { return this._rotation; } }
		public Matrix[] Transforms { get { return this._transforms; } }

		private Model _model;
		private Vector3 _position;
		private float _rotation;
		private Matrix[] _transforms;

		public CModel(Model model, Vector3 position, float rotation)
		{
			this._model = model;
			this._position = position;
			this._rotation = rotation;

			this._transforms = new Matrix[this.Model.Bones.Count];
			this.Model.CopyAbsoluteBoneTransformsTo(this._transforms);
		}

		public CModel(Model model, Vector3 position) : this(model, position, 0)
		{
		}
	}
}
