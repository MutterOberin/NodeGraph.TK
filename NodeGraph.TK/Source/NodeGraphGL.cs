using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;

namespace NodeGraph.TK.Source
{
    public class NodeGraphGL : GLControl 
    {
        public NodeGraphGL()
            : base(new GraphicsMode(new ColorFormat(32), 24, 8, 8), 4, 5, GraphicsContextFlags.ForwardCompatible)
        {

        }
    }
}
