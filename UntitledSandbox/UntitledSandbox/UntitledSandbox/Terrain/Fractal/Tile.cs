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

		public float NW
		{
			get { return this.HeightMap[0, this.SubSize]; }
			set { this.HeightMap[0, this.SubSize] = value; }
		}

		public float NE
		{
			get { return this.HeightMap[this.SubSize, this.SubSize]; }
			set { this.HeightMap[this.SubSize, this.SubSize] = value; }
		}

		public float SW
		{
			get { return this.HeightMap[0, 0]; }
			set { this.HeightMap[0, 0] = value; }
		}

		public float SE
		{
			get { return this.HeightMap[this.SubSize, 0]; }
			set { this.HeightMap[this.SubSize, 0] = value; }
		}

		public int Size { get; set; }
		private int SubSize { get { return this.Size - 1; } }

		public int TileX { get; set; }
		public int TileZ { get; set; }

		public int X { get { return this.TileX * this.SubSize; } }
		public int Z { get { return this.TileZ * this.SubSize; } }

		public Matrix TranslationMatrix { get { return Matrix.CreateTranslation(this.X * 2, 0, this.Z * 2); } }

		public int Seed { get; set; }
		public float HeightScale { get; set; }
		public float Roughness { get; set; }

		private VertexBuffer TerrainVertexBuffer { get; set; }
		private IndexBuffer TerrainIndexBuffer { get; set; }

		public BoundingBox Bounds;
		public bool IsInView
		{
			//get { return this.Bounds.Contains(Game.Instance.Player.Camera.Frustum) != ContainmentType.Disjoint; }
			get { return Game.Instance.Player.Camera.Frustum.FastIntersects(ref this.Bounds); }
			//get { return true; }
		}

		public Tile(int size, int x, int z)
		{
			this.TileX = x;
			this.TileZ = z;

			this.Size = size;
			this.HeightMap = NewMap();

			this.Seed = 43465647;
			this.HeightScale = 100f;
			this.Roughness = 0.99f;
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

		public void SeedMap(Tile north, Tile east, Tile south, Tile west)
		{
			for (int i = 0; i < this.Size; i++)
			{
				if (north != null)
					this.HeightMap[i, this.SubSize] = north.HeightMap[i, 0];

				if (east != null)
					this.HeightMap[this.SubSize, i] = east.HeightMap[0, i];

				if (south != null)
					this.HeightMap[i, 0] = south.HeightMap[i, this.SubSize];

				if (west != null)
					this.HeightMap[0, i] = west.HeightMap[this.SubSize, i];
			}

			SeedHeightScale(north, east, south, west);

			SeedCorners();

			if (north == null) SeedNorth();
			if (east == null) SeedEast();
			if (south == null) SeedSouth();
			if (west == null) SeedWest();
		}

		private void SeedCorners()
		{
			float[] corners = { NE, NW, SE, SW };
			float[] seededCorners = corners.Where(p => p != -255).ToArray();
			int seeds = seededCorners.Length;
			if (seeds == 0)
				seeds = 1;
			float avg = seededCorners.Sum() / ((float) seeds);

			float initialScale = 50f;

			if (seeds == 1)
			{
				NE = (float) new Random().NextDouble() * initialScale;
				NW = (float) new Random().NextDouble() * initialScale;
				SE = (float) new Random().NextDouble() * initialScale;
				SW = (float) new Random().NextDouble() * initialScale;
			}

			if (seeds == 2)
			{
				float diff = Math.Abs(seededCorners[0] - seededCorners[1]);
				if (NE == -255)
					NE = ((float) new Random().NextDouble() * 4 - 2) * diff + avg;
				if (NW == -255)
					NW = ((float) new Random().NextDouble() * 4 - 2) * diff + avg;
				if (SE == -255)
					SE = ((float) new Random().NextDouble() * 4 - 2) * diff + avg;
				if (SW == -255)
					SW = ((float) new Random().NextDouble() * 4 - 2) * diff + avg;
			}
			else if (seeds == 3)
			{
				int i = 0;
				for (; i < corners.Length; i++)
				{
					if (corners[i] == -255)
						break;
				}

				switch (i)
				{
					case 0:
						NE = avg * ((float) new Random().NextDouble() + 0.5f);
						break;
					case 1:
						NW = avg * ((float) new Random().NextDouble() + 0.5f);
						break;
					case 2:
						SE = avg * ((float) new Random().NextDouble() + 0.5f);
						break;
					case 3:
						SW = avg * ((float) new Random().NextDouble() + 0.5f);
						break;
				}
			}
		}

		private void SeedHeightScale(params Tile[] tiles)
		{
			float[] heights = tiles.Select(p => TryGetHeightScale(p)).ToArray<float>();
			float[] realHeights = heights.Where(p => p != -255).ToArray();
			int num = realHeights.Length;
			if (num == 0)
				return;
			float avg = realHeights.Sum() / ((float) num);

			if (num == 1)
			{
				this.HeightScale = avg * ((float) new Random().NextDouble() / 2 + 0.75f);
			}
			else if (num == 2)
			{
				//float diff = Math.Abs(realHeights[0] - realHeights[1]);
				//this.HeightScale = ((float) new Random().NextDouble() * 3f - 1.5f) * diff + avg;

				this.HeightScale = avg * ((float) new Random().NextDouble() + 0.5f);
			}
		}

		public void SeedNorth()
		{
			float[] array = new float[this.Size];
			array[0] = NW;
			array[this.SubSize] = NE;
			float[] n = FractalUtils.Fill1DFractArray(array, this.Size, this.Seed, this.HeightScale, this.Roughness);

			for (int i = 0; i < this.Size; i++)
				this.HeightMap[i, this.SubSize] = n[i];
		}

		public void SeedEast()
		{
			float[] array = new float[this.Size];
			array[0] = SE;
			array[this.SubSize] = NE;
			float[] n = FractalUtils.Fill1DFractArray(array, this.SubSize, this.Seed, this.HeightScale, this.Roughness);

			for (int i = 0; i < this.Size; i++)
				this.HeightMap[this.SubSize, i] = n[i];
		}

		public void SeedSouth()
		{

			float[] array = new float[this.Size];
			array[0] = SW;
			array[this.SubSize] = SE;
			float[] n = FractalUtils.Fill1DFractArray(array, this.SubSize, this.Seed, this.HeightScale, this.Roughness);

			for (int i = 0; i < this.Size; i++)
				this.HeightMap[i, 0] = n[i];
		}

		public void SeedWest()
		{
			float[] array = new float[this.Size];
			array[0] = SW;
			array[this.SubSize] = NW;
			float[] n = FractalUtils.Fill1DFractArray(array, this.SubSize, this.Seed, this.HeightScale, this.Roughness);

			for (int i = 0; i < this.Size; i++)
				this.HeightMap[0, i] = n[i];
		}

		private float TryGetHeightScale(Tile t)
		{
			try
			{
				return t.HeightScale;
			}
			catch (NullReferenceException)
			{
				return -255;
			}
		}

		public void GenerateMap()
		{
			this.HeightMap = FractalUtils.Fill2DFractArray(this.HeightMap, this.SubSize, this.Seed, this.HeightScale, this.Roughness);

			IEnumerable<float> vals = this.HeightMap.Cast<float>();

			Vector3 min = new Vector3(this.X * 2, vals.Min(), this.Z * 2);
			Vector3 max = new Vector3((this.X + this.SubSize) * 2, vals.Max(), (this.Z + this.SubSize) * 2);
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
					terrainVertices[x + y * this.Size].Position = new Vector3(x * 2, heightMap[x, y], y * 2);
					terrainVertices[x + y * this.Size].TextureCoordinate.X = (float) (x + (this.TileX * this.Size)) / (30) * 2;
					terrainVertices[x + y * this.Size].TextureCoordinate.Y = (float) (y + (this.TileZ * this.Size)) / (30) * 2;
				}
			}

			return terrainVertices;
		}

		private int[] SetUpTerrainIndices()
		{
			int[] indices = new int[this.SubSize * this.SubSize * 6];
			int counter = 0;
			for (int y = 0; y < this.SubSize; y++)
			{
				for (int x = 0; x < this.SubSize; x++)
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
