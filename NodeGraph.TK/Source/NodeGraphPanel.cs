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
using System.Data;
using System.Text;
using System.Windows.Forms;

using NodeGraph.TK;
using OpenTK;

namespace NodeGraph.TK
{
    #region ENUM EditMode
    /// <summary>
    /// Current edit mode
    /// </summary>
    public enum NodeGraphEditMode
    {
        Idle,
        Scrolling,
        Zooming,
        Selecting,
        SelectingBox,
        MovingSelection,
        Linking
    }
    #endregion

    #region ENUM HitType
    /// <summary>
    /// Type of mouse hit
    /// </summary>
    public enum HitType
    {
        None,
        Node,
        Connector
    }
    #endregion

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

    #region CLASS NodeGraphPanel
    /// <summary>
    /// 
    /// </summary>
    public partial class NodeGraphPanel : UserControl

    //     GLControl
    //{
    //    public CustomGLControl()
    //        : base(new GraphicsMode(new ColorFormat(32), 24, 8, 8), 4, 5, GraphicsContextFlags.ForwardCompatible)

        {
        #region - Event Handler -

        public event NodeGraphPanelSelectionEventHandler onSelectionChanged;
        public event NodeGraphPanelSelectionEventHandler onSelectionCleared;
        public event NodeGraphPanelLinkEventHandler onLinkCreated;
        public event NodeGraphPanelLinkEventHandler onLinkDestroyed;
        public event PaintEventHandler onDrawBackground;

        #endregion

        #region - Private Variables -

        // General Behavior
        private int m_iScrollLastX;
        private int m_iScrollLastY;
        private Font m_oDebugFont;
        private Point m_SelectBoxOrigin;
        private Point m_SelectBoxCurrent;
        private Point m_ViewSpaceCursorLocation;
        private int m_ScrollMargins;
        private int m_ScrollMarginsValue;
        private Point m_MoveLastPosition;
        private int m_NodeHeaderSize;
        private float m_LinkHardness;
        private bool m_bDrawDebug;

        // Colors
        private Color m_NodeTextColor;
        private Color m_NodeTextShadowColor;
        private Color m_NodeFillColor;
        private Color m_NodeFillSelectedColor;
        private Color m_NodeOutlineColor;
        private Color m_NodeOutlineSelectedColor;
        private Color m_ConnectorTextColor;
        private Color m_ConnectorFillColor;
        private Color m_ConnectorOutlineColor;
        private Color m_ConnectorSelectedFillColor;
        private Color m_ConnectorOutlineSelectedColor;
        private Color m_SelectionFillColor;
        private Color m_SelectionOutlineColor;
        private Color m_LinkColor;
        private Color m_LinkEditableColor;
        private Color m_NodeSignalValidColor;
        private Color m_NodeSignalInvalidColor;

        // Fonts
        private Font m_NodeTitleFont;
        private Font m_NodeConnectorFont;

        private Font m_NodeScaledTitleFont;
        private Font m_NodeScaledConnectorFont;


        // Brushes and Pens
        private SolidBrush m_NodeText;
        private SolidBrush m_NodeTextShadow;

        // Nodes
        private Pen m_NodeOutline;
        private Pen m_NodeOutlineSelected;
        private SolidBrush m_NodeFill;
        private SolidBrush m_NodeFillSelected;
        private SolidBrush m_NodeSignalValid;
        private SolidBrush m_NodeSignalInvalid;
        private bool m_bDrawShadow;


        // Connectors
        private SolidBrush m_ConnectorText;
        private Pen m_ConnectorOutline;
        private SolidBrush m_ConnectorFill;
        private Pen m_ConnectorOutlineSelected;
        private SolidBrush m_ConnectorFillSelected;
        private int m_ConnectorHitZoneBleed;

        // Text Draw tresholds
        private float m_fNodeConnectorTextZoomTreshold;
        private float m_fNodeTitleZoomThreshold;
        // Selection Box
        private Pen m_SelectionOutline;
        private SolidBrush m_SelectionFill;

        // Links
        private Pen m_Link;
        private Pen m_LinkEditable;
        private Brush m_LinkArrow;

        // Node HeaderGradient
        private SolidBrush m_NodeHeaderFill;
        private Color m_NodeHeaderColor;

        // For Linking
        private NodeGraphConnector m_InputLink;
        private NodeGraphConnector m_OutputLink;
        private bool m_bAltPressed;
        private bool m_bCtrlPressed;
        private LinkVisualStyle m_LinkVisualStyle;

        // Grid support
        private int m_iGridPadding;
        private bool m_bShowGrid;
        private byte m_iGridAlpha;

        // Smooth Behavior
        private bool m_bSmoothBehavior;

        private NodeGraphEditMode m_eEditMode;

        private NodeGraphView  view;
        private NodeGraphGraph graph;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a UserControl component that displays NodeGraphViews and let interact with NodeGraphNodes & NodeGraphLinks
        /// </summary>
        public NodeGraphPanel()
        {
            InitializeComponent();

            this.graph                         = new NodeGraphGraph();
            this.view                          = new NodeGraphView(this, this.graph);
            this.m_oDebugFont                  = new Font("Tahoma", 8.0f);
            this.m_ScrollMargins               = 32;
            this.m_ScrollMarginsValue          = 10;
            this.m_eEditMode                   = NodeGraphEditMode.Idle;
            this.m_ViewSpaceCursorLocation     = new Point();
            this.m_bAltPressed                 = false;
            this.m_bCtrlPressed                = false;
            this.m_NodeText                    = new SolidBrush(Color.White);
            this.m_NodeTextShadow              = new SolidBrush(Color.Black);
            this.NodeTitleZoomThreshold        = 0.5f;
            this.NodeConnectorTextZoomTreshold = 0.8f;
            this.NodeHeaderSize                = 24;
            this.NodeTextColor                 = Color.FromArgb(255, 255, 255, 255);
            this.NodeTextShadowColor           = Color.FromArgb(128, 0, 0, 0);
            this.NodeFillColor                 = Color.FromArgb(255, 128, 128, 128);
            this.NodeFillSelectedColor         = Color.FromArgb(255, 160, 128, 100);
            this.NodeOutlineColor              = Color.FromArgb(255, 180, 180, 180);
            this.NodeOutlineSelectedColor      = Color.FromArgb(255, 192, 160, 128);
            this.ConnectorTextColor            = Color.FromArgb(255, 64, 64, 64);
            this.ConnectorFillColor            = Color.FromArgb(255, 0, 0, 0);
            this.ConnectorFillSelectedColor    = Color.FromArgb(255, 32, 32, 32);
            this.ConnectorOutlineColor         = Color.FromArgb(255, 32, 32, 32);
            this.ConnectorOutlineSelectedColor = Color.FromArgb(255, 64, 64, 64);
            this.SelectionFillColor            = Color.FromArgb(64, 128, 90, 30);
            this.SelectionOutlineColor         = Color.FromArgb(192, 255, 180, 60);
            this.NodeHeaderColor               = Color.FromArgb(128, 0, 0, 0);
            this.LinkColor                     = Color.FromArgb(255, 180, 180, 180);
            this.LinkEditableColor             = Color.FromArgb(255, 64, 255, 0);
            this.NodeSignalValidColor          = Color.FromArgb(255, 0, 255, 0);
            this.NodeSignalInvalidColor        = Color.FromArgb(255, 255, 0, 0);
            this.m_NodeTitleFont               = new Font("Tahoma", 8.0f);
            this.m_NodeConnectorFont           = new Font("Tahoma", 7.0f);
            this.m_NodeScaledTitleFont         = new Font(m_NodeTitleFont.Name, m_NodeTitleFont.Size);
            this.m_NodeScaledConnectorFont     = new Font(m_NodeConnectorFont.Name, m_NodeConnectorFont.Size);
            this.m_LinkVisualStyle             = LinkVisualStyle.Curve;
            this.ConnectorHitZoneBleed         = 2;
            this.m_SelectBoxOrigin             = new Point();
            this.m_SelectBoxCurrent            = new Point();
            this.LinkHardness                  = 2.0f;
            this.m_iScrollLastX                = 0;
            this.m_iScrollLastY                = 0;
            this.EnableDrawDebug               = true;
            this.Dock                          = DockStyle.Fill;
            this.DoubleBuffered                = true;
            this.m_InputLink                   = null;
            this.m_OutputLink                  = null;
            this.m_bShowGrid                   = false;
            this.m_iGridPadding                = 256;
            this.m_iGridAlpha                  = 32;
            this.m_bDrawShadow                 = true;
            this.m_bSmoothBehavior             = false;
        }

        #endregion

        #region - Properties -

        #region - - Fonts / Brushes - -

        // Brushes
        [Browsable(false)]
        public SolidBrush NodeText { get { return this.m_NodeText; } }
        [Browsable(false)]
        public SolidBrush NodeTextShadow { get { return this.m_NodeTextShadow; } }
        [Browsable(false)]
        public SolidBrush NodeHeaderFill { get { return this.m_NodeHeaderFill; } }
        [Browsable(false)]
        public Pen NodeOutline { get { return this.m_NodeOutline; } }
        [Browsable(false)]
        public Pen NodeOutlineSelected { get { return this.m_NodeOutlineSelected; } }
        [Browsable(false)]
        public SolidBrush NodeFill { get { return this.m_NodeFill; } }
        [Browsable(false)]
        public SolidBrush NodeFillSelected { get { return this.m_NodeFillSelected; } }
        [Browsable(false)]
        public SolidBrush NodeSignalValid { get { return this.m_NodeSignalValid; } }
        [Browsable(false)]
        public SolidBrush NodeSignalInvalid { get { return this.m_NodeSignalInvalid; } }
        [Browsable(false)]
        public SolidBrush ConnectorText { get { return this.m_ConnectorText; } }
        [Browsable(false)]
        public Pen ConnectorOutline { get { return this.m_ConnectorOutline; } }
        [Browsable(false)]
        public SolidBrush ConnectorFill { get { return this.m_ConnectorFill; } }
        [Browsable(false)]
        public Pen ConnectorOutlineSelected { get { return this.m_ConnectorOutlineSelected; } }
        [Browsable(false)]
        public SolidBrush ConnectorFillSelected { get { return this.m_ConnectorFillSelected; } }

        // Fonts
        [Category("NodeGraph Panel Fonts")]
        public Font NodeTitleFont { get { return this.m_NodeTitleFont; } set { this.m_NodeTitleFont = value; } }
        [Category("NodeGraph Panel Fonts")]
        public Font NodeConnectorFont { get { return this.m_NodeConnectorFont; } set { this.m_NodeConnectorFont = value; } }
        [Browsable(false)]
        public Font NodeScaledTitleFont { get { return this.m_NodeScaledTitleFont; } set { this.m_NodeScaledTitleFont = value; } }
        [Browsable(false)]
        public Font NodeScaledConnectorFont { get { return this.m_NodeScaledConnectorFont; } set { this.m_NodeScaledConnectorFont = value; } }

        #endregion

        # region - - Colors - -

        [Category("NodeGraph Panel Colors")]
        public Color NodeTextColor
        {
            get
            {
                return this.m_NodeTextColor;
            }
            set
            {
                this.m_NodeText = new SolidBrush(value);
                this.m_NodeTextColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeTextShadowColor
        {
            get
            {
                return this.m_NodeTextShadowColor;
            }
            set
            {
                this.m_NodeTextShadow = new SolidBrush(value);
                this.m_NodeTextShadowColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeFillColor
        {
            get
            {
                return this.m_NodeFillColor;
            }
            set
            {
                this.m_NodeFill = new SolidBrush(value);
                this.m_NodeFillColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeFillSelectedColor
        {
            get
            {
                return this.m_NodeFillSelectedColor;
            }
            set
            {
                this.m_NodeFillSelected = new SolidBrush(value);
                this.m_NodeFillSelectedColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeOutlineColor
        {
            get
            {
                return this.m_NodeOutlineColor;
            }
            set
            {
                this.m_NodeOutline = new Pen(value);
                this.m_NodeOutlineColor = value;
            }
        }
        [Category("NodeGraph Panel Colors")]
        public Color NodeOutlineSelectedColor
        {
            get { return this.m_NodeOutlineSelectedColor; }
            set
            {
                this.m_NodeOutlineSelected = new Pen(value);
                this.m_NodeOutlineSelectedColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeSignalValidColor
        {
            get
            {
                return this.m_NodeSignalValidColor;
            }
            set
            {
                this.m_NodeSignalValid = new SolidBrush(value);
                this.m_NodeSignalValidColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeSignalInvalidColor
        {
            get
            {
                return this.m_NodeSignalInvalidColor;
            }
            set
            {
                this.m_NodeSignalInvalid = new SolidBrush(value);
                this.m_NodeSignalInvalidColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color ConnectorTextColor
        {
            get
            {
                return this.m_ConnectorTextColor;
            }
            set
            {
                this.m_ConnectorText = new SolidBrush(value);
                this.m_ConnectorTextColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color ConnectorFillColor
        {
            get
            {
                return this.m_ConnectorFillColor;
            }
            set
            {
                this.m_ConnectorFill = new SolidBrush(value);
                this.m_ConnectorFillColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color ConnectorOutlineColor
        {
            get
            {
                return this.m_ConnectorOutlineColor;
            }
            set
            {
                this.m_ConnectorOutline = new Pen(value);
                this.m_ConnectorOutlineColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color ConnectorFillSelectedColor
        {
            get
            {
                return this.m_ConnectorSelectedFillColor;
            }
            set
            {
                this.m_ConnectorFillSelected = new SolidBrush(value);
                this.m_ConnectorSelectedFillColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color ConnectorOutlineSelectedColor
        {
            get
            {
                return this.m_ConnectorOutlineSelectedColor;
            }
            set
            {
                this.m_ConnectorOutlineSelected = new Pen(value);
                this.m_ConnectorOutlineSelectedColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color SelectionFillColor
        {
            get
            {
                return this.m_SelectionFillColor;
            }
            set
            {
                this.m_SelectionFill = new SolidBrush(value);
                this.m_SelectionFillColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color SelectionOutlineColor
        {
            get
            {
                return this.m_SelectionOutlineColor;
            }
            set
            {
                this.m_SelectionOutline = new Pen(value);
                this.m_SelectionOutlineColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color NodeHeaderColor
        {
            get { return this.m_NodeHeaderColor; }
            set
            {
                this.m_NodeHeaderFill = new SolidBrush(value);
                this.m_NodeHeaderColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color LinkColor
        {
            get
            {
                return this.m_LinkColor;
            }
            set
            {
                this.m_Link      = new Pen(value, 0.5f);
                this.m_LinkArrow = new SolidBrush(value);
                this.m_LinkColor = value;
            }
        }

        [Category("NodeGraph Panel Colors")]
        public Color LinkEditableColor
        {
            get
            {
                return this.m_LinkEditableColor;
            }
            set
            {
                this.m_LinkEditable      = new Pen(value, 0.5f);
                this.m_LinkEditableColor = value;
            }
        }

        #endregion

        #region - - Panel - -

        [Category("NodeGraph Panel")]
        public float NodeTitleZoomThreshold
        {
            get { return this.m_fNodeTitleZoomThreshold; }
            set { this.m_fNodeTitleZoomThreshold = value; }
        }

        [Category("NodeGraph Panel")]
        public float NodeConnectorTextZoomTreshold
        {
            get { return this.m_fNodeConnectorTextZoomTreshold; }
            set { this.m_fNodeConnectorTextZoomTreshold = value; }
        }

        public Rectangle ViewSpaceRectangle;

        [Category("NodeGraph Panel")]
        public bool EnableDrawDebug
        {
            get { return this.m_bDrawDebug; }
            set { this.m_bDrawDebug = value; }
        }

        [Category("NodeGraph Panel")]
        public int NodeHeaderSize
        {
            get { return this.m_NodeHeaderSize; }
            set { this.m_NodeHeaderSize = value; }
        }

        [Category("NodeGraph Panel")]
        public LinkVisualStyle LinkVisualStyle
        {
            get { return this.m_LinkVisualStyle; }
            set { this.m_LinkVisualStyle = value; Refresh(); }
        }

        [Category("NodeGraph Panel")]
        public int ConnectorHitZoneBleed
        {
            get { return this.m_ConnectorHitZoneBleed; }
            set { this.m_ConnectorHitZoneBleed = value; }
        }

        [Category("NodeGraph Panel")]
        public float LinkHardness
        {
            get
            {
                return this.m_LinkHardness;
            }
            set
            {
                if (value < 1.0) this.m_LinkHardness = 1.0f;
                else this.m_LinkHardness = value;
            }
        }

        [Category("NodeGraph Panel")]
        public int GridPadding
        {
            get { return m_iGridPadding; }
            set { m_iGridPadding = value; }
        }

        [Category("NodeGraph Panel")]
        public bool ShowGrid
        {
            get { return m_bShowGrid; }
            set { m_bShowGrid = value; }
        }

        [Category("NodeGraph Panel")]
        public byte GridAlpha
        {
            get { return m_iGridAlpha; }
            set { m_iGridAlpha = value; }
        }

        [Category("NodeGraph Panel")]
        public bool DrawShadow
        {
            get { return m_bDrawShadow; }
            set { m_bDrawShadow = value; }
        }

        [Category("NodeGraph Panel")]
        public bool SmoothBehavior
        {
            get { return m_bSmoothBehavior; }
            set { m_bSmoothBehavior = value; }
        }

        #endregion

        public NodeGraphView View
        {
            get { return this.view; }
            set { this.view = value; }
        }

        public NodeGraphGraph Graph
        {
            get { return this.graph; }
            set { this.graph = value; }
        }

        #endregion

        #region - Methods -

        /// <summary>
        /// Adds a node to the current NodeGraphView
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(NodeGraphNode node)
        {
            this.graph.Nodes.Add(node);

            this.Refresh();
        }

        /// <summary>
        /// Adds a node to the current NodeGraphView, without redraw
        /// </summary>
        /// <param name="node"></param>
        public void AddNode_Fast(NodeGraphNode node)
        {
            this.graph.Nodes.Add(node);
        }

        /// <summary>
        /// Adds a link to the current NodeGraphView
        /// </summary>
        /// <param name="node"></param>
        public void AddLink(NodeGraphLink link)
        {
            this.graph.Links.Add(link);

            onLinkCreated(null, new NodeGraphPanelLinkEventArgs(link));

            this.Refresh();
        }

        /// <summary>
        /// Draws the selection rectangle
        /// </summary>
        /// <param name="e"></param>
        private void DrawSelectionBox(PaintEventArgs e)
        {
            if (this.m_eEditMode == NodeGraphEditMode.SelectingBox)
            {
                Rectangle ViewRectangle = new Rectangle();
                if (this.m_SelectBoxOrigin.X > this.m_SelectBoxCurrent.X)
                {
                    ViewRectangle.X = this.m_SelectBoxCurrent.X;
                    ViewRectangle.Width = this.m_SelectBoxOrigin.X - this.m_SelectBoxCurrent.X;
                }
                else
                {
                    ViewRectangle.X = this.m_SelectBoxOrigin.X;
                    ViewRectangle.Width = this.m_SelectBoxCurrent.X - this.m_SelectBoxOrigin.X;
                }
                if (this.m_SelectBoxOrigin.Y > this.m_SelectBoxCurrent.Y)
                {
                    ViewRectangle.Y = this.m_SelectBoxCurrent.Y;
                    ViewRectangle.Height = this.m_SelectBoxOrigin.Y - this.m_SelectBoxCurrent.Y;
                }
                else
                {
                    ViewRectangle.Y = this.m_SelectBoxOrigin.Y;
                    ViewRectangle.Height = this.m_SelectBoxCurrent.Y - this.m_SelectBoxOrigin.Y;
                }


                e.Graphics.FillRectangle(this.m_SelectionFill, this.ViewToControl(ViewRectangle));
                e.Graphics.DrawRectangle(this.m_SelectionOutline, this.ViewToControl(ViewRectangle));



            }
        }

        /// <summary>
        /// Draws the currently edited link
        /// </summary>
        /// <param name="e"></param>
        private void DrawLinkEditable(PaintEventArgs e)
        {
            if (this.m_eEditMode == NodeGraphEditMode.Linking)
            {
                Rectangle StartRect = this.m_InputLink.GetArea();
                Point v_StartPos = new Point(StartRect.X + (int)(6 * this.View.ViewZoomCurrent), StartRect.Y + (int)(4 * this.View.ViewZoomCurrent));
                Point v_EndPos = this.ViewToControl(new Point(this.m_ViewSpaceCursorLocation.X, this.m_ViewSpaceCursorLocation.Y));
                Point v_StartPosBezier = new Point(v_StartPos.X + (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_StartPos.Y);
                Point v_EndPosBezier = new Point(v_EndPos.X - (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_EndPos.Y);

                switch (this.m_LinkVisualStyle)
                {
                    case LinkVisualStyle.Curve:

                        e.Graphics.DrawBezier(this.m_LinkEditable, v_StartPos, v_StartPosBezier, v_EndPosBezier, v_EndPos);
                        break;

                    case LinkVisualStyle.Direct:

                        e.Graphics.DrawLine(this.m_LinkEditable, v_StartPos, v_EndPos);
                        break;

                    case LinkVisualStyle.Rectangle:

                        e.Graphics.DrawLine(this.m_LinkEditable, v_StartPos, v_StartPosBezier);
                        e.Graphics.DrawLine(this.m_LinkEditable, v_StartPosBezier, v_EndPosBezier);
                        e.Graphics.DrawLine(this.m_LinkEditable, v_EndPosBezier, v_EndPos);
                        break;

                    default: 
                        break;

                }
            }
        }

        /// <summary>
        /// Draws all links already created
        /// </summary>
        /// <param name="e"></param>
        private void DrawAllLinks(PaintEventArgs e)
        {
            Rectangle v_InRect, v_Outrect;
            Point v_StartPos, v_EndPos, v_StartPosBezier, v_EndPosBezier;

            Pen   linkPen   = this.m_Link;
            Brush linkBrush = this.m_LinkArrow;

            LinkVisualStyle style = this.m_LinkVisualStyle;

            foreach (NodeGraphLink i_Link in this.graph.Links)
            {
                v_InRect         = i_Link.Input.GetArea();
                v_Outrect        = i_Link.Output.GetArea();
                v_StartPos       = new Point(v_InRect.X + (int)(6 * this.View.ViewZoomCurrent), v_InRect.Y + (int)(4 * this.View.ViewZoomCurrent));
                v_EndPos         = new Point(v_Outrect.X + (int)(-4 * this.View.ViewZoomCurrent), v_Outrect.Y + (int)(4 * this.View.ViewZoomCurrent));
                v_StartPosBezier = new Point(v_StartPos.X + (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_StartPos.Y);
                v_EndPosBezier   = new Point(v_EndPos.X - (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_EndPos.Y);

                Point[] Arrow = { new Point(v_EndPos.X + (int)(10*this.View.ViewZoomCurrent), v_EndPos.Y),
                                  new Point(v_EndPos.X , v_EndPos.Y - (int)(4*this.View.ViewZoomCurrent)),
                                  new Point(v_EndPos.X , v_EndPos.Y + (int)(4*this.View.ViewZoomCurrent))};

                linkPen   = this.m_Link;
                linkBrush = this.m_LinkArrow;

                style = this.m_LinkVisualStyle;

                if (i_Link.Input.Parent.Highlighted || i_Link.Output.Parent.Highlighted)
                {
                    linkPen   = Pens.GreenYellow;
                    linkBrush = Brushes.GreenYellow;
                }

                //if (i_Link.Input.Parent.Highlighted || i_Link.Output.Parent.Highlighted)
                //{
                //    linkPen   = Pens.YellowGreen;
                //    linkBrush = Brushes.YellowGreen;
                //}

                if (Math.Abs(v_StartPos.X - v_EndPos.X) > 512 && !i_Link.Input.Parent.Highlighted && !i_Link.Output.Parent.Highlighted)
                {
                    //linkPen   = Pens.DarkGray;
                    //linkBrush = Brushes.DarkGray;

                    style = LinkVisualStyle.Dummy;
                }

                if (Math.Abs(v_StartPos.Y - v_EndPos.Y) > 512 && !i_Link.Input.Parent.Highlighted && !i_Link.Output.Parent.Highlighted)
                {
                    //linkPen   = Pens.DarkGray;
                    //linkBrush = Brushes.DarkGray;

                    style = LinkVisualStyle.Dummy;
                }

                switch (style)
                {
                    case LinkVisualStyle.Curve:
                        e.Graphics.DrawBezier(linkPen, v_StartPos, v_StartPosBezier, v_EndPosBezier, v_EndPos);
                        break;
                    case LinkVisualStyle.Direct:
                        v_EndPos = new Point(v_Outrect.X + (int)(-4 * this.View.ViewZoomCurrent), v_Outrect.Y + +(int)(4 * this.View.ViewZoomCurrent));
                        e.Graphics.DrawLine(linkPen, v_StartPos, v_EndPos);
                        break;
                    case LinkVisualStyle.Rectangle:
                        e.Graphics.DrawLine(linkPen, v_StartPos, v_StartPosBezier);
                        e.Graphics.DrawLine(linkPen, v_StartPosBezier, v_EndPosBezier);
                        e.Graphics.DrawLine(linkPen, v_EndPosBezier, v_EndPos);
                        break;

                    case LinkVisualStyle.Dummy:
                        e.Graphics.DrawLine(linkPen, v_StartPos, v_StartPos + new Size(32, 0));
                        e.Graphics.DrawLine(linkPen, v_EndPos - new Size(26, 0), v_EndPos);
                        e.Graphics.DrawEllipse(linkPen, v_StartPos.X + 29, v_StartPos.Y - 3, 6, 6);
                        e.Graphics.DrawEllipse(linkPen, v_EndPos.X - 29, v_EndPos.Y - 3, 6, 6);
                        break;

                    default: break;
                }

                e.Graphics.FillPolygon(linkBrush, Arrow);
            }
        }

        /// <summary>
        /// OnPaint Manager: Draws the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (this.onDrawBackground != null) onDrawBackground(this, e);

            // Smooth Behavior
            if (this.m_bSmoothBehavior)
            {
                this.View.ViewZoomCurrent += (this.View.ViewZoom - this.View.ViewZoomCurrent) * 0.08f;
                if (Math.Abs(this.View.ViewZoomCurrent - this.View.ViewZoom) < 0.005)
                {
                    this.View.ViewZoomCurrent = this.View.ViewZoom;
                    UpdateFontSize();
                }
                else
                {
                    UpdateFontSize();
                    this.Invalidate();
                }
            }
            else
            {

            }


            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            if (this.m_bShowGrid)
            {
                Color v_GridColor;
                int bgLum = (int)((BackColor.R + BackColor.G + BackColor.B) / 3);
                if (bgLum < 128) v_GridColor = Color.FromArgb(this.m_iGridAlpha, 255, 255, 255);
                else v_GridColor = Color.FromArgb(this.m_iGridAlpha, 0, 0, 0);

                Pen v_GridPen = new Pen(v_GridColor);

                int v_minGridX, v_maxGridX, v_minGridY, v_maxGridY;
                Point v_ViewTopLeft = ControlToView(new Point(0, 0));
                Point v_ViewBottomRight = ControlToView(new Point(Width, Height));
                v_minGridX = v_ViewTopLeft.X - (v_ViewTopLeft.X % this.m_iGridPadding);
                v_minGridY = v_ViewTopLeft.Y - (v_ViewTopLeft.Y % this.m_iGridPadding);
                v_maxGridX = v_ViewBottomRight.X + (v_ViewBottomRight.X % this.m_iGridPadding);
                v_maxGridY = v_ViewBottomRight.Y + (v_ViewBottomRight.Y % this.m_iGridPadding);

                Point v_CurrentGridIn, v_CurrentGridOut;

                for (int i = v_minGridX; i < v_maxGridX; i += m_iGridPadding)
                {
                    v_CurrentGridIn = ViewToControl(new Point(i, v_ViewTopLeft.Y));
                    v_CurrentGridOut = ViewToControl(new Point(i, v_ViewBottomRight.Y));
                    e.Graphics.DrawLine(v_GridPen, v_CurrentGridIn, v_CurrentGridOut);

                }
                for (int j = v_minGridY; j < v_maxGridY; j += m_iGridPadding)
                {
                    v_CurrentGridIn = ViewToControl(new Point(v_ViewTopLeft.X, j));
                    v_CurrentGridOut = ViewToControl(new Point(v_ViewBottomRight.X, j));
                    e.Graphics.DrawLine(v_GridPen, v_CurrentGridIn, v_CurrentGridOut);
                }

            }

            foreach (NodeGraphNode i_Node in this.graph.Nodes)
            {
                i_Node.Draw(e);
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            DrawLinkEditable(e);
            DrawAllLinks(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            // Select Box
            DrawSelectionBox(e);

            if (this.EnableDrawDebug) this.DrawDebug(e);
        }

        /// <summary>
        /// Draws debug information
        /// </summary>
        /// <param name="e"></param>
        private void DrawDebug(PaintEventArgs e)
        {
            e.Graphics.DrawString("Edit Mode:" + m_eEditMode.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0.0f, 0.0f));
            e.Graphics.DrawString("ViewX: " + this.View.ViewX.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0.0f, 10.0f));
            e.Graphics.DrawString("ViewY: " + this.View.ViewY.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0.0f, 20.0f));
            e.Graphics.DrawString("ViewZoom: " + this.View.ViewZoom.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0.0f, 30.0f));

            e.Graphics.DrawString("ViewSpace Cursor Location:" + this.m_ViewSpaceCursorLocation.X.ToString() + " : " + this.m_ViewSpaceCursorLocation.Y.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0.0f, 50.0f));

            e.Graphics.DrawString("AltPressed: " + this.m_bAltPressed.ToString(), this.m_oDebugFont, this.m_NodeText, new PointF(0.0f, 70.0f));

            // BELOW: DEBUG ELEMENTS

            Pen originPen = new Pen(Color.Lime);
            e.Graphics.DrawLine(originPen, this.ViewToControl(new Point(-100, 0)), this.ViewToControl(new Point(100, 0)));
            e.Graphics.DrawLine(originPen, this.ViewToControl(new Point(0, -100)), this.ViewToControl(new Point(0, 100)));

            e.Graphics.DrawBezier(originPen, this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxCurrent), this.ViewToControl(this.m_SelectBoxCurrent));

        }

        /// <summary>
        /// Behavior when Mouse is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseDown(object sender, MouseEventArgs e)
        {
            switch (this.m_eEditMode)
            {
                case NodeGraphEditMode.Idle:
                    switch (e.Button)
                    {
                        case MouseButtons.Middle:

                            this.m_eEditMode = NodeGraphEditMode.Scrolling;
                            this.m_iScrollLastX = e.Location.X;
                            this.m_iScrollLastY = e.Location.Y;

                            break;
                        case MouseButtons.Left:

                            if (this.HitAll(e.Location) == HitType.Connector)
                            {
                                //if (ModifierKeys == Keys.Alt)
                                if (!m_bAltPressed)
                                {
                                    this.m_eEditMode = NodeGraphEditMode.Linking;
                                    this.m_InputLink = GetHitConnector(e.Location);
                                    this.m_OutputLink = null;
                                }
                                else
                                {
                                    NodeGraphConnector v_Connector = GetHitConnector(e.Location);
                                    this.Delete_LinkConnectors(v_Connector);
                                }

                            }
                            // Selection is present => Move Existing Selection Arround
                            else if (this.graph.NodesSelected.Count >= 1 && this.HitSelected(e.Location) == HitType.Node)
                            {
                                this.m_eEditMode = NodeGraphEditMode.MovingSelection;
                                this.m_MoveLastPosition = this.ControlToView(e.Location);
                            }
                            // Selection is not present => Select and Move
                            else if (this.graph.NodesSelected.Count == 0 && this.HitAll(e.Location) == HitType.Node)
                            {
                                this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                                this.m_SelectBoxOrigin = this.ControlToView(new Point(e.X, e.Y));

                                this.UpdateHighlights();
                                this.CreateSelection();

                                this.m_eEditMode = NodeGraphEditMode.MovingSelection;
                                this.m_MoveLastPosition = this.ControlToView(e.Location);
                            }
                            else
                            {
                                this.m_eEditMode = NodeGraphEditMode.Selecting;

                                this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                                this.m_SelectBoxOrigin  = this.ControlToView(new Point(e.X, e.Y));
                                this.UpdateHighlights();
                                this.CreateSelection();

                                //if (this.graph.NodesSelected.Count > 0)
                                //{
                                //    this.m_eEditMode = NodeGraphEditMode.MovingSelection;
                                //    this.m_MoveLastPosition = this.ControlToView(e.Location);
                                //}
                            }
                            break;
                        default:

                            break;
                    }
                    break;

                default: break;
            }

            this.Refresh();
        }

        /// <summary>
        /// Behavior when Mouse Wheel is turned
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float newViewZoom;

            if (e.Delta != 0)
            {
                newViewZoom = this.View.ViewZoom + ((float)e.Delta * 0.001f);
                if (newViewZoom > 0.1f && newViewZoom < 2.0f)
                    this.View.ViewZoom = newViewZoom;

            }

            if (this.m_eEditMode == NodeGraphEditMode.SelectingBox)
            {
                this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
            }
            UpdateFontSize();

            Refresh();
        }

        /// <summary>
        /// Behavior when Mouse Click is released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseUp(object sender, MouseEventArgs e)
        {
            switch (this.m_eEditMode)
            {
                case NodeGraphEditMode.Scrolling:

                    if (e.Button == MouseButtons.Middle) 
                        this.m_eEditMode = NodeGraphEditMode.Idle;
                    break;

                case NodeGraphEditMode.Selecting:

                case NodeGraphEditMode.SelectingBox:

                    if (e.Button == MouseButtons.Left)
                    {
                        this.CreateSelection();
                        this.m_eEditMode = NodeGraphEditMode.Idle;

                        Refresh();
                    }
                    break;

                case NodeGraphEditMode.MovingSelection:

                    if (e.Button == MouseButtons.Left)
                    {
                        this.m_eEditMode = NodeGraphEditMode.Idle;
                    }
                    break;

                case NodeGraphEditMode.Linking:

                    this.m_OutputLink = GetHitConnector(e.Location);

                    this.ValidateLink();

                    this.m_eEditMode = NodeGraphEditMode.Idle;
                    break;

                default:
                    break;
            }

            this.Refresh();
        }

        /// <summary>
        /// Behavior when mouse is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.m_ViewSpaceCursorLocation = this.ControlToView(new Point(e.X, e.Y));

            switch (this.m_eEditMode)
            {
                case NodeGraphEditMode.Scrolling:

                    this.View.ViewX += (int)((e.Location.X - m_iScrollLastX) / this.View.ViewZoom);
                    this.View.ViewY += (int)((e.Location.Y - m_iScrollLastY) / this.View.ViewZoom);

                    this.m_iScrollLastX = e.Location.X;
                    this.m_iScrollLastY = e.Location.Y;

                    this.Refresh();

                    break;

                case NodeGraphEditMode.Selecting:

                    this.m_eEditMode = NodeGraphEditMode.SelectingBox;
                    this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                    this.UpdateHighlights();
                    this.Refresh();

                    break;

                case NodeGraphEditMode.SelectingBox:

                    if (this.IsInScrollArea(e.Location))
                    {
                        this.UpdateScroll(e.Location);
                    }

                    this.m_SelectBoxCurrent = this.ControlToView(new Point(e.X, e.Y));
                    this.UpdateHighlights();
                    this.Refresh();

                    break;

                case NodeGraphEditMode.MovingSelection:

                    if (this.IsInScrollArea(e.Location))
                    {
                        this.UpdateScroll(e.Location);
                    }

                    Point currentCursorLoc = this.ControlToView(e.Location);

                    int deltaX = this.m_MoveLastPosition.X - currentCursorLoc.X;
                    int deltaY = this.m_MoveLastPosition.Y - currentCursorLoc.Y;

                    this.MoveSelection(new Point(deltaX, deltaY));

                    this.Refresh();
                    this.m_MoveLastPosition = currentCursorLoc;

                    break;

                case NodeGraphEditMode.Linking:
                    
                    if (this.IsInScrollArea(e.Location))
                    {
                        this.UpdateScroll(e.Location);
                    }
                    this.Refresh();

                    break;

                default:
                    //this.Refresh();

                    break;
            }
        }

        /// <summary>
        /// Behavior when panel is resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }

        /// <summary>
        /// Behavior when keyboard key is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt) 
                m_bAltPressed = true;

            if (e.Control) 
                m_bCtrlPressed = true;

            if (e.KeyCode == Keys.Delete) 
                Delete_SelectedNodes();
        }

        /// <summary>
        /// Behavior when keyboard key is released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Alt) 
                m_bAltPressed = false;

            if (!e.Control) 
                m_bCtrlPressed = false;
        }

        /*
         *  Utilities
         * 
         */
        /// <summary>
        /// Converts Control Space to View Space (Point)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 ControlToView(Vector2 point)
        {
            return new Vector2((int)((point.X - (this.Width  / 2)) / this.View.ViewZoomCurrent) - this.View.ViewX,
                               (int)((point.Y - (this.Height / 2)) / this.View.ViewZoomCurrent) - this.View.ViewY);
        }

        /// <summary>
        /// Converts View Space to Control Space (Point)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 ViewToControl(Vector2 point)
        {
            return new Vector2((int)((point.X + this.View.ViewX) * this.View.ViewZoomCurrent) + (this.Width  / 2),
                               (int)((point.Y + this.View.ViewY) * this.View.ViewZoomCurrent) + (this.Height / 2));
        }

        /// <summary>
        /// Converts Control Space to View Space (Rectangle)
        /// </summary>
        /// <param name="p_Rectangle"></param>
        /// <returns></returns>
        public RectangleF ControlToView(RectangleF p_Rectangle)
        {
            Vector2 pos = this.ControlToView(new Vector2(p_Rectangle.X, p_Rectangle.Y));

            float w = (int)(p_Rectangle.Width  / this.View.ViewZoomCurrent);
            float h = (int)(p_Rectangle.Height / this.View.ViewZoomCurrent);

            return new RectangleF(pos.X, pos.Y, w, h);
        }

        /// <summary>
        /// Converts View Space to Control Space (Rectangle)
        /// </summary>
        /// <param name="p_Rectangle"></param>
        /// <returns></returns>
        public RectangleF ViewToControl(RectangleF p_Rectangle)
        {
            Vector2 pos = this.ControlToView(new Vector2(p_Rectangle.X, p_Rectangle.Y));

            float w = (int)(p_Rectangle.Width  * this.View.ViewZoomCurrent);
            float h = (int)(p_Rectangle.Height * this.View.ViewZoomCurrent);

            return new RectangleF(pos.X, pos.Y, w, h);
        }

        /// <summary>
        /// Tells if the cursor is in the Scroll Area
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        public bool IsInScrollArea(Point p_CursorLocation)
        {
            if (p_CursorLocation.X > this.m_ScrollMargins
                && p_CursorLocation.X < this.Width - this.m_ScrollMargins
                && p_CursorLocation.Y > this.m_ScrollMargins
                && p_CursorLocation.Y < this.Height - this.m_ScrollMargins) return false;
            else return true;
        }

        /// <summary>
        /// Updates Panel Scrolling
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        private void UpdateScroll(Point p_CursorLocation)
        {
            if (p_CursorLocation.X < this.m_ScrollMargins)
            {
                this.View.ViewX += (int)(this.m_ScrollMarginsValue / this.View.ViewZoomCurrent);
            }
            else if (p_CursorLocation.X > this.Width - this.m_ScrollMargins)
            {
                this.View.ViewX -= (int)(this.m_ScrollMarginsValue / this.View.ViewZoomCurrent);
            }
            else if (p_CursorLocation.Y < this.m_ScrollMargins)
            {
                this.View.ViewY += (int)(this.m_ScrollMarginsValue / this.View.ViewZoomCurrent);
            }
            else if (p_CursorLocation.Y > this.Height - this.m_ScrollMargins)
            {
                this.View.ViewY -= (int)(this.m_ScrollMarginsValue / this.View.ViewZoomCurrent);
            }
        }

        /// <summary>
        /// Updates Selection Highlights
        /// </summary>
        private void UpdateHighlights()
        {
            Rectangle ViewRectangle = new Rectangle();
            if (this.m_SelectBoxOrigin.X > this.m_SelectBoxCurrent.X)
            {
                ViewRectangle.X = this.m_SelectBoxCurrent.X;
                ViewRectangle.Width = this.m_SelectBoxOrigin.X - this.m_SelectBoxCurrent.X;
            }
            else
            {
                ViewRectangle.X = this.m_SelectBoxOrigin.X;
                ViewRectangle.Width = this.m_SelectBoxCurrent.X - this.m_SelectBoxOrigin.X;
            }
            if (this.m_SelectBoxOrigin.Y > this.m_SelectBoxCurrent.Y)
            {
                ViewRectangle.Y = this.m_SelectBoxCurrent.Y;
                ViewRectangle.Height = this.m_SelectBoxOrigin.Y - this.m_SelectBoxCurrent.Y;
            }
            else
            {
                ViewRectangle.Y = this.m_SelectBoxOrigin.Y;
                ViewRectangle.Height = this.m_SelectBoxCurrent.Y - this.m_SelectBoxOrigin.Y;
            }

            foreach (NodeGraphNode i_Node in this.graph.Nodes)
            {
                //Console.WriteLine(ModifierKeys == Keys.Control);

                if (ModifierKeys == Keys.Control && ModifierKeys != Keys.Alt)
                {
                    if (i_Node.HitRectangle.IntersectsWith(ViewRectangle) && i_Node.Selectable)
                        i_Node.Highlighted = true;
                }

                if (ModifierKeys != Keys.Control && ModifierKeys == Keys.Alt)
                {
                    if (i_Node.HitRectangle.IntersectsWith(ViewRectangle) && i_Node.Selectable)
                        i_Node.Highlighted = false;
                }

                if (ModifierKeys != Keys.Control && ModifierKeys != Keys.Alt)
                {
                    if (i_Node.HitRectangle.IntersectsWith(ViewRectangle) && i_Node.Selectable)
                        i_Node.Highlighted = true;
                    else
                        i_Node.Highlighted = false;
                }
            }
        }

        /// <summary>
        /// Creates a selection of NodeGraphNodes depending of the click or selection rectangle
        /// </summary>
        private void CreateSelection()
        {
            this.graph.NodesSelected.Clear();
            int i = 0;
            foreach (NodeGraphNode i_Node in this.graph.Nodes)
            {
                if (i_Node.Highlighted)
                {
                    i++;
                    this.graph.NodesSelected.Add(i_Node);
                }

            }
            if (i > 0 && this.onSelectionChanged != null)
            {
                onSelectionChanged(this, new NodeGraphPanelSelectionEventArgs(i));
            }
            if (i == 0 && this.onSelectionCleared != null)
            {
                onSelectionCleared(this, new NodeGraphPanelSelectionEventArgs(i));
            }
        }

        /// <summary>
        /// Returns a HitType depending on what Hit the cursor within the selected items
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        private HitType HitSelected(Point p_CursorLocation)
        {
            Rectangle HitTest = new Rectangle(this.ControlToView(p_CursorLocation), new Size());

            foreach (NodeGraphNode i_Node in this.graph.NodesSelected)
            {
                if (HitTest.IntersectsWith(i_Node.HitRectangle))
                {
                    NodeGraphConnector v_HitConnector = i_Node.GetConnectorMouseHit(p_CursorLocation);

                    if (v_HitConnector == null) 
                        return HitType.Node;
                    else 
                        return HitType.Connector;
                }
            }

            return HitType.None;
        }

        /// <summary>
        /// Returns a HitType depending on what Hit the cursor within the All items
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        private HitType HitAll(Point p_CursorLocation)
        {

            Rectangle HitTest = new Rectangle(this.ControlToView(p_CursorLocation), new Size());

            foreach (NodeGraphNode i_Node in this.graph.Nodes)
            {
                if (HitTest.IntersectsWith(i_Node.HitRectangle))
                {
                    NodeGraphConnector v_HitConnector = i_Node.GetConnectorMouseHit(p_CursorLocation);
                    if (v_HitConnector == null) return HitType.Node;
                    else return HitType.Connector;
                }
            }

            return HitType.None;
        }

        /// <summary>
        /// Gets the connector associated to the mouse hit
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        private NodeGraphConnector GetHitConnector(Point p_CursorLocation)
        {
            NodeGraphConnector v_OutConnector = null;

            Rectangle HitTest = new Rectangle(this.ControlToView(p_CursorLocation), new Size());

            foreach (NodeGraphNode i_Node in this.graph.Nodes)
            {
                if (HitTest.IntersectsWith(i_Node.HitRectangle))
                {
                    return i_Node.GetConnectorMouseHit(p_CursorLocation);
                }
            }


            return v_OutConnector;
        }

        /// <summary>
        /// Validates a link being edited
        /// </summary>
        private void ValidateLink()
        {
            if (this.m_InputLink      != null &&
                this.m_OutputLink     != null &&
                this.m_InputLink      != this.m_OutputLink &&
                this.m_InputLink.Type != this.m_OutputLink.Type &&
                this.m_InputLink.Data == this.m_OutputLink.Data)
            {
                if (m_InputLink.Type == ConnectorType.Output)
                {
                    if (IsLinked(m_OutputLink)) 
                        Delete_LinkConnectors(m_OutputLink);

                    // Create Link
                    NodeGraphLink link = new NodeGraphLink(m_InputLink, m_OutputLink);

                    // Add Link to View
                    this.graph.Links.Add(link);

                    // Fire Event
                    onLinkCreated(null, new NodeGraphPanelLinkEventArgs(link));
                }
                else
                {
                    if (IsLinked(m_InputLink)) 
                        Delete_LinkConnectors(m_InputLink);
                    
                    // Create Link
                    NodeGraphLink link = new NodeGraphLink(m_OutputLink, m_InputLink);

                    // Add Link to View
                    this.graph.Links.Add(link);

                    // Fire Event
                    onLinkCreated(null, new NodeGraphPanelLinkEventArgs(link));
                }
            }

            m_InputLink  = null;
            m_OutputLink = null;
        }

        /// <summary>
        /// Returns the other end of a connector
        /// </summary>
        /// <param name="p_LinkOutConnector"></param>
        /// <returns></returns>
        public NodeGraphConnector GetLink(NodeGraphConnector p_LinkOutConnector)
        {
            foreach (NodeGraphLink i_Link in this.graph.Links)
            {
                if (i_Link.Output == p_LinkOutConnector) return i_Link.Input;
            }
            return null;
        }

        /// <summary>
        /// Returns whether a connector is already linked
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool IsLinked(NodeGraphConnector node)
        {
            foreach (NodeGraphLink i_Link in this.graph.Links)
            {
                if (i_Link.Input == node || i_Link.Output == node) return true;
            }
            return false;
        }

        /// <summary>
        /// Moves the selection, given an offset
        /// </summary>
        /// <param name="offset"></param>
        private void MoveSelection(Point offset)
        {
            foreach (NodeGraphNode i_Node in this.graph.NodesSelected)
            {
                i_Node.X -= offset.X;
                i_Node.Y -= offset.Y;
                i_Node.UpdateHitRectangle();
            }
        }

        /// <summary>
        /// Deletes a link, given a connector
        /// </summary>
        /// <param name="con"></param>
        public void Delete_LinkConnectors(NodeGraphConnector con)
        {
            List<NodeGraphLink> linksToDelete = new List<NodeGraphLink>();

            foreach (NodeGraphLink link in this.graph.Links)
            {
                if (link.Input == con || link.Output == con)
                {
                    linksToDelete.Add(link);
                }
            }

            foreach (NodeGraphLink link in linksToDelete)
            {
                this.graph.Links.Remove(link);

                onLinkDestroyed(null, new NodeGraphPanelLinkEventArgs(link));
            }

            Refresh();
        }

        /// <summary>
        /// Deletes the current selection
        /// </summary>
        public void Delete_SelectedNodes()
        {
            //foreach (NodeGraphNode node in this.graph.NodesSelected)
            //{
            //    foreach (NodeGraphConnector con in node.Connectors)
            //    {
            //        this.Delete_LinkConnectors(con);
            //    }

            //    node.DisposeItem();

            //    this.graph.Nodes.Remove(node);
            //}

            //Refresh();
        }

        /// <summary>
        /// Updates font size, depending of the View Zoom
        /// </summary>
        private void UpdateFontSize()
        {

            this.m_NodeScaledTitleFont     = new Font(m_NodeTitleFont.Name, m_NodeTitleFont.Size * this.View.ViewZoomCurrent);
            this.m_NodeScaledConnectorFont = new Font(m_NodeConnectorFont.Name, m_NodeConnectorFont.Size * this.View.ViewZoomCurrent);

        }

        #endregion
    }
    #endregion

    #region DELEGATES / EVENTARGS

    public delegate void NodeGraphPanelSelectionEventHandler(object sender, NodeGraphPanelSelectionEventArgs args);

    public delegate void NodeGraphPanelLinkEventHandler(object sender, NodeGraphPanelLinkEventArgs args);

    public class NodeGraphPanelSelectionEventArgs : EventArgs
    {
        public int NewSelectionCount;

        public NodeGraphPanelSelectionEventArgs(int count)
        {
            this.NewSelectionCount = count;
        }
    }

    public class NodeGraphPanelLinkEventArgs : EventArgs
    {
        public List<NodeGraphLink> Links;

        public NodeGraphPanelLinkEventArgs(NodeGraphLink link)
        {
            this.Links = new List<NodeGraphLink>();

            this.Links.Add(link);
        }

        public NodeGraphPanelLinkEventArgs(List<NodeGraphLink> links)
        {
            this.Links = links;
        }
    }

    #endregion
}
