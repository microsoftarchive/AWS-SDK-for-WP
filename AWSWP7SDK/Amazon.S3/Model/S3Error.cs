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

namespace Amazon.S3.Model
{
    /// <summary>
    /// Represents an error returned by the S3 service. Exposes 
    /// an error code, a message, a host ID and a request ID for
    /// debugging purposes.
    /// </summary>
    [XmlRootAttribute(ElementName = "Error")]
    public class S3Error
    {
        private string code;
        private string message;
        private string hostId;
        private string requestId;
        private string uploadId;
        private string etag;

        /// <summary>
        /// Gets and sets the Code property.
        /// </summary>
        public string Code
        {
            get { return this.code; }
            set { this.code = value; }
        }

        /// <summary>
        /// Gets and sets the Message property.
        /// </summary>
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        /// <summary>
        /// Gets and sets the HostId property.
        /// </summary>
        public string HostId
        {
            get { return this.hostId; }
            set { this.hostId = value; }
        }

        /// <summary>
        /// Gets and sets the RequestId property.
        /// </summary>
        public string RequestId
        {
            get { return this.requestId; }
            set { this.requestId = value; }
        }

        /// <summary>
        /// Gets and sets the UploadId property.
        /// </summary>
        public string UploadId
        {
            get { return this.uploadId; }
            set { this.uploadId = value; }
        }

        /// <summary>
        /// Gets and sets the ETag property.
        /// </summary>
        public string ETag
        {
            get { return this.etag; }
            set { this.etag = value; }
        }
    }
}
