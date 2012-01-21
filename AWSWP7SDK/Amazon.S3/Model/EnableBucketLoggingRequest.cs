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
    /// The EnableBucketLoggingRequest contains the parameters used for the EnableBucketLogging operation.
    /// <br />Required Parameters: BucketName, LoggingConfig
    /// </summary>
    public class EnableBucketLoggingRequest : S3Request
    {
        #region Private Members

        private string bucketName;
        private S3BucketLoggingConfig loggingConfig;

        #endregion

        #region BucketName

        /// <summary>
        /// Gets and sets the BucketName property.
        /// </summary>
        [XmlElementAttribute(ElementName = "BucketName")]
        public string BucketName
        {
            get { return this.bucketName; }
            set { this.bucketName = value; }
        }

        /// <summary>
        /// Sets the BucketName property for this request.
        /// This is the S3 Bucket that you want to enable logging.
        /// </summary>
        /// <param name="bucketName">The value that BucketName is set to</param>
        /// <returns>the request with the BucketName set</returns>
        public EnableBucketLoggingRequest WithBucketName(string bucketName)
        {
            this.bucketName = bucketName;
            return this;
        }

        /// <summary>
        /// Checks if BucketName property is set.
        /// </summary>
        /// <returns>true if BucketName property is set.</returns>
        internal bool IsSetBucketName()
        {
            return !System.String.IsNullOrEmpty(this.bucketName);
        }

        #endregion

        #region LoggingConfig

        /// <summary>
        /// Gets and sets the LoggingConfig property.
        /// </summary>
        [XmlElementAttribute(ElementName = "LoggingConfig")]
        public S3BucketLoggingConfig LoggingConfig
        {
            get
            {
                if (this.loggingConfig == null)
                {
                    this.loggingConfig = new S3BucketLoggingConfig();
                }
                return this.loggingConfig;
            }
            set { this.loggingConfig = value; }
        }

        /// <summary>
        /// Sets the LoggingConfig property for this request.
        /// This config declares all of the logging configuration properties.
        /// </summary>
        /// <param name="loggingConfig">The value that LoggingConfig is set to</param>
        /// <returns>the request with the LoggingConfig set</returns>
        public EnableBucketLoggingRequest WithLoggingConfig(S3BucketLoggingConfig loggingConfig)
        {
            this.loggingConfig = loggingConfig;
            return this;
        }

        #endregion
    }
}