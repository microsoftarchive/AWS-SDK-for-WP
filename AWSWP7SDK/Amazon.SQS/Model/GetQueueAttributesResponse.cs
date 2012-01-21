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
    /// Returns the attributes that match the request.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class GetQueueAttributesResponse : SQSBaseResponse
    {    
        private GetQueueAttributesResult getQueueAttributesResultField;

        /// <summary>
        /// Gets and sets the GetQueueAttributesResult property.
        /// A list of attributes returned by the GetQueueAttributesRequest.
        /// </summary>
        [XmlElementAttribute(ElementName = "GetQueueAttributesResult")]
        public GetQueueAttributesResult GetQueueAttributesResult
        {
            get { return this.getQueueAttributesResultField; }
            set { this.getQueueAttributesResultField = value; }
        }

        /// <summary>
        /// Checks if GetQueueAttributesResult property is set
        /// </summary>
        /// <returns>true if GetQueueAttributesResult property is set</returns>
        public bool IsSetGetQueueAttributesResult()
        {
            return this.getQueueAttributesResultField != null;
        }       
    }
}
