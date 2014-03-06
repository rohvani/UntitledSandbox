using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common.UI
{
	public class UIComponent
	{
		protected Vector2 position { get; set; }
		protected Vector2 size { get; set; }
		protected List<UIComponent> children { get; set; }

		virtual public void Draw() { }
		virtual public void Update() { }

		virtual public void handleClick(Vector2 clickPosition) { }
		virtual public void handleDrag() { }

		virtual public bool containsClick(Vector2 clickPosition)
		{
			if (clickPosition.X >= position.X && clickPosition.X <= (position.X + size.X) &&
				clickPosition.Y >= position.Y && clickPosition.Y <= (position.Y + size.Y)) 
				return true;

			return false;
		}
	}
}
