using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UntitledSandbox.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Common.UI
{
	public class Label : Component
	{
		private string Text;
		private string Font;

		public Label(string text, Vector2 position, string font = "fonts/default")
		{
			this.Position = position;
			this.Size = ContentManager.Get<SpriteFont>(font).MeasureString(text);

			this.Text = text;
			this.Font = font;	
		}

		public Label(string text, Vector2 position, Container parent, string font="fonts/default")
		{
			this.Parent = parent;
			this.Parent.Children.Add(this);

			this.Position = position; // Position will be local position within parent
			this.Size = ContentManager.Get<SpriteFont>(font).MeasureString(text);

			this.Text = text;
			this.Font = font;	
		}

		public override void Draw()
		{
			Vector2 pos = Parent == null ? this.Position : this.Parent.Position + this.Position;
			SpriteBatch.DrawString(ContentManager.Get<SpriteFont>(Font), Text, pos, Color.White);
		}

		public override void Update()
		{
		}
	}
}
