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
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using System.Xml;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using NodeGraphTK.Source;
using System.Drawing.Imaging;

namespace NodeGraph.TK
{
    /// <summary>
    /// Represents a base node for use in a NodeGraphView
    /// </summary>
    public class Node
    {
        #region - Private Variables -

        private static uint count;

        protected uint id;

        protected Vector4 color_fill;

        protected Vector3 position; // X,Y = Position, Z currently unused
        protected Vector2 property; // X,Y = Width, Height

        protected bool hovered;
        protected bool selected;
        protected bool selectable;

        protected string name;
        protected string comment;

        protected View view;

        protected List<Connector> connectors;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new <see cref="Node"/> into the <see cref="Graph"/>, given coordinates and ability to be selected
        /// </summary>
        public Node(float X, float Y, View view, bool selectable = true)
        {
            this.id = count; count++;

            this.position.X = X;
            this.position.Y = Y;
            this.view       = view;
            this.property.X = 192;
            this.property.Y = 64;
            this.name       = "DI: Dummy";
            this.hovered    = false;
            this.selected   = false;
            this.selectable = selectable;
            this.comment    = "";

            this.color_fill = new Vector4(0.56f, 0.50f, 0.50f, 1.0f);

            this.connectors = new List<Connector>();

            this.connectors.Add(new Connector("Connector 1", this, ConnectorType.Input, 0));
            this.connectors.Add(new Connector("Connector 2", this, ConnectorType.Input, 1));

            this.connectors.Add(new Connector("Connector 1", this, ConnectorType.Output, 0));
            this.connectors.Add(new Connector("Connector 2", this, ConnectorType.Output, 1));
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets / Sets Node Fill Color
        /// </summary>
        [Category("Node Properties")]
        public Color NodeFillColor
        {
            get => Util.VectorToColor(this.color_fill);
            set => this.color_fill = Util.ColorToVector4(value);
        }

        [Browsable(false)]
        public bool Hovered
        {
            get => this.hovered;
            set => this.hovered = value;
        }

        /// <summary>
        /// Whether the node is selected
        /// </summary>
        [Browsable(false)]
        public bool Selected
        {
            get => this.selected;
            set => this.selected = value;
        }

        /// <summary>
        /// Whether the node can be selected
        /// </summary>
        [Browsable(false)]
        public bool Selectable
        {
            get => this.selectable;
            set => this.selectable = value;
        }

        /// <summary>
        /// The Display name of the node
        /// </summary>
        [Category("Node Properties")]
        public string Name => this.GetName();

        /// <summary>
        /// Position in X (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public Vector3 Position
        {
            get => this.position;
            set => this.position = value;
        }

        /// <summary>
        /// Properties (Width, Height)
        /// </summary>
        [Category("Node Properties")]
        public Vector2 Property
        {
            get => this.property;
            set => this.property = value;
        }

        /// <summary>
        /// Position in X (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public float X
        {
            get => this.position.X;
            set => this.position.X = value;
        }

        /// <summary>
        /// Position in Y (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public float Y
        {
            get => this.position.Y;
            set => this.position.Y = value;
        }

        /// <summary>
        /// Width (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public float Width
        {
            get => this.property.X;
            set => this.property.X = value;
        }

        /// <summary>
        /// Height (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public float Height
        {
            get => this.property.Y;
            set => this.property.Y = value;
        }

        /// <summary>
        /// The NodeGraphView associated to this node
        /// </summary>
        [Browsable(false)]
        public View View => this.view;

        /// <summary>
        /// The hit rectangle of the Node
        /// </summary>
        [Browsable(false)]
        public RectangleF HitRectangle => this.GetAreaHit();

        /// <summary>
        /// The list of NodeGraphConnectors owned by this Node
        /// </summary>
        [Category("Node Properties")]
        public List<Connector> Connectors => this.connectors;

        /// <summary>
        /// The displayed comment of the node
        /// </summary>
        [Category("Node Properties")]
        public string Comment
        {
            get => this.comment;
            set => this.comment = value;
        }


        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the name of the node: can be overriden to match custom names.
        /// </summary>
        protected virtual string GetName()
        {
            return $"Node {this.id.ToString("00")} Name: {this.name}";
        }

        public virtual RectangleF GetArea()
        {
            return new RectangleF(this.position.X, this.position.Y, this.property.X, this.property.Y);
        }

        /// <summary>
        /// Returns the Click Area of the Node. Global Coordinates.
        /// </summary>
        public virtual RectangleF GetAreaHit()
        {
            // Hit Zone Bleeding can be done here
            return new RectangleF(this.position.X, this.position.Y, this.property.X, this.property.Y);
        }

        /// <summary>
        /// Gets the connector index, given the connector object reference
        /// </summary>
        /// <param name="connector">the connector reference</param>
        /// <returns>the connector index</returns>
        public int GetConnectorIndex(Connector connector)
        {
            return this.connectors.IndexOf(connector);
        }

        /// <summary>
        /// Gets the Connector Count ether for Inputs or Outputs
        /// </summary>
        public int GetConnectorCount(ConnectorType type)
        {
            return this.connectors.FindAll(c => c.Type == type).Count;
        }

        /// <summary>
        /// Gets the Connector Count ether for Inputs or Outputs
        /// </summary>
        public int GetConnectorLinkedCount(ConnectorType type)
        {
            return this.connectors.FindAll(c => c.Type == type && c.Linked).Count;
        }

        /// <summary>
        /// Gets the Maximal Connector Count
        /// </summary>
        public int GetConnectorCount_Max()
        {
            return Math.Max(GetConnectorCount(ConnectorType.Input), GetConnectorCount(ConnectorType.Output));
        }

        /// <summary>
        /// Gets the Maximal Connector Count
        /// </summary>
        public int GetConnectorCount_Min()
        {
            return Math.Min(GetConnectorCount(ConnectorType.Input), GetConnectorCount(ConnectorType.Output));
        }

        public bool HitTest(Vector3 coordinates, out HitType hitType, out Connector connector)
        {
            hitType   = HitType.None;
            connector = null;

            RectangleF hit = new RectangleF(coordinates.X, coordinates.Y, 0, 0);

            if (hit.IntersectsWith(this.GetAreaHit()))
            {
                if (HitTestConnector(coordinates, out connector))
                    hitType = HitType.Connector;
                else
                    hitType = HitType.Node;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if a Connector is by given coordinates
        /// </summary>
        /// <param name="coordinates">Absolute World Coordinates to perform Hit-Test</param>
        /// <param name="connector">If Hit-Test true, the Connector hit</param>
        private bool HitTestConnector(Vector3 coordinates, out Connector connector)
        {
            connector = null;

            RectangleF hit = new RectangleF(coordinates.X, coordinates.Y, 0, 0);

            foreach (Connector con in this.connectors)
            {
                if (hit.IntersectsWith(con.GetAreaHit()))
                {
                    connector = con;

                    return true;
                }
            }

            return false;
        }

        public virtual void Render()
        {
            if (this.selected)
            {
                if (this.hovered)
                    GL.Color4(Util.VectorToColor(view.ColorNodeFillHovered));
                else
                    GL.Color4(Util.VectorToColor(view.ColorNodeFillSelected));
            }
            else
            {
                if (this.hovered)
                    GL.Color4(Util.VectorToColor(view.ColorNodeFillHovered));
                else
                    GL.Color4(this.color_fill);
            }

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(position.X, position.Y, position.Z);
            GL.Vertex3(position.X + property.X, position.Y, position.Z);
            GL.Vertex3(position.X + property.X, position.Y + property.Y, position.Z);
            GL.Vertex3(position.X, position.Y + property.Y, position.Z);

            GL.End();
        }

        /// <summary>
        /// Updates the Texture used during Node Draw
        /// </summary>
        public virtual void UpdateTexture(Graphics g)
        {
            //GL.Color4(this.view.ColorNodeFill);

            //GL.Begin(PrimitiveType.Quads);

            //GL.Vertex3(this.position.X, position.Y, 0);
            //GL.Vertex3(this.position.X + property.X, position.Y, 0);
            //GL.Vertex3(this.position.X + property.X, position.Y + property.Y, 0);
            //GL.Vertex3(this.position.X, position.Y + property.Y, 0);

            //GL.End();

            //GL.Color4(this.view.ColorNodeOutline);

            //GL.Begin(PrimitiveType.LineLoop);

            //GL.Vertex3(this.position.X, position.Y, 0);
            //GL.Vertex3(this.position.X + property.X, position.Y, 0);
            //GL.Vertex3(this.position.X + property.X, position.Y + property.Y, 0);
            //GL.Vertex3(this.position.X, position.Y + property.Y, 0);

            //GL.End();

            float zoom = 1.0f;

            bool shadow = false;

            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //Vector2 CtrlPos = view.Panel.ViewToControl(new Vector2(x, y));

            //float ScaledX = CtrlPos.X;
            //float ScaledY = CtrlPos.Y;

            Rectangle ViewRectangle = new Rectangle(0, 0, (int)(this.Width * zoom), (int)(this.Height * zoom));

            // Node Shadow
            if (shadow)
            {
                g.DrawImage(Resources.NodeShadow, new Rectangle(8 - (int)(0.1f * this.Width), 8 - (int)(0.1f * this.Height), 
                    (int)this.Width + (int)(0.2f * this.Width), (int)this.Height + (int)(0.2f * this.Height)));
            }
            
            // Node Todo: change how selection works
            if (!this.selected)
            {
                g.FillRectangle(new SolidBrush(this.NodeFillColor), ViewRectangle);
                
                g.FillRectangle(new SolidBrush(Util.VectorToColor(view.ColorNodeHeader)), new Rectangle(ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, (int)(view.SizeNodeHeader * zoom)));
                g.DrawRectangle(new Pen(Util.VectorToColor(view.ColorNodeOutline), 1.0f), ViewRectangle);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Util.VectorToColor(view.ColorNodeFillSelected)), ViewRectangle);

                g.FillRectangle(new SolidBrush(Util.VectorToColor(view.ColorNodeHeader)), new Rectangle(ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, (int)(view.SizeNodeHeader * zoom)));
                g.DrawRectangle(new Pen(Util.VectorToColor(view.ColorNodeOutlineSelected), 1.0f), ViewRectangle);
            }

            // Node Text
            if (zoom > 0.8f)
            {                
                //g.DrawString(this.Name, new Font(view.Font_Node_Title.Name, view.Font_Node_Title.Size * zoom), new SolidBrush(Util.VectorToColor(view.ColorNodeTextShadow)), 
                //    new Point(ViewRectangle.X + (int)(2.0f * zoom) + 1, ViewRectangle.Y + (int)(2.0f * zoom) + 1));
                g.DrawString(this.Name, new Font(view.Font_Node_Title.Name, view.Font_Node_Title.Size * zoom), new SolidBrush(Util.VectorToColor(view.ColorNodeText)), 
                    new Point(ViewRectangle.X + (int)(2.0f * zoom) + 0, ViewRectangle.Y + (int)(2.0f * zoom) + 3));
            }

            // Connectors Input
            for (int i = 0; i < this.connectors.Count; i++)
            {
                this.connectors[i].Draw(g, zoom);
            }

            // Comment
            if (!string.IsNullOrEmpty(this.comment))
            {
                //g.DrawString(this.comment, view.Panel.NodeScaledTitleFont, view.Panel.NodeText, new Point(ScaledX, ScaledY - (int)(16 * zoom)));
            }        
        }

        public override string ToString()
        {
            return GetName();
        }

        #endregion
    }
}
