using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Common.UI
{
	public abstract class Container : Component
	{
		public List<Component> Children { get; set; }

		public Container(Vector2 position, Vector2 size, string name="Window") : base(position, size, name)
		{
			this.Children = new List<Component>();
			this.Clicked += this.ChildClickHandler;
		}

		public Container() : this(Vector2.Zero, Vector2.Zero)
		{
		}

		protected void ChildClickHandler(object sender, ClickEventArgs args)
		{
			foreach (Component child in this.Children)
			{
				Console.WriteLine("clicked lol");
				if (child.Contains(args.ClickLocation))
				{
					child.RaiseClickEvent(args);
				}
			}
		}
	}
}
