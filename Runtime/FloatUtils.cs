using System;

namespace Maths
{
    public static class FloatUtils
    {
        public const float Epsilon = 0.0001f;
        public static bool FloatEquality(float a, float b) => MathF.Abs(a - b) < Epsilon;
    }
}
