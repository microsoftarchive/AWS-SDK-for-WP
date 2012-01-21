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
using Amazon.S3.Model;

namespace Amazon.S3
{
    public class ResponseEventArgs : EventArgs
    {
        public IS3Response Response { get; private set; }
        public ResponseEventArgs(IS3Response response)
        {
            this.Response = response;
        }
    }

    /// <summary>
    /// Interface for Amazon S3 Clients.
    /// For more information about Amazon S3, go to <see href="http://aws.amazon.com/s3"/>
    /// </summary>
    public interface AmazonS3 : IDisposable
    {
        /// <summary>
        /// The event to register the response of S3 Service
        /// </summary>
        event S3ResponseEventHandler<object, ResponseEventArgs> OnS3Response;

        #region GetPreSignedURL

        /// <summary>
        /// The GetPreSignedURL operations creates a signed http request.
        /// Query string authentication is useful for giving HTTP or browser
        /// access to resources that would normally require authentication.
        /// When using query string authentication, you create a query,
        /// specify an expiration time for the query, sign it with your
        /// signature, place the data in an HTTP request, and distribute
        /// the request to a user or embed the request in a web page.
        /// A PreSigned URL can be generated for GET, PUT and HEAD
        /// operations on your bucket, keys, and versions.
        /// </summary>
        /// <param name="request">The GetPreSignedUrlRequest that defines the
        /// parameters of the operation.</param>
        /// <returns>A string that is the signed http request.</returns>
        /// <exception cref="T:System.ArgumentException" />
        /// <exception cref="T:System.ArgumentNullException" />
        string GetPreSignedURL(GetPreSignedUrlRequest request);

        #endregion

        #region ListBuckets

        /// <summary>
        /// The ListBuckets operation returns a list of all of the buckets
        /// owned by the authenticated sender of the request.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void ListBuckets();

        /// <summary>
        /// Initiates the asynchronous execution of the ListBuckets operation. 
        /// The ListBuckets operation returns a list of all of the buckets
        /// owned by the authenticated sender of the request.
        /// </summary>
        /// <param name="request">The ListBucketsRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void ListBuckets(ListBucketsRequest request);

        #endregion

        #region GetBucketLocation

        /// <summary>
        /// Initiates the asynchronous execution of the GetBucketLocation operation. 
        /// The GetBucketLogging operating gets the logging status for the bucket specified.
        /// For more information about S3Bucket Logging
        /// see <see href="http://docs.amazonwebservices.com/AmazonS3/latest/ServerLogs.html" />
        /// </summary>
        /// <param name="request">The GetBucketLocationRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void GetBucketLocation(GetBucketLocationRequest request);

        #endregion

        #region GetBucketLogging

        /// <summary>
        /// Initiates the asynchronous execution of the GetBucketLogging operation. 
        /// The GetBucketLogging operating gets the logging status for the bucket specified.
        /// For more information about S3Bucket Logging
        /// see <see href="http://docs.amazonwebservices.com/AmazonS3/latest/ServerLogs.html" />
        /// </summary>
        /// <param name="request">The GetBucketLoggingRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void GetBucketLogging(GetBucketLoggingRequest request);

        #endregion

        #region EnableBucketLogging

        /// <summary>
        /// Initiates the asynchronous execution of the EnableBucketLogging operation. 
        /// 
        /// The EnableBucketLogging operation will turn on bucket logging for the bucket
        /// specified in the request.
        /// 
        /// An Amazon S3 bucket can be configured to create access log records for the
        /// requests made against it. An access log record contains details about the
        /// request such as the request type, the resource with which the request worked,
        /// and the time and date that the request was processed. Server access logs are
        /// useful for many applications, because they give bucket owners insight into
        /// the nature of requests made by clients not under their control.
        /// </summary>
        /// <param name="request">The EnableBucketLoggingRequest that defines the parameters of
        /// the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <remarks>
        /// Changes to the logging status for a bucket are visible in the configuration API immediately,
        /// but they take time to actually affect the delivery of log files. For example, if you enable
        /// logging for a bucket, some requests made in the following hour might be logged, while others
        /// might not. Or, if you change the target bucket for logging from bucket A to bucket B, some
        /// logs for the next hour might continue to be delivered to bucket A, while others might be delivered
        /// to the new target bucket B. In all cases, the new settings will eventually take effect without any
        /// further action on your part.
        /// </remarks>
        void EnableBucketLogging(EnableBucketLoggingRequest request);

        #endregion

        #region DisableBucketLogging

        /// <summary>
        /// Initiates the asynchronous execution of the DisableBucketLogging operation.
        /// The DisableBucketLogging will turn off bucket logging for the bucket specified
        /// in the request.
        ///
        /// An Amazon S3 bucket can be configured to create access log records for the
        /// requests made against it. An access log record contains details about the
        /// request such as the request type, the resource with which the request worked,
        /// and the time and date that the request was processed. Server access logs are
        /// useful for many applications, because they give bucket owners insight into
        /// the nature of requests made by clients not under their control.
        /// </summary>
        /// <param name="request">
        /// The DisableBucketLoggingRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void DisableBucketLogging(DisableBucketLoggingRequest request);

        #endregion

        #region GetBucketPolicy

        /// <summary>
        /// Initiates the asynchronous execution of the GetBucketPolicy operation. 
        /// <para>
        /// Retrieves the policy for the specified bucket. Only the owner of the
        /// bucket can retrieve the policy. If no policy has been set for the bucket,
        /// then an error will be thrown.
        /// </para>
        /// <para>
        /// Bucket policies provide access control management at the bucket level for
        /// both the bucket resource and contained object resources. Only one policy
        /// can be specified per-bucket.
        /// </para>
        /// <para>
        /// For more information on forming bucket polices, 
        /// refer: <see href="http://docs.amazonwebservices.com/AmazonS3/latest/dev/"/>
        /// </para>
        /// </summary>
        /// <param name="request">The GetBucketPolicyRequest that defines the parameters of the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void GetBucketPolicy(GetBucketPolicyRequest request);

        #endregion

        #region PutBucketPolicy

        /// <summary>
        /// Initiates the asynchronous execution of the PutBucketPolicy operation. 
        /// <para>
        /// Sets the policy associated with the specified bucket. Only the owner of
        /// the bucket can set a bucket policy. If a policy already exists for the
        /// specified bucket, the new policy will replace the existing policy.
        /// </para>
        /// <para>
        /// Bucket policies provide access control management at the bucket level for
        /// both the bucket resource and contained object resources. Only one policy
        /// may be specified per-bucket.
        /// </para>
        /// <para>
        /// For more information on forming bucket polices, 
        /// refer: <see href="http://docs.amazonwebservices.com/AmazonS3/latest/dev/"/>
        /// </para>
        /// </summary>
        /// <param name="request">The PutBucketPolicyRequest that defines the parameters of the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void PutBucketPolicy(PutBucketPolicyRequest request);

        #endregion

        #region DeleteBucketPolicy

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteBucketPolicy operation
        /// <para>
        /// Deletes the policy associated with the specified bucket. Only the owner
        /// of the bucket can delete the bucket policy.
        /// </para>
        /// <para>
        /// If you delete a policy that does not exist, Amazon S3 will return a
        /// success (not an error message).
        /// </para>
        /// <para>
        /// Bucket policies provide access control management at the bucket level for
        /// both the bucket resource and contained object resources. Only one policy
        /// may be specified per-bucket.
        /// </para>
        /// <para>
        /// For more information on forming bucket polices, 
        /// refer: <see href="http://docs.amazonwebservices.com/AmazonS3/latest/dev/"/>
        /// </para>
        /// </summary>
        /// <param name="request">The DeleteBucketPolicyRequest that defines the parameters of the operation.</param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void DeleteBucketPolicy(DeleteBucketPolicyRequest request);

        #endregion

        #region ListObjects

        /// <summary>
        /// Initiates the asynchronous execution of the ListObjects operation. 
        /// 
        /// The ListObjects operation lists the objects/keys in a bucket ordered
        /// lexicographically (from a-Z). The list can be filtered via the Marker
        /// property of the ListObjectsRequest.
        /// In order to List Objects, you must have READ access to the bucket.
        /// </summary>
        /// <param name="request">
        /// The ListObjectsRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <returns>Returns a ListObjectsResponse from S3 with a list of S3Objects,
        /// headers and request parameters used to filter the list.</returns>
        /// <remarks>
        /// Since buckets can contain a virtually unlimited number of objects, the complete
        /// results of a list query can be extremely large. To manage large result sets,
        /// Amazon S3 uses pagination to split them into multiple responses. Callers should
        /// always check the <see cref="P:Amazon.S3.Model.ListObjectsResponse.IsTruncated" />
        /// to see if the returned listing
        /// is complete, or if callers need to make additional calls to get more results.
        /// The marker parameter allows callers to specify where to start the object listing.
        /// List performance is not substantially affected by the total number of keys in your
        /// bucket, nor by the presence or absence of any additional query parameters.
        /// </remarks>
        void ListObjects(ListObjectsRequest request);

        #endregion

        #region GetACL

        /// <summary>
        /// Initiates the asynchronous execution of the GetACL operation.
        /// 
        /// The GetACL operation gets the S3AccessControlList for a given bucket or object.
        /// Each bucket and object in Amazon S3 has an ACL that defines its access control
        /// policy, which is a list of grants. A grant consists of one grantee and one permission.
        /// ACLs only grant permissions; they do not deny them.
        /// </summary>
        /// <param name="request">
        /// The GetACLRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <seealso cref="T:Amazon.S3.Model.S3AccessControlList"/>
        void GetACL(GetACLRequest request);

        #endregion

        #region SetACL

        /// <summary>
        /// Initiates the asynchronous execution of the SetACL operation.
        /// 
        /// The SetACL operation gets the S3AccessControlList for a given bucket or object.
        /// Each bucket and object in Amazon S3 has an ACL that defines its access control
        /// policy, which is a list of grants. A grant consists of one grantee and one permission.
        /// ACLs only grant permissions; they do not deny them.
        /// </summary>
        /// <remarks>
        /// Often used ACLs are defined in the S3CannedACL enumeration.
        /// </remarks>
        /// <param name="request">
        /// The SetACLRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <seealso cref="T:Amazon.S3.Model.S3AccessControlList"/>
        void SetACL(SetACLRequest request);

        #endregion

        #region PutBucket

        /// <summary>
        /// Initiates the asynchronous execution of the PutBucket operation.
        /// 
        /// The PutBucket operation creates a new S3 Bucket.
        /// Depending on your latency and legal requirements, you can specify a location
        /// constraint that will affect where your data physically resides.
        /// </summary>
        /// <param name="request">
        /// The PutBucketRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <remarks>
        /// Every object stored in Amazon S3 is contained in a bucket. Buckets
        /// partition the namespace of objects stored in Amazon S3 at the top level.
        /// Within a bucket, you can use any names for your objects, but bucket names
        /// must be unique across all of Amazon S3.
        /// <para>
        /// Buckets are similar to Internet domain names. Just as Amazon is the only owner
        /// of the domain name Amazon.com, only one person or organization can own a bucket
        /// within Amazon S3. The similarities between buckets and domain names is not a
        /// coincidence - there is a direct mapping between Amazon S3 buckets and subdomains
        /// of s3.amazonaws.com. Objects stored in Amazon S3 are addressable using the REST API
        /// under the domain bucketname.s3.amazonaws.com. For example, the object homepage.html
        /// stored in the Amazon S3 bucket mybucket can be addressed as
        /// http://mybucket.s3.amazonaws.com/homepage.html.</para>
        /// To conform with DNS requirements, the following constraints apply:
        /// <list type="bullet">
        /// <item>Bucket names should not contain underscores (_)</item>
        /// <item>Bucket names should be between 3 and 63 characters long</item>
        /// <item>Bucket names should not end with a dash</item>
        /// <item>Bucket names cannot contain two, adjacent periods</item>
        /// <item>Bucket names cannot contain dashes next to periods
        ///   (e.g., "my-.bucket.com" and "my.-bucket" are invalid)</item>
        /// <item>Bucket names cannot contain uppercase characters</item>
        /// </list>
        /// There is no limit to the number of objects that can be stored in a bucket and no
        /// variation in performance when using many buckets or just a few. You can store all
        /// of your objects in a single bucket or organize them across several buckets.
        /// </remarks>
        /// <seealso cref="T:Amazon.S3.Model.S3Region"/>
        void PutBucket(PutBucketRequest request);

        #endregion

        #region DeleteBucket

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteBucket operation. 
        /// 
        /// The DeleteBucket operation deletes the bucket named in the request.
        /// All objects in the bucket must be deleted before the bucket itself can be deleted.
        /// Only the owner of a bucket can delete it, regardless of the bucket's access control policy.
        /// </summary>
        /// <param name="request">
        /// The DeleteBucketRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void DeleteBucket(DeleteBucketRequest request);

        #endregion

        #region GetObject

        /// <summary>
        /// Initiates the asynchronous execution of the GetObject operation. 
        /// 
        /// The GetObject operation fetches the most recent version of an S3 object
        /// from the specified S3 bucket. You must have READ access to the object.
        /// If READ access is granted to an anonymous user, an object can be retrieved
        /// without an authorization header. Providing a version-id for the object will
        /// fetch the specific version from S3 instead of the most recent one. To get the result, hook on to 
        /// <see cref="S3ResponseDelgate<IS3Response> S3WebResponse" event./>
        /// </summary>
        /// <param name="request">
        /// The GetObjectRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <remarks>
        /// Please wrap the response you get from calling GetObject in a using clause.
        /// This ensures that all underlying IO resources allocated for the response
        /// are disposed once the response has been processed. This is one way to
        /// call GetObject:
        /// <code>
        /// using (GetObjectResponse response = s3Client.GetObject(request))
        /// {
        ///     ... Process the response:
        ///     Get the Stream, get the content-length, write contents to disk, etc
        /// }
        /// </code>
        /// To see what resources are cleaned up at the end of the using block, please
        /// see <see cref="M:Amazon.S3.Model.S3Response.Dispose"/>
        /// </remarks>
        void GetObject(GetObjectRequest request);

        #endregion

        #region GetObjectMetadata

        /// <summary>
        /// Initiates the asynchronous execution of the GetObjectMetadata operation.
        /// 
        /// The GetObjectMetadata operation is used to retrieve information about a specific object
        /// or object size, without actually fetching the object itself. This is useful if you're
        /// only interested in the object metadata, and don't want to waste bandwidth on the object data.
        /// The response is identical to the GetObject response, except that there is no response body.
        /// Hook on the <see cref="S3ResponseDelgate<IS3Response> S3WebResponse"/> event to get
        /// the response.
        /// </summary>
        /// <param name="request">
        /// The GetObjectMetadataRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void GetObjectMetadata(GetObjectMetadataRequest request);

        #endregion

        #region PutObject

        /// <summary>
        /// Initiates the asynchronous execution of the PutObject operation.
        /// 
        /// The PutObject operation adds an object to an S3 Bucket.
        /// The response indicates that the object has been successfully stored.
        /// Amazon S3 never stores partial objects: if you receive a successful
        /// response, then you can be confident that the entire object was stored. 
        /// Hook on the <see cref="S3ResponseDelgate<IS3Response> S3WebResponse"/> event to get
        /// the response.
        ///
        /// To ensure data is not corrupted over the network, use the Content-MD5
        /// header. When you use the Content-MD5 header, Amazon S3 checks the object
        /// against the provided MD5 value. If they do not match, Amazon S3 returns an error.
        /// Additionally, you can calculate the MD5 while putting an object to
        /// Amazon S3 and compare the returned Etag to the calculated MD5 value.
        ///
        /// If an object already exists in a bucket, the new object will overwrite
        /// it because Amazon S3 stores the last write request. However, Amazon S3
        /// is a distributed system. If Amazon S3 receives multiple write requests
        /// for the same object nearly simultaneously, all of the objects might be
        /// stored, even though only one wins in the end. Amazon S3 does not provide
        /// object locking; if you need this, make sure to build it into your application
        /// layer.
        ///
        /// If you specify a location constraint when creating a bucket, all objects
        /// added to the bucket are stored in the bucket's location.
        ///
        /// You must have WRITE access to the bucket to add an object.
        /// </summary>
        /// <param name="request">
        /// The PutObjectRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentException"></exception>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <exception cref="T:System.IO.FileNotFoundException"></exception>
        void PutObject(PutObjectRequest request);

        #endregion

        #region DeleteObject

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteObject operation. 
        /// 
        /// The DeleteObject operation removes the specified object from Amazon S3.
        /// Once deleted, there is no method to restore or undelete an object. 
        /// Hook on the <see cref="S3ResponseDelgate<IS3Response> S3WebResponse"/> event to get
        /// the response.
        ///
        /// If you delete an object that does not exist, Amazon S3 will return a
        /// success (not an error message).
        /// </summary>
        /// <param name="request">
        /// The DeleteObjectRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        void DeleteObject(DeleteObjectRequest request);

        #endregion

        #region CopyObject

        /// <summary>
        /// Initiates the asynchronous execution of the CopyObject operation. 
        /// 
        /// The copy operation creates a copy of an object that is already stored in Amazon S3.
        /// When copying an object, you can preserve all metadata (default) or specify new metadata.
        /// However, the ACL is not preserved and is set to private for the user making the request.
        /// To override the default ACL setting, specify a new ACL when generating a copy request.
        /// If versioning has been enabled on the source bucket, and you want to copy a specific
        /// version of an object, please use
        /// <see cref="P:Amazon.S3.Model.CopyObjectRequest.SourceVersionId" /> to specify the
        /// version. By default, the most recent version of an object is copied to the
        /// destination bucket. Hook on the <see cref="S3ResponseDelgate<IS3Response> S3WebResponse"/> event to get
        /// the response.
        /// </summary>
        /// <param name="request">
        /// The CopyObjectRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        /// <seealso cref="T:Amazon.S3.Model.S3AccessControlList"/>
        /// <remarks>
        /// If Versioning has been enabled on the target bucket, S3 generates a
        /// unique version ID for the object being copied. This version ID is different
        /// from the version ID of the source object. Additionally, S3 returns the version
        /// ID of the copied object in the x-amz-version-id response header in the response.
        /// If you do not enable Versioning or suspend it on the target bucket, the version ID
        /// S3 generates is always the string literal - "null".
        /// </remarks>
        void CopyObject(CopyObjectRequest request);

        #endregion
    }
}