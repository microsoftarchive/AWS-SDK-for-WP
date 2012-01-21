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
using System.Collections.Generic;

namespace Amazon.SQS.Model
{
    /// <summary>
    /// he ListQueues action returns a list of your queues. The maximum number of queues that can be returned is 1000.
    /// If you specify a value for the optional QueueNamePrefix parameter, only queues with a name beginning with the
    /// specified value are returned.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class ListQueuesRequest
    {
        private string queueNamePrefixField;
        private List<Attribute> attributeField;

        /// <summary>
        /// Gets and sets the QueueNamePrefix property.
        /// String to use for filtering the list results. Only those queues whose name begins with the specified string are returned.
        /// </summary>
        [XmlElementAttribute(ElementName = "QueueNamePrefix")]
        public string QueueNamePrefix
        {
            get { return this.queueNamePrefixField; }
            set { this.queueNamePrefixField = value; }
        }

        /// <summary>
        /// Sets the QueueNamePrefix property
        /// </summary>
        /// <param name="queueNamePrefix">String to use for filtering the list results. Only those queues whose name begins with the specified string are returned.</param>
        /// <returns>this instance</returns>
        public ListQueuesRequest WithQueueNamePrefix(string queueNamePrefix)
        {
            this.queueNamePrefixField = queueNamePrefix;
            return this;
        }

        /// <summary>
        /// Checks if QueueNamePrefix property is set
        /// </summary>
        /// <returns>true if QueueNamePrefix property is set</returns>
        public bool IsSetQueueNamePrefix()
        {
            return this.queueNamePrefixField != null;
        }

        /// <summary>
        /// Gets and sets the Attribute property.
        /// Name and value pair of an attribute associated with the queue.
        /// </summary>
        [XmlElementAttribute(ElementName = "Attribute")]
        public List<Attribute> Attribute
        {
            get
            {
                if (this.attributeField == null)
                {
                    this.attributeField = new List<Attribute>();
                }
                return this.attributeField;
            }
        }

        /// <summary>
        /// Sets the Attribute property
        /// </summary>
        /// <param name="list">Name and value pair of an attribute associated with the queue.</param>
        /// <returns>this instance</returns>
        public ListQueuesRequest WithAttribute(params Attribute[] list)
        {
            foreach (Attribute item in list)
            {
                Attribute.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if Attribute property is set
        /// </summary>
        /// <returns>true if Attribute property is set</returns>
        public bool IsSetAttribute()
        {
            return (Attribute.Count > 0);
        }

    }
}
