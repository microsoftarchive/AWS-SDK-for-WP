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
    /// Returns all of the attributes associated with the item. Optionally, the attributes returned can
    /// be limited to one or more specified attribute name parameters. If the item does not exist on the
    /// replica that was accessed for this operation, an empty set is returned. The system does not return
    /// an error as it cannot guarantee the item does not exist on other replicas.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class GetAttributesResponse : SimpleDBResponse
    {
        private GetAttributesResult getAttributesResultField;

        /// <summary>
        /// Gets and sets the GetAttributesResult property.
        /// Returns all of the attributes associated with the item. Optionally, the attributes returned can
        /// be limited to one or more specified attribute name parameters. If the item does not exist on the
        /// replica that was accessed for this operation, an empty set is returned. The system does not return
        /// an error as it cannot guarantee the item does not exist on other replicas.
        /// </summary>
        [XmlElementAttribute(ElementName = "GetAttributesResult")]
        public GetAttributesResult GetAttributesResult
        {
            get { return this.getAttributesResultField; }
            set { this.getAttributesResultField = value; }
        }

        /// <summary>
        /// Sets the GetAttributesResult property
        /// </summary>
        /// <param name="getAttributesResult">Returns all of the attributes associated with the item. Optionally, the attributes returned can
        /// be limited to one or more specified attribute name parameters. If the item does not exist on the
        /// replica that was accessed for this operation, an empty set is returned. The system does not return
        /// an error as it cannot guarantee the item does not exist on other replicas.</param>
        /// <returns>this instance</returns>
        public GetAttributesResponse WithGetAttributesResult(GetAttributesResult getAttributesResult)
        {
            this.getAttributesResultField = getAttributesResult;
            return this;
        }

        /// <summary>
        /// Checks if GetAttributesResult property is set
        /// </summary>
        /// <returns>true if GetAttributesResult property is set</returns>
        public bool IsSetGetAttributesResult()
        {
            return this.getAttributesResultField != null;
        }

        /// <summary>
        /// Sets the ResponseMetadata property
        /// </summary>
        /// <param name="responseMetadata">Information about the request provided by Amazon SimpleDB.</param>
        /// <returns>this instance</returns>
        public GetAttributesResponse WithResponseMetadata(ResponseMetadata responseMetadata)
        {
            this.ResponseMetadata = responseMetadata;
            return this;
        }
    }
}
