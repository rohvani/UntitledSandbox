using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace UntitledSandbox.Managers
{
	public class UIManager
	{
		private Game instance;

		public UIManager()
		{
			this.instance = Game.Instance;
		}

		public void drawText(Vector2 position, string text, Color color)
		{
			
		}

		public void drawText(Vector2 position, string text)
		{
			this.drawText(position, text, Color.White);
		}
	}
}
