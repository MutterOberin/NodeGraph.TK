﻿/*

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
    /// Encapsulates a node view
    /// </summary>
    public class NodeGraphView
    {
        #region - Private Variables -

        private Vector4 viewProp;

        private NodeGraphGraph graph;

        #region - - Misc - -

        private float node_header_size;
        private float link_hardness;

        private float connector_hzb;

        private float node_connector_text_th;
        private float node_title_text_th;

        private LinkVisualStyle linkVisualStyle;

        #endregion

        #region - - Colors - -

        // Colors
        private Vector4 color_background;
        private Vector4 color_node_header;
        private Vector4 color_node_text;
        private Vector4 color_node_text_shadow;
        private Vector4 color_node_fill;
        private Vector4 color_node_fill_selected;
        private Vector4 color_node_outline;
        private Vector4 color_node_outline_selected;
        private Vector4 color_connector_text;
        private Vector4 color_connector_fill;
        private Vector4 color_connector_outline;
        private Vector4 color_connector_fill_selected;
        private Vector4 color_connector_outline_selected;
        private Vector4 color_selection_fill;
        private Vector4 color_selection_outline;
        private Vector4 color_link;
        private Vector4 color_link_edit;
        private Vector4 color_node_signal_valid;
        private Vector4 color_node_signal_invalid;

        #endregion

        #region - - Fonts - -

        // Fonts
        private Font font_debug;

        private Font font_node_title;
        private Font font_node_connector;

        private Font font_node_title_scaled;
        private Font font_node_connector_scaled;

        #endregion

        #region - - Grid - -

        private bool grid_enable;
        private float grid_padding;
        private float grid_alpha;
        private Vector4 grid_color;

        #endregion

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphView in a NodeGraphPanel
        /// </summary>
        public NodeGraphView(NodeGraphGraph graph)
        {
            this.graph = graph;

            this.ViewX           = 100;
            this.ViewY           = 100;
            this.ViewZoom        = 1.0f;
            this.ViewZoomCurrent = 1.0f;

            this.node_title_text_th     = 0.5f;
            this.node_connector_text_th = 0.8f;
            this.node_header_size       = 24;

            this.linkVisualStyle = LinkVisualStyle.Curve;

            this.connector_hzb = 2;

            this.color_background                 = NodeGraphUtil.ColorToVector4(Color.FromArgb(58, 58, 58));
            this.color_node_text                  = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 255, 255, 255));
            this.color_node_text_shadow           = NodeGraphUtil.ColorToVector4(Color.FromArgb(128, 0, 0, 0));
            this.color_node_fill                  = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 128, 128, 128));
            this.color_node_fill_selected         = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 160, 128, 100));
            this.color_node_header                = NodeGraphUtil.ColorToVector4(Color.FromArgb(128, 0, 0, 0));
            this.color_node_outline               = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 180, 180, 180));
            this.color_node_outline_selected      = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 192, 160, 128));
            this.color_connector_text             = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 64, 64, 64));
            this.color_connector_fill             = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 0, 0, 0));
            this.color_connector_fill_selected    = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 32, 32, 32));
            this.color_connector_outline          = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 32, 32, 32));
            this.color_connector_outline_selected = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 64, 64, 64));
            this.color_selection_fill             = NodeGraphUtil.ColorToVector4(Color.FromArgb(64, 128, 90, 30));
            this.color_selection_outline          = NodeGraphUtil.ColorToVector4(Color.FromArgb(192, 255, 180, 60));
            this.color_link                       = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 180, 180, 180));
            this.color_link_edit                  = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 64, 255, 0));
            this.color_node_signal_valid          = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 0, 255, 0));
            this.color_node_signal_invalid        = NodeGraphUtil.ColorToVector4(Color.FromArgb(255, 255, 0, 0));

            this.font_debug                 = new Font("Tahoma", 8.0f);
            this.font_node_title            = new Font("Tahoma", 8.0f);
            this.font_node_connector        = new Font("Tahoma", 7.0f);
            this.font_node_title_scaled     = new Font(font_node_title.Name, font_node_title.Size);
            this.font_node_connector_scaled = new Font(font_node_connector.Name, font_node_connector.Size);

            // Grid
            this.grid_enable  = true;
            this.grid_padding = 64;
            this.grid_alpha   = 0.125f;

            if ((this.color_background.X + this.color_background.Y + this.color_background.Z) / 3.0f < 0.5f)
                this.grid_color = new Vector4(1.0f, 1.0f, 1.0f, this.grid_alpha);
            else
                this.grid_color = new Vector4(0.0f, 0.0f, 0.0f, this.grid_alpha);
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// The graph this view is displaying
        /// </summary>
        [Browsable(false)]
        public NodeGraphGraph Graph { get => this.graph; set => this.graph = value; }

        #region - - View - -

        /// <summary>
        /// The orthographic X coordinate of the current View
        /// </summary>
        [Category("NodeGraph View")]
        public int ViewX { get => (int)this.viewProp.X; set => this.viewProp.X = value; }

        /// <summary>
        /// The orthographic Y coordinate of the current View
        /// </summary>
        [Category("NodeGraph View")]
        public int ViewY { get => (int)this.viewProp.Y; set => this.viewProp.Y = value; }

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
        public Vector4 ColorNodeText { get => this.color_node_text; set => this.color_node_text = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeTextShadow { get => this.color_node_text_shadow; set => this.color_node_text_shadow = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeFill { get => this.color_node_fill; set => this.color_node_fill = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeFillSelected { get => this.color_node_fill_selected; set => this.color_node_fill_selected = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeOutline { get => this.color_node_outline; set => this.color_node_outline = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeOutlineSelected { get => this.color_node_outline_selected; set => this.color_node_outline_selected = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorText { get => this.color_connector_text; set => this.color_connector_text = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorFill { get => this.color_connector_fill; set => this.color_connector_fill = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorOutline { get => this.color_connector_outline; set => this.color_connector_outline = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorFillSelected { get => this.color_connector_fill_selected; set => this.color_connector_fill_selected = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorConnectorOutline_elected { get => this.color_connector_outline_selected; set => this.color_connector_outline_selected = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorSelectionFill { get => this.color_selection_fill; set => this.color_selection_fill = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorSelectionOutline { get => this.color_selection_outline; set => this.color_selection_outline = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorLink { get => this.color_link; set => this.color_link = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorLinkEdit { get => this.color_link_edit; set => this.color_link_edit = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeSignalValid { get => this.color_node_signal_valid; set => this.color_node_signal_valid = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorNodeSignalInvalid { get => this.color_node_signal_invalid; set => this.color_node_signal_invalid = value; }

        [Category("NodeGraph View Colors")]
        public Vector4 ColorBackground { get => this.color_background; set => this.color_background = value; }

        #endregion

        #region - - Fonts - -

        [Category("NodeGraph View Fonts")]
        public Font Font_Debug { get => this.font_debug; set => this.font_debug = value; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Title { get => this.font_node_title; set => this.font_node_title = value; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Connector { get => this.font_node_connector; set => this.font_node_connector = value; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Title_Scaled { get => this.font_node_title_scaled; set => this.font_node_title_scaled = value; }

        [Category("NodeGraph View Fonts")]
        public Font Font_Node_Connector_Scaled { get => this.font_node_connector_scaled; set => this.font_node_connector_scaled = value; }

        #endregion

        #region - - Grid - -

        [Category("NodeGraph View Grid")]
        public bool Grid_Enable { get => this.grid_enable; set => this.grid_enable = value; }

        [Category("NodeGraph View Grid")]
        public float Grid_Padding { get => this.grid_padding; set => this.grid_padding = value; }

        [Category("NodeGraph View Grid")]
        public float Grid_Alpha { get => this.grid_alpha; set => this.grid_alpha = value; }

        [Category("NodeGraph View Grid")]
        public Vector4 Grid_Color { get => this.grid_color; set => this.grid_color = value; }

        [Category("NodeGraph View Misc")]
        public LinkVisualStyle LinkVisualStyle { get => this.linkVisualStyle; set => this.linkVisualStyle = value; }

        [Category("NodeGraph View Misc")]
        public float Node_Connector_Text_TH { get => this.node_connector_text_th; set => this.node_connector_text_th = value; }

        [Category("NodeGraph View Misc")]
        public float Node_Title_Text_TH { get => this.node_title_text_th; set => this.node_title_text_th = value; }

        [Category("NodeGraph View Misc")]
        public float Connector_HZB { get => this.connector_hzb; set => this.connector_hzb = value; }

        [Category("NodeGraph View Misc")]
        public float Link_Hardness { get => this.link_hardness; set => this.link_hardness = value; }

        [Category("NodeGraph View Misc")]
        public float Node_Header_Size { get => this.node_header_size; set => this.node_header_size = value; }

        #endregion

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's collection
        /// </summary>
        public int GetNodeIndex(NodeGraphNode node)
        {
            return this.graph.GetNodeIndex(node);
        }

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's current selection
        /// </summary>
        public int GetNodeIndexSelected(NodeGraphNode node)
        {
            return this.graph.GetNodeIndexSelected(node);
        }

        #endregion
    }
}
