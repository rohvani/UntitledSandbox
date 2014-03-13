using System.Collections.Generic; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UntitledSandbox.Managers;

namespace UntitledSandbox.Common.UI
{
	public class Panel : Container
	{
		public Panel(Vector2 position, Vector2 size, string name="Window") : base(position, size, name)
		{
			// add Exit button via UIButton
			this.Dragged += this.HandleDrag;
		}

		public Panel() : this(Vector2.Zero, Vector2.Zero)
		{
		}

		public override void Draw()
		{
			Rectangle background = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
			Rectangle titleBar = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, 16); // titlebar is 16 pixels tall, 'magic number', perhaps move 16 to a static in UIManager incase we add in UI scaling or something

			Rectangle borderLeft = new Rectangle((int)Position.X, (int)Position.Y, 1, (int) Size.Y);
			Rectangle borderRight = new Rectangle((int)(Position.X + Size.X), (int)Position.Y, 1, (int)Size.Y);
			Rectangle borderBottom = new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)Size.X + 1, 1);

			this.SpriteBatch.Draw(this.ContentManager.Get<Texture2D>("textures/ui/windowBackground"), background, Color.White);
			
			Texture2D windowForegroundTexture = this.ContentManager.Get<Texture2D>("textures/ui/windowForeground");
			this.SpriteBatch.Draw(windowForegroundTexture, titleBar, Color.White);
			this.SpriteBatch.Draw(windowForegroundTexture, borderLeft, Color.White);
			this.SpriteBatch.Draw(windowForegroundTexture, borderRight, Color.White);
			this.SpriteBatch.Draw(windowForegroundTexture, borderBottom, Color.White);

			// [Todo] render panel name/title in the center of title bar
		}

		public override void Update()
		{

		}

		public void HandleDrag(object sender, DragEventArgs args)
		{
			if (args.From.X > this.Position.X && 
				args.From.Y > this.Position.Y && 
				args.From.X < this.Position.X + this.Size.X && 
				args.From.Y < this.Position.Y + 16) // 16 = titlebar height
			{
				this.Position += (args.From - args.To);
			}

			
		}
	}
}
