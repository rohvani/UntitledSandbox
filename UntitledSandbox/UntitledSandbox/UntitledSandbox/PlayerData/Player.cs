using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UntitledSandbox.PlayerData
{
	public class Player
	{
		public Camera Camera { get; private set; }
		public Controls Controls { get; private set; }
		public KeyboardState KeyboardState { get; set; }
		public MouseState MouseState { get; private set; }
		public bool MouseLook { get; set; }
		
		public Player()
		{
			this.Camera = new Camera();
			this.Controls = new Controls(this);

			this.UpdateKeyboardState();
			this.UpdateMouseState();

			this.MouseLook = true;

			this.Camera.UpdateViewMatrix();
		}

		public bool UpdateKeyboardState()
		{
			KeyboardState state = Keyboard.GetState();
			bool changed = state != this.KeyboardState;
			this.KeyboardState = state;
			return changed;
		}

		public void UpdateMouseState()
		{
			Game.CenterMouse();
			this.MouseState = Mouse.GetState();
		}
	}
}
