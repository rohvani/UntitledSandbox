using System.Collections.Generic; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common.UI
{
	public class UIPanel : UIComponent
	{
		public string name;

		public UIPanel(Vector2 position, Vector2 size, string name)
		{
			this.position = position;
			this.size = size;
			this.name = name;

			this.children = new List<UIComponent>();

			// add Exit button via UIButton
		}

		public UIPanel(Vector2 position, Vector2 size) : this(position, size, "Window")
		{
		}

		public override void Draw()
		{
			Vector2 temp = position;

			Rectangle background = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
			Rectangle titlebar = new Rectangle((int)position.X, (int)position.Y, (int)size.X, 16); // titlebar is 16 pixels tall

			Rectangle borderleft = new Rectangle((int)position.X, (int)position.Y, 1, (int) size.Y);
			Rectangle borderright = new Rectangle((int)(position.X + size.X), (int)position.Y, 1, (int)size.Y);
			Rectangle borderbottom = new Rectangle((int)position.X, (int)(position.Y + size.Y), (int)size.X + 1, 1);

			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowBackground"), background, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), titlebar, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), borderleft, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), borderright, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), borderbottom, Color.White);

			// render panel name in the center of title bar
		}
	}
}
