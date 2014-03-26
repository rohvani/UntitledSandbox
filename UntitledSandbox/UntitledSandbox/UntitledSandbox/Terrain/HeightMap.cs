using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UntitledSandbox.Terrain
{
	public class HeightMap
	{
		public float[,] Heights { get; set; }
		public int Size { get; set; }

		private PerlinGenerator Perlin { get; set; }

		public HeightMap(int size)
		{
			this.Size = size;
			this.Heights = new float[this.Size, this.Size];
			this.Perlin = new PerlinGenerator(32546);
		}

		public HeightMap() : this(641)
		{
		}

		// flat: 1.0f hilly: 4.0 craggy: 1.0 + 8.0
		public void AddPerlinNoise(float f)
		{
			for (int i = 0; i < this.Size; i++)
			{
				for (int j = 0; j < this.Size; j++)
				{
					this.Heights[i, j] += this.Perlin.Noise(f * i / (float) this.Size, f * j / (float) this.Size, 0);
				}
			}
		}

		public void Perturb(float f, float d)
		{
			int u, v;
			float[,] temp = new float[this.Size, this.Size];
			for (int i = 0; i < this.Size; ++i)
			{
				for (int j = 0; j < this.Size; ++j)
				{
					u = i + (int) (this.Perlin.Noise(f * i / (float) this.Size, f * j / (float) this.Size, 0) * d);
					v = j + (int) (this.Perlin.Noise(f * i / (float) this.Size, f * j / (float) this.Size, 1) * d);
					if (u < 0) u = 0;
					if (u >= this.Size) u = this.Size - 1;
					if (v < 0) v = 0;
					if (v >= this.Size) v = this.Size - 1;
					temp[i, j] = this.Heights[u, v];
				}
			}
			this.Heights = temp;
		}

		public void Erode(float smoothness)
		{
			for (int i = 1; i < this.Size - 1; i++)
			{
				for (int j = 1; j < this.Size - 1; j++)
				{
					float d_max = 0.0f;
					int[] match = { 0, 0 };

					for (int u = -1; u <= 1; u++)
					{
						for (int v = -1; v <= 1; v++)
						{
							if (Math.Abs(u) + Math.Abs(v) > 0)
							{
								float d_i = this.Heights[i, j] - this.Heights[i + u, j + v];
								if (d_i > d_max)
								{
									d_max = d_i;
									match[0] = u;
									match[1] = v;
								}
							}
						}
					}

					if (0 < d_max && d_max <= (smoothness / (float) this.Size))
					{
						float d_h = 0.5f * d_max;
						this.Heights[i, j] -= d_h;
						this.Heights[i + match[0], j + match[1]] += d_h;
					}
				}
			}
		}

		public void Smoothen()
		{
			for (int i = 1; i < this.Size - 1; ++i)
			{
				for (int j = 1; j < this.Size - 1; ++j)
				{
					float total = 0.0f;
					for (int u = -1; u <= 1; u++)
					{
						for (int v = -1; v <= 1; v++)
						{
							total += this.Heights[i + u, j + v];
						}
					}

					this.Heights[i, j] = total / 9.0f;
				}
			}
		}

		public void Normalize()
		{
			float largest = 0;
			for (int i = 0; i < this.Size; i++)
			{
				for (int j = 0; j < this.Size; j++)
				{
					if (this.Heights[i, j] > largest)
						largest = this.Heights[i, j];
				}
			}

			for (int i = 0; i < this.Size; i++)
			{
				for (int j = 0; j < this.Size; j++)
				{
					this.Heights[i, j] /= largest;
				}
			}
		}
	}
}
