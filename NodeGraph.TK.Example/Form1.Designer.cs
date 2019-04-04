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
            this.components = new System.ComponentModel.Container();
            this.timer_redraw = new System.Windows.Forms.Timer(this.components);
            this.nodeGraphGL1 = new NodeGraph.TK.NodeGraphPanel();
            this.Btn_Node_Add = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timer_redraw
            // 
            this.timer_redraw.Interval = 16;
            this.timer_redraw.Tick += new System.EventHandler(this.timer_redraw_Tick);
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
            this.nodeGraphGL1.VSync = true;
            // 
            // Btn_Node_Add
            // 
            this.Btn_Node_Add.Location = new System.Drawing.Point(713, 415);
            this.Btn_Node_Add.Name = "Btn_Node_Add";
            this.Btn_Node_Add.Size = new System.Drawing.Size(75, 23);
            this.Btn_Node_Add.TabIndex = 1;
            this.Btn_Node_Add.Text = "Add Node";
            this.Btn_Node_Add.UseVisualStyleBackColor = true;
            this.Btn_Node_Add.Click += new System.EventHandler(this.Btn_Node_Add_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Btn_Node_Add);
            this.Controls.Add(this.nodeGraphGL1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private NodeGraphPanel nodeGraphGL1;
        private System.Windows.Forms.Timer timer_redraw;
        private System.Windows.Forms.Button Btn_Node_Add;
    }
}

