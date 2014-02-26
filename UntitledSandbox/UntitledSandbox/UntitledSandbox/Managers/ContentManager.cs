using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Managers
{
	public class ContentManager
	{
		private List<Model> modelList;

		public ContentManager()
		{
			this.modelList = new List<Model>();
		}

		public Model loadModel(string filePath)
		{
			try
			{
				Model model = Game.Instance.Content.Load<Model>(filePath);
				if (model != null) this.modelList.Add(model);
				return model;
			}
			catch
			{
				Console.WriteLine("[ContentManager] Error loading '{0}'", filePath);
				return null;
			}
		}

		public Model getModel(string filePath)
		{
			try
			{
				return this.modelList.First(p => p.Tag.Equals(filePath));
			}
			catch
			{
				return loadModel(filePath);
			}
		}
	}
}
