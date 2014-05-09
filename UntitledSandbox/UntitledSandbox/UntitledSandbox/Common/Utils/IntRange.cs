using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UntitledSandbox.Common.Utils
{
	public class IntRange
	{
		public int Min { get; private set; }
		public int Max { get; private set; }

		public int Size { get { return this.Max - this.Min; } }

		public IntRange(int a, int b)
		{
			this.Min = Math.Min(a, b);
			this.Max = Math.Max(a, b);
		}

		public bool Contains(int i)
		{
			return i >= this.Min && i <= this.Max;
		}

		public int Clamp(int i)
		{
			if (i <= this.Min) return this.Min;
			if (i >= this.Max) return this.Max;
			return i;
		}
	}
}
