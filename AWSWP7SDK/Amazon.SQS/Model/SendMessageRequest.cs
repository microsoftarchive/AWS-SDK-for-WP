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
    /// The SendMessage action delivers a message to the specified queue.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class SendMessageRequest
    {    
        private string queueUrlField;
        private string messageBodyField;

        /// <summary>
        /// Gets and sets the QueueUrl property.
        /// The URL associated with the Amazon SQS queue.
        /// </summary>
        [XmlElementAttribute(ElementName = "QueueUrl")]
        public string QueueUrl
        {
            get { return this.queueUrlField; }
            set { this.queueUrlField = value; }
        }

        /// <summary>
        /// Sets the QueueUrl property
        /// </summary>
        /// <param name="queueUrl">The URL associated with the Amazon SQS queue.</param>
        /// <returns>this instance</returns>
        public SendMessageRequest WithQueueUrl(string queueUrl)
        {
            this.queueUrlField = queueUrl;
            return this;
        }

        /// <summary>
        /// Checks if QueueUrl property is set
        /// </summary>
        /// <returns>true if QueueUrl property is set</returns>
        public bool IsSetQueueUrl()
        {
            return this.queueUrlField != null;
        }

        /// <summary>
        /// Gets and sets the MessageBody property.
        /// The message to send. The maximum allowed message size is 8 KB.
        /// The following list shows the characters (in Unicode) allowed in your message, according to the W3C XML specification
        /// (for more information, go to http://www.w3.org/TR/REC-xml/#charsets). If you send any characters not included in the
        /// list, your request will be rejected.
        ///
        /// #x9 | #xA | #xD | [#x20 to #xD7FF] | [#xE000 to #xFFFD] | [#x10000 to #x10FFFF]
        /// </summary>
        [XmlElementAttribute(ElementName = "MessageBody")]
        public string MessageBody
        {
            get { return this.messageBodyField; }
            set { this.messageBodyField = value; }
        }

        /// <summary>
        /// Sets the MessageBody property
        /// </summary>
        /// <param name="messageBody">The message to send. The maximum allowed message size is 8 KB.
        /// The following list shows the characters (in Unicode) allowed in your message, according to the W3C XML specification
        /// (for more information, go to http://www.w3.org/TR/REC-xml/#charsets). If you send any characters not included in the
        /// list, your request will be rejected.
        ///
        /// #x9 | #xA | #xD | [#x20 to #xD7FF] | [#xE000 to #xFFFD] | [#x10000 to #x10FFFF]</param>
        /// <returns>this instance</returns>
        public SendMessageRequest WithMessageBody(string messageBody)
        {
            this.messageBodyField = messageBody;
            return this;
        }

        /// <summary>
        /// Checks if MessageBody property is set
        /// </summary>
        /// <returns>true if MessageBody property is set</returns>
        public bool IsSetMessageBody()
        {
            return this.messageBodyField != null;
        }

    }
}
