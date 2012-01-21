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

namespace Amazon.S3.Model
{
    /// <summary>
    /// The DeleteObjectResponse contains any headers returned by S3.
    /// </summary>
    public class DeleteObjectResponse : S3Response
    {
        private bool isDeleteMarker;

        /// <summary>
        /// Gets and sets the IsDeleteMarker property.
        /// Specifies whether the object deleted was (true) or 
        /// was not (false) a Delete Marker.
        /// </summary>
        public bool IsDeleteMarker
        {
            get { return this.isDeleteMarker; }
            set { this.isDeleteMarker = value; }
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
                if (!String.IsNullOrEmpty(hdr = value[Util.S3Constants.AmzDeleteMarkerHeader]))
                {
                    isDeleteMarker = System.Convert.ToBoolean(hdr, CultureInfo.InvariantCulture);
                }
            }
        }              
    }
}