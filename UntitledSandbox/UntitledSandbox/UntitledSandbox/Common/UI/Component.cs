using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UntitledSandbox.Managers;

namespace UntitledSandbox.Common.UI
{
	public abstract class Component
	{
		protected ContentManager ContentManager
		{
			get { return Game.Instance.ContentManager; }
		}

		protected GraphicsDevice GraphicsDevice
		{
			get { return Game.Instance.GraphicsDevice; }
		}

		protected SpriteBatch SpriteBatch
		{
			get { return Game.Instance.SpriteBatch; }
		}

		public event EventHandler<ClickEventArgs> Clicked;
		public event EventHandler<DragEventArgs> Dragged;

		public string Name { get; set; }

		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }
		public Container Parent { get; protected set; }

		public Component(Vector2 position, Vector2 size, string name="Window")
		{
			this.Position = position;
			this.Size = size;
			this.Name = name;
		}

		public Component() : this(Vector2.Zero, Vector2.Zero)
		{
		}

		public abstract void Draw();
		public abstract void Update();

		public virtual bool Contains(Vector2 pixel)
		{
			// There's a better way to do this calculation, probably using a Range system
			// I'm pondering (pixel.X - this.Position.X) > 0 && < this.Size.X
			if (pixel.X >= this.Position.X && pixel.X <= (this.Position.X + this.Size.X) &&
				pixel.Y >= this.Position.Y && pixel.Y <= (this.Position.Y + this.Size.Y))
				return true;
			
			return false;
		}

		public void RaiseClickEvent(Vector2 clickLocation, object sender = null)
		{
			this.Clicked(sender, new ClickEventArgs(clickLocation));
		}

		public void RaiseClickEvent(ClickEventArgs args, object sender = null)
		{
			this.Clicked(sender, args);
		}

		public void RaiseDragEvent(Vector2 from, Vector2 to, object sender = null)
		{
			this.Dragged(sender, new DragEventArgs(from, to));
		}

		public void RaiseDragEvent(DragEventArgs args, object sender = null)
		{
			this.Dragged(sender, args);
		}
	}

	public class ClickEventArgs : EventArgs
	{
		public ClickEventArgs(Vector2 clickLocation)
		{
			this.ClickLocation = clickLocation;
		}

		public Vector2 ClickLocation { get; private set; }
	}

	public class DragEventArgs : EventArgs
	{
		public DragEventArgs(Vector2 from, Vector2 to)
		{
			this.From = from;
			this.To = to;
		}

		public Vector2 From { get; private set; }
		public Vector2 To { get; private set; }
	}
}
