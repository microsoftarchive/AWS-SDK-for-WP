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

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Amazon.SimpleDB.Model
{
    /// <summary>
    /// A list of errors associated with a request returned by Amazon SimpleDB.
    /// </summary>
    [XmlRootAttribute(Namespace = "http://sdb.amazonaws.com/doc/2009-04-15/", IsNullable = false)]
    public class ErrorResponse
    {
        private List<Error> errors;
        private string requestIdField;
        /// <summary>
        /// Gets and sets the Error property.
        /// A specific error associated with a Amazon SimpleDB request.
        /// </summary>
        [XmlElementAttribute(ElementName = "Error")]
        public List<Error> Error
        {
            get
            {
                if (this.errors == null)
                {
                    this.errors = new List<Error>();
                }
                return this.errors;
            }
        }

        /// <summary>
        /// Sets the Error property
        /// </summary>
        /// <param name="list">A specific error associated with a Amazon SimpleDB request.</param>
        /// <returns>this instance</returns>
        public ErrorResponse WithError(params Error[] list)
        {
            foreach (Error item in list)
            {
                Error.Add(item);
            }
            return this;
        }

        /// <summary>
        /// Checks if Error property is set
        /// </summary>
        /// <returns>true if Error property is set</returns>
        public bool IsSetError()
        {
            return (Error.Count > 0);
        }

        /// <summary>
        /// Gets and sets the RequestId property.
        /// A unique ID for tracking the request.
        /// </summary>
        [XmlElementAttribute(ElementName = "RequestId")]
        public string RequestId
        {
            get { return this.requestIdField; }
            set { this.requestIdField = value; }
        }

        /// <summary>
        /// Sets the RequestId property
        /// </summary>
        /// <param name="requestId">A unique ID for tracking the request.</param>
        /// <returns>this instance</returns>
        public ErrorResponse WithRequestId(string requestId)
        {
            this.requestIdField = requestId;
            return this;
        }

        /// <summary>
        /// Checks if RequestId property is set
        /// </summary>
        /// <returns>true if RequestId property is set</returns>
        public bool IsSetRequestId()
        {
            return this.requestIdField != null;
        }

        /// <summary>
        /// XML Representation for this object
        /// </summary>
        /// <returns>XML String</returns>
        public string ToXML()
        {
            StringBuilder xml = new StringBuilder(1024);
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(this.GetType());
            using (StringWriter sw = new StringWriter(xml, CultureInfo.InvariantCulture))
            {
                serializer.Serialize(sw, this);
            }
            return xml.ToString();
        }
    }
}
