using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UntitledSandbox.Common.Utils
{
	public class FloatRange
	{	
		public float Min { get; private set; }
		public float Max { get; private set; }

		public float Size { get { return this.Max - this.Min; } }

		public FloatRange(float a, float b)
		{
			this.Min = Math.Min(a, b);
			this.Max = Math.Max(a, b);
		}

		public bool Contains(float f)
		{
			return f >= this.Min && f <= this.Max;
		}
	}
}
