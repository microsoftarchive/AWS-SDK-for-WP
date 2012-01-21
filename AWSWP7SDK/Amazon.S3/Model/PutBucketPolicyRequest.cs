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
using System.Xml.Serialization;

namespace Amazon.S3.Model
{
    /// <summary>
    /// The PutBucketPolicyRequest contains the parameters used for the PutBucketPolicy operation.
    /// <br />Required Parameters: BucketName, Policy
    /// </summary>
    public class PutBucketPolicyRequest : S3Request
    {
        #region Private Members

        private string bucketName;
        private string policy;

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the BucketName property.
        /// </summary>
        [XmlElementAttribute(ElementName = "BucketName")]
        public string BucketName
        {
            get
            {
                return this.bucketName;
            }
            set
            {
                this.bucketName = value;
            }
        }

        /// <summary>
        /// Sets the BucketName property for this request.
        /// This is the S3 Bucket the request will get the location for.
        /// </summary>
        /// <param name="bucketName">The value that BucketName is set to</param>
        /// <returns>this instance</returns>
        public PutBucketPolicyRequest WithBucketName(string bucketName)
        {
            this.BucketName = bucketName;
            return this;
        }

        /// <summary>
        /// Checks if BucketName property is set.
        /// </summary>
        /// <returns>true if BucketName property is set.</returns>
        internal bool IsSetBucketName()
        {
            return !System.String.IsNullOrEmpty(this.BucketName);
        }


        /// <summary>
        /// Gets and sets the Policy property.
        /// </summary>
        public string Policy
        {
            get
            {
                return this.policy;
            }
            set
            {
                this.policy = value;
            }
        }

        /// <summary>
        /// Sets the Policy property for this request.
        /// This is the JSON string representing the policy that will be applied to the S3 Bucket.
        /// </summary>
        /// <param name="policy">The JSON string for the policy</param>
        /// <returns>this instance</returns>
        public PutBucketPolicyRequest WithPolicy(string policy)
        {
            this.Policy = policy;
            return this;
        }

        /// <summary>
        /// Checks if policy property is set.
        /// </summary>
        /// <returns>true if Policy property is set.</returns>
        internal bool IsSetPolicy()
        {
            return !System.String.IsNullOrEmpty(this.Policy);
        }

        #endregion
    }
}
