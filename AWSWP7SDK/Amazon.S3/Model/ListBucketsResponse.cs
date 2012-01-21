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

namespace Amazon.S3.Model
{
    /// <summary>
    /// The ListBucketsResponse contains the ListBucketsResult and
    /// any headers or metadata returned by S3.
    /// </summary>
    [XmlTypeAttribute(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    [XmlRootAttribute(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/", IsNullable = false, ElementName = "ListAllMyBucketsResult")]
    public class ListBucketsResponse : S3Response
    {
        #region Private Members

        private List<S3Bucket> buckets = new List<S3Bucket>();
        private Owner owner;

        #endregion

        #region Buckets

        /// <summary>
        /// Gets the Bucket property. This property has been deprecated -
        /// please use the Buckets property of ListBucketsResponse.
        /// <see cref="P:Amazon.S3.Model.ListBucketsResponse.Buckets"/>
        /// </summary>       
        [XmlArrayItemAttribute("Bucket", IsNullable = false)]
        public List<S3Bucket> Buckets
        {
            get { return this.buckets; }
        }

        #endregion

        #region Owner

        /// <summary>
        /// Gets and sets the Owner property.
        /// </summary>
        [XmlElementAttribute(ElementName = "Owner")]
        public Owner Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        #endregion
    }
}