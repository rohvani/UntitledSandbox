using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Managers
{
	public class ContentManager
	{
		private Game instance;
		private List<Model> modelList;

		public ContentManager()
		{
			instance = Game.Instance;
			modelList = new List<Model>();
		}

		public Model loadModel(string filePath)
		{
			try
			{
				Model model = instance.Content.Load<Model>(filePath);
				if (model != null)
				{
					modelList.Add(model);
					return model;
				}
			}
			catch { Console.WriteLine("[ContentManager] Error loading '{0}'", filePath); }

			return null;
		}

		public Model getModel(string filePath)
		{
			try
			{
				return modelList.First(p => p.Tag.Equals(filePath));
			}
			catch
			{
				return loadModel(filePath);
			}
		}
	}
}
