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
    /// Returns information about the message, including an MD5 of the message body, message ID, and request metadata.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class SendMessageResponse : SQSBaseResponse
    {
        private SendMessageResult sendMessageResultField;       

        /// <summary>
        /// Gets and sets the SendMessageResult property.
        /// Information about the message sent to Amazon SQS.
        /// </summary>
        [XmlElementAttribute(ElementName = "SendMessageResult")]
        public SendMessageResult SendMessageResult
        {
            get { return this.sendMessageResultField; }
            set { this.sendMessageResultField = value; }
        }

        /// <summary>
        /// Checks if SendMessageResult property is set
        /// </summary>
        /// <returns>true if SendMessageResult property is set</returns>
        public bool IsSetSendMessageResult()
        {
            return this.sendMessageResultField != null;
        }        
    }
}
