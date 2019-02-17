using System;

namespace NodeGraph.TK.Example
{
    public class SI : IDisposable
    {
        public string ItemName;

        public SI()
        {
            this.ItemName = "Test";
        }

        public void Dispose()
        {
            
        }
    }
}