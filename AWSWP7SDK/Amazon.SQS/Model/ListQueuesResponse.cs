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

namespace Amazon.SQS.Model
{
    /// <summary>
    /// Returns a list of queues and related metadata about the request.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class ListQueuesResponse : SQSBaseResponse
    {    
        private ListQueuesResult listQueuesResultField;

        /// <summary>
        /// Gets and sets the ListQueuesResult property.
        /// Information returned by the ListQueuesRequest, including queue URL.
        /// </summary>
        [XmlElementAttribute(ElementName = "ListQueuesResult")]
        public ListQueuesResult ListQueuesResult
        {
            get { return this.listQueuesResultField; }
            set { this.listQueuesResultField = value; }
        }

        /// <summary>
        /// Checks if ListQueuesResult property is set
        /// </summary>
        /// <returns>true if ListQueuesResult property is set</returns>
        public bool IsSetListQueuesResult()
        {
            return this.listQueuesResultField != null;
        }
    }
}
