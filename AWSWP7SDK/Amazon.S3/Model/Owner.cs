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
    /// Represents an Owner of an S3 Bucket.
    /// </summary>
    public class Owner
    {
        #region Private Members

        private string id;
        private string displayName;

        #endregion

        #region Id

        /// <summary>
        /// Gets and sets the Id property.
        /// </summary>
        [XmlElementAttribute(ElementName = "ID")]
        public string Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Sets the Id property.
        /// </summary>
        /// <param name="id">Id property</param>
        /// <returns>this instance</returns>
        public Owner WithId(string id)
        {
            this.id = id;
            return this;
        }

        #endregion

        #region DisplayName

        /// <summary>
        /// Gets and sets the DisplayName property.
        /// </summary>
        [XmlElementAttribute(ElementName = "DisplayName")]
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        /// <summary>
        /// Sets the DisplayName property.
        /// </summary>
        /// <param name="displayName">DisplayName property</param>
        /// <returns>this instance</returns>
        public Owner WithDisplayName(string displayName)
        {
            this.displayName = displayName;
            return this;
        }

        #endregion
    }
}