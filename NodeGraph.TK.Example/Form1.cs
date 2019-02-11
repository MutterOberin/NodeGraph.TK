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

            NodeGraphPanel.AddNode(new NodeGraphNode(0, 0, NodeGraphPanel.View, true));
        }
    }
}
