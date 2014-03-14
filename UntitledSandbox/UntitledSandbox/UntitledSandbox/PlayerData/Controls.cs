using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UntitledSandbox.Common.UI;
using UntitledSandbox.Managers;
using System;

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
			MouseState oldMouseState = Player.MouseState;
			MouseState newMouseState = Mouse.GetState();

			KeyboardState oldKeyboardState = this.Player.KeyboardState;
			bool keyboardChanged = this.Player.UpdateKeyboardState();

			if (Game.Instance.IsActive)
			{
				if (Player.MouseLook)
				{
					float xDifference = newMouseState.X - oldMouseState.X;
					float yDifference = newMouseState.Y - oldMouseState.Y;
					this.Player.Camera.RotationYaw -= this.Player.Camera.RotationSpeed * xDifference * amount;
					this.Player.Camera.RotationPitch -= this.Player.Camera.RotationSpeed * yDifference * amount;

					Game.CenterMouse();
				}
				else
				{
					if (oldMouseState.LeftButton == ButtonState.Pressed && newMouseState.LeftButton == ButtonState.Released)
					{
						UIManager.HandleClick(new Vector2(newMouseState.X, newMouseState.Y));
					}
					if (oldMouseState.LeftButton == ButtonState.Released && newMouseState.LeftButton == ButtonState.Pressed)
					{
						UIManager.CheckDragging(new Vector2(oldMouseState.X, oldMouseState.Y));
						UIManager.HandleDrag(new Vector2(newMouseState.X, newMouseState.Y), new Vector2(oldMouseState.X, oldMouseState.Y), true);
					}
					else if (UIManager.IsDragging)
					{
						if (newMouseState.LeftButton == ButtonState.Released)
						{
							UIManager.IsDragging = false;
						}

						if (oldMouseState.X != newMouseState.X || oldMouseState.Y != newMouseState.Y)
						{
							UIManager.HandleDrag(new Vector2(newMouseState.X, newMouseState.Y), new Vector2(oldMouseState.X, Player.MouseState.Y));
						}
					}
				}

				if (keyboardChanged)
				{
					KeyboardState keyboardState = this.Player.KeyboardState;

					if (oldKeyboardState.IsKeyUp(Keys.F1) && keyboardState.IsKeyDown(Keys.F1))
					{
						Game.Instance.IsMouseVisible = !Game.Instance.IsMouseVisible;
						Player.MouseLook = !Player.MouseLook;
					}

					if (oldKeyboardState.IsKeyUp(Keys.F2) && keyboardState.IsKeyDown(Keys.F2))
					{
						Panel panel = new Panel(new Vector2(100, 100), new Vector2(315, 200));
						Label label = new Label("This is a label.  My parent's name is " + panel.Name, new Vector2(15, 25), panel);

						UIManager.RegisterComponent(panel);
					}

					if (oldKeyboardState.IsKeyUp(Keys.F3) && keyboardState.IsKeyDown(Keys.F3))
					{
						Panel panel = new Panel(new Vector2(100, 100), new Vector2(200, 200));
						Label label = new Label("This is a label.", new Vector2(15, 25), panel);

						UIManager.RegisterComponent(panel);
					}

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
