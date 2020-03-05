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
        public List<string> items;

        public Form1()
        {
            InitializeComponent();

            //NodeGraphPanel panel = new NodeGraphPanel();

            //panel.Dock = DockStyle.Fill;

            //this.Controls.Add(panel);

            this.timer_redraw.Enabled = true;

            this.items = new List<string>();

            this.items.Add("DI Item1 2D");
            this.items.Add("DI Item1 3D");
            this.items.Add("DI Item2 2D");

            this.items.Add("OI Item 2D");
            this.items.Add("OI Item 3D");
            this.items.Add("OI Item 4D");

            LstBx_Items.Items.AddRange(this.items.ToArray());
        }

        private void timer_redraw_Tick(object sender, EventArgs e)
        {
            nodeGraphGL1.Invalidate();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //if (nodeGraphGL1.Graph.Nodes.Count > 0)
            //    nodeGraphGL1.Graph.Nodes[0].Render_Update_Texture(e.Graphics);
        }
         
        private void Btn_Node_Add_Click(object sender, EventArgs e)
        {
            nodeGraphGL1.Add_Node(new Node(64, 192, nodeGraphGL1.View, nodeGraphGL1.ShaderProgramNodes));

            panel1.Refresh();
        }

        private void Btn_Node_Add_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;

            btn.DoDragDrop("DI Test", DragDropEffects.Copy | DragDropEffects.Move);

            // use QueryContinueDrag to do autoconnect to other nodes while dragging and holding Ctrl

            // From here any form which has AllowDrop = true, DragEnter, DragDrop events
        }

        private void TxtBx_Filter_TextChanged(object sender, EventArgs e)
        {
            LstBx_Items.Items.Clear();

            LstBx_Items.Items.AddRange(this.items.FindAll(n => n.ToLower().Contains(TxtBx_Filter.Text.ToLower())).ToArray()); 
        }

        private void LstBx_Items_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LstBx_Items_MouseDown(object sender, MouseEventArgs e)
        {
            ListBox box = sender as ListBox;

            if (box.SelectedItem != null)
                box.DoDragDrop(box.SelectedItem, DragDropEffects.Copy | DragDropEffects.Move);
        }
    }
}
