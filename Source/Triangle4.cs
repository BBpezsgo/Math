using System;
using System.Numerics;

namespace Maths
{
    public struct Triangle4 :
        IAdditionOperators<Triangle4, Vector4, Triangle4>,
        ISubtractionOperators<Triangle4, Vector4, Triangle4>
    {
        public Vector4 A, B, C;

        public Triangle4(Vector3 a, Vector3 b, Vector3 c) : this(a.To4(), b.To4(), c.To4()) { }
        public Triangle4(Vector4 a, Vector4 b, Vector4 c)
        {
            A = a;
            B = b;
            C = c;
        }

        public static int ClipAgainstPlane(Vector3 planePoint, Vector3 planeNormal, Triangle4 triangleIn, out Triangle4 triangleOut1, out Triangle4 triangleOut2)
        {
            triangleOut1 = default;
            triangleOut2 = default;

            planeNormal = Vector3.Normalize(planeNormal);

            // Return signed shortest distance from point to plane, plane normal must be normalized
            float Distance(Vector3 p) =>
                (planeNormal.X * p.X) +
                (planeNormal.Y * p.Y) +
                (planeNormal.Z * p.Z) -
                Vector3.Dot(planeNormal, planePoint);

            Span<Vector3> insidePoints = stackalloc Vector3[3];
            int insidePointCount = 0;

            Span<Vector3> outsidePoints = stackalloc Vector3[3];
            int outsidePointCount = 0;

            float d0 = Distance(triangleIn.A.To3());
            float d1 = Distance(triangleIn.B.To3());
            float d2 = Distance(triangleIn.C.To3());

            if (d0 >= 0f)
            { insidePoints[insidePointCount++] = triangleIn.A.To3(); }
            else
            { outsidePoints[outsidePointCount++] = triangleIn.A.To3(); }

            if (d1 >= 0f)
            { insidePoints[insidePointCount++] = triangleIn.B.To3(); }
            else
            { outsidePoints[outsidePointCount++] = triangleIn.B.To3(); }

            if (d2 >= 0f)
            { insidePoints[insidePointCount++] = triangleIn.C.To3(); }
            else
            { outsidePoints[outsidePointCount++] = triangleIn.C.To3(); }

            if (insidePointCount == 0)
            {
                return 0;
            }

            if (insidePointCount == 3)
            {
                triangleOut1 = triangleIn;
                return 1;
            }

            if (insidePointCount == 1 && outsidePointCount == 2)
            {
                triangleOut1 = triangleIn;

                triangleOut1.A = insidePoints[0].To4();
                triangleOut1.B = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]).To4();
                triangleOut1.C = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[1]).To4();

                return 1;
            }

            if (insidePointCount == 2 && outsidePointCount == 1)
            {
                triangleOut1.A = insidePoints[0].To4();
                triangleOut1.B = insidePoints[1].To4();
                triangleOut1.C = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]).To4();

                triangleOut2.A = insidePoints[1].To4();
                triangleOut2.B = triangleOut1.C;
                triangleOut2.C = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[1], outsidePoints[0]).To4();

                return 2;
            }

            return 0;
        }

        public static explicit operator Triangle4(Triangle3 triangle) => new(triangle.A.To4(), triangle.B.To4(), triangle.C.To4());

        public static Triangle4 operator +(Triangle4 tri, Vector4 vec)
        {
            tri.A += vec;
            tri.B += vec;
            tri.C += vec;
            return tri;
        }

        public static Triangle4 operator -(Triangle4 tri, Vector4 vec)
        {
            tri.A -= vec;
            tri.B -= vec;
            tri.C -= vec;
            return tri;
        }
    }
}
