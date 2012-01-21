/*
   Copyright 2011 Microsoft Corp.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.

*/

/*******************************************************************************
 *  AWS SDK for WP7
 */

using System.Collections.Generic;
using System.Xml.Serialization;

namespace Amazon.SimpleDB.Model
{
    ///<summary>
    ///Item represent individual objects that contain one or more attribute name-value pairs.
    ///</summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class ReplaceableItem
    {
        private string itemName;
        private List<ReplaceableAttribute> attributes;

        /// <summary>
        /// Gets and sets the ItemName property.
        /// The name of the item.
        /// </summary>
        [XmlElementAttribute(ElementName = "ItemName")]
        public string ItemName
        {
            get { return this.itemName; }
            set { this.itemName = value; }
        }

        /// <summary>
        /// Sets the ItemName property
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <returns>this instance</returns>
        public ReplaceableItem WithItemName(string itemName)
        {
            this.itemName = itemName;
            return this;
        }

        /// <summary>
        /// Checks if ItemName property is set
        /// </summary>
        /// <returns>true if ItemName property is set</returns>
        public bool IsSetItemName()
        {
            return this.itemName != null;
        }
        /// <summary>
        /// Gets and sets the Attribute property.
        /// An attribute associated with an item. Similar to columns on a spreadsheet, attributes represent
        /// categories of data that can be assigned to items.
        /// </summary>
        [XmlElementAttribute(ElementName = "Attribute")]
        public List<ReplaceableAttribute> Attribute
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = new List<ReplaceableAttribute>();
                }
                return this.attributes;
            }
        }

        /// <summary>
        /// Sets the Attribute property
        /// </summary>
        /// <param name="list">An attribute associated with an item. Similar to columns on a spreadsheet, attributes represent
        /// categories of data that can be assigned to items.</param>
        /// <returns>this instance</returns>
        public ReplaceableItem WithAttribute(params ReplaceableAttribute[] list)
        {
            foreach (ReplaceableAttribute item in list)
            {
                Attribute.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if Attribute property is set
        /// </summary>
        /// <returns>true if Attribute property is set</returns>
        public bool IsSetAttribute()
        {
            return (Attribute.Count > 0);
        }

    }
}
