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

namespace Amazon.SimpleDB.Model
{
    /// <summary>
    /// Perform multiple DeleteAttributes operations in a single call. This helps you yield savings in round trips and
    /// latencies, and enables Amazon SimpleDB to optimize requests, which generally yields better throughput.
    ///
    /// Amazon SimpleDB uniquely identifies attributes in an item by their name/value combinations. For example, a
    /// single item can have the attributes { "first_name", "first_value" } and { "first_name", second_value" }. However,
    /// it cannot have two attribute instances where both the attribute name and attribute value are the same.
    ///
    /// Note: You cannot specify an empty string as an item or attribute name.
    ///
    /// The operation succeeds or fails in its entirety. There are no partial deletes.
    ///
    /// You can execute multiple BatchDeleteAttributes operations and other operations in parallel. However, large numbers of
    /// concurrent BatchDeleteAttributes calls can result in Service Unavailable (503) responses.
    ///
    /// This operation is vulnerable to exceeding the maximum URL size.
    ///
    /// The following limitations are enforced for this operation:
    ///
    /// * 256 attribute name-value pairs per item
    /// * 1 MB request size
    /// * 1 billion attributes per domain
    /// * 10 GB of total user data storage per domain
    /// * 25 item limit per BatchDeleteAttributes operation
    /// </summary>    
    public class BatchDeleteAttributesRequest
    {
        private string domainNameField;
        private List<DeleteableItem> items;

        /// <summary>
        /// Gets and sets the DomainName property.
        /// The name of the domain in which to perform the operation.
        /// </summary>
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
        public BatchDeleteAttributesRequest WithDomainName(string domainName)
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
        /// Gets and sets the Item property.
        /// Item represent individual objects that contain one or more attribute name-value pairs.
        /// </summary>
        public List<DeleteableItem> Item
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new List<DeleteableItem>();
                }
                return this.items;
            }
        }

        /// <summary>
        /// Sets the Item property
        /// </summary>
        /// <param name="list">Item represent individual objects that contain one or more attribute name-value pairs.</param>
        /// <returns>this instance</returns>
        public BatchDeleteAttributesRequest WithItem(params DeleteableItem[] list)
        {
            foreach (DeleteableItem item in list)
            {
                Item.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if Item property is set
        /// </summary>
        /// <returns>true if Item property is set</returns>
        public bool IsSetItem()
        {
            return (Item.Count > 0);
        }
    }
}
