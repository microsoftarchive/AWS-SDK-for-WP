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

using System;
using System.Globalization;
using Amazon.Util;

namespace Amazon.S3.Model
{
    /// <summary>
    /// The GetObjectMetadataResponse contains any headers returned by S3.
    /// </summary>
    public class GetObjectMetadataResponse : S3Response
    {
        private string etag;
        private long contentLength;
        private string contentType;
        private string versionId;

        /// <summary>
        /// Gets and sets the ETag property.
        /// </summary>
        public string ETag
        {
            get { return this.etag; }
            set { this.etag = value; }
        }

        /// <summary>
        /// Gets and sets the ContentType property.
        /// </summary>
        public string ContentType
        {
            get { return this.contentType; }
            set { this.contentType = value; }
        }

        /// <summary>
        /// Gets and sets the ContentLength property.
        /// </summary>
        public long ContentLength
        {
            get { return this.contentLength; }
            set { this.contentLength = value; }
        }

        /// <summary>
        /// Gets and sets the VersionId property.
        /// This is the version-id of the S3 object
        /// </summary>
        public string VersionId
        {
            get { return this.versionId; }
            set { this.versionId = value; }
        }

        /// <summary>
        /// Gets and sets the Headers property.
        /// </summary>
        public override System.Net.WebHeaderCollection Headers
        {
            set
            {
                base.Headers = value;

                string hdr = null;
                if (!String.IsNullOrEmpty(hdr = value[AWSSDKUtils.ETagHeader]))
                {
                    this.ETag = hdr;
                }

                if (!String.IsNullOrEmpty(hdr = value[AWSSDKUtils.ContentTypeHeader]))
                {
                    this.ContentType = hdr;
                }

                if (!String.IsNullOrEmpty(hdr = value[AWSSDKUtils.ContentLengthHeader]))
                {
                    this.ContentLength = System.Convert.ToInt64(hdr, CultureInfo.InvariantCulture);
                }

                if (!System.String.IsNullOrEmpty(hdr = value[Util.S3Constants.AmzVersionIdHeader]))
                {
                    this.VersionId = hdr;
                }
            }
        }        
    }
}