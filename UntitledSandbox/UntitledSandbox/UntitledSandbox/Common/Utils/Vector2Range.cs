using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Common.Utils
{
	public class Vector2Range
	{
		public Vector2 Size { get { return this.Max - this.Min; } }

		public Vector2 Min { get; private set; }
		public Vector2 Max { get; private set; }

		private FloatRange RangeX { get; set; }
		private FloatRange RangeY { get; set; }

		public Vector2Range(float x1, float y1, float x2, float y2)
		{
			this.RangeX = new FloatRange(x1, x2);
			this.RangeY = new FloatRange(y1, y2);

			this.Min = new Vector2(this.RangeX.Min, this.RangeY.Min);
			this.Max = new Vector2(this.RangeX.Max, this.RangeY.Max);
		}

		public Vector2Range(Vector2 a, Vector2 b) : this(a.X, a.Y, b.X, b.Y)
		{
		}

		public bool Contains(Vector2 f)
		{
			return this.RangeX.Contains(f.X) && this.RangeY.Contains(f.Y);
		}
	}
}
