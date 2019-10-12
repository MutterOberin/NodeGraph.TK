using System.ComponentModel;
using System.Drawing;

using NodeGraph.TK;

namespace NodeGraph.TK.Example
{
    public class UserNode : Node
    {
        #region - Private Variables -

        protected Point locationUI;
        
        protected SI item;

        #endregion

        #region - Constructors -

        public UserNode(float X, float Y, View view)
            : base((int)X, (int)Y, view, true)
        {
            base.color_fill = Util.ColorToVector4(Color.FromArgb(145, 128, 128));
        }

        #endregion

        #region - Properties -

        [Category("Node GUI")]
        public Point GUI_Location
        {
            get { return this.locationUI; }
            set { this.locationUI = value; }
        }

        [Category("Node GUI")]
        public int GUI_Location_X
        {
            get { return this.locationUI.X; }
            set { this.locationUI.X = value; }
        }

        [Category("Node GUI")]
        public int GUI_Location_Y
        {
            get { return this.locationUI.Y; }
            set { this.locationUI.Y = value; }
        }

        /// <summary>
        /// Gets Sets the User Item
        /// </summary>
        [Category("Node Item")]
        public virtual SI Item
        {
            get { return item; }
            set { item = value; }
        }

        #endregion

        #region - Methods -

        /// <summary>
        /// Returns the name of the node: can be overriden to match custom names.
        /// </summary>
        /// <returns></returns>
        protected override string GetName()
        {
            if (this.item != null)
                return base.GetName() + $" Item: None";
            else
                return base.GetName() + $" Item: {item.ItemName}";
        }

        /// <summary>
        /// Dispose the coresponding item
        /// </summary>
        public virtual void DisposeItem()
        {
            if (item != null)
                item.Dispose();
        }

        ///// <summary>
        ///// Serialize to XML Node
        ///// </summary>
        //public virtual void Serialize(XmlElement node)
        //{
        //    //Util_XML.Append(node.OwnerDocument, node, "GUI_Location", this.guiLoc);
        //}

        ///// <summary>
        ///// Deserialize from XML Node
        ///// </summary>
        //public virtual void Deserialize(XmlNode node)
        //{
        //    //foreach (XmlNode childNode in node.ChildNodes)
        //    //{
        //    //    if (childNode.Name.Equals("GUI_Location"))
        //    //        this.guiLoc = Util_XML.Read_Point(childNode);
        //    //}
        //}

        #endregion

    }
}
