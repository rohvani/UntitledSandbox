using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Managers
{
	public class ContentManager
	{
		private List<Model> Models { get; set; }

		public ContentManager()
		{
			this.Models = new List<Model>();
		}

		public Model LoadModel(string filePath)
		{
			try
			{
				Model model = Game.Instance.Content.Load<Model>(filePath);
				if (model != null) this.Models.Add(model);
				return model;
			}
			catch
			{
				Console.WriteLine("[ContentManager] Error loading '{0}'", filePath);
				return null;
			}
		}

		public Model GetModel(string filePath)
		{
			try
			{
				return this.Models.First(p => p.Tag.Equals(filePath));
			}
			catch
			{
				return LoadModel(filePath);
			}
		}
	}
}
