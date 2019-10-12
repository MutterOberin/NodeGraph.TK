using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NodeGraph.TK.Example
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //NodeGraphPanel panel = new NodeGraphPanel();

            //panel.Dock = DockStyle.Fill;

            //this.Controls.Add(panel);

            this.timer_redraw.Enabled = true;
        }

        private void timer_redraw_Tick(object sender, EventArgs e)
        {
            nodeGraphGL1.Invalidate();
        }

        private void Btn_Node_Add_Click(object sender, EventArgs e)
        {
            nodeGraphGL1.Add_Node(new Node(64, 192, nodeGraphGL1.View));

            panel1.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (nodeGraphGL1.Graph.Nodes.Count > 0)
                nodeGraphGL1.Graph.Nodes[0].UpdateTexture(e.Graphics);
        }
    }
}
