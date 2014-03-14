using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UntitledSandbox.Managers;

namespace UntitledSandbox.Terrain.Renderers
{
	public class NoiseTerrainRenderer : Renderer
	{
		public const string EFFECT = "effects/Series4Effects";
		public const string TEXTURE = "textures/grass";
		public const float AMBIENT_LIGHT = 0.4f;
		public static Vector3 LIGHT_DIRECTION
		{
			get { return new Vector3(-0.5f, -1, -0.5f); }
		}

		private int TerrainWidth
		{
			get { return this.HeightMap.Size; }
		}
		private int TerrainLength
		{
			get { return this.HeightMap.Size; }
		}

		private HeightMap HeightMap { get; set; }

		private VertexBuffer TerrainVertexBuffer { get; set; }
		private IndexBuffer TerrainIndexBuffer { get; set; }
		private VertexDeclaration TerrainVertexDeclaration { get; set; }

		public NoiseTerrainRenderer()
		{
		}

		public override void Load()
		{
			this.ContentManager.Load<Texture2D>(TEXTURE);
			this.ContentManager.Load<Effect>(EFFECT);

			this.HeightMap = new HeightMap(512);
			this.HeightMap.AddPerlinNoise(5.0f);
			this.HeightMap.Perturb(32.0f, 32.0f);
			for (int i = 0; i < 10; i++)
				this.HeightMap.Erode(16.0f);
			this.HeightMap.Smoothen();
			this.HeightMap.Normalize();

			this.LoadVertices();
		}

		public override void Draw()
		{
			this.DrawTerrain();
		}

		private void LoadVertices()
		{
			//Texture2D heightMap = this.ContentManager.Load<Texture2D>("textures/heightmap");
			//this.LoadHeightData(heightMap);

			VertexPositionNormalTexture[] terrainVertices = this.SetUpTerrainVertices();
			int[] terrainIndices = this.SetUpTerrainIndices();
			terrainVertices = this.CalculateNormals(terrainVertices, terrainIndices);
			this.CopyToTerrainBuffers(terrainVertices, terrainIndices);
		}

		//private void LoadHeightData(Texture2D heightMap)
		//{
		//    float minimumHeight = float.MaxValue;
		//    float maximumHeight = float.MinValue;

		//    this.TerrainWidth = heightMap.Width;
		//    this.TerrainLength = heightMap.Height;

		//    Color[] heightMapColors = new Color[this.TerrainWidth * this.TerrainLength];
		//    heightMap.GetData(heightMapColors);

		//    this.HeightMap = new float[this.TerrainWidth, this.TerrainLength];
		//    for (int x = 0; x < this.TerrainWidth; x++)
		//        for (int y = 0; y < this.TerrainLength; y++)
		//        {
		//            this.HeightMap[x, y] = heightMapColors[x + y * this.TerrainWidth].R;
		//            if (this.HeightMap[x, y] < minimumHeight)
		//                minimumHeight = this.HeightMap[x, y];
		//            if (this.HeightMap[x, y] > maximumHeight)
		//                maximumHeight = this.HeightMap[x, y];
		//        }

		//    for (int x = 0; x < this.TerrainWidth; x++)
		//        for (int y = 0; y < this.TerrainLength; y++)
		//            this.HeightMap[x, y] = (this.HeightMap[x, y] - minimumHeight) / (maximumHeight - minimumHeight) * 30.0f;
		//}


		private VertexPositionNormalTexture[] SetUpTerrainVertices()
		{
			VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[this.HeightMap.Size * this.HeightMap.Size];
			
			for (int x = 0; x < this.TerrainWidth; x++)
			{
				for (int y = 0; y < this.TerrainLength; y++)
				{
					terrainVertices[x + y * this.TerrainWidth].Position = new Vector3(x, this.HeightMap.Heights[x, y] * 20, -y);
					terrainVertices[x + y * this.TerrainWidth].TextureCoordinate.X = (float) x / 30.0f;
					terrainVertices[x + y * this.TerrainWidth].TextureCoordinate.Y = (float) y / 30.0f;
				}
			}

			return terrainVertices;
		}


		private int[] SetUpTerrainIndices()
		{
			int[] indices = new int[(this.TerrainWidth - 1) * (this.TerrainLength - 1) * 6];
			int counter = 0;
			for (int y = 0; y < this.TerrainLength - 1; y++)
			{
				for (int x = 0; x < this.TerrainWidth - 1; x++)
				{
					int lowerLeft = x + y * this.TerrainWidth;
					int lowerRight = (x + 1) + y * this.TerrainWidth;
					int topLeft = x + (y + 1) * this.TerrainWidth;
					int topRight = (x + 1) + (y + 1) * this.TerrainWidth;

					indices[counter++] = topLeft;
					indices[counter++] = lowerRight;
					indices[counter++] = lowerLeft;

					indices[counter++] = topLeft;
					indices[counter++] = topRight;
					indices[counter++] = lowerRight;
				}
			}

			return indices;
		}

		private VertexPositionNormalTexture[] CalculateNormals(VertexPositionNormalTexture[] vertices, int[] indices)
		{
			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Normal = new Vector3(0, 0, 0);

			for (int i = 0; i < indices.Length / 3; i++)
			{
				int index1 = indices[i * 3];
				int index2 = indices[i * 3 + 1];
				int index3 = indices[i * 3 + 2];

				Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
				Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
				Vector3 normal = Vector3.Cross(side1, side2);

				vertices[index1].Normal += normal;
				vertices[index2].Normal += normal;
				vertices[index3].Normal += normal;
			}

			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Normal.Normalize();

			return vertices;
		}

		private void CopyToTerrainBuffers(VertexPositionNormalTexture[] vertices, int[] indices)
		{
			this.TerrainVertexBuffer = new VertexBuffer(this.GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
			this.TerrainVertexBuffer.SetData(vertices);

			this.TerrainIndexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
			this.TerrainIndexBuffer.SetData(indices);
		}

		private void DrawTerrain()
		{
			//RasterizerState state = new RasterizerState();
			//state.FillMode = FillMode.WireFrame;
			//this.GraphicsDevice.RasterizerState = state;

			Effect effect = this.ContentManager.Get<Effect>(EFFECT);
			effect.CurrentTechnique = effect.Techniques["Textured"];
			effect.Parameters["xTexture"].SetValue(this.ContentManager.Get<Texture2D>(TEXTURE));

			Matrix worldMatrix = Matrix.Identity;
			effect.Parameters["xWorld"].SetValue(worldMatrix);
			effect.Parameters["xView"].SetValue(this.Player.Camera.ViewMatrix);
			effect.Parameters["xProjection"].SetValue(this.Player.Camera.ProjectionMatrix);

			effect.Parameters["xEnableLighting"].SetValue(true);
			effect.Parameters["xAmbient"].SetValue(AMBIENT_LIGHT);
			effect.Parameters["xLightDirection"].SetValue(LIGHT_DIRECTION);

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				this.GraphicsDevice.SetVertexBuffer(this.TerrainVertexBuffer);
				this.GraphicsDevice.Indices = this.TerrainIndexBuffer;

				int noVertices = this.TerrainVertexBuffer.VertexCount;
				int noTriangles = this.TerrainIndexBuffer.IndexCount / 3;
				this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, noVertices, 0, noTriangles);

			}
		}
	}
}
