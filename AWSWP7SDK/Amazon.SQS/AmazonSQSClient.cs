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
 *
 *  AWS SDK for WP7
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Amazon.SQS.Model;
using Amazon.Util;
using Attribute = Amazon.SQS.Model.Attribute;
using Amazon.Runtime;

namespace Amazon.SQS
{
    /// <summary>
    /// AmazonSQSClient is an implementation of AmazonSQS;
    /// the client allows you to manage your AmazonSQS resources.<br />
    /// If you want to use the AmazonSQSClient from a Medium Trust
    /// hosting environment, please create the client with an
    /// AmazonSQSConfig object whose UseSecureStringForAwsSecretKey
    /// property is false.
    /// </summary>
    /// <remarks>
    /// Amazon Simple Queue Service (Amazon SQS) offers a reliable, highly scalable hosted queue for storing
    /// messages as they travel between computers. By using Amazon SQS, developers can simply move data between
    /// distributed application components performing different tasks, without losing messages or requiring each
    /// component to be always available.  Amazon SQS works by exposing Amazon's web-scale messaging infrastructure
    /// as a web service. Any computer on the Internet can add or read messages without any installed software or
    /// special firewall configurations. Components of applications using Amazon SQS can run independently, and do
    /// not need to be on the same network, developed with the same technologies, or running at the same time.
    /// </remarks>
    /// <seealso cref="P:Amazon.SQS.AmazonSQSConfig.UseSecureStringForAwsSecretKey"/>
    public delegate void SQSResponseEventHandler<TSender, T>(object sender, T result);

    public class AmazonSQSClient : AmazonSQS
    {
        private string awsAccessKeyId;
        private AmazonSQSConfig config;
        private bool disposed;
        private string clearAwsSecretAccessKey;

        public event SQSResponseEventHandler<object, ResponseEventArgs> OnSQSResponse;

        #region Dispose Pattern Implementation

        /// <summary>
        /// Implements the Dispose pattern for the AmazonSQSClient
        /// </summary>
        /// <param name="disposing">Whether this object is being disposed via a call to Dispose
        /// or garbage collected.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
            }
        }

        /// <summary>
        /// Disposes of all managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The destructor for the client class.
        /// </summary>
        ~AmazonSQSClient()
        {
            this.Dispose(false);
        }

        #endregion

        /// <summary>
        /// Create a client for the Amazon SQS Service with the credentials defined in the App.config.
        /// Example App.config with credentials set. 
        /// <?xml version="1.0" encoding="utf-8" ?>
        /// <configuration>
        ///     <appSettings>
        ///         <add key="AWSAccessKey" value="********************"/>
        ///         <add key="AWSSecretKey" value="****************************************"/>
        ///     </appSettings>
        /// </configuration>
        /// </summary>
        public AmazonSQSClient()
            : this(new EnvironmentAWSCredentials(), new AmazonSQSConfig())
        {
        }

        /// <summary>
        /// Constructs AmazonSQSClient with AWSCredentials.
        /// </summary>
        /// <param name="credentials"></param>
        public AmazonSQSClient(AWSCredentials credentials)
            : this(credentials, new AmazonSQSConfig())
        {
        }

        /// <summary>
        /// Constructs AmazonSQSClient with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        public AmazonSQSClient(string awsAccessKeyId, string awsSecretAccessKey)
            : this(awsAccessKeyId, awsSecretAccessKey, new AmazonSQSConfig())
        {
        }

        /// <summary>
        /// Constructs AmazonSQSClient with AWS Access Key ID, AWS Secret Key and an
        /// AmazonSQS Configuration object. If the config object's
        /// UseSecureStringForAwsSecretKey is false, the AWS Secret Key
        /// is stored as a clear-text string. Please use this option only
        /// if the application environment doesn't allow the use of SecureStrings.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        /// <param name="config">The AmazonSQS Configuration Object</param>
        public AmazonSQSClient(string awsAccessKeyId, string awsSecretAccessKey, AmazonSQSConfig config)
        {
            if (!String.IsNullOrEmpty(awsSecretAccessKey))
            {
                clearAwsSecretAccessKey = awsSecretAccessKey;
            }
            this.awsAccessKeyId = awsAccessKeyId;
            this.config = config;
        }

        private AmazonSQSClient(AWSCredentials credentials, AmazonSQSConfig config)
        {
            this.config = config;
            this.awsAccessKeyId = credentials.GetCredentials().AccessKey;
            this.clearAwsSecretAccessKey = credentials.GetCredentials().SecretKey;
        }

        #region Public API

        /// <summary>
        /// Create Queue 
        /// </summary>
        /// <param name="request">Create Queue  request</param>
        /// <returns>Create Queue  Response from the service</returns>
        /// <remarks>
        /// The CreateQueue action creates a new queue, or returns the URL of an existing one.
        /// When you request CreateQueue, you provide a name for the queue. To successfully create
        /// a new queue, you must provide a name that is unique within the scope of your own queues.
        /// If you provide the name of an existing queue, a new queue isn't created and an error
        /// isn't returned. Instead, the request succeeds and the queue URL for the existing queue is
        /// returned. Exception: if you provide a value for DefaultVisibilityTimeout that is different
        /// from the value for the existing queue, you receive an error.
        /// </remarks>
        public CreateQueueResponse CreateQueue(CreateQueueRequest request)
        {
            return Invoke<CreateQueueResponse>(ConvertCreateQueue(request));
        }

        /// <summary>
        /// List Queues 
        /// </summary>
        /// <param name="request">List Queues  request</param>
        /// <returns>List Queues  Response from the service</returns>
        /// <remarks>
        /// The ListQueues action returns a list of your queues.
        /// </remarks>
        public ListQueuesResponse ListQueues(ListQueuesRequest request)
        {
            return Invoke<ListQueuesResponse>(ConvertListQueues(request));
        }

        /// <summary>
        /// Add Permission 
        /// </summary>
        /// <param name="request">Add Permission  request</param>
        /// <returns>Add Permission  Response from the service</returns>
        /// <remarks>
        /// Adds the specified permission(s) to a queue for the specified principal(s). This allows for sharing access to the queue.
        /// </remarks>
        public AddPermissionResponse AddPermission(AddPermissionRequest request)
        {
            return Invoke<AddPermissionResponse>(ConvertAddPermission(request));
        }

        /// <summary>
        /// Change Message Visibility 
        /// </summary>
        /// <param name="request">Change Message Visibility  request</param>
        /// <returns>Change Message Visibility  Response from the service</returns>
        /// <remarks>
        /// The ChangeMessageVisibility action extends the read lock timeout of the specified message from the specified queue to the specified value.
        /// </remarks>
        public ChangeMessageVisibilityResponse ChangeMessageVisibility(ChangeMessageVisibilityRequest request)
        {
            return Invoke<ChangeMessageVisibilityResponse>(ConvertChangeMessageVisibility(request));
        }

        /// <summary>
        /// Delete Message 
        /// </summary>
        /// <param name="request">Delete Message  request</param>
        /// <returns>Delete Message  Response from the service</returns>
        /// <remarks>
        /// The DeleteMessage action unconditionally removes the specified message from the specified queue. Even if the message is locked by another reader due to the visibility timeout setting, it is still deleted from the queue.
        /// </remarks>
        public DeleteMessageResponse DeleteMessage(DeleteMessageRequest request)
        {
            return Invoke<DeleteMessageResponse>(ConvertDeleteMessage(request));
        }

        /// <summary>
        /// Delete Queue 
        /// </summary>
        /// <param name="request">Delete Queue  request</param>
        /// <returns>Delete Queue  Response from the service</returns>
        /// <remarks>
        /// This action unconditionally deletes the queue specified by the queue URL. Use this operation WITH CARE!  The queue is deleted even if it is NOT empty.
        /// </remarks>
        public DeleteQueueResponse DeleteQueue(DeleteQueueRequest request)
        {
            return Invoke<DeleteQueueResponse>(ConvertDeleteQueue(request));
        }

        /// <summary>
        /// Get Queue Attribute 
        /// </summary>
        /// <param name="request">Get Queue Attribute  request</param>
        /// <returns>Get Queue Attribute  Response from the service</returns>
        /// <remarks>
        /// Gets one or all attributes of a queue. Queues currently have two attributes you can get: ApproximateNumberOfMessages and VisibilityTimeout.
        /// </remarks>
        public GetQueueAttributesResponse GetQueueAttributes(GetQueueAttributesRequest request)
        {
            return Invoke<GetQueueAttributesResponse>(ConvertGetQueueAttributes(request));
        }

        /// <summary>
        /// Remove Permission 
        /// </summary>
        /// <param name="request">Remove Permission  request</param>
        /// <returns>Remove Permission  Response from the service</returns>
        /// <remarks>
        /// Removes the permission with the specified statement id from the queue.
        /// </remarks>
        public RemovePermissionResponse RemovePermission(RemovePermissionRequest request)
        {
            return Invoke<RemovePermissionResponse>(ConvertRemovePermission(request));
        }

        /// <summary>
        /// Receive Message 
        /// </summary>
        /// <param name="request">Receive Message  request</param>
        /// <returns>Receive Message  Response from the service</returns>
        /// <remarks>
        /// Retrieves one or more messages from the specified queue.  For each message returned, the response includes
        /// the message body; MD5 digest of the message body; receipt handle, which is the identifier you must provide
        /// when deleting the message; and message ID of each message. Message returned by this action stay in the queue
        /// until you delete them. However, once a message is returned to a ReceiveMessage request, it is not returned
        /// on subsequent ReceiveMessage requests for the duration of the VisibilityTimeout. If you do not specify a
        /// VisibilityTimeout in the request, the overall visibility timeout for the queue is used for the returned messages.
        /// </remarks>
        public ReceiveMessageResponse ReceiveMessage(ReceiveMessageRequest request)
        {
            return Invoke<ReceiveMessageResponse>(ConvertReceiveMessage(request));
        }

        /// <summary>
        /// Send Message 
        /// </summary>
        /// <param name="request">Send Message  request</param>
        /// <returns>Send Message  Response from the service</returns>
        /// <remarks>
        /// The SendMessage action delivers a message to the specified queue.
        /// </remarks>
        public SendMessageResponse SendMessage(SendMessageRequest request)
        {
            return Invoke<SendMessageResponse>(ConvertSendMessage(request));
        }

        /// <summary>
        /// Set Queue Attribute 
        /// </summary>
        /// <param name="request">Set Queue Attribute  request</param>
        /// <returns>Set Queue Attribute  Response from the service</returns>
        /// <remarks>
        /// Sets an attribute of a queue. Currently, you can set only the VisibilityTimeout attribute for a queue.
        /// </remarks>
        public SetQueueAttributesResponse SetQueueAttributes(SetQueueAttributesRequest request)
        {
            return Invoke<SetQueueAttributesResponse>(ConvertSetQueueAttributes(request));
        }

        #endregion

        #region Private API

        /**
         * Configure HttpClient with set of defaults as well as configuration
         * from AmazonSQSConfig instance
         */
        private static HttpWebRequest ConfigureWebRequest(string queueUrl, string queryString)
        {
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}?{1}", queueUrl, queryString));
            HttpWebRequest request = WebRequest.Create(uri) as HttpWebRequest;
            if (request != null)
            {
                request.Method = "GET";
            }
            return request;
        }

        /**
         * Invoke request and return response
         */
        private T Invoke<T>(IDictionary<string, string> parameters)
        {
            T response = default(T);
            string queueUrl = parameters.ContainsKey("QueueUrl") ? parameters["QueueUrl"] : config.ServiceURL;

            if (parameters.ContainsKey("QueueUrl"))
            {
                parameters.Remove("QueueUrl");
            }
            HttpStatusCode statusCode = default(HttpStatusCode);

            /* Add required request parameters */
            AddRequiredParameters(parameters, queueUrl);

            string queryString = AWSSDKUtils.GetParametersAsString(parameters);
            bool shouldRetry = true;
            int retries = 0;
            int maxRetries = config.IsSetMaxErrorRetry() ? config.MaxErrorRetry : AWSSDKUtils.DefaultMaxRetry;
            do
            {
                string responseBody = string.Empty;
                HttpWebRequest request = ConfigureWebRequest(queueUrl, queryString);
                /* Submit the request and read response body */
                try
                {
                    request.BeginGetResponse(asyncResponse =>
                               {
                                   try
                                   {
                                       HttpWebResponse asyncResult = (HttpWebResponse)request.EndGetResponse(asyncResponse);
                                       Stream streamResponse = asyncResult.GetResponseStream();

                                       using (StreamReader streamRead = new StreamReader(streamResponse))
                                       {
                                           responseBody = streamRead.ReadToEnd();
                                           XDocument doc = XDocument.Parse(responseBody);
                                           XmlSerializer serializer = new XmlSerializer(typeof(T));
                                           XmlReader xmlReader = doc.CreateReader();
                                           response = (T)serializer.Deserialize(xmlReader);
                                           // Disposing StreamReader automatically closes the underlying streams
                                       }

                                       OnSQSResponse.Invoke(this, new ResponseEventArgs(response as ISQSResponse));
                                   }
                                   catch (WebException we)
                                   {
                                       shouldRetry = false;
                                       using (HttpWebResponse httpErrorResponse = we.Response as HttpWebResponse)
                                       {
                                           if (httpErrorResponse == null)
                                           {
                                               // Abort the unsuccessful request
                                               request.Abort();
                                               throw;
                                           }
                                           statusCode = httpErrorResponse.StatusCode;
                                           using (StreamReader reader = new StreamReader(httpErrorResponse.GetResponseStream(), Encoding.UTF8))
                                           {
                                               responseBody = reader.ReadToEnd();
                                           }

                                           // Abort the unsuccessful request
                                           request.Abort();
                                       }

                                       if (statusCode == HttpStatusCode.InternalServerError ||
                                           statusCode == HttpStatusCode.ServiceUnavailable)
                                       {
                                           shouldRetry = true;
                                           PauseOnRetry(++retries, maxRetries, statusCode);
                                       }
                                       else
                                       {
                                           AmazonSQSException amazonException = null;
                                           /* Attempt to deserialize response into ErrorResponse type */
                                           try
                                           {
                                               XDocument doc = XDocument.Parse(responseBody);
                                               using (XmlReader sr = doc.CreateReader())
                                               {
                                                   XmlSerializer serializer = new XmlSerializer(typeof(ErrorResponse));
                                                   ErrorResponse errorResponse = (ErrorResponse)serializer.Deserialize(sr);
                                                   Error error = errorResponse.Error.ElementAt(0);

                                                   ///* Throw formatted exception with information available from the error response */
                                                   amazonException = new AmazonSQSException(
                                                       error.Message,
                                                       statusCode,
                                                       error.Code,
                                                       error.Type,
                                                       errorResponse.RequestId,
                                                       errorResponse.ToXML()
                                                       );
                                                   throw amazonException;
                                               }
                                           }
                                           /* Rethrow on deserializer error */
                                           catch (Exception e)
                                           {
                                               if (e is AmazonSQSException)
                                               {
                                                   if (null != OnSQSResponse)
                                                       OnSQSResponse.Invoke(this, new ResponseEventArgs(amazonException));
                                               }
                                               else
                                               {
                                                   if (null != OnSQSResponse)
                                                       OnSQSResponse.Invoke(this, new ResponseEventArgs(ReportAnyErrors(responseBody, statusCode)));
                                               }
                                           }
                                       }
                                   }
                               }, request);

                    shouldRetry = false;
                }
                /* Catch other exceptions, attempt to convert to formatted exception,
                 * else rethrow wrapped exception */
                catch (Exception)
                {
                    // Abort the unsuccessful request
                    request.Abort();
                    throw;
                }
            } while (shouldRetry);

            return response;
        }

        /**
         * Look for additional error strings in the response and return formatted exception
         */
        private static AmazonSQSException ReportAnyErrors(string responseBody, HttpStatusCode status)
        {
            AmazonSQSException ex = null;

            if (responseBody != null && responseBody.StartsWith("<", StringComparison.OrdinalIgnoreCase))
            {
                Match errorMatcherOne = Regex.Match(
                    responseBody,
                    "<RequestId>(.*)</RequestId>.*<Error><Code>(.*)</Code><Message>(.*)</Message></Error>.*(<Error>)?",
                    RegexOptions.Multiline);
                Match errorMatcherTwo = Regex.Match(
                    responseBody,
                    "<Error><Code>(.*)</Code><Message>(.*)</Message></Error>.*(<Error>)?.*<RequestID>(.*)</RequestID>",
                    RegexOptions.Multiline);

                if (errorMatcherOne.Success)
                {
                    string requestId = errorMatcherOne.Groups[1].Value;
                    string code = errorMatcherOne.Groups[2].Value;
                    string message = errorMatcherOne.Groups[3].Value;

                    ex = new AmazonSQSException(message, status, code, "Unknown", requestId, responseBody);
                }
                else if (errorMatcherTwo.Success)
                {
                    string code = errorMatcherTwo.Groups[1].Value;
                    string message = errorMatcherTwo.Groups[2].Value;
                    string requestId = errorMatcherTwo.Groups[4].Value;

                    ex = new AmazonSQSException(message, status, code, "Unknown", requestId, responseBody);
                }
                else
                {
                    ex = new AmazonSQSException("Internal Error", status);
                }
            }
            else
            {
                ex = new AmazonSQSException("Internal Error", status);
            }
            return ex;
        }

        /**
         * Exponential sleep on failed request
         */
        private static void PauseOnRetry(int retries, int maxRetries, HttpStatusCode status)
        {
            if (retries <= maxRetries)
            {
                int delay = (int)Math.Pow(4, retries) * 100;
                System.Threading.Thread.Sleep(delay);
            }
            else
            {
                throw new AmazonSQSException(
                    "Maximum number of retry attempts reached : " + (retries - 1),
                    status
                    );
            }
        }

        /**
         * Add authentication related and version parameters
         */
        private void AddRequiredParameters(IDictionary<string, string> parameters, string queueUrl)
        {
            if (String.IsNullOrEmpty(this.awsAccessKeyId))
            {
                throw new AmazonSQSException("The AWS Access Key ID cannot be NULL or a Zero length string");
            }

            parameters["AWSAccessKeyId"] = this.awsAccessKeyId;
            parameters["SignatureVersion"] = config.SignatureVersion;
            parameters["SignatureMethod"] = config.SignatureMethod;
            parameters["Timestamp"] = AWSSDKUtils.FormattedCurrentTimestampISO8601;
            parameters["Version"] = config.ServiceVersion;
            if (!config.SignatureVersion.Equals("2"))
            {
                throw new AmazonSQSException("Invalid Signature Version specified");
            }
            string toSign = AWSSDKUtils.CalculateStringToSignV2(parameters, queueUrl, "GET");

            using (KeyedHashAlgorithm algorithm = new HMACSHA256())
            {
                string auth = AWSSDKUtils.HMACSign(toSign, clearAwsSecretAccessKey, algorithm);
                parameters["Signature"] = auth;
            }
        }

        /**
         * Convert CreateQueueRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertCreateQueue(CreateQueueRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "CreateQueue";
            if (null == request.QueueName)
                throw new ArgumentNullException("QueueName", "The queue name cannot be null.");

            if (request.IsSetQueueName())
            {
                parameters["QueueName"] = request.QueueName;
            }
            if (request.IsSetDefaultVisibilityTimeout())
            {
                parameters["DefaultVisibilityTimeout"] = request.DefaultVisibilityTimeout.ToString(CultureInfo.InvariantCulture);
            }
            List<Attribute> createQueueRequestAttributeList = request.Attribute;
            int createQueueRequestAttributeListIndex = 1;
            foreach (Attribute createQueueRequestAttribute in createQueueRequestAttributeList)
            {
                if (createQueueRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", createQueueRequestAttributeListIndex, ".", "Name")] = createQueueRequestAttribute.Name;
                }
                if (createQueueRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", createQueueRequestAttributeListIndex, ".", "Value")] = createQueueRequestAttribute.Value;
                }

                createQueueRequestAttributeListIndex++;
            }

            return parameters;
        }

        /**
         * Convert ListQueuesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertListQueues(ListQueuesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "ListQueues";
            if (request.IsSetQueueNamePrefix())
            {
                parameters["QueueNamePrefix"] = request.QueueNamePrefix;
            }
            List<Attribute> listQueuesRequestAttributeList = request.Attribute;
            int listQueuesRequestAttributeListIndex = 1;
            foreach (Attribute listQueuesRequestAttribute in listQueuesRequestAttributeList)
            {
                if (listQueuesRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", listQueuesRequestAttributeListIndex, ".", "Name")] = listQueuesRequestAttribute.Name;
                }
                if (listQueuesRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", listQueuesRequestAttributeListIndex, ".", "Value")] = listQueuesRequestAttribute.Value;
                }

                listQueuesRequestAttributeListIndex++;
            }

            return parameters;
        }

        /**
         * Convert ChangeMessageVisibilityRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertChangeMessageVisibility(ChangeMessageVisibilityRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "ChangeMessageVisibility";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            if (request.IsSetReceiptHandle())
            {
                parameters["ReceiptHandle"] = request.ReceiptHandle;
            }
            if (request.IsSetVisibilityTimeout())
            {
                parameters["VisibilityTimeout"] = request.VisibilityTimeout.ToString(CultureInfo.InvariantCulture);
            }
            List<Attribute> changeMessageVisibilityRequestAttributeList = request.Attribute;
            int changeMessageVisibilityRequestAttributeListIndex = 1;
            foreach (Attribute changeMessageVisibilityRequestAttribute in changeMessageVisibilityRequestAttributeList)
            {
                if (changeMessageVisibilityRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", changeMessageVisibilityRequestAttributeListIndex, ".", "Name")] = changeMessageVisibilityRequestAttribute.Name;
                }
                if (changeMessageVisibilityRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", changeMessageVisibilityRequestAttributeListIndex, ".", "Value")] = changeMessageVisibilityRequestAttribute.Value;
                }

                changeMessageVisibilityRequestAttributeListIndex++;
            }

            return parameters;
        }

        /**
         * Convert DeleteMessageRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertDeleteMessage(DeleteMessageRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "DeleteMessage";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            if (request.IsSetReceiptHandle())
            {
                parameters["ReceiptHandle"] = request.ReceiptHandle;
            }
            List<Attribute> deleteMessageRequestAttributeList = request.Attribute;
            int deleteMessageRequestAttributeListIndex = 1;
            foreach (Attribute deleteMessageRequestAttribute in deleteMessageRequestAttributeList)
            {
                if (deleteMessageRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", deleteMessageRequestAttributeListIndex, ".", "Name")] = deleteMessageRequestAttribute.Name;
                }
                if (deleteMessageRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", deleteMessageRequestAttributeListIndex, ".", "Value")] = deleteMessageRequestAttribute.Value;
                }

                deleteMessageRequestAttributeListIndex++;
            }

            return parameters;
        }

        /**
         * Convert DeleteQueueRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertDeleteQueue(DeleteQueueRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "DeleteQueue";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl.ToString();
            }
            List<Attribute> deleteQueueRequestAttributeList = request.Attribute;
            int deleteQueueRequestAttributeListIndex = 1;
            foreach (Attribute deleteQueueRequestAttribute in deleteQueueRequestAttributeList)
            {
                if (deleteQueueRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", deleteQueueRequestAttributeListIndex, ".", "Name")] = deleteQueueRequestAttribute.Name;
                }
                if (deleteQueueRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", deleteQueueRequestAttributeListIndex, ".", "Value")] = deleteQueueRequestAttribute.Value;
                }

                deleteQueueRequestAttributeListIndex++;
            }

            return parameters;
        }

        /**
         * Convert GetQueueAttributesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertGetQueueAttributes(GetQueueAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "GetQueueAttributes";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            List<string> getQueueAttributesRequestAttributeNameList = request.AttributeName;
            int getQueueAttributesRequestAttributeNameListIndex = 1;
            foreach (string getQueueAttributesRequestAttributeName in getQueueAttributesRequestAttributeNameList)
            {
                parameters[String.Concat("AttributeName", ".", getQueueAttributesRequestAttributeNameListIndex)] = getQueueAttributesRequestAttributeName;
                getQueueAttributesRequestAttributeNameListIndex++;
            }

            return parameters;
        }

        /**
         * Convert ReceiveMessageRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertReceiveMessage(ReceiveMessageRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "ReceiveMessage";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            if (request.IsSetMaxNumberOfMessages())
            {
                parameters["MaxNumberOfMessages"] = request.MaxNumberOfMessages.ToString(CultureInfo.InvariantCulture);
            }
            if (request.IsSetVisibilityTimeout())
            {
                parameters["VisibilityTimeout"] = request.VisibilityTimeout.ToString(CultureInfo.InvariantCulture);
            }
            List<string> receiveMessageRequestAttributeNameList = request.AttributeName;
            int receiveMessageRequestAttributeNameListIndex = 1;
            foreach (string receiveMessageRequestAttributeName in receiveMessageRequestAttributeNameList)
            {
                parameters[String.Concat("AttributeName", ".", receiveMessageRequestAttributeNameListIndex)] = receiveMessageRequestAttributeName;
                receiveMessageRequestAttributeNameListIndex++;
            }

            return parameters;
        }

        /**
         * Convert SendMessageRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertSendMessage(SendMessageRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "SendMessage";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            if (request.IsSetMessageBody())
            {
                parameters["MessageBody"] = request.MessageBody;
            }

            return parameters;
        }

        /**
         * Convert SetQueueAttributesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertSetQueueAttributes(SetQueueAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "SetQueueAttributes";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            List<Attribute> setQueueAttributesRequestAttributeList = request.Attribute;
            int setQueueAttributesRequestAttributeListIndex = 1;
            foreach (Attribute setQueueAttributesRequestAttribute in setQueueAttributesRequestAttributeList)
            {
                if (setQueueAttributesRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", setQueueAttributesRequestAttributeListIndex, ".", "Name")] = setQueueAttributesRequestAttribute.Name;
                }
                if (setQueueAttributesRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", setQueueAttributesRequestAttributeListIndex, ".", "Value")] = setQueueAttributesRequestAttribute.Value;
                }

                setQueueAttributesRequestAttributeListIndex++;
            }

            return parameters;
        }

        /**
         * Convert AddPermissionRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertAddPermission(AddPermissionRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "AddPermission";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            if (request.IsSetLabel())
            {
                parameters["Label"] = request.Label;
            }
            List<string> addPermissionRequestAWSAccountIdList = request.AWSAccountId;
            int addPermissionRequestAWSAccountIdListIndex = 1;
            foreach (string addPermissionRequestAWSAccountId in addPermissionRequestAWSAccountIdList)
            {
                parameters[String.Concat("AWSAccountId", ".", addPermissionRequestAWSAccountIdListIndex)] = addPermissionRequestAWSAccountId;
                addPermissionRequestAWSAccountIdListIndex++;
            }
            List<string> addPermissionRequestActionNameList = request.ActionName;
            int addPermissionRequestActionNameListIndex = 1;
            foreach (string addPermissionRequestActionName in addPermissionRequestActionNameList)
            {
                parameters[String.Concat("ActionName", ".", addPermissionRequestActionNameListIndex)] = addPermissionRequestActionName;
                addPermissionRequestActionNameListIndex++;
            }

            return parameters;
        }

        /**
         * Convert RemovePermissionRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertRemovePermission(RemovePermissionRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "RemovePermission";
            if (request.IsSetQueueUrl())
            {
                parameters["QueueUrl"] = request.QueueUrl;
            }
            if (request.IsSetLabel())
            {
                parameters["Label"] = request.Label;
            }

            return parameters;
        }

        #endregion
    }
}
