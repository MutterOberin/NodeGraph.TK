namespace NodeGraph.TK.Example
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            NodeGraph.TK.NodeGraphGraph nodeGraphGraph3 = new NodeGraph.TK.NodeGraphGraph();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.nodeGraphPanel1 = new NodeGraph.TK.NodeGraphPanel();
            this.SuspendLayout();
            // 
            // nodeGraphPanel1
            // 
            this.nodeGraphPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.nodeGraphPanel1.ConnectorFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeGraphPanel1.ConnectorFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.nodeGraphPanel1.ConnectorHitZoneBleed = 2;
            this.nodeGraphPanel1.ConnectorOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.nodeGraphPanel1.ConnectorOutlineSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.nodeGraphPanel1.ConnectorTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.nodeGraphPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeGraphPanel1.DrawShadow = true;
            this.nodeGraphPanel1.EnableDrawDebug = true;
            this.nodeGraphPanel1.Graph = nodeGraphGraph3;
            this.nodeGraphPanel1.GridAlpha = 0.125F;
            this.nodeGraphPanel1.GridPadding = 64F;
            this.nodeGraphPanel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nodeGraphPanel1.LinkEditableColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.nodeGraphPanel1.LinkHardness = 2F;
            this.nodeGraphPanel1.LinkVisualStyle = NodeGraph.TK.LinkVisualStyle.Curve;
            this.nodeGraphPanel1.Location = new System.Drawing.Point(0, 0);
            this.nodeGraphPanel1.Name = "nodeGraphPanel1";
            this.nodeGraphPanel1.NodeConnectorFont = new System.Drawing.Font("Tahoma", 7F);
            this.nodeGraphPanel1.NodeConnectorTextZoomTreshold = 0.8F;
            this.nodeGraphPanel1.NodeFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.nodeGraphPanel1.NodeFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(128)))), ((int)(((byte)(100)))));
            this.nodeGraphPanel1.NodeHeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeGraphPanel1.NodeHeaderSize = 24;
            this.nodeGraphPanel1.NodeOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.nodeGraphPanel1.NodeOutlineSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(160)))), ((int)(((byte)(128)))));
            this.nodeGraphPanel1.NodeScaledConnectorFont = new System.Drawing.Font("Tahoma", 7F);
            this.nodeGraphPanel1.NodeScaledTitleFont = new System.Drawing.Font("Tahoma", 8F);
            this.nodeGraphPanel1.NodeSignalInvalidColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeGraphPanel1.NodeSignalValidColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.nodeGraphPanel1.NodeTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.nodeGraphPanel1.NodeTextShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.nodeGraphPanel1.NodeTitleFont = new System.Drawing.Font("Tahoma", 8F);
            this.nodeGraphPanel1.NodeTitleZoomThreshold = 0.5F;
            this.nodeGraphPanel1.SelectionFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(90)))), ((int)(((byte)(30)))));
            this.nodeGraphPanel1.SelectionOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))), ((int)(((byte)(60)))));
            this.nodeGraphPanel1.ShowGrid = true;
            this.nodeGraphPanel1.Size = new System.Drawing.Size(536, 448);
            this.nodeGraphPanel1.SmoothBehavior = false;
            this.nodeGraphPanel1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private NodeGraphPanel nodeGraphPanel1;
    }
}

