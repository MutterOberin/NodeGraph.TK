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
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace NodeGraph.TK
{
    /// <summary>
    /// Represents a Link between two Connectors
    /// </summary>
    public class Link
    {
        #region - Private Variables -

        private Connector conn_0;
        private Connector conn_1;

        protected View view;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new Link, given input and output Connector
        /// </summary>
        public Link(Connector input, Connector output, View view)
        {
            this.conn_0 = input;
            this.conn_1 = output;

            this.view = view;
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// The Input of this Link which is connected to an Connector of with Type Output
        /// </summary>
        public Connector Input => this.conn_0;

        /// <summary>
        /// The Output of this Link which is connected to an Connector of with Type Input
        /// </summary>
        public Connector Output => this.conn_1;

        /// <summary>
        /// The View this Link is styled by
        /// </summary>
        public View View { get => this.view; set => this.view = value; }

        #endregion

        #region - Methods -

        public virtual void Render()
        {
            RectangleF rect0 = conn_0.GetAreaHit();
            RectangleF rect1 = conn_1.GetAreaHit();

            Vector3 pos_0 = new Vector3(rect0.X + 0.5f * rect0.Width, rect0.Y + 0.5f * rect0.Height, 0);
            Vector3 pos_1 = new Vector3(rect1.X + 0.5f * rect1.Width, rect1.Y + 0.5f * rect1.Height, 0); ;
            Vector3 pos_2 = pos_0 + 0.5f * (pos_1 - pos_0);

            if (conn_0.Parent.Selected || conn_1.Parent.Selected || conn_0.Parent.Hovered || conn_1.Parent.Hovered)
                GL.Color4(view.ColorLinkSelected);
            else
                GL.Color4(view.ColorLink);

            GL.Begin(PrimitiveType.LineStrip);

            GL.Vertex3(pos_0.X, pos_0.Y, pos_0.Z);
            GL.Vertex3(pos_2.X, pos_0.Y, pos_0.Z);
            GL.Vertex3(pos_2.X, pos_1.Y, pos_0.Z);
            GL.Vertex3(pos_1.X, pos_1.Y, pos_0.Z);

            GL.End();
        }

        #endregion
    }
}
