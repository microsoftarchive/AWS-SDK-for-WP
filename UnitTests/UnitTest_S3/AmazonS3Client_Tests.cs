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
using Amazon;
using System.Collections.Generic;
using Amazon.S3.Model;
using Amazon.S3;
using System.IO;
using System.Threading;

namespace UnitTest_S3
{
    [TestClass]
    public class AmazonS3Client_Tests : SilverlightTest
    {
        #region Private Variables

        private const string Access_Key_ID = "Insert your Amazon Web Services Access Key ID here";
        private const string Secret_Access_Key = "Insert your Amazon Web Services Secret Access Key here";

        //Please visit http://docs.amazonwebservices.com/AmazonS3/latest/dev/ and browse to 
        //Access Control--> Using ACLs --> Access Control List (ACL) Overview on the left side 
        // of the page. Find the topic 'To find Canonical User ID' and click 'Security Credentials' just below it.
        private const string CanonicalUserID = "Insert your Amazon Web Services Canonical User ID here";

        private AmazonS3 _client = null;
        private string _bucketName = "Bucket_UnitTesting";
        private string _bucketNameDestination = "Bucket_UnitTesting_Destination";
        private string _key = "key_UnitTesting";
        private string _bucketNameForBucketAPIs = "BucketForBucketAPIUnitTest";

        #endregion

        #region Setup Functions

        ///// <summary>
        ///// Clean up code before the test exits for the class.
        ///// </summary>
        [ClassCleanup]
        [Asynchronous]
        public void TearDown()
        {
            //A post-requisite for testing S3 objects. Ensure that all the Buckets and objects created are deleted before we exit the test-class.
            bool hasCallbackArrived = false;

            //B. Delete the destination object.
            hasCallbackArrived = false;
            S3ResponseEventHandler<object, ResponseEventArgs> deleteObjectHandler = null;

            deleteObjectHandler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                _client.OnS3Response -= deleteObjectHandler;
                hasCallbackArrived = true;
            };

            _client.OnS3Response += deleteObjectHandler;
            DeleteObjectRequest objectrequest = new DeleteObjectRequest { BucketName = _bucketNameDestination, Key = "key_UnitTesting_destination_1" };
            _client.DeleteObject(objectrequest);

            EnqueueConditional(() => hasCallbackArrived);

            //A. Delete the normal bucket.
            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                if (result is DeleteBucketResponse)
                {
                    //Unhook from event.
                    _client.OnS3Response -= handler;
                    hasCallbackArrived = true;
                }
            };
            _client.OnS3Response += handler;

            DeleteBucketRequest request = new DeleteBucketRequest() { BucketName = _bucketName };
            _client.DeleteBucket(request);

            EnqueueConditional(() => hasCallbackArrived);

            //Delete the destination Bucket
            hasCallbackArrived = false;
            S3ResponseEventHandler<object, ResponseEventArgs> deleteBucketHandler = null;

            deleteBucketHandler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response bucketResult = args.Response;
                if (bucketResult is DeleteBucketResponse)
                {
                    //Unhook from event.
                    _client.OnS3Response -= deleteBucketHandler;
                    hasCallbackArrived = true;

                    //Finally, set the client as null.
                    _client = null;
                }
            };

            _client.OnS3Response += deleteBucketHandler;
            DeleteBucketRequest deleteRequest = new DeleteBucketRequest() { BucketName = _bucketNameDestination };
            _client.DeleteBucket(deleteRequest);

            EnqueueConditional(() => hasCallbackArrived);

            EnqueueTestComplete();
        }

        /// <summary>
        /// A place to initialize the test class.
        /// </summary>
        [ClassInitialize]
        [Asynchronous]
        public void SetUp()
        {
            _client = AWSClientFactory.CreateAmazonS3Client(Access_Key_ID, Secret_Access_Key);

            //A pre-requisite for testing S3 Objects. Ensure that we create two temporary buckets to test the Objects. 
            //One for normal operations, other for Copying to the destination bucket.
            //A. Create the bucket.
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            PutBucketRequest request = new PutBucketRequest() { BucketName = _bucketName };
            _client.PutBucket(request);

            EnqueueConditional(() => hasCallbackArrived);

            //B. Create the destination bucket as well.
            bool hasDestinationCallbackArrived = false;
            S3ResponseEventHandler<object, ResponseEventArgs> destinationHandler = null;
            destinationHandler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= destinationHandler;
                hasDestinationCallbackArrived = true;
            };
            _client.OnS3Response += destinationHandler;

            PutBucketRequest requestDestination = new PutBucketRequest() { BucketName = _bucketNameDestination };
            _client.PutBucket(requestDestination);

            EnqueueConditional(() => hasDestinationCallbackArrived);

            EnqueueTestComplete();
        }

        #endregion

        #region Bucket Test Cases

        ///// <summary>
        ///// List Bucket Test. The test passes, when the result is of type ListBucketsResponse
        ///// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketA_ListBucketTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                ListBucketsResponse response = result as ListBucketsResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };

            ListBucketsRequest request = new ListBucketsRequest();
            Guid id = request.Id;
            _client.OnS3Response += handler;
            _client.ListBuckets();

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Put Bucket Test. The test passes, when the result is of type PutBucketResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketB_PutBucketTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                PutBucketResponse response = result as PutBucketResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            PutBucketRequest request = new PutBucketRequest() { BucketName = _bucketNameForBucketAPIs };
            _client.PutBucket(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Put Bucket Test. This test is to check the result, when the empty bucket name is passed in request 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BucketB_PutBucketTest_ForException_EmptyBucketName()
        {
            string actualValue = string.Empty;
            string emptyBucketName = string.Empty;

            //Create request object.
            PutBucketRequest request = new PutBucketRequest { BucketName = emptyBucketName };
            _client.PutBucket(request);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Set ACLForBucket Test. 
        /// This test sets the base for Bucket Logging APIs. This test sets the permissions for 
        /// LogDelivery group, which are used to test Bucket Logging APIs
        /// This test passes, when the result is of type SetACLResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketC_SetACLForBucketTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                SetACLResponse response = result as SetACLResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };

            _client.OnS3Response += handler;

            S3AccessControlList list = new S3AccessControlList();
            list.AddGrant(new S3Grantee()
            {
                CanonicalUser = new Amazon.S3.Model.Tuple<string, string> { First = CanonicalUserID, Second = "Me" }
            }, S3Permission.FULL_CONTROL);

            list.AddGrant(new S3Grantee()
            {
                URI = "http://acs.amazonaws.com/groups/s3/LogDelivery"
            }, S3Permission.WRITE);

            list.AddGrant(new S3Grantee()
            {
                URI = "http://acs.amazonaws.com/groups/s3/LogDelivery",
            }, S3Permission.READ_ACP);

            list.Owner = new Owner { DisplayName = "Me", Id = CanonicalUserID };

            SetACLRequest request = new SetACLRequest { BucketName = _bucketNameForBucketAPIs, ACL = list };

            _client.SetACL(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Set ACLForBucket Test. This is to check the result, when an empty ACL is passed to SetACLRequest
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "No ACL or CannedACL specified!")]
        public void BucketC_SetACLForBucketTest_ForException_WithoutACLList()
        {
            SetACLRequest request = new SetACLRequest { BucketName = _bucketNameForBucketAPIs };
            _client.SetACL(request);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Set ACLForBucket Test. This is to check the result, when an empty owner is passed in ACL to SetACLRequest
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "No owner for the ACL specified!")]
        public void BucketC_SetACLForBucketTest_ForException_WithoutACLOwner()
        {
            S3AccessControlList list = new S3AccessControlList();
            list.AddGrant(new S3Grantee()
            {
                CanonicalUser = new Amazon.S3.Model.Tuple<string, string> { First = CanonicalUserID, Second = "Me" }
            }, S3Permission.FULL_CONTROL);

            SetACLRequest request = new SetACLRequest { BucketName = _bucketNameForBucketAPIs, ACL = list };

            _client.SetACL(request);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Enable Bucket logging Test. The test passes, when the result is of type EnableBucketLoggingResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketD_EnableBucketLoggingTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                EnableBucketLoggingResponse response = result as EnableBucketLoggingResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };

            EnableBucketLoggingRequest loggingRequest = new EnableBucketLoggingRequest() { BucketName = _bucketNameForBucketAPIs };
            S3BucketLoggingConfig config = loggingRequest.LoggingConfig;

            config.TargetBucketName = _bucketNameForBucketAPIs;

            _client.OnS3Response += handler;
            _client.EnableBucketLogging(loggingRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Enable Bucket logging Test. This is to check the result, when an empty LoggingConfig is passed to EnableBucketLoggingRequest
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "The LoggingConfig is null!")]
        public void BucketC_EnableBucketLoggingTest_ForException_WithoutLoggingConfig()
        {
            EnableBucketLoggingRequest loggingRequest = new EnableBucketLoggingRequest() { BucketName = _bucketNameForBucketAPIs };

            _client.EnableBucketLogging(loggingRequest);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Get Bucket logging Test. The test passes, when the result is of type GetBucketLoggingResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketE_GetBucketLoggingTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                GetBucketLoggingResponse response = result as GetBucketLoggingResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };

            GetBucketLoggingRequest request = new GetBucketLoggingRequest() { BucketName = _bucketNameForBucketAPIs };

            _client.OnS3Response += handler;
            _client.GetBucketLogging(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Disable Bucket logging Test. The test passes, when the result is of type DisableBucketLoggingResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketF_DisbleBucketLoggingTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                DisableBucketLoggingResponse response = result as DisableBucketLoggingResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };

            DisableBucketLoggingRequest request = new DisableBucketLoggingRequest() { BucketName = _bucketNameForBucketAPIs };

            _client.OnS3Response += handler;
            _client.DisableBucketLogging(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Bucket Location Test. The test passes, when the result is of type GetBucketLocationResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketG_GetBucketLocationTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                GetBucketLocationResponse response = result as GetBucketLocationResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            GetBucketLocationRequest request = new GetBucketLocationRequest() { BucketName = _bucketNameForBucketAPIs };
            _client.GetBucketLocation(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Get Bucket Location Test. This test is to check the result, when the empty bucket name is passed in request 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BucketG_GetBucketLocationTest_ForException_EmptyBucketName()
        {
            string actualValue = string.Empty;
            string emptyBucketName = string.Empty;

            //Create request object.
            GetBucketLocationRequest request = new GetBucketLocationRequest { BucketName = emptyBucketName };
            _client.GetBucketLocation(request);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Put Bucket Policy Test. The test passes, when the result is of type PutBucketPolicyResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketH_PutBucketPolicyTest()
        {
            Func<String> PolicyValue = () => "{\"Version\": \"2008-10-17\",\"Id\": \"aaaa-bbbb-cccc-dddd\",\"Statement\" :" +
                "[{\"Effect\": \"Allow\",\"Sid\": \"1\",\"Principal\": {\"AWS\": \"*\"}," +
                "\"Action\": [\"s3:*\"],\"Resource\": \"arn:aws:s3:::" + _bucketNameForBucketAPIs + "/*\"}]}";

            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                PutBucketPolicyResponse response = result as PutBucketPolicyResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            PutBucketPolicyRequest request = new PutBucketPolicyRequest() { BucketName = _bucketNameForBucketAPIs, Policy = PolicyValue.Invoke() };
            _client.PutBucketPolicy(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Get Bucket Location Test. This test is to check the result, when policy name is left 
        /// blank for PutBucketPolicy request
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BucketH_PutBucketPolicyTest_ForException_EmptyPolicyName()
        {
            string actualValue = string.Empty;
            string emptyPolicy = string.Empty;

            //Create request object.
            PutBucketPolicyRequest request = new PutBucketPolicyRequest()
            {
                BucketName = _bucketNameForBucketAPIs,
                Policy = emptyPolicy
            };
            _client.PutBucketPolicy(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Get Bucket Policy Test. The test passes, when the result is of type GetBucketPolicyResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketI_GetBucketPolicyTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                GetBucketPolicyResponse response = result as GetBucketPolicyResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            GetBucketPolicyRequest request = new GetBucketPolicyRequest() { BucketName = _bucketNameForBucketAPIs };
            _client.GetBucketPolicy(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Get Bucket Policy Test. This test is to check the result, when the empty bucket name is passed in request 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BucketI_GetBucketPolicyTest_ForException_EmptyBucketName()
        {
            string actualValue = string.Empty;
            string emptyBucketName = string.Empty;

            //Create request object.
            GetBucketPolicyRequest request = new GetBucketPolicyRequest { BucketName = emptyBucketName };
            _client.GetBucketPolicy(request);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Delete Bucket Policy Test.  The test passes, when the result is of type DeleteBucketPolicyResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketJ_DeleteBucketPolicyTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                DeleteBucketPolicyResponse response = result as DeleteBucketPolicyResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            DeleteBucketPolicyRequest request = new DeleteBucketPolicyRequest() { BucketName = _bucketNameForBucketAPIs };
            _client.DeleteBucketPolicy(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Delete Bucket Policy Test. This test is to check the result, when the empty bucket name is passed in request 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BucketJ_DeleteBucketPolicyTest_ForException_EmptyBucketName()
        {
            string actualValue = string.Empty;
            string emptyBucketName = string.Empty;

            //Create request object.
            DeleteBucketPolicyRequest request = new DeleteBucketPolicyRequest { BucketName = emptyBucketName };
            _client.DeleteBucketPolicy(request);
            EnqueueTestComplete();
        }

        /// <summary>
        /// Delete Bucket Policy Test. The test passes, when the result is of type DeleteBucketResponse
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void BucketK_DeleteBucketTest()
        {
            bool expectedValue = true;
            bool actualValue = false;
            bool hasCallbackArrived = false;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                DeleteBucketResponse response = result as DeleteBucketResponse;
                if (null != response)
                    actualValue = response.IsRequestSuccessful;
                hasCallbackArrived = true;
            };
            _client.OnS3Response += handler;

            DeleteBucketRequest request = new DeleteBucketRequest() { BucketName = _bucketNameForBucketAPIs };
            _client.DeleteBucket(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue,
                string.Format("Expected Value = {0}, Actual Value = {1}", expectedValue, actualValue)));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Delete Bucket Policy. This test is to check the result, when the empty bucket name is passed in request 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BucketK_DeleteBucketTest_ForException_EmptyBucketName()
        {
            string actualValue = string.Empty;
            string emptyBucketName = string.Empty;

            //Create request object.
            DeleteBucketRequest request = new DeleteBucketRequest { BucketName = emptyBucketName };
            _client.DeleteBucket(request);
            EnqueueTestComplete();
        }

        #endregion

        #region Object Test Cases

        /// <summary>
        /// Tests the Get-Object API. The test passes by checking the value of the object.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_B_GetObject_And_Check_For_Value()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "This body-content is put through Unit-testing.";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                GetObjectResponse response = result as GetObjectResponse;
                if (null != response)
                {
                    using (StreamReader streamRead = new StreamReader(response.ResponseStream))
                    {
                        actualValue = streamRead.ReadToEnd();
                    }
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            GetObjectRequest request = new GetObjectRequest { BucketName = _bucketName, Key = _key };
            _client.GetObject(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object API by passing a non-existing bucket name. The test passes by checking the error-message from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetObject_With_NonExisting_BucketName_And_Check_For_ErrorMessage()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "The specified bucket does not exist";
            string nonExistingBucketName = "xzyuiop";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;

                if (null != exceptionResponse)
                    actualValue = exceptionResponse.Message;
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            ListObjectsRequest request = new ListObjectsRequest { BucketName = nonExistingBucketName };
            _client.ListObjects(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object API by passing a non-existing key name. The test passes by checking the error-message sent by the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetObject_With_NonExisting_KeyName_And_Check_For_ErrorMessage()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "The specified key does not exist.";
            string nonExistingKeyName = "xzyuiop";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;

                if (null != exceptionResponse)
                    actualValue = exceptionResponse.Message;
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            GetObjectRequest request = new GetObjectRequest { BucketName = _bucketName, Key = nonExistingKeyName };
            _client.GetObject(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object API. The test passes with the expected no of objects for the specified Bucket.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_D_ListObjects_And_Check_For_Object_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 1;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;

                ListObjectsResponse response = result as ListObjectsResponse;
                if (null != response)
                {
                    actualValue = response.S3Objects.Count;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            ListObjectsRequest request = new ListObjectsRequest { BucketName = _bucketName };
            _client.ListObjects(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the List-Objects API by passing a non-existing bucket name. 
        /// The test passes with the expected no of objects for the specified Bucket.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_ListObjects_With_NonExisting_BucketName_And_Check_For_ErrorMessage()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "The specified bucket does not exist";
            string nonExistingBucketName = "xzyuiop";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;

                if (null != exceptionResponse)
                    actualValue = exceptionResponse.Message;
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            ListObjectsRequest request = new ListObjectsRequest { BucketName = nonExistingBucketName };
            _client.ListObjects(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the List-Objects API by passing an empty bucket name.
        /// The test passes by expecting an ArgumentNullException to be thrown.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_ListObjects_With_Empty_BucketName_Check_For_ErrorMessage()
        {
            string actualValue = string.Empty;
            string emptyBucketName = string.Empty;

            //Create request object.
            ListObjectsRequest request = new ListObjectsRequest { BucketName = emptyBucketName };
            _client.ListObjects(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object-ACL API for a non-existing key. The test passes by checking the error-code. The expected value is "NoSuchKey".
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetObjectACL_With_NonExisting_Key_And_Check_Error_Message()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "NoSuchKey";
            string nonExistingKey = "poiuytrewdq";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;

                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;
                if (null != exceptionResponse)
                {
                    actualValue = exceptionResponse.ErrorCode;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            GetACLRequest request = new GetACLRequest { BucketName = _bucketName, Key = nonExistingKey };
            _client.GetACL(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Set-Object-ACL API that sets an ACL for an object. 
        /// The test passes by checking the boolean result returned. The expected result is true.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_G_SetObjectACL_And_Check_For_Boolean_Result()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                SetACLResponse response = result as SetACLResponse;

                if (null != response)
                    actualValue = response.IsRequestSuccessful;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            S3AccessControlList list = new S3AccessControlList();
            list.AddGrant(new S3Grantee
            {
                CanonicalUser = new Amazon.S3.Model.Tuple<string, string> { First = CanonicalUserID, Second = "Me" }
            }, S3Permission.FULL_CONTROL);

            list.Owner = new Owner { DisplayName = "Me", Id = CanonicalUserID };
            SetACLRequest request = new SetACLRequest { BucketName = _bucketName, Key = _key, ACL = list };

            _client.SetACL(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Set-Object-ACL API that sets an ACL for an object with an invalid Owner-canonical-userid. 
        /// The test passes by checking the error-code returned by the server. The expected result is "InvalidArgument".
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_H_SetObjectACL_With_Invalid_GranteeCanonicalUserID_And_Check_For_Error_Message()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidArgument";
            string invalidOwnerCanonicalUserID = "09876567890"; //For more info on CanonicalUserIDs refer CanonicalUserID variable.

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;

                if (null != exceptionResponse)
                    actualValue = exceptionResponse.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            S3AccessControlList list = new S3AccessControlList();
            list.AddGrant(new S3Grantee
            {
                CanonicalUser = new Amazon.S3.Model.Tuple<string, string> { First = CanonicalUserID, Second = "Me" }
            }, S3Permission.FULL_CONTROL);

            list.Owner = new Owner { DisplayName = "Me", Id = invalidOwnerCanonicalUserID };
            SetACLRequest request = new SetACLRequest { BucketName = _bucketName, Key = _key, ACL = list };

            _client.SetACL(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }
        /// <summary>
        /// Tests the Set-Object-ACL API that sets an ACL for an object with an invalid grantee-canonical-userid. 
        /// The test passes by checking the error-code returned by the server. The expected result is "InvalidArgument".
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_I_SetObjectACL_With_Invalid_GranteeCanonicalUserID_And_Check_For_Error_Message()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "InvalidArgument";
            string invalidGranteeCanonicalUserID = "09876567890"; //For more info on CanonicalUserIDs refer CanonicalUserID variable.

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;

                if (null != exceptionResponse)
                    actualValue = exceptionResponse.ErrorCode;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            S3AccessControlList list = new S3AccessControlList();
            list.AddGrant(new S3Grantee
            {
                CanonicalUser = new Amazon.S3.Model.Tuple<string, string> { First = invalidGranteeCanonicalUserID, Second = "Me" }
            }, S3Permission.FULL_CONTROL);

            list.Owner = new Owner { DisplayName = "Me", Id = CanonicalUserID };
            SetACLRequest request = new SetACLRequest { BucketName = _bucketName, Key = _key, ACL = list };

            _client.SetACL(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object-ACL API that gets an ACL for an object. 
        /// The test passes by checking the Permission returned. The expected result is S3Permission.FULL_CONTROL.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_J_GetObjectACL_And_Check_For_Permission_Returned()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = S3Permission.FULL_CONTROL.ToString();

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                GetACLResponse response = result as GetACLResponse;

                if (null != response)
                    actualValue = response.AccessControlList.Grants[0].Permission.ToString();
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            GetACLRequest request = new GetACLRequest { BucketName = _bucketName, Key = _key };
            _client.GetACL(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Delete-Object API. The test passes by checking the bool IsRequestSuccessful property value. The expected result is True.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_K_DeleteObject_And_Check_For_Boolean_Value()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;

                DeleteObjectResponse response = result as DeleteObjectResponse;
                if (null != response)
                {
                    actualValue = response.IsRequestSuccessful;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            DeleteObjectRequest request = new DeleteObjectRequest { BucketName = _bucketName, Key = _key };
            _client.DeleteObject(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object-MetaData API. The test passes by checking the Metadata.Count property value. The expected result is 0.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_F_GetObjectMetaData_And_Check_For_Metadata_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = -1;
            int expectedValue = 0;

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;

                GetObjectMetadataResponse response = result as GetObjectMetadataResponse;
                if (null != response)
                {
                    actualValue = response.Metadata.Count;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            GetObjectMetadataRequest request = new GetObjectMetadataRequest { BucketName = _bucketName, Key = _key };
            _client.GetObjectMetadata(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Object API that puts a content. 
        /// The test passes by checking the boolean result returned. The expected result is true.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_A_PutObject_And_Check_For_Boolean_Result()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            string contentBody = "This body-content is put through Unit-testing.";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                PutObjectResponse response = result as PutObjectResponse;

                if (null != response)
                    actualValue = response.IsRequestSuccessful;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            PutObjectRequest request = new PutObjectRequest { BucketName = _bucketName, Key = _key, ContentBody = contentBody };
            _client.PutObject(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Object API with a non-existing bucket name. 
        /// The test passes by checking the error message.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_PutObject_With_NonExisting_BucketName_And_Check_For_Error_Message()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "The specified bucket does not exist";
            string nonExistingBucketName = "xzyiop";
            string contentBody = "This body-content is put through Unit-testing.";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                AmazonS3Exception exceptionResponse = result as AmazonS3Exception;

                if (null != exceptionResponse)
                    actualValue = exceptionResponse.Message;

                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            PutObjectRequest request = new PutObjectRequest { BucketName = nonExistingBucketName, Key = _key, ContentBody = contentBody };
            _client.PutObject(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Object API with a empty of null key name. 
        /// The test passes by expecting an ArgumentException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_PutObject_With_Empty_KeyName_And_ExpectException()
        {
            string emptyKeyName = string.Empty;
            string contentBody = "This body-content is put through Unit-testing.";

            //Create request object.
            PutObjectRequest request = new PutObjectRequest { BucketName = _bucketName, Key = emptyKeyName, ContentBody = contentBody };
            _client.PutObject(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Object API that put a specified file from the PC. 
        /// The test passes by checking the boolean result returned. The expected result is true.
        /// NOTE: This test is ignored, because Silverlight for Windows Phone 7 does not support reading files from the system.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [Ignore]
        public void Test_PutObjectAsFile_And_Check_For_Boolean_Result()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            string key = "key_z";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                PutObjectResponse response = result as PutObjectResponse;

                if (null != response)
                {
                    string responseBody = string.Empty;
                    actualValue = response.IsRequestSuccessful;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            //Create request object.
            PutObjectRequest request = new PutObjectRequest { BucketName = _bucketName, Key = key, FilePath = @"C:\UnitTestObject.txt" };
            _client.PutObject(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object API that is not provided with a valid Bucket name.
        /// The test passes by expecting an ArgumentNullException to be thrown.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_GetObject_With_No_Bucket_Name_And_Expect_Exception()
        {
            string bucketName = string.Empty;
            string key = "key_z";

            //Create request object.
            GetObjectRequest request = new GetObjectRequest { BucketName = bucketName, Key = key };
            _client.GetObject(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Object API that is not provided with a valid Key name.
        /// The test passes by expecting an ArgumentNullException to be thrown.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_GetObject_With_No_Key_Name_And_Expect_Exception()
        {
            string bucketName = "New_Bucket_3";
            string key = string.Empty;

            //Create request object.
            GetObjectRequest request = new GetObjectRequest { BucketName = bucketName, Key = key, };
            _client.GetObject(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Copy-Object API that copies an existing object to a new object. 
        /// The test passes by checking the boolean result returned. The expected result is true.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_E_CopyObject_And_Check_For_Boolean_Result()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            string keyDestination = "key_UnitTesting_destination_1";

            S3ResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                IS3Response result = args.Response;
                //Unhook from event.
                _client.OnS3Response -= handler;
                CopyObjectResponse response = result as CopyObjectResponse;

                if (null != response)
                {
                    string responseBody = string.Empty;
                    actualValue = response.IsRequestSuccessful;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnS3Response += handler;

            _client.CopyObject(new CopyObjectRequest
            {
                SourceBucket = _bucketName,
                DestinationBucket = _bucketNameDestination,
                SourceKey = _key,
                DestinationKey = keyDestination
            });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Copy-Object API that is not provided with a valid Source-Bucket-name.
        /// The test passes by expecting an ArgumentNullException to be thrown.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CopyObject_With_No_SourceBucket_Name_And_Expect_Exception()
        {
            string bucketName = string.Empty;
            string bucketNameDestination = "New_Bucket_3_Destination";
            string key = "key_z";
            string keyDestination = "key_destination_1";

            //Create request object.
            CopyObjectRequest request = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                DestinationBucket = bucketNameDestination,
                SourceKey = key,
                DestinationKey = keyDestination
            };
            _client.CopyObject(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Copy-Object API that is not provided with a valid Source-Key-name.
        /// The test passes by expecting an ArgumentNullException to be thrown.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CopyObject_With_No_SourceKey_Name_And_Expect_Exception()
        {
            string bucketName = "New_Bucket_3";
            string bucketNameDestination = "New_Bucket_3_Destination";
            string key = string.Empty;
            string keyDestination = "key_destination_1";

            //Create request object.
            CopyObjectRequest request = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                DestinationBucket = bucketNameDestination,
                SourceKey = key,
                DestinationKey = keyDestination
            };
            _client.CopyObject(request);

            EnqueueTestComplete();
        }

        /// <summary>
        /// Tets the Copy-Object API that is not provided with a valid Destination-Bucket-name.
        /// The test passes by expecting an ArgumentNullException to be thrown.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CopyObject_With_No_DestinationBucket_Name_And_Expect_Exception()
        {
            string bucketName = "New_Bucket_3";
            string bucketNameDestination = string.Empty;
            string key = "Key_z";
            string keyDestination = "key_destination_1";

            //Create request object.
            CopyObjectRequest request = new CopyObjectRequest
            {
                SourceBucket = bucketName,
                DestinationBucket = bucketNameDestination,
                SourceKey = key,
                DestinationKey = keyDestination
            };
            _client.CopyObject(request);

            EnqueueTestComplete();
        }

        #endregion
    }
}
