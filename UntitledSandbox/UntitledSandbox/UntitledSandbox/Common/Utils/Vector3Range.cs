using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Common.Utils
{
	public class Vector3Range
	{
		public Vector3 Size { get { return this.Max - this.Min; } }

		public Vector3 Min { get; private set; }
		public Vector3 Max { get; private set; }

		private FloatRange RangeX { get; set; }
		private FloatRange RangeY { get; set; }
		private FloatRange RangeZ { get; set; }

		public Vector3Range(float x1, float y1, float z1, float x2, float y2, float z2)
		{
			this.RangeX = new FloatRange(x1, x2);
			this.RangeY = new FloatRange(y1, y2);
			this.RangeZ = new FloatRange(z1, z2);

			this.Min = new Vector3(this.RangeX.Min, this.RangeY.Min, this.RangeZ.Min);
			this.Max = new Vector3(this.RangeX.Max, this.RangeY.Max, this.RangeZ.Max);
		}
		
		public Vector3Range(Vector3 a, Vector3 b) : this(a.X, a.Y, a.Z, b.X, b.Y, b.Z)
		{
		}

		public bool Contains(Vector3 f)
		{
			return this.RangeX.Contains(f.X) && this.RangeY.Contains(f.Y) && this.RangeZ.Contains(f.Z);
		}
	}
}
