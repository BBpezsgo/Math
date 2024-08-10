using System;
using System.Numerics;

namespace Maths
{
    public struct Triangle3 :
        IAdditionOperators<Triangle3, Vector3, Triangle3>,
        ISubtractionOperators<Triangle3, Vector3, Triangle3>
    {
        public Vector3 A, B, C;

        public Triangle3(Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
        }

#if UNITY

        public Triangle3(UnityEngine.Vector3 a, UnityEngine.Vector3 b, UnityEngine.Vector3 c)
        {
            A = a.To();
            B = b.To();
            C = c.To();
        }

#endif

        /// <summary>
        /// Source: <see href="https://forum.unity.com/threads/closest-point-on-mesh-collider.34660/"/>
        /// </summary>
        public readonly Vector3 NearestPoint(Vector3 pt)
        {
            Vector3 edge1 = B - A;
            Vector3 edge2 = C - A;
            Vector3 edge3 = C - B;
            float edge1Len = edge1.Length();
            float edge2Len = edge2.Length();
            float edge3Len = edge3.Length();

            Vector3 ptLineA = pt - A;
            Vector3 ptLineB = pt - B;
            Vector3 ptLineC = pt - C;
            Vector3 xAxis = edge1 / edge1Len;
            Vector3 zAxis = Vector3.Cross(edge1, edge2).Normalized();
            Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

            Vector3 edge1Cross = Vector3.Cross(edge1, ptLineA);
            Vector3 edge2Cross = Vector3.Cross(edge2, -ptLineC);
            Vector3 edge3Cross = Vector3.Cross(edge3, ptLineB);
            bool edge1On = Vector3.Dot(edge1Cross, zAxis) > 0f;
            bool edge2On = Vector3.Dot(edge2Cross, zAxis) > 0f;
            bool edge3On = Vector3.Dot(edge3Cross, zAxis) > 0f;

            // If the point is inside the triangle then return its coordinate.
            if (edge1On && edge2On && edge3On)
            {
                float xExtent = Vector3.Dot(ptLineA, xAxis);
                float yExtent = Vector3.Dot(ptLineA, yAxis);
                return A + (xAxis * xExtent) + (yAxis * yExtent);
            }

            // Otherwise, the nearest point is somewhere along one of the edges.
            Vector3 edge1Norm = xAxis;
            Vector3 edge2Norm = edge2.Normalized();
            Vector3 edge3Norm = edge3.Normalized();

            float edge1Ext = Math.Clamp(Vector3.Dot(edge1Norm, ptLineA), 0f, edge1Len);
            float edge2Ext = Math.Clamp(Vector3.Dot(edge2Norm, ptLineA), 0f, edge2Len);
            float edge3Ext = Math.Clamp(Vector3.Dot(edge3Norm, ptLineB), 0f, edge3Len);

            Vector3 edge1Pt = A + (edge1Ext * edge1Norm);
            Vector3 edge2Pt = A + (edge2Ext * edge2Norm);
            Vector3 edge3Pt = B + (edge3Ext * edge3Norm);

            float sqDistance1 = (pt - edge1Pt).LengthSquared();
            float sqDistance2 = (pt - edge2Pt).LengthSquared();
            float sqDistance3 = (pt - edge3Pt).LengthSquared();

            if (sqDistance1 < sqDistance2)
            {
                if (sqDistance1 < sqDistance3)
                { return edge1Pt; }
                else
                { return edge3Pt; }
            }
            else if (sqDistance2 < sqDistance3)
            { return edge2Pt; }
            else
            { return edge3Pt; }
        }

        public static int ClipAgainstPlane(Vector3 planePoint, Vector3 planeNormal, Triangle3 triangleIn, out Triangle3 triangleOut1, out Triangle3 triangleOut2)
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

            float d0 = Distance(triangleIn.A);
            float d1 = Distance(triangleIn.B);
            float d2 = Distance(triangleIn.C);

            if (d0 >= 0f)
            { insidePoints[insidePointCount++] = triangleIn.A; }
            else
            { outsidePoints[outsidePointCount++] = triangleIn.A; }

            if (d1 >= 0f)
            { insidePoints[insidePointCount++] = triangleIn.B; }
            else
            { outsidePoints[outsidePointCount++] = triangleIn.B; }

            if (d2 >= 0f)
            { insidePoints[insidePointCount++] = triangleIn.C; }
            else
            { outsidePoints[outsidePointCount++] = triangleIn.C; }

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

                triangleOut1.A = insidePoints[0];
                triangleOut1.B = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]);
                triangleOut1.C = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[1]);

                return 1;
            }

            if (insidePointCount == 2 && outsidePointCount == 1)
            {
                triangleOut1.A = insidePoints[0];
                triangleOut1.B = insidePoints[1];
                triangleOut1.C = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[0], outsidePoints[0]);

                triangleOut2.A = insidePoints[1];
                triangleOut2.B = triangleOut1.C;
                triangleOut2.C = Vector.IntersectPlane(planePoint, planeNormal, insidePoints[1], outsidePoints[0]);

                return 2;
            }

            return 0;
        }

        public static explicit operator Triangle3(Triangle4 triangle) => new(triangle.A.To3(), triangle.B.To3(), triangle.C.To3());

        public static Triangle3 operator +(Triangle3 tri, Vector3 vec)
        {
            tri.A += vec;
            tri.B += vec;
            tri.C += vec;
            return tri;
        }

        public static Triangle3 operator -(Triangle3 tri, Vector3 vec)
        {
            tri.A -= vec;
            tri.B -= vec;
            tri.C -= vec;
            return tri;
        }
    }
}
