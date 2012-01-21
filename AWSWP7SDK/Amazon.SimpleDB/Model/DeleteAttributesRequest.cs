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
    /// <summary>
    /// Deletes one or more attributes associated with the item. If all attributes of an item are deleted, the
    /// item is deleted.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class DeleteAttributesRequest
    {
        private string domainNameField;
        private string itemNameField;
        private List<Attribute> attributes;
        private UpdateCondition expectedField;

        /// <summary>
        /// Gets and sets the DomainName property.
        /// The name of the domain in which to perform the operation.
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName
        {
            get { return this.domainNameField; }
            set { this.domainNameField = value; }
        }

        /// <summary>
        /// Sets the DomainName property
        /// </summary>
        /// <param name="domainName">The name of the domain in which to perform the operation.</param>
        /// <returns>this instance</returns>
        public DeleteAttributesRequest WithDomainName(string domainName)
        {
            this.domainNameField = domainName;
            return this;
        }

        /// <summary>
        /// Checks if DomainName property is set
        /// </summary>
        /// <returns>true if DomainName property is set</returns>
        public bool IsSetDomainName()
        {
            return this.domainNameField != null;
        }

        /// <summary>
        /// Gets and sets the ItemName property.
        /// The name of the item in which to perform the operation.
        /// </summary>
        [XmlElementAttribute(ElementName = "ItemName")]
        public string ItemName
        {
            get { return this.itemNameField; }
            set { this.itemNameField = value; }
        }

        /// <summary>
        /// Sets the ItemName property
        /// </summary>
        /// <param name="itemName">The name of the item in which to perform the operation.</param>
        /// <returns>this instance</returns>
        public DeleteAttributesRequest WithItemName(string itemName)
        {
            this.itemNameField = itemName;
            return this;
        }

        /// <summary>
        /// Checks if ItemName property is set
        /// </summary>
        /// <returns>true if ItemName property is set</returns>
        public bool IsSetItemName()
        {
            return this.itemNameField != null;
        }
        /// <summary>
        /// Gets and sets the Attribute property.
        /// List of attributes to delete.
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
        /// <param name="list">List of attributes to delete.</param>
        /// <returns>this instance</returns>
        public DeleteAttributesRequest WithAttribute(params Attribute[] list)
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

        /// <summary>
        /// Gets and sets the Expected property.
        /// Performs the operation if the specified attribute name and value match.
        /// </summary>
        [XmlElementAttribute(ElementName = "Expected")]
        public UpdateCondition Expected
        {
            get { return this.expectedField; }
            set { this.expectedField = value; }
        }

        /// <summary>
        /// Sets the Expected property
        /// </summary>
        /// <param name="expected">Performs the operation if the specified attribute name and value match.</param>
        /// <returns>this instance</returns>
        public DeleteAttributesRequest WithExpected(UpdateCondition expected)
        {
            this.expectedField = expected;
            return this;
        }

        /// <summary>
        /// Checks if Expected property is set
        /// </summary>
        /// <returns>true if Expected property is set</returns>
        public bool IsSetExpected()
        {
            return this.expectedField != null;
        }

    }
}
