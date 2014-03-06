using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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
				if (mouseState != this.Player.MouseState && Player.MouseLook)
				{
					float xDifference = mouseState.X - this.Player.MouseState.X;
					float yDifference = mouseState.Y - this.Player.MouseState.Y;
					this.Player.Camera.RotationYaw -= this.Player.Camera.RotationSpeed * xDifference * amount;
					this.Player.Camera.RotationPitch -= this.Player.Camera.RotationSpeed * yDifference * amount;

					Game.CenterMouse();
				}

				if (keyboardChanged)
				{
					KeyboardState ks = this.Player.KeyboardState;

					if (oldKeyboardState.IsKeyUp(Keys.F1) && ks.IsKeyDown(Keys.F1))
						Player.MouseLook = !Player.MouseLook;
					if (oldKeyboardState.IsKeyUp(Keys.F2) && ks.IsKeyDown(Keys.F2))
						Game.Instance.UIManager.registerUIComponent(new Common.UI.UIPanel(new Vector2(100, 100), new Vector2(600, 200)));
					if (oldKeyboardState.IsKeyUp(Keys.F3) && ks.IsKeyDown(Keys.F3))
						Game.Instance.UIManager.registerUIComponent(new Common.UI.UIPanel(new Vector2(150, 90), new Vector2(600, 200)));

					this.moveVector = new Vector3(0, 0, 0);

					if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
						this.moveVector.Z -= 1;
					if (ks.IsKeyDown(Keys.Down) || ks.IsKeyDown(Keys.S))
						this.moveVector.Z += 1;
					if (ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
						this.moveVector.X += 1;
					if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
						this.moveVector.X -= 1;
					if (ks.IsKeyDown(Keys.Q))
						this.moveVector.Y += 1;
					if (ks.IsKeyDown(Keys.Z))
						this.moveVector.Y -= 1;
				}

				this.Player.Camera.AddToCameraPosition(this.moveVector * amount);
			}
		}
	}
}
