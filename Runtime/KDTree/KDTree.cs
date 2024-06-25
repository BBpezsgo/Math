// KDTree.cs - A Stark, September 2009.

using System;
using System.Numerics;
using System.Text;

#nullable enable

namespace Maths
{
    /// <summary>
    /// <para>
    /// This class implements a data structure that stores a list of points in space.
    /// A common task in game programming is to take a supplied point and discover which
    /// of a stored set of points is nearest to it. For example, in path-plotting, it is often
    /// useful to know which waypoint is nearest to the player's current
    /// position. The kd-tree allows this "nearest neighbor" search to be carried out quickly,
    /// or at least much more quickly than a simple linear search through the list.
    /// </para>
    /// <para>
    /// At present, the class only allows for construction (using the <see cref="MakeFromPoints"/> static method)
    /// and nearest-neighbor searching (using <see cref="FindNearest"/>). More exotic kd-trees are possible, and
    /// this class may be extended in the future if there seems to be a need.
    /// </para>
    /// <para>
    /// The nearest-neighbor search returns an integer index - it is assumed that the original
    /// array of points is available for the lifetime of the tree, and the index refers to that
    /// array.
    /// </para>
    /// </summary>
    public class KDTree
    {
        /// <summary>
        /// Change this value to 2 if you only need two-dimensional X,Y points.
        /// The search will be quicker in two dimensions.
        /// </summary>
        const int NumDims = 3;

        KDTree? left;
        KDTree? right;
        Vector3 pivot;
        int pivotIndex;
        int axis;

        KDTree? this[int lr] => lr switch
        {
            0 => left,
            1 => right,
            _ => throw new IndexOutOfRangeException(),
        };

        /// <summary>
        /// Make a new tree from a list of points.
        /// </summary>
        public static KDTree MakeFromPoints(ReadOnlySpan<Vector3> points)
        {
            int[] indices = Iota(points.Length);
            return MakeFromPointsInner(0, 0, points.Length - 1, points, indices);
        }

        /// <summary>
        /// Make a new tree from a list of points.
        /// </summary>
        public static KDTree MakeFromPoints(params Vector3[] points)
        {
            int[] indices = Iota(points.Length);
            return MakeFromPointsInner(0, 0, points.Length - 1, points, indices);
        }

        /// <summary>
        /// Recursively build a tree by separating points at plane boundaries.
        /// </summary>
        static KDTree MakeFromPointsInner(int depth, int startIndex, int endIndex, ReadOnlySpan<Vector3> points, Span<int> indices)
        {
            KDTree root = new()
            { axis = depth % NumDims };

            int splitPoint = FindPivotIndex(points, indices, startIndex, endIndex, root.axis);

            root.pivotIndex = indices[splitPoint];
            root.pivot = points[root.pivotIndex];

            int leftEndIndex = splitPoint - 1;

            if (leftEndIndex >= startIndex)
            { root.left = MakeFromPointsInner(depth + 1, startIndex, leftEndIndex, points, indices); }

            int rightStartIndex = splitPoint + 1;

            if (rightStartIndex <= endIndex)
            { root.right = MakeFromPointsInner(depth + 1, rightStartIndex, endIndex, points, indices); }

            return root;
        }

#if UNITY

        /// <summary>
        /// Make a new tree from a list of points.
        /// </summary>
        public static KDTree MakeFromPoints(ReadOnlySpan<UnityEngine.Vector3> points)
        {
            int[] indices = Iota(points.Length);
            return MakeFromPointsInner(0, 0, points.Length - 1, points, indices);
        }

        /// <summary>
        /// Make a new tree from a list of points.
        /// </summary>
        public static KDTree MakeFromPoints(params UnityEngine.Vector3[] points)
        {
            int[] indices = Iota(points.Length);
            return MakeFromPointsInner(0, 0, points.Length - 1, points, indices);
        }

        /// <summary>
        /// Recursively build a tree by separating points at plane boundaries.
        /// </summary>
        static KDTree MakeFromPointsInner(int depth, int startIndex, int endIndex, ReadOnlySpan<UnityEngine.Vector3> points, Span<int> indices)
        {
            KDTree root = new()
            { axis = depth % NumDims };

            int splitPoint = FindPivotIndex(points, indices, startIndex, endIndex, root.axis);

            root.pivotIndex = indices[splitPoint];
            root.pivot = points[root.pivotIndex].To();

            int leftEndIndex = splitPoint - 1;

            if (leftEndIndex >= startIndex)
            { root.left = MakeFromPointsInner(depth + 1, startIndex, leftEndIndex, points, indices); }

            int rightStartIndex = splitPoint + 1;

            if (rightStartIndex <= endIndex)
            { root.right = MakeFromPointsInner(depth + 1, rightStartIndex, endIndex, points, indices); }

            return root;
        }

#endif

        static void SwapElements(Span<int> array, int indexA, int indexB)
        {
            int temp = array[indexA];
            array[indexA] = array[indexB];
            array[indexB] = temp;
        }

        /// <summary>
        /// Simple "median of three" heuristic to find a reasonable splitting plane.
        /// </summary>
        static int FindSplitPoint(ReadOnlySpan<Vector3> points, ReadOnlySpan<int> indices, int startIndex, int endIndex, int axis)
        {
            float a = points[indices[startIndex]].Axis(axis);
            float b = points[indices[endIndex]].Axis(axis);
            int midIndex = (startIndex + endIndex) / 2;
            float m = points[indices[midIndex]].Axis(axis);

            if (a > b)
            {
                if (m > a)
                { return startIndex; }

                if (b > m)
                { return endIndex; }

                return midIndex;
            }
            else
            {
                if (a > m)
                { return startIndex; }

                if (m > b)
                { return endIndex; }

                return midIndex;
            }
        }

        /// <summary>
        /// Find a new pivot index from the range by splitting the points that fall either side
        /// of its plane.
        /// </summary>
        public static int FindPivotIndex(ReadOnlySpan<Vector3> points, Span<int> indices, int startIndex, int endIndex, int axis)
        {
            int splitPoint = FindSplitPoint(points, indices, startIndex, endIndex, axis);
            // int splitPoint = Random.Range(stIndex, enIndex);

            Vector3 pivot = points[indices[splitPoint]];
            SwapElements(indices, startIndex, splitPoint);

            int currPt = startIndex + 1;
            int endPt = endIndex;

            while (currPt <= endPt)
            {
                Vector3 curr = points[indices[currPt]];

                if (curr.Axis(axis) > pivot.Axis(axis))
                {
                    SwapElements(indices, currPt, endPt);
                    endPt--;
                }
                else
                {
                    SwapElements(indices, currPt - 1, currPt);
                    currPt++;
                }
            }

            return currPt - 1;
        }

#if UNITY

        /// <summary>
        /// Simple "median of three" heuristic to find a reasonable splitting plane.
        /// </summary>
        static int FindSplitPoint(ReadOnlySpan<UnityEngine.Vector3> points, ReadOnlySpan<int> indices, int startIndex, int endIndex, int axis)
        {
            float a = points[indices[startIndex]][axis];
            float b = points[indices[endIndex]][axis];
            int midIndex = (startIndex + endIndex) / 2;
            float m = points[indices[midIndex]][axis];

            if (a > b)
            {
                if (m > a)
                { return startIndex; }

                if (b > m)
                { return endIndex; }

                return midIndex;
            }
            else
            {
                if (a > m)
                { return startIndex; }

                if (m > b)
                { return endIndex; }

                return midIndex;
            }
        }

        /// <summary>
        /// Find a new pivot index from the range by splitting the points that fall either side
        /// of its plane.
        /// </summary>
        public static int FindPivotIndex(ReadOnlySpan<UnityEngine.Vector3> points, Span<int> indices, int startIndex, int endIndex, int axis)
        {
            int splitPoint = FindSplitPoint(points, indices, startIndex, endIndex, axis);
            // int splitPoint = Random.Range(stIndex, enIndex);

            Vector3 pivot = points[indices[splitPoint]].To();
            SwapElements(indices, startIndex, splitPoint);

            int currPt = startIndex + 1;
            int endPt = endIndex;

            while (currPt <= endPt)
            {
                Vector3 curr = points[indices[currPt]].To();

                if (curr.Axis(axis) > pivot.Axis(axis))
                {
                    SwapElements(indices, currPt, endPt);
                    endPt--;
                }
                else
                {
                    SwapElements(indices, currPt - 1, currPt);
                    currPt++;
                }
            }

            return currPt - 1;
        }

#endif

        public static int[] Iota(int num)
        {
            int[] result = new int[num];

            for (int i = 0; i < num; i++)
            { result[i] = i; }

            return result;
        }

        public static void Iota(Span<int> result)
        {
            for (int i = 0; i < result.Length; i++)
            { result[i] = i; }
        }

        /// <summary>
        /// Find the nearest point in the set to the supplied point.
        /// </summary>
        public (int Index, float SqrDistance) FindNearest(Vector3 point)
        {
            float bestSqDistance = float.MaxValue;
            int bestIndex = -1;

            Search(point, ref bestSqDistance, ref bestIndex);

            return (bestIndex, bestSqDistance);
        }

        /// <summary>
        /// Recursively search the tree.
        /// </summary>
        void Search(Vector3 point, ref float bestSqrDistanceSoFar, ref int bestIndex)
        {
            float mySqDistance = (pivot - point).LengthSquared();

            if (mySqDistance < bestSqrDistanceSoFar)
            {
                bestSqrDistanceSoFar = mySqDistance;
                bestIndex = pivotIndex;
            }

            float planeDistance = DistanceFromSplitPlane(point, pivot, axis);

            int selector = planeDistance <= 0 ? 0 : 1;

            this[selector]?.Search(point, ref bestSqrDistanceSoFar, ref bestIndex);

            selector = (selector + 1) % 2;

            float sqPlaneDistance = planeDistance * planeDistance;

            if ((this[selector] != null) && (bestSqrDistanceSoFar > sqPlaneDistance))
            {
                this[selector]!.Search(point, ref bestSqrDistanceSoFar, ref bestIndex);
            }
        }

        /// <summary>
        /// Get a point's distance from an axis-aligned plane.
        /// </summary>
        static float DistanceFromSplitPlane(Vector3 pt, Vector3 planePt, int axis)
            => pt.Axis(axis) - planePt.Axis(axis);

        /// <summary>
        /// Simple output of tree structure - mainly useful for getting a rough
        /// idea of how deep the tree is (and therefore how well the splitting
        /// heuristic is performing).
        /// </summary>
        public string Dump(int level)
        {
            StringBuilder builder = new();
            Dump(level, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Simple output of tree structure - mainly useful for getting a rough
        /// idea of how deep the tree is (and therefore how well the splitting
        /// heuristic is performing).
        /// </summary>
        public void Dump(int level, StringBuilder builder)
        {
            builder.Append(pivotIndex.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(level, ' '));
            builder.Append('\n');

            left?.Dump(level + 2, builder);
            right?.Dump(level + 2, builder);
        }
    }
}
