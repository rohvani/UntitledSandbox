using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UntitledSandbox.PlayerData;
using UntitledSandbox.Common.Utils;

namespace UntitledSandbox.Terrain.Renderers
{
	public class FractralTerrainRenderer : Renderer
	{
		public int Scale { get; private set; }
		public const string EFFECT = "effects/Series4Effects";
		public const string TEXTURE = "textures/grass";
		public const float AMBIENT_LIGHT = 0.4f;
		public static Vector3 LIGHT_DIRECTION
		{
			get { return new Vector3(-0.5f, -1, -0.5f); }
		}

		private int TerrainWidth
		{
			get { return this.HeightMap.GetLength(1); }
		}

		private int TerrainLength
		{
			get { return this.HeightMap.GetLength(0); }
		}

		private float[,] HeightMap;

		private VertexBuffer TerrainVertexBuffer { get; set; }
		private IndexBuffer TerrainIndexBuffer { get; set; }
		private VertexDeclaration TerrainVertexDeclaration { get; set; }

		private float[,] HeightMap2;
		private VertexBuffer TerrainVertexBuffer2 { get; set; }
		private IndexBuffer TerrainIndexBuffer2 { get; set; }

		public FractralTerrainRenderer(int scale = 1)
		{
			this.Scale = scale;
		}

		public override void Load()
		{
			this.ContentManager.Load<Texture2D>(TEXTURE);
			this.ContentManager.Load<Effect>(EFFECT);

			int size = 257;

			this.HeightMap = FractalUtils.Fill2DFractArray(initArray(new float[size, size], -255), size - 1, 32546, 10f, 1f);

			float[,] map2 = new float[size, size];
			map2 = initArray(map2, -255);

			for (int x = 0; x < size; x++)
			{
				map2[x, 0] = this.HeightMap[x, 256];
			}

			this.HeightMap2 = FractalUtils.Fill2DFractArray(map2, size - 1, 32546, 10f, 1f);

			this.LoadVertices();
		}

		private static float[,] initArray(float[,] array, float value)
		{
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					array[i, j] = value;
				}
			}
			return array;
		}

		public override void Draw()
		{
			this.DrawTerrain();
		}

		private void LoadVertices()
		{
			//Texture2D heightMap = this.ContentManager.Load<Texture2D>("textures/heightmap");
			//this.LoadHeightData(heightMap);

			VertexPositionNormalTexture[] terrainVertices = this.SetUpTerrainVertices(this.HeightMap);
			int[] terrainIndices = this.SetUpTerrainIndices();
			terrainVertices = this.CalculateNormals(terrainVertices, terrainIndices);
			this.CopyToTerrainBuffers(terrainVertices, terrainIndices);

			terrainVertices = this.SetUpTerrainVertices(this.HeightMap2, yPos: 256);
			terrainIndices = this.SetUpTerrainIndices();
			terrainVertices = this.CalculateNormals(terrainVertices, terrainIndices);
			this.CopyToTerrainBuffers2(terrainVertices, terrainIndices);
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


		private VertexPositionNormalTexture[] SetUpTerrainVertices(float[,] heightMap, int xPos=0, int yPos=0)
		{
			VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[heightMap.Length];
			
			for (int x = 0; x < this.TerrainWidth; x++)
			{
				for (int y = 0; y < this.TerrainLength; y++)
				{
					terrainVertices[x + y * this.TerrainWidth].Position = new Vector3(x + xPos, heightMap[x, y] * 20, -y - yPos) * this.Scale;
					terrainVertices[x + y * this.TerrainWidth].TextureCoordinate.X = (float) (x + xPos) / (this.Scale * 30);
					terrainVertices[x + y * this.TerrainWidth].TextureCoordinate.Y = (float) (y + yPos) / (this.Scale * 30);
				}
			}

			return terrainVertices;
		}


		private int[] SetUpTerrainIndices(int x=0, int y=0)
		{
			int xdef = x, ydef = y;
			int[] indices = new int[(this.TerrainWidth - 1) * (this.TerrainLength - 1) * 6];
			int counter = 0;
			for (; y < this.TerrainLength - 1 + ydef; y++)
			{
				for (x = xdef; x < this.TerrainWidth - 1 + xdef; x++)
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

		private void CopyToTerrainBuffers2(VertexPositionNormalTexture[] vertices, int[] indices)
		{
			this.TerrainVertexBuffer2 = new VertexBuffer(this.GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
			this.TerrainVertexBuffer2.SetData(vertices);

			this.TerrainIndexBuffer2 = new IndexBuffer(this.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
			this.TerrainIndexBuffer2.SetData(indices);
		}

		private void DrawTerrain()
		{
			RasterizerState state = new RasterizerState();
			state.FillMode = Controls.IsWire ? FillMode.WireFrame : FillMode.Solid;
			state.CullMode = CullMode.None;
			this.GraphicsDevice.RasterizerState = state;


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

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				this.GraphicsDevice.SetVertexBuffer(this.TerrainVertexBuffer2);
				this.GraphicsDevice.Indices = this.TerrainIndexBuffer2;

				int noVertices = this.TerrainVertexBuffer2.VertexCount;
				int noTriangles = this.TerrainIndexBuffer2.IndexCount / 3;
				this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, noVertices, 0, noTriangles);
			}
		}
	}
}