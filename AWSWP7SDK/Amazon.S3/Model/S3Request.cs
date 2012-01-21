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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace Amazon.S3.Model
{
    using System.Globalization;
    using Map = System.Collections.Generic.IDictionary<Amazon.S3.Model.S3QueryParameter, string>;

    /// <summary>
    /// Base class for all S3 operation requests.
    /// Provides a header collection which can is used to store the request headers.
    /// </summary>
    public class S3Request
    {
        #region Private Members

        private WebHeaderCollection headers;
        private Stream inputStream;

        // Most requests have less than 10 parameters, so 10 is a safe starting capacity
        // This way, the Map.Add operation will be an O(1) operation
        internal Map parameters = new Dictionary<S3QueryParameter, string>(10);

        // The maximum number of headers removed from an S3 Request is 2:
        // If-Modified and Content-Type. Since this is such a small number, 
        // we choose not to allocate memory for all requests.
        internal Dictionary<string, string> removedHeaders = new Dictionary<string,string>();

        #endregion

        #region Headers

        /// <summary>
        /// Gets the Headers property.
        /// </summary>
        internal WebHeaderCollection Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new WebHeaderCollection();                    
                }
                return this.headers;
            }
        }

        #endregion

        #region InputStream
        /// <summary>
        /// Gets and sets the InputStream property.
        /// </summary>
        [XmlElementAttribute(ElementName = "InputStream")]
        public Stream InputStream
        {
            get { return this.inputStream; }
            set { this.inputStream = value; }
        }

        /// <summary>
        /// Sets the InputStream property.
        /// </summary>
        /// <param name="inputStream">InputStream property</param>
        /// <returns>this instance</returns>
        public S3Request WithInputStream(Stream inputStream)
        {
            this.inputStream = inputStream;
            return this;
        }

        /// <summary>
        /// Checks if InputStream property is set.
        /// </summary>
        /// <returns>true if InputStream property is set.</returns>
        internal bool IsSetInputStream()
        {
            return this.inputStream != null;
        }

        #endregion

        #region Properties

        private Guid id = Guid.NewGuid();
        public Guid Id { get { return this.id; } }

        public TimeSpan TotalRequestTime { get; set; }
        public TimeSpan ResponseReadTime { get; set; }
        public TimeSpan ResponseProcessingTime { get; set; }
        public TimeSpan ResponseTime { get; set; }
        public long BytesProcessed { get; set; }

        public TimeSpan MissingTime
        {
            get
            {
                return (TotalRequestTime - (ResponseReadTime + ResponseProcessingTime + ResponseTime));
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            string contents = string.Format(CultureInfo.InvariantCulture, "S3Request: Type - {0}, ID - {1}, ResponseTime - {2}, ResponseReadTime - {3}, ResponseProcessingTime - {4}, TotalRequestTime - {5}, Unaccounted time - {6}, Bytes processed - {7}",
                this.GetType().FullName,
                this.Id,
                this.ResponseTime,
                this.ResponseReadTime,
                this.ResponseProcessingTime,
                this.TotalRequestTime,
                this.MissingTime,
                this.BytesProcessed);
            return contents;
        }

        #endregion

        #region Virtual methods

        internal virtual bool SupportTimeout
        {
            get { return false; }
        }

        internal virtual bool Expect100Continue
        {
            get { return false; }
        }

        internal virtual void OnRaiseProgressEvent(long incrementTransferred, long transferred, long total)
        {
        }

        #endregion
    }
}