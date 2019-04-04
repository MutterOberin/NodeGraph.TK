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
using System.Drawing.Text;
using System.Windows.Forms;

//using QuickFont;
//using QuickFont.Configuration;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


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

    public partial class NodeGraphPanel : GLControl
    {
        #region - Event Handler -

        public event NodeGraphPanelSelectionEventHandler SelectionChanged;
        public event NodeGraphPanelSelectionEventHandler SelectionCleared;
        public event NodeGraphPanelLinkEventHandler LinkCreated;
        public event NodeGraphPanelLinkEventHandler LinkDestroyed;
        public event PaintEventHandler DrawBackground;

        #endregion

        #region - Private Variables -

        // General Behavior
        private Vector3 scroll_last;

        private int scroll_Margins;
        private int scroll_Margins_Value;

        private Vector3 select_BoxOrigin;
        private Vector3 select_BoxCurrent;

        private Vector3 mouse_position_ctrl;
        private Vector3 mouse_position_view;

        private Vector3 mouse_position_last;

        private bool enable_debug;
        private bool enable_shadow;
        private bool enable_smooth;

        // For Linking
        private NodeGraphConnector link_input;
        private NodeGraphConnector link_output;

        private bool key_down_alt;
        private bool key_down_ctrl;

        private NodeGraphEditMode editMode;

        private NodeGraphView view;
        private NodeGraphGraph graph;

        //private QFont qFont;
        //private QFontDrawing qFontDraw;
        //private QFontRenderOptions options;

        private Matrix4 projection;

        #region - - OpenGL - -

        private int gl_tick;
        private bool gl_loaded;
        private float gl_aspect;

        #endregion

        #endregion

        #region - Constructors -

        public NodeGraphPanel()
            : base(new GraphicsMode(new ColorFormat(32), 24, 8, 8), 4, 5, GraphicsContextFlags.ForwardCompatible)
        {
            InitializeComponent();

            this.graph = new NodeGraphGraph("Test");
            this.view  = new NodeGraphView(graph);

            this.Dock           = DockStyle.Fill;
            this.DoubleBuffered = true;

            this.scroll_Margins       = 32;
            this.scroll_Margins_Value = 10;

            this.editMode = NodeGraphEditMode.Idle;

            this.mouse_position_view = Vector3.Zero;
            this.mouse_position_ctrl = Vector3.Zero;

            this.key_down_alt  = false;
            this.key_down_ctrl = false;

            this.scroll_last = Vector3.Zero;

            this.select_BoxOrigin  = Vector3.Zero;
            this.select_BoxCurrent = Vector3.Zero;

            this.link_input  = null;
            this.link_output = null;

            this.enable_debug  = true;
            this.enable_shadow = true;
            this.enable_smooth = false;
        }

        #endregion

        #region - Properties -

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

        [Category("NodeGraph Panel")]
        public bool EnableDrawDebug
        {
            get { return this.enable_debug; }
            set { this.enable_debug = value; }
        }

        [Category("NodeGraph Panel")]
        public bool EnableShadow
        {
            get { return this.enable_shadow; }
            set { this.enable_shadow = value; }
        }

        [Category("NodeGraph Panel")]
        public bool EnableSmooth
        {
            get { return this.enable_smooth; }
            set { this.enable_smooth = value; }
        }

        #endregion

        #region - Methods -

        /// <summary>
        /// Adds a node to the current NodeGraphGraph
        /// </summary>
        public void Add_Node(NodeGraphNode node)
        {
            this.graph.Nodes.Add(node);

            this.Invalidate();
        }

        /// <summary>
        /// Adds a node to the current NodeGraphGraph, without redraw
        /// </summary>
        public void Add_Node_Fast(NodeGraphNode node)
        {
            this.graph.Nodes.Add(node);
        }

        /// <summary>
        /// Adds a link to the current NodeGraphGraph
        /// </summary>
        public void Add_Link(NodeGraphLink link)
        {
            this.graph.Links.Add(link);

            this.LinkCreated?.Invoke(null, new NodeGraphPanelLinkEventArgs(link));

            this.Invalidate();
        }

        /// <summary>
        /// Adds a link to the current NodeGraphGraph, without redraw
        /// </summary>
        public void Add_Link_Fast(NodeGraphLink link)
        {
            this.graph.Links.Add(link);

            this.LinkCreated?.Invoke(null, new NodeGraphPanelLinkEventArgs(link));
        }

        /// <summary>
        /// Draws the currently edited link
        /// </summary>
        /// <param name="e"></param>
        private void DrawLinkEditable(PaintEventArgs e)
        {
            //if (this.m_eEditMode == NodeGraphEditMode.Linking)
            //{
            //    Rectangle StartRect = this.m_InputLink.GetArea();
            //    Point v_StartPos = new Point(StartRect.X + (int)(6 * this.View.ViewZoomCurrent), StartRect.Y + (int)(4 * this.View.ViewZoomCurrent));
            //    Point v_EndPos = this.ViewToControl(new Point(this.m_ViewSpaceCursorLocation.X, this.m_ViewSpaceCursorLocation.Y));
            //    Point v_StartPosBezier = new Point(v_StartPos.X + (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_StartPos.Y);
            //    Point v_EndPosBezier = new Point(v_EndPos.X - (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_EndPos.Y);

            //    switch (this.m_LinkVisualStyle)
            //    {
            //        case LinkVisualStyle.Curve:

            //            e.Graphics.DrawBezier(this.m_LinkEditable, v_StartPos, v_StartPosBezier, v_EndPosBezier, v_EndPos);
            //            break;

            //        case LinkVisualStyle.Direct:

            //            e.Graphics.DrawLine(this.m_LinkEditable, v_StartPos, v_EndPos);
            //            break;

            //        case LinkVisualStyle.Rectangle:

            //            e.Graphics.DrawLine(this.m_LinkEditable, v_StartPos, v_StartPosBezier);
            //            e.Graphics.DrawLine(this.m_LinkEditable, v_StartPosBezier, v_EndPosBezier);
            //            e.Graphics.DrawLine(this.m_LinkEditable, v_EndPosBezier, v_EndPos);
            //            break;

            //        default: 
            //            break;

            //    }
            //}
        }

        /// <summary>
        /// Draws all links already created
        /// </summary>
        /// <param name="e"></param>
        private void DrawAllLinks(PaintEventArgs e)
        {
            //Rectangle v_InRect, v_Outrect;
            //Point v_StartPos, v_EndPos, v_StartPosBezier, v_EndPosBezier;

            //Pen   linkPen   = this.m_Link;
            //Brush linkBrush = this.m_LinkArrow;

            //LinkVisualStyle style = this.m_LinkVisualStyle;

            //foreach (NodeGraphLink i_Link in this.graph.Links)
            //{
            //    v_InRect         = i_Link.Input.GetArea();
            //    v_Outrect        = i_Link.Output.GetArea();
            //    v_StartPos       = new Point(v_InRect.X + (int)(6 * this.View.ViewZoomCurrent), v_InRect.Y + (int)(4 * this.View.ViewZoomCurrent));
            //    v_EndPos         = new Point(v_Outrect.X + (int)(-4 * this.View.ViewZoomCurrent), v_Outrect.Y + (int)(4 * this.View.ViewZoomCurrent));
            //    v_StartPosBezier = new Point(v_StartPos.X + (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_StartPos.Y);
            //    v_EndPosBezier   = new Point(v_EndPos.X - (int)((v_EndPos.X - v_StartPos.X) / LinkHardness), v_EndPos.Y);

            //    Point[] Arrow = { new Point(v_EndPos.X + (int)(10*this.View.ViewZoomCurrent), v_EndPos.Y),
            //                      new Point(v_EndPos.X , v_EndPos.Y - (int)(4*this.View.ViewZoomCurrent)),
            //                      new Point(v_EndPos.X , v_EndPos.Y + (int)(4*this.View.ViewZoomCurrent))};

            //    linkPen   = this.m_Link;
            //    linkBrush = this.m_LinkArrow;

            //    style = this.m_LinkVisualStyle;

            //    if (i_Link.Input.Parent.Selected || i_Link.Output.Parent.Selected)
            //    {
            //        linkPen   = Pens.GreenYellow;
            //        linkBrush = Brushes.GreenYellow;
            //    }

            //    //if (i_Link.Input.Parent.Highlighted || i_Link.Output.Parent.Highlighted)
            //    //{
            //    //    linkPen   = Pens.YellowGreen;
            //    //    linkBrush = Brushes.YellowGreen;
            //    //}

            //    if (Math.Abs(v_StartPos.X - v_EndPos.X) > 512 && !i_Link.Input.Parent.Selected && !i_Link.Output.Parent.Selected)
            //    {
            //        //linkPen   = Pens.DarkGray;
            //        //linkBrush = Brushes.DarkGray;

            //        style = LinkVisualStyle.Dummy;
            //    }

            //    if (Math.Abs(v_StartPos.Y - v_EndPos.Y) > 512 && !i_Link.Input.Parent.Selected && !i_Link.Output.Parent.Selected)
            //    {
            //        //linkPen   = Pens.DarkGray;
            //        //linkBrush = Brushes.DarkGray;

            //        style = LinkVisualStyle.Dummy;
            //    }

            //    switch (style)
            //    {
            //        case LinkVisualStyle.Curve:
            //            e.Graphics.DrawBezier(linkPen, v_StartPos, v_StartPosBezier, v_EndPosBezier, v_EndPos);
            //            break;
            //        case LinkVisualStyle.Direct:
            //            v_EndPos = new Point(v_Outrect.X + (int)(-4 * this.View.ViewZoomCurrent), v_Outrect.Y + +(int)(4 * this.View.ViewZoomCurrent));
            //            e.Graphics.DrawLine(linkPen, v_StartPos, v_EndPos);
            //            break;
            //        case LinkVisualStyle.Rectangle:
            //            e.Graphics.DrawLine(linkPen, v_StartPos, v_StartPosBezier);
            //            e.Graphics.DrawLine(linkPen, v_StartPosBezier, v_EndPosBezier);
            //            e.Graphics.DrawLine(linkPen, v_EndPosBezier, v_EndPos);
            //            break;

            //        case LinkVisualStyle.Dummy:
            //            e.Graphics.DrawLine(linkPen, v_StartPos, v_StartPos + new Size(32, 0));
            //            e.Graphics.DrawLine(linkPen, v_EndPos - new Size(26, 0), v_EndPos);
            //            e.Graphics.DrawEllipse(linkPen, v_StartPos.X + 29, v_StartPos.Y - 3, 6, 6);
            //            e.Graphics.DrawEllipse(linkPen, v_EndPos.X - 29, v_EndPos.Y - 3, 6, 6);
            //            break;

            //        default: break;
            //    }

            //    e.Graphics.FillPolygon(linkBrush, Arrow);
            //}
        }

        /// <summary>
        /// Behavior when Mouse is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseDown(object sender, MouseEventArgs e)
        {
            Vector3 location = new Vector3(e.Location.X, e.Location.Y, 0);

            switch (this.editMode)
            {
                case NodeGraphEditMode.Idle:
                    switch (e.Button)
                    {
                        case MouseButtons.Middle:

                            this.editMode = NodeGraphEditMode.Scrolling;
                            this.mouse_position_last = location;

                            break;
                        case MouseButtons.Left:

                            if (this.HitAll(location) == HitType.Connector)
                            {
                                //if (ModifierKeys == Keys.Alt)
                                if (!key_down_alt)
                                {
                                    this.editMode = NodeGraphEditMode.Linking;
                                    this.link_input = GetHitConnector(location);
                                    this.link_output = null;
                                }
                                else
                                {
                                    NodeGraphConnector v_Connector = GetHitConnector(location);
                                    this.Delete_LinkConnectors(v_Connector);
                                }

                            }
                            // Selection is present => Move Existing Selection Arround
                            else if (this.graph.NodesSelected.Count >= 1 && this.HitSelected(location) == HitType.Node)
                            {
                                this.editMode = NodeGraphEditMode.MovingSelection;
                                this.mouse_position_last = this.ControlToView(location);
                            }
                            // Selection is not present => Select and Move
                            else if (this.graph.NodesSelected.Count == 0 && this.HitAll(location) == HitType.Node)
                            {
                                this.select_BoxCurrent = this.ControlToView(location);
                                this.select_BoxOrigin = this.ControlToView(location);

                                this.UpdateHighlights();
                                this.UpdateSelection();

                                this.editMode = NodeGraphEditMode.MovingSelection;
                                this.mouse_position_last = this.ControlToView(location);
                            }
                            else
                            {
                                this.editMode = NodeGraphEditMode.Selecting;

                                this.select_BoxCurrent = this.ControlToView(location);
                                this.select_BoxOrigin = this.ControlToView(location);
                                this.UpdateHighlights();
                                this.UpdateSelection();

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

            this.Invalidate();
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
                newViewZoom = this.view.ViewZoom + ((float)e.Delta * 0.001f);

                if (newViewZoom > 0.1f && newViewZoom < 2.0f)
                    this.view.ViewZoom = newViewZoom;

            }

            if (this.editMode == NodeGraphEditMode.SelectingBox)
            {
                this.select_BoxCurrent = this.ControlToView(new Vector3(e.Location.X, e.Location.Y, 0));
            }

            //UpdateFontSize();

            this.Invalidate();
        }

        /// <summary>
        /// Behavior when Mouse Click is released
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseUp(object sender, MouseEventArgs e)
        {
            Vector3 location = new Vector3(e.Location.X, e.Location.Y, 0);

            switch (this.editMode)
            {
                case NodeGraphEditMode.Scrolling:

                    if (e.Button == MouseButtons.Middle)
                        this.editMode = NodeGraphEditMode.Idle;
                    break;

                case NodeGraphEditMode.Selecting:

                case NodeGraphEditMode.SelectingBox:

                    if (e.Button == MouseButtons.Left)
                    {
                        this.UpdateSelection();
                        this.editMode = NodeGraphEditMode.Idle;

                        Refresh();
                    }
                    break;

                case NodeGraphEditMode.MovingSelection:

                    if (e.Button == MouseButtons.Left)
                    {
                        this.editMode = NodeGraphEditMode.Idle;
                    }
                    break;

                case NodeGraphEditMode.Linking:

                    this.link_output = GetHitConnector(location);

                    this.ValidateLink();

                    this.editMode = NodeGraphEditMode.Idle;
                    break;

                default:
                    break;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Behavior when mouse is moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouse_position_ctrl = new Vector3(e.Location.X, e.Location.Y, 0);
            this.mouse_position_view = this.ControlToView(mouse_position_ctrl);

            //if (e.Button == MouseButtons.Left)
            //    this.Invalidate();

            //if (e.Button == MouseButtons.Right)
            //    this.Refresh();

            switch (this.editMode)
            {
                case NodeGraphEditMode.Scrolling:

                    this.view.ViewX -= (int)((e.Location.X - scroll_last.X) * this.view.ViewZoom);
                    this.view.ViewY += (int)((e.Location.Y - scroll_last.Y) * this.view.ViewZoom);

                    this.scroll_last.X = e.Location.X;
                    this.scroll_last.Y = e.Location.Y;

                    //this.Invalidate();

                    break;

                case NodeGraphEditMode.Selecting:

                    this.editMode = NodeGraphEditMode.SelectingBox;
                    this.select_BoxCurrent = mouse_position_view;
                    this.UpdateHighlights();
                    //this.Invalidate();

                    break;

                case NodeGraphEditMode.SelectingBox:

                    if (this.IsInScrollArea(new Vector2(e.Location.X, e.Location.Y)))
                    {
                        this.UpdateScroll(new Vector2(e.Location.X, e.Location.Y));
                    }

                    this.select_BoxCurrent = mouse_position_view;
                    this.UpdateHighlights();
                    //this.Invalidate();

                    break;

                case NodeGraphEditMode.MovingSelection:

                    if (this.IsInScrollArea(new Vector2(e.Location.X, e.Location.Y)))
                    {
                        this.UpdateScroll(new Vector2(e.Location.X, e.Location.Y));
                    }

                    Vector3 delta = this.mouse_position_last - mouse_position_view;

                    this.MoveSelection(delta);

                    //this.Invalidate();

                    this.mouse_position_last = mouse_position_view;

                    break;

                case NodeGraphEditMode.Linking:

                    if (this.IsInScrollArea(new Vector2(e.Location.X, e.Location.Y)))
                    {
                        this.UpdateScroll(new Vector2(e.Location.X, e.Location.Y));
                    }
                    //this.Invalidate();

                    break;

                default:
                    //this.Invalidate();

                    break;
            }
        }

        /// <summary>
        /// Behavior when keyboard key is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
                key_down_alt = true;

            if (e.Control)
                key_down_ctrl = true;

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
                key_down_alt = false;

            if (!e.Control)
                key_down_ctrl = false;
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
        public Vector3 ControlToView(Vector3 point)
        {
            return new Vector3((int)((point.X - (this.Width / 2)) * this.view.ViewZoom) + this.view.ViewX,
                               (int)((point.Y - (this.Height / 2)) * this.view.ViewZoom) - this.view.ViewY, 0);
        }

        /// <summary>
        /// Converts View Space to Control Space (Point)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector3 ViewToControl(Vector3 point)
        {
            return new Vector3((int)((point.X + this.view.ViewX) / this.view.ViewZoom) + (this.Width / 2),
                               (int)((point.Y + this.view.ViewY) / this.view.ViewZoom) - (this.Height / 2), 0);
        }

        /// <summary>
        /// Tells if the cursor is in the Scroll Area
        /// </summary>
        /// <param name="cursorLocation"></param>
        /// <returns></returns>
        public bool IsInScrollArea(Vector2 cursorLocation)
        {
            if (cursorLocation.X > this.scroll_Margins &&
                cursorLocation.X < this.Width - this.scroll_Margins &&
                cursorLocation.Y > this.scroll_Margins &&
                cursorLocation.Y < this.Height - this.scroll_Margins)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Updates Panel Scrolling
        /// </summary>
        /// <param name="cursorLocation"></param>
        private void UpdateScroll(Vector2 cursorLocation)
        {
            if (cursorLocation.X < this.scroll_Margins)
            {
                this.view.ViewX += (int)(this.scroll_Margins_Value / this.view.ViewZoomCurrent);
            }
            else if (cursorLocation.X > this.Width - this.scroll_Margins)
            {
                this.view.ViewX -= (int)(this.scroll_Margins_Value / this.view.ViewZoomCurrent);
            }
            else if (cursorLocation.Y < this.scroll_Margins)
            {
                this.view.ViewY += (int)(this.scroll_Margins_Value / this.view.ViewZoomCurrent);
            }
            else if (cursorLocation.Y > this.Height - this.scroll_Margins)
            {
                this.view.ViewY -= (int)(this.scroll_Margins_Value / this.view.ViewZoomCurrent);
            }
        }

        /// <summary>
        /// Updates Selection Highlights
        /// </summary>
        private void UpdateHighlights()
        {
            RectangleF ViewRectangle = new RectangleF();
            if (this.select_BoxOrigin.X > this.select_BoxCurrent.X)
            {
                ViewRectangle.X = this.select_BoxCurrent.X;
                ViewRectangle.Width = this.select_BoxOrigin.X - this.select_BoxCurrent.X;
            }
            else
            {
                ViewRectangle.X = this.select_BoxOrigin.X;
                ViewRectangle.Width = this.select_BoxCurrent.X - this.select_BoxOrigin.X;
            }
            if (this.select_BoxOrigin.Y > this.select_BoxCurrent.Y)
            {
                ViewRectangle.Y = this.select_BoxCurrent.Y;
                ViewRectangle.Height = this.select_BoxOrigin.Y - this.select_BoxCurrent.Y;
            }
            else
            {
                ViewRectangle.Y = this.select_BoxOrigin.Y;
                ViewRectangle.Height = this.select_BoxCurrent.Y - this.select_BoxOrigin.Y;
            }

            foreach (NodeGraphNode i_Node in this.graph.Nodes)
            {
                //Console.WriteLine(ModifierKeys == Keys.Control);

                if (ModifierKeys == Keys.Control && ModifierKeys != Keys.Alt)
                {
                    if (i_Node.HitRectangle.IntersectsWith(ViewRectangle) && i_Node.Selectable)
                        i_Node.Selected = true;
                }

                if (ModifierKeys != Keys.Control && ModifierKeys == Keys.Alt)
                {
                    if (i_Node.HitRectangle.IntersectsWith(ViewRectangle) && i_Node.Selectable)
                        i_Node.Selected = false;
                }

                if (ModifierKeys != Keys.Control && ModifierKeys != Keys.Alt)
                {
                    if (i_Node.HitRectangle.IntersectsWith(ViewRectangle) && i_Node.Selectable)
                        i_Node.Selected = true;
                    else
                        i_Node.Selected = false;
                }
            }
        }

        /// <summary>
        /// Creates a selection of NodeGraphNodes depending of the click or selection rectangle
        /// </summary>
        private void UpdateSelection()
        {
            this.graph.NodesSelected.Clear();

            var selectedNodes = this.graph.Nodes.FindAll(n => n.Selected);

            this.graph.NodesSelected.AddRange(selectedNodes);

            if (selectedNodes.Count >= 1)
            {
                SelectionChanged?.Invoke(this, new NodeGraphPanelSelectionEventArgs(selectedNodes.Count));
            }
            if (selectedNodes.Count == 0)
            {
                SelectionCleared?.Invoke(this, new NodeGraphPanelSelectionEventArgs(selectedNodes.Count));
            }
        }

        /// <summary>
        /// Returns a HitType depending on what Hit the cursor within the selected items
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private HitType HitSelected(Vector3 location)
        {
            //Rectangle HitTest = new RectangleF(this.ControlToView(cursorLocation), new Size());

            //foreach (NodeGraphNode i_Node in this.graph.NodesSelected)
            //{
            //    if (HitTest.IntersectsWith(i_Node.HitRectangle))
            //    {
            //        NodeGraphConnector v_HitConnector = i_Node.GetConnectorMouseHit(cursorLocation);

            //        if (v_HitConnector == null) 
            //            return HitType.Node;
            //        else 
            //            return HitType.Connector;
            //    }
            //}

            return HitType.None;
        }

        /// <summary>
        /// Returns a HitType depending on what Hit the cursor within the All items
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        private HitType HitAll(Vector3 location)
        {
            //Rectangle HitTest = new Rectangle(this.ControlToView(p_CursorLocation), new Size());

            //foreach (NodeGraphNode i_Node in this.graph.Nodes)
            //{
            //    if (HitTest.IntersectsWith(i_Node.HitRectangle))
            //    {
            //        NodeGraphConnector v_HitConnector = i_Node.GetConnectorMouseHit(p_CursorLocation);
            //        if (v_HitConnector == null) return HitType.Node;
            //        else return HitType.Connector;
            //    }
            //}

            return HitType.None;
        }

        /// <summary>
        /// Gets the connector associated to the mouse hit
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        private NodeGraphConnector GetHitConnector(Vector3 location)
        {
            //NodeGraphConnector v_OutConnector = null;

            //Rectangle HitTest = new Rectangle(this.ControlToView(p_CursorLocation), new Size());

            //foreach (NodeGraphNode i_Node in this.graph.Nodes)
            //{
            //    if (HitTest.IntersectsWith(i_Node.HitRectangle))
            //    {
            //        return i_Node.GetConnectorMouseHit(p_CursorLocation);
            //    }
            //}

            //return v_OutConnector;

            return null;
        }

        /// <summary>
        /// Validates a link being edited
        /// </summary>
        private void ValidateLink()
        {
            if (this.link_input != null &&
                this.link_output != null &&
                this.link_input != this.link_output &&
                this.link_input.Type != this.link_output.Type &&
                this.link_input.Data == this.link_output.Data)
            {
                if (link_input.Type == ConnectorType.Output)
                {
                    if (IsLinked(link_output))
                        Delete_LinkConnectors(link_output);

                    // Create Link
                    NodeGraphLink link = new NodeGraphLink(link_input, link_output);

                    // Add Link to View
                    this.graph.Links.Add(link);

                    // Fire Event
                    LinkCreated(null, new NodeGraphPanelLinkEventArgs(link));
                }
                else
                {
                    if (IsLinked(link_input))
                        Delete_LinkConnectors(link_input);

                    // Create Link
                    NodeGraphLink link = new NodeGraphLink(link_output, link_input);

                    // Add Link to View
                    this.graph.Links.Add(link);

                    // Fire Event
                    LinkCreated(null, new NodeGraphPanelLinkEventArgs(link));
                }
            }

            this.link_input = null;
            this.link_output = null;
        }

        /// <summary>
        /// Returns the other end of a connector
        /// </summary>
        public NodeGraphConnector GetLink(NodeGraphConnector connector)
        {
            // Get the Correct Link
            NodeGraphLink link = this.graph.Links.Find(l => l.Input == connector || l.Output == connector);

            if (link == null)
                return null;

            // Return Input if Output is connected
            if (link.Input == connector)
                return link.Output;

            if (link.Output == connector)
                return link.Input;

            return null;
        }

        /// <summary>
        /// Returns whether a connector is already linked
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        public bool IsLinked(NodeGraphConnector connector)
        {
            NodeGraphLink link = this.graph.Links.Find(l => l.Input == connector || l.Output == connector);

            if (link != null)
                return true;

            return false;
        }

        /// <summary>
        /// Moves the selection, given an offset
        /// </summary>
        /// <param name="offset"></param>
        private void MoveSelection(Vector3 offset)
        {
            foreach (NodeGraphNode node in this.graph.NodesSelected)
            {
                node.Position += offset;
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

                LinkDestroyed(null, new NodeGraphPanelLinkEventArgs(link));
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

        private void Setup()
        {
            if (base.DesignMode)
            {
                Console.WriteLine("Design Mode");
                return;
            }

            // Background Color
            GL.ClearColor(this.view.ColorBackground.X, this.view.ColorBackground.Y, this.view.ColorBackground.Z, 1.0f);

            // Backface Culling
            GL.CullFace(CullFaceMode.Back);

            GL.Enable(EnableCap.CullFace);

            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.PointSmooth);
            GL.Enable(EnableCap.LineSmooth);

            // Nicest!
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);

            // Depth Test
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // Blending Function
            GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            this.gl_loaded = true;
        }

        private void Setup_Viewport()
        {
            if (base.DesignMode)
            {
                Console.WriteLine("Design Mode");
                return;
            }

            // Other Stuff
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            this.gl_aspect = (float)w / (float)h;

            //screenCenter = new Vector3(w * 0.5f, h * 0.5f, 0);
            //oldMouse = screenCenter;

            GL.Viewport(0, 0, w, h);
        }

        /// <summary>
        /// Camera
        /// </summary>
        private void Setup_Camera()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            projection = Matrix4.CreateOrthographic(this.ClientSize.Width  * this.view.ViewZoom,
                                                    this.ClientSize.Height * this.view.ViewZoom, -5, 5);

            GL.MultMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            Matrix4 lookAt = Matrix4.LookAt(this.ClientSize.Width / 2, this.ClientSize.Height / 2, 1,
                                            this.ClientSize.Width / 2, this.ClientSize.Height / 2, 0,
                                            0, 1, 0);

            //Matrix4 lookAt = Matrix4.LookAt(this.view.ViewX, this.view.ViewY, 1,
            //                                this.view.ViewX, this.view.ViewY, 0,
            //                                0, 1, 0);

            GL.MultMatrix(ref lookAt);
        }

        private void Draw_Grid()
        {
            Vector2 v0 = new Vector2(-4096, -4096);
            Vector2 v1 = new Vector2(+4096, +4096);

            GL.Color4(this.view.Grid_Color);

            GL.Begin(PrimitiveType.Lines);

            for (float i = v0.X; i < v1.X; i += this.view.Grid_Padding)
            {
                GL.Vertex3(i, v0.Y, 0);
                GL.Vertex3(i, v1.Y, 0);

            }
            for (float i = v0.Y; i < v1.Y; i += this.view.Grid_Padding)
            {
                GL.Vertex3(v0.X, i, 0);
                GL.Vertex3(v1.X, i, 0);
            }

            GL.End();
        }

        /// <summary>
        /// Draws the selection rectangle
        /// </summary>
        /// <param name="e"></param>
        private void Draw_SelectionBox()
        {
            //if (this.m_eEditMode == NodeGraphEditMode.SelectingBox)
            //{
            //    Vector3 v0 = Vector3.Zero;
            //    Vector3 v1 = Vector3.Zero;

            //    if (this.select_BoxOrigin.X > this.select_BoxCurrent.X)
            //    {
            //        r.X = this.select_BoxCurrent.X;
            //        r.Width = this.select_BoxOrigin.X - this.select_BoxCurrent.X;
            //    }
            //    else
            //    {
            //        r.X = this.select_BoxOrigin.X;
            //        r.Width = this.select_BoxCurrent.X - this.select_BoxOrigin.X;
            //    }
            //    if (this.select_BoxOrigin.Y > this.select_BoxCurrent.Y)
            //    {
            //        r.Y = this.select_BoxCurrent.Y;
            //        r.Height = this.select_BoxOrigin.Y - this.select_BoxCurrent.Y;
            //    }
            //    else
            //    {
            //        r.Y = this.select_BoxOrigin.Y;
            //        r.Height = this.select_BoxCurrent.Y - this.select_BoxOrigin.Y;
            //    }


            //    //e.Graphics.FillRectangle(this.m_SelectionFill, this.ViewToControl(r));
            //    //e.Graphics.DrawRectangle(this.m_SelectionOutline, this.ViewToControl(r));

            //    r = this.ViewToControl(r);

            //    GL.Color4(this.view.Color_selection_fill);

            //    GL.Begin(PrimitiveType.Quads);

            //    GL.Vertex3(r.X, r.Y, 0);
            //    GL.Vertex3(r.X + r.Width, r.Y, 0);
            //    GL.Vertex3(r.X + r.Width, r.Y + r.Height, 0);
            //    GL.Vertex3(r.X, r.Y + r.Height, 0);

            //    GL.End();

            //    GL.Color4(this.view.Color_selection_outline);

            //    GL.Begin(PrimitiveType.LineLoop);

            //    GL.Vertex3(r.X, r.Y, 0);
            //    GL.Vertex3(r.X + r.Width, r.Y, 0);
            //    GL.Vertex3(r.X + r.Width, r.Y + r.Height, 0);
            //    GL.Vertex3(r.X, r.Y + r.Height, 0);

            //    GL.End();
            //}
        }

        private void Draw_Debug()
        {
            GL.Color3(Color.Aquamarine);

            GL.Begin(PrimitiveType.Points);

            GL.Vertex3(this.mouse_position_view.X, this.mouse_position_view.Y, 0);

            GL.End();

            GL.Begin(PrimitiveType.Points);

            GL.Vertex3(0, 0, 0);

            GL.End();
        }

        private void Draw_Debug_RefreshTick()
        {
            GL.Color3(Color.Aquamarine);

            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(10, 0, 0);
            GL.Vertex3(10, 4, 0);
            GL.Vertex3(0, 4, 0);

            GL.End();

            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0 + gl_tick, 0, 0);
            GL.Vertex3(0 + gl_tick, 4, 0);
            GL.Vertex3(0, 4, 0);

            GL.End();

            this.gl_tick++;
            this.gl_tick = this.gl_tick % 10;
        }

        private void Setup_QFont()
        {
            //this.qFont = new QFont(@"Resources\Tahoma.ttf", this.view.Font_Debug.Size, new QFontBuilderConfiguration(true));

            //this.qFontDraw = new QFontDrawing(false);

            //this.options = new QFontRenderOptions();

            //this.options.DropShadowActive        = false;
            //this.options.DropShadowOffset        = new Vector2(0.2f, 0.2f);
            //this.options.DropShadowOpacity       = 50;
            //this.options.DropShadowColour        = Color.Black;
            //this.options.UseDefaultBlendFunction = false;

            //this.options.Colour = NodeGraphUtil.VectorToColor(this.view.ColorLink);

            //this.qFontDraw.DrawingPrimitives.Clear();

            //this.qFontDraw.Print(this.qFont, "QFontText", new Vector3(0, 100, 0), QFontAlignment.Left, options);
            //this.qFontDraw.Print(this.qFont, "Edit Mode:" + editMode.ToString(), new Vector3(0, 100, 0), QFontAlignment.Left, options);

            //this.qFontDraw.RefreshBuffers();
        }

        private void NodeGraphPanel_Load(object sender, EventArgs e)
        {
            if (base.DesignMode)
                return;
            
            this.Setup();
            this.Setup_Viewport();

            this.Setup_QFont();
        }

        private void NodeGraphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (base.DesignMode)
                return;

            if (!this.gl_loaded || this.graph == null)
                return;

            //GL.Flush();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            this.Setup_Camera();

            if (this.view.Grid_Enable)
                this.Draw_Grid();

            // Select Box
            //this.Draw_SelectionBox();

            //if (this.DrawBackground != null) DrawBackground(this, e);

            //// Smooth Behavior
            //if (this.m_bSmoothBehavior)
            //{
            //    this.View.ViewZoomCurrent += (this.View.ViewZoom - this.View.ViewZoomCurrent) * 0.08f;
            //    if (Math.Abs(this.View.ViewZoomCurrent - this.View.ViewZoom) < 0.005)
            //    {
            //        this.View.ViewZoomCurrent = this.View.ViewZoom;
            //        UpdateFontSize();
            //    }
            //    else
            //    {
            //        UpdateFontSize();
            //        this.Invalidate();
            //    }
            //}
            //else
            //{

            //}


            //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            //e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;


            //foreach (NodeGraphNode i_Node in this.graph.Nodes)
            //{
            //    i_Node.Draw(e);
            //}

            //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //DrawLinkEditable(e);
            //DrawAllLinks(e);
            //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            //// Select Box
            //DrawSelectionBox(e);

            if (this.EnableDrawDebug)
            {
                this.NodeGraphPanel_Paint_Debug();
            }

            this.SwapBuffers();
        }

        private void NodeGraphPanel_Paint_Debug()
        {
            //this.qFontDraw.ProjectionMatrix = this.projection;

            //this.qFontDraw.Draw();

            this.Draw_Debug();
            this.Draw_Debug_RefreshTick();

            //e.Graphics.DrawString("Edit Mode:" + m_eEditMode.ToString(), this.view.Font_Debug, Brushes.GreenYellow, new PointF(0.0f, 0.0f));
            //e.Graphics.DrawString("ViewX: " + this.view.ViewX.ToString(), this.view.Font_Debug, Brushes.GreenYellow, new PointF(0.0f, 10.0f));
            //e.Graphics.DrawString("ViewY: " + this.view.ViewY.ToString(), this.view.Font_Debug, Brushes.GreenYellow, new PointF(0.0f, 20.0f));
            //e.Graphics.DrawString("ViewZoom: " + this.view.ViewZoom.ToString(), this.view.Font_Debug, Brushes.GreenYellow, new PointF(0.0f, 30.0f));

            //e.Graphics.DrawString("ViewSpace Cursor Location:" + this.mouse_position_view.X.ToString() + " : " + this.mouse_position_view.Y.ToString(), this.view.Font_Debug, Brushes.GreenYellow, new PointF(0.0f, 50.0f));

            //e.Graphics.DrawString("AltPressed: " + this.key_down_alt.ToString(), this.view.Font_Debug, Brushes.GreenYellow, new PointF(0.0f, 70.0f));

            // BELOW: DEBUG ELEMENTS

            GL.Color3(Color.Aquamarine);

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 0, 0);

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 100, 0);

            GL.End();

            //Pen originPen = new Pen(Color.Lime);
            //e.Graphics.DrawLine(originPen, this.ViewToControl(new Point(-100, 0)), this.ViewToControl(new Point(100, 0)));
            //e.Graphics.DrawLine(originPen, this.ViewToControl(new Point(0, -100)), this.ViewToControl(new Point(0, 100)));

            //e.Graphics.DrawBezier(originPen, this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxOrigin), this.ViewToControl(this.m_SelectBoxCurrent), this.ViewToControl(this.m_SelectBoxCurrent));
        }

        private void NodeGraphPanel_Resize(object sender, EventArgs e)
        {
            if (base.DesignMode)
                return;

            this.Setup_Viewport();

            this.Invalidate();
        }

        #endregion
    }

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
