using OpenTK;
using OpenTK.Graphics;

namespace NodeGraph.TK
{
    public partial class NodeGraphGL : GLControl
    {
        public NodeGraphGL()
            : base(new GraphicsMode(new ColorFormat(32), 24, 8, 8), 4, 5, GraphicsContextFlags.ForwardCompatible)
        {
            InitializeComponent();
        }
    }
}
