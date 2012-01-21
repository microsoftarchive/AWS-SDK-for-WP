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
using Attribute = Amazon.SimpleDB.Model.Attribute;

namespace Amazon.SimpleDB.Model
{
    /// <summary>
    /// Returns all of the attributes associated with the item. Optionally, the attributes returned can be limited to
    /// one or more specified attribute name parameters.
    ///
    /// If the item does not exist on the replica that was accessed for this operation, an empty set is returned. The
    /// system does not return an error as it cannot guarantee the item does not exist on other replicas.
    ///
    /// If you do not specify any attribute names, all the attributes for the item are returned.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class GetAttributesRequest
    {
        private string domainNameField;
        private string itemNameField;
        private List<string> attributeNames;
        private bool? consistentReadField;

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
        public GetAttributesRequest WithDomainName(string domainName)
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
        public GetAttributesRequest WithItemName(string itemName)
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
        /// Gets and sets the AttributeName property.
        /// The name of the attribute.
        /// </summary>
        [XmlElementAttribute(ElementName = "AttributeName")]
        public List<string> AttributeName
        {
            get
            {
                if (this.attributeNames == null)
                {
                    this.attributeNames = new List<string>();
                }
                return this.attributeNames;
            }
        }

        /// <summary>
        /// Sets the AttributeName property
        /// </summary>
        /// <param name="list">The name of the attribute.</param>
        /// <returns>this instance</returns>
        public GetAttributesRequest WithAttributeName(params string[] list)
        {
            foreach (string item in list)
            {
                AttributeName.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if AttributeName property is set
        /// </summary>
        /// <returns>true if AttributeName property is set</returns>
        public bool IsSetAttributeName()
        {
            return (AttributeName.Count > 0);
        }

        /// <summary>
        /// Gets and sets the ConsistentRead property.
        /// If set to true, this flag ensures that the most recently written data is
        /// returned.
        /// </summary>
        [XmlElementAttribute(ElementName = "ConsistentRead")]
        public bool ConsistentRead
        {
            get { return this.consistentReadField.GetValueOrDefault(); }
            set { this.consistentReadField = value; }
        }

        /// <summary>
        /// Sets the ConsistentRead property
        /// </summary>
        /// <param name="consistentRead">If set to true, this flag ensures that the most recently written data is
        /// returned.</param>
        /// <returns>this instance</returns>
        public GetAttributesRequest WithConsistentRead(bool consistentRead)
        {
            this.consistentReadField = consistentRead;
            return this;
        }

        /// <summary>
        /// Checks if ConsistentRead property is set
        /// </summary>
        /// <returns>true if ConsistentRead property is set</returns>
        public bool IsSetConsistentRead()
        {
            return this.consistentReadField.HasValue;
        }

    }
}
