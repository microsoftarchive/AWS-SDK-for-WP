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
    /// Information about the request provided by Amazon SimpleDB.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class ResponseMetadata
    {
        private string requestIdField;
        private string boxUsageField;

        /// <summary>
        /// Gets and sets the RequestId property.
        /// A unique ID for tracking the request.
        /// </summary>
        [XmlElementAttribute(ElementName = "RequestId")]
        public string RequestId
        {
            get { return this.requestIdField; }
            set { this.requestIdField = value; }
        }

        /// <summary>
        /// Sets the RequestId property
        /// </summary>
        /// <param name="requestId">A unique ID for tracking the request.</param>
        /// <returns>this instance</returns>
        public ResponseMetadata WithRequestId(string requestId)
        {
            this.requestIdField = requestId;
            return this;
        }

        /// <summary>
        /// Checks if RequestId property is set
        /// </summary>
        /// <returns>true if RequestId property is set</returns>
        public bool IsSetRequestId()
        {
            return this.requestIdField != null;
        }

        /// <summary>
        /// Gets and sets the BoxUsage property.
        /// The measure of machine utilization for this request. This does not include storage or transfer usage.
        /// </summary>
        [XmlElementAttribute(ElementName = "BoxUsage")]
        public string BoxUsage
        {
            get { return this.boxUsageField; }
            set { this.boxUsageField = value; }
        }

        /// <summary>
        /// Sets the BoxUsage property
        /// </summary>
        /// <param name="boxUsage">The measure of machine utilization for this request. This does not include storage or transfer usage.</param>
        /// <returns>this instance</returns>
        public ResponseMetadata WithBoxUsage(string boxUsage)
        {
            this.boxUsageField = boxUsage;
            return this;
        }

        /// <summary>
        /// Checks if BoxUsage property is set
        /// </summary>
        /// <returns>true if BoxUsage property is set</returns>
        public bool IsSetBoxUsage()
        {
            return this.boxUsageField != null;
        }

    }
}
