namespace NodeGraph.TK
{
    partial class NodeGraphPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ViewGL = new NodeGraphGL();
            this.NodeGraphPanel_Debug = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // ViewGL
            // 
            this.ViewGL.BackColor = System.Drawing.Color.Black;
            this.ViewGL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ViewGL.Location = new System.Drawing.Point(0, 0);
            this.ViewGL.Name = "ViewGL";
            this.ViewGL.Size = new System.Drawing.Size(512, 512);
            this.ViewGL.TabIndex = 0;
            this.ViewGL.VSync = false;
            this.ViewGL.Load += new System.EventHandler(this.ViewGL_Load);
            this.ViewGL.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewGL_Paint);
            this.ViewGL.Resize += new System.EventHandler(this.ViewGL_Resize);
            // 
            // NodeGraphPanel_Debug
            // 
            this.NodeGraphPanel_Debug.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NodeGraphPanel_Debug.Location = new System.Drawing.Point(3, 3);
            this.NodeGraphPanel_Debug.Name = "NodeGraphPanel_Debug";
            this.NodeGraphPanel_Debug.Size = new System.Drawing.Size(200, 100);
            this.NodeGraphPanel_Debug.TabIndex = 1;
            this.NodeGraphPanel_Debug.Paint += new System.Windows.Forms.PaintEventHandler(this.NodeGraphPanel_Debug_Paint);
            // 
            // NodeGraphPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.Controls.Add(this.NodeGraphPanel_Debug);
            this.Controls.Add(this.ViewGL);
            this.Name = "NodeGraphPanel";
            this.Size = new System.Drawing.Size(512, 512);
            this.ResumeLayout(false);

        }

        #endregion

        private NodeGraphGL ViewGL;
        private System.Windows.Forms.Panel NodeGraphPanel_Debug;
    }
}
