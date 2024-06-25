using System;
using System.Numerics;

namespace Maths
{
    public static class Random
    {
        public static readonly System.Random Shared = new();

        public static int Integer() => Shared.Next();
        public static int Integer(int max) => Shared.Next(max);
        public static int Integer(int min, int max) => Shared.Next(min, max);
        public static int Integer(RangeInt range) => Shared.Next(range.A, range.B);

        public static int Integer(this System.Random random) => random.Next();
        public static int Integer(this System.Random random, int max) => random.Next(max);
        public static int Integer(this System.Random random, int min, int max) => random.Next(min, max);
        public static int Integer(this System.Random random, RangeInt range) => random.Next(range.A, range.B);

        const string NonceCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public static string Nonce(this System.Random random, int length) => random.Nonce(length, NonceCharacters);
        public static string Nonce(this System.Random random, int length, ReadOnlySpan<char> characters)
        {
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = characters[random.Next(0, characters.Length)];
            }
            return new string(result);
        }

        public static float Float() => (float)Shared.NextDouble();
        public static float Float(float max) => (float)Shared.NextDouble() * max;
        public static float Float(float min, float max) => ((float)Shared.NextDouble() * (max - min)) + min;
        public static float Float(Range range) => ((float)Shared.NextDouble() * (range.B - range.A)) + range.A;

        public static float Float(this System.Random random) => (float)random.NextDouble();
        public static float Float(this System.Random random, float max) => (float)random.NextDouble() * max;
        public static float Float(this System.Random random, float min, float max) => ((float)random.NextDouble() * (max - min)) + min;
        public static float Float(this System.Random random, Range range) => ((float)random.NextDouble() * (range.B - range.A)) + range.A;

        public static Vector2 Direction()
        {
            float x = (Random.Float() - .5f) * 2f * MathF.PI * 2f;
            float y = (Random.Float() - .5f) * 2f * MathF.PI * 2f;

            x = MathF.Cos(x);
            y = MathF.Sin(y);

            return new Vector2(x, y);
        }

        public static Vector2 Direction(this System.Random random)
        {
            float x = ((float)random.NextDouble() - .5f) * 2f * MathF.PI * 2f;
            float y = ((float)random.NextDouble() - .5f) * 2f * MathF.PI * 2f;

            x = MathF.Cos(x);
            y = MathF.Sin(y);

            return new Vector2(x, y);
        }

        public static Vector3 Direction3()
        {
            float theta = 2f * MathF.PI * (float)Shared.NextDouble();
            float phi = MathF.Acos(1 - (2 * (float)Shared.NextDouble()));
            float x = MathF.Sin(phi) * MathF.Cos(theta);
            float y = MathF.Sin(phi) * MathF.Sin(theta);
            float z = MathF.Cos(phi);

            return new Vector3(x, y, z);
        }

        public static Vector3 Direction3(this System.Random random)
        {
            float theta = 2f * MathF.PI * (float)random.NextDouble();
            float phi = MathF.Acos(1 - (2 * (float)random.NextDouble()));
            float x = MathF.Sin(phi) * MathF.Cos(theta);
            float y = MathF.Sin(phi) * MathF.Sin(theta);
            float z = MathF.Cos(phi);

            return new Vector3(x, y, z);
        }

        public static Vector2 Point(float min, float max) => new(Random.Float(min, max), Random.Float(min, max));
        public static Vector2 Point(RectF limits) => new(Random.Float(limits.Width) + limits.X, Random.Float(limits.Height) + limits.Y);
        public static Vector2 Point(RectInt limits) => new(Random.Float(limits.Width) + limits.X, Random.Float(limits.Height) + limits.Y);
        public static Vector2Int PointInt(RectInt limits) => new(Random.Integer(limits.Width) + limits.X, Random.Integer(limits.Height) + limits.Y);

        public static Vector2 Point(this System.Random random, float min, float max) => new(random.Float(min, max), random.Float(min, max));
        public static Vector2 Point(this System.Random random, RectF limits) => new(random.Float(limits.Width) + limits.X, random.Float(limits.Height) + limits.Y);
        public static Vector2 Point(this System.Random random, RectInt limits) => new(random.Float(limits.Width) + limits.X, random.Float(limits.Height) + limits.Y);
        public static Vector2Int PointInt(this System.Random random, RectInt limits) => new(random.Integer(limits.Width) + limits.X, random.Integer(limits.Height) + limits.Y);

        public static long Int64()
        {
            Span<byte> buffer = stackalloc byte[8];
            Shared.NextBytes(buffer);
            return
                (buffer[0] << (8 * 0)) |
                (buffer[1] << (8 * 1)) |
                (buffer[2] << (8 * 2)) |
                (buffer[3] << (8 * 3)) |
                (buffer[4] << (8 * 4)) |
                (buffer[5] << (8 * 5)) |
                (buffer[6] << (8 * 6)) |
                (buffer[7] << (8 * 7));
        }

        public static long Int64(long max)
        {
            long result = Int64();
            if (result > max)
            { result %= max; }
            return result;
        }
    }
}
