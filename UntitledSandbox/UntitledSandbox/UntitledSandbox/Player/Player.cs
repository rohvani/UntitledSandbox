using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UntitledSandbox.Player
{
	public class Player
	{
		public Camera camera;
		public Controls controls;

		public KeyboardState keyboardState;
		public MouseState mouseState;

		public Player()
		{
			this.camera = new Camera(new Vector3(0,0,0));
			this.controls = new Controls(this);

			this.keyboardState = Keyboard.GetState();
			this.mouseState = Mouse.GetState();
		}
	}
}
