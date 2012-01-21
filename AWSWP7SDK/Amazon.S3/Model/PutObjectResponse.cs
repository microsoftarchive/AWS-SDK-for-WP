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

using Amazon.Util;

namespace Amazon.S3.Model
{
    /// <summary>
    /// The PutObjectResponse contains any headers returned by S3.
    /// </summary>
    public class PutObjectResponse : S3Response
    {
        private string etag;
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

                foreach (var key in value.AllKeys)
                {
                    if (key.StartsWith(AWSSDKUtils.ETagHeader, System.StringComparison.Ordinal))
                        this.etag = value[key];

                    else if (key.StartsWith(AWSSDKUtils.SDKVersionNumber, System.StringComparison.OrdinalIgnoreCase))
                        this.versionId = value[key];
                }
            }
        }
    }
}