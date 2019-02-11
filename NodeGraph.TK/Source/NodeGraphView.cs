/*

Copyright (c) 2019, Wito Engelke
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the 
following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer 
          in the documentation and/or other materials provided with the distribution.
        * Neither the name of PeeWeeK.NET nor the names of its contributors may be used to endorse or promote products derived from this 
          software without specific prior written permission.


THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

Inspired by Project: https://github.com/peeweek/NodeGraph/tree/master/NodeGraphControl

*/
using OpenTK;

namespace NodeGraph.TK
{
    /// <summary>
    /// Encapsulates a node view
    /// </summary>
    public class NodeGraphView
    {
        #region - Private Variables -

        private Vector4 viewProp;

        private NodeGraphGraph graph;

        private NodeGraphPanel panel;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphView in a NodeGraphpanel
        /// </summary>
        public NodeGraphView(NodeGraphPanel panel, NodeGraphGraph graph)
        {
            this.graph           = graph;
            this.ViewX           = 0;
            this.ViewY           = 0;
            this.ViewZoom        = 1.0f;
            this.ViewZoomCurrent = 1.0f;
            this.panel           = panel;
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// The graph this view is displaying
        /// </summary>
        public NodeGraphGraph Graph { get => this.graph; set => this.graph = value; }

        /// <summary>
        /// The panel that contains this view
        /// </summary>
        public NodeGraphPanel Panel { get => panel; set => panel = value; }

        /// <summary>
        /// The orthographic X coordinate of the current View
        /// </summary>
        public int ViewX { get => (int)this.viewProp.X; set => this.viewProp.X = value; }

        /// <summary>
        /// The orthographic Y coordinate of the current View
        /// </summary>
        public int ViewY { get => (int)this.viewProp.Y; set => this.viewProp.Y = value; }

        /// <summary>
        /// The zoom factor of the current view
        /// </summary>
        public float ViewZoom { get => this.viewProp.Z; set => this.viewProp.Z = value; }

        /// <summary>
        /// Current zoom Factor, used on smooth behavior
        /// </summary>
        public float ViewZoomCurrent { get => this.viewProp.W; set => this.viewProp.W = value; }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's collection
        /// </summary>
        public int GetNodeIndex(NodeGraphNode node)
        {
            return this.graph.GetNodeIndex(node);
        }

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's current selection
        /// </summary>
        public int GetNodeIndexSelected(NodeGraphNode node)
        {
            return this.graph.GetNodeIndexSelected(node);
        }

        #endregion
    }
}
