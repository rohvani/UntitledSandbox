using System.Collections.Generic; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UntitledSandbox.Managers;
using UntitledSandbox.Common.Utils;

namespace UntitledSandbox.Common.UI
{
	public class Panel : Container
	{
		const int TITLE_BAR_HEIGHT = 16;

		private Vector2Range TitleBarRange { get; set; }

		public Panel(Vector2 position, Vector2 size, string name="Window") : base(position, size, name)
		{
			// add Exit button via UIButton
			this.Dragged += this.HandleDrag;

			this.TitleBarRange = new Vector2Range(this.Position.X, this.Position.Y, this.Position.X + this.Size.X, this.Position.Y + TITLE_BAR_HEIGHT);
		}

		public Panel() : this(Vector2.Zero, Vector2.Zero)
		{
		}

		public override void Draw()
		{
			Rectangle background = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
			Rectangle titleBar = new Rectangle((int) Position.X, (int) Position.Y, (int) Size.X, TITLE_BAR_HEIGHT); // titlebar is 16 pixels tall, 'magic number', perhaps move 16 to a static in UIManager incase we add in UI scaling or something

			Rectangle borderLeft = new Rectangle((int)Position.X, (int)Position.Y, 1, (int) Size.Y);
			Rectangle borderRight = new Rectangle((int)(Position.X + Size.X), (int)Position.Y, 1, (int)Size.Y);
			Rectangle borderBottom = new Rectangle((int)Position.X, (int)(Position.Y + Size.Y), (int)Size.X + 1, 1);

			Texture2D windowPixelTexture = this.ContentManager.Get<Texture2D>("textures/ui/windowPixel");

			this.SpriteBatch.Draw(windowPixelTexture, background, Color.Gray);

			this.SpriteBatch.Draw(windowPixelTexture, titleBar, Color.DarkGray);
			this.SpriteBatch.Draw(windowPixelTexture, borderLeft, Color.DarkGray);
			this.SpriteBatch.Draw(windowPixelTexture, borderRight, Color.DarkGray);
			this.SpriteBatch.Draw(windowPixelTexture, borderBottom, Color.DarkGray);

			SpriteFont font = this.ContentManager.Get<SpriteFont>("fonts/windowTitle");
			this.SpriteBatch.DrawString(font, this.Name, this.Position + new Vector2(this.Size.X / 2, 0) - new Vector2(font.MeasureString(Name).X / 2, 0), Color.LightGray);

			foreach (Component child in Children) child.Draw();
		}

		public override void Update()
		{

		}

		public void HandleDrag(object sender, DragEventArgs args)
		{
			if (this.TitleBarRange.Contains(args.From) || UIManager.IsDragging)
				this.Position += (args.From - args.To);

			this.TitleBarRange = new Vector2Range(this.Position.X, this.Position.Y, this.Position.X + this.Size.X, this.Position.Y + TITLE_BAR_HEIGHT);
		}
	}
}
