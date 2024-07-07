using System;
using System.Numerics;

namespace Maths
{
    public static class Ballistics
    {
#if UNITY
        public readonly struct ProfilerMarkers
        {
            public static readonly Unity.Profiling.ProfilerMarker TrajectoryMath = new("Game.Math.Trajectory");
        }
#endif

#if UNITY
        public static readonly float GForce = UnityEngine.Physics.gravity.y;
        public static readonly Vector2 GVector2 = new(0f, GForce);
        public static readonly Vector3 GVector3 = UnityEngine.Physics.gravity.To();
#else
        public const float GForce = -9.80665f;
        public static readonly Vector2 GVector2 = new(0f, GForce);
        public static readonly Vector3 GVector3 = new(0f, GForce, 0f);
#endif

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
            public readonly Vector3 Origin;

            public Trajectory(float angle, float direction, float velocity, Vector3 startPosition)
            {
                Angle = angle;
                Direction = direction;
                Velocity = velocity;
                Origin = startPosition;
            }

            public Vector3 Position(float t)
            {
                Vector2 displacement = Ballistics.Displacement(Angle * Rotation.Deg2Rad, Velocity, t);
                Vector3 displacement3D = default;

                displacement3D.X = displacement.X * MathF.Sin(Direction * Rotation.Deg2Rad);
                displacement3D.Y = displacement.Y;
                displacement3D.Z = displacement.X * MathF.Cos(Direction * Rotation.Deg2Rad);

                displacement3D += Origin;

                return displacement3D;
            }
        }

#if UNITY
        public static Vector3? PredictImpact(UnityEngine.Transform shootPosition, float projectileVelocity, float projectileLifetime, out bool outOfRange)
        {
            outOfRange = false;
            float angle = -shootPosition.eulerAngles.x;

            float? _hitDistance = Ballistics.CalculateX(angle * Rotation.Deg2Rad, projectileVelocity, 0f);
            if (!_hitDistance.HasValue) return null;
            float hitDistance = _hitDistance.Value;

            Vector3 flatForward = shootPosition.forward.To().Flatten();
            Vector3 point = shootPosition.position.To() + (hitDistance * flatForward);
            point.Y = 0f;

            if (projectileLifetime > 0f)
            {
                Vector2 maxHitDistance = Ballistics.Displacement(angle * Rotation.Deg2Rad, projectileVelocity, projectileLifetime);
                if (maxHitDistance.X < hitDistance)
                {
                    outOfRange = true;
                    point = shootPosition.position.To() + (maxHitDistance.X * flatForward);
                    point.Y = maxHitDistance.Y;
                }
            }

            return point;
        }
#endif

        public static void GetTrajectory(Vector3 velocity, Span<Vector3> trajectory, float timeStep)
        {
            for (int i = 0; i < trajectory.Length; i++)
            { trajectory[i] = Displacement(velocity, i * timeStep); }
        }

        public static void GetTrajectory(Vector3 origin, Vector3 velocity, Span<Vector3> trajectory, float timeStep)
        {
            for (int i = 0; i < trajectory.Length; i++)
            { trajectory[i] = origin + Displacement(velocity, i * timeStep); }
        }

#if UNITY
        public static void GetTrajectory(UnityEngine.Vector3 velocity, Span<UnityEngine.Vector3> trajectory, float timeStep)
        {
            for (int i = 0; i < trajectory.Length; i++)
            { trajectory[i] = Displacement(velocity, i * timeStep); }
        }

        public static void GetTrajectory(UnityEngine.Vector3 origin, UnityEngine.Vector3 velocity, Span<UnityEngine.Vector3> trajectory, float timeStep)
        {
            for (int i = 0; i < trajectory.Length; i++)
            { trajectory[i] = origin + Displacement(velocity, i * timeStep); }
        }
#endif

        public static float? CalculateTime(float velocity, float angleRad, float heightDisplacement)
        {
            float a = velocity * MathF.Sin(angleRad);
            float b = 2 * -GForce * heightDisplacement;

            float discriminant = (a * a) + b;
            if (discriminant < 0)
            {
                return null;
            }

            float sqrt = MathF.Sqrt(discriminant);

            return (a + sqrt) / -GForce;
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

        /// <returns>
        /// The required <b>angle in radians</b> to hit a <paramref name="target"/>
        /// fired from <paramref name="origin"/> initial projectile speed <paramref name="velocity"/>
        /// </returns>
        public static (float, float)? AngleOfReach(float velocity, Vector3 origin, Vector3 target)
        {
            Vector3 diff = target - origin;
            float y = diff.Y;
            float x = MathF.Sqrt((diff.X * diff.X) + (diff.Z * diff.Z));
            return Ballistics.AngleOfReach(velocity, new Vector2(x, y));
        }

        /// <returns>
        /// The required <b>angle in radians</b> to hit a <paramref name="target"/>
        /// fired from <paramref name="origin"/> initial projectile speed <paramref name="velocity"/>
        /// </returns>
        public static float? AngleOfReach1(float velocity, Vector3 origin, Vector3 target)
        {
            Vector3 diff = target - origin;
            float y = diff.Y;
            float x = MathF.Sqrt((diff.X * diff.X) + (diff.Z * diff.Z));
            return Ballistics.AngleOfReach1(velocity, new Vector2(x, y));
        }

        /// <returns>
        /// The required <b>angle in radians</b> to hit a <paramref name="target"/>
        /// fired from <paramref name="origin"/> initial projectile speed <paramref name="velocity"/>
        /// </returns>
        public static float? AngleOfReach2(float velocity, Vector3 origin, Vector3 target)
        {
            Vector3 diff = target - origin;
            float y = diff.Y;
            float x = MathF.Sqrt((diff.X * diff.X) + (diff.Z * diff.Z));
            return Ballistics.AngleOfReach2(velocity, new Vector2(x, y));
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

        public static float? CalculateX(float angleRad, float velocity, float heightDisplacement)
        {
            float? time = CalculateTime(velocity, angleRad, heightDisplacement);

            if (time.HasValue)
            { return DisplacementX(angleRad, time.Value, velocity); }

            return null;
        }

        public static float CalculateY(float angleRad, float time, float velocity)
            => (velocity * MathF.Sin(angleRad) * time) + (GForce * time * time / 2f);

        public static float CalculateTimeToMaxHeight(float angleRad, float velocity)
            => velocity * MathF.Sin(angleRad) / -GForce;

        /// <summary>
        /// To hit a target at range <c><paramref name="target"/>.x</c> and altitude <c><paramref name="target"/>.y</c> when fired from <c>(0,0)</c> and with initial speed <paramref name="velocity"/>.
        /// </summary>
        public static (float, float)? AngleOfReach(float velocity, Vector2 target)
        {
            float v2 = velocity * velocity;

            float x = target.X;
            float y = target.Y;

            float discriminant = (v2 * v2) - (-GForce * ((-GForce * x * x) + (2 * y * v2)));

            if (discriminant < 0f)
            { return null; }

            float dSqrt = MathF.Sqrt(discriminant);

            float a = (v2 + dSqrt) / (-GForce * x);
            float b = (v2 - dSqrt) / (-GForce * x);

            float a_ = MathF.Atan(a);
            float b_ = MathF.Atan(b);

            return (a_, b_);
        }

        /// <summary>
        /// To hit a target at range <c><paramref name="target"/>.x</c> and altitude <c><paramref name="target"/>.y</c> when fired from <c>(0,0)</c> and with initial speed <paramref name="velocity"/>.
        /// </summary>
        public static float? AngleOfReach1(float velocity, Vector2 target)
        {
            float v2 = velocity * velocity;

            float x = target.X;
            float y = target.Y;

            float discriminant = (v2 * v2) - (-GForce * ((-GForce * x * x) + (2 * y * v2)));

            if (discriminant < 0f)
            { return null; }

            float dSqrt = MathF.Sqrt(discriminant);

            float a = (v2 + dSqrt) / (-GForce * x);

            return MathF.Atan(a);
        }

        /// <summary>
        /// To hit a target at range <c><paramref name="target"/>.x</c> and altitude <c><paramref name="target"/>.y</c> when fired from <c>(0,0)</c> and with initial speed <paramref name="velocity"/>.
        /// </summary>
        public static float? AngleOfReach2(float velocity, Vector2 target)
        {
            float v2 = velocity * velocity;

            float x = target.X;
            float y = target.Y;

            float discriminant = (v2 * v2) - (-GForce * ((-GForce * x * x) + (2 * y * v2)));

            if (discriminant < 0f)
            { return null; }

            float dSqrt = MathF.Sqrt(discriminant);

            float b = (v2 - dSqrt) / (-GForce * x);

            return MathF.Atan(b);
        }

        /// <returns>The greatest height that the object will reach</returns>
        public static float MaxHeight(float angleRad, float velocity)
            => velocity * velocity * MathF.Pow(MathF.Sin(angleRad), 2f) / (2f * -GForce);

        /// <summary>
        /// <para>
        /// The angle at which a projectile must be launched in order to go a distance <paramref name="distance"/>, given the initial velocity <paramref name="velocity"/>.
        /// </para>
        /// <para>
        /// <seealso href="https://en.wikipedia.org/wiki/Projectile_motion#Angle_of_reach"/>
        /// </para>
        /// </summary>
        /// <returns><c>(shallow, steep)</c> in radians or <c><see langword="null"/></c> if there is no solution</returns>
        public static (float, float)? AngleOfReach(float velocity, float distance)
        {
            float a = -GForce * distance / velocity * velocity;

            if (a is < (-1f) or > 1f) return null;

            float shallow = 0.5f * MathF.Asin(a);
            float steep = 0.5f * MathF.Acos(a);

            return (shallow, steep);
        }

        public static float Radius(float velocity, float angleRad)
            => velocity * velocity / -GForce * MathF.Sin(angleRad * 2f);

        public static float MaxRadius(float velocity)
            => velocity * velocity / -GForce;

        /// <returns>The velocity after time <paramref name="time"/> or <c><see langword="null"/></c> if there is no solution</returns>
        public static float? Velocity(float angleRad, float velocity, float time)
        {
            float vx = velocity * MathF.Cos(angleRad);
            float vy = (velocity * MathF.Sin(angleRad)) + (GForce * time);
            float a = (vx * vx) + (vy * vy);
            if (a < 0f)
            { return null; }
            return MathF.Sqrt(a);
        }

        public static Vector2 Displacement(Vector2 velocity, float time)
            => (velocity * time) + (.5f * time * time * GVector2);

        public static Vector3 Displacement(Vector3 velocity, float time)
            => (velocity * time) + (.5f * time * time * GVector3);

#if UNITY
        public static UnityEngine.Vector3 Displacement(UnityEngine.Vector3 velocity, float time)
            => (velocity * time) + (.5f * time * time * UnityEngine.Physics.gravity);
#endif

        public static Vector2 Displacement(float angleRad, float velocity, float time)
        {
            float x = velocity * time * MathF.Cos(angleRad);
            float y = (velocity * time * MathF.Sin(angleRad)) + (0.5f * GForce * time * time);
            return new Vector2(x, y);
        }

        public static float DisplacementX(float angleRad, float velocity, float time)
            => velocity * time * MathF.Cos(angleRad);

        public static float DisplacementY(float angleRad, float velocity, float time)
            => (velocity * time * MathF.Sin(angleRad)) + (0.5f * GForce * time * time);

        /// <returns>The initial velocity or <c><see langword="null"/></c> if there is no solution</returns>
        public static float? InitialVelocity(float angleRad, Vector2 displacement)
        {
            float x = displacement.X;
            float y = displacement.Y;

            float a = x * x * -GForce;
            float b = x * MathF.Sin(angleRad * 2f);
            float c = 2f * y * MathF.Pow(MathF.Cos(angleRad), 2f);
            float d = a / (b - c);
            if (d < 0f)
            { return null; }
            return MathF.Sqrt(d);
        }

        public static float MaxRadius(float velocity, float heightDisplacement)
        {
            float t = CalculateTime(velocity, 45f * Rotation.Deg2Rad, heightDisplacement) ?? throw new Exception();
            return DisplacementX(45f * Rotation.Deg2Rad, t, velocity);
        }

        /// <summary>
        /// <para>
        /// The total time for which the projectile remains in the air.
        /// </para>
        /// <para>
        /// <seealso href="https://en.wikipedia.org/wiki/Projectile_motion#Time_of_flight_or_total_time_of_the_whole_journey"/>
        /// </para>
        /// </summary>
        public static float TimeOfFlight(float velocity, float angleRad)
            => 2f * velocity * MathF.Sin(angleRad) / -GForce;

        public static float? TimeToReachDistance(float velocity, float angleRad, float distance)
        {
            float a = velocity * MathF.Cos(angleRad);
            if (a <= 0f)
            { return null; }
            return distance / a;
        }

        public static float MaxHeight2(float distance, float angleRad)
            => distance * MathF.Tan(angleRad) / 4;

        /// <summary>
        /// <para>
        /// <see href="https://www.toppr.com/guides/physics/motion-in-a-plane/projectile-motion/"/>
        /// </para>
        /// <para>
        /// <c>y = (tan θ) * x – g (x ^ 2) / 2 * (v * cos θ) ^ 2</c>
        /// </para>
        /// </summary>
        public static float GetHeight(float distance, float angleRad, float velocity)
        {
            float a = MathF.Tan(angleRad) * distance;
            float b = -GForce * distance * distance;
            float c = MathF.Pow(velocity * MathF.Cos(angleRad), 2) * 2f;
            return a - (b / c);
        }

#if UNITY

        public static (Vector3 PredictedPosition, float TimeToReach)? CalculateInterceptCourse(float velocity, float lifetime, Vector3 origin, Trajectory targetTrajectory)
        {
            float? angle_;
            float? t;
            Vector3 targetPosition;
            const int iterations = 3;

            float? projectileTimeOfFlight = Ballistics.CalculateTime(targetTrajectory.Velocity, targetTrajectory.Angle * Rotation.Deg2Rad, targetTrajectory.Origin.Y);

            if (projectileTimeOfFlight.HasValue && (projectileTimeOfFlight - lifetime) < .5f)
            { return null; }

            targetPosition = targetTrajectory.Position(lifetime);

            float distance = Vector2.Distance(origin.To2D(), targetPosition.To2D());

            angle_ = Ballistics.AngleOfReach2(velocity, origin, targetPosition);

            t = angle_.HasValue ? Ballistics.TimeToReachDistance(velocity, angle_.Value, distance) : null;

            for (int i = 0; i < iterations; i++)
            {
                if (!angle_.HasValue) break;
                if (!t.HasValue) break;

                targetPosition = targetTrajectory.Position(lifetime + t.Value);

                distance = Vector2.Distance(origin.To2D(), targetPosition.To2D());

                angle_ = Ballistics.AngleOfReach2(velocity, origin, targetPosition);

                t = angle_.HasValue ? Ballistics.TimeToReachDistance(velocity, angle_.Value, distance) : null;
            }

            return (targetPosition, t!.Value);
        }

#endif

        public static Vector2? CalculateInterceptCourse(Vector2 origin, float velocity, Vector2 targetPosition, Vector2 targetVelocity)
        {
            float time = 0f;
            const int iterations = 3;
            Vector2 targetOriginalPosition = targetPosition;

            float height = origin.Y - targetPosition.Y;

            for (int i = 0; i < iterations; i++)
            {
                float? _angle = Ballistics.AngleOfReach2(velocity, origin.To3(), targetPosition.To3());
                if (!_angle.HasValue)
                { return null; }
                float angle = _angle.Value;

                float? _time = Ballistics.CalculateTime(velocity, angle, height);
                if (!_time.HasValue)
                { return null; }
                time = _time.Value;

                targetPosition = targetOriginalPosition + (targetVelocity * time);
            }

            return targetVelocity * time;
        }
    }
}
