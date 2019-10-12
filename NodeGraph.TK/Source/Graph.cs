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
    public class Graph
    {
        #region - Private Variables -

        private List<Node> nodes;
        private List<Link> links;

        #endregion

        #region - Constructors -

        public Graph(string name)
        {
            this.Name = name;

            this.nodes = new List<Node>();            
            this.links = new List<Link>();
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// List of <see cref="Node"/> in this <see cref="Graph"/>
        /// </summary>
        public List<Node> Nodes
        {
            get { return this.nodes; }
        }

        ///// <summary>
        ///// List of <see cref="Node"/> in this <see cref="Graph"/> which are selected
        ///// </summary>
        //public List<Node> NodesSelected
        //{
        //    get { return this.nodesSelected; }
        //    set { this.nodesSelected = value; }
        //}

        /// <summary>
        /// List of <see cref="Link"/> in this <see cref="Graph"/>
        /// </summary>
        public List<Link> Links
        {
            get { return this.links; }
            set { this.links = value; }
        }

        /// <summary>
        /// Gets / Sets the Name of this Graph
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the Node Index of the NodeGraphNode in this graphs's collection
        /// </summary>
        public int GetNodeIndex(Node node, bool selected)
        {
            if (!selected)
                return this.nodes.FindIndex(n => n == node);
            else
                return this.nodes.FindIndex(n => n == node && n.Selected);
        }

        public int GetNodeCount(bool selected)
        {
            if (!selected)
                return this.nodes.Count;
            else
                return this.nodes.FindAll(n => n.Selected).Count;
        }

        ///// <summary>
        ///// Returns the Node Index of the NodeGraphNode in this graphs's current selection
        ///// </summary>
        //public int GetNodeIndexSelected(Node node)
        //{
        //    return this.nodesSelected.FindIndex(x => x == node);
        //}

        #endregion
    }
}