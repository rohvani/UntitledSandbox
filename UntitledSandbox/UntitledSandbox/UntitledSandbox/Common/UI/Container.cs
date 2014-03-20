using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Common.UI
{
	public abstract class Container : Component
	{
		public List<Component> Children;

		public Container(Vector2 position, Vector2 size, string name="Window") : base(position, size, name)
		{
			this.Children = new List<Component>();
			this.Clicked += delegate(object sender, ClickEventArgs args)
			{
				foreach (Component child in this.Children)
				{
					if (child.Contains(args.ClickLocation))
					{
						child.RaiseClickEvent(args, sender);
					}
				}
			};

			this.Dragged += delegate(object sender, DragEventArgs args)
			{
				foreach (Component child in this.Children)
				{
					child.RaiseDragEvent(args, sender);
				}
			};
		}

		public Container() : this(Vector2.Zero, Vector2.Zero)
		{
		}

		public void AddChild(Component component)
		{
			this.Children.Add(component);
			component.Parent = this;
		}
	}
}
