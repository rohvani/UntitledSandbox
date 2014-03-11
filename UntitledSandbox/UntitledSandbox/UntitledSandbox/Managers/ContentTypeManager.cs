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
		private List<Asset> Assets { get; set; }

		public ContentTypeManager()
		{
			this.Assets = new List<Asset>();
		}

		public T Load(string filePath)
		{
			try
			{
				T resource = Game.Instance.Content.Load<T>(filePath);
				if (resource != null)
				{
					Asset asset = new Asset(filePath, resource);
					this.Assets.Add(asset);
				}
				Console.WriteLine("[ContentManager] Loaded new resource '{0}'", filePath);
				return resource;
			}
			catch (Exception e)
			{
				Console.WriteLine("[ContentManager] Error loading resource '{0}'", filePath);
				Console.Write(e);
				return default(T);
			}
		}

		public T Get(string filePath)
		{
			try
			{
				return this.Assets.First<Asset>(p => p.Name.Equals(filePath)).Value;
			}
			catch
			{
				return Load(filePath);
			}
		}

		private sealed class Asset
		{
			public string Name { get; private set; }
			public T Value { get; private set; }

			public Asset(string name, T value)
			{
				this.Name = name;
				this.Value = value;
			}
		}
	}
}