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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

using System.Xml;

using OpenTK;
using OpenTK.Graphics.OpenGL;

using NodeGraphTK.Source;

namespace NodeGraph.TK
{
    /// <summary>
    /// Represents a Base <see cref="NodeGraph.TK.Node"/> for use in a <see cref="NodeGraph.TK.View"/>
    /// </summary>
    public class Node
    {
        #region - Private Variables -

        private static uint count;

        protected uint id;

        protected Vector4 color_fill;

        protected Matrix4 tMatrix;

        //protected Vector3 position; // X,Y = Position, Z currently unused
        protected Vector2 property; // X,Y = Width, Height

        protected bool hovered;
        protected bool selected;
        protected bool selectable;

        protected string name;
        protected string comment;

        protected View view;

        protected List<Connector> connectors;

        protected Bitmap texture;
        protected int program;
        protected int loc_pr;
        protected int loc_mv;
        protected int loc_t0;
        protected int loc_st;

        protected uint tex_id;

        protected float[] attrib_0;
        protected float[] attrib_1;
        
        protected uint VAO_1;        
        protected uint VBO_ATTRIB_0;
        protected uint VBO_ATTRIB_1;
        
        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new <see cref="Node"/> into the <see cref="Graph"/>, given coordinates and ability to be selected
        /// </summary>
        public Node(float X, float Y, View view, int shader, bool selectable = true)
        {
            this.id = count; count++;

            this.X = X;
            this.Y = Y;
            this.W = 192;
            this.H = 64;

            this.Tex_Scale = 4.0f;

            this.view       = view;
            this.name       = "DI: Dummy";
            this.program    = shader;
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

            this.tMatrix = Matrix4.Identity;

            this.RenderSetup();

            this.texture = new Bitmap((int)(this.Tex_Scale * this.W), (int)(this.Tex_Scale * this.H));

            this.RenderUpdateTexture(Graphics.FromImage(this.texture));

            UploadTextureBitmap(ref this.tex_id, ref this.texture);

            this.loc_pr = GL.GetUniformLocation(this.program, "MatrixPr");
            this.loc_mv = GL.GetUniformLocation(this.program, "MatrixMV");
            this.loc_t0 = GL.GetUniformLocation(this.program, "Texture");
            this.loc_st = GL.GetUniformLocation(this.program, "State");

            //this.texture.Save(@"C:\6_Projects\Projects_FlowVis\Flow.GUI.Data\_export\bild.png");
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
            get => this.tMatrix.Row3.Xyz;
            set => this.tMatrix.Row3 = new Vector4(value, 1.0f);
        }

        [Category("Node Properties")]
        protected Matrix4 TMatrix { get => this.tMatrix; set => this.tMatrix = value; }

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
            get => this.tMatrix.M41;
            set => this.tMatrix.M41 = value;
        }

        /// <summary>
        /// Position in Y (ViewSpace) of the Node
        /// </summary>
        [Category("Node Properties")]
        public float Y
        {
            get => this.tMatrix.M42;
            set => this.tMatrix.M42 = value;
        }

        /// <summary>
        /// Width (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public float W
        {
            get => this.property.X;
            set => this.property.X = value;
        }

        /// <summary>
        /// Height (ViewSpace) of the node
        /// </summary>
        [Category("Node Properties")]
        public float H
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

        /// <summary>
        /// Internal Scaling of rendered Node Texture
        /// </summary>
        [Category("Node Properties")]
        public float Tex_Scale { get; set; }


        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the Name of the Node. Override for Custom Names.
        /// </summary>
        protected virtual string GetName()
        {
            return $"Node {this.id.ToString("00")} Name: {this.name}";
        }

        public virtual RectangleF GetArea()
        {
            return new RectangleF(this.X, this.Y, this.W, this.H);
        }

        /// <summary>
        /// Returns the Click Area of the Node. Global Coordinates.
        /// </summary>
        public virtual RectangleF GetAreaHit()
        {
            // Hit Zone Bleeding can be done here
            return new RectangleF(this.X, this.Y, this.W, this.H);
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
        public int GetConnectorCountMax()
        {
            return Math.Max(GetConnectorCount(ConnectorType.Input), GetConnectorCount(ConnectorType.Output));
        }

        /// <summary>
        /// Gets the Maximal Connector Count
        /// </summary>
        public int GetConnectorCountMin()
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
            int hs = 0;

            if (this.selected) hs = 1;
            if (this.hovered)  hs = 2;            
            
            GL.PushMatrix();
            {
                GL.MultMatrix(ref this.tMatrix);

                GL.BindTexture(TextureTarget.Texture2D, this.tex_id);

                GL.UseProgram(this.program);

                GL.GetFloat(GetPName.ModelviewMatrix,  out Matrix4 mv);
                GL.GetFloat(GetPName.ProjectionMatrix, out Matrix4 pr);
                
                // Upload Matrix
                GL.UniformMatrix4(this.loc_mv, false, ref mv);
                GL.UniformMatrix4(this.loc_pr, false, ref pr);

                // Loc t0 = texture -> 0, hs = Hovered / Selected -> 0, 1, 2
                GL.Uniform1(this.loc_t0, 0);
                GL.Uniform1(this.loc_st, hs);

                GL.BindVertexArray(this.VAO_1);

                GL.DrawArrays(PrimitiveType.Quads, 0, 4);

                GL.BindVertexArray(0);

                GL.UseProgram(0);

                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GL.PopMatrix();
        }

        public virtual void RenderSetup()
        {
            Vector3[] attrib_0 = new Vector3[4];
            Vector3[] attrib_1 = new Vector3[4];

            float w = property.X;
            float h = property.Y;

            attrib_0[0] = new Vector3(0, 0, 0);
            attrib_0[1] = new Vector3(w, 0, 0);
            attrib_0[2] = new Vector3(w, h, 0);
            attrib_0[3] = new Vector3(0, h, 0);

            attrib_1[0] = new Vector3(0, 1, 0);
            attrib_1[1] = new Vector3(1, 1, 0);
            attrib_1[2] = new Vector3(1, 0, 0);
            attrib_1[3] = new Vector3(0, 0, 0);

            GL.DeleteVertexArrays(1, ref this.VAO_1);
            GL.DeleteBuffers(1, ref this.VBO_ATTRIB_0);
            GL.DeleteBuffers(1, ref this.VBO_ATTRIB_1);

            // Buffer Generate
            Node.CreateVBO3(ref attrib_0, ref this.VBO_ATTRIB_0, 0);
            Node.CreateVBO3(ref attrib_1, ref this.VBO_ATTRIB_1, 1);
        }

        /// <summary>
        /// Updates the Texture used during Node Draw
        /// </summary>
        public virtual void RenderUpdateTexture(Graphics g)
        {
            bool shadow = false;

            g.SmoothingMode      = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            //Vector2 CtrlPos = view.Panel.ViewToControl(new Vector2(x, y));

            //float ScaledX = CtrlPos.X;
            //float ScaledY = CtrlPos.Y;

            RectangleF ViewRectangle = new RectangleF(0.0f, 0.0f, this.W * Tex_Scale, this.H * Tex_Scale);

            // Node Shadow
            if (shadow)
            {
                g.DrawImage(Resources.NodeShadow, new Rectangle(8 - (int)(0.1f * this.W * Tex_Scale), 8 - (int)(0.1f * this.H * Tex_Scale), 
                    (int)(this.W * Tex_Scale) + (int)(0.2f * this.W * Tex_Scale), (int)(this.H * Tex_Scale) + (int)(0.2f * this.H * Tex_Scale)));
            }

            
            //g.FillRectangle(new SolidBrush(this.NodeFillColor), ViewRectangle);
            //g.FillRectangle(Brushes.Transparent, ViewRectangle);

            g.FillRectangle(new SolidBrush(Util.VectorToColor(view.ColorNodeHeader)), ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, view.SizeNodeHeader * Tex_Scale);
            g.DrawRectangle(new Pen(Util.VectorToColor(view.ColorNodeOutline), Tex_Scale), ViewRectangle.X, ViewRectangle.Y, ViewRectangle.Width, ViewRectangle.Height);
           
            // Node Text
            //g.DrawString(this.Name, new Font(view.Font_Node_Title.Name, view.Font_Node_Title.Size * zoom), new SolidBrush(Util.VectorToColor(view.ColorNodeTextShadow)), 
            //    new Point(ViewRectangle.X + (int)(2.0f * zoom) + 1, ViewRectangle.Y + (int)(2.0f * zoom) + 1));

            g.DrawString(this.Name, new Font(view.Font_Node_Title.Name, view.Font_Node_Title.Size * Tex_Scale), new SolidBrush(Util.VectorToColor(view.ColorNodeText)), 
                ViewRectangle.X + 2.0f * Tex_Scale + 0, ViewRectangle.Y + (int)(2.0f * Tex_Scale) + 3);

            // Debug - Hit Rectange
            //g.DrawRectangle(new Pen(Color.DarkBlue, 1.0f), this.HitRectangle.X, this.HitRectangle.Y, this.HitRectangle.Width, this.HitRectangle.Height);

            // Connectors Input
            for (int i = 0; i < this.connectors.Count; i++)
            {
                this.connectors[i].Draw(g);
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

        #region - Methods Static -

        /// <summary>
        /// Creates Buffer and Uploads Data to GPU. Used for Setup Methods
        /// </summary>
        /// <param name="array">Data as float array (x,y,z)</param>
        /// <param name="buffer">Buffer, will be generated</param>
        /// <param name="index">Attrib Index</param>
        protected static void CreateVBO3(ref Vector3[] array, ref uint buffer, int index)
        {
            if (array == null || array.Length == 0)
                return;

            GL.GenBuffers(1, out buffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(array.Length * Vector3.SizeInBytes), array, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.EnableVertexAttribArray(index);
        }

        /// <summary>
        /// Gets the Pixel of the Current Bitmap
        /// </summary>
        protected static IntPtr GetBitmapPixels(ref Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            bitmap.UnlockBits(data);

            return data.Scan0;
        }

        /// <summary>
        /// Upload Bitmap Texture
        /// </summary>
        protected static void UploadTextureBitmap(ref uint texID, ref Bitmap texture)
        {
            GL.DeleteTextures(1, ref texID);

            GL.GenTextures(1, out texID);

            GL.BindTexture(TextureTarget.Texture2D, texID);

            if (texture != null)
            {
                if (texture.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, texture.Width, texture.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Red, PixelType.UnsignedByte, Node.GetBitmapPixels(ref texture));
                }

                if (texture.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, texture.Width, texture.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, Node.GetBitmapPixels(ref texture));
                }

                if (texture.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                {
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0,
                        OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Node.GetBitmapPixels(ref texture));
                }
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);   // X
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);   // Y

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void CompileShader(ref int program)
        {
            int svert = GL.CreateShader(ShaderType.VertexShader);
            int sfrag = GL.CreateShader(ShaderType.FragmentShader);

            string svertsource = "#version 330  \n" +
                "in layout(location = 0) vec3 in_Position; \n" +
                "in layout(location = 1) vec3 in_TexCoords; \n" +
                "out VS_OUT \n" +
                "{ \n" +
                "   vec3 TC; \n" +
                "} vs_out; \n" +
                " \n" +
                "uniform mat4 MatrixPr; \n" +
                "uniform mat4 MatrixMV; \n" +
                "void main() \n" +
                "{ \n" +
                "   vs_out.TC = in_TexCoords; \n" +
                "   gl_Position = MatrixPr * MatrixMV * vec4(in_Position, 1); \n" +
                "}";
            
            string sfragsource = "#version 330  \n" +
                "in VS_OUT \n" +
                "{ \n" +
                "   vec3 TC; \n" +
                "} fs_in; \n" +
                "out vec4 frag_color; \n" +
                "uniform sampler2D Texture; \n" +
                "uniform int State; \n" +
                "vec4 BlendBackToFront(vec4 result, vec4 color) \n" +
                "{ \n" +
                "result.rgb = (color.a) * color.rgb + (1.0 - color.a) * result.rgb;" +
                "result.a   = (color.a) * result.a + (1.0 - color.a) * result.a;" +
                "return result;" +
                "}" +
                "void main() \n" +
                "{ \n" +
                "   vec4 c0 = vec4(0.560, 0.500, 0.500, 1.0); \n" +
                "   vec4 c1 = vec4(0.627, 0.501, 0.391, 1.0); \n" +
                "   vec4 c2 = vec4(0.725, 0.580, 0.470, 1.0); \n" +
                "   vec4 base = c0; \n" +
                "   if (State == 1) base = c1; \n" +
                "   if (State == 2) base = c2; \n" +
                "   //frag_color = vec4(fs_in.TC.x, fs_in.TC.y, 0.0, 1.0); \n" +
                "   frag_color = BlendBackToFront(base, texture(Texture, fs_in.TC.xy)); \n" +
            "}";

            // Set source and compile
            GL.ShaderSource(svert, svertsource);
            GL.ShaderSource(sfrag, sfragsource);

            GL.CompileShader(svert);
            GL.CompileShader(sfrag);

            GL.GetShaderInfoLog(svert, out string svertinfo);
            GL.GetShaderInfoLog(sfrag, out string sfraginfo);

            // Delete Shader Program (if exists)
            GL.DeleteProgram(program);

            // Create Shader Program
            program = GL.CreateProgram();

            GL.AttachShader(program, svert);
            GL.AttachShader(program, sfrag);

            GL.LinkProgram(program);
        }

        #endregion
    }
}
