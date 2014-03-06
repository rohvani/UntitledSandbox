using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace UntitledSandbox.Common.UI
{
	public class UIComponent
	{
		public string name { get; set; }

		protected Vector2 position { get; set; }
		protected Vector2 size { get; set; }
		protected UIComponent parent { get; set; }
		protected List<UIComponent> children { get; set; }

		virtual public void Draw() { }
		virtual public void Update() { }

		virtual public void handleClick(Vector2 clickPosition) { }
		virtual public void handleDrag() { }

		virtual public bool containsClick(Vector2 clickPosition)
		{
			if (clickPosition.X >= position.X && clickPosition.X <= (position.X + size.X) &&
				clickPosition.Y >= position.Y && clickPosition.Y <= (position.Y + size.Y))
			{
				Console.WriteLine("[UIPanel] Detected click");
				return true;
			}
			Console.WriteLine("[UIPanel] Not click");
			return false;
		}
	}
}
