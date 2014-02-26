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

		public bool MouseLook
		{
			get { return this._mouseLook; }
			set { this._mouseLook = value; }
		}

		private Camera _camera;
		private Controls _controls;
		private KeyboardState _keyboardState;
		private MouseState _mouseState;
		private bool _mouseLook;
		
		public Player()
		{
			this._camera = new Camera();
			this._controls = new Controls(this);

			this._keyboardState = Keyboard.GetState();
			this._mouseState = Mouse.GetState();

			this._mouseLook = true;

			this.Camera.UpdateViewMatrix();
		}
	}
}
