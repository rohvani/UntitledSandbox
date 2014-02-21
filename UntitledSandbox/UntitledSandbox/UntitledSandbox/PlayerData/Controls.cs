using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UntitledSandbox.PlayerData
{
	public class Controls
	{
		public Player Player
		{
			get { return this._player; }
		}

		private Player _player;

		public Controls(Player player)
		{
			this._player = player;
		}

		public void ProcessInput(float amount)
		{
			MouseState mouseState = Mouse.GetState();
			KeyboardState keyState = Keyboard.GetState();

			if (mouseState != this.Player.MouseState)
			{
				float xDifference = mouseState.X - this.Player.MouseState.X;
				float yDifference = mouseState.Y - this.Player.MouseState.Y;
				this.Player.Camera._rotationYaw -= this.Player.Camera._rotationSpeed * xDifference * amount;
				this.Player.Camera._rotationPitch -= this.Player.Camera._rotationSpeed * yDifference * amount;
				Mouse.SetPosition(Game.graphics.GraphicsDevice.Viewport.Width / 2, Game.graphics.GraphicsDevice.Viewport.Height / 2);
				this.Player.Camera.UpdateViewMatrix();
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

			this.Player.Camera.AddToCameraPosition(moveVector * amount);

			this.Player.KeyboardState = keyState;
		}
	}
}
