using System;
using System.Numerics;

namespace Maths
{
    public struct Line
    {
        public Vector2 PointA;
        public Vector2 PointB;

        public Line(Vector2 pointA, Vector2 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        public static implicit operator (Vector2, Vector2)(Line v)
            => (v.PointA, v.PointB);
        public static implicit operator Line((Vector2, Vector2) v)
            => (v.Item1, v.Item2);

        public static Line operator +(Line a, Vector2 b)
            => new(a.PointA + b, a.PointB + b);
        public static Line operator -(Line a, Vector2 b)
            => new(a.PointA - b, a.PointB - b);
        public static Line operator *(Line a, Vector2 b)
            => new(a.PointA * b, a.PointB * b);
        public static Line operator *(Line a, float b)
            => new(a.PointA * b, a.PointB * b);

        public static Vector2? Line2LineIntersection(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            // Line AB represented as a1x + b1y = c1
            float a1 = b.Y - a.Y;
            float b1 = a.X - b.X;
            float c1 = (a1 * a.X) + (b1 * a.Y);

            // Line CD represented as a2x + b2y = c2
            float a2 = d.Y - c.Y;
            float b2 = c.Y - d.X;
            float c2 = (a2 * c.X) + (b2 * c.Y);

            float determinant = (a1 * b2) - (a2 * b1);

            if (determinant == 0)
            {
                // The lines are parallel. This is simplified
                // by returning a pair of FLT_MAX
                return null;
            }
            else
            {
                float x = ((b2 * c1) - (b1 * c2)) / determinant;
                float y = ((a1 * c2) - (a2 * c1)) / determinant;
                return new Vector2(x, y);
            }
        }

        public static bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
        {
            float crossProduct = ((c.Y - a.Y) * (b.X - a.X)) - ((c.X - a.X) * (b.Y - a.Y));

            // compare versus epsilon for floating point values, or != 0 if using integers
            if (Math.Abs(crossProduct) > 0.0001f)
            { return false; }

            float dotProduct = ((c.X - a.X) * (b.X - a.X)) + ((c.Y - a.Y) * (b.Y - a.Y));
            if (dotProduct < 0)
            { return false; }

            float squaredLengthBA = ((b.X - a.X) * (b.X - a.X)) + ((b.Y - a.Y) * (b.Y - a.Y));
            if (dotProduct > squaredLengthBA)
            { return false; }

            return true;
        }
    }
}
