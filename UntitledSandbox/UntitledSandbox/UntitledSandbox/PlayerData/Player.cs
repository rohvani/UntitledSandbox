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
		public Camera Camera
		{
			get { return this._camera; }
		}

		public Controls Controls
		{
			get { return this._controls; }
		}

		public KeyboardState KeyboardState
		{
			get { return this._keyboardState; }
			set { this._keyboardState = value; }
		}

		public MouseState MouseState
		{
			get { return this._mouseState; }
		}

		private Camera _camera;
		private Controls _controls;
		private KeyboardState _keyboardState;
		private MouseState _mouseState;
		
		public Player()
		{
			this._camera = new Camera(new Vector3(0,0,0));
			this._controls = new Controls(this);

			this._keyboardState = Keyboard.GetState();
			this._mouseState = Mouse.GetState();

			this.Camera.UpdateViewMatrix();
		}
	}
}
