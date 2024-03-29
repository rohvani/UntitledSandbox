﻿using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UntitledSandbox.Managers;
using UntitledSandbox.Common.Utils;

namespace UntitledSandbox.Common.UI
{
	public abstract class Component
	{
		protected ContentManager ContentManager { get { return Game.Instance.ContentManager; } }
		protected GraphicsDevice GraphicsDevice { get { return Game.Instance.GraphicsDevice; } }
		protected SpriteBatch SpriteBatch { get { return Game.Instance.SpriteBatch; } }

		public Vector2Range Range { get; set; }

		public Vector2 Position { get; set; }
		public Vector2 Size;

		public string Name { get; set; }
		public Container Parent { get; set; }

		public Component(Vector2 position, Vector2 size, string name="Window")
		{
			this.Position = position;
			this.Size = size;
			this.Name = name;

			this.Range = new Vector2Range(this.Position, this.Position + this.Size);

			this.Dragged += delegate(object sender, DragEventArgs args) { this.UpdateRange(); };
		}

		public Component() : this(Vector2.Zero, Vector2.Zero)
		{
		}

		public abstract void Draw();
		public abstract void Update();

		public virtual void Drag(Vector2 from, Vector2 to)
		{

		}

		public virtual bool Contains(Vector2 pixel)
		{
			return this.Range.Contains(pixel);
		}

		public virtual void UpdateRange()
		{
			this.Range = new Vector2Range(this.Position, this.Position + this.Size);
		}

		public virtual bool isDraggable(Vector2 from)
		{
			return false;
		}

		#region Events
		public event EventHandler<ClickEventArgs> Clicked;
		public event EventHandler<DragEventArgs> Dragged;

		public void RaiseClickEvent(Vector2 clickLocation, object sender = null)
		{
			if (this.Clicked != null)
				this.Clicked(sender, new ClickEventArgs(clickLocation));
		}

		public void RaiseClickEvent(ClickEventArgs args, object sender = null)
		{
			if (this.Clicked != null)
				this.Clicked(sender, args);
		}

		public void RaiseDragEvent(Vector2 from, Vector2 to, object sender = null)
		{
			if (this.Dragged != null)
				this.Dragged(sender, new DragEventArgs(from, to));
		}

		public void RaiseDragEvent(DragEventArgs args, object sender = null)
		{
			if (this.Dragged != null)
				this.Dragged(sender, args);
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
		#endregion
	}
}
