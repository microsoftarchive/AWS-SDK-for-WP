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
using Amazon.SimpleDB.Model;

namespace Amazon.SimpleDB
{
    /// <summary>
    /// Amazon SimpleDB is a web service for running queries on structured
    /// data in real time. This service works in close conjunction with Amazon
    /// Simple Storage Service (Amazon S3) and Amazon Elastic Compute Cloud
    /// (Amazon EC2), collectively providing the ability to store, process
    /// and query data sets in the cloud. These services are designed to make
    /// web-scale computing easier and more cost-effective for developers.
    /// Traditionally, this type of functionality has been accomplished with
    /// a clustered relational database that requires a sizable upfront
    /// investment, brings more complexity than is typically needed, and often
    /// requires a DBA to maintain and administer. In contrast, Amazon SimpleDB
    /// is easy to use and provides the core functionality of a database -
    /// real-time lookup and simple querying of structured data without the
    /// operational complexity.  Amazon SimpleDB requires no schema, automatically
    /// indexes your data and provides a simple API for storage and access.
    /// This eliminates the administrative burden of data modeling, index
    /// maintenance, and performance tuning. Developers gain access to this
    /// functionality within Amazon's proven computing environment, are able
    /// to scale instantly, and pay only for what they use.
    /// </summary>
    public interface AmazonSimpleDB : IDisposable
    {
        event SimpleDBResponseEventHandler<object, ResponseEventArgs> OnSimpleDBResponse;
        #region CreateDomain Operation

        /// <summary>
        /// Initiates the asynchronous execution of the CreateDomain operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.CreateDomain"/>
        /// </summary>
        /// <param name="request">The CreateDomainRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginCreateDomain(CreateDomainRequest request);

        /// <summary>
        /// Create Domain. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.CreateDomain"/>
        /// </summary>
        /// <param name="request">The CreateDomainRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void CreateDomain(CreateDomainRequest request);

        #endregion

        #region ListDomains Operation

        /// <summary>
        /// Initiates the asynchronous execution of the ListDomains operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.ListDomains"/>
        /// </summary>
        /// <param name="request">The ListDomainsRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginListDomains(ListDomainsRequest request);

        /// <summary>
        /// List Domains. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.ListDomains"/>
        /// </summary>
        /// <param name="request">The ListDomainsRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void ListDomains(ListDomainsRequest request);

        #endregion

        #region DomainMetadata Operation

        /// <summary>
        /// Initiates the asynchronous execution of the DomainMetadata operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DomainMetadata"/>
        /// </summary>
        /// <param name="request">The DomainMetadataRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginDomainMetadata(DomainMetadataRequest request);

        /// <summary>
        /// Domain Metadata. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DomainMetadata"/>
        /// </summary>
        /// <param name="request">The DomainMetadataRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void DomainMetadata(DomainMetadataRequest request);

        #endregion

        #region DeleteDomain Operation

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteDomain operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteDomain"/>
        /// </summary>
        /// <param name="request">The DeleteDomainRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginDeleteDomain(DeleteDomainRequest request);

        /// <summary>
        /// Delete Domain. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteDomain"/>
        /// </summary>
        /// <param name="request">The DeleteDomainRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void DeleteDomain(DeleteDomainRequest request);

        #endregion

        #region PutAttributes Operation

        /// <summary>
        /// Put Attribute
        /// </summary>
        /// <param name="request">Put Attribute  request</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
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
        void PutAttributes(PutAttributesRequest request);

        /// <summary>
        /// Initiates the asynchronous execution of the PutAttributes operation. 
        /// </summary>
        /// <param name="request">Put Attribute  request</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
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
        void BeginPutAttributes(PutAttributesRequest request);
        
        #endregion

        #region BatchPutAttributes Operation

        /// <summary>
        /// Batch Put Attributes. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchPutAttributes"/>
        /// </summary>
        /// <param name="request">The BatchPutAttributesRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BatchPutAttributes(BatchPutAttributesRequest request);

        /// <summary>
        /// Initiates the asynchronous execution of the BatchPutAttributes operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchPutAttributes"/>
        /// </summary>
        /// <param name="request">The BatchPutAttributesRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginBatchPutAttributes(BatchPutAttributesRequest request);

        #endregion

        #region GetAttributes Operation

        /// <summary>
        /// Get Attributes.
        /// </summary>
        /// <param name="request">Get Attribute request</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        /// <remarks>
        /// Returns all of the attributes associated with the item. Optionally, the attributes returned can be limited to
        /// the specified AttributeName parameter.
        /// If the item does not exist on the replica that was accessed for this operation, an empty attribute is
        /// returned. The system does not return an error as it cannot guarantee the item does not exist on other
        /// replicas.
        /// </remarks>
        void GetAttributes(GetAttributesRequest request);

        /// <summary>
        /// Initiates the asynchronous execution of the GetAttributes operation. 
        /// </summary>
        /// <param name="request">Get Attribute  request</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        /// <remarks>
        /// Returns all of the attributes associated with the item. Optionally, the attributes returned can be limited to
        /// the specified AttributeName parameter.
        /// If the item does not exist on the replica that was accessed for this operation, an empty attribute is
        /// returned. The system does not return an error as it cannot guarantee the item does not exist on other
        /// replicas.
        /// </remarks>
        void BeginGetAttributes(GetAttributesRequest request);

        #endregion

        #region DeleteAttributes Operation

        /// <summary>
        /// Delete Attributes. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteAttributes"/>
        /// </summary>
        /// <param name="request">The DeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void DeleteAttributes(DeleteAttributesRequest request);

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteAttributes operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.DeleteAttributes"/>
        /// </summary>
        /// <param name="request">The DeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginDeleteAttributes(DeleteAttributesRequest request);

        #endregion

        #region BatchDeleteAttributes Operation

        /// <summary>
        /// Initiates the asynchronous execution of the BatchDeleteAttributes operation. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchDeleteAttributes"/>
        /// </summary>
        /// <param name="request">The BatchDeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BeginBatchDeleteAttributes(BatchDeleteAttributesRequest request);

        /// <summary>
        /// Batch Delete Attributes. 
        /// <seealso cref="M:Amazon.SimpleDB.AmazonSimpleDB.BatchDeleteAttributes"/>
        /// </summary>
        /// <param name="request">The BatchDeleteAttributesRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.SimpleDB.AmazonSimpleDBException"></exception>
        void BatchDeleteAttributes(BatchDeleteAttributesRequest request);

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
        void Select(SelectRequest request);

        #endregion
    }

    /// <summary>
    /// A type that acts as the event argument for the <see cref=""/> event.
    /// </summary>
    public class ResponseEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the <see cref="ISimpleDBResponse"/> object.
        /// </summary>
        public ISimpleDBResponse Response { get; private set; }
        public ResponseEventArgs(ISimpleDBResponse response)
        {
            this.Response = response;
        }
    }
}
