using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace UntitledSandbox.Managers
{
	public class ContentTypeManager<T> where T : class
	{
		public List<T> Resources { get; set; }

		public ContentTypeManager()
		{
			if (!typeof(T).IsSubclassOf(typeof(GraphicsResource)) && !typeof(T).Equals(typeof(Model)))
				throw new Exception("ContentTypeManager created with illegal type parameter");
			this.Resources = new List<T>();
		}

		public T Load(string filePath)
		{
			try
			{
				T resource = Game.Instance.Content.Load<T>(filePath);
				if (resource != null) this.Resources.Add(resource);
				return resource;
			}
			catch
			{
				Console.WriteLine("[ContentManager] Error loading '{0}'", filePath);
				return default(T);
			}
		}

		public T Get(string filePath)
		{
			try
			{
				return this.Resources.First(p => GetTag(p).Equals(filePath));
			}
			catch
			{
				return Load(filePath);
			}
		}

		private object GetTag(object o)
		{
			PropertyInfo tag = o.GetType().GetProperty("Tag");
			if (tag != null) return tag.GetValue(o, null);
			return null;
		}
	}

}
