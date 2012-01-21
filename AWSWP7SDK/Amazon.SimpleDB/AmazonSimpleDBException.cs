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
using System.Net;

namespace Amazon.SimpleDB
{
    /// <summary>
    /// Amazon Simple DB  Exception provides details of errors
    /// returned by Amazon Simple DB  service
    /// </summary>   
    public class AmazonSimpleDBException : Exception, ISimpleDBResponse
    {
        private string message;
        private HttpStatusCode statusCode = default(HttpStatusCode);
        private string errorCode;
        private string errorType;
        private string boxUsage;
        private string requestId;
        private string xml;

        /// <summary>
        /// Gets a bool value indicating if the request was successful.
        /// </summary>
        public bool IsRequestSuccessful { get; protected set; }

        /// <summary>
        /// Initializes a new AmazonSimpleDBException with default values.
        /// </summary>
        public AmazonSimpleDBException()
            : base()
        {
            this.IsRequestSuccessful = false;
        }

        /// <summary>
        /// Initializes a new AmazonSimpleDBException with a specified
        /// error message
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        public AmazonSimpleDBException(string message)
            : this()
        {
            this.message = message;
        }

        /// <summary>
        /// Constructs AmazonSimpleDBException with message and status code
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        /// <param name="statusCode">HTTP status code for error response</param>
        public AmazonSimpleDBException(string message, HttpStatusCode statusCode)
            : this(message)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new AmazonSimpleDBException from the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="innerException">The nested exception that caused the AmazonS3Exception</param>
        public AmazonSimpleDBException(Exception innerException)
            : this(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Constructs AmazonSimpleDBException with message and wrapped exception
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        /// <param name="innerException">The nested exception that caused the AmazonS3Exception</param>
        public AmazonSimpleDBException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.message = message;
            AmazonSimpleDBException ex = innerException as AmazonSimpleDBException;
            if (ex != null)
            {
                this.statusCode = ex.StatusCode;
                this.errorCode = ex.ErrorCode;
                this.errorType = ex.ErrorType;
                this.boxUsage = ex.BoxUsage;
                this.requestId = ex.RequestId;
                this.xml = ex.XML;
            }
        }

        /// <summary>
        /// Constructs AmazonSimpleDBException with information available from service
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        /// <param name="statusCode">HTTP status code for error response</param>
        /// <param name="errorCode">Error Code returned by the service</param>
        /// <param name="errorType">Error type. Possible types:  Sender, Receiver or Unknown</param>
        /// <param name="boxUsage">Measure of machine utilization for this request</param>
        /// <param name="requestId">Request ID returned by the service</param>
        /// <param name="xml">Compete xml found in response</param>
        public AmazonSimpleDBException(string message, HttpStatusCode statusCode, string errorCode, string errorType, string boxUsage, string requestId, string xml)
            : this(message, statusCode)
        {
            this.errorCode = errorCode;
            this.errorType = errorType;
            this.boxUsage = boxUsage;
            this.requestId = requestId;
            this.xml = xml;
        }

        /// <summary>
        /// Gets and sets of the ErrorCode property.
        /// </summary>
        public string ErrorCode
        {
            get { return this.errorCode; }
        }

        /// <summary>
        /// Gets and sets of the ErrorType property.
        /// </summary>
        public string ErrorType
        {
            get { return this.errorType; }
        }

        /// <summary>
        /// Gets error message
        /// </summary>
        public override string Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets status code returned by the service if available. If status
        /// code is set to -1, it means that status code was unavailable at the
        /// time exception was thrown
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return this.statusCode; }
        }

        /// <summary>
        /// Gets measure of machine utilization for this request
        /// </summary>
        public string BoxUsage
        {
            get { return this.boxUsage; }
        }

        /// <summary>
        /// Gets XML returned by the service if available.
        /// </summary>
        public string XML
        {
            get { return this.xml; }
        }

        /// <summary>
        /// Gets Request ID returned by the service if available.
        /// </summary>
        public string RequestId
        {
            get { return this.requestId; }
        }
    }
}
