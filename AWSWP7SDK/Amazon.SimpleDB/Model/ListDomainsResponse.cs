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

using System.Xml.Serialization;

namespace Amazon.SimpleDB.Model
{
    /// <summary>
    /// Lists all domains associated with the account. It returns domain names up to the limit set by MaxNumberOfDomains.
    /// A NextToken is returned if there are more than MaxNumberOfDomains domains. Calling ListDomains successive times
    /// with the NextToken returns up to MaxNumberOfDomains more domain names each time.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class ListDomainsResponse : SimpleDBResponse
    {
        private ListDomainsResult listDomainsResultField;

        /// <summary>
        /// Gets and sets the ListDomainsResult property.
        /// Lists all domains associated with the account. It returns domain names up to the limit set by MaxNumberOfDomains.
        /// A NextToken is returned if there are more than MaxNumberOfDomains domains. Calling ListDomains successive times
        /// with the NextToken returns up to MaxNumberOfDomains more domain names each time.
        /// </summary>
        [XmlElementAttribute(ElementName = "ListDomainsResult")]
        public ListDomainsResult ListDomainsResult
        {
            get { return this.listDomainsResultField; }
            set { this.listDomainsResultField = value; }
        }

        /// <summary>
        /// Sets the ListDomainsResult property
        /// </summary>
        /// <param name="listDomainsResult">Lists all domains associated with the account. It returns domain names up to the limit set by MaxNumberOfDomains.
        /// A NextToken is returned if there are more than MaxNumberOfDomains domains. Calling ListDomains successive times
        /// with the NextToken returns up to MaxNumberOfDomains more domain names each time.</param>
        /// <returns>this instance</returns>
        public ListDomainsResponse WithListDomainsResult(ListDomainsResult listDomainsResult)
        {
            this.listDomainsResultField = listDomainsResult;
            return this;
        }

        /// <summary>
        /// Checks if ListDomainsResult property is set
        /// </summary>
        /// <returns>true if ListDomainsResult property is set</returns>
        public bool IsSetListDomainsResult()
        {
            return this.listDomainsResultField != null;
        }

        /// <summary>
        /// Sets the ResponseMetadata property
        /// </summary>
        /// <param name="responseMetadata">Information about the request provided by Amazon SimpleDB.</param>
        /// <returns>this instance</returns>
        public ListDomainsResponse WithResponseMetadata(ResponseMetadata responseMetadata)
        {
            this.ResponseMetadata = responseMetadata;
            return this;
        }
    }
}
