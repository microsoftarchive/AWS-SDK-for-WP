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
    /// Returns information about the domain, including when the domain was created, the number of items and
    /// attributes, and the size of attribute names and values.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class DomainMetadataResponse : SimpleDBResponse
    {
        private DomainMetadataResult domainMetadataResultField;

        /// <summary>
        /// Gets and sets the DomainMetadataResult property.
        /// Returns information about the domain, including when the domain was created, the number of items and
        /// attributes, and the size of attribute names and values.
        /// </summary>
        [XmlElementAttribute(ElementName = "DomainMetadataResult")]
        public DomainMetadataResult DomainMetadataResult
        {
            get { return this.domainMetadataResultField; }
            set { this.domainMetadataResultField = value; }
        }

        /// <summary>
        /// Sets the DomainMetadataResult property
        /// </summary>
        /// <param name="domainMetadataResult">Returns information about the domain, including when the domain was created, the number of items and
        /// attributes, and the size of attribute names and values.</param>
        /// <returns>this instance</returns>
        public DomainMetadataResponse WithDomainMetadataResult(DomainMetadataResult domainMetadataResult)
        {
            this.domainMetadataResultField = domainMetadataResult;
            return this;
        }

        /// <summary>
        /// Checks if DomainMetadataResult property is set
        /// </summary>
        /// <returns>true if DomainMetadataResult property is set</returns>
        public bool IsSetDomainMetadataResult()
        {
            return this.domainMetadataResultField != null;
        }

        /// <summary>
        /// Sets the ResponseMetadata property
        /// </summary>
        /// <param name="responseMetadata">Information about the request provided by Amazon SimpleDB.</param>
        /// <returns>this instance</returns>
        public DomainMetadataResponse WithResponseMetadata(ResponseMetadata responseMetadata)
        {
            this.ResponseMetadata = responseMetadata;
            return this;
        }
    }
}
