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

namespace NodeGraph.TK
{
    /// <summary>
    /// Represents a link between two NodeGraphConnectors
    /// </summary>
    public class NodeGraphLink
    {
        #region - Private Variables -

        private NodeGraphConnector inputConnector;
        private NodeGraphConnector outputConnector;

        #endregion

        #region - Constructor -

        /// <summary>
        /// Creates a new NodeGraphLink, given input and output Connectors
        /// </summary>
        /// <param name="p_Input"></param>
        /// <param name="p_Output"></param>
        public NodeGraphLink(NodeGraphConnector p_Input, NodeGraphConnector p_Output)
        {
            this.inputConnector  = p_Input;
            this.outputConnector = p_Output;
        }

        #endregion

        #region - Properties -

        /// <summary>
        /// The first end of the link, that's connected to an Output Connector
        /// </summary>
        public NodeGraphConnector Input
        {
            get { return this.inputConnector; }
        }

        /// <summary>
        /// The last end of the link, that's connected to an Input Connector
        /// </summary>
        public NodeGraphConnector Output
        {
            get { return this.outputConnector; }
        }

        #endregion
    }
}
