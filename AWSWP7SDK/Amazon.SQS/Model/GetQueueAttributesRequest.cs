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

using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Attribute = Amazon.SQS.Model.Attribute;
using System.Collections.Generic;

namespace Amazon.SQS.Model
{
    /// <summary>
    /// Gets one or all attributes of a queue.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class GetQueueAttributesRequest
    {
        private string queueUrl;
        private List<string> attributeName;

        /// <summary>
        /// Gets and sets the QueueUrl property.
        /// The URL associated with the Amazon SQS queue.
        /// </summary>
        [XmlElementAttribute(ElementName = "QueueUrl")]
        public string QueueUrl
        {
            get { return this.queueUrl; }
            set { this.queueUrl = value; }
        }

        /// <summary>
        /// Sets the QueueUrl property
        /// </summary>
        /// <param name="queueUrl">The URL associated with the Amazon SQS queue.</param>
        /// <returns>this instance</returns>
        public GetQueueAttributesRequest WithQueueUrl(string queueUrl)
        {
            this.queueUrl = queueUrl;
            return this;
        }

        /// <summary>
        /// Checks if QueueUrl property is set
        /// </summary>
        /// <returns>true if QueueUrl property is set</returns>
        public bool IsSetQueueUrl()
        {
            return this.queueUrl != null;
        }

        /// <summary>
        /// Gets the AttributeName property.
        /// The attribute you want to get. Values can be:
        /// All | ApproximateNumberOfMessages | ApproximateNumberOfMessagesNotVisible | VisibilityTimeout | CreatedTimestamp | LastModifiedTimestamp | Policy
        /// </summary>
        [XmlElementAttribute(ElementName = "AttributeName")]
        public List<string> AttributeName
        {
            get
            {
                if (this.attributeName == null)
                {
                    this.attributeName = new List<string>();
                }
                return this.attributeName;
            }
        }

        /// <summary>
        /// Sets the AttributeName property
        /// </summary>
        /// <param name="list">The attribute you want to get. Values can be:
        /// All | ApproximateNumberOfMessages | ApproximateNumberOfMessagesNotVisible | VisibilityTimeout | CreatedTimestamp | LastModifiedTimestamp | Policy</param>
        /// <returns>this instance</returns>
        public GetQueueAttributesRequest WithAttributeName(params string[] list)
        {
            foreach (string item in list)
            {
                AttributeName.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if AttributeName property is set
        /// </summary>
        /// <returns>true if AttributeName property is set</returns>
        public bool IsSetAttributeName()
        {
            return (AttributeName.Count > 0);
        }

    }
}
