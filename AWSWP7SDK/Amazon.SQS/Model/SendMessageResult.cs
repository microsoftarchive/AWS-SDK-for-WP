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
    /// Information about the message sent to Amazon SQS.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class SendMessageResult : SQSBaseResponse
    {
        private string messageIdField;
        private string MD5OfMessageBodyField;

        /// <summary>
        /// Gets and sets the MessageId property.
        /// An element containing the message ID of the message sent to the queue.
        /// </summary>
        [XmlElementAttribute(ElementName = "MessageId")]
        public string MessageId
        {
            get { return this.messageIdField; }
            set { this.messageIdField = value; }
        }

        /// <summary>
        /// Checks if MessageId property is set
        /// </summary>
        /// <returns>true if MessageId property is set</returns>
        public bool IsSetMessageId()
        {
            return this.messageIdField != null;
        }

        /// <summary>
        /// Gets and sets the MD5OfMessageBody property.
        /// An MD5 digest of the non-URL-encoded message body string. You can use this to verify that SQS received the message
        /// correctly. SQS first URL decodes the message before creating the MD5 digest. For information about MD5, go to
        /// http://faqs.org/rfcs/rfc1321.html.
        /// </summary>
        [XmlElementAttribute(ElementName = "MD5OfMessageBody")]
        public string MD5OfMessageBody
        {
            get { return this.MD5OfMessageBodyField; }
            set { this.MD5OfMessageBodyField = value; }
        }

        /// <summary>
        /// Checks if MD5OfMessageBody property is set
        /// </summary>
        /// <returns>true if MD5OfMessageBody property is set</returns>
        public bool IsSetMD5OfMessageBody()
        {
            return this.MD5OfMessageBodyField != null;
        }       
    }
}
