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

namespace NodeGraph.TK
{
    /// <summary>
    /// Represents a base node for use in a NodeGraphView
    /// </summary>
    public class NodeGraphNode
    {
        #region - Private Variables -

        private static uint count;

        protected Vector4 color_fill;

        protected uint id;

        protected Vector3 position; // X,Y = Position, Z currently unused
        protected Vector2 property; // X,Y = Width, Height

        protected bool selected;
        protected bool selectable;

        protected string name;
        protected string comment;

        protected NodeGraphView view;

        protected List<NodeGraphConnector> connectors;

        public event PaintEventHandler PostDraw;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphNode into the NodeGraphView, given coordinates and ability to be selected
        /// </summary>
        /// <param name="p_X"></param>
        /// <param name="p_Y"></param>
        /// <param name="view"></param>
        /// <param name="selectable"></param>
        public NodeGraphNode(float X, float Y, NodeGraphView view, bool selectable = true)
        {
            this.id = count; count++;

            this.position.X = X;
            this.position.Y = Y;
            this.view       = view;
            this.property.X = 128;
            this.property.Y = 64;
            this.name       = "Test Void Node";
            this.selected   = false;
            this.selectable = selectable;
            this.comment    = "";

            this.connectors = new List<NodeGraphConnector>();

            this.connectors.Add(new NodeGraphConnector("Connector 1", this, ConnectorType.Input, 0));
            this.connectors.Add(new NodeGraphConnector("Connector 2", this, ConnectorType.Input, 1));

            this.connectors.Add(new NodeGraphConnector("Connector 1", this, ConnectorType.Output, 0));
            this.connectors.Add(new NodeGraphConnector("Connector 2", this, ConnectorType.Output, 1));
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Gets / Sets Node Fill Color
        /// </summary>
        [Category("Node Properties")]
        public Color NodeFillColor
        {
            get { return NodeGraphUtil.VectorToColor(this.color_fill); }
            set { this.color_fill = NodeGraphUtil.ColorToVector4(value); }
        }

        /// <summary>
        /// Whether the node can be selected
        /// </summary>
        [Browsable(false)]
        public bool Selected
        {
            get { return this.selected; }
            set { this.selected = value; }
        }

        /// <summary>
        /// Whether the node can be selected
        /// </summary>
        [Browsable(false)]
        public bool Selectable
        {
            get { return this.selectable; }
            set { this.selectable = value; }
        }

        /// <summary>
        /// The Display name of the node
        /// </summary>
        [Category("Node Properties")]
        public string Name
        {
            get { return this.GetName(); }
        }

        /// <summary>
        /// Position in X (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public Vector3 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
            }
        }

        /// <summary>
        /// Position in X (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public float X
        {
            get { return this.position.X; }
            set
            {
                this.position.X = value;
            }
        }

        /// <summary>
        /// Position in Y (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public float Y
        {
            get { return this.position.Y; }
            set
            {
                this.position.Y = value;
            }
        }

        /// <summary>
        /// Width (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public float Width
        {
            get { return this.property.X; }
            set
            {
                this.property.X = value;
            }
        }

        /// <summary>
        /// Height (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public float Height
        {
            get { return this.property.Y; }
            set
            {
                this.property.Y = value;
            }
        }

        /// <summary>
        /// The NodeGraphView associated to this node
        /// </summary>
        [Browsable(false)]
        public NodeGraphView ParentView
        {
            get { return this.view; }
        }

        /// <summary>
        /// The hit rectangle of the Node
        /// </summary>
        [Browsable(false)]
        public RectangleF HitRectangle;

        /// <summary>
        /// The list of NodeGraphConnectors owned by this Node
        /// </summary>
        [Category("Node Properties")]
        public List<NodeGraphConnector> Connectors
        {
            get { return this.connectors; }
        }

        /// <summary>
        /// The displayed comment of the node
        /// </summary>
        [Category("Node Properties")]
        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the name of the node: can be overriden to match custom names.
        /// </summary>
        protected virtual string GetName()
        {
            return $"Node {this.id.ToString("000")} Name: {this.name}";
        }

        /// <summary>
        /// Gets the connector index, given the connector object reference
        /// </summary>
        /// <param name="connector">the connector reference</param>
        /// <returns>the connector index</returns>
        public virtual int GetConnectorIndex(NodeGraphConnector connector)
        {
            return this.connectors.IndexOf(connector);
        }

        /// <summary>
        /// Gets the Connector Count ether for Inputs or Outputs
        /// </summary>
        public virtual int GetConnectorCount(ConnectorType type)
        {
            return this.connectors.FindAll(x => x.Type == type).Count;
        }

        /// <summary>
        /// Gets the Connector Count ether for Inputs or Outputs
        /// </summary>
        public virtual int GetConnectorLinkedCount(ConnectorType type)
        {
            return this.connectors.FindAll(x => x.Type == type && x.CanProcess()).Count;
        }

        /// <summary>
        /// Gets the Maximal Connector Count
        /// </summary>
        public virtual int GetConnectorCount_Max()
        {
            return Math.Max(GetConnectorCount(ConnectorType.Input), GetConnectorCount(ConnectorType.Output));
        }

        /// <summary>
        /// Intercepts a mouse hit and returns a NodeGraphConnector if hit by the mouse, null otherwise
        /// </summary>
        /// <param name="p_ScreenPosition"></param>
        /// <returns></returns>
        public NodeGraphConnector GetConnectorMouseHit(Point p_ScreenPosition)
        {
            Rectangle v_HitRectangle = new Rectangle(p_ScreenPosition, Size.Empty);

            foreach (NodeGraphConnector i_Connector in this.connectors)
            {
                if (v_HitRectangle.IntersectsWith(i_Connector.GetAreaHit()))
                {
                    return i_Connector;
                }
            }
            return null;
        }

        /// <summary>
        /// Draws the node
        /// </summary>
        /// <param name="e"></param>
        public virtual void Draw(PaintEventArgs e)
        {
            GL.Color4(this.view.ColorNodeFill);

            GL.Begin(PrimitiveType.Quads);

            GL.Vertex3(this.position.X, position.Y, 0);
            GL.Vertex3(this.position.X + property.X, position.Y, 0);
            GL.Vertex3(this.position.X + property.X, position.Y + property.Y, 0);
            GL.Vertex3(this.position.X, position.Y + property.Y, 0);

            GL.End();

            GL.Color4(this.view.ColorNodeOutline);

            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(this.position.X, position.Y, 0);
            GL.Vertex3(this.position.X + property.X, position.Y, 0);
            GL.Vertex3(this.position.X + property.X, position.Y + property.Y, 0);
            GL.Vertex3(this.position.X, position.Y + property.Y, 0);

            GL.End();

            //Vector2 CtrlPos = view.Panel.ViewToControl(new Vector2(x, y));

            //float ScaledX = CtrlPos.X;
            //float ScaledY = CtrlPos.Y;

            //Rectangle ViewRectangle = new Rectangle((int)CtrlPos.X,
            //                                        (int)CtrlPos.Y,
            //                                        (int)(this.HitRectangle.Width * view.ViewZoomCurrent),
            //                                        (int)(this.HitRectangle.Height * view.ViewZoomCurrent)
            //                                        );
            // NODE SHADOW
            //if (this.ParentView.Panel.EnableShadow)
            //{
            //e.Graphics.DrawImage(NodeGraphResources.NodeShadow, ParentView.Panel.ViewToControl(new Rectangle(this.x - (int)(0.1f * this.Width) + 4,
            //                                                                                                       this.y - (int)(0.1f * this.Height) + 4,
            //                                                                                                       this.Width + (int)(0.2f * this.Width) - 4,
            //                                                                                                       this.Height + (int)(0.2f * this.Height) - 4)));
            //}
            //// NODE
            //if (!this.Selected)
            //{
            //    if (nodeFill != null)
            //        e.Graphics.FillRectangle(nodeFill, ViewRectangle);
            //    else
            //        e.Graphics.FillRectangle(view.Color_node_fill, ViewRectangle);

            //    e.Graphics.FillRectangle(view.NodeHeaderFill, new Rectangle(ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, (int)(view.NodeHeaderSize * view.ViewZoomCurrent)));
            //    e.Graphics.DrawRectangle(view.NodeOutline, ViewRectangle);
            //}
            //else
            //{
            //    e.Graphics.FillRectangle(view.color, ViewRectangle);
            //    e.Graphics.FillRectangle(view.NodeHeaderFill, new Rectangle(ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, (int)(view.NodeHeaderSize * view.ViewZoomCurrent)));
            //    e.Graphics.DrawRectangle(view.Panel.NodeOutlineSelected, ViewRectangle);
            //}

            // VALID/INVALID NODE
            // Removed

            // IF SUFFICENT ZOOM LEVEL = DRAW TEXT
            //if (view.ViewZoomCurrent > view.Panel.NodeTitleZoomThreshold)
            //{
            //    // DrawText
            //    //e.Graphics.DrawString(this.Name, view.Panel.NodeScaledTitleFont, view.Panel.NodeTextShadow, new Point(ScaledX + (int)(2 * view.ViewZoomCurrent) + 1, ScaledY + (int)(2 * view.ViewZoomCurrent) + 1));
            //    //e.Graphics.DrawString(this.Name, view.Panel.NodeScaledTitleFont, view.Panel.NodeText, new Point(ScaledX + (int)(2 * view.ViewZoomCurrent), ScaledY + (int)(2 * view.ViewZoomCurrent)));
            //}

            //InputConnectors
            for (int i_ConnectorIndex = 0; i_ConnectorIndex < this.connectors.Count; i_ConnectorIndex++)
            {
                this.connectors[i_ConnectorIndex].Draw(e, i_ConnectorIndex);
            }

            // Comment
            if (!string.IsNullOrEmpty(this.comment))
            {
                //e.Graphics.DrawString(this.comment, view.Panel.NodeScaledTitleFont, view.Panel.NodeText, new Point(ScaledX, ScaledY - (int)(16 * view.ViewZoomCurrent)));
            }

            // Post-draw event
            this.PostDraw?.Invoke(this, e);
        }

        /// <summary>
        /// VIRTUAL: Returns if logic validity has been approved (all input connector linked by default) : can be overriden to get custom approval
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            if (this.GetConnectorCount(ConnectorType.Input) > 0)
                return this.GetConnectorLinkedCount(ConnectorType.Input) > 0 ? true : false;
            else
                return true;
        }

        /// <summary>
        /// Process with given Index ??
        /// </summary>
        /// <returns></returns>
        public virtual object Process()
        {
            return null;
        }

        public override string ToString()
        {
            return GetName();
        }

        #endregion
    }
}
