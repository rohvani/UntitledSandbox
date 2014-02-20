using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common
{
	public class CModel
	{
		public Model model;
		public Vector3 position;
		public float rotation;

		public CModel(Model model, Vector3 position, float rotation)
		{
			this.model = model;
			this.position = position;
			this.rotation = rotation;
		}

		public CModel(Model model, Vector3 position)
		{
			this.model = model;
			this.position = position;
			this.rotation = 0f;
		}
	}
}
