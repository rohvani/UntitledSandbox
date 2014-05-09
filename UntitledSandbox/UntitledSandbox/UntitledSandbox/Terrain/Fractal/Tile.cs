using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using UntitledSandbox.Common.Utils;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Terrain.Fractal
{
	public class Tile
	{
		public float[,] HeightMap;
		public int Size { get; set; }
		public int TileX { get; set; }
		public int TileZ { get; set; }

		public int X { get { return this.TileX * (this.Size - 1); } }
		public int Z { get { return this.TileZ * (this.Size - 1); } }
		public Matrix TranslationMatrix { get { return Matrix.CreateTranslation(this.X, 0, this.Z); } }

		private VertexBuffer TerrainVertexBuffer { get; set; }
		private IndexBuffer TerrainIndexBuffer { get; set; }

		public BoundingBox Bounds;
		public bool IsInView
		{
			get { return this.Bounds.Contains(Game.Instance.Player.Camera.Frustum) != ContainmentType.Disjoint; }
		}

		public Tile(int size, int x, int z)
		{
			this.TileX = x;
			this.TileZ = z;

			this.Size = size;
			this.HeightMap = NewMap();
		}

		/// <summary>
		/// Returns true if the Tile contains a specific point.
		/// </summary>
		/// <param name="point">Vector3 representing the target point</param>
		/// <returns>True if point is contained in the tile's bounding box</returns>
		public bool Contains(Vector3 point)
		{
			point.Y = 0;
			return Bounds.Contains(point) == ContainmentType.Contains;
		}

		public bool Contains(BoundingFrustum boundingFrustrum)
		{
			return Bounds.Intersects(boundingFrustrum);
		}

		public void SeedMap(float[,] north, float[,] east, float[,] south, float[,] west)
		{	
			for (int i = 0; i < this.Size; i++)
			{
				if (north != null)
					this.HeightMap[i, this.Size - 1] = north[i, 0];

				if (east != null) 
					this.HeightMap[this.Size - 1, i] = east[0, i];

				if (south != null)
					this.HeightMap[i, 0] = south[i, this.Size - 1];

				if (west != null)
					this.HeightMap[0, i] = west[this.Size - 1, i];
			}
		}

		public void GenerateMap(int seed, float heightScale, float roughness)
		{
			this.HeightMap = FractalUtils.Fill2DFractArray(this.HeightMap, this.Size-1, seed, heightScale, roughness);

			IEnumerable<float> vals = this.HeightMap.Cast<float>();

			Vector3 min = new Vector3(this.X, vals.Min(), this.Z - (this.Size - 1));
			Vector3 max = new Vector3(this.X + (this.Size - 1), vals.Max(), this.Z);
			this.Bounds = BoundingBox.CreateFromPoints(new Vector3[] {min, max});
			Console.Out.WriteLine("x:{0} z:{1} bounds:{2}", this.TileX, this.TileZ, this.Bounds); 
		}

		private static float[,] InitArray(float[,] array)
		{
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					array[i, j] = -255;
				}
			}
			return array;
		}

		private float[,] NewMap()
		{
			return InitArray(new float[this.Size, this.Size]);
		}

		public void Draw(GraphicsDevice device)
		{
			device.SetVertexBuffer(this.TerrainVertexBuffer);
			device.Indices = this.TerrainIndexBuffer;

			int noVertices = this.TerrainVertexBuffer.VertexCount;
			int noTriangles = this.TerrainIndexBuffer.IndexCount / 3;
			device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, noVertices, 0, noTriangles);
		}

		public void LoadVertices()
		{
			VertexPositionNormalTexture[] terrainVertices = this.SetUpTerrainVertices(this.HeightMap);
			int[] terrainIndices = this.SetUpTerrainIndices();
			terrainVertices = this.CalculateNormals(terrainVertices, terrainIndices);
			this.CopyToTerrainBuffers(terrainVertices, terrainIndices);
		}

		private VertexPositionNormalTexture[] SetUpTerrainVertices(float[,] heightMap)
		{
			VertexPositionNormalTexture[] terrainVertices = new VertexPositionNormalTexture[heightMap.Length];

			for (int x = 0; x < this.Size; x++)
			{
				for (int y = 0; y < this.Size; y++)
				{
					terrainVertices[x + y * this.Size].Position = new Vector3(x, heightMap[x, y], -y);
					terrainVertices[x + y * this.Size].TextureCoordinate.X = (float) (x + (this.TileX * this.Size)) / (30);
					terrainVertices[x + y * this.Size].TextureCoordinate.Y = (float) (y + (this.TileZ * this.Size)) / (30);
				}
			}

			return terrainVertices;
		}

		private int[] SetUpTerrainIndices()
		{
			int[] indices = new int[(this.Size - 1) * (this.Size - 1) * 6];
			int counter = 0;
			for (int y = 0; y < this.Size - 1; y++)
			{
				for (int x = 0; x < this.Size - 1; x++)
				{
					int lowerLeft = x + y * this.Size;
					int lowerRight = (x + 1) + y * this.Size;
					int topLeft = x + (y + 1) * this.Size;
					int topRight = (x + 1) + (y + 1) * this.Size;

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
			this.TerrainVertexBuffer = new VertexBuffer(Game.Instance.GraphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
			this.TerrainVertexBuffer.SetData(vertices);

			this.TerrainIndexBuffer = new IndexBuffer(Game.Instance.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
			this.TerrainIndexBuffer.SetData(indices);
		}
	}
}
