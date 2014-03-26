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

		bool generateHeightMap = true;
		public override void Load()
		{
			_rsDefault.CullMode = CullMode.CullCounterClockwiseFace;
			_rsDefault.FillMode = FillMode.Solid;

			_rsWire.CullMode = CullMode.CullCounterClockwiseFace;
			_rsWire.FillMode = FillMode.WireFrame;

			int size;
			dynamic map;

			if (generateHeightMap)
			{
				HeightMap heightMap = new HeightMap(513);
				heightMap.AddPerlinNoise(5.0f);
				heightMap.Perturb(32.0f, 32.0f);
				for (int i = 0; i < 10; i++)
					heightMap.Erode(16.0f);
				heightMap.Smoothen();
				heightMap.Normalize();

				size = heightMap.Size;
				map = heightMap;
			}
			else
			{

				Texture2D heightMap = Game.Instance.ContentManager.Load<Texture2D>("textures/hmSmall");
				size = heightMap.Height;
				map = heightMap;
			}

			this.TopNodeSize = size - 1;

			this.Vertices = new TreeVertexCollection(this.Position, map, this.Scale);
			this.Buffers = new BufferManager(this.Vertices.Vertices, this.Device);
			this.RootNode = new QuadNode(NodeType.FullNode, this.TopNodeSize, 1, null, this, 0);

			//Construct an array large enough to hold all of the indices we'll need.
			// SOMETHING IS WRONG HERE
			this.Indices = new int[((size + 1) * (size + 1)) * 3];

			Effect = new BasicEffect(this.Device);
			Effect.LightingEnabled = true;
			Effect.AmbientLightColor = new Vector3(0.4f);
			Effect.DirectionalLight0.Enabled = true;
			Effect.DirectionalLight0.Direction = new Vector3(-0.5f, -1, -0.5f);
			Effect.FogEnabled = true;
			Effect.FogStart = 300f;
			Effect.FogEnd = 1000f;
			Effect.FogColor = Color.Black.ToVector3();
			Effect.TextureEnabled = true;
			Effect.Texture = Game.Instance.ContentManager.Load<Texture2D>("textures/grass");//new Texture2D(this.Device, 100, 100);
			Effect.Projection = this.Projection;
			Effect.View = this.View;
			Effect.World = Matrix.Identity;

			this.RootNode.EnforceMinimumDepth();
		}

		public void Update(object sender, EventArgs args)
		{
			this.Effect.View = View;
			this.Effect.Projection = Projection;

			this.LastCameraPosition = this.CameraPosition;
			this.IndexCount = 0;

			//this.RootNode.Merge();
			//this.RootNode.EnforceMinimumDepth();
			//this.ActiveNode = this.RootNode.DeepestNodeWithPoint(this.CameraPosition);
			//if (this.ActiveNode != null)
				//this.ActiveNode.Split();
			this.RootNode.SetActiveVertices();

			this.Buffers.UpdateIndexBuffer(this.Indices, this.IndexCount);
			this.Buffers.SwapBuffer();

			Game.Instance.Window.Title = String.Format("Triangles Rendered: {0}", this.IndexCount / 3);
		}

		public override void Draw()
		{
			if (this.IndexCount == 0)
				return;
			this.GraphicsDevice.RasterizerState = Controls.IsWire ? _rsWire : _rsDefault;

			this.Device.SetVertexBuffer(this.Buffers.VertexBuffer);
			this.Device.Indices = this.Buffers.IndexBuffer;

			foreach (EffectPass pass in this.Effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				// These method arguments will be our salvation, use multiple draw calls instead of rebuilding the buffer every frame
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
