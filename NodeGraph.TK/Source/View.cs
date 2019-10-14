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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace NodeGraph.TK
{
    #region ENUM LinkVisualStyle
    /// <summary>
    /// Visual mode of viewing links
    /// </summary>
    public enum LinkVisualStyle
    {
        Direct,
        Rectangle,
        Curve,
        Dummy
    }
    #endregion

    /// <summary>
    /// Encapsulates a Graph View
    /// </summary>
    public class View
    {
        #region - Private Variables -

        private Vector4 viewProp;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphView in a NodeGraphPanel
        /// </summary>
        public View(Graph graph)
        {
            this.Graph = graph;

            this.ViewX           = 100;
            this.ViewY           = 100;
            this.ViewZoom        = 1.0f;
            this.ViewZoomCurrent = 1.0f;

            this.Node_Title_Text_TH     = 0.5f;
            this.Node_Connector_Text_TH = 0.8f;
            this.SizeNodeHeader         = 24;

            this.LinkVisualStyle = LinkVisualStyle.Curve;

            this.Connector_HZB = 2;

            this.ColorBackground                  = Util.ColorToVector4(Color.FromArgb(58, 58, 58));
            this.ColorNodeText                    = Util.ColorToVector4(Color.FromArgb(255, 255, 255, 255));
            this.ColorNodeTextShadow              = Util.ColorToVector4(Color.FromArgb(128, 0, 0, 0));
            this.ColorNodeFill                    = Util.ColorToVector4(Color.FromArgb(255, 128, 128, 128));
            this.ColorNodeFillHovered             = Util.ColorToVector4(Color.FromArgb(255, 185, 148, 120));
            this.ColorNodeFillSelected            = Util.ColorToVector4(Color.FromArgb(255, 160, 128, 100));
            this.ColorNodeHeader                  = Util.ColorToVector4(Color.FromArgb(128, 0, 0, 0));
            this.ColorNodeOutline                 = Util.ColorToVector4(Color.FromArgb(255, 180, 180, 180));
            this.ColorNodeOutlineHovered          = Util.ColorToVector4(Color.FromArgb(255, 222, 180, 128));
            this.ColorNodeOutlineSelected         = Util.ColorToVector4(Color.FromArgb(255, 192, 160, 128));
            this.ColorConnectorText               = Util.ColorToVector4(Color.FromArgb(255, 64, 64, 64));
            this.ColorConnectorFill               = Util.ColorToVector4(Color.FromArgb(255, 0, 0, 0));
            this.ColorConnectorFillSelected       = Util.ColorToVector4(Color.FromArgb(255, 32, 32, 32));
            this.ColorConnectorOutline            = Util.ColorToVector4(Color.FromArgb(255, 32, 32, 32));
            this.ColorConnectorOutlineSelected    = Util.ColorToVector4(Color.FromArgb(255, 64, 64, 64));
            this.ColorSelectionFill               = Util.ColorToVector4(Color.FromArgb(64, 128, 90, 30));
            this.ColorSelectionOutline            = Util.ColorToVector4(Color.FromArgb(192, 255, 180, 60));
            this.ColorLink                        = Util.ColorToVector4(Color.YellowGreen);
            this.ColorLinkSelected                = Util.ColorToVector4(Color.GreenYellow);
            this.ColorLinkEdit                    = Util.ColorToVector4(Color.FromArgb(255, 64, 255, 0));
            this.ColorNodeSignalValid             = Util.ColorToVector4(Color.FromArgb(255, 0, 255, 0));
            this.ColorNodeSignalInvalid           = Util.ColorToVector4(Color.FromArgb(255, 255, 0, 0));

            this.Font_Debug                 = new Font("Tahoma", 8.0f);
            this.Font_Node_Title            = new Font("Tahoma", 8.0f);
            this.Font_Node_Connector        = new Font("Tahoma", 7.0f);
            this.Font_Node_Title_Scaled     = new Font(Font_Node_Title.Name, Font_Node_Title.Size);
            this.Font_Node_Connector_Scaled = new Font(Font_Node_Connector.Name, Font_Node_Connector.Size);

            // Grid
            this.Grid_Enable  = true;
            this.Grid_Padding = 64;
            this.Grid_Alpha   = 0.125f;

            if ((this.ColorBackground.X + this.ColorBackground.Y + this.ColorBackground.Z) / 3.0f < 0.5f)
                this.Grid_Color = new Vector4(1.0f, 1.0f, 1.0f, this.Grid_Alpha);
            else
                this.Grid_Color = new Vector4(0.0f, 0.0f, 0.0f, this.Grid_Alpha);
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// The Graph this View is displaying
        /// </summary>
        [Browsable(false)]
        public Graph Graph { get; set; }

        #region - - View - -

        /// <summary>
        /// The orthographic X coordinate of the current View
        /// </summary>
        [Category("NodeGraph View")]
        public float ViewX { get => this.viewProp.X; set => this.viewProp.X = value; }

        /// <summary>
        /// The orthographic Y coordinate of the current View
        /// </summary>
        [Category("NodeGraph View")]
        public float ViewY { get => this.viewProp.Y; set => this.viewProp.Y = value; }

        /// <summary>
        /// The zoom factor of the current view
        /// </summary>
        [Category("NodeGraph View")]
        public float ViewZoom { get => this.viewProp.Z; set => this.viewProp.Z = value; }

        /// <summary>
        /// Current zoom Factor, used on smooth behavior
        /// </summary>
        [Category("NodeGraph View")]
        public float ViewZoomCurrent { get => this.viewProp.W; set => this.viewProp.W = value; }

        #endregion

        #region - - Colors - -

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeText { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeTextShadow { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeFill { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeFillHovered { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeFillSelected { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeOutline { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeOutlineHovered { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeOutlineSelected { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeHeader { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorText { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorFill { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorOutline { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorFillSelected { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorOutlineSelected { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorSelectionFill { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorSelectionOutline { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorLink { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorLinkSelected { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorLinkEdit { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeSignalValid { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeSignalInvalid { get; set; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorBackground { get; set; }

        #endregion

        #region - - Fonts - -

        [Category("NodeGraph View Fonts")]
        public Font Font_Debug { get; set; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Title { get; set; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Connector { get; set; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Title_Scaled { get; set; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Connector_Scaled { get; set; }

        #endregion

        #region - - Grid - -

        [Category("NodeGraph View Grid")]
        public bool Grid_Enable { get; set; }

        [Category("NodeGraph View Grid")]
        public float Grid_Padding { get; set; }

        [Category("NodeGraph View Grid")]
        public float Grid_Alpha { get; set; }

        [Category("NodeGraph View Grid")]
        public Vector4 Grid_Color { get; set; }

        [Category("NodeGraph View Misc")]
        public LinkVisualStyle LinkVisualStyle { get; set; }

        [Category("NodeGraph View Misc")]
        public float Node_Connector_Text_TH { get; set; }

        [Category("NodeGraph View Misc")]
        public float Node_Title_Text_TH { get; set; }

        [Category("NodeGraph View Misc")]
        public float Connector_HZB { get; set; }

        [Category("NodeGraph View Misc")]
        public float Link_Hardness { get; set; }

        [Category("NodeGraph View Misc")]
        public float SizeNodeHeader { get; set; }


        #endregion

        #endregion

        #region - Methods -

        ///// <summary>
        ///// Returns the Node Index of the NodeGraphNode in this graphs's collection
        ///// </summary>
        //public int GetNodeIndex(NodeGraphNode node)
        //{
        //    return this.graph.GetNodeIndex(node);
        //}

        ///// <summary>
        ///// Returns the Node Index of the NodeGraphNode in this graphs's current selection
        ///// </summary>
        //public int GetNodeIndexSelected(NodeGraphNode node)
        //{
        //    return this.graph.GetNodeIndexSelected(node);
        //}

        #endregion
    }
}
