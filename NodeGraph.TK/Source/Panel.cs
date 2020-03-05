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

        private Vector3 selectionBox_Origin;
        private Vector3 selectionBox_Current;
        private Vector4 selectionBox;

        private Vector3 mouse_position_ctrl;
        private Vector3 mouse_position_wrld;

        private Vector3 mouse_position_last;

        private bool enable_debug;
        private bool enable_shadow;
        private bool enable_smooth;

        // For Linking
        private Connector link_input;
        private Connector link_output;

        private bool key_down_alt;
        private bool key_down_ctrl;

        private NodeGraphEditMode editMode;

        private View view;
        private Graph graph;

        private Link tempLink;
        private Node tempNode;

        private int shaderNodes;
        private int shaderLinks;

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

            this.MouseWheel += NodeGraphPanel_MouseWheel;

            //this.label1.Parent = this;

            this.graph = new Graph("Default");
            this.view  = new View(graph);

            this.Dock = DockStyle.Fill;
            //this.DoubleBuffered = true;

            this.scroll_Margins       = 32;
            this.scroll_Margins_Value = 10;

            this.editMode = NodeGraphEditMode.Idle;

            this.mouse_position_wrld = Vector3.Zero;
            this.mouse_position_ctrl = Vector3.Zero;

            this.key_down_alt  = false;
            this.key_down_ctrl = false;

            this.scroll_last = Vector3.Zero;

            this.selectionBox_Origin  = Vector3.Zero;
            this.selectionBox_Current = Vector3.Zero;

            this.link_input  = null;
            this.link_output = null;

            this.enable_debug  = true;
            this.enable_shadow = true;
            this.enable_smooth = false;
        }

        #endregion

        #region - Properties -

        public View View
        {
            get { return this.view; }
            set { this.view = value; }
        }

        public Graph Graph
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

        [Category("NodeGraph Panel")]
        public int ShaderProgramNodes { get => this.shaderNodes; set => this.shaderNodes = value; }

        [Category("NodeGraph Panel")]
        public int ShaderProgramLinks { get => this.shaderLinks; set => this.shaderLinks = value; }

        #endregion

        #region - Methods -

        /// <summary>
        /// Adds a node to the current NodeGraphGraph
        /// </summary>
        public void Add_Node(Node node)
        {
            this.graph.Nodes.Add(node);

            this.Invalidate();
        }

        /// <summary>
        /// Adds a link to the current NodeGraphGraph
        /// </summary>
        public void Add_Link(Link link)
        {
            this.graph.Links.Add(link);

            this.LinkCreated?.Invoke(null, new NodeGraphPanelLinkEventArgs(link));

            this.Invalidate();
        }

        /// <summary>
        /// Draws the currently edited link
        /// </summary>
        /// <param name="e"></param>
        private void Render_Link_Edit()
        {
            if (this.editMode == NodeGraphEditMode.Linking)
            {
                RectangleF rect_0 = this.link_input.GetAreaHit();

                Vector3 pos_0 = new Vector3(rect_0.X + 0.5f * rect_0.Width, rect_0.Y + 0.5f * rect_0.Height, 0);
                Vector3 pos_1 = this.mouse_position_wrld;
                Vector3 pos_2 = pos_0 + 0.5f * (pos_1 - pos_0);

                GL.Color4(Util.VectorToColor(this.view.ColorLinkEdit));

                switch (this.view.LinkVisualStyle)
                {
                    case LinkVisualStyle.Curve:

                        //e.Graphics.DrawBezier(this.m_LinkEditable, v_StartPos, v_StartPosBezier, v_EndPosBezier, v_EndPos);
                        //break;

                    case LinkVisualStyle.Direct:

                        //e.Graphics.DrawLine(this.m_LinkEditable, v_StartPos, v_EndPos);
                        //break;

                    case LinkVisualStyle.Rectangle:

                        GL.Begin(PrimitiveType.LineStrip);

                        GL.Vertex3(pos_0.X, pos_0.Y, pos_0.Z);
                        GL.Vertex3(pos_2.X, pos_0.Y, pos_0.Z);
                        GL.Vertex3(pos_2.X, pos_1.Y, pos_0.Z);
                        GL.Vertex3(pos_1.X, pos_1.Y, pos_0.Z);

                        GL.End();

                        break;

                    default:
                        break;

                }
            }
        }

        private void Render_Node_Edit()
        {
            if (this.tempNode != null)
                this.tempNode.Render();
        }

        /// <summary>
        /// Draws all links already created
        /// </summary>
        /// <param name="e"></param>
        private void Render_Links()
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
        /// Behavior when Mouse Wheel is turned
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                float newViewZoom = this.view.ViewZoom + ((float)e.Delta * 0.002f);

                if (newViewZoom > 0.1f && newViewZoom < 2.5f)
                    this.view.ViewZoom = newViewZoom;

                this.Setup_Camera();

                this.mouse_position_ctrl = new Vector3(e.Location.X, e.Location.Y, 0);

                Util.Unproject(ref this.mouse_position_ctrl, out this.mouse_position_wrld);

            }

            if (this.editMode == NodeGraphEditMode.SelectingBox)
            {                
                this.selectionBox_Current = this.mouse_position_wrld;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Behavior when Mouse is Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeGraphPanel_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouse_position_ctrl = new Vector3(e.Location.X, e.Location.Y, 0);

            switch (this.editMode)
            {
                case NodeGraphEditMode.Idle:
                    switch (e.Button)
                    {
                        case MouseButtons.Middle:

                            this.editMode = NodeGraphEditMode.Scrolling;
                            this.mouse_position_last = this.mouse_position_wrld;

                            this.scroll_last.X = e.Location.X;
                            this.scroll_last.Y = e.Location.Y;

                            break;
                        case MouseButtons.Left:

                            var result1 = this.HitTestNodes(this.mouse_position_wrld);
                            var result2 = this.HitTestNodes(this.mouse_position_wrld, true);

                            if (result1.Item1 == HitType.Connector)
                            {
                                //if (ModifierKeys == Keys.Alt)
                                if (!key_down_alt)
                                {
                                    this.editMode    = NodeGraphEditMode.Linking;
                                    this.link_input  = result1.Item2;
                                    this.link_output = null;
                                }
                                else
                                {
                                    this.Delete_LinkConnectors(result1.Item2);
                                }

                            }
                            // Selection is not present => Move
                            else if (this.graph.GetNodeCount(true) == 0 && result1.Item1 == HitType.Node)
                            {
                                //this.selectionBox_Current = this.ControlToView(this.mouse_position_ctrl);
                                //this.selectionBox_Origin = this.ControlToView(this.mouse_position_ctrl);

                                //this.UpdateSelection();
                                //this.TriggerEvents();

                                this.editMode = NodeGraphEditMode.MovingSelection;

                                this.mouse_position_last = this.mouse_position_wrld;
                            }

                            // Selection is present => Move Existing Selection Arround
                            else if (this.graph.GetNodeCount(true) >= 1 && result2.Item1 == HitType.Node)
                            {
                                this.editMode = NodeGraphEditMode.MovingSelection;

                                this.mouse_position_last = this.mouse_position_wrld;
                            }

                            else
                            {
                                this.editMode = NodeGraphEditMode.Selecting;

                                this.selectionBox_Current = Util.Unproject(ref this.mouse_position_ctrl);
                                this.selectionBox_Origin  = Util.Unproject(ref this.mouse_position_ctrl);
                                this.Selection_Update();
                                this.Selection_Events();

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
                    break;

                case NodeGraphEditMode.SelectingBox:

                    if (e.Button == MouseButtons.Left)
                    {
                        this.Selection_Events();
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

                    var result1 = this.HitTestNodes(this.mouse_position_wrld);

                    if (result1.Item1 == HitType.Connector)
                    {
                        this.link_output = result1.Item2;

                        this.Link_Validate();
                    }

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
            this.mouse_position_wrld = Util.Unproject(ref this.mouse_position_ctrl);

            //if (e.Button == MouseButtons.Left)
            //    this.Invalidate();

            //if (e.Button == MouseButtons.Right)
            //    this.Refresh();

            switch (this.editMode)
            {
                case NodeGraphEditMode.Scrolling:

                    ////This is all in Control Space
                    //this.view.ViewX -= (int)((this.mouse_position_wrld.X - this.mouse_position_last.X) * this.view.ViewZoom);
                    //this.view.ViewY += (int)((this.mouse_position_wrld.Y - this.mouse_position_last.Y) * this.view.ViewZoom);

                    //this.mouse_position_last = this.mouse_position_wrld;

                    // This is all in Control Space
                    this.view.ViewX -= (int)((e.Location.X - this.scroll_last.X) * this.view.ViewZoom);
                    this.view.ViewY += (int)((e.Location.Y - this.scroll_last.Y) * this.view.ViewZoom);

                    this.scroll_last.X = e.Location.X;
                    this.scroll_last.Y = e.Location.Y;

                    this.Invalidate();

                    break;

                case NodeGraphEditMode.Selecting:

                    this.editMode = NodeGraphEditMode.SelectingBox;

                    this.selectionBox_Current = this.mouse_position_wrld;

                    this.Selection_Update();

                    break;

                case NodeGraphEditMode.SelectingBox:

                    if (this.IsInScrollArea(new Vector2(e.Location.X, e.Location.Y)))
                    {
                        this.UpdateScroll(new Vector2(e.Location.X, e.Location.Y));
                    }

                    this.selectionBox_Current = this.mouse_position_wrld;

                    this.Selection_Update();

                    break;

                case NodeGraphEditMode.MovingSelection:

                    if (this.IsInScrollArea(new Vector2(e.Location.X, e.Location.Y)))
                    {
                        this.UpdateScroll(new Vector2(e.Location.X, e.Location.Y));
                    }

                    Vector3 delta = this.mouse_position_wrld - this.mouse_position_last;

                    this.Selection_Move(delta);
                    
                    this.mouse_position_last = this.mouse_position_wrld;

                    this.Refresh();

                    break;

                case NodeGraphEditMode.Linking:

                    if (this.IsInScrollArea(new Vector2(e.Location.X, e.Location.Y)))
                    {
                        this.UpdateScroll(new Vector2(e.Location.X, e.Location.Y));
                    }                    

                    break;

                case NodeGraphEditMode.Idle:

                    foreach (Node node in this.graph.Nodes)
                    {
                        if (node.HitTest(this.mouse_position_wrld, out HitType hitType, out Connector connector))
                        {
                            node.Hovered = true;
                        }   
                        else
                        {
                            node.Hovered = false;
                        }
                    }

                    break;

                default:                    

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
        private void Selection_Update()
        {
            RectangleF ViewRectangle = new RectangleF();

            if (this.selectionBox_Origin.X > this.selectionBox_Current.X)
            {
                ViewRectangle.X = this.selectionBox_Current.X;
                ViewRectangle.Width = this.selectionBox_Origin.X - this.selectionBox_Current.X;
            }
            else
            {
                ViewRectangle.X = this.selectionBox_Origin.X;
                ViewRectangle.Width = this.selectionBox_Current.X - this.selectionBox_Origin.X;
            }
            if (this.selectionBox_Origin.Y > this.selectionBox_Current.Y)
            {
                ViewRectangle.Y = this.selectionBox_Current.Y;
                ViewRectangle.Height = this.selectionBox_Origin.Y - this.selectionBox_Current.Y;
            }
            else
            {
                ViewRectangle.Y = this.selectionBox_Origin.Y;
                ViewRectangle.Height = this.selectionBox_Current.Y - this.selectionBox_Origin.Y;
            }

            foreach (Node node in this.graph.Nodes)
            {
                //Console.WriteLine(ModifierKeys == Keys.Control);

                if (ModifierKeys == Keys.Control && ModifierKeys != Keys.Alt)
                {
                    if (node.HitRectangle.IntersectsWith(ViewRectangle) && node.Selectable)
                        node.Selected = true;
                }

                if (ModifierKeys != Keys.Control && ModifierKeys == Keys.Alt)
                {
                    if (node.HitRectangle.IntersectsWith(ViewRectangle) && node.Selectable)
                        node.Selected = false;
                }

                if (ModifierKeys != Keys.Control && ModifierKeys != Keys.Alt)
                {
                    if (node.HitRectangle.IntersectsWith(ViewRectangle) && node.Selectable && ViewRectangle.Width > 1 && ViewRectangle.Height > 1)
                        node.Selected = true;
                    else
                        node.Selected = false;
                }
            }
        }

        /// <summary>
        /// Moves the selection, given an offset
        /// </summary>
        /// <param name="offset"></param>
        private void Selection_Move(Vector3 offset)
        {
            foreach (Node node in this.graph.Nodes)
            {
                if (!node.Selected && !node.Hovered)
                    continue;

                node.Position += offset;
            }
        }

        /// <summary>
        /// Creates a selection of NodeGraphNodes depending of the click or selection rectangle
        /// </summary>
        private void Selection_Events()
        {
            if (this.graph.GetNodeCount(true) >= 1)
            {
                SelectionChanged?.Invoke(this, new NodeGraphPanelSelectionEventArgs(this.graph.GetNodeCount(true)));
            }
            if (this.graph.GetNodeCount(true) == 0)
            {
                SelectionCleared?.Invoke(this, new NodeGraphPanelSelectionEventArgs(this.graph.GetNodeCount(true)));
            }
        }

        /// <summary>
        /// Returns a HitType depending on what Hit the cursor within the All items
        /// </summary>
        /// <param name="p_CursorLocation"></param>
        /// <returns></returns>
        private Tuple<HitType, Connector> HitTestNodes(Vector3 location, bool selected = false)
        {
            HitType hitType = HitType.None;

            Connector connector = null;

            foreach (Node node in this.graph.Nodes)
            {
                if (!node.Selected && selected)
                    continue;

                if (node.HitTest(location, out hitType, out connector))
                    break;
            }

            return new Tuple<HitType, Connector>(hitType, connector);
        }

        ///// <summary>
        ///// Gets the connector associated to the mouse hit
        ///// </summary>
        ///// <param name="p_CursorLocation"></param>
        ///// <returns></returns>
        //private Connector GetHitConnector(Vector3 location)
        //{
        //    //NodeGraphConnector v_OutConnector = null;

        //    //Rectangle HitTest = new Rectangle(this.ControlToView(p_CursorLocation), new Size());

        //    //foreach (NodeGraphNode i_Node in this.graph.Nodes)
        //    //{
        //    //    if (HitTest.IntersectsWith(i_Node.HitRectangle))
        //    //    {
        //    //        return i_Node.GetConnectorMouseHit(p_CursorLocation);
        //    //    }
        //    //}

        //    //return v_OutConnector;

        //    return null;
        //}

        /// <summary>
        /// Validates a link being edited
        /// </summary>
        private void Link_Validate()
        {
            if (this.link_input  != null &&
                this.link_output != null &&
                this.link_input  != this.link_output &&
                this.link_input.Type != this.link_output.Type &&
                this.link_input.Data == this.link_output.Data)
            {
                if (this.link_input.Type == ConnectorType.Output)
                {
                    if (this.link_output.Linked)
                        Delete_LinkConnectors(link_output);

                    // Create Link
                    Link link = new Link(this.link_input, this.link_output, this.view);

                    // Add Link to View
                    this.graph.Links.Add(link);

                    // Fire Event
                    LinkCreated?.Invoke(null, new NodeGraphPanelLinkEventArgs(link));
                }
                else
                {
                    if (this.link_input.Linked)
                        Delete_LinkConnectors(this.link_input);

                    // Create Link
                    Link link = new Link(this.link_output, this.link_input, this.view);

                    // Add Link to View
                    this.graph.Links.Add(link);

                    // Fire Event
                    LinkCreated?.Invoke(null, new NodeGraphPanelLinkEventArgs(link));
                }
            }

            this.link_input  = null;
            this.link_output = null;
        }

        /// <summary>
        /// Returns the other end of a connector
        /// </summary>
        public Connector GetLink(Connector connector)
        {
            // Get the Correct Link
            Link link = this.graph.Links.Find(l => l.Input == connector || l.Output == connector);

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
        /// Deletes a link, given a connector
        /// </summary>
        /// <param name="con"></param>
        public void Delete_LinkConnectors(Connector con)
        {
            List<Link> linksToDelete = new List<Link>();

            foreach (Link link in this.graph.Links)
            {
                if (link.Input == con || link.Output == con)
                {
                    linksToDelete.Add(link);
                }
            }

            foreach (Link link in linksToDelete)
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

            Node.CompileShader(ref this.shaderNodes);

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

            //Matrix4 lookAt = Matrix4.LookAt(this.ClientSize.Width / 2, this.ClientSize.Height / 2, 1,
            //                                this.ClientSize.Width / 2, this.ClientSize.Height / 2, 0,
            //                                0, 1, 0);

            Matrix4 lookAt = Matrix4.LookAt(this.view.ViewX, this.view.ViewY, 1,
                                            this.view.ViewX, this.view.ViewY, 0,
                                            0, 1, 0);

            GL.MultMatrix(ref lookAt);
        }

        private void Render_Grid()
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

        private void Render_SelectionBox()
        {
            if (this.editMode == NodeGraphEditMode.SelectingBox)
            {
                if (this.selectionBox_Origin.X > this.selectionBox_Current.X)
                {
                    selectionBox.X = this.selectionBox_Current.X;
                    selectionBox.Z = this.selectionBox_Origin.X - this.selectionBox_Current.X;
                }
                else
                {
                    selectionBox.X = this.selectionBox_Origin.X;
                    selectionBox.Z = this.selectionBox_Current.X - this.selectionBox_Origin.X;
                }
                if (this.selectionBox_Origin.Y > this.selectionBox_Current.Y)
                {
                    selectionBox.Y = this.selectionBox_Current.Y;
                    selectionBox.W = this.selectionBox_Origin.Y - this.selectionBox_Current.Y;
                }
                else
                {
                    selectionBox.Y = this.selectionBox_Origin.Y;
                    selectionBox.W = this.selectionBox_Current.Y - this.selectionBox_Origin.Y;
                }

                GL.Color4(this.view.ColorSelectionFill);

                GL.Begin(PrimitiveType.Quads);

                GL.Vertex3(selectionBox.X, selectionBox.Y, 0);
                GL.Vertex3(selectionBox.X + selectionBox.Z, selectionBox.Y, 0);
                GL.Vertex3(selectionBox.X + selectionBox.Z, selectionBox.Y + selectionBox.W, 0);
                GL.Vertex3(selectionBox.X, selectionBox.Y + selectionBox.W, 0);

                GL.End();

                GL.Color4(this.view.ColorSelectionOutline);

                GL.Begin(PrimitiveType.LineLoop);

                GL.Vertex3(selectionBox.X, selectionBox.Y, 0);
                GL.Vertex3(selectionBox.X + selectionBox.Z, selectionBox.Y, 0);
                GL.Vertex3(selectionBox.X + selectionBox.Z, selectionBox.Y + selectionBox.W, 0);
                GL.Vertex3(selectionBox.X, selectionBox.Y + selectionBox.W, 0);

                GL.End();
            }
        }

        private void Render_Debug()
        {
            GL.Color3(Color.LightSkyBlue);

            GL.PointSize(3);

            GL.Begin(PrimitiveType.Points);

            GL.Vertex3(this.mouse_position_wrld.X, this.mouse_position_wrld.Y, 0);

            GL.End();

            //GL.Begin(PrimitiveType.Points);

            //GL.Vertex3(64, 64, 0);

            //GL.End();

            GL.PointSize(1);
        }

        private void Render_Debug_RefreshTick()
        {
            GL.Color3(Color.LightSkyBlue);

            Vector3 pos_screen1 = Vector3.Zero;
            Vector3 pos_screen2 = Vector3.Zero;
            Vector3 pos_worlds1 = Vector3.Zero;
            Vector3 pos_worlds2 = Vector3.Zero;

            pos_screen1.X = this.ClientSize.Width  - 75;
            pos_screen1.Y = this.ClientSize.Height - 20;

            pos_screen2.X = this.ClientSize.Width  -  5;
            pos_screen2.Y = this.ClientSize.Height -  5;

            Util.Unproject(ref pos_screen1, out pos_worlds1);
            Util.Unproject(ref pos_screen2, out pos_worlds2);

            GL.Begin(PrimitiveType.LineLoop);

            GL.Vertex3(pos_worlds1.X, pos_worlds1.Y, 0);
            GL.Vertex3(pos_worlds1.X, pos_worlds2.Y, 0);
            GL.Vertex3(pos_worlds2.X, pos_worlds2.Y, 0);
            GL.Vertex3(pos_worlds2.X, pos_worlds1.Y, 0);

            GL.End();

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(pos_worlds1.X + this.gl_tick / 100.0f * (pos_worlds2.X - pos_worlds1.X), pos_worlds1.Y, 0);
            GL.Vertex3(pos_worlds1.X + this.gl_tick / 100.0f * (pos_worlds2.X - pos_worlds1.X), pos_worlds2.Y, 0);

            GL.End();

            this.gl_tick++;
            this.gl_tick = this.gl_tick % 100;
        }

        private void NodeGraphPanel_Load(object sender, EventArgs e)
        {
            if (base.DesignMode)
                return;
            
            this.Setup();
            this.Setup_Viewport();
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

            //this.DrawBackground?.Invoke(this, e);

            if (this.View.Grid_Enable)
                this.Render_Grid();

            // Select Box
            this.Render_SelectionBox();

            foreach (Node node in this.graph.Nodes)
            {
                node.Render();
            }

            foreach (Link link in this.graph.Links)
            {
                link.Render();
            }

            Render_Link_Edit();
            Render_Node_Edit();

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

            float zoom = 1.0f;

            //Bitmap bitmap = new Bitmap((int)(this.Width * zoom), (int)(this.Height * zoom));

            //Graphics g = Graphics.FromImage(bitmap);

            //foreach (Node node in this.graph.Nodes)
            //{
            //    node.UpdateTexture(g);
            //}

            //bitmap.Save(@"C:\6_Projects\Projects_Other\NodeGraph.TK\test.png");

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
            this.Render_Debug();
            this.Render_Debug_RefreshTick();

            label1.Text = $"MousePosition Ctrl: {this.mouse_position_ctrl.X}, {this.mouse_position_ctrl.Y}";
            label2.Text = $"MousePosition View: {this.mouse_position_wrld.X}, {this.mouse_position_wrld.Y}";


            label3.Text = $"Edit Mode: {this.editMode.ToString()}";
            label4.Text = $"ViewX: {this.view.ViewX.ToString()}";
            label5.Text = $"ViewY: {this.view.ViewY.ToString()}";
            label6.Text = $"ViewZoom: {this.view.ViewZoom.ToString()}";

            label7.Text = $"ViewSpace Cursor Location: {this.mouse_position_wrld.X.ToString()} : {this.mouse_position_wrld.Y.ToString()}";
            label8.Text = $"AltPressed: {this.key_down_alt.ToString()}";

            // BELOW: DEBUG ELEMENTS

            GL.Color3(Color.LightSkyBlue);

            GL.Begin(PrimitiveType.Lines);

            GL.Vertex3(-1 * this.view.Grid_Padding, 0, 0);
            GL.Vertex3(+1 * this.view.Grid_Padding, 0, 0);

            GL.Vertex3(0, -1 * this.view.Grid_Padding, 0);
            GL.Vertex3(0, +1 * this.view.Grid_Padding, 0);

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

        #region - - Drag & Drop - -

        private void NodeGraphPanel_DragEnter(object sender, DragEventArgs e)
        {
            // Enter Panel Area: Create Temporary Node

            Point p = (sender as NodeGraphPanel).PointToClient(new Point(e.X, e.Y));

            this.mouse_position_ctrl.X = p.X;
            this.mouse_position_ctrl.Y = p.Y;

            Util.Unproject(ref this.mouse_position_ctrl, out this.mouse_position_wrld);

            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Copy;

                this.tempNode = new Node(0, 0, this.view, this.shaderNodes);

                tempNode.X = this.mouse_position_wrld.X - tempNode.W / 2.0f;
                tempNode.Y = this.mouse_position_wrld.Y - tempNode.H / 2.0f;
            }
        }

        private void NodeGraphPanel_DragLeave(object sender, EventArgs e)
        {
            // Leave Panel Area: Delete Temporary Node

            this.tempNode = null;
        }

        private void NodeGraphPanel_DragOver(object sender, DragEventArgs e)
        {
            // Move during DragDrop: Move Temporary Node + Refresh

            Point p = (sender as NodeGraphPanel).PointToClient(new Point(e.X, e.Y));

            this.mouse_position_ctrl.X = p.X;
            this.mouse_position_ctrl.Y = p.Y;

            Util.Unproject(ref this.mouse_position_ctrl, out this.mouse_position_wrld);

            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                this.tempNode.X = this.mouse_position_wrld.X - this.tempNode.W / 2.0f;
                this.tempNode.Y = this.mouse_position_wrld.Y - this.tempNode.H / 2.0f;
            }

            this.Invalidate();
        }

        private void NodeGraphPanel_DragDrop(object sender, DragEventArgs e)
        {
            // Drop to final Position

            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                this.Add_Node(this.tempNode);

                this.tempNode = null;             
            }
        }

        #endregion 

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
        public List<Link> Links;

        public NodeGraphPanelLinkEventArgs(Link link)
        {
            this.Links = new List<Link>();

            this.Links.Add(link);
        }

        public NodeGraphPanelLinkEventArgs(List<Link> links)
        {
            this.Links = links;
        }
    }

    #endregion
}
