using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaturnGame
{
    /// <summary>
    /// A Math Library specifically made for Saturn.
    /// </summary>
    public static class SaturnMath
    {
        /// <summary>
        /// Used to convert a number of Ticks to it's equivalent fraction of a Measure. <br />
        /// <i>This conversion should happen as late as possible!</i><br />
        /// <c>measureFraction = ticks * SaturnMath.tickToMeasure;</c>
        /// </summary>
        public const float tickToMeasure = 1.0f / 1920.0f;

        /// <summary>
        /// Unclamped version of Unity's <c>Mathf.InverseLerp</c>.
        /// </summary>
        /// <returns>
        /// Where <c>value</c> lies between <c>a</c> and <c>b</c>.
        /// </returns>
        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
            {
                return (value - a) / (b - a);
            }

            return 0f;
        }

        /// <summary>
        /// Returns the Euclidean remainder ("true modulo") of a number. <br />
        /// The result will always be positive, unlike using the <c>%</c> operator in C#.
        /// </summary>
        public static int Modulo(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        /// <summary>
        /// Similar to Modulo. Loops a value within a specified range. <br />
        /// For example: if x is 1 greater than max, it'll "loop back around" and return min.
        /// </summary>
        public static int Loop(int x, int min, int max)
        {
            if (min < x && x < max) return x;

            return Modulo(x - min, max - min + 1) + min;
        }

        /// <summary>
        /// Remaps a value from range <c>inMin - inMax</c> to <c>outMin - outMax</c>
        /// </summary>
        public static float Remap(float input, float inMin, float inMax, float outMin, float outMax, bool clamp = false)
        {
            if (inMin == inMax || outMin == outMax) return 0;
            if (inMin == outMin && inMax == outMax) return input;

            float result = outMin + (input - inMin) * (outMax - outMin) / (inMax - inMin);
            return clamp ? Mathf.Clamp(result, outMin, outMax) : result;
        }

        /// <summary>
        /// Linearly Interpolate around a circle/clock, where <c>m</c> is the maximum value before it loops back to 0.
        /// </summary>
        public static float LerpRound(int a, int b, float t, int m)
        {
            if (Mathf.Abs(a - b) > m * 0.5f)
            {
                if (a > b) b += m;
                else a += m;
            }
            
            return Mathf.Lerp(a, b, t);
        }

        public static string GetDifficultyString(float difficulty)
        {
            return ((int)difficulty).ToString() + (difficulty % 1 > 0.6f ? "+" : "");
        }

        public static class Ease
        {
            // This is kinda wrong but looks "good enough".
            /// <summary>
            /// Custom Easing curve for Reverse Gimmicks. <br />
            /// Slightly more linear than Sine easing.
            /// </summary>
            public static float Reverse(float x)
            {
                return Sine.Out(x) * 0.75f + x * 0.25f;
            }

            /// <summary>Sine Easing.</summary>
            public static class Sine
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => 1 - Mathf.Cos(x * Mathf.PI * 0.5f);
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => Mathf.Sin(x * Mathf.PI * 0.5f);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x) => (Mathf.Cos(x * Mathf.PI) - 1) * -0.5f;
            }

            /// <summary>Quadratic Easing.</summary>
            public static class Quad
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => x * x;
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Cubic Easing.</summary>
            public static class Cubic
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => x * x * x;
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Quartic Easing.</summary>
            public static class Quart
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => x * x * x * x;
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Quintic Easing.</summary>
            public static class Quint
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => x * x * x * x * x;
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Exponential Easing.</summary>
            public static class Exponential
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => Mathf.Pow(2, 10 * (x - 1));
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Circular Easing.</summary>
            public static class Circular
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => -(Mathf.Sqrt(1 - x * x) - 1);
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Elastic Easing.</summary>
            public static class Elastic
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => 1 - Out(1 - x);
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x)
                {
                    float p = 0.3f;
                    return Mathf.Pow(2, -10 * x) * Mathf.Sin((x - p * 0.25f) * (2 * Mathf.PI) / p) + 1;
                }
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Back Easing. Pulls back slowly then speeds up.</summary>
            public static class Back
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x)
                {
                    float b = 1.70158f;
                    return x * x * ((b + 1) * x - b);
                }
                /// <summary>Starts fast and slows down.</summary>
                public static float OutBack(float x) => 1 - In(1 - x);
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOutBack(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }

            /// <summary>Bounce Easing.</summary>
            public static class Bounce
            {
                /// <summary>Starts slow and speeds up.</summary>
                public static float In(float x) => 1 - Out(1 - x);
                /// <summary>Starts fast and slows down.</summary>
                public static float Out(float x)
                {
                    float div = 2.75f;
                    float mult = 7.5625f;

                    if (x < 1 / div)
                    {
                        return mult * x * x;
                    }
                    else if (x < 2 / div)
                    {
                        x -= 1.5f / div;
                        return mult * x * x + 0.75f;
                    }
                    else if (x < 2.5 / div)
                    {
                        x -= 2.25f / div;
                        return mult * x * x + 0.9375f;
                    }
                    else
                    {
                        x -= 2.625f / div;
                        return mult * x * x + 0.984375f;
                    }
                }
                /// <summary>Starts slow, speeds up, then ends slow.</summary>
                public static float InOut(float x)
                {
                    return x < 0.5 ? In(x * 2) * 0.5f : 1 - In((1 - x) * 2) * 0.5f;
                }
            }
        }
    }
}
