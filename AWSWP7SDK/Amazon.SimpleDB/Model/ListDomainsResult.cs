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
    /// Lists all domains associated with the account. It returns domain names up to the limit set by MaxNumberOfDomains.
    /// A NextToken is returned if there are more than MaxNumberOfDomains domains. Calling ListDomains successive times
    /// with the NextToken returns up to MaxNumberOfDomains more domain names each time.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class ListDomainsResult
    {
        private List<string> domainNames;
        private string nextToken;

        /// <summary>
        /// Gets and sets the DomainName property.
        /// Domain name that matches the expression.
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainName")]
        public List<string> DomainName
        {
            get
            {
                if (this.domainNames == null)
                {
                    this.domainNames = new List<string>();
                }
                return this.domainNames;
            }
        }

        /// <summary>
        /// Sets the DomainName property
        /// </summary>
        /// <param name="list">Domain name that matches the expression.</param>
        /// <returns>this instance</returns>
        public ListDomainsResult WithDomainName(params string[] list)
        {
            foreach (string item in list)
            {
                DomainName.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if DomainName property is set
        /// </summary>
        /// <returns>true if DomainName property is set</returns>
        public bool IsSetDomainName()
        {
            return (DomainName.Count > 0);
        }

        /// <summary>
        /// Gets and sets the NextToken property.
        /// An opaque token indicating that there are more than MaxNumberOfDomains domains still available.
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
        /// <param name="nextToken">An opaque token indicating that there are more than MaxNumberOfDomains domains still available.</param>
        /// <returns>this instance</returns>
        public ListDomainsResult WithNextToken(string nextToken)
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
