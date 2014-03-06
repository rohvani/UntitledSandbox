using System.Collections.Generic; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace UntitledSandbox.Common.UI
{
	public class UIPanel : UIComponent
	{
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

		public override void handleClick(Vector2 clickPosition)
		{
			foreach (UIComponent child in children)
			{
				if (child.containsClick(clickPosition))
				{
					child.handleClick(clickPosition); // Should add some form of action parameter to buttons that can determine what the button does...  perhaps add scripting?
					if (child.name == "exit") Game.Instance.UIManager.unregisterUIComponent(this); // for now, we're going to check the name of a button to determine if it's the exit button...					
					break;
				}
			}
		}

		public override void Draw()
		{
			Rectangle background = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
			Rectangle titlebar = new Rectangle((int)position.X, (int)position.Y, (int)size.X, 16); // titlebar is 16 pixels tall, 'magic number', perhaps move 16 to a static in UIManager incase we add in UI scaling or something

			Rectangle borderleft = new Rectangle((int)position.X, (int)position.Y, 1, (int) size.Y);
			Rectangle borderright = new Rectangle((int)(position.X + size.X), (int)position.Y, 1, (int)size.Y);
			Rectangle borderbottom = new Rectangle((int)position.X, (int)(position.Y + size.Y), (int)size.X + 1, 1);

			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowBackground"), background, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), titlebar, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), borderleft, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), borderright, Color.White);
			Game.Instance.SpriteBatch.Draw(Game.Instance.ContentManager.Get<Texture2D>("textures/ui/windowForeground"), borderbottom, Color.White);

			// [Todo] render panel name/title in the center of title bar
		}
	}
}
