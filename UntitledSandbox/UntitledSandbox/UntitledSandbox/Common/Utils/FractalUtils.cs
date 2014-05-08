using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UntitledSandbox.Common.Utils
{
	class FractalUtils
	{
		public static bool Debug = false;
		private static Random random;
		private static int RAND_MAX = 32767;

		/*
		 * RandNum - Return a random floating point number such that
		 *      (min <= return-value <= max)
		 * 32,767 values are possible for any given range.
		 */
		static float RandNum(float min, float max)
		{
			int r;
			float x;
    
			r = random.Next(RAND_MAX);
			
			x = (float)(r & 0x7fff) / (float)0x7fff;
			return (x * (max - min) + min);
		} 
		
		/*
		 * FractRand is a useful interface to RandNum.
		 */
		static float FractRand(float f)
		{
			return RandNum(-f, f);
		}

		/*
		 * AvgEndpoints - Given the i location and a stride to the data
		 * values, return the average those data values. "i" can be thought of
		 * as the data value in the center of two line endpoints. We use
		 * "stride" to get the values of the endpoints. Averaging them yields
		 * the midpoint of the line.
		 *
		 * Called by fill1DFractArray.
		 */
		static float AvgEndpoints(int i, int stride, float[] array)
		{
			return (float) (array[i-stride] + array[i+stride]) * .5f;
		}

		/*
		 * AvgDiamondVals - Given the i,j location as the center of a diamond,
		 * average the data values at the four corners of the diamond and
		 * return it. "Stride" represents the distance from the diamond center
		 * to a diamond corner.
		 *
		 * Called by fill2DFractArray.
		 */
		static float AvgDiamondVals(int i, int j, int stride, int size, int subSize, float[,] array)
		{
			/* In this diagram, our input stride is 1, the i,j location is
			   indicated by "X", and the four value we want to average are
			   "*"s:
				   .   *   .

				   *   X   *

				   .   *   .
			   */

			/* In order to support tiled surfaces which meet seamless at the
			   edges (that is, they "wrap"), We need to be careful how we
			   calculate averages when the i,j diamond center lies on an edge
			   of the array. The first four 'if' clauses handle these
			   cases. The final 'else' clause handles the general case (in
			   which i,j is not on an edge).
			 */
			if (i == 0)
			return ((float) (array[i, j-stride] +
					 array[i, j+stride] +
					 array[subSize-stride, j] +
					 array[i+stride, j]) * .25f);
			else if (i == size-1)
			return ((float) (array[i, j-stride] +
					 array[i, j+stride] +
					 array[i-stride, j] +
					 array[0+stride, j]) * .25f);
			else if (j == 0)
			return ((float) (array[i-stride, j] +
					 array[i+stride, j] +
					 array[i, j+stride] +
					 array[i, subSize-stride]) * .25f);
			else if (j == size-1)
			return ((float) (array[i-stride, j] +
					 array[i+stride, j] +
					 array[i, j-stride] +
					 array[i, 0+stride]) * .25f);
			else
			return ((float) (array[i-stride, j] +
					 array[i+stride, j] +
					 array[i, j-stride] +
					 array[i, j+stride]) * .25f);
		}


		/*
		 * AvgSquareVals - Given the i,j location as the center of a square,
		 * average the data values at the four corners of the square and return
		 * it. "Stride" represents half the length of one side of the square.
		 *
		 * Called by fill2DFractArray.
		 */
		static float AvgSquareVals(int i, int j, int stride, int size, float[,] array)
		{
			/* In this diagram, our input stride is 1, the i,j location is
			   indicated by "*", and the four value we want to average are
			   "X"s:
				   X   .   X

				   .   *   .

				   X   .   X
			   */
			return ((float) (array[i-stride, j-stride] +
					 array[i-stride, j+stride] +
					 array[i+stride, j-stride] +
					 array[i+stride, j+stride]) * .25f);
		}

#region Debug
		/*
		 * Dump1DFractArray - Use for debugging.
		 */
		static void Dump1DFractArray (float[] array, int size)
		{
			for (int i = 0; i < size; i++)
				Console.WriteLine(array[i].ToString("n2")); // "(%.2f)" 
		}

		/*
		 * Dump2DFractArray - Use for debugging.
		 */
		static void Dump2DFractArray (float[] array, int size)
		{
			for (int i = 0; i < size; i++)
			{
				int j = 0;
				Console.Write("[{0},{1}]: ", i, j);

				for (; j < size; j++)
					Console.Write(array[(i*size)+j].ToString("n2")); // "(%.2f)"

				Console.WriteLine();
			}
		} 
#endregion

		/*
		 * PowerOf2 - Returns true if size is a power of 2. Returns false if size is
		 * not a power of 2, or is zero.
		 */
		static bool PowerOf2(int size)
		{
			int i, bitcount = 0;

			/* Note this code assumes that (sizeof(int)*8) will yield the
			   number of bits in an int. Should be portable to most
			   platforms. */
			for (i=0; i < sizeof(int) * 8; i++)
				if ((size & (1 << i)) == 1) bitcount++;

			if (bitcount == 1) 
				return true; /* One bit. Must be a power of 2. */
			else 
				return false; /* either size==0, or size not a power of 2. Sorry, Charlie. */
		}


		/*
		 * Fill1DFractArray - Tessalate an array of values into an
		 * approximation of fractal Brownian motion.
		 */
		public static void Fill1DFractArray(float[] array, int size, int seedValue, float heightScale, float h)
		{
			int	i;
			int	stride;
			int subSize;
			float ratio, scale;

			if (!PowerOf2(size) || (size==1)) 
			{
				/* We can't tesselate the array if it is not a power of 2. */
				if (Debug)
					Console.WriteLine("Error: fill1DFractArray: size {0} is not a power of 2.", size);
				return;
			}

			/* subSize is the dimension of the array in terms of connected line
			   segments, while size is the dimension in terms of number of
			   vertices. */
			subSize = size;
			size++;
    
			/* initialize random number generator */
			//srandom (seedValue);
			random = new Random(seedValue);

			if (Debug)
			{
				Console.WriteLine("initialized");
				Dump1DFractArray(array, size);
			}

			/* Set up our roughness constants.
			   Random numbers are always generated in the range 0.0 to 1.0.
			   'scale' is multiplied by the randum number.
			   'ratio' is multiplied by 'scale' after each iteration
			   to effectively reduce the randum number range.
			   */
			ratio = (float) Math.Pow(2.0f, -h);
			scale = heightScale * ratio;

			/* Seed the endpoints of the array. To enable seamless wrapping,
			   the endpoints need to be the same point. */
			stride = subSize / 2;
			array[0] = array[subSize] = 0.0f;

			if (Debug)
			{
				Console.WriteLine("seeded");
				Dump1DFractArray(array, size);
			}

			while (stride != 0) 
			{
				for (i = stride; i < subSize; i += stride) 
				{
					array[i] = scale * FractRand(.5f) + AvgEndpoints(i, stride, array);

					/* reduce random number range */
					scale *= ratio;

					i += stride;
				}
				stride >>= 1;
			}

			if (Debug)
			{
				Console.WriteLine("complete");
				Dump1DFractArray(array, size);
			}
		}

		/*
		 * Fill2DFractArray - Use the diamond-square algorithm to tessalate a
		 * grid of float values into a fractal height map.
		 */
		public static float[,] Fill2DFractArray(float[,] array, int size, int seedValue, float heightScale=1f, float h=1f)
		{
			int	x, y;
			int	stride;
			bool oddline;
			int subSize;
			float ratio, scale;

			//if (!PowerOf2(size) || (size==1))
			//{
			//    /* We can't tesselate the array if it is not a power of 2. */
		
			//    if (Debug)
			//        Console.WriteLine("Error: fill2DFractArray: size {0} is not a power of 2.", size);
		
			//    return array;
			//}

			/* subSize is the dimension of the array in terms of connected line
			   segments, while size is the dimension in terms of number of
			   vertices. */
			subSize = size;
			size++;
    
			/* initialize random number generator */
			//srandom (seedValue);
			random = new Random(seedValue);
    
			if (Debug)
			{
				Console.WriteLine("initialized");
				//Dump2DFractArray(array, size);
			}

			/* Set up our roughness constants.
			 * Random numbers are always generated in the range 0.0 to 1.0.
			 * 'scale' is multiplied by the random number.
			 * 'ratio' is multiplied by 'scale' after each iteration
			 * to effectively reduce the random number range.
			 */
			ratio = (float) Math.Pow(2.0f, -h);
			scale = heightScale * ratio;

			/* Seed the first four values. For example, in a 4x4 array, we
			   would initialize the data points indicated by '*':

				   *   .   .   .   *

				   .   .   .   .   .

				   .   .   .   .   .

				   .   .   .   .   .

				   *   .   .   .   *

			   In terms of the "diamond-square" algorithm, this gives us
			   "squares".

			   We want the four corners of the array to have the same
			   point. This will allow us to tile the arrays next to each other
			   such that they join seemlessly. */

			stride = subSize / 2;
			array[0, 0] = 
				array[subSize, 0] =
				array[subSize, subSize] =
				  array[0, subSize] = 0.0f;
    
			if (Debug)
			{
				Console.WriteLine("seeded");
				//Dump2DFractArray(array, size);
			}

			/* Now we add ever-increasing detail based on the "diamond" seeded
			   values. We loop over stride, which gets cut in half at the
			   bottom of the loop. Since it's an int, eventually division by 2
			   will produce a zero result, terminating the loop. */
			while (stride != 0)
			{
				/* Take the existing "square" data and produce "diamond"
				   data. On the first pass through with a 4x4 matrix, the
				   existing data is shown as "X"s, and we need to generate the
				   "*" now:

					   X   .   .   .   X

					   .   .   .   .   .

					   .   .   *   .   .

					   .   .   .   .   .

					   X   .   .   .   X

				  It doesn't look like diamonds. What it actually is, for the
				  first pass, is the corners of four diamonds meeting at the
				  center of the array. */
				for (x = stride; x < subSize; x += stride)
				{
					for (y = stride; y < subSize; y += stride)
					{
						//array[(i * size) + j] = scale * FractRand(.5f) + AvgSquareVals(i, j, stride, size, array);
						if (array[x, y] == -255)
							array[x, y] = scale * FractRand(.5f) + AvgSquareVals(x, y, stride, size, array);
						y += stride;
					}
					x += stride;
				}

				if (Debug)
				{
					Console.WriteLine("Diamonds:");
					//Dump2DFractArray(array, size);
				}

				/* Take the existing "diamond" data and make it into
				   "squares". Back to our 4X4 example: The first time we
				   encounter this code, the existing values are represented by
				   "X"s, and the values we want to generate here are "*"s:

					   X   .   *   .   X

					   .   .   .   .   .

					   *   .   X   .   *

					   .   .   .   .   .

					   X   .   *   .   X

				   i and j represent our (x,y) position in the array. The
				   first value we want to generate is at (i=2,j=0), and we use
				   "oddline" and "stride" to increment j to the desired value.
				   */
				oddline = false;
				for (x = 0; x < subSize; x += stride)
				{
					oddline = !oddline;

					for (y = 0; y < subSize; y += stride)
					{
						if ((oddline) && (y == 0)) y += stride;

						/* i and j are setup. Call avgDiamondVals with the
						   current position. It will return the average of the
						   surrounding diamond data points. */
						if (array[x, y] == -255)
							array[x, y] = scale * FractRand(.5f) + AvgDiamondVals(x, y, stride, size, subSize, array);

						/* To wrap edges seamlessly, copy edge values around
						   to other side of array */
						if (x == 0 && array[subSize, y] == -255)
							array[subSize, y] = array[x, y];
						if (y == 0 && array[x, subSize] == -255)
							array[x, subSize] = array[x, y];

						y += stride;
					}
				}

				if (Debug)
				{
					Console.WriteLine("Squares:");
					//Dump2DFractArray(array, size);
				}

				/* reduce random number range. */
				scale *= ratio;
				stride >>= 1;
			}

			if (Debug)
			{
				Console.WriteLine("complete");
				//Dump2DFractArray(array, size);
			}

			return array;
		}
	}
}