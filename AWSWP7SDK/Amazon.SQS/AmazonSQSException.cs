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

namespace Amazon.SQS
{
    /// <summary>
    /// Amazon SQS  Exception provides details of errors
    /// returned by Amazon SQS  service
    /// </summary>
    public class AmazonSQSException : Exception, ISQSResponse
    {
        private string message;
        private HttpStatusCode statusCode = default(HttpStatusCode);
        private string errorCode;
        private string errorType;
        private string requestId;
        private string xml;

        /// <summary>
        /// Initializes a new AmazonSQSException with default values.
        /// </summary>
        public AmazonSQSException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new AmazonSQSException with a specified
        /// error message
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        public AmazonSQSException(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// Initializes a new AmazonSQSException with a specified error message
        /// and HTTP status code
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        /// <param name="statusCode">HTTP status code for error response</param>
        public AmazonSQSException(string message, HttpStatusCode statusCode)
            : this(message)
        {
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new AmazonSQSException from the inner exception that is
        /// the cause of this exception.
        /// </summary>
        /// <param name="innerException">The nested exception that caused the AmazonSQSException</param>
        public AmazonSQSException(Exception innerException)
            : this(innerException.Message, innerException)
        {
        }

        /// <summary>
        /// Constructs AmazonSQSException with message and wrapped exception
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        /// <param name="innerException">The nested exception that caused the AmazonS3Exception</param>
        public AmazonSQSException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.message = message;
            AmazonSQSException ex = innerException as AmazonSQSException;
            if (ex != null)
            {
                this.statusCode = ex.StatusCode;
                this.errorCode = ex.ErrorCode;
                this.errorType = ex.ErrorType;
                this.requestId = ex.RequestId;
                this.xml = ex.XML;
            }
        }

        /// <summary>
        /// Initializes an AmazonSQSException with error information provided in an
        /// AmazonSQS response.
        /// </summary>
        /// <param name="message">A message that describes the error</param>
        /// <param name="statusCode">HTTP status code for error response</param>
        /// <param name="errorCode">Error Code returned by the service</param>
        /// <param name="errorType">Error type. Possible types:  Sender, Receiver or Unknown</param>
        /// <param name="requestId">Request ID returned by the service</param>
        /// <param name="xml">Compete xml found in response</param>
        public AmazonSQSException(string message, HttpStatusCode statusCode, string errorCode, string errorType, string requestId, string xml)
            : this(message, statusCode)
        {
            this.errorCode = errorCode;
            this.errorType = errorType;
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
