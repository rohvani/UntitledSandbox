using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UntitledSandbox.Common.Utils
{
	public static class FrustumHelper
	{
		public static Vector3 GetNegativeVertex(this BoundingBox aabb, ref Vector3 normal)
		{
			Vector3 p = aabb.Max;
			if (normal.X >= 0)
				p.X = aabb.Min.X;
			if (normal.Y >= 0)
				p.Y = aabb.Min.Y;
			if (normal.Z >= 0)
				p.Z = aabb.Min.Z;

			return p;
		}

		public static Vector3 GetPositiveVertex(this BoundingBox aabb, ref Vector3 normal)
		{
			Vector3 p = aabb.Min;
			if (normal.X >= 0)
				p.X = aabb.Max.X;
			if (normal.Y >=0)
				p.Y = aabb.Max.Y;
			if (normal.Z >= 0)
				p.Z = aabb.Max.Z;

			return p;
		}


		// TODO: Calculate the normals once when the frustum changes instead of recalculating them every draw
		public static bool FastIntersects(this BoundingFrustum boundingfrustum, ref BoundingBox aabb)
		{
			Plane plane;
			Vector3 normal, p;

			plane = boundingfrustum.Bottom;
			normal = plane.Normal;
			normal.X = -normal.X;
			normal.Y = -normal.Y;
			normal.Z = -normal.Z;
			p = aabb.GetPositiveVertex(ref normal);
			if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
				return false;

			plane = boundingfrustum.Far;
			normal = plane.Normal;
			normal.X = -normal.X;
			normal.Y = -normal.Y;
			normal.Z = -normal.Z;
			p = aabb.GetPositiveVertex(ref normal);
			if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
				return false;

			plane = boundingfrustum.Left;
			normal = plane.Normal;
			normal.X = -normal.X;
			normal.Y = -normal.Y;
			normal.Z = -normal.Z;
			p = aabb.GetPositiveVertex(ref normal);
			if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
				return false;

			plane = boundingfrustum.Near;
			normal = plane.Normal;
			normal.X = -normal.X;
			normal.Y = -normal.Y;
			normal.Z = -normal.Z;
			p = aabb.GetPositiveVertex(ref normal);
			if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
				return false;

			plane = boundingfrustum.Right;
			normal = plane.Normal;
			normal.X = -normal.X;
			normal.Y = -normal.Y;
			normal.Z = -normal.Z;
			p = aabb.GetPositiveVertex(ref normal);
			if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
				return false;

			plane = boundingfrustum.Top;
			normal = plane.Normal;
			normal.X = -normal.X;
			normal.Y = -normal.Y;
			normal.Z = -normal.Z;
			p = aabb.GetPositiveVertex(ref normal);
			if (-plane.D + normal.X * p.X + normal.Y * p.Y + normal.Z * p.Z < 0)
				return false;

			return true;
		}
	}
}
