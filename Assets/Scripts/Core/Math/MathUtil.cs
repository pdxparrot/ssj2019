using UnityEngine;

namespace pdxpartyparrot.Core.Math
{
    public static class MathUtil
    {
        // a less forgiving epsilon than float.Epsilon
        public const float Epsilon = 0.001f;

        // modulus that wraps negative numbers
        // NOTE: this does floating point math under the hood so only use if necessary
        public static int WrapMod(int n, int m)
        {
            return (int)WrapMod((float)n, (float)m);
        }

        public static float WrapMod(float n, float m)
        {
            return n - m * Mathf.Floor(n / m);
        }

        public static float WrapAngle(float angle)
        {
            return angle % 360.0f;
        }

        public static float WrapAngleRad(float angle)
        {
            return angle % (Mathf.PI * 2.0f);
        }
    }
}