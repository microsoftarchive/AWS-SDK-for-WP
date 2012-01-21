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

using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Amazon.SQS.Model
{
    /// <summary>
    /// Returns information about the created queue, including queue URL and request metadata.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://queue.amazonaws.com/doc/2009-02-01/", IsNullable = false)]
    public class CreateQueueResponse : SQSBaseResponse
    {
        private CreateQueueResult createQueueResultField;
        private ResponseMetadata responseMetadataField;

        /// <summary>
        /// Gets and sets the CreateQueueResult property.
        /// Information returned by the CreateQueueRequest, including queue URL.
        /// </summary>
        [XmlElementAttribute(ElementName = "CreateQueueResult")]
        public CreateQueueResult CreateQueueResult
        {
            get { return this.createQueueResultField; }
            set { this.createQueueResultField = value; }
        }

        /// <summary>
        /// Checks if CreateQueueResult property is set
        /// </summary>
        /// <returns>true if CreateQueueResult property is set</returns>
        public bool IsSetCreateQueueResult()
        {
            return this.createQueueResultField != null;
        }

        /// <summary>
        /// Gets and sets the ResponseMetadata property.
        /// Information about the request provided by Amazon SQS.
        /// </summary>
        [XmlElementAttribute(ElementName = "ResponseMetadata")]
        new public ResponseMetadata ResponseMetadata
        {
            get { return this.responseMetadataField; }
            set { this.responseMetadataField = value; }
        }

        /// <summary>
        /// Checks if ResponseMetadata property is set
        /// </summary>
        /// <returns>true if ResponseMetadata property is set</returns>
        new public bool IsSetResponseMetadata()
        {
            return this.responseMetadataField != null;
        }

        /// <summary>
        /// XML Representation of this object
        /// </summary>
        /// <returns>XML String</returns>
        new public string ToXML()
        {
            StringBuilder xml = new StringBuilder(1024);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
            using (StringWriter sw = new StringWriter(xml, CultureInfo.InvariantCulture))
            {
                serializer.Serialize(sw, this);
            }
            return xml.ToString();
        }

        /// <summary>
        /// String Representation of this object. Overrides Object.ToString()
        /// </summary>
        /// <returns>This object as a string</returns>
        public override string ToString()
        {
            return this.ToXML();
        }
    }
}
