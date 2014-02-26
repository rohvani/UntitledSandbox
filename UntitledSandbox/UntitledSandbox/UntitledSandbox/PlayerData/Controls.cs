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
			KeyboardState keyboardState = Keyboard.GetState();

			if (Game.Instance.IsActive)
			{
				//if (Player.KeyboardState.IsKeyUp(Keys.LeftAlt) && keyboardState.IsKeyDown(Keys.LeftAlt)) Player.MouseLook = !Player.MouseLook;

				if (mouseState != this.Player.MouseState)
				{
					float xDifference = mouseState.X - this.Player.MouseState.X;
					float yDifference = mouseState.Y - this.Player.MouseState.Y;
					this.Player.Camera.RotationYaw -= this.Player.Camera.RotationSpeed * xDifference * amount;
					this.Player.Camera.RotationPitch -= this.Player.Camera.RotationSpeed * yDifference * amount;
					Mouse.SetPosition(Game.Instance.graphics.GraphicsDevice.Viewport.Width / 2, Game.Instance.graphics.GraphicsDevice.Viewport.Height / 2);
				}

				Vector3 moveVector = new Vector3(0, 0, 0);

				if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
					moveVector += new Vector3(0, 0, -1);
				if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
					moveVector += new Vector3(0, 0, 1);
				if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
					moveVector += new Vector3(1, 0, 0);
				if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
					moveVector += new Vector3(-1, 0, 0);
				if (keyboardState.IsKeyDown(Keys.Q))
					moveVector += new Vector3(0, 1, 0);
				if (keyboardState.IsKeyDown(Keys.Z))
					moveVector += new Vector3(0, -1, 0);

				this.Player.Camera.AddToCameraPosition(moveVector * amount);
			}

			this.Player.KeyboardState = keyboardState;
		}
	}
}
