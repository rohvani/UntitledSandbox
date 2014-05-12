using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UntitledSandbox.Managers;

namespace UntitledSandbox.Common.UI
{
	public class FrameRateCounter : DrawableGameComponent
	{
		ContentManager content;
		SpriteBatch spriteBatch;
		SpriteFont Font { get { return content.Get<SpriteFont>("fonts/default"); } }

		int frameRate = 0;
		int frameCounter = 0;
		TimeSpan elapsedTime = TimeSpan.Zero;


		public FrameRateCounter(Game game) : base(game)
		{
			this.content = game.ContentManager;
			this.spriteBatch = game.SpriteBatch;
		}


		protected void LoadGraphicsContent(bool loadAllContent)
		{
		}


		protected void UnloadGraphicsContent(bool unloadAllContent)
		{
		}


		public override void Update(GameTime gameTime)
		{
			elapsedTime += gameTime.ElapsedGameTime;

			if (elapsedTime > TimeSpan.FromSeconds(1))
			{
				elapsedTime -= TimeSpan.FromSeconds(1);
				frameRate = frameCounter;
				frameCounter = 0;
			}
		}


		public override void Draw(GameTime gameTime)
		{
			frameCounter++;

			string fps = string.Format("fps: {0}", frameRate);

			spriteBatch.Begin();

			spriteBatch.DrawString(this.Font, fps, new Vector2(33, 33), Color.Black);
			spriteBatch.DrawString(this.Font, fps, new Vector2(32, 32), Color.White);

			spriteBatch.End();
		}
	}
}
