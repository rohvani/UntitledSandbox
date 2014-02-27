using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace UntitledSandbox.Managers
{
	public class UIManager
	{
		public UIManager()
		{
		}

		public void DrawText(Vector2 position, string text, Color color)
		{
			
		}

		public void DrawText(Vector2 position, string text)
		{
			this.DrawText(position, text, Color.White);
		}
	}
}
