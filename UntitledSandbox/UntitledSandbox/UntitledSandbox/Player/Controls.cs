using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UntitledSandbox.Player
{
	public class Controls
	{
		Player player;

		public Controls(Player player)
		{
			this.player = player;
		}

		public void ProcessInput(float amount)
		{
			MouseState mouseState = Mouse.GetState();
			KeyboardState keyState = Keyboard.GetState();

			if (mouseState != player.mouseState)
			{
				float xDifference = mouseState.X - player.mouseState.X;
				float yDifference = mouseState.Y - player.mouseState.Y;
				player.camera.yawRot -= player.camera.rotationSpeed * xDifference * amount;
				player.camera.pitchRot -= player.camera.rotationSpeed * yDifference * amount;
				Mouse.SetPosition(Game.graphics.GraphicsDevice.Viewport.Width / 2, Game.graphics.GraphicsDevice.Viewport.Height / 2);
				player.camera.UpdateViewMatrix();
			}

			Vector3 moveVector = new Vector3(0, 0, 0);

			if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
				moveVector += new Vector3(0, 0, -1);
			if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
				moveVector += new Vector3(0, 0, 1);
			if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
				moveVector += new Vector3(1, 0, 0);
			if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
				moveVector += new Vector3(-1, 0, 0);
			if (keyState.IsKeyDown(Keys.Q))
				moveVector += new Vector3(0, 1, 0);
			if (keyState.IsKeyDown(Keys.Z))
				moveVector += new Vector3(0, -1, 0);

			player.camera.AddToCameraPosition(moveVector * amount);

			player.keyboardState = keyState;
		}
	}
}
