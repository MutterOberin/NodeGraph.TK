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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace NodeGraph.TK
{
    public class NodeGraphGraph
    {
        #region - Private Variables -

        private List<NodeGraphNode> nodes;
        private List<NodeGraphNode> nodesSelected;
        private List<NodeGraphLink> links;

        #endregion

        #region - Constructors -

        #endregion

        #region - Properties -

        /// <summary>
        /// The node Collection contained in this view
        /// </summary>
        public List<NodeGraphNode> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
        /// <summary>
        /// The collection of currently Selected nodes in this view
        /// </summary>
        public List<NodeGraphNode> NodesSelected
        {
            get { return nodesSelected; }
            set { nodesSelected = value; }
        }
        /// <summary>
        /// The collection of Links created in this view
        /// </summary>
        public List<NodeGraphLink> Links
        {
            get { return links; }
            set { links = value; }
        }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's collection
        /// </summary>
        public int GetNodeIndex(NodeGraphNode p_Node)
        {
            return this.nodes.FindIndex(x => x == p_Node);
        }

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's current selection
        /// </summary>
        public int GetNodeIndexSelected(NodeGraphNode p_Node)
        {
            return this.nodesSelected.FindIndex(x => x == p_Node);
        }

        #endregion

    }
}