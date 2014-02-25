using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.PlayerData
{
	public class Camera
	{
		private Vector3 _position;
		public Vector3 Position
		{
			get { return this._position; }
			set { this._position = value; }
		}

		private float _rotationYaw;
		public float RotationYaw
		{
			get { return this._rotationYaw; }
			set { this._rotationYaw = value; }
		}

		private float _rotationPitch;
		public float RotationPitch
		{
			get { return this._rotationPitch; }
			set { this._rotationPitch = value; }
		}

		private float _rotationSpeed;
		public float RotationSpeed
		{
			get { return this._rotationSpeed; }
			set { this._rotationSpeed = value; }
		}

		private float _moveSpeed;
		public float MoveSpeed
		{
			get { return this._moveSpeed; }
			set { this._moveSpeed = value; }
		}

		private Matrix _viewMatrix;
		public Matrix ViewMatrix
		{
			get { return this._viewMatrix; }
			set { this._viewMatrix = value; }
		}

		private Matrix _projectionMatrix;
		public Matrix ProjectionMatrix
		{
			get { return this._projectionMatrix; }
			set { this._projectionMatrix = value; }
		}

		public BoundingFrustum Frustum;

		public Camera(Vector3 position, float cameraMoveSpeed)
		{
			this._position = position;

			this._rotationYaw = MathHelper.PiOver2;
			this._rotationPitch = -MathHelper.Pi / 10.0f;

			this._rotationSpeed = 0.3f;
			this._moveSpeed = cameraMoveSpeed;

			this._viewMatrix = new Matrix();
			this._projectionMatrix = new Matrix();

			this.Frustum = new BoundingFrustum(this.ViewMatrix * this.ProjectionMatrix);
		}

		public Camera(Vector3 position) : this(position, 30.0F)
		{
		}

		public Camera() : this(Vector3.Zero, 30.0F)
		{
		}

		public void UpdateViewMatrix()
		{
			Matrix cameraRotation = Matrix.CreateRotationX(this.RotationPitch) * Matrix.CreateRotationY(this.RotationYaw);

			Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
			Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
			Vector3 cameraFinalTarget = this.Position + cameraRotatedTarget;

			Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
			Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

			this.ViewMatrix = Matrix.CreateLookAt(this.Position, cameraFinalTarget, cameraRotatedUpVector);
		}

		public void AddToCameraPosition(Vector3 vectorToAdd)
		{
			Matrix cameraRotation = Matrix.CreateRotationX(this.RotationPitch) * Matrix.CreateRotationY(this.RotationYaw);
			Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
			this.Position += this.MoveSpeed * rotatedVector;
			UpdateViewMatrix();
		}
	}
}
