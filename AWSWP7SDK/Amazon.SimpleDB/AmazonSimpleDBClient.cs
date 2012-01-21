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
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Amazon.Runtime.Internal.Util;
using Amazon.SimpleDB.Model;
using Amazon.SimpleDB.Util;
using Amazon.Util;
using Attribute = Amazon.SimpleDB.Model.Attribute;
using Amazon.Runtime;

namespace Amazon.SimpleDB
{
    public delegate void SimpleDBResponseEventHandler<TSender, T>(object sender, T result);

    public class AmazonSimpleDBClient : AmazonSimpleDB
    {
        public event SimpleDBResponseEventHandler<object, ResponseEventArgs> OnSimpleDBResponse;
        static Logger LOGGER = new Logger(typeof(AmazonSimpleDBClient));
        private AmazonSimpleDBConfig config;

        private string awsAccessKeyId;
        private bool disposed;
        private string clearAwsSecretAccessKey;

        #region Dispose Pattern Implementation

        /// <summary>
        /// Implements the Dispose pattern for the AmazonSimpleDBClient
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
        ~AmazonSimpleDBClient()
        {
            this.Dispose(false);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a client for the Amazon SimpleDB Service with the credentials defined in the App.config.
        /// Example App.config with credentials set. 
        /// <?xml version="1.0" encoding="utf-8" ?>
        /// <configuration>
        ///     <appSettings>
        ///         <add key="AWSAccessKey" value="********************"/>
        ///         <add key="AWSSecretKey" value="****************************************"/>
        ///     </appSettings>
        /// </configuration>
        /// </summary>
        public AmazonSimpleDBClient()
            : this(new EnvironmentAWSCredentials(), new AmazonSimpleDBConfig())
        {
        }

        /// <summary>
        /// Constructs AmazonSimpleDBClient with AWSCredentials
        /// </summary>
        /// <param name="credentials">Credentials</param>
        public AmazonSimpleDBClient(AWSCredentials credentials)
            : this(credentials, new AmazonSimpleDBConfig())
        {
        }

        /// <summary>
        /// Constructs AmazonSimpleDBClient with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        public AmazonSimpleDBClient(string awsAccessKeyId, string awsSecretAccessKey)
            : this(awsAccessKeyId, awsSecretAccessKey, new AmazonSimpleDBConfig())
        {
        }

        /// <summary>
        /// Constructs AmazonSimpleDBClient with AWS Access Key ID, AWS Secret Key and an
        /// AmazonSimpleDB Configuration object. If the config object's
        /// UseSecureStringForAwsSecretKey is false, the AWS Secret Key
        /// is stored as a clear-text string. Please use this option only
        /// if the application environment doesn't allow the use of SecureStrings.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        /// <param name="config">The AmazonSimpleDB Configuration Object</param>
        public AmazonSimpleDBClient(string awsAccessKeyId, string awsSecretAccessKey, AmazonSimpleDBConfig config)
        {
            if (!String.IsNullOrEmpty(awsSecretAccessKey))
            {
                clearAwsSecretAccessKey = awsSecretAccessKey;
            }
            this.awsAccessKeyId = awsAccessKeyId;
            this.config = config;
        }

        private AmazonSimpleDBClient(AWSCredentials credentials, AmazonSimpleDBConfig config)
        {
            this.config = config;
            this.awsAccessKeyId = credentials.GetCredentials().AccessKey;
            this.clearAwsSecretAccessKey = credentials.GetCredentials().SecretKey;
        }

        #endregion

        #region CreateDomain Operation

        /// <summary>
        /// Initiates the asynchronous execution of the CreateDomain operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.CreateDomain"/>
        /// </summary>
        /// <param name="request">The CreateDomainRequest that defines the parameters of
        /// the operation.</param>        
        public void BeginCreateDomain(CreateDomainRequest request)
        {
            IDictionary<string, string> parameters = ConvertCreateDomain(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<CreateDomainResponse>(result);
        }

        /// <summary>
        /// Create Domain. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.CreateDomain"/>
        /// </summary>
        /// <param name="request">The CreateDomainRequest that defines the parameters of
        /// the operation.</param>     
        public void CreateDomain(CreateDomainRequest request)
        {
            BeginCreateDomain(request);
        }

        /**
         * Convert CreateDomainRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertCreateDomain(CreateDomainRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "CreateDomain";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }

            return parameters;
        }

        #endregion

        #region ListDomains Operation

        /// <summary>
        /// Initiates the asynchronous execution of the ListDomains operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.ListDomains"/>
        /// </summary>
        /// <param name="request">The ListDomainsRequest that defines the parameters of
        /// the operation.</param>       
        public void BeginListDomains(ListDomainsRequest request)
        {
            IDictionary<string, string> parameters = ConvertListDomains(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<ListDomainsResponse>(result);
        }

        /// <summary>
        /// List Domains. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.ListDomains"/>
        /// </summary>
        /// <param name="request">The ListDomainsRequest that defines the parameters of
        /// the operation.</param>       
        public void ListDomains(ListDomainsRequest request)
        {
            BeginListDomains(request);
        }

        /**
         * Convert ListDomainsRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertListDomains(ListDomainsRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "ListDomains";
            if (request.IsSetMaxNumberOfDomains())
            {
                parameters["MaxNumberOfDomains"] = request.MaxNumberOfDomains.ToString(CultureInfo.InvariantCulture);
            }
            if (request.IsSetNextToken())
            {
                parameters["NextToken"] = request.NextToken;
            }

            return parameters;
        }

        #endregion

        #region DomainMetadata Operation

        /// <summary>
        /// Initiates the asynchronous execution of the DomainMetadata operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DomainMetadata"/>
        /// </summary>
        /// <param name="request">The DomainMetadataRequest that defines the parameters of
        /// the operation.</param>
        public void BeginDomainMetadata(DomainMetadataRequest request)
        {
            IDictionary<string, string> parameters = ConvertDomainMetadata(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<DomainMetadataResponse>(result);
        }

        /// <summary>
        /// Domain Metadata. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DomainMetadata"/>
        /// </summary>
        /// <param name="request">The DomainMetadataRequest that defines the parameters of
        /// the operation.</param>
        public void DomainMetadata(DomainMetadataRequest request)
        {
            BeginDomainMetadata(request);
        }

        /**
         * Convert DomainMetadataRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertDomainMetadata(DomainMetadataRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "DomainMetadata";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }

            return parameters;
        }

        #endregion

        #region DeleteDomain Operation

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteDomain operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteDomain"/>
        /// </summary>
        /// <param name="request">The DeleteDomainRequest that defines the parameters of
        /// the operation.</param>
        public void BeginDeleteDomain(DeleteDomainRequest request)
        {
            IDictionary<string, string> parameters = ConvertDeleteDomain(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<DeleteDomainResponse>(result);
        }

        /// <summary>
        /// Delete Domain. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteDomain"/>
        /// </summary>
        /// <param name="request">The DeleteDomainRequest that defines the parameters of
        /// the operation.</param>
        public void DeleteDomain(DeleteDomainRequest request)
        {
            BeginDeleteDomain(request);
        }

        /**
         * Convert DeleteDomainRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertDeleteDomain(DeleteDomainRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "DeleteDomain";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }

            return parameters;
        }

        #endregion

        #region PutAttributes Operation

        /// <summary>
        /// Put Attribute.
        /// </summary>
        /// <param name="request">Put Attribute request</param>
        /// <remarks>
        /// The PutAttributes operation creates or replaces attributes within an item. You specify new attributes
        /// using a combination of the Attribute.X.Name and Attribute.X.Value parameters. You specify
        /// the first attribute by the parameters Attribute.0.Name and Attribute.0.Value, the second
        /// attribute by the parameters Attribute.1.Name and Attribute.1.Value, and so on.
        /// Attribute are uniquely identified within an item by their name/value combination. For example, a single
        /// item can have the attributes { "first_name", "first_value" } and { "first_name",
        /// second_value" }. However, it cannot have two attribute instances where both the Attribute.X.Name and
        /// Attribute.X.Value are the same.
        /// Optionally, the requestor can supply the Replace parameter for each individual value. Setting this value
        /// to true will cause the new attribute value to replace the existing attribute value(s). For example, if an
        /// item has the attributes { 'a', '1' }, { 'b', '2'} and { 'b', '3' } and the requestor does a
        /// PutAttributes of { 'b', '4' } with the Replace parameter set to true, the final attributes of the
        /// item will be { 'a', '1' } and { 'b', '4' }, replacing the previous values of the 'b' attribute
        /// with the new value.
        /// </remarks>
        public void PutAttributes(PutAttributesRequest request)
        {
            BeginPutAttributes(request);
        }

        /// <summary>
        /// Initiates the asynchronous execution of the PutAttributes operation.
        /// </summary>
        /// <param name="request">Put Attribute  request</param>
        /// <remarks>
        /// The PutAttributes operation creates or replaces attributes within an item. You specify new attributes
        /// using a combination of the Attribute.X.Name and Attribute.X.Value parameters. You specify
        /// the first attribute by the parameters Attribute.0.Name and Attribute.0.Value, the second
        /// attribute by the parameters Attribute.1.Name and Attribute.1.Value, and so on.
        /// Attribute are uniquely identified within an item by their name/value combination. For example, a single
        /// item can have the attributes { "first_name", "first_value" } and { "first_name",
        /// second_value" }. However, it cannot have two attribute instances where both the Attribute.X.Name and
        /// Attribute.X.Value are the same.
        /// Optionally, the requestor can supply the Replace parameter for each individual value. Setting this value
        /// to true will cause the new attribute value to replace the existing attribute value(s). For example, if an
        /// item has the attributes { 'a', '1' }, { 'b', '2'} and { 'b', '3' } and the requestor does a
        /// PutAttributes of { 'b', '4' } with the Replace parameter set to true, the final attributes of the
        /// item will be { 'a', '1' } and { 'b', '4' }, replacing the previous values of the 'b' attribute
        /// with the new value.
        /// </remarks>
        public void BeginPutAttributes(PutAttributesRequest request)
        {
            IDictionary<string, string> parameters = ConvertPutAttributes(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<PutAttributesResponse>(result);
        }

        /**
         * Convert PutAttributesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertPutAttributes(PutAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "PutAttributes";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }
            if (request.IsSetItemName())
            {
                parameters["ItemName"] = request.ItemName;
            }
            List<ReplaceableAttribute> putAttributesRequestAttributeList = request.Attribute;
            int putAttributesRequestAttributeListIndex = 1;
            foreach (ReplaceableAttribute putAttributesRequestAttribute in putAttributesRequestAttributeList)
            {
                if (putAttributesRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", putAttributesRequestAttributeListIndex, ".", "Name")] = putAttributesRequestAttribute.Name;
                }
                if (putAttributesRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", putAttributesRequestAttributeListIndex, ".", "Value")] = putAttributesRequestAttribute.Value;
                }
                if (putAttributesRequestAttribute.IsSetReplace())
                {
                    parameters[String.Concat("Attribute", ".", putAttributesRequestAttributeListIndex,
                        ".", "Replace")] = putAttributesRequestAttribute.Replace.ToString().ToLower(CultureInfo.InvariantCulture);
                }

                putAttributesRequestAttributeListIndex++;
            }
            if (request.IsSetExpected())
            {
                UpdateCondition putAttributesRequestExpected = request.Expected;
                if (putAttributesRequestExpected.IsSetName())
                {
                    parameters[String.Concat("Expected", ".", "Name")] = putAttributesRequestExpected.Name;
                }
                if (putAttributesRequestExpected.IsSetValue())
                {
                    parameters[String.Concat("Expected", ".", "Value")] = putAttributesRequestExpected.Value;
                }
                if (putAttributesRequestExpected.IsSetExists())
                {
                    parameters[String.Concat("Expected", ".", "Exists")] = putAttributesRequestExpected.Exists.ToString().ToLower(CultureInfo.InvariantCulture);
                }
            }

            return parameters;
        }

        #endregion

        #region BatchPutAttributes Operation

        /// <summary>
        /// Batch Put Attributes. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchPutAttributes"/>
        /// </summary>
        /// <param name="request">The BatchPutAttributesRequest that defines the parameters of
        /// the operation.</param>
        public void BatchPutAttributes(BatchPutAttributesRequest request)
        {
            BeginBatchPutAttributes(request);
        }

        /// <summary>
        /// Initiates the asynchronous execution of the BatchPutAttributes operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchPutAttributes"/>
        /// </summary>
        /// <param name="request">The BatchPutAttributesRequest that defines the parameters of
        /// the operation.</param>
        public void BeginBatchPutAttributes(BatchPutAttributesRequest request)
        {
            IDictionary<string, string> parameters = ConvertBatchPutAttributes(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<BatchPutAttributesResponse>(result);
        }

        /**
         * Convert BatchPutAttributesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertBatchPutAttributes(BatchPutAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "BatchPutAttributes";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }
            List<ReplaceableItem> batchPutAttributesRequestItemList = request.Items;
            int batchPutAttributesRequestItemListIndex = 1;
            foreach (ReplaceableItem batchPutAttributesRequestItem in batchPutAttributesRequestItemList)
            {
                if (batchPutAttributesRequestItem.IsSetItemName())
                {
                    parameters[String.Concat("Item", ".", batchPutAttributesRequestItemListIndex, ".", "ItemName")] = batchPutAttributesRequestItem.ItemName;
                }
                List<ReplaceableAttribute> itemAttributeList = batchPutAttributesRequestItem.Attribute;
                int itemAttributeListIndex = 1;
                foreach (ReplaceableAttribute itemAttribute in itemAttributeList)
                {
                    if (itemAttribute.IsSetName())
                    {
                        parameters[String.Concat("Item", ".", batchPutAttributesRequestItemListIndex, ".", "Attribute", ".", itemAttributeListIndex, ".", "Name")] = itemAttribute.Name;
                    }
                    if (itemAttribute.IsSetValue())
                    {
                        parameters[String.Concat("Item", ".", batchPutAttributesRequestItemListIndex, ".", "Attribute", ".", itemAttributeListIndex, ".", "Value")] = itemAttribute.Value;
                    }
                    if (itemAttribute.IsSetReplace())
                    {
                        parameters[String.Concat("Item", ".", batchPutAttributesRequestItemListIndex, ".",
                            "Attribute", ".", itemAttributeListIndex, ".", "Replace")] = itemAttribute.Replace.ToString().ToLowerInvariant();
                    }

                    itemAttributeListIndex++;
                }

                batchPutAttributesRequestItemListIndex++;
            }

            return parameters;
        }

        #endregion

        #region GetAttributes Operation

        /// <summary>
        /// Get Attributes
        /// </summary>
        /// <param name="request">Get Attribute  request</param>
        /// <remarks>
        /// Returns all of the attributes associated with the item. Optionally, the attributes returned can be limited to
        /// the specified AttributeName parameter.
        /// If the item does not exist on the replica that was accessed for this operation, an empty attribute is
        /// returned. The system does not return an error as it cannot guarantee the item does not exist on other
        /// replicas.
        /// </remarks>
        public void GetAttributes(GetAttributesRequest request)
        {
            BeginGetAttributes(request);
        }

        /// <summary>
        /// Initiates the asynchronous execution of the GetAttributes operation.
        /// </summary>
        /// <param name="request">Get Attribute  request</param>
        /// <remarks>
        /// Returns all of the attributes associated with the item. Optionally, the attributes returned can be limited to
        /// the specified AttributeName parameter.
        /// If the item does not exist on the replica that was accessed for this operation, an empty attribute is
        /// returned. The system does not return an error as it cannot guarantee the item does not exist on other
        /// replicas.
        /// </remarks>
        public void BeginGetAttributes(GetAttributesRequest request)
        {
            IDictionary<string, string> parameters = ConvertGetAttributes(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<GetAttributesResponse>(result);
        }

        /**
         * Convert GetAttributesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertGetAttributes(GetAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "GetAttributes";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }
            if (request.IsSetItemName())
            {
                parameters["ItemName"] = request.ItemName;
            }
            List<string> getAttributesRequestAttributeNameList = request.AttributeName;
            int getAttributesRequestAttributeNameListIndex = 1;
            foreach (string getAttributesRequestAttributeName in getAttributesRequestAttributeNameList)
            {
                parameters[String.Concat("AttributeName", ".", getAttributesRequestAttributeNameListIndex)] = getAttributesRequestAttributeName;
                getAttributesRequestAttributeNameListIndex++;
            }
            if (request.IsSetConsistentRead())
            {
                parameters["ConsistentRead"] = request.ConsistentRead.ToString().ToLower(CultureInfo.InvariantCulture);
            }

            return parameters;
        }


        #endregion

        #region DeleteAttributes Operation
        /// <summary>
        /// Delete Attributes. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteAttributes"/>
        /// </summary>
        /// <param name="request">The DeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        public void DeleteAttributes(DeleteAttributesRequest request)
        {
            BeginDeleteAttributes(request);
        }

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteAttributes operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteAttributes"/>
        /// </summary>
        /// <param name="request">The DeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        public void BeginDeleteAttributes(DeleteAttributesRequest request)
        {
            IDictionary<string, string> parameters = ConvertDeleteAttributes(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<DeleteAttributesResponse>(result);
        }

        /**
         * Convert DeleteAttributesRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertDeleteAttributes(DeleteAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "DeleteAttributes";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }
            if (request.IsSetItemName())
            {
                parameters["ItemName"] = request.ItemName;
            }
            List<Attribute> deleteAttributesRequestAttributeList = request.Attribute;
            int deleteAttributesRequestAttributeListIndex = 1;
            foreach (Attribute deleteAttributesRequestAttribute in deleteAttributesRequestAttributeList)
            {
                if (deleteAttributesRequestAttribute.IsSetName())
                {
                    parameters[String.Concat("Attribute", ".", deleteAttributesRequestAttributeListIndex, ".", "Name")] = deleteAttributesRequestAttribute.Name;
                }
                if (deleteAttributesRequestAttribute.IsSetValue())
                {
                    parameters[String.Concat("Attribute", ".", deleteAttributesRequestAttributeListIndex, ".", "Value")] = deleteAttributesRequestAttribute.Value;
                }
                if (deleteAttributesRequestAttribute.IsSetNameEncoding())
                {
                    parameters[String.Concat("Attribute", ".", deleteAttributesRequestAttributeListIndex, ".", "NameEncoding")] = deleteAttributesRequestAttribute.NameEncoding;
                }
                if (deleteAttributesRequestAttribute.IsSetValueEncoding())
                {
                    parameters[String.Concat("Attribute", ".", deleteAttributesRequestAttributeListIndex, ".", "ValueEncoding")] = deleteAttributesRequestAttribute.ValueEncoding;
                }

                deleteAttributesRequestAttributeListIndex++;
            }
            if (request.IsSetExpected())
            {
                UpdateCondition deleteAttributesRequestExpected = request.Expected;
                if (deleteAttributesRequestExpected.IsSetName())
                {
                    parameters[String.Concat("Expected", ".", "Name")] = deleteAttributesRequestExpected.Name;
                }
                if (deleteAttributesRequestExpected.IsSetValue())
                {
                    parameters[String.Concat("Expected", ".", "Value")] = deleteAttributesRequestExpected.Value;
                }
                if (deleteAttributesRequestExpected.IsSetExists())
                {
                    parameters[String.Concat("Expected", ".", "Exists")] = deleteAttributesRequestExpected.Exists.ToString().ToLower(CultureInfo.InvariantCulture);
                }
            }

            return parameters;
        }

        #endregion

        #region BatchDeleteAttributes Operation

        /// <summary>
        /// Batch Delete Attributes.
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchDeleteAttributes"/>
        /// </summary>
        /// <param name="request">The BatchDeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        public void BatchDeleteAttributes(BatchDeleteAttributesRequest request)
        {
            BeginBatchDeleteAttributes(request);
        }

        /// <summary>
        /// Initiates the asynchronous execution of the BatchDeleteAttributes operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchDeleteAttributes"/>
        /// </summary>
        /// <param name="request">The BatchDeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        public void BeginBatchDeleteAttributes(BatchDeleteAttributesRequest request)
        {
            IDictionary<string, string> parameters = ConvertBatchDeleteAttributes(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<BatchDeleteAttributesResponse>(result);
        }

        private static IDictionary<string, string> ConvertBatchDeleteAttributes(BatchDeleteAttributesRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "BatchDeleteAttributes";
            if (request.IsSetDomainName())
            {
                parameters["DomainName"] = request.DomainName;
            }
            List<DeleteableItem> batchDeleteAttributesRequestItemList = request.Item;
            int batchDeleteAttributesRequestItemListIndex = 1;
            foreach (DeleteableItem batchDeleteAttributesRequestItem in batchDeleteAttributesRequestItemList)
            {
                if (batchDeleteAttributesRequestItem.IsSetItemName())
                {
                    parameters[String.Concat("Item", ".", batchDeleteAttributesRequestItemListIndex, ".", "ItemName")] = batchDeleteAttributesRequestItem.ItemName;
                }
                List<Attribute> itemAttributeList = batchDeleteAttributesRequestItem.Attribute;
                int itemAttributeListIndex = 1;
                foreach (Attribute itemAttribute in itemAttributeList)
                {
                    if (itemAttribute.IsSetName())
                    {
                        parameters[String.Concat("Item", ".", batchDeleteAttributesRequestItemListIndex, ".", "Attribute", ".", itemAttributeListIndex, ".", "Name")] = itemAttribute.Name;
                    }
                    if (itemAttribute.IsSetValue())
                    {
                        parameters[String.Concat("Item", ".", batchDeleteAttributesRequestItemListIndex, ".", "Attribute", ".", itemAttributeListIndex, ".", "Value")] = itemAttribute.Value;
                    }

                    itemAttributeListIndex++;
                }

                batchDeleteAttributesRequestItemListIndex++;
            }

            return parameters;
        }

        #endregion

        #region Select Operation

        /// <summary>
        /// Select
        /// </summary>
        /// <param name="request">Select  request</param>
        /// <remarks>
        /// The Select operation returns a set of item names and associate attributes that match the
        /// query expression. Select operations that run longer than 5 seconds will likely time-out
        /// and return a time-out error response.
        /// </remarks>
        public void Select(SelectRequest request)
        {
            IDictionary<string, string> parameters = ConvertSelect(request);
            SDBAsyncResult result = new SDBAsyncResult(parameters, null);
            invoke<SelectResponse>(result);
        }

        /**
         * Convert SelectRequest to name value pairs
         */
        private static IDictionary<string, string> ConvertSelect(SelectRequest request)
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["Action"] = "Select";
            if (request.IsSetSelectExpression())
            {
                parameters["SelectExpression"] = request.SelectExpression;
            }
            if (request.IsSetNextToken())
            {
                parameters["NextToken"] = request.NextToken;
            }
            if (request.IsSetConsistentRead())
            {
                parameters["ConsistentRead"] = request.ConsistentRead.ToString().ToLower(CultureInfo.InvariantCulture);
            }

            return parameters;
        }

        #endregion

        #region Runtime

        void invoke<T>(SDBAsyncResult sdbAsyncResult) where T : new()
        {
            /* Add required request parameters */
            addRequiredParameters(sdbAsyncResult.Parameters);

            string queryString = AWSSDKUtils.GetParametersAsString(sdbAsyncResult.Parameters);

            byte[] requestData = Encoding.UTF8.GetBytes(queryString);

            HttpWebRequest request = configureWebRequest(config);
            sdbAsyncResult.RequestState = new RequestState(request, requestData);

            request.BeginGetRequestStream(new AsyncCallback(this.getRequestStreamCallback<T>), sdbAsyncResult);
        }

        [method: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void getRequestStreamCallback<T>(IAsyncResult result) where T : new()
        {
            SDBAsyncResult sdbAsyncResult = result as SDBAsyncResult;
            if (null == sdbAsyncResult)
                sdbAsyncResult = result.AsyncState as SDBAsyncResult;

            sdbAsyncResult.RequestState.GetRequestStreamCallbackCalled = true;
            try
            {
                RequestState state = sdbAsyncResult.RequestState;

                Stream requestStream;
                requestStream = state.WebRequest.EndGetRequestStream(result);

                using (requestStream)
                {
                    requestStream.Write(sdbAsyncResult.RequestState.RequestData, 0, sdbAsyncResult.RequestState.RequestData.Length);
                }
                state.WebRequest.BeginGetResponse(new AsyncCallback(this.getResponseCallback<T>), sdbAsyncResult);
            }
            catch (Exception e)
            {
                sdbAsyncResult.RequestState.WebRequest.Abort();
                LOGGER.Error("Error for GetRequestStream", e);
                sdbAsyncResult.Exception = e;
            }
        }

        [method: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void getResponseCallback<T>(IAsyncResult result) where T : new()
        {
            SDBAsyncResult sdbAsyncResult = result as SDBAsyncResult;

            if (null == sdbAsyncResult)
                sdbAsyncResult = result.AsyncState as SDBAsyncResult;

            sdbAsyncResult.RequestState.GetResponseCallbackCalled = true;
            bool shouldRetry = false;
            try
            {
                HttpStatusCode statusCode;
                RequestState state = sdbAsyncResult.RequestState;
                HttpWebResponse httpResponse = null;
                try
                {
                    httpResponse = state.WebRequest.EndGetResponse(result) as HttpWebResponse;
                    sdbAsyncResult.FinalSDBResponse = handleHttpResponse<T>(httpResponse, out statusCode) as SimpleDBResponse;
                }
                catch (WebException we)
                {
                    shouldRetry = handleHttpWebErrorResponse(sdbAsyncResult, we, out statusCode);
                }

                if (shouldRetry)
                {
                    sdbAsyncResult.RequestState.WebRequest.Abort();
                    sdbAsyncResult.RetriesAttempt++;
                    handleRetry(sdbAsyncResult, statusCode);
                    invoke<T>(sdbAsyncResult);
                }
            }
            catch (Exception e)
            {
                AmazonSimpleDBException exception = e as AmazonSimpleDBException;

                if (null != exception)
                {
                    sdbAsyncResult.FinalSDBResponse = exception;
                    return;
                }
                sdbAsyncResult.RequestState.WebRequest.Abort();
                LOGGER.Error("Error for GetResponse", e);
                sdbAsyncResult.Exception = e;
                shouldRetry = false;
            }
            finally
            {
                if (!shouldRetry)
                {
                    if (null != OnSimpleDBResponse)
                        OnSimpleDBResponse.Invoke(this, new ResponseEventArgs(sdbAsyncResult.FinalSDBResponse));
                }
            }
        }


        static T handleHttpResponse<T>(HttpWebResponse httpResponse, out HttpStatusCode statusCode)
        {
            string responseBody;
            using (httpResponse)
            {
                if (httpResponse == null)
                {
                    throw new WebException(
                        "The Web Response for a successful request is null!",
                        WebExceptionStatus.ProtocolError
                        );
                }
                statusCode = httpResponse.StatusCode;
                using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                {
                    responseBody = reader.ReadToEnd().Trim();
                }
            }

            T response;
            /* Attempt to deserialize response into <Action> Response type */
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XDocument doc = XDocument.Parse(responseBody);

            using (XmlReader sr = doc.CreateReader())
            {
                response = (T)serializer.Deserialize(sr);
            }

            return response;
        }

        bool handleHttpWebErrorResponse(SDBAsyncResult sdbAsyncResult, WebException we, out HttpStatusCode statusCode)
        {
            string responseBody;
            using (HttpWebResponse httpErrorResponse = we.Response as HttpWebResponse)
            {
                if (httpErrorResponse == null)
                {
                    // Abort the unsuccessful request
                    sdbAsyncResult.RequestState.WebRequest.Abort();
                    throw we;
                }
                statusCode = httpErrorResponse.StatusCode;
                using (StreamReader reader = new StreamReader(httpErrorResponse.GetResponseStream(), Encoding.UTF8))
                {
                    responseBody = reader.ReadToEnd();
                }

                // Abort the unsuccessful request
                sdbAsyncResult.RequestState.WebRequest.Abort();
            }

            bool shouldRetry = false;
            if (statusCode == HttpStatusCode.InternalServerError ||
                statusCode == HttpStatusCode.ServiceUnavailable)
            {
                shouldRetry = true;
                sdbAsyncResult.RetriesAttempt++;
                pauseOnRetry(sdbAsyncResult.RetriesAttempt, this.config.MaxErrorRetry, statusCode);
            }
            else
            {
                throw reportAnyErrors(responseBody, statusCode);
            }

            return shouldRetry;
        }


        void handleRetry(SDBAsyncResult sdbAsyncResult, HttpStatusCode statusCode)
        {
            int retries = sdbAsyncResult.RetriesAttempt;
            if (retries <= this.config.MaxErrorRetry)
            {
                LOGGER.InfoFormat("Retry number {0} for request {1}.", retries, sdbAsyncResult.ActionName);
            }
            pauseOnRetry(retries, this.config.MaxErrorRetry, statusCode);
        }

        /**
          * Configure HttpClient with set of defaults as well as configuration
          * from AmazonSimpleDBConfig instance
          */
        private static HttpWebRequest configureWebRequest(AmazonSimpleDBConfig config)
        {
            //Create the header and assign it to the HttpWebRequest
            WebHeaderCollection header = new WebHeaderCollection();
            header[HttpRequestHeader.Expires] = "50000";

            HttpWebRequest request = WebRequest.Create(config.ServiceURL) as HttpWebRequest;
            request.Headers = header;

            if (request != null)
            {
                request.UserAgent = config.UserAgent;
                request.Method = "POST";
                request.ContentType = AWSSDKUtils.UrlEncodedContent;
            }

            return request;
        }

        /**
          * Look for additional error strings in the response and return formatted exception
          */
        private static AmazonSimpleDBException reportAnyErrors(string responseBody, HttpStatusCode status)
        {
            AmazonSimpleDBException ex = null;

            if (responseBody != null && responseBody.StartsWith("<", StringComparison.OrdinalIgnoreCase))
            {
                Match errorMatcherOne = Regex.Match(
                    responseBody,
                    "<RequestId>(.*)</RequestId>.*<Error><Code>(.*)</Code><Message>(.*)</Message></Error>.*(<Error>)?",
                    RegexOptions.Multiline
                    );
                Match errorMatcherTwo = Regex.Match(
                    responseBody,
                    "<Error><Code>(.*)</Code><Message>(.*)</Message></Error>.*(<Error>)?.*<RequestID>(.*)</RequestID>",
                    RegexOptions.Multiline
                    );
                Match errorMatcherThree = Regex.Match(
                    responseBody,
                    "<Error><Code>(.*)</Code><Message>(.*)</Message><BoxUsage>(.*)</BoxUsage></Error>.*(<Error>)?.*<RequestID>(.*)</RequestID>",
                    RegexOptions.Multiline);

                if (errorMatcherOne.Success)
                {
                    string requestId = errorMatcherOne.Groups[1].Value;
                    string code = errorMatcherOne.Groups[2].Value;
                    string message = errorMatcherOne.Groups[3].Value;

                    ex = new AmazonSimpleDBException(message, status, code, "Unknown", null, requestId, responseBody);
                }
                else if (errorMatcherTwo.Success)
                {
                    string code = errorMatcherTwo.Groups[1].Value;
                    string message = errorMatcherTwo.Groups[2].Value;
                    string requestId = errorMatcherTwo.Groups[4].Value;

                    ex = new AmazonSimpleDBException(message, status, code, "Unknown", null, requestId, responseBody);
                }
                else if (errorMatcherThree.Success)
                {
                    string code = errorMatcherThree.Groups[1].Value;
                    string message = errorMatcherThree.Groups[2].Value;
                    string boxUsage = errorMatcherThree.Groups[3].Value;
                    string requestId = errorMatcherThree.Groups[5].Value;

                    ex = new AmazonSimpleDBException(message, status, code, "Unknown", boxUsage, requestId, responseBody);
                }
                else
                {
                    ex = new AmazonSimpleDBException("Internal Error", status);
                }
            }
            else
            {
                ex = new AmazonSimpleDBException("Internal Error", status);
            }
            return ex;
        }

        /**
         * Exponential sleep on failed request
         */
        private static void pauseOnRetry(int retries, int maxRetries, HttpStatusCode status)
        {
            if (retries <= maxRetries)
            {
                int delay = (int)Math.Pow(4, retries) * 100;
                System.Threading.Thread.Sleep(delay);
            }
            else
            {
                throw new AmazonSimpleDBException(
                    "Maximum number of retry attempts reached : " + (retries - 1),
                    status
                    );
            }
        }

        /**
         * Add authentication related and version parameters
         */
        private void addRequiredParameters(IDictionary<string, string> parameters)
        {
            if (String.IsNullOrEmpty(this.awsAccessKeyId))
            {
                throw new AmazonSimpleDBException("The AWS Access Key ID cannot be NULL or a Zero length string");
            }

            if (parameters.ContainsKey("Signature"))
            {
                parameters.Remove("Signature");
            }

            parameters["AWSAccessKeyId"] = this.awsAccessKeyId;
            parameters["SignatureVersion"] = config.SignatureVersion;
            parameters["SignatureMethod"] = config.SignatureMethod;
            parameters["Timestamp"] = AmazonSimpleDBUtil.FormattedCurrentTimestamp;
            parameters["Version"] = config.ServiceVersion;
            if (!config.SignatureVersion.Equals("2"))
            {
                throw new AmazonSimpleDBException("Invalid Signature Version specified");
            }
            string toSign = AWSSDKUtils.CalculateStringToSignV2(parameters, config.ServiceURL, "POST");

            using (KeyedHashAlgorithm algorithm = new HMACSHA256())
            {
                string auth = AWSSDKUtils.HMACSign(toSign, clearAwsSecretAccessKey, algorithm);

                parameters["Signature"] = auth;
            }
        }

        #region Async Classes
        class SDBAsyncResult
        {
            IDictionary<string, string> _parameters;
            RequestState _requestState;
            object _state;
            int _retiresAttempt;
            Exception _exception;
            ISimpleDBResponse _finalSDBResponse;

            internal SDBAsyncResult(IDictionary<string, string> parameters, object state)
            {
                this._parameters = parameters;
                this._state = state;
            }

            internal string ActionName
            {
                get { return this._parameters["Action"]; }
            }

            internal IDictionary<string, string> Parameters
            {
                get { return this._parameters; }
            }

            [property: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            internal Exception Exception
            {
                get { return this._exception; }
                set { this._exception = value; }
            }

            internal int RetriesAttempt
            {
                get { return this._retiresAttempt; }
                set { this._retiresAttempt = value; }
            }

            internal RequestState RequestState
            {
                get { return this._requestState; }
                set { this._requestState = value; }
            }

            internal ISimpleDBResponse FinalSDBResponse
            {
                get { return this._finalSDBResponse; }
                set { this._finalSDBResponse = value; }
            }
        }


        class RequestState
        {
            HttpWebRequest _webRequest;
            byte[] _requestData;
            bool _getRequestStreamCallbackCalled;
            bool _getResponseCallbackCalled;

            public RequestState(HttpWebRequest webRequest, byte[] requestData)
            {
                this._webRequest = webRequest;
                this._requestData = requestData;
            }

            internal HttpWebRequest WebRequest
            {
                get { return this._webRequest; }
            }

            internal byte[] RequestData
            {
                get { return this._requestData; }
            }


            internal bool GetRequestStreamCallbackCalled
            {
                set { this._getRequestStreamCallbackCalled = value; }
            }

            internal bool GetResponseCallbackCalled
            {
                set { this._getResponseCallbackCalled = value; }
            }
        }
        #endregion

        #endregion
    }
}
