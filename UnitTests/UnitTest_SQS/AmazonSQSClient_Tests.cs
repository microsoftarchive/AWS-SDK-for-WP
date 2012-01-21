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

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Amazon.SQS;
using Amazon;
using Amazon.SQS.Model;
using System.Collections;
using System.Collections.Generic;

namespace UnitTest_SQS
{
    /// <summary>
    /// Represents a test class to test the AmazonSQSClient class.
    /// </summary>
    [TestClass]
    public class AmazonSQSClient_Tests : SilverlightTest
    {
        private const string Access_Key_ID = "Insert your Amazon Web Services Access Key ID here";
        private const string Secret_Access_Key = "Insert your Amazon Web Services Secret Access Key here";

        private string _queue_UnitTesting = "Queue_UnitTesting";
        private string _queue_UnitTesting_1 = "Queue_UnitTesting_1";
        private string _queueWithInvalidAttributeName = "QueueWithInvalidAttributeName";
        private AmazonSQS _client;
        private AmazonSQSConfig _sqsConfig;

        /// <summary>
        /// Gets the ServiceURL of the Queue - e.g. Example Queue URL https://queue.amazonaws.com
        /// </summary>
        /// <remarks>
        /// The service URL is obtained from the AmazonSQSConfig object 
        /// </remarks>
        private string QueueURL
        {
            get
            {
                if (_sqsConfig == null)
                {
                    _sqsConfig = new AmazonSQSConfig();
                }
                return _sqsConfig.ServiceURL;
            }
        }


        /// <summary>
        /// A place to initialize the test class.
        /// </summary>
        [ClassInitialize]
        [Asynchronous]
        public void SetUp()
        {
            _client = AWSClientFactory.CreateAmazonSQSClient(Access_Key_ID, Secret_Access_Key);

            //Ensure we create a test queue for the test cases. And remember to delete it in the teardown method.
            bool hasCallbackArrived = false;
            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;
            _client.CreateQueue(new CreateQueueRequest { QueueName = _queue_UnitTesting, DefaultVisibilityTimeout = 3 });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Clean up code before the test exits for the class.
        /// </summary>
        [ClassCleanup]
        [Asynchronous]
        public void TearDown()
        {
            bool hasCallbackArrived = false;
            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event
                _client.OnSQSResponse -= handler;
                hasCallbackArrived = true;
            };
            _client.OnSQSResponse += handler;
            _client.DeleteQueue(new DeleteQueueRequest { QueueUrl = ((string.Format("{0}/{1}", QueueURL, _queue_UnitTesting))) });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueTestComplete();
        }

        #region List Queue Test

        /// <summary>
        /// Tests the List-Queues API. The test passes by checking the non-null response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_A_ListQueues_And_Check_For_NonNull_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                ListQueuesResponse response = result as ListQueuesResponse;
                if (null != response)
                    actualValue = true;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            ListQueuesRequest listQueue = new ListQueuesRequest();
            _client.ListQueues(listQueue);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        #endregion List Queue Test

        #region Create Queue

        /// <summary>
        /// Tests the Create-Queue API. The test passes by checking a valid response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_B_CreateQueue_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                CreateQueueResponse response = result as CreateQueueResponse;
                if (null != response)
                {
                    CreateQueueResult createResult = response.CreateQueueResult;
                    if (null != createResult)
                        actualValue = true;
                }

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            _client.CreateQueue(new CreateQueueRequest { QueueName = _queue_UnitTesting_1 });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Create-Queue API again with an already existing queue-name. The test passes by expecting an valid response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_C_CreateQueue_With_AlreadyExisting_Queue_Name_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                CreateQueueResponse response = result as CreateQueueResponse;
                if (null != response)
                {
                    CreateQueueResult createResult = response.CreateQueueResult;
                    if (null != createResult)
                        actualValue = true;
                }

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            _client.CreateQueue(new CreateQueueRequest { QueueName = _queue_UnitTesting_1 });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Create-Queue API with Queue name having special characters. The test passes by checking an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_CreateQueue_With_QueueName_Having_SpecialCharacters_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidParameterValue";
            string queueNameWithSpecialChars = "queue&";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                {
                    actualValue = exception.ErrorCode;
                }

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            _client.CreateQueue(new CreateQueueRequest { QueueName = queueNameWithSpecialChars });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Create-Queue API with Null Queue name. The test passes by expecting an ArgumentNullException thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CreateQueue_With_Null_QueueName_And_Expect_Exception()
        {
            //Create request object.
            _client.CreateQueue(new CreateQueueRequest { QueueName = null });
        }

        /// <summary>
        /// Tests the Create-Queue API with a valid Queue name and an invalid Attribute name. The test passes by checking a valid response from the server.
        /// NOTE: INVALID ATTRIBUTES ARE IGNORED AND CREATION OF QUEUE IS CARRIED OUT WITHOUT ANY EXCEPTION.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_CreateQueue_With_Invalid_Attribute_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                CreateQueueResponse response = result as CreateQueueResponse;
                if (null != response)
                {
                    if (null != response.CreateQueueResult)
                        actualValue = true;
                }

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            CreateQueueRequest request = new CreateQueueRequest
            {
                QueueName = _queueWithInvalidAttributeName
                /*Attribute = new List<Amazon.SQS.Model.Attribute> { new Amazon.SQS.Model.Attribute { Name = "Hello", Value = "Test_Value" } } */
            };
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "Hello", Value = "Test_Value" });

            _client.CreateQueue(request);


            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Create-Queue API with an Empty Queue name. The test passes by checking an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_CreateQueue_With_EmptyQueueName_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidParameterValue";
            string emptyQueueName = string.Empty;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            _client.CreateQueue(new CreateQueueRequest
            {
                QueueName = emptyQueueName,
            });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        #endregion Create Queue

        #region Delete Queue

        /// <summary>
        /// Tests the Delete-Queue API. The test passes by checking a valid response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_E_DeleteQueue_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                DeleteQueueResponse response = result as DeleteQueueResponse;
                if (null != response)
                {
                    actualValue = true;
                }

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            _client.DeleteQueue(new DeleteQueueRequest { QueueUrl = ((string.Format("{0}/{1}", QueueURL, _queue_UnitTesting_1))) });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        #endregion Delete Queue

        #region Get-Queue-Attribute Test
        /// <summary>
        /// Tests the Get-Queues-Attribute API. The test passes by checking the value returned from the server. The attribute checked is 'VisibilityTimeout'. The 
        /// expected value is 30.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetQueueAttributes_And_Check_For_Valid_Value()
        {
            bool hasCallbackArrived = false;
            string actualValue = "-1";
            string expectedValue = "3";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                GetQueueAttributesResponse response = result as GetQueueAttributesResponse;
                if (null != response)
                {
                    GetQueueAttributesResult attributeResult = response.GetQueueAttributesResult;
                    if (null != attributeResult)
                    {
                        if (response.GetQueueAttributesResult.Attribute.Count > 0)
                            actualValue = response.GetQueueAttributesResult.Attribute[0].Value;
                    }
                }

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            GetQueueAttributesRequest request = new GetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.AttributeName.Add("VisibilityTimeout");
            _client.GetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Queues-Attribute API with a non-existing-attribute. The test passes by expecting an error message from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetQueueAttributes_With_NonExisting_Attribute_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidAttributeName";
            string nonExistingAttribute = "poiu";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            GetQueueAttributesRequest request = new GetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.AttributeName.Add(nonExistingAttribute);
            _client.GetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        #endregion Get-Queue-Attribute Test

        #region Send-Message Test

        /// <summary>
        /// Tests the Send-Message API to a valid queue. The test passes by expecting a valid response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_A_SendMessage_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;
            string messageBody = "A test message body during Unit-Testing";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                SendMessageResponse response = result as SendMessageResponse;
                if (null != response)
                    if (null != response.SendMessageResult)
                        actualValue = true;


                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SendMessageRequest request = new SendMessageRequest
            {
                QueueUrl = (string.Format("{0}/{1}", QueueURL, _queue_UnitTesting)),
                MessageBody = messageBody
            };
            _client.SendMessage(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Send-Message API to a non-existing queue. The test passes by expecting an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SendMessage_To_NonExisting_Queue_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "AWS.SimpleQueueService.NonExistentQueue";
            string nonExistingQueue = "nonExistingQueue";
            string messageBody = "A test message body during Unit-Testing";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SendMessageRequest request = new SendMessageRequest
            {
                QueueUrl = (string.Format("{0}/{1}", QueueURL, nonExistingQueue)),
                MessageBody = messageBody
            };
            _client.SendMessage(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Send-Message API with a queue-name having special characters. The test passes by expecting an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [Ignore]
        public void Test_SendMessage_To_Queue_With_SpecialCharacters_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "";
            string queueNameWithSpecialCharacters = "hello%";
            string messageBody = "A test message body during Unit-Testing";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;


                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SendMessageRequest request = new SendMessageRequest
            {
                QueueUrl = (string.Format("{0}/{1}", QueueURL, queueNameWithSpecialCharacters)),
                MessageBody = messageBody
            };
            _client.SendMessage(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Send-Message API with an invalid queue-url. The test passes by expecting an error response from the server. The error is NotFound.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SendMessage_With_InvalidQueueURL_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "NotFound";
            string expectedValue2 = "BadGateway";
            string invalidQueueUrl = "https://queue.amazonawsabcdpoiu.com";
            string messageBody = "A test message body during Unit-Testing";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.StatusCode.ToString();

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SendMessageRequest request = new SendMessageRequest
            {
                QueueUrl = (string.Format("{0}/{1}", invalidQueueUrl, _queue_UnitTesting)),
                MessageBody = messageBody
            };
            _client.SendMessage(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue((expectedValue == actualValue) || (expectedValue2 == actualValue)));
            EnqueueTestComplete();
        }

        #endregion Send-Message Test

        #region Recieve-Message Test

        /// <summary>
        /// Tests the Receive-Message API with a valid queue. The test passes by expecting messages returned for the Queue from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_ReceiveMessage_With_ValidQueue_And_Check_For_ReceivedMessage()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "A test message body during Unit-Testing";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                ReceiveMessageResponse response = result as ReceiveMessageResponse;
                if (null != response)
                {
                    ReceiveMessageResult messageResult = response.ReceiveMessageResult;
                    if (null != messageResult)
                    {
                        if (messageResult.Message.Count > 0)
                        {
                            Message message = messageResult.Message[0];
                            if (null != message)
                                actualValue = message.Body;
                        }
                    }
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                QueueUrl = (string.Format("{0}/{1}", QueueURL, _queue_UnitTesting)),
            };
            _client.ReceiveMessage(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Receive-Message API with a non-existing queue. The test passes by expecting an error message from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_ReceiveMessage_With_NonExistingQueue_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "AWS.SimpleQueueService.NonExistentQueue";
            string invalidQueueName = "InvalidQueueName_poiu";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            ReceiveMessageRequest request = new ReceiveMessageRequest
            {
                QueueUrl = (string.Format("{0}/{1}", QueueURL, invalidQueueName)),
            };
            _client.ReceiveMessage(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        #endregion Recieve-Message Test

        #region Set Attribute Test

        /// <summary>
        /// Tests the Set-Queues-Attribute API for a valid-attribute. The test passes by expecting an valid response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SetQueueAttributes_With_Valid_Attribute_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                SetQueueAttributesResponse response = result as SetQueueAttributesResponse;
                if (null != response)
                    actualValue = true;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SetQueueAttributesRequest request = new SetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "MaximumMessageSize", Value = "5000" });
            _client.SetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Set-Queues-Attribute API with an invalid-attribute. The test passes by expecting an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SetQueueAttributes_With_InValid_Attribute_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidAttributeName";
            string invalidAttribute = "attribute_poiu";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SetQueueAttributesRequest request = new SetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = invalidAttribute, Value = "500" });
            _client.SetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Set-Queues-Attribute API with an invalid-attribute-value. The test passes by expecting an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SetQueueAttributes_With_InValid_Attribute_Value_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidAttributeValue";
            string invalidAttributeValue = "4";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SetQueueAttributesRequest request = new SetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "MaximumMessageSize", Value = invalidAttributeValue });
            _client.SetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Set-Queues-Attribute API with an multiple-attributes and their values. The test passes by expecting an valid response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SetQueueAttributes_With_Multiple_Attributes_And_Check_For_Valid_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                SetQueueAttributesResponse response = result as SetQueueAttributesResponse;
                if (null != response)
                    actualValue = true;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SetQueueAttributesRequest request = new SetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "VisibilityTimeout", Value = "3" });
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "MaximumMessageSize", Value = "6000" });
            _client.SetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Set-Queues-Attribute API with an multiple-attributes and their values with one invalid attribute. The test passes by expecting an error response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SetQueueAttributes_With_Multiple_Attributes_With_One_InvalidAttribute_And_Check_For_Error_Response()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidAttributeName";

            SQSResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                ISQSResponse result = args.Response;
                //Unhook from event.
                _client.OnSQSResponse -= handler;
                AmazonSQSException exception = result as AmazonSQSException;
                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSQSResponse += handler;

            //Create request object.
            SetQueueAttributesRequest request = new SetQueueAttributesRequest();
            request.QueueUrl = string.Format("{0}/{1}", QueueURL, _queue_UnitTesting);
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "VisibilityTimeout", Value = "3" });
            request.Attribute.Add(new Amazon.SQS.Model.Attribute { Name = "invalidAttribute", Value = "6000" });
            _client.SetQueueAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        #endregion Set Attribute Test

    }
}
