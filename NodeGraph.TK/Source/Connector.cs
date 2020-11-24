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

using OpenTK;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NodeGraph.TK
{
    /// <summary>
    /// Type of Connector.
    /// </summary>
    public enum ConnectorType
    {
        Input,
        Output
    }

    /// <summary>
    /// Data of Connector.
    /// </summary>
    public enum ConnectorData
    {
        None,
        Scalars,
        Vectors,
        ScalarField,
        VectorField,
        Bounds,
        Points,
        Edges,
        Lines,
        Faces3,
        Faces4,
        Values,
        Tetras,
        Prism,
        Pyramid,
        Hexahedral,    
        Texture,
        Bitmap,
        Image2,
        Image3,
        Material,
        Camera,
    }

    /// <summary>
    /// Represents a Connector on a Node
    /// </summary>    
    public class Connector
    {
        #region - Private Variables -

        private float scale;

        private Pen pen;
        private Font font;
        private Brush brush;

        private Vector4 color;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphConnector, given a name, a parent container, type and index
        /// </summary>
        /// <param name="newName">The display name of the connector</param>
        /// <param name="newParent">Reference to the parent NodeGraphNode</param>
        /// <param name="newConnectorType">Type of the connector (input/output)</param>
        /// <param name="newConnectorIndex">Connector Index</param>
        public Connector(string newName, Node newParent, ConnectorType newConnectorType, int newConnectorIndex)
        {
            // Try Setting Connector Data Correct
            ConnectorData newConnectorData = ConnectorData.None;

            bool success = Enum.TryParse(newName, out newConnectorData);

            if (!success)
                Console.WriteLine($"## ERROR:\t\tCasting {newName} to Enum failed");

            this.color = new Vector4(0, 0, 0, 1);

            this.Name   = newName;
            this.Parent = newParent;
            this.Type   = newConnectorType;
            this.Data   = newConnectorData;
            this.Index  = newConnectorIndex;

            this.scale = newParent.Tex_Scale;

            this.pen   = new Pen(Util.VectorToColor(this.color), 1.0f);
            this.font  = new Font(this.Parent.View.Font_Node_Connector.Name, this.Parent.View.Font_Node_Connector.Size * this.scale);
            this.brush = new SolidBrush(Util.VectorToColor(this.color));
        }

        /// <summary>
        /// Creates a new NodeGraphConnector, given a name, a parent container, type and index
        /// </summary>
        /// <param name="newName">The display name of the connector</param>
        /// <param name="newParent">Reference to the parent NodeGraphNode</param>
        /// <param name="newConnectorType">Type of the connector (input/output)</param>
        /// <param name="newConnectorIndex">Connector Index</param>
        public Connector(string newName, Node newParent, ConnectorType newConnectorType, ConnectorData newConnectorData, int newConnectorIndex)
        {
            this.color = new Vector4(0, 0, 0, 1);

            this.Name   = newName;
            this.Parent = newParent;
            this.Type   = newConnectorType;
            this.Data   = newConnectorData;
            this.Index  = newConnectorIndex;

            this.scale = newParent.Tex_Scale;

            this.pen = new Pen(Util.VectorToColor(this.color), 1.0f);
            this.font = new Font(this.Parent.View.Font_Node_Connector.Name, this.Parent.View.Font_Node_Connector.Size * this.scale);
            this.brush = new SolidBrush(Util.VectorToColor(this.color));
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Index of the Connector
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Name of the Connector that will be displayed
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets / Sets Connectors Color
        /// </summary>
        public Color Color
        {
            get { return Util.VectorToColor(this.color); }
            set { this.color = Util.ColorToVector4(value); }
        }

        /// <summary>
        /// The parent node that contains the Connector
        /// </summary>
        public Node Parent { get; }

        /// <summary>
        /// Type of the Connector (input/output)
        /// </summary>
        public ConnectorType Type { get; }

        /// <summary>
        /// Data of the Connector
        /// </summary>
        public ConnectorData Data { get; }

        public bool Linked { get; set; }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the visible Area of the Connector. Local Coordinates.
        /// </summary>
        public RectangleF GetArea()
        {
            Vector2 pos = Vector2.Zero;

            switch (this.Type)
            {
                case ConnectorType.Input:
                    {
                        pos.X = 0;
                        pos.Y = this.Parent.View.SizeNodeHeader * this.scale + 6 * this.scale + (this.Index * 16 * this.scale);
                    }
                    break;

                case ConnectorType.Output:
                    {
                        pos.X = this.Parent.HitRectangle.Width * this.scale - 12 * this.scale;
                        pos.Y = this.Parent.View.SizeNodeHeader * this.scale + 6 * this.scale + (this.Index * 16 * this.scale);
                    }
                    break;

                default:
                    break;
            }
            
            return new RectangleF(pos.X, pos.Y, 12 * scale, 8 * scale);
        }

        /// <summary>
        /// Returns the Click Area of the Connector. Global Coordinates.
        /// </summary>
        public RectangleF GetAreaHit()
        {
            // Hit Zone Bleeding can be done here

            Vector2 pos = Vector2.Zero;

            switch (this.Type)
            {
                case ConnectorType.Input:
                    {
                        pos.X = this.Parent.X;
                        pos.Y = this.Parent.Y + this.Parent.HitRectangle.Height * this.scale - this.Parent.View.SizeNodeHeader * this.scale - 10 * this.scale - (this.Index * 16 * this.scale);
                    }
                    break;

                case ConnectorType.Output:
                    {
                        pos.X = this.Parent.X + this.Parent.HitRectangle.Width * this.scale - 12 * this.scale;
                        pos.Y = this.Parent.Y + this.Parent.View.SizeNodeHeader * this.scale + 6 * this.scale + (this.Index * 16 * this.scale);
                    }
                    break;

                default:
                    break;
            }
            
            return new RectangleF(pos.X - 2, pos.Y - 2, 12 * this.scale + 4, 8 * this.scale + 4);
        }

        /// <summary>
        /// Returns the Position of the Displayed Text. Local Coordinates.
        /// </summary>
        public Vector2 GetTextPosition(Graphics g)
        {
            Vector2 pos = Vector2.Zero;

            switch (this.Type)
            {
                case ConnectorType.Input:
                    {
                        pos.X = 16 * this.scale;
                        pos.Y = this.Parent.View.SizeNodeHeader * this.scale + 5 * this.scale + (this.Index * 16 * this.scale);
                    }
                    break;

                case ConnectorType.Output:
                    {
                        SizeF measure = g.MeasureString(this.Name, this.font);

                        pos.X = this.Parent.HitRectangle.Width * this.scale - 15 * this.scale - measure.Width;
                        pos.Y = this.Parent.View.SizeNodeHeader * this.scale + 5 * this.scale + (this.Index * 16 * this.scale);                  
                    }
                    break;

                default:
                    break;

            }

            return pos;
        }

        /// <summary>
        /// Draws the connector
        /// </summary>
        public void Draw(Graphics g)
        {
            if (this.Parent.View.ViewZoomCurrent < this.Parent.View.Node_Connector_Text_TH)
                return;

            RectangleF r0 = this.GetArea();
            RectangleF r1 = this.GetAreaHit();

            g.FillRectangle(brush, r0);
            g.DrawRectangle(pen, r0.X, r0.Y, r0.Width, r0.Height); // Someone at MS forgot to write the RectangleF overload :-D
            g.DrawRectangle(new Pen(Color.LightSkyBlue), r1.X, r1.Y, r1.Width, r1.Height); // Someone at MS forgot to write the RectangleF overload :-D

            Vector2 pos = GetTextPosition(g);

            g.DrawString(this.Name, this.font, brush, pos.X, pos.Y);            
        }

        #endregion
    }

}
