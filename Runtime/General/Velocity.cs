using System.Numerics;

namespace Maths
{
    public static class Velocity
    {
        public static float CalculateTime(Vector2 pointA, Vector2 pointB, float speed)
            => CalculateTime((pointA - pointB).Length(), speed);

        public static float CalculateSpeed(float distance, float time)
        {
            if (time == 0f) return 0f;
            return distance / time;
        }

        public static float CalculateDistance(float velocity, float time)
            => velocity * time;

        public static float CalculateTime(float distance, float velocity)
        {
            if (velocity == 0f) return 0f;
            return distance / velocity;
        }

        /// <returns>Aim offset</returns>
        public static Vector2 CalculateInterceptCourse(Vector2 targetPosition, Vector2 targetVelocity, Vector2 projectilePosition, float projectileVelocity)
        {
            float distance;
            float time = 0f;

            const int iterations = 3;
            for (int i = 0; i < iterations; i++)
            {
                distance = (projectilePosition - (targetPosition + (targetVelocity * time))).Length();
                time = CalculateTime(distance, projectileVelocity);
            }

            return targetVelocity * time;
        }

        /// <returns>Aim offset</returns>
        public static Vector2 CalculateInterceptCourse(Vector2 targetPosition, Vector2 targetVelocity, Vector2 projectilePosition, float projectileVelocity, General.Circle circle)
        {
            float p = 1 / projectileVelocity;

            float distance = Vector2.Distance(projectilePosition, targetPosition);
            float time = distance * p;

            distance = Vector2.Distance(projectilePosition, circle.GetPointAfterTime(targetVelocity.Length(), time, circle.GetAngle(targetPosition)));
            time = distance * p;

            distance = Vector2.Distance(projectilePosition, circle.GetPointAfterTime(targetVelocity.Length(), time, circle.GetAngle(targetPosition)));
            time = distance * p;

            Vector2 aim = circle.GetPointAfterTime(targetVelocity.Length(), time, circle.GetAngle(targetPosition));
            return targetPosition - aim;
        }

#if UNITY

        public static float CalculateTime(UnityEngine.Vector2 pointA, UnityEngine.Vector2 pointB, float speed)
            => CalculateTime((pointA - pointB).magnitude, speed);

        /// <returns>Aim offset</returns>
        public static UnityEngine.Vector2 CalculateInterceptCourse(UnityEngine.Vector2 targetPosition, UnityEngine.Vector2 targetVelocity, UnityEngine.Vector2 projectilePosition, float projectileVelocity)
        {
            float distance;
            float time = 0f;

            const int iterations = 3;
            for (int i = 0; i < iterations; i++)
            {
                distance = (projectilePosition - (targetPosition + (targetVelocity * time))).magnitude;
                time = CalculateTime(distance, projectileVelocity);
            }

            return targetVelocity * time;
        }

#endif
    }
}
