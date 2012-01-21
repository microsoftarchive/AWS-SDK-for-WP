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
    /// The Select operation returns a set of Attribute for ItemNames that match the select expression.
    /// The total size of the response cannot exceed 1 MB in total size. Amazon SimpleDB automatically adjusts the number of items
    /// returned per page to enforce this limit. For example, even if you ask to retrieve 2500 items, but each individual
    /// item is 10 kB in size, the system returns 100 items and an appropriate next token so you can get the next page of results.
    /// Operations that run longer than 5 seconds return a time-out error response or a partial or empty result set. Partial
    /// and empty result sets contains a next token which allow you to continue the operation from where it left off.
    /// Responses larger than one megabyte return a partial result set.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class SelectResult
    {
        private List<Item> items;
        private string nextToken;

        /// <summary>
        /// Gets and sets the Item property.
        /// Item represent individual objects that contain one or more attribute name-value pairs.
        /// </summary>
        [XmlElementAttribute(ElementName = "Item")]
        public List<Item> Item
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new List<Item>();
                }
                return this.items;
            }
        }

        /// <summary>
        /// Sets the Item property
        /// </summary>
        /// <param name="list">Item represent individual objects that contain one or more attribute name-value pairs.</param>
        /// <returns>this instance</returns>
        public SelectResult WithItem(params Item[] list)
        {
            foreach (Item item in list)
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

        /// <summary>
        /// Gets and sets the NextToken property.
        /// An opaque token indicating that more than MaxNumberOfItems matched, the response size exceeded 1 megabyte,
        /// or the execution time exceeded 5 seconds.
        /// </summary>
        [XmlElementAttribute(ElementName = "NextToken")]
        public string NextToken
        {
            get { return this.nextToken; }
            set { this.nextToken = value; }
        }

        /// <summary>
        /// Sets the NextToken property
        /// </summary>
        /// <param name="nextToken">An opaque token indicating that more than MaxNumberOfItems matched, the response size exceeded 1 megabyte,
        /// or the execution time exceeded 5 seconds.</param>
        /// <returns>this instance</returns>
        public SelectResult WithNextToken(string nextToken)
        {
            this.nextToken = nextToken;
            return this;
        }

        /// <summary>
        /// Checks if NextToken property is set
        /// </summary>
        /// <returns>true if NextToken property is set</returns>
        public bool IsSetNextToken()
        {
            return this.nextToken != null;
        }

    }
}
