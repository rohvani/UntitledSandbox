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
	public class Label : Component
	{
		const string DEFAULT_FONT = "fonts/default";

		private Vector2 TextSize { get { return ContentManager.Get<SpriteFont>(this.Font).MeasureString(this.Text); } }

		public string Text { get; set; }
		public string Font { get; set; }

		public Label(string text, Vector2 position, string font=DEFAULT_FONT)
		{
			this.Name = "Label";

			this.Text = text;
			this.Font = font;

			this.Position = position;
			this.Size = this.TextSize;

			this.UpdateRange();

			this.Dragged += delegate(object sender, DragEventArgs args) { this.UpdateRange(); };
		}

		public Label(Container parent, string text, Vector2 position, string font=DEFAULT_FONT)
		{
			this.Name = "Label";

			parent.AddChild(this);

			this.Text = text;
			this.Font = font;

			this.Position = position; // Position will be local position within parent
			this.Size = this.TextSize;

			this.UpdateRange();

			this.Dragged += delegate(object sender, DragEventArgs args) { this.UpdateRange(); };
		}

		public override void Draw()
		{
			Vector2 pos = Parent == null ? this.Position : this.Parent.Position + this.Position;
			SpriteBatch.DrawString(ContentManager.Get<SpriteFont>(Font), Text, pos, Color.White);
		}

		public override void UpdateRange()
		{
			Vector2 parentPosition = this.Parent == null ? Vector2.Zero : this.Parent.Position; 
			this.Range = new Vector2Range(parentPosition + this.Position, parentPosition + this.Position + this.Size);
		}

		public override void Update()
		{
		}
	}
}
