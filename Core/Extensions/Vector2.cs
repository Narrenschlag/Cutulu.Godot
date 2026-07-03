#if GODOT4_0_OR_GREATER
namespace Cutulu.Core
{
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;
    using Godot;

    public static class Vector2f
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(this Vector2 a, Vector2 b, float lerp) => new(Mathf.Lerp(a.X, b.X, lerp), Mathf.Lerp(a.Y, b.Y, lerp));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 setX(this Vector2 v2, float value) => new(value, v2.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 setY(this Vector2 v2, float value) => new(v2.X, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void pasteX(this float value, ref Vector2 v2) => v2.X = value;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void pasteY(this float value, ref Vector2 v2) => v2.Y = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I RoundToInt(this Vector2 v2) => new(Mathf.RoundToInt(v2.X), Mathf.RoundToInt(v2.Y));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I FloorToInt(this Vector2 v2) => new(Mathf.FloorToInt(v2.X), Mathf.FloorToInt(v2.Y));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2I CeilToInt(this Vector2 v2) => new(Mathf.CeilToInt(v2.X), Mathf.CeilToInt(v2.Y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Abs(this Vector2 v2) => new(Mathf.Abs(v2.X), Mathf.Abs(v2.Y));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Max(this Vector2 o, Vector2 a, Vector2 b) => o.DistanceTo(a) > o.DistanceTo(b) ? a : b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Min(this Vector2 o, Vector2 a, Vector2 b) => o.DistanceTo(a) < o.DistanceTo(b) ? a : b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NoNaN(this Vector2 v2) => new(float.IsNaN(v2.X) ? 0 : v2.X, float.IsNaN(v2.Y) ? 0 : v2.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 toXY(this Vector3 value) => new(value.X, value.Z);

        public static Vector2 RotatedD(this Vector2 v2, float degrees) => v2.Rotated(degrees.toRadians());

        /// <summary>
        /// Returns angle from Vector2.Right. In Degrees.
        /// </summary>
        public static float GetAngleD(this Vector2 direction) => GetAngle(direction).toDegrees();
        public static float GetAngleD(this Vector2 direction, Vector2 from) => GetAngle(direction, from).toDegrees().AbsMod(360f);

        /// <summary>
        /// Returns angle from Vector2.Right. In Radians.
        /// </summary>
        public static float GetAngle(this Vector2 direction) => GetAngle(direction, Vector2.Right);
        public static float GetAngle(this Vector2 direction, Vector2 up)
        {
            direction = direction.Normalized();
            up = up.Normalized();

            return Mathf.Wrap(Mathf.Atan2(direction.Y, direction.X) - Mathf.Atan2(up.Y, up.X), -Mathf.Pi, Mathf.Pi);
        }

        /// <summary>
        /// Returns direction from Vector2.Right. In Degrees.
        /// </summary>
        public static Vector2 GetDirectionD(this float degrees) => GetDirection(degrees.toRadians());

        /// <summary>
        /// Returns direction from Vector2.Right. In Degrees.
        /// </summary>
        public static Vector2 GetDirection(this float radians) => Vector2.Right.Rotated(radians).Normalized();

        // Cross product for Vector2 (returns a zero Vector3)
        public static float Cross(this Vector2 vectorA, Vector2 vectorB)
        {
            return vectorA.X * vectorB.Y - vectorA.Y * vectorB.X;
        }

        /// <summary>
        /// Determines if two lines intersect and returns the intersection point. Does not limit the intersection to the line segments.
        /// </summary>
        /// <param name="pivot">Origin of the first line.</param>
        /// <param name="direction">Direction of the first line.</param>
        /// <param name="pivotOther">Origin of the second line.</param>
        /// <param name="directionOther">Direction of the second line.</param>
        /// <param name="intersection">Output intersection point if the lines intersect.</param>
        /// <returns>True if the lines intersect, otherwise false.</returns>
        public static bool TryIntersect(this Vector2 pivot, Vector2 direction, Vector2 pivotOther, Vector2 directionOther, out Vector2 intersection)
        {
            float denominator = direction.X * directionOther.Y - direction.Y * directionOther.X;
            intersection = default;

            // Check if lines are parallel (denominator is zero)
            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                return false;
            }

            float t = ((pivotOther.X - pivot.X) * directionOther.Y - (pivotOther.Y - pivot.Y) * directionOther.X) / denominator;

            intersection = pivot + t * direction;
            return true;
        }

        /// <summary>
        /// Determines if two line segments intersect and returns the intersection point.
        /// Unlike TryIntersect, this treats the lines as bounded segments rather than infinite lines,
        /// so the intersection point must lie within both segments' start and end points.
        /// </summary>
        /// <param name="from">Start point of the first segment.</param>
        /// <param name="to">End point of the first segment.</param>
        /// <param name="fromOther">Start point of the second segment.</param>
        /// <param name="toOther">End point of the second segment.</param>
        /// <param name="intersection">Output intersection point if the segments intersect.</param>
        /// <returns>True if the segments intersect within their bounds, otherwise false.</returns>
        public static bool TryIntersectLimited(this Vector2 from, Vector2 to, Vector2 fromOther, Vector2 toOther, out Vector2 intersection)
        {
            Vector2 a = to - from;
            Vector2 b = toOther - fromOther;

            intersection = default;

            float denominator = a.X * b.Y - a.Y * b.X;

            // Parallel (or collinear) lines
            if (Mathf.Abs(denominator) < Mathf.Epsilon)
            {
                return false;
            }

            Vector2 diff = fromOther - from;

            float t = (diff.X * b.Y - diff.Y * b.X) / denominator;
            float u = (diff.X * a.Y - diff.Y * a.X) / denominator;

            // Check that the intersection lies within both segments
            if (t < 0f || t > 1f || u < 0f || u > 1f)
            {
                return false;
            }

            intersection = from + t * a;
            return true;
        }

        public static (Vector2 min, Vector2 max) MinMax(params Vector2[] values) => (Min(values), Min(values));

        public static Vector2 Min(params Vector2[] values)
        {
            var value = values[0];

            for (byte i = 1; i < values.Length && i < byte.MaxValue; i++)
            {
                for (byte k = 0; k < 2; k++)
                {
                    value[k] = Mathf.Min(value[k], values[i][k]);
                }
            }

            return value;
        }

        public static Vector2 Max(params Vector2[] values)
        {
            var value = values[0];

            for (byte i = 1; i < values.Length && i < byte.MaxValue; i++)
            {
                for (byte k = 0; k < 2; k++)
                {
                    value[k] = Mathf.Max(value[k], values[i][k]);
                }
            }

            return value;
        }

        public static Vector2 Average(params Vector2[] corners)
        {
            var sum = corners[0];

            for (int i = 1; i < corners.Length; i++)
            {
                sum += corners[i];
            }

            return sum / corners.Length;
        }

        /// <summary>
        /// Clamps the vector to a normalized vector. Works a little like Vector3.MoveTowards.
        /// </summary>
        public static Vector2 ClampNormalized(this Vector2 value)
        {
            var normalized = value.Normalized();
            return new(
                Mathf.Abs(value.X) > Mathf.Abs(normalized.X) ? normalized.X : value.X,
                Mathf.Abs(value.Y) > Mathf.Abs(normalized.Y) ? normalized.Y : value.Y
            );
        }

        public static Vector2[] InterpolateEvenly(this Vector2[] originalPoints, int targetLength)
        {
            if (originalPoints == null || originalPoints.Length < 2 || targetLength < 2)
            {
                throw new System.ArgumentException("Invalid input data.");
            }

            // Step 1: Calculate cumulative distances
            var cumulativeDistances = new float[originalPoints.Length];
            cumulativeDistances[0] = 0f;

            for (int i = 1; i < originalPoints.Length; i++)
            {
                cumulativeDistances[i] = cumulativeDistances[i - 1] + originalPoints[i - 1].DistanceTo(originalPoints[i]);
            }

            var totalDistance = cumulativeDistances[^1];
            var intervalDistance = totalDistance / (targetLength - 1);

            // Step 2: Interpolate new points at evenly spaced intervals
            var newPoints = new List<Vector2>()
            {
                originalPoints[0]
            };

            for (int i = 1; i < targetLength - 1; i++)
            {
                var targetDistance = i * intervalDistance;
                var newPoint = InterpolateAtDistance(originalPoints, cumulativeDistances, targetDistance);
                newPoints.Add(newPoint);
            }

            newPoints.Add(originalPoints[^1]);

            return newPoints.ToArray();

            static Vector2 InterpolateAtDistance(Vector2[] points, float[] cumulativeDistances, float targetDistance)
            {
                for (int i = 1; i < points.Length; i++)
                {
                    if (cumulativeDistances[i] >= targetDistance)
                    {
                        var segmentStartDist = cumulativeDistances[i - 1];
                        var segmentEndDist = cumulativeDistances[i];
                        var segmentLength = segmentEndDist - segmentStartDist;

                        var t = (targetDistance - segmentStartDist) / segmentLength;
                        return points[i - 1].Lerp(points[i], t);
                    }
                }

                return points[^1]; // Should never reach here if inputs are valid
            }
        }

        public static Vector2I Min(this Vector2I a, Vector2I b) => new(Mathf.Min(a.X, b.X), Mathf.Min(a.Y, b.Y));
        public static Vector2I Max(this Vector2I a, Vector2I b) => new(Mathf.Max(a.X, b.X), Mathf.Max(a.Y, b.Y));

        public static Vector2 Random(this Vector2 a, Vector2 b)
        {
            return new(Cutulu.Core.Random.Range(a.X, b.X), Cutulu.Core.Random.Range(a.Y, b.Y));
        }

        public static Vector2 Sum(this Vector2[] b)
        {
            if (b.IsEmpty()) return default;
            var sum = Vector2.Zero;

            for (ushort i = 0; i < b.Length; i++)
            {
                sum += b[i];
            }

            return sum;
        }

        public static Vector2I Sum(this Vector2I[] b)
        {
            if (b.IsEmpty()) return default;
            var sum = Vector2I.Zero;

            for (ushort i = 0; i < b.Length; i++)
            {
                sum += b[i];
            }

            return sum;
        }
    }
}
#endif