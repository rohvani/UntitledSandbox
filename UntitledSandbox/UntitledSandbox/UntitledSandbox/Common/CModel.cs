using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common
{
	public class CModel
	{
		public Model Model
		{
			get { return this._model; }
		}

		public Vector3 Position
		{
			get { return this._position; }
		}

		public float Rotation
		{
			get { return this._rotation; }
		}

		private Model _model;
		private Vector3 _position;
		private float _rotation;

		public CModel(Model model, Vector3 position, float rotation)
		{
			this._model = model;
			this._position = position;
			this._rotation = rotation;
		}

		public CModel(Model model, Vector3 position) : this(model, position, 0)
		{
		}
	}
}
