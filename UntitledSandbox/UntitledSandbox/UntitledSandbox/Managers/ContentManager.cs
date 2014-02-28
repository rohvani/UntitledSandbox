using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Managers
{
	public class ContentManager
	{
		private Dictionary<Type, dynamic> Managers { get; set; }

		public ContentManager()
		{
			this.Managers = new Dictionary<Type, dynamic>();
		}

		public T Load<T>(string filePath) where T : class
		{
			return this.GetManager<T>().Load(filePath);
		}

		public T Get<T>(string filePath) where T : class
		{
			return this.GetManager<T>().Get(filePath);
		}

		public ContentTypeManager<T> GetManager<T>() where T : class
		{
			Type type = typeof(T);
			dynamic manager;
			if (!this.Managers.TryGetValue(type, out manager))
			{
				manager = new ContentTypeManager<T>();
				this.Managers.Add(type, manager);
			}
			return manager;
		}
	}
}
