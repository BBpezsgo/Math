using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Maths
{
    public static class Ballistics
    {
        public readonly struct ProfilerMarkers
        {
            public static readonly Unity.Profiling.ProfilerMarker TrajectoryMath = new("Game.Math.Trajectory");
        }

        /// <summary>
        /// This is positive
        /// </summary>
#if UNITY
        public static float G => Math.Abs(UnityEngine.Physics.gravity.y);
#else
        public static float G => 9.80665f;
#endif

        /// <summary>
        /// This points downwards
        /// </summary>
        public static Vector2 GVector => new(0f, -G);

        public readonly struct Trajectory
        {
            /// <summary>
            /// In degrees
            /// </summary>
            public readonly float Angle;
            /// <summary>
            /// In degrees
            /// </summary>
            public readonly float Direction;
            public readonly float Velocity;
            public readonly Vector3 StartPosition;

            public Trajectory(float angle, float direction, float velocity, Vector3 startPosition)
            {
                Angle = angle;
                Direction = direction;
                Velocity = velocity;
                StartPosition = startPosition;
            }

            public Vector2 Velocity2D()
                => new(Velocity * MathF.Cos(Angle * Rotation.Deg2Rad), Velocity * MathF.Sin(Angle * Rotation.Deg2Rad));

            public Vector3 Velocity3D()
            {
                Vector3 result = default;
                result.X = MathF.Sin(Direction * Rotation.Deg2Rad);
                result.Y = MathF.Sin(Angle * Rotation.Deg2Rad);
                result.Z = MathF.Cos(Direction * Rotation.Deg2Rad);
                return result;
            }

            public Vector3 Position(float t)
            {
                Vector2 displacement = Ballistics.Displacement(Angle * Rotation.Deg2Rad, Velocity, t);
                Vector3 displacement3D = default;

                displacement3D.X = displacement.X * MathF.Sin(Direction * Rotation.Deg2Rad);
                displacement3D.Y = displacement.Y;
                displacement3D.Z = displacement.X * MathF.Cos(Direction * Rotation.Deg2Rad);

                displacement3D += StartPosition;

                return displacement3D;
            }

            public static Vector2 TransformPositionToPlane(Vector3 position, float directionRad) => new()
            {
                Y = position.Y,
                X = (position.X * MathF.Cos(directionRad)) + (position.Y * MathF.Sin(directionRad)),
            };
        }

#if UNITY
        public static Vector3? PredictImpact(UnityEngine.Transform shootPosition, float projectileVelocity, float projectileLifetime, out bool outOfRange)
        {
            outOfRange = false;
            float? _relativeHitDistance;
            float angle = -shootPosition.eulerAngles.x;

            using (Ballistics.ProfilerMarkers.TrajectoryMath.Auto())
            { _relativeHitDistance = Ballistics.CalculateX(angle * Rotation.Deg2Rad, projectileVelocity, shootPosition.position.y); }
            if (!_relativeHitDistance.HasValue) return null;
            float relativeHitDistance = _relativeHitDistance.Value;

            Vector3 turretRotation = shootPosition.forward.To().Flatten();
            Vector3 point = shootPosition.position.To() + (relativeHitDistance * turretRotation);
            point.Y = 0f;

            if (projectileLifetime > 0f)
            {
                Vector2 maxHitDistance;
                using (Ballistics.ProfilerMarkers.TrajectoryMath.Auto())
                { maxHitDistance = Ballistics.Displacement(angle * Rotation.Deg2Rad, projectileVelocity, projectileLifetime); }

                if (maxHitDistance.X < relativeHitDistance)
                {
                    outOfRange = true;
                    point = shootPosition.position.To() + (maxHitDistance.X * turretRotation);
                    point.Y = maxHitDistance.Y;
                }
            }

            // point.Y = Maths.Max(point.Y, TheTerrain.Height(point));

            return point;
        }
#endif

        /// <param name="v">
        /// Projectile's initial velocity
        /// </param>
        public static float? CalculateTime(float v, float angle, float heightDisplacement)
        {
            float a = v * MathF.Sin(angle);
            float b = 2 * G * heightDisplacement;

            float discriminant = (a * a) + b;
            if (discriminant < 0)
            {
                return null;
            }

            float sqrt = MathF.Sqrt(discriminant);

            return (a + sqrt) / G;
        }

        /*
        public static float CalculateAngle(float v, float x)
        {
            // v0y = v0 * Maths.Sin(theta)
            // v0x = v0 * Maths.Cos(theta)

            // y = y0 + v0y * t + 0.5 * G * t * t
            // 0 = 0 + v0y * t + 0.5 * G * t * t

            // x = v0x * t



            // 0 = 0 + v0 * Maths.Sin(theta) * t + 0.5 * G * t * t

            // x = v0 * Maths.Cos(theta) * t
            // t = x / ( v0 * Maths.Cos(theta) )

            // 0 = 0 + v0 * Maths.Sin(theta) * (x / ( v0 * Maths.Cos(theta) )) + 0.5 * G * Maths.Pow((x / ( v0 * Maths.Cos(theta) )), 2)

            // 0 = Maths.Sin(theta) * Maths.Cos(theta) - ( (G * x) / (2 * v0 * v0) )

            // 0 = 0.5 * Maths.Sin(2 * theta) - ( ... )

            // Maths.Sin(2 * theta) = ( ... )

            float theta = 0.5f * Maths.Asin((G * x) / (v * v));

            return theta;
        }
        */

        /// <param name="v">Initial velocity</param>
        /// <param name="target">Position of the target</param>
        /// <returns>
        /// The required <b>angle in radians</b> to hit a <paramref name="target"/>
        /// fired <paramref name="from"/> initial projectile speed <paramref name="v"/>
        /// </returns>
        public static (float, float)? AngleOfReach(float v, Vector3 from, Vector3 target)
        {
            Vector3 diff = target - from;
            float y = diff.Y;
            float x = MathF.Sqrt((diff.X * diff.X) + (diff.Z * diff.Z));
            return Ballistics.AngleOfReach(v, new Vector2(x, y));
        }

        /// <param name="v">
        /// Projectile's initial velocity
        /// </param>
        /// <param name="target">
        /// Position of the target in <b>world space</b>
        /// </param>
        /// <returns>
        /// The required <b>angle in radians</b> to hit a <paramref name="target"/>
        /// fired <paramref name="from"/> initial projectile speed <paramref name="v"/>
        /// </returns>
        public static float? AngleOfReach1(float v, Vector3 from, Vector3 target)
        {
            Vector3 diff = target - from;
            float y = diff.Y;
            float x = MathF.Sqrt((diff.X * diff.X) + (diff.Z * diff.Z));
            return Ballistics.AngleOfReach1(v, new Vector2(x, y));
        }

        /// <param name="v">
        /// Projectile's initial velocity
        /// </param>
        /// <param name="target">
        /// Position of the target in <b>world space</b>
        /// </param>
        /// <returns>
        /// The required <b>angle in radians</b> to hit a <paramref name="target"/>
        /// fired <paramref name="from"/> initial projectile speed <paramref name="v"/>
        /// </returns>
        public static float? AngleOfReach2(float v, Vector3 from, Vector3 target)
        {
            Vector3 diff = target - from;
            float y = diff.Y;
            float x = MathF.Sqrt((diff.X * diff.X) + (diff.Z * diff.Z));
            return Ballistics.AngleOfReach2(v, new Vector2(x, y));
        }

        /*
        /// <param name="v">
        /// Projectile's initial velocity
        /// </param>
        /// <param name="x">
        /// Range / Distance to the target
        /// </param>
        /// <param name="y">
        /// Altitude of the target
        /// </param>
        /// <summary>
        /// <seealso href="https://en.wikipedia.org/wiki/Projectile_motion#Angle_%CE%B8_required_to_hit_coordinate_(x,_y)"/>
        /// </summary>
        /// <returns>
        /// The required <b>angle in radians</b> to hit a target at range <paramref name="x"/> and altitude <paramref name="y"/> when
        /// fired from (0,0) and with initial projectile speed <paramref name="v"/>
        /// </returns>
        public static float CalculateAngle(float v, float x, float y)
        {
            float g = G;

            float v2 = v * v;
            float v4 = v2 * v2;
            float x2 = x * x;

            float theta = Maths.Atan2(v2 - Maths.Sqrt(v4 - g * (g * x2 + 2 * y * v2)), g * x);

            return theta;
        }
        */

        public static float? CalculateX(float angle, float v, float heightDisplacement)
        {
            float? t = CalculateTime(v, angle, heightDisplacement);

            if (t.HasValue)
            { return DisplacementX(angle, t.Value, v); }

            return null;
        }

        public static float CalculateY(float angleRad, float t, float v)
            => (v * MathF.Sin(angleRad) * t) - ((G * t * t) / 2f);

        public static float CalculateTimeToMaxHeight(float angleRad, float v)
            => (v * MathF.Sin(angleRad)) / G;

        /// <summary>
        /// To hit a target at range x and altitude y when fired from (0,0) and with initial speed v.
        /// </summary>
        public static (float, float)? AngleOfReach(float v, Vector2 target)
        {
            float v2 = v * v;

            float x = target.X;
            float y = target.Y;

            float discriminant = (v2 * v2) - (G * ((G * x * x) + (2 * y * v2)));

            if (discriminant < 0f)
            { return null; }

            float dSqrt = MathF.Sqrt(discriminant);

            float a = (v2 + dSqrt) / (G * x);
            float b = (v2 - dSqrt) / (G * x);

            float a_ = MathF.Atan(a);
            float b_ = MathF.Atan(b);

            return (a_, b_);
        }

        /// <summary>
        /// To hit a <paramref name="target"/> at range <c>x</c> and altitude <c>y</c> when fired from <c>(0,0)</c> and with initial speed <paramref name="v"/>.
        /// </summary>
        public static float? AngleOfReach1(float v, Vector2 target)
        {
            float v2 = v * v;

            float x = target.X;
            float y = target.Y;

            float discriminant = (v2 * v2) - (G * ((G * x * x) + (2 * y * v2)));

            if (discriminant < 0f)
            { return null; }

            float dSqrt = MathF.Sqrt(discriminant);

            float a = (v2 + dSqrt) / (G * x);

            float a_ = MathF.Atan(a);

            return a_;
        }

        /// <summary>
        /// To hit a <paramref name="target"/> at range <c>x</c> and altitude <c>y</c> when fired from <c>(0,0)</c> and with initial speed <paramref name="v"/>.
        /// </summary>
        public static float? AngleOfReach2(float v, Vector2 target)
        {
            float v2 = v * v;

            float x = target.X;
            float y = target.Y;

            float discriminant = (v2 * v2) - (G * ((G * x * x) + (2 * y * v2)));

            if (discriminant < 0f)
            { return null; }

            float dSqrt = MathF.Sqrt(discriminant);

            float b = (v2 - dSqrt) / (G * x);

            float b_ = MathF.Atan(b);

            return b_;
        }

        /// <param name="angleRad">Launch angle</param>
        /// <param name="v">Initial velocity</param>
        /// <returns>The greatest height that the object will reach</returns>
        public static float MaxHeight(float angleRad, float v)
            => (v * v * MathF.Pow(MathF.Sin(angleRad), 2f)) / (2f * G);

        /// <summary>
        /// The "angle of reach" is the angle at which a projectile must be launched in order to go a distance <paramref name="d"/>, given the initial velocity <paramref name="v"/>.
        /// <seealso href="https://en.wikipedia.org/wiki/Projectile_motion#Angle_of_reach"/>
        /// </summary>
        /// <param name="v">Initial velocity</param>
        /// <param name="d">Target distance</param>
        /// <returns><c>(shallow, steep)</c> in radians or <c><see langword="null"/></c> if there is no solution</returns>
        public static (float, float)? AngleOfReach(float v, float d)
        {
            float a = (G * d) / v * v;

            if (a < -1f || a > 1f) return null;

            float shallow = 0.5f * MathF.Asin(a);
            float steep = 0.5f * MathF.Acos(a);

            return (shallow, steep);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Radius(float v, float angleRad)
            => ((v * v) / G) * MathF.Sin(angleRad * 2f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxRadius(float v)
            => (v * v) / G;

        /// <param name="angleRad">Launch angle</param>
        /// <param name="v">Initial velocity</param>
        /// <param name="t">Time</param>
        /// <returns>The velocity after time <paramref name="t"/> or <c><see langword="null"/></c> if there is no solution</returns>
        public static float? Velocity(float angleRad, float v, float t)
        {
            float vx = v * MathF.Cos(angleRad);
            float vy = (v * MathF.Sin(angleRad)) - (G * t);
            float a = (vx * vx) + (vy * vy);
            if (a < 0f)
            { return null; }
            return MathF.Sqrt(a);
        }

        /// <param name="angleRad">Launch angle</param>
        /// <param name="v">Initial velocity</param>
        /// <param name="t">Time</param>
        public static Vector2 Displacement(float angleRad, float v, float t)
        {
            float x = v * t * MathF.Cos(angleRad);
            float y = (v * t * MathF.Sin(angleRad)) - (0.5f * G * t * t);
            return new Vector2(x, y);
        }

        /// <param name="angleRad">Launch angle</param>
        /// <param name="v">Initial velocity</param>
        /// <param name="t">Time</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DisplacementX(float angleRad, float v, float t)
            => v * t * MathF.Cos(angleRad);

        /// <param name="angleRad">Launch angle</param>
        /// <param name="v">Initial velocity</param>
        /// <param name="t">Time</param>
        public static float DisplacementY(float angleRad, float v, float t)
            => (v * t * MathF.Sin(angleRad)) - (0.5f * G * t * t);

        /// <param name="angleRad">Launch angle</param>
        /// <param name="displacement">Displacement</param>
        /// <returns>The initial velocity or <c><see langword="null"/></c> if there is no solution</returns>
        public static float? InitialVelocity(float angleRad, Vector2 displacement)
        {
            float x = displacement.X;
            float y = displacement.Y;

            float a = x * x * G;
            float b = x * MathF.Sin(angleRad * 2f);
            float c = 2f * y * MathF.Pow(MathF.Cos(angleRad), 2f);
            float d = a / (b - c);
            if (d < 0f)
            { return null; }
            return MathF.Sqrt(d);
        }

        public static float MaxRadius(float v, float heightDisplacement)
        {
            float t = CalculateTime(v, 45f * Rotation.Deg2Rad, heightDisplacement) ?? throw new Exception();
            float x = DisplacementX(45f * Rotation.Deg2Rad, t, v);
            return x;
        }

        /// <summary>
        /// The total time for which the projectile remains in the air.
        /// <seealso href="https://en.wikipedia.org/wiki/Projectile_motion#Time_of_flight_or_total_time_of_the_whole_journey"/>
        /// </summary>
        /// <param name="v">Initial velocity</param>
        /// <param name="angleRad">Launch angle</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TimeOfFlight(float v, float angleRad)
            => (2f * v * MathF.Sin(angleRad)) / G;

        public static float? TimeToReachDistance(float v, float angleRad, float d)
        {
            float a = v * MathF.Cos(angleRad);
            if (a <= 0f)
            { return null; }
            return d / a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxHeight2(float d, float angleRad)
            => (d * MathF.Tan(angleRad)) / 4;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 GetPosition(Vector2 v, float t)
            => (v * t) + (t * t * GVector / 2);

        /// <summary>
        /// <see href="https://www.toppr.com/guides/physics/motion-in-a-plane/projectile-motion/"/>
        /// <c>y = (tan θ) * x – g (x ^ 2) / 2 * (v * cos θ) ^ 2</c>
        /// </summary>
        public static float GetHeight(float d, float angleRad, float v)
        {
            float a = MathF.Tan(angleRad) * d;
            float b = G * d * d;
            float c = MathF.Pow(v * MathF.Cos(angleRad), 2) * 2f;
            return a - (b / c);
        }

#if UNITY

        public static (Vector3 PredictedPosition, float TimeToReach)? CalculateInterceptCourse(float projectileVelocity, float projectileLifetime, Vector3 shootPosition, Trajectory targetTrajectory)
        {
            float? angle_;
            float? t;
            Vector3 targetPosition;
            const int iterations = 3;

            using (ProfilerMarkers.TrajectoryMath.Auto())
            {
                // projectileVelocity *= .95f;

                float lifetime = projectileLifetime + UnityEngine.Time.fixedDeltaTime;

                float? projectileTimeOfFlight = Ballistics.CalculateTime(targetTrajectory.Velocity, targetTrajectory.Angle * Rotation.Deg2Rad, targetTrajectory.StartPosition.Y);

                if (projectileTimeOfFlight.HasValue && (projectileTimeOfFlight - lifetime) < .5f)
                { return null; }

                targetPosition = targetTrajectory.Position(lifetime);

                float distance = Vector2.Distance(shootPosition.To2D(), targetPosition.To2D());

                angle_ = Ballistics.AngleOfReach2(projectileVelocity, shootPosition, targetPosition);

                t = angle_.HasValue ? Ballistics.TimeToReachDistance(projectileVelocity, angle_.Value, distance) : null;

                for (int i = 0; i < iterations; i++)
                {
                    if (!angle_.HasValue) break;
                    if (!t.HasValue) break;

                    targetPosition = targetTrajectory.Position(lifetime + t.Value);

                    distance = Vector2.Distance(shootPosition.To2D(), targetPosition.To2D());

                    angle_ = Ballistics.AngleOfReach2(projectileVelocity, shootPosition, targetPosition);

                    t = angle_.HasValue ? Ballistics.TimeToReachDistance(projectileVelocity, angle_.Value, distance) : null;
                }
            }

            return (targetPosition, t!.Value);
        }
        
#endif

        public static Vector2? CalculateInterceptCourse(Vector2 projectilePosition, float projectileVelocity, Vector2 targetPosition, Vector2 targetVelocity)
        {
            float time = 0f;
            const int iterations = 3;
            Vector2 targetOriginalPosition = targetPosition;

            float height = projectilePosition.Y - targetPosition.Y;

            using (ProfilerMarkers.TrajectoryMath.Auto())
            {
                // projectileVelocity *= .95f;

                for (int i = 0; i < iterations; i++)
                {
                    float? _angle = Ballistics.AngleOfReach2(projectileVelocity, projectilePosition.To3(), targetPosition.To3());
                    if (!_angle.HasValue)
                    { return null; }
                    float angle = _angle.Value;

                    float? _time = Ballistics.CalculateTime(projectileVelocity, angle, height);
                    if (!_time.HasValue)
                    { return null; }
                    time = _time.Value;

                    targetPosition = targetOriginalPosition + (targetVelocity * time);
                }
            }

            return targetVelocity * time;
        }
    }
}
