using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UntitledSandbox.Terrain.Quad;
using UntitledSandbox.PlayerData;

namespace UntitledSandbox.Terrain.Quad
{
	public class QuadTreeRenderer : Renderer
	{
		private RasterizerState _rsDefault = new RasterizerState();
		private RasterizerState _rsWire = new RasterizerState();

		private QuadNode ActiveNode { get; set; }

		public int MinimumDepth { get; set; }

		public BasicEffect Effect { get; set; }
		
		private BufferManager Buffers { get; set; }

		private int Scale { get; set; }
		private Vector3 Position { get; set; }
		private Vector3 LastCameraPosition { get; set; }

		public int[] Indices { get; set; }
		public int IndexCount { get; private set; }

		public Matrix View { get { return Game.Instance.Player.Camera.ViewMatrix; } }
		public Matrix Projection { get { return Game.Instance.Player.Camera.ProjectionMatrix; } }
		public Vector3 CameraPosition { get { return Game.Instance.Player.Camera.Position; } }

		public GraphicsDevice Device { get { return Game.Instance.GraphicsDevice; } }

		public int TopNodeSize { get; private set; }
		public QuadNode RootNode { get; private set; }
		public TreeVertexCollection Vertices { get; private set; }

		public QuadTreeRenderer(Vector3 position, int scale)
		{
			this.Position = position;
			this.Scale = scale;

			this.MinimumDepth = 6;

			Game.Instance.Player.Camera.CameraUpdated += this.Update;
		}

		public override void Load()
		{
			_rsDefault.CullMode = CullMode.CullCounterClockwiseFace;
			_rsDefault.FillMode = FillMode.Solid;

			_rsWire.CullMode = CullMode.CullCounterClockwiseFace;
			_rsWire.FillMode = FillMode.WireFrame;

			Texture2D heightMap = Game.Instance.ContentManager.Load<Texture2D>("textures/hmSmall");

			this.TopNodeSize = heightMap.Width - 1;

			this.Vertices = new TreeVertexCollection(this.Position, heightMap, this.Scale);
			this.Buffers = new BufferManager(this.Vertices.Vertices, this.Device);
			this.RootNode = new QuadNode(NodeType.FullNode, this.TopNodeSize, 1, null, this, 0);

			//Construct an array large enough to hold all of the indices we'll need.
			this.Indices = new int[((heightMap.Width + 1) * (heightMap.Height + 1)) * 3];

			Effect = new BasicEffect(this.Device);
			Effect.EnableDefaultLighting();
			Effect.FogEnabled = true;
			Effect.FogStart = 300f;
			Effect.FogEnd = 1000f;
			Effect.FogColor = Color.Black.ToVector3();
			Effect.TextureEnabled = true;
			Effect.Texture = Game.Instance.ContentManager.Load<Texture2D>("textures/jigsaw");//new Texture2D(this.Device, 100, 100);
			Effect.Projection = this.Projection;
			Effect.View = this.View;
			Effect.World = Matrix.Identity;
		}

		public void Update(object sender, EventArgs args)
		{
			this.Effect.View = View;
			this.Effect.Projection = Projection;

			this.LastCameraPosition = this.CameraPosition;
			this.IndexCount = 0;

			this.RootNode.Merge();
			this.RootNode.EnforceMinimumDepth();
			this.ActiveNode = this.RootNode.DeepestNodeWithPoint(this.CameraPosition);
			if (this.ActiveNode != null)
				this.ActiveNode.Split();
			this.RootNode.SetActiveVertices();

			this.Buffers.UpdateIndexBuffer(this.Indices, this.IndexCount);
			this.Buffers.SwapBuffer();
		}

		public override void Draw()
		{
			this.GraphicsDevice.RasterizerState = Controls.IsWire ? _rsWire : _rsDefault;

			this.Device.SetVertexBuffer(this.Buffers.VertexBuffer);
			this.Device.Indices = this.Buffers.IndexBuffer;

			foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.Vertices.Vertices.Length, 0, this.IndexCount / 3);
			}
		}

		internal void UpdateBuffer(int vIndex)
		{
			this.Indices[IndexCount] = vIndex;
			this.IndexCount++;
		}
	}
}
