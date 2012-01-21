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
using System.Xml.Serialization;
using Amazon.Util;

namespace Amazon.S3.Model
{
    /// <summary>
    /// Represents an S3 Bucket. 
    /// Contains a Bucket Name which is the name of the S3 Bucket. 
    /// And a Creation Date which is the date that the S3 Bucket was created.
    /// </summary>
    public class S3Bucket
    {
        #region Private Members

        private string bucketName;
        private DateTime? creationDate;

        #endregion

        #region BucketName

        /// <summary>
        /// Gets and sets the BucketName property.
        /// </summary>
        [XmlElementAttribute(ElementName = "Name")]
        public string BucketName
        {
            get { return this.bucketName; }
            set { this.bucketName = value; }
        }

        #endregion

        #region CreationDate

        /// <summary>
        /// Gets and sets the CreationDate property.
        /// The date conforms to the ISO8601 date format.
        /// </summary>
        [XmlElementAttribute(ElementName = "CreationDate")]
        public string CreationDate
        {
            get
            {
                return this.creationDate.GetValueOrDefault().ToString(
                    AWSSDKUtils.GMTDateFormat, CultureInfo.InvariantCulture
                    );
            }
            set
            {
                this.creationDate = DateTime.ParseExact(
                    value,
                    AWSSDKUtils.ISO8601DateFormat,
                    CultureInfo.InvariantCulture
                    );
            }
        }

        #endregion
    }
}