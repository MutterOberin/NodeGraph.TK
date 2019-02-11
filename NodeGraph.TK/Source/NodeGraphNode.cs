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
using NodeGraph.TK.Source;

namespace NodeGraph.TK
{
    /// <summary>
    /// Represents a base node for use in a NodeGraphView
    /// </summary>
    public class NodeGraphNode
    {
        #region - Private Variables -

        protected SolidBrush nodeFill;

        protected Point guiLoc;

        protected int x;
        protected int y;
        protected int height;
        protected int width;
        protected bool selectable;
        protected string comment;

        //protected SI item;

        protected String name;
        protected List<NodeGraphConnector> connectors;
        protected NodeGraphView view;

        public event PaintEventHandler onPostDraw;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphNode into the NodeGraphView, given coordinates and ability to be selected
        /// </summary>
        /// <param name="p_X"></param>
        /// <param name="p_Y"></param>
        /// <param name="view"></param>
        /// <param name="selectable"></param>
        public NodeGraphNode(int p_X, int p_Y, NodeGraphView view, bool selectable)
        {
            this.x           = p_X;
            this.y           = p_Y;
            this.view        = view;
            this.width       = 140;
            this.height      = 64;
            this.name        = "Test Void Node";
            this.selectable  = selectable;
            this.Highlighted = false;
            this.comment     = "";
            this.guiLoc      = new Point(-1, -1);

            //Point p = new Point(view.ParentPanel.Location.X + view.ParentPanel.Width / 2,
            //                    view.ParentPanel.Location.Y + view.ParentPanel.Height / 2);

            //this.guiLoc = view.ParentPanel.PointToScreen(p);
            

            UpdateHitRectangle();

            this.connectors = new List<NodeGraphConnector>();
        }

        #endregion

        #region - Properties -

        [Category("Node GUI")]
        public Point GUI_Location
        {
            get { return guiLoc; }
            set { guiLoc = value; }
        }

        [Category("Node GUI")]
        public int GUI_Location_X
        {
            get { return guiLoc.X; }
            set { guiLoc.X = value; }
        }

        [Category("Node GUI")]
        public int GUI_Location_Y
        {
            get { return guiLoc.Y; }
            set { guiLoc.Y = value; }
        }

        /// <summary>
        /// Gets / Sets Node Fill Color
        /// </summary>
        [Category("Node Properties")]
        public Color NodeFillColor
        {
            get { return nodeFill.Color; }
            set { nodeFill = new SolidBrush(value); }
        }

        ///// <summary>
        ///// Gets Sets the StreamViz SceneItem
        ///// </summary>
        //[Category("Node Item")]
        //public virtual SI Item
        //{
        //    get { return item; }
        //    set { item = value; }
        //}

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
        /// X Position (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public int X
        {
            get { return x; }
            set { x = value; UpdateHitRectangle(); } 
        }

        /// <summary>
        /// Y Position (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public int Y
        {
            get { return y; }
            set { y = value; UpdateHitRectangle(); }
        }

        /// <summary>
        /// Width (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public int Width
        {
            get { return this.width; }
            set { this.width = value; this.UpdateHitRectangle(); }
        }

        /// <summary>
        /// Height (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public int Height
        {
            get { return this.height; }
            set { this.height = value; this.UpdateHitRectangle(); }
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
        /// Whether the node is highlighted
        /// </summary>
        [Browsable(false)]
        public bool Highlighted;

        /// <summary>
        /// The Hit (Mouse Click) rectangle of the Node
        /// </summary>
        [Browsable(false)]
        public Rectangle HitRectangle;

        /// <summary>
        /// The list of NodeGraphConnectors owned by this Node
        /// </summary>
        [Category("Node Properties")]
        public List<NodeGraphConnector> Connectors
        {
            get { return this.connectors; }
        }

        /// <summary>
        /// The displayed Commentary of the node
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
        /// <returns></returns>
        protected virtual string GetName()
        {
            //if (item != null)
            //    return this.item.ToString();

            return "No Item";
        }

        /// <summary>
        /// Gets the connector index, given the connector object reference
        /// </summary>
        /// <param name="connector">the connector reference</param>
        /// <returns>the connector index</returns>
        public virtual int GetConnectorIndex(NodeGraphConnector connector)
        {
            for (int i = 0; i < this.connectors.Count; i++)
            {
                if (this.connectors[i] == connector) 
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Gets the Connector Count ether for Inputs or Outputs
        /// </summary>
        public virtual int GetConnectorCount(ConnectorType type)
        {
            int count = 0;

            for (int i = 0; i < this.connectors.Count; i++)
            {
                if (this.connectors[i].Type == type)
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Gets the Connector Count ether for Inputs or Outputs
        /// </summary>
        public virtual int GetConnectorLinkedCount(ConnectorType type)
        {
            int count = 0;

            for (int i = 0; i < this.connectors.Count; i++)
            {
                if (this.connectors[i].Type == type && this.connectors[i].CanProcess())
                    count++;
            }

            return count;
        }

        /// <summary>
        /// Gets the Maximal Connector Count
        /// </summary>
        public virtual int GetConnectorCount_Max()
        {
            return Math.Max(GetConnectorCount(ConnectorType.Input), GetConnectorCount(ConnectorType.Output));
        }

        /// <summary>
        /// Updates HitRectangle (when moved)
        /// </summary>
        public void UpdateHitRectangle()
        {
            this.HitRectangle = new Rectangle(x, y, Width, Height);
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
            Point CtrlPos = view.Panel.ViewToControl(new Point(x, y));

            int ScaledX = CtrlPos.X;
            int ScaledY = CtrlPos.Y;

            Rectangle ViewRectangle = new Rectangle(CtrlPos.X,
                                                    CtrlPos.Y,
                                                    (int)(this.HitRectangle.Width * view.ViewZoomCurrent),
                                                    (int)(this.HitRectangle.Height * view.ViewZoomCurrent)
                                                    );
            // NODE SHADOW
            if (this.ParentView.Panel.DrawShadow)
            {
                e.Graphics.DrawImage(NodeGraphResources.NodeShadow, ParentView.Panel.ViewToControl(new Rectangle(this.x - (int)(0.1f * this.Width) + 4,
                                                                                                                       this.y - (int)(0.1f * this.Height) + 4,
                                                                                                                       this.Width + (int)(0.2f * this.Width) - 4,
                                                                                                                       this.Height + (int)(0.2f * this.Height) - 4)
                                                                                                         ));
            }
            // NODE
            if (!this.Highlighted)
            {
                if (nodeFill != null)
                    e.Graphics.FillRectangle(nodeFill, ViewRectangle);
                else
                    e.Graphics.FillRectangle(view.Panel.NodeFill, ViewRectangle);

                e.Graphics.FillRectangle(view.Panel.NodeHeaderFill, new Rectangle(ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, (int)(view.Panel.NodeHeaderSize * view.ViewZoomCurrent)));
                e.Graphics.DrawRectangle(view.Panel.NodeOutline, ViewRectangle);
            }
            else
            {
                e.Graphics.FillRectangle(view.Panel.NodeFillSelected, ViewRectangle);
                e.Graphics.FillRectangle(view.Panel.NodeHeaderFill, new Rectangle(ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, (int)(view.Panel.NodeHeaderSize * view.ViewZoomCurrent)));
                e.Graphics.DrawRectangle(view.Panel.NodeOutlineSelected, ViewRectangle);
            }

            // VALID/INVALID NODE

            Point v_CtrlSignalPosition = view.Panel.ViewToControl(new Point(x + Width - 20, y + 4));
            Rectangle v_SignalRectangle = new Rectangle(v_CtrlSignalPosition.X, v_CtrlSignalPosition.Y, (int)(16 * view.ViewZoomCurrent), (int)(16 * view.ViewZoomCurrent));

            // IF SUFFICENT ZOOM LEVEL = DRAW TEXT
            if (view.ViewZoomCurrent > view.Panel.NodeTitleZoomThreshold)
            {
                // DrawText
                e.Graphics.DrawString(this.Name, view.Panel.NodeScaledTitleFont, view.Panel.NodeTextShadow, new Point(ScaledX + (int)(2 * view.ViewZoomCurrent) + 1, ScaledY + (int)(2 * view.ViewZoomCurrent) + 1));
                e.Graphics.DrawString(this.Name, view.Panel.NodeScaledTitleFont, view.Panel.NodeText, new Point(ScaledX + (int)(2 * view.ViewZoomCurrent), ScaledY + (int)(2 * view.ViewZoomCurrent)));
            }

            //InputConnectors
            for (int i_ConnectorIndex = 0; i_ConnectorIndex < this.connectors.Count; i_ConnectorIndex++)
            {
                this.connectors[i_ConnectorIndex].Draw(e, i_ConnectorIndex);
            }

            // Comment
            if (!string.IsNullOrEmpty(this.comment))
            {
                e.Graphics.DrawString(this.comment, view.Panel.NodeScaledTitleFont, view.Panel.NodeText, new Point(ScaledX, ScaledY - (int)(16 * view.ViewZoomCurrent)));
            }

            // Post-draw event
            this.onPostDraw?.Invoke(this, e);
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

        /// <summary>
        /// Dispose the coresponding item
        /// </summary>
        public virtual void DisposeItem()
        {
            //if (item != null)
            //    item.Dispose();
        }

        /// <summary>
        /// Serialize to XML Node
        /// </summary>
        /// <param name="node"></param>
        public virtual void Serialize(XmlElement node)
        {
            //Util_XML.Append(node.OwnerDocument, node, "GUI_Location", this.guiLoc);
        }

        /// <summary>
        /// Deserialize from XML Node
        /// </summary>
        /// <param name="node"></param>
        public virtual void Deserialize(XmlNode node)
        {
            //foreach (XmlNode childNode in node.ChildNodes)
            //{
            //    if (childNode.Name.Equals("GUI_Location"))
            //        this.guiLoc = Util_XML.Read_Point(childNode);
            //}
        }

        public override string ToString()
        {
            return "Node " + GetName();
        }

        #endregion
    }
}
