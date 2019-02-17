using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;

namespace NodeGraph.TK
{
    public static class NodeGraphUtil
    {
        /// <summary>
        /// Clamps val to [min, max]
        /// </summary>
        /// <param name="val">value to clamp</param>
        /// <param name="min">min posible value, default 0</param>
        /// <param name="max">max posible value, default 1</param>
        /// <returns>value in range [min, max]</returns>
        public static float Clamp(float val, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(val, max));
        }

        /// <summary>
        /// Converts a Color to Vector3
        /// </summary>
        public static Vector3 ColorToVector3(Color color)
        {
            Vector3 result = Vector3.Zero;

            result.X = color.R / 255.0f;
            result.Y = color.G / 255.0f;
            result.Z = color.B / 255.0f;

            return result;
        }

        /// <summary>
        /// Converts a Color to Vector4
        /// </summary>
        public static Vector4 ColorToVector4(Color color)
        {
            Vector4 result = Vector4.Zero;

            result.X = color.R / 255.0f;
            result.Y = color.G / 255.0f;
            result.Z = color.B / 255.0f;
            result.W = color.A / 255.0f;

            return result;
        }

        /// <summary>
        /// Converts a Vector3 to Color
        /// </summary>
        public static Color VectorToColor(Vector3 color)
        {
            return Color.FromArgb((int)(Clamp(color.X) * 255),
                                  (int)(Clamp(color.Y) * 255),
                                  (int)(Clamp(color.Z) * 255));
        }

        /// <summary>
        /// Converts a Vector4 to Color
        /// </summary>
        public static Color VectorToColor(Vector4 color)
        {
            return Color.FromArgb((int)(Clamp(color.W) * 255),
                                  (int)(Clamp(color.X) * 255),
                                  (int)(Clamp(color.Y) * 255),
                                  (int)(Clamp(color.Z) * 255));
        }
    }
}
