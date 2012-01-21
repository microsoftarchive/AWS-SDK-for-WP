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

using System.Collections.Generic;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest_SimpleDB
{
    /// <summary>
    /// Represents a test class to test AmazonSimpleDBClient.
    /// </summary>
    [TestClass]
    public class AmazonSimpleDBClient_Tests : SilverlightTest
    {
        #region Private Variables

        private AmazonSimpleDB _client;
        private const string Access_Key_ID = "Insert your Amazon Web Services Access Key ID here";
        private const string Secret_Access_Key = "Insert your Amazon Web Services Secret Access Key here";

        private string _domainName_UnitTesting = "Domain_UnitTesting";
        private string _itemName_UnitTesting = "Item_UnitTesting";

        #endregion

        #region Startup Functions

        /// <summary>
        /// Clean up code before the test exits for the class.
        /// </summary>
        [ClassCleanup]
        [Asynchronous]
        public void TearDown()
        {
            //A post-requisite for testing SimpleDB objects. Ensure that all the Domains and Item created are deleted before we exit the test-class.
            bool hasCallbackArrived = false;
            //1. Remove the attribute (that was set during one of the test cases) as well.
            hasCallbackArrived = false;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler2 = null;
            handler2 = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler2;
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler2;

            DeleteAttributesRequest deleteRequest = new DeleteAttributesRequest()
            {
                DomainName = _domainName_UnitTesting,
                ItemName = _itemName_UnitTesting
            };

            deleteRequest.Attribute.Add(
                new Amazon.SimpleDB.Model.Attribute
                    {
                        Name = "Name",
                        Value = "Name_1"
                    }
            );
            deleteRequest.Attribute.Add(
                new Amazon.SimpleDB.Model.Attribute
                {
                    Name = "Color",
                    Value = "blue"
                }
            );
            deleteRequest.Attribute.Add(
                new Amazon.SimpleDB.Model.Attribute
                {
                    Name = "Size",
                    Value = "Small"
                }
            );
            deleteRequest.Attribute.Add(
                new Amazon.SimpleDB.Model.Attribute
                {
                    Name = "Size",
                    Value = "Medium"
                }
            );

            _client.DeleteAttributes(deleteRequest);

            //Wait till the request is actually carried out.
            EnqueueConditional(() => hasCallbackArrived);

            //2.. Delete the normal domain.
            hasCallbackArrived = false;
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;

            _client.BeginDeleteDomain(new Amazon.SimpleDB.Model.DeleteDomainRequest { DomainName = _domainName_UnitTesting });

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
            _client = AWSClientFactory.CreateAmazonSimpleDBClient(Access_Key_ID, Secret_Access_Key);

            //A pre-requisite for testing SimpleDB Objects. Ensure that we create one Domain to test the Objects. 
            //Create the Domain.
            bool hasCallbackArrived = false;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;

            _client.BeginCreateDomain(new Amazon.SimpleDB.Model.CreateDomainRequest { DomainName = _domainName_UnitTesting });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueTestComplete();
        }

        #endregion

        #region Domain Test Cases

        /// <summary>
        /// Test for Create Domain API.
        /// The test is successful, when expected result is of type CreateDomainResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestA_CreateDomain()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                CreateDomainResponse response = args.Response as CreateDomainResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;

            _client.BeginCreateDomain(new Amazon.SimpleDB.Model.CreateDomainRequest { DomainName = "UnitTestDomain" });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Create Domain API. 
        /// This is a negative test, and results in an expection, when empty domain name is provided.
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestA_CreateDomain_ForException_EmptyDomainName()
        {
            string actualValue = "Value () for parameter DomainName is invalid.";
            string emptyDomainName = string.Empty;
            bool hasCallbackArrived = false;
            string expectedValue = string.Empty;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;

            responseHandler = delegate(object sender, ResponseEventArgs args)
            {
                _client.OnSimpleDBResponse -= responseHandler;
                AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;
                if (null != exception)
                {
                    expectedValue = exception.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += responseHandler;

            _client.BeginCreateDomain(new Amazon.SimpleDB.Model.CreateDomainRequest { DomainName = emptyDomainName });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for List Domain API.
        /// The test is successful, when expected result as of type ListDomainsResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestB_ListDomain()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                ListDomainsResponse response = args.Response as ListDomainsResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;

            _client.BeginListDomains(new Amazon.SimpleDB.Model.ListDomainsRequest());

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for List Domain API. 
        /// This is a negative test, it test for the exception when MaxNumberofDomains of 
        /// ListDomainRequest is less than 1
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestB_ListDomain_ForException_MaxNumberOfDomain()
        {
            string actualValue = "Value (0) for parameter MaxNumberOfDomains is invalid. MaxNumberOfDomains must be between 1 and 100";
            bool hasCallbackArrived = false;
            string expectedValue = string.Empty;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;

            responseHandler = delegate(object sender, ResponseEventArgs args)
            {
                _client.OnSimpleDBResponse -= responseHandler;
                AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;
                if (null != exception)
                {
                    expectedValue = exception.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += responseHandler;

            _client.BeginListDomains(new Amazon.SimpleDB.Model.ListDomainsRequest() { MaxNumberOfDomains = 0 });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for List Domain API.
        /// This is the test for empty NextToken property of ListDomainRequest
        /// The test is successful, when expected result is of type ListDomainsResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestB_ListDomain_WithEmptyNextToken()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;

            responseHandler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= responseHandler;
                ListDomainsResponse response = args.Response as ListDomainsResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += responseHandler;

            _client.BeginListDomains(new ListDomainsRequest() { NextToken = string.Empty });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Domain Metadata API.
        /// The test is successful when expected result is of type DomainMetadataResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestC_DomainMetadata()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                DomainMetadataResponse response = args.Response as DomainMetadataResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;

            _client.BeginDomainMetadata(new Amazon.SimpleDB.Model.DomainMetadataRequest() { DomainName = "UnitTestDomain" });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Domain Metadata API. 
        /// This is a negative test, and results in an expection, when empty domain name is provided.
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestC_DomainMetadata_ForException_EmptyDomainName()
        {
            string actualValue = "Value () for parameter DomainName is invalid.";
            string emptyDomainName = string.Empty;
            bool hasCallbackArrived = false;
            string expectedValue = string.Empty;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;

            responseHandler = delegate(object sender, ResponseEventArgs args)
            {
                _client.OnSimpleDBResponse -= responseHandler;
                AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;
                if (null != exception)
                {
                    expectedValue = exception.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += responseHandler;

            _client.BeginDomainMetadata(new Amazon.SimpleDB.Model.DomainMetadataRequest { DomainName = emptyDomainName });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Delete Domain API.
        /// The test is successful, when expected result is of type DeleteDomainResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestD_DeleteDomain()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                DeleteDomainResponse response = args.Response as DeleteDomainResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;

            _client.BeginDeleteDomain(new Amazon.SimpleDB.Model.DeleteDomainRequest() { DomainName = "UnitTestDomain" });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Delete Domain API 
        /// This is a negative test, and results in an expection, when empty domain name is provided.
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void DomainTestD_DeleteDomain_ForException_EmptyDomainName()
        {
            string actualValue = "Value () for parameter DomainName is invalid.";
            string emptyDomainName = string.Empty;
            bool hasCallbackArrived = false;
            string expectedValue = string.Empty;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;

            responseHandler = delegate(object sender, ResponseEventArgs args)
            {
                _client.OnSimpleDBResponse -= responseHandler;
                AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;
                if (null != exception)
                {
                    expectedValue = exception.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += responseHandler;

            _client.BeginDeleteDomain(new Amazon.SimpleDB.Model.DeleteDomainRequest { DomainName = emptyDomainName });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        #endregion

        #region Attribute Test Cases

        /// <summary>
        /// Test for Batch Put Attribute API.
        /// The test is successful, when expected result is of type BatchPutAttributesResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void AttributeTestA_BatchPutAttribute()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                BatchPutAttributesResponse response = args.Response as BatchPutAttributesResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;
            BatchPutAttributesRequest request = new BatchPutAttributesRequest() { DomainName = _domainName_UnitTesting };

            //List<ReplaceableAttribute> itemA = new List<ReplaceableAttribute>();
            ReplaceableItem itemA = new ReplaceableItem { ItemName = "ItemA" };
            itemA.Attribute.Add(new ReplaceableAttribute().WithName("Category").WithValue("Company"));
            itemA.Attribute.Add(new ReplaceableAttribute().WithName("Subcategory").WithValue("Private Limited"));
            itemA.Attribute.Add(new ReplaceableAttribute().WithName("Name").WithValue("Neudesic Technologies"));

            //List<ReplaceableAttribute> itemB = new List<ReplaceableAttribute>();
            ReplaceableItem itemB = new ReplaceableItem { ItemName = "ItemB" };
            itemB.Attribute.Add(new ReplaceableAttribute().WithName("Sector").WithValue("IT"));
            itemB.Attribute.Add(new ReplaceableAttribute().WithName("Location").WithValue("Hydrabad"));
            itemB.Attribute.Add(new ReplaceableAttribute().WithName("Location").WithValue("Bangalore"));
            itemB.Attribute.Add(new ReplaceableAttribute().WithName("Size").WithValue("Large"));

            List<ReplaceableItem> replacableItem = request.Items;

            replacableItem.Add(itemA);
            replacableItem.Add(itemB);
            _client.BatchPutAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Batch Put Attribute API. 
        /// This is a negative test, and results in an expection, when item name is not provided.
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void AttributeTestA_BatchPutAttribute_ForException_WithoutItem()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "No items specified";
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                AmazonSimpleDBException response = args.Response as AmazonSimpleDBException;
                if (null != response)
                {
                    actualValue = response.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;
            BatchPutAttributesRequest request = new BatchPutAttributesRequest() { DomainName = _domainName_UnitTesting };
            _client.BatchPutAttributes(request);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Delete Attribute API.
        /// The test is successful, when expected result is of type DeleteAttributesResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void AttributeTestB_DeleteAttribute()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                DeleteAttributesResponse response = args.Response as DeleteAttributesResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;
            DeleteAttributesRequest deleteRequest = new DeleteAttributesRequest()
            {
                DomainName = _domainName_UnitTesting,
                ItemName = "ItemB"
            };
            List<Amazon.SimpleDB.Model.Attribute> deleteItem = deleteRequest.Attribute;

            deleteItem.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Location").WithValue("Hydrabad"));

            _client.DeleteAttributes(deleteRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Delete Attribute API. 
        /// This is a negative test, and results in an expection, when item name is not provided.
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void AttributeTestB_DeleteAttribute_ForException_WithoutItem()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "The request must contain the parameter ItemName";
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                AmazonSimpleDBException response = args.Response as AmazonSimpleDBException;
                if (null != response)
                {
                    actualValue = response.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;
            DeleteAttributesRequest deleteRequest = new DeleteAttributesRequest()
            {
                DomainName = _domainName_UnitTesting
            };
            _client.DeleteAttributes(deleteRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Delete Attribute API.
        /// This test is done, for checking the result, when wrong item name is provided for Delete Attribute request
        /// The test is successful, when expected result is of type DeleteAttributesResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void AttributeTestB_DeleteAttribute_WithWrongItemName()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                DeleteAttributesResponse response = args.Response as DeleteAttributesResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;
            DeleteAttributesRequest deleteRequest = new DeleteAttributesRequest()
            {
                DomainName = _domainName_UnitTesting,
                ItemName = "WrongName"
            };
            _client.DeleteAttributes(deleteRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Batch Delete Attribute API.
        /// The test is successful, when expected result as of type BatchDeleteAttributesResponse.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void ZAttributeTestC_BatchDeleteAttribute()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                BatchDeleteAttributesResponse response = args.Response as BatchDeleteAttributesResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            BatchDeleteAttributesRequest deleteRequest = new BatchDeleteAttributesRequest() { DomainName = _domainName_UnitTesting };
            List<DeleteableItem> deleteItem = deleteRequest.Item;

            #region Commented
            //Commented because there was change in the property defination during resolving FxCop warning

            //List<Amazon.SimpleDB.Model.Attribute> itemA = new List<Amazon.SimpleDB.Model.Attribute>();
            //itemA.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Subcategory").WithValue("Private Limited"));

            //List<Amazon.SimpleDB.Model.Attribute> itemB = new List<Amazon.SimpleDB.Model.Attribute>();
            //itemB.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Size").WithValue("Large"));

            //deleteItem.Add(new DeleteableItem() { Attribute = itemA, ItemName = "ItemA" });
            //deleteItem.Add(new DeleteableItem() { Attribute = itemB, ItemName = "ItemB" });
            #endregion

            DeleteableItem item1 = new DeleteableItem { ItemName = "ItemA" };
            item1.Attribute.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Subcategory").WithValue("Private Limited"));
            DeleteableItem item2 = new DeleteableItem { ItemName = "ItemB" };
            item2.Attribute.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Size").WithValue("Large"));
            deleteItem.Add(item1);
            deleteItem.Add(item2);

            _client.BatchDeleteAttributes(deleteRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Test for Batch Delete Attribute API. 
        /// This is a negative test, and results in an expection, when item name is not provided.
        /// The test is successful, when expected result is of type AmazonSimpleDBException.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void ZAttributeTestC_BatchDeleteAttribute_ForException_WithoutItems()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "No items specified";
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                AmazonSimpleDBException response = args.Response as AmazonSimpleDBException;
                if (null != response)
                {
                    actualValue = response.Message.Trim();
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            BatchDeleteAttributesRequest deleteRequest = new BatchDeleteAttributesRequest() { DomainName = _domainName_UnitTesting };
            _client.BatchDeleteAttributes(deleteRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Attribute API. The test passes by checking the non-null response from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_A_PutAttribute_And_Check_For_NonNull_Response()
        {
            bool hasCallbackArrived = false;
            bool actualValue = false;
            bool expectedValue = true;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                PutAttributesResponse response = args.Response as PutAttributesResponse;
                if (null != response)
                {
                    actualValue = true;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            //Create request object.
            PutAttributesRequest putAttributesRequest = new PutAttributesRequest { DomainName = _domainName_UnitTesting, ItemName = _itemName_UnitTesting };
            List<ReplaceableAttribute> attributes = putAttributesRequest.Attribute;
            attributes.Add(new ReplaceableAttribute().WithName("Name").WithValue("Name_1"));
            attributes.Add(new ReplaceableAttribute().WithName("Color").WithValue("Blue"));
            attributes.Add(new ReplaceableAttribute().WithName("Size").WithValue("Small"));
            attributes.Add(new ReplaceableAttribute().WithName("Size").WithValue("Medium"));

            _client.PutAttributes(putAttributesRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Attribute API for a non-existing domain-name. The test passes by expecting an exception from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_B_PutAttribute_With_NonExisting_DomainName_And_Expect_Exception()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "MissingParameter";
            string domainName = "poiu";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                AmazonSimpleDBException response = args.Response as AmazonSimpleDBException;
                if (null != response)
                {
                    actualValue = response.ErrorCode;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            //Create request object.
            PutAttributesRequest putAttributesRequest = new PutAttributesRequest { DomainName = domainName, ItemName = _itemName_UnitTesting };
            List<ReplaceableAttribute> attributes = putAttributesRequest.Attribute;

            _client.PutAttributes(putAttributesRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Put-Attribute API for a non-existing item-name. The test passes by expecting an exception from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_C_PutAttribute_With_NonExisting_ItemName_And_Expect_Exception()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "MissingParameter";
            string itemName = "poiu";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                AmazonSimpleDBException response = args.Response as AmazonSimpleDBException;
                if (null != response)
                {
                    actualValue = response.ErrorCode;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            //Create request object.
            PutAttributesRequest putAttributesRequest = new PutAttributesRequest { DomainName = _domainName_UnitTesting, ItemName = itemName };
            List<ReplaceableAttribute> attributes = putAttributesRequest.Attribute;

            _client.PutAttributes(putAttributesRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Attribute API for a valid attribute. The test passes by expecting a valid attribute from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetAttribute_And_Check_For_Valid_Attribute()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "Name_1";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                GetAttributesResponse response = args.Response as GetAttributesResponse;
                if (null != response)
                {
                    GetAttributesResult attributeResult = response.GetAttributesResult;
                    if (null != attributeResult)
                    {
                        if (attributeResult.Attribute.Count > 0)
                            actualValue = attributeResult.Attribute[0].Value;
                    }
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            //Create request object.
            //Commented because of AttributeName property changed while resolving FxCop warnings.
            //List<string> attributes = new List<string> { "Name" };
            GetAttributesRequest getAttributesRequest = new GetAttributesRequest
            {
                DomainName = _domainName_UnitTesting,
                ItemName = _itemName_UnitTesting
            };
            getAttributesRequest.AttributeName.Add("Name");

            _client.GetAttributes(getAttributesRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Attribute API for the count of attributes. The test passes by checking the no of valid attributes from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetAttributes_And_Check_For_Attribute_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 4;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                GetAttributesResponse response = args.Response as GetAttributesResponse;
                if (null != response)
                {
                    GetAttributesResult attributeResult = response.GetAttributesResult;
                    if (null != attributeResult)
                        actualValue = attributeResult.Attribute.Count;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            //Create request object.
            //List<string> attributes = new List<string>();
            GetAttributesRequest getAttributesRequest = new GetAttributesRequest
            {
                DomainName = _domainName_UnitTesting,
                ItemName = _itemName_UnitTesting
            };

            _client.GetAttributes(getAttributesRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Get-Attribute API with a non-existing attribute. The test passes by epecting no attribute from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_GetAttributes_With_NonExisting_Attribute_And_Expect_Zero_Result()
        {
            bool hasCallbackArrived = false;
            int actualValue = -1;
            int expectedValue = 0;
            string nonExistingAttribute = "poiu";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                GetAttributesResponse response = args.Response as GetAttributesResponse;
                if (null != response)
                {
                    GetAttributesResult attributeResult = response.GetAttributesResult;
                    if (null != attributeResult)
                        actualValue = attributeResult.Attribute.Count;
                }
                hasCallbackArrived = true;
            };

            //Hook to event
            _client.OnSimpleDBResponse += handler;

            //Create request object.
            //Commented because of AttributeName property change during resolving FxCop warining.
            //List<string> attributes = new List<string>{ nonExistingAttribute };
            GetAttributesRequest getAttributesRequest = new GetAttributesRequest
            {
                DomainName = _domainName_UnitTesting,
                ItemName = _itemName_UnitTesting,
            };
            getAttributesRequest.AttributeName.Add(nonExistingAttribute);

            _client.GetAttributes(getAttributesRequest);

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        #endregion Attribute Test Cases

        #region Select Test Cases

        /// <summary>
        /// Tests the Select API for a domain. The test passes by expecting a matching result count from the server. The expected value is 3.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 3;
            string query = "SELECT * FROM " + _domainName_UnitTesting;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                        actualValue = selectResult.Item.Count;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API for a domain with one where clause. The test passes by expecting a matching result count from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_Simple_Where_Clause_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 1;
            string query = "SELECT * FROM " + _domainName_UnitTesting + " WHERE Name = 'Name_1'";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                        actualValue = selectResult.Item.Count;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API for a domain with where clause having multiple attributes. The test passes by expecting a matching result count from the server.
        /// The usage of And is tested.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_Where_Clause_And_Multiple_Attributes_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 1;
            string query = "SELECT * FROM " + _domainName_UnitTesting + " WHERE Name = 'Name_1' AND Color = 'Blue'";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                        actualValue = selectResult.Item.Count;
                }
                hasCallbackArrived = true;
            };
            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API for a non-existing domain. The test passes by expecting an error from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_NonExisting_Domain_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            string actualValue = string.Empty;
            string expectedValue = "NoSuchDomain";
            string nonExistingDomain = "Domain_poiu";
            string query = "SELECT * FROM " + nonExistingDomain;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;

                if (null != exception)
                    actualValue = exception.ErrorCode;

                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(string.Compare(expectedValue, actualValue) == 0));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API for a non-existing attribute. The test passes by expecting no results from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_NonExisting_Attribute_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = -1;
            int expectedValue = 0;
            string nonExistingAttribute = "Attribute_poiu";
            string query = "SELECT * FROM " + _domainName_UnitTesting + " WHERE " + nonExistingAttribute + " = 'some_value'";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                        actualValue = selectResult.Item.Count;
                }

                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API with an ItemName specified. The test passes by expecting the result count from the server. The expected value is 3.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_ItemName_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 3;
            string query = "SELECT " + _itemName_UnitTesting + " FROM " + _domainName_UnitTesting;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                        actualValue = selectResult.Item.Count;
                }

                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API with COUNT token specified. The test passes by expecting the result count from the server. The expected value is 3.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_Count_Token_And_Check_For_Result_Count()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 3;
            string query = "SELECT COUNT(*) FROM " + _domainName_UnitTesting;

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {
                        Item item = selectResult.Item[0];
                        if (null != item)
                        {
                            Attribute attribute = item.Attribute[0];
                            if (null != attribute)
                                int.TryParse(attribute.Value, out actualValue);
                        }
                    }
                }

                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }

        /// <summary>
        /// Tests the Select API with multiple items in query specified. The test passes by matching the result count of attributes from the server.
        /// </summary>
        [Asynchronous]
        [TestMethod]
        public void Test_SelectQuery_With_Multiple_Attributes_And_Check_For_valid_Response()
        {
            bool hasCallbackArrived = false;
            int actualValue = 0;
            int expectedValue = 7;
            string query = "SELECT * FROM " + _domainName_UnitTesting + " WHERE ItemName() in ('" + _itemName_UnitTesting + "', 'ItemA')";

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                _client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {
                        foreach (var i in selectResult.Item)
                            actualValue += i.Attribute.Count;
                    }
                }

                hasCallbackArrived = true;
            };

            _client.OnSimpleDBResponse += handler;
            _client.Select(new SelectRequest { SelectExpression = query, ConsistentRead = true });

            EnqueueConditional(() => hasCallbackArrived);
            EnqueueCallback(() => Assert.IsTrue(expectedValue == actualValue));
            EnqueueTestComplete();
        }
        #endregion Select Test Cases
    }
}
