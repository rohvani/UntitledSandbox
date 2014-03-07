using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UntitledSandbox.Common.UI;
using UntitledSandbox.Managers;

namespace UntitledSandbox.PlayerData
{
	public class Controls
	{
		public Player Player { get; private set; }
		private Vector3 moveVector;

		public Controls(Player player)
		{
			this.Player = player;
			this.moveVector = Vector3.Zero;
		}

		public void ProcessInput(float amount)
		{
			MouseState mouseState = Mouse.GetState();

			KeyboardState oldKeyboardState = this.Player.KeyboardState;
			bool keyboardChanged = this.Player.UpdateKeyboardState();

			if (Game.Instance.IsActive)
			{
				if (Player.MouseLook)
				{
					float xDifference = mouseState.X - this.Player.MouseState.X;
					float yDifference = mouseState.Y - this.Player.MouseState.Y;
					this.Player.Camera.RotationYaw -= this.Player.Camera.RotationSpeed * xDifference * amount;
					this.Player.Camera.RotationPitch -= this.Player.Camera.RotationSpeed * yDifference * amount;

					Game.CenterMouse();
				}

				if (!Player.MouseLook && mouseState.LeftButton == ButtonState.Pressed && Player.MouseState.LeftButton == ButtonState.Released)
					UIManager.HandleClick(new Vector2(mouseState.X, mouseState.Y));

				if (keyboardChanged)
				{
					KeyboardState keyboardState = this.Player.KeyboardState;

					if (oldKeyboardState.IsKeyUp(Keys.F1) && keyboardState.IsKeyDown(Keys.F1))
					{
						Game.Instance.IsMouseVisible = !Game.Instance.IsMouseVisible;
						Player.MouseLook = !Player.MouseLook;
					}

					if (oldKeyboardState.IsKeyUp(Keys.F2) && keyboardState.IsKeyDown(Keys.F2))
						UIManager.RegisterComponent(new Panel(new Vector2(100, 100), new Vector2(600, 200)));

					if (oldKeyboardState.IsKeyUp(Keys.F3) && keyboardState.IsKeyDown(Keys.F3))
						UIManager.RegisterComponent(new Panel(new Vector2(150, 90), new Vector2(600, 200)));

					this.moveVector = new Vector3(0, 0, 0);

					if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
						this.moveVector.Z -= 1;
					if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
						this.moveVector.Z += 1;
					if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
						this.moveVector.X += 1;
					if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
						this.moveVector.X -= 1;
					if (keyboardState.IsKeyDown(Keys.Q))
						this.moveVector.Y += 1;
					if (keyboardState.IsKeyDown(Keys.Z))
						this.moveVector.Y -= 1;
				}

				this.Player.Camera.AddToCameraPosition(this.moveVector * amount);
			}

			Player.UpdateMouseState(false);
		}
	}
}
