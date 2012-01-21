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
    /// The PutAttributes operation creates or replaces attributes in an item. Attribute are uniquely identified in an
    /// item by their name/value combination. For example, a single item can have the attributes { "first_name",
    /// "first_value" } and { "first_name", second_value" }. However, it cannot have two attribute instances where both
    /// the attribute name and attribute value are the same.
    ///
    /// Optionally, the requestor can supply the Replace parameter for each individual attribute. Setting this value to
    /// true causes the new attribute value to replace the existing attribute value(s). For example, if an item has the
    /// attributes { 'a', '1' }, { 'b', '2'} and { 'b', '3' } and the requestor calls PutAttributes using the attributes
    /// { 'b', '4' } with the Replace parameter set to true, the final attributes of the item are changed to { 'a', '1' }
    /// and { 'b', '4' }, which replaces the previous values of the 'b' attribute with the new value.
    ///
    /// Note: Using PutAttributes to replace attribute values that do not exist will not result in an error response.
    ///
    /// You cannot specify an empty string as an attribute name. Because Amazon SimpleDB makes multiple copies of your data
    /// and uses an eventual consistency update model, an immediate GetAttributes or Select request (read) immediately after a
    /// DeleteAttributes request (write) might not return the updated data.
    ///
    /// The following limitations are enforced for this operation:
    ///
    /// * 256 total attribute name-value pairs per item
    /// * One billion attributes per domain
    /// * 10 GB of total user data storage per domain
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class PutAttributesRequest
    {
        private string domainName;
        private string itemName;
        private List<ReplaceableAttribute> attributes;
        private UpdateCondition expected;

        /// <summary>
        /// Gets and sets the DomainName property.
        /// The name of the domain in which to perform the operation.
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainName")]
        public string DomainName
        {
            get { return this.domainName; }
            set { this.domainName = value; }
        }

        /// <summary>
        /// Sets the DomainName property
        /// </summary>
        /// <param name="domainName">The name of the domain in which to perform the operation.</param>
        /// <returns>this instance</returns>
        public PutAttributesRequest WithDomainName(string domainName)
        {
            this.domainName = domainName;
            return this;
        }

        /// <summary>
        /// Checks if DomainName property is set
        /// </summary>
        /// <returns>true if DomainName property is set</returns>
        public bool IsSetDomainName()
        {
            return this.domainName != null;
        }

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
        public PutAttributesRequest WithItemName(string itemName)
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
        public PutAttributesRequest WithAttribute(params ReplaceableAttribute[] list)
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

        /// <summary>
        /// Gets and sets the Expected property.
        /// Performs the operation if the specified attribute name and value match.
        /// </summary>
        [XmlElementAttribute(ElementName = "Expected")]
        public UpdateCondition Expected
        {
            get { return this.expected; }
            set { this.expected = value; }
        }

        /// <summary>
        /// Sets the Expected property
        /// </summary>
        /// <param name="expected">Performs the operation if the specified attribute name and value match.</param>
        /// <returns>this instance</returns>
        public PutAttributesRequest WithExpected(UpdateCondition expected)
        {
            this.expected = expected;
            return this;
        }

        /// <summary>
        /// Checks if Expected property is set
        /// </summary>
        /// <returns>true if Expected property is set</returns>
        public bool IsSetExpected()
        {
            return this.expected != null;
        }

    }
}
