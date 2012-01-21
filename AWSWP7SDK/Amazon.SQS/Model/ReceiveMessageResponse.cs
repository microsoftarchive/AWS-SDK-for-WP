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
    /// Returns a list of messages and metadata about the request.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class ReceiveMessageResponse : SQSBaseResponse
    {    
        private ReceiveMessageResult receiveMessageResultField;

        /// <summary>
        /// Gets and sets the ReceiveMessageResult property.
        /// A list of messages associated with the ReceiveMessageRequest.
        /// </summary>
        [XmlElementAttribute(ElementName = "ReceiveMessageResult")]
        public ReceiveMessageResult ReceiveMessageResult
        {
            get { return this.receiveMessageResultField; }
            set { this.receiveMessageResultField = value; }
        }

        /// <summary>
        /// Checks if ReceiveMessageResult property is set
        /// </summary>
        /// <returns>true if ReceiveMessageResult property is set</returns>
        public bool IsSetReceiveMessageResult()
        {
            return this.receiveMessageResultField != null;
        }
        
    }
}
