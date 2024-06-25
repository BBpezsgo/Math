using System;
using System.Numerics;

namespace Maths
{
    public static class VectorExtensions
    {
        /// <exception cref="IndexOutOfRangeException"/>
        public static float Axis(this Vector3 vector3, int axis) => axis switch
        {
            0 => vector3.X,
            1 => vector3.Y,
            2 => vector3.Z,
            _ => throw new IndexOutOfRangeException(),
        };
    }
}
