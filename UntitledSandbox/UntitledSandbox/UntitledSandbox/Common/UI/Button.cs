using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UntitledSandbox.Managers;
using Microsoft.Xna.Framework.Graphics;
using UntitledSandbox.Common.Utils;

namespace UntitledSandbox.Common.UI
{
	public class Button : Component
	{
		public string CustomImage { get; set; }
		public string Text { get; set; }
		public Color Color { get; set; }
		
		public Button(Vector2 position, Vector2 size, string name="Button") : base(position, size, name)
		{
			this.CustomImage = "";
			this.Color = Color.White;
		}

		public Button() : this(Vector2.Zero, new Vector2(32, 16))
		{
		}

		public override void Draw()
		{
			Vector2 pos = Parent == null ? this.Position : this.Parent.Position + this.Position;

			if (CustomImage.Length > 0)
			{
				Texture2D buttonTexture = this.ContentManager.Get<Texture2D>(CustomImage);
				Rectangle button = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y);
				this.SpriteBatch.Draw(buttonTexture, button, Color);
			}
			else
			{
				Rectangle background = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X, (int)Size.Y);

				Rectangle borderTop = new Rectangle((int)pos.X, (int)pos.Y, (int)Size.X + 1, 1);
				Rectangle borderBottom = new Rectangle((int)pos.X, (int)(pos.Y + Size.Y), (int)Size.X + 1, 1);
				Rectangle borderLeft = new Rectangle((int)pos.X, (int)pos.Y, 1, (int)Size.Y);
				Rectangle borderRight = new Rectangle((int)(pos.X + Size.X), (int)pos.Y, 1, (int)Size.Y);

				Texture2D windowPixelTexture = this.ContentManager.Get<Texture2D>("textures/ui/windowPixel");

				this.SpriteBatch.Draw(windowPixelTexture, background, Color.Gray);
				this.SpriteBatch.Draw(windowPixelTexture, borderTop, Color.DarkGray);
				this.SpriteBatch.Draw(windowPixelTexture, borderBottom, Color.DarkGray);
				this.SpriteBatch.Draw(windowPixelTexture, borderLeft, Color.DarkGray);
				this.SpriteBatch.Draw(windowPixelTexture, borderRight, Color.DarkGray);

				SpriteFont font = this.ContentManager.Get<SpriteFont>("fonts/windowTitle");
				this.SpriteBatch.DrawString(font, this.Text, pos + new Vector2(this.Size.X / 2, 0) - new Vector2(font.MeasureString(Text).X / 2, 0), Color.LightGray);

			}
		}

		public override void Update()
		{
		}

		public override void UpdateRange()
		{
			Vector2 parentPosition = this.Parent == null ? Vector2.Zero : this.Parent.Position;
			this.Range = new Vector2Range(parentPosition + this.Position, parentPosition + this.Position + this.Size);
		}

		public void setText(string text)
		{
			SpriteFont font = this.ContentManager.Get<SpriteFont>("fonts/windowTitle");
			this.Size.X = font.MeasureString(text).X;
		}
	}
}
