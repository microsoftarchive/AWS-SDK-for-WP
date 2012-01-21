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
    /// The GetBucketLocationResponse contains the GetBucketLocationResult and 
    /// any headers returned by S3.
    /// </summary>
    [XmlTypeAttribute(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/")]
    [XmlRootAttribute(Namespace = "http://s3.amazonaws.com/doc/2006-03-01/", IsNullable = false, ElementName = "LocationConstraint")]
    public class GetBucketLocationResponse : S3Response
    {
        #region Private Members

        private string location;

        #endregion

        #region Location

        /// <summary>
        /// Gets and sets the Location property.
        /// </summary>
        [XmlElementAttribute(ElementName = "Location")]
        public string Location
        {
            get 
            {
                if (string.IsNullOrEmpty(this.location))
                {
                    this.location = "US";                   
                }
                return this.location; 
            }
            set 
            {
                this.location = value; 
            }
        }

        #endregion
    }
}