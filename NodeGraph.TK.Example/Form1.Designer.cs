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
            this.NodeGraphPanel = new NodeGraph.TK.NodeGraphPanel();
            this.SuspendLayout();
            // 
            // NodeGraphPanel
            // 
            this.NodeGraphPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NodeGraphPanel.ConnectorFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NodeGraphPanel.ConnectorFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.NodeGraphPanel.ConnectorHitZoneBleed = 2;
            this.NodeGraphPanel.ConnectorOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.NodeGraphPanel.ConnectorOutlineSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NodeGraphPanel.ConnectorTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NodeGraphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NodeGraphPanel.DrawShadow = true;
            this.NodeGraphPanel.EnableDrawDebug = false;
            this.NodeGraphPanel.GridAlpha = ((byte)(32));
            this.NodeGraphPanel.GridPadding = 256;
            this.NodeGraphPanel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.NodeGraphPanel.LinkEditableColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.NodeGraphPanel.LinkHardness = 2F;
            this.NodeGraphPanel.LinkVisualStyle = NodeGraph.TK.LinkVisualStyle.Curve;
            this.NodeGraphPanel.Location = new System.Drawing.Point(0, 0);
            this.NodeGraphPanel.Name = "NodeGraphPanel";
            this.NodeGraphPanel.NodeConnectorFont = new System.Drawing.Font("Tahoma", 7F);
            this.NodeGraphPanel.NodeConnectorTextZoomTreshold = 0.8F;
            this.NodeGraphPanel.NodeFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.NodeGraphPanel.NodeFillSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(128)))), ((int)(((byte)(100)))));
            this.NodeGraphPanel.NodeHeaderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NodeGraphPanel.NodeHeaderSize = 24;
            this.NodeGraphPanel.NodeOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.NodeGraphPanel.NodeOutlineSelectedColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(160)))), ((int)(((byte)(128)))));
            this.NodeGraphPanel.NodeScaledConnectorFont = new System.Drawing.Font("Tahoma", 7F);
            this.NodeGraphPanel.NodeScaledTitleFont = new System.Drawing.Font("Tahoma", 8F);
            this.NodeGraphPanel.NodeSignalInvalidColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NodeGraphPanel.NodeSignalValidColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(0)))));
            this.NodeGraphPanel.NodeTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.NodeGraphPanel.NodeTextShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NodeGraphPanel.NodeTitleFont = new System.Drawing.Font("Tahoma", 8F);
            this.NodeGraphPanel.NodeTitleZoomThreshold = 0.5F;
            this.NodeGraphPanel.SelectionFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(128)))), ((int)(((byte)(90)))), ((int)(((byte)(30)))));
            this.NodeGraphPanel.SelectionOutlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(180)))), ((int)(((byte)(60)))));
            this.NodeGraphPanel.ShowGrid = true;
            this.NodeGraphPanel.Size = new System.Drawing.Size(1184, 761);
            this.NodeGraphPanel.SmoothBehavior = false;
            this.NodeGraphPanel.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.NodeGraphPanel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private NodeGraphPanel NodeGraphPanel;
    }
}

