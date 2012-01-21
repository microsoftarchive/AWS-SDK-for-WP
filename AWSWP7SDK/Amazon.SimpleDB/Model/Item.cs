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
    public class Item
    {
        private string name;
        private string nameEncoding;
        private List<Attribute> attributes;

        /// <summary>
        /// Gets and sets the Name property.
        /// The name of the item.
        /// </summary>
        [XmlElementAttribute(ElementName = "Name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Sets the Name property
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>this instance</returns>
        public Item WithName(string name)
        {
            this.name = name;
            return this;
        }

        /// <summary>
        /// Checks if Name property is set
        /// </summary>
        /// <returns>true if Name property is set</returns>
        public bool IsSetName()
        {
            return this.name != null;
        }

        /// <summary>
        /// Gets and sets the NameEncoding property.
        /// The encoding for the item's name. For example: base64
        /// </summary>
        [XmlElementAttribute(ElementName = "NameEncoding")]
        public string NameEncoding
        {
            get { return this.nameEncoding; }
            set { this.nameEncoding = value; }
        }

        /// <summary>
        /// Sets the NameEncoding property
        /// </summary>
        /// <param name="nameEncoding">The encoding for the item's name. For example: base64</param>
        /// <returns>this instance</returns>
        public Item WithNameEncoding(string nameEncoding)
        {
            this.nameEncoding = nameEncoding;
            return this;
        }

        /// <summary>
        /// Checks if NameEncoding property is set
        /// </summary>
        /// <returns>true if NameEncoding property is set</returns>
        public bool IsSetNameEncoding()
        {
            return this.nameEncoding != null;
        }
        /// <summary>
        /// Gets and sets the Attribute property.
        /// Attribute associated with an Item.
        /// </summary>
        [XmlElementAttribute(ElementName = "Attribute")]
        public List<Attribute> Attribute
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = new List<Attribute>();
                }
                return this.attributes;
            }
        }

        /// <summary>
        /// Sets the Attribute property
        /// </summary>
        /// <param name="list">Attribute associated with an Item.</param>
        /// <returns>this instance</returns>
        public Item WithAttribute(params Attribute[] list)
        {
            foreach (Attribute item in list)
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
