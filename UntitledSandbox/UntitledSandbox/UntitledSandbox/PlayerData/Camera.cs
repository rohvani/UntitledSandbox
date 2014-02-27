using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;

namespace UntitledSandbox.PlayerData
{
	public class Camera
	{
		public Vector3 Position { get; set; }
		public float RotationYaw { get; set; }
		public float RotationPitch { get; set; }
		public float RotationSpeed { get; set; }
		public float MoveSpeed { get; set; }
		public Matrix ViewMatrix { get; set; }
		public Matrix ProjectionMatrix { get; set; }
		public BoundingFrustum Frustum { get; private set; }

		public Camera(Vector3 position, float cameraMoveSpeed)
		{
			this.Position = position;

			this.RotationYaw = MathHelper.PiOver2;
			this.RotationPitch = -MathHelper.Pi / 10.0f;

			this.RotationSpeed = 0.3f;
			this.MoveSpeed = cameraMoveSpeed;

			this.ViewMatrix = new Matrix();
			this.ProjectionMatrix = new Matrix();

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

			this.Frustum.Matrix = (this.ViewMatrix * this.ProjectionMatrix);
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
