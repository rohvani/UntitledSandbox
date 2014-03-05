using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UntitledSandbox.Managers;
using UntitledSandbox.PlayerData;

namespace UntitledSandbox.Terrain
{
	public abstract class Renderer
	{
		protected ContentManager ContentManager
		{
			get { return Game.Instance.ContentManager; }
		}

		protected GraphicsDevice GraphicsDevice
		{
			get { return Game.Instance.GraphicsDevice; }
		}

		protected Player Player
		{
			get { return Game.Instance.Player; }
		}

		public abstract void Load();

		public abstract void Draw();
	}
}
