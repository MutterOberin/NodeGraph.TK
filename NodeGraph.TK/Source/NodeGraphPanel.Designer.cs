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
            this.SuspendLayout();
            // 
            // NodeGraphPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "NodeGraphPanel";
            this.Load += new System.EventHandler(this.NodeGraphPanel_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NodeGraphPanel_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NodeGraphPanel_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.NodeGraphPanel_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NodeGraphPanel_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.NodeGraphPanel_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.NodeGraphPanel_MouseUp);
            this.Resize += new System.EventHandler(this.NodeGraphPanel_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
