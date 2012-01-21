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
    /// Creates a new domain. The domain name must be unique among the domains associated with the account used in the request.
    /// The operation might take 10 or more seconds to complete. Creating a domain is an idempotent operation; running it
    /// multiple times using the same domain name will not result in an error response. You can create up to 100 domains
    /// per account. If you require additional domains, go to http://aws.amazon.com/contact-us/simpledb-limit-request/.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class CreateDomainRequest
    {
        private string domainNameField;

        /// <summary>
        /// Gets and sets the DomainName property.
        /// The name of the domain to create. The name can range between 3 and 255 characters
        /// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.
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
        /// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
        /// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
        /// <returns>this instance</returns>
        public CreateDomainRequest WithDomainName(string domainName)
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

    }
}
