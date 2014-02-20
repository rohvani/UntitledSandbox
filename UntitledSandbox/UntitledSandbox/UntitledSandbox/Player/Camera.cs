using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UntitledSandbox.Player
{
	public class Camera
	{
		public Vector3 position;

		public float yawRot, pitchRot;
		public float rotationSpeed, moveSpeed;

		public Matrix viewMatrix;
		public Matrix projectionMatrix;

		public Camera()
		{
			position = Vector3.Zero;

			yawRot = MathHelper.PiOver2;
			pitchRot = -MathHelper.Pi / 10.0f;

			rotationSpeed = 0.3f;
			moveSpeed = 30.0f;

			viewMatrix = new Matrix();
			projectionMatrix = new Matrix();
		}

		public Camera(Vector3 position)
		{
			this.position = position;

			yawRot = MathHelper.PiOver2;
			pitchRot = -MathHelper.Pi / 10.0f;

			rotationSpeed = 0.3f;
			moveSpeed = 30.0f;
		}

		public Camera(Vector3 position, float cameraMoveSpeed)
		{
			this.position = position;

			yawRot = MathHelper.PiOver2;
			pitchRot = -MathHelper.Pi / 10.0f;

			rotationSpeed = 0.3f;
			moveSpeed = cameraMoveSpeed;
		}

		public void UpdateViewMatrix()
		{
			Matrix cameraRotation = Matrix.CreateRotationX(pitchRot) * Matrix.CreateRotationY(yawRot);

			Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
			Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
			Vector3 cameraFinalTarget = position + cameraRotatedTarget;

			Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
			Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

			viewMatrix = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);
		}

		public void AddToCameraPosition(Vector3 vectorToAdd)
		{
			Matrix cameraRotation = Matrix.CreateRotationX(pitchRot) * Matrix.CreateRotationY(yawRot);
			Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
			position += moveSpeed * rotatedVector;
			UpdateViewMatrix();
		}
	}
}
