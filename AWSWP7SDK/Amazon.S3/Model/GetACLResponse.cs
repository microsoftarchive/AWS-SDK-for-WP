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
    /// The GetACLResponse contains all the information about the    
    /// GetACL operation.    
    /// </summary>
    [XmlTypeAttribute(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    [XmlRootAttribute(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/", IsNullable = false, ElementName = "AccessControlPolicy")]
    public class GetACLResponse : S3Response
    {
        #region Private Members

        private S3AccessControlList accessControlList;
        private string versionId;
        private Owner _owner;

        #endregion

        #region Owner

        [XmlElement(ElementName = "Owner")]
        public Owner Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        #endregion Owner

        #region S3AccessControlList

        /// <summary>
        /// Gets and sets the AccessControlList property.
        /// </summary>
        [XmlElementAttribute(ElementName = "AccessControlList")]
        public S3AccessControlList AccessControlList
        {
            get { return this.accessControlList; }
            set { this.accessControlList = value; }
        }

        #endregion

        #region VersionId

        /// <summary>
        /// Gets and sets the VersionId property.
        /// This is the version-id of the S3 object
        /// </summary>
        [XmlIgnore]
        public string VersionId
        {
            get { return this.versionId; }
            set { this.versionId = value; }
        }

        #endregion

        #region Headers

        /// <summary>
        /// Gets and sets the Headers property.
        /// </summary>
        public override System.Net.WebHeaderCollection Headers
        {
            set
            {
                base.Headers = value;

                string hdr = null;
                if (!String.IsNullOrEmpty(hdr = value[Util.S3Constants.AmzVersionIdHeader]))
                {
                    VersionId = hdr;
                }
            }
        }

        #endregion
    }
}