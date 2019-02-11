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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;

namespace NodeGraph.TK
{
    /// <summary>
    /// The type of connector associated to the Node
    /// </summary>
    public enum ConnectorType
    {
        Input,
        Output
    }

    /// <summary>
    /// Data of the Connector
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
    /// Represents a connector on a node
    /// </summary>
    public class NodeGraphConnector
    {
        #region - Private Variables -

        private string name;

        private NodeGraphNode parentNode;
        private ConnectorType connectorType;
        private ConnectorData connectorData;
        private NodeGraphView view;

        private int connectorIndex;

        private SolidBrush brush;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphConnector, given a name, a parent container, type and index
        /// </summary>
        /// <param name="newName">The display name of the connector</param>
        /// <param name="newParent">Reference to the parent NodeGraphNode</param>
        /// <param name="newConnectorType">Type of the connector (input/output)</param>
        /// <param name="newConnectorIndex">Connector Index</param>
        public NodeGraphConnector(string newName, NodeGraphNode newParent, ConnectorType newConnectorType, int newConnectorIndex)
        {
            // Try Setting Connector Data Correct
            ConnectorData newConnectorData = ConnectorData.None;

            bool success = Enum.TryParse(newName, out newConnectorData);

            if (!success)
                Console.WriteLine($"## ERROR: Casting {newName} to Enum failed");

            this.name           = newName;
            this.parentNode     = newParent;
            this.view           = newParent.ParentView;
            this.connectorType  = newConnectorType;
            this.connectorData  = newConnectorData;
            this.connectorIndex = newConnectorIndex;

            this.brush = view.Panel.ConnectorFill;
        }

        /// <summary>
        /// Creates a new NodeGraphConnector, given a name, a parent container, type and index
        /// </summary>
        /// <param name="newName">The display name of the connector</param>
        /// <param name="newParent">Reference to the parent NodeGraphNode</param>
        /// <param name="newConnectorType">Type of the connector (input/output)</param>
        /// <param name="newConnectorIndex">Connector Index</param>
        public NodeGraphConnector(string newName, NodeGraphNode newParent, ConnectorType newConnectorType, ConnectorData newConnectorData, int newConnectorIndex)
        {
            this.name           = newName;
            this.parentNode     = newParent;
            this.view           = newParent.ParentView;
            this.connectorType  = newConnectorType;
            this.connectorData  = newConnectorData;
            this.connectorIndex = newConnectorIndex;

            this.brush = view.Panel.ConnectorFill;
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// Index of the connector
        /// </summary>
        public int Index
        {
            get { return this.connectorIndex; }
        }

        /// <summary>
        /// Name of the connector that will be displayed
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets / Sets Connectors Color
        /// </summary>
        public Color Color
        {
            get { return brush.Color; }
            set { brush.Color = value; }
        }

        /// <summary>
        /// The parent node that contains the connector
        /// </summary>
        public NodeGraphNode Parent
        {
            get { return this.parentNode; }
        }

        /// <summary>
        /// Type of the connector (input/output)
        /// </summary>
        public ConnectorType Type
        {
            get { return this.connectorType; }
        }

        /// <summary>
        /// DataType of the connector
        /// </summary>
        public ConnectorData Data
        {
            get { return this.connectorData; }
        }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the visible area of the connector
        /// </summary>
        /// <returns>a rectangle determining the visible area of the connector</returns>
        public RectangleF GetArea()
        {
            Vector2 position;

            RectangleF rectangle;

            if (connectorType == ConnectorType.Input)
            {

                position = view.Panel.ViewToControl(new Vector2(parentNode.X, (parentNode.Y + view.Panel.NodeHeaderSize + 6 + (connectorIndex * 16))));
                rectangle = new RectangleF(position.X, position.Y, (int)(12 * view.ViewZoomCurrent), (int)(8 * view.ViewZoomCurrent));

            }
            else
            {

                position = view.Panel.ViewToControl(new Vector2(parentNode.X + (parentNode.HitRectangle.Width - 12), (parentNode.Y + view.Panel.NodeHeaderSize + 6 + (connectorIndex * 16))));
                rectangle = new RectangleF(position.X, position.Y, (int)(12 * view.ViewZoomCurrent), (int)(8 * view.ViewZoomCurrent));
            }

            return rectangle;
        }

        /// <summary>
        /// Returns the Click Area of the connector
        /// </summary>
        /// <returns>a rectangle determining the Click area of the connector</returns>
        public Rectangle GetAreaHit()
        {
            Point v_Position;
            Rectangle v_ConnectorRectangle;
            int Bleed = view.Panel.ConnectorHitZoneBleed;

            if (connectorType == ConnectorType.Input)
            {
                v_Position = view.Panel.ViewToControl(new Point(parentNode.X, (parentNode.Y + view.Panel.NodeHeaderSize + 6 + (connectorIndex * 16))));

                v_ConnectorRectangle = new Rectangle(v_Position.X - Bleed, v_Position.Y - Bleed,
                                                       (int)(12 * view.ViewZoomCurrent) + (2 * Bleed),
                                                       (int)(8 * view.ViewZoomCurrent) + (2 * Bleed));

            }
            else
            {

                v_Position = view.Panel.ViewToControl(new Point(parentNode.X + (parentNode.HitRectangle.Width - 12), (parentNode.Y + view.Panel.NodeHeaderSize + 6 + (connectorIndex * 16))));
                v_ConnectorRectangle = new Rectangle(v_Position.X - Bleed, v_Position.Y - Bleed,
                                                       (int)(12 * view.ViewZoomCurrent) + (2 * Bleed),
                                                       (int)(8 * view.ViewZoomCurrent) + (2 * Bleed));
            }

            return v_ConnectorRectangle;
        }

        /// <summary>
        /// Returns the position of the Displayed Text
        /// </summary>
        /// <param name="e">PaintEventArgs used for measure</param>
        /// <returns>Position of the text</returns>
        public Point GetTextPosition(PaintEventArgs e)
        {
            Point v_TextPosition;

            if (connectorType == ConnectorType.Input)
            {
                v_TextPosition = view.Panel.ViewToControl(new Point(parentNode.X + 16, parentNode.Y + view.Panel.NodeHeaderSize + 4 + (connectorIndex * 16)));
            }
            else
            {
                SizeF measure = e.Graphics.MeasureString(this.Name, view.Panel.NodeScaledConnectorFont);
                v_TextPosition = view.Panel.ViewToControl(new Point(parentNode.X + (parentNode.HitRectangle.Width), parentNode.Y + view.Panel.NodeHeaderSize + 4 + (connectorIndex * 16)));
                v_TextPosition.X = v_TextPosition.X - (int)(16.0f * view.ViewZoomCurrent) - (int)measure.Width;
            }

            return v_TextPosition;
        }

        /// <summary>
        /// Draws the connector
        /// </summary>
        /// <param name="e"></param>
        /// <param name="ConnectorIndex"></param>
        public void Draw(PaintEventArgs e, int ConnectorIndex)
        {
            Rectangle v_ConnectorRectangle = this.GetArea();

            e.Graphics.FillRectangle(view.Panel.ConnectorFill, v_ConnectorRectangle);
            e.Graphics.DrawRectangle(view.Panel.ConnectorOutline, v_ConnectorRectangle);

            // If under zoom requirements for connector text...
            if (view.ViewZoomCurrent > view.Panel.NodeConnectorTextZoomTreshold)
            {
                Point v_TextPosition = GetTextPosition(e);

                e.Graphics.DrawString(this.Name, view.Panel.NodeScaledConnectorFont, view.Panel.ConnectorText, (float)v_TextPosition.X, (float)v_TextPosition.Y);
            }
        }

        /// <summary>
        /// Virtual: Sample Process for a node, must be overriden in child class, processes only unlinked inputs.
        /// </summary>
        /// <returns>NodeGraphData depending of the validity</returns>
        public virtual object Process()
        {
            if (this.connectorType == ConnectorType.Output)
            {
                // Nodes Process Method can figure out wich connector requests data
                return parentNode.Process();
            }
            if (this.connectorType == ConnectorType.Input)
            {
                NodeGraphConnector linkedOutputConnector = view.Panel.GetLink(this);

                if (linkedOutputConnector != null)
                    return linkedOutputConnector.Process();
            }

            return this.parentNode.Name + " Connector: " + this.Name + " not linked";
        }

        /// <summary>
        /// Returns true if Connector can be processed
        /// </summary>
        /// <returns></returns>
        public bool CanProcess()
        {
            // For Input Only
            if (this.connectorType == ConnectorType.Input)
            {
                NodeGraphConnector v_Connector = view.Panel.GetLink(this);

                // No link is connected
                if (v_Connector == null)
                    return false;        
            }

            // Output or Connected Inputs are fine
            return true;
        }
        #endregion
    }

}
