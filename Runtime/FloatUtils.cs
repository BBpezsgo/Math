using System;

namespace Maths
{
    public static class FloatUtils
    {
#if UNITY
        public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;
#else
        public const float Epsilon = 0.0001f;
#endif
        public static bool FloatEquality(float a, float b) => MathF.Abs(a - b) < Epsilon;
    }
}
