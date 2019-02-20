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
            this.nodeGraphGL1 = new NodeGraph.TK.NodeGraphPanel();
            this.SuspendLayout();
            // 
            // nodeGraphGL1
            // 
            this.nodeGraphGL1.BackColor = System.Drawing.Color.Black;
            this.nodeGraphGL1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodeGraphGL1.EnableDrawDebug = true;
            this.nodeGraphGL1.EnableShadow = true;
            this.nodeGraphGL1.EnableSmooth = false;
            this.nodeGraphGL1.Location = new System.Drawing.Point(0, 0);
            this.nodeGraphGL1.Name = "nodeGraphGL1";
            this.nodeGraphGL1.Size = new System.Drawing.Size(800, 450);
            this.nodeGraphGL1.TabIndex = 0;
            this.nodeGraphGL1.VSync = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.nodeGraphGL1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private NodeGraphPanel nodeGraphGL1;
    }
}

