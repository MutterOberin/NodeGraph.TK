/*

Copyright (c) 2019, Wito Engelke
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer 
          in the documentation and/or other materials provided with the distribution.
        * Neither the name of PeeWeeK.NET nor the names of its contributors may be used to endorse or promote products derived from this 
          software without specific prior written permission.


THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

Inspired by Project: https://github.com/peeweek/NodeGraph/tree/master/NodeGraphControl

*/

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
