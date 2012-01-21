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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Util;
using Map = System.Collections.Generic.IDictionary<Amazon.S3.Model.S3QueryParameter, string>;
using Amazon.Runtime;

namespace Amazon.S3
{
    /// <summary>
    /// A delegate for event, which handles the responses from the S3 Service
    /// </summary>
    public delegate void S3ResponseEventHandler<TSender, T>(object sender, T result);

    public class AmazonS3Client : AmazonS3
    {
        public event S3ResponseEventHandler<object, ResponseEventArgs> OnS3Response;
        static Logger LOGGER = new Logger(typeof(AmazonS3Client));
        #region Private Members

        private readonly string awsAccessKeyId;
        private string awsSecretAccessKey;
        private AmazonS3Config config;
        private bool disposed;
        private Type myType;
        private readonly string clearAwsSecretAccessKey;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AWS-Access-Key-Id.
        /// </summary>
        protected internal string AWSAccessKeyId
        {
            get { return awsAccessKeyId; }
        }

        /// <summary>
        /// Gets or sets the AWS-Secret-Access-Key.
        /// </summary>
        protected internal string AWSSecretAccessKey
        {
            get { return awsSecretAccessKey; }
            set { awsSecretAccessKey = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="AmazonS3Config"/>.
        /// </summary>
        protected internal AmazonS3Config Config
        {
            get { return config; }
            set { config = value; }
        }

        /// <summary>
        /// Gets a boolean value indicating if this instance is disposed.
        /// </summary>
        protected internal bool Disposed
        {
            get { return disposed; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        protected internal Type MyType
        {
            get { return myType; }
        }

        /// <summary>
        /// Gets the AWS-Access-Key-Id.
        /// </summary>
        protected internal string ClearAWSSecretAccessKey
        {
            get { return clearAwsSecretAccessKey; }
        }

        #endregion Properties

        #region Dispose Pattern

        /// <summary>
        /// Implements the Dispose pattern for the AmazonS3Client
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
        ~AmazonS3Client()
        {
            this.Dispose(false);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs AmazonS3Client with AWS Access Key ID and AWS Secret Key
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey)
            : this(awsAccessKeyId, awsSecretAccessKey, new AmazonS3Config())
        {
        }

        /// <summary>
        /// Create a client for the Amazon S3 Service with the credentials defined in the App.config.
        /// Example App.config with credentials set. 
        /// <?xml version="1.0" encoding="utf-8" ?>
        /// <configuration>
        ///     <appSettings>
        ///         <add key="AWSAccessKey" value="********************"/>
        ///         <add key="AWSSecretKey" value="****************************************"/>
        ///     </appSettings>
        /// </configuration>
        /// </summary>
        public AmazonS3Client()
            : this(new EnvironmentAWSCredentials(), new AmazonS3Config())
        {
        }

        /// <summary>
        /// Constructs an AmazonS3Client with AWSCredentials.
        /// </summary>
        /// <param name="credentials">Credentials</param>
        public AmazonS3Client(AWSCredentials credentials)
            : this(credentials, new AmazonS3Config())
        {
        }

        private AmazonS3Client(AWSCredentials credentials, AmazonS3Config config)
        {
            this.config = config;
            this.myType = base.GetType();
            this.awsAccessKeyId = credentials.GetCredentials().AccessKey;
            this.clearAwsSecretAccessKey = credentials.GetCredentials().SecretKey;
        }

        /// <summary>
        /// Constructs AmazonS3Client with AWS Access Key ID, AWS Secret Key and an
        /// AmazonS3 Configuration object. If the config object's
        /// UseSecureStringForAwsSecretKey is false, the AWS Secret Key
        /// is stored as a clear-text string. Please use this option only
        /// if the application environment doesn't allow the use of SecureStrings.
        /// </summary>
        /// <param name="awsAccessKeyId">AWS Access Key ID</param>
        /// <param name="awsSecretAccessKey">AWS Secret Access Key</param>
        /// <param name="config">The S3 Configuration Object</param>
        public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, AmazonS3Config config)
        {
            if (!String.IsNullOrEmpty(awsSecretAccessKey))
            {
                clearAwsSecretAccessKey = awsSecretAccessKey;
            }
            this.awsAccessKeyId = awsAccessKeyId;
            this.config = config;
            this.myType = this.GetType();
        }

        #endregion

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
        public string GetPreSignedURL(GetPreSignedUrlRequest request)
        {
            if (String.IsNullOrEmpty(this.awsAccessKeyId))
            {
                throw new AmazonS3Exception("The AWS Access Key ID cannot be NULL or a Zero length string");
            }

            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The PreSignedUrlRequest specified is null!");
            }

            if (!request.IsSetExpires())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The Expires Specified is null!");
            }

            if (request.Verb > HttpVerb.PUT)
            {
                throw new ArgumentException(
                    "An Invalid HttpVerb was specified for the GetPreSignedURL request. Valid - GET, HEAD, PUT",
                    S3Constants.RequestParam
                    );
            }

            ConvertGetPreSignedUrl(request);
            return request.parameters[S3QueryParameter.Url];
        }

        #endregion

        #region ListBuckets

        /// <summary>
        /// The ListBuckets operation returns a list of all of the buckets
        /// owned by the authenticated sender of the request.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        public void ListBuckets()
        {
            ListBuckets(new ListBucketsRequest());
        }

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
        public void ListBuckets(ListBucketsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The ListObjectsRequest is null!");
            }

            ConvertListBuckets(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<ListBucketsResponse>(asyncResult);
        }

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
        public void GetBucketLocation(GetBucketLocationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The GetBucketLocationRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertGetBucketLocation(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<GetBucketLocationResponse>(asyncResult);
        }

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
        public void GetBucketLogging(GetBucketLoggingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The GetBucketLoggingRequest specified is null!");
            }
            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertGetBucketLogging(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<GetBucketLoggingResponse>(asyncResult);
        }

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
        public void EnableBucketLogging(EnableBucketLoggingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The SetBucketLoggingRequest specified is null!");
            }
            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            S3BucketLoggingConfig config = request.LoggingConfig;
            if (config == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The LoggingConfig is null!");
            }

            if (!config.IsSetTargetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName of the LoggingConfig is null or empty!");
            }

            ConvertEnableBucketLogging(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<EnableBucketLoggingResponse>(asyncResult);
        }

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
        public void DisableBucketLogging(DisableBucketLoggingRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The SetBucketLoggingRequest specified is null!");
            }
            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertDisableBucketLogging(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<DisableBucketLoggingResponse>(asyncResult);
        }

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
        public void GetBucketPolicy(GetBucketPolicyRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The GetBucketPolicyRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertGetBucketPolicy(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<GetBucketPolicyResponse>(asyncResult);
        }

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
        public void PutBucketPolicy(PutBucketPolicyRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The PutBucketPolicyRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            if (!request.IsSetPolicy())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The policy specified is null or empty!");
            }

            ConvertPutBucketPolicy(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<PutBucketPolicyResponse>(asyncResult);
        }

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
        public void DeleteBucketPolicy(DeleteBucketPolicyRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The DeleteBucketPolicyRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertDeleteBucketPolicy(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<DeleteBucketPolicyResponse>(asyncResult);
        }

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
        public void ListObjects(ListObjectsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The ListObjectsRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertListObjects(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<ListObjectsResponse>(asyncResult);
        }

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
        public void GetACL(GetACLRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The GetACLRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertGetACL(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<GetACLResponse>(asyncResult);
        }

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
        public void SetACL(SetACLRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The SetACLRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            if (!request.IsSetACL() &&
                !request.IsSetCannedACL())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "No ACL or CannedACL specified!");
            }

            if (request.IsSetACL() && request.ACL.Owner == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "No owner for the ACL specified!");
            }

            ConvertSetACL(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<SetACLResponse>(asyncResult);
        }

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
        public void PutBucket(PutBucketRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The PutBucketRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertPutBucket(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<PutBucketResponse>(asyncResult);
        }

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
        public void DeleteBucket(DeleteBucketRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The DeleteBucketRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            ConvertDeleteBucket(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<DeleteBucketResponse>(asyncResult);
        }

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
        /// <see cref="S3ResponseDelgate<object, IS3Response> S3WebResponse" event./>
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
        public void GetObject(GetObjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The GetObjectRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }
            if (!request.IsSetKey())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The Key Specified is null or empty!");
            }

            ConvertGetObject(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<GetObjectResponse>(asyncResult);
        }

        #endregion

        #region GetObjectMetadata

        /// <summary>
        /// Initiates the asynchronous execution of the GetObjectMetadata operation.
        /// 
        /// The GetObjectMetadata operation is used to retrieve information about a specific object
        /// or object size, without actually fetching the object itself. This is useful if you're
        /// only interested in the object metadata, and don't want to waste bandwidth on the object data.
        /// The response is identical to the GetObject response, except that there is no response body.
        /// Hook on the <see cref="S3ResponseDelgate<object, IS3Response> S3WebResponse"/> event to get
        /// the response.
        /// </summary>
        /// <param name="request">
        /// The GetObjectMetadataRequest that defines the parameters of the operation.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        /// <exception cref="T:System.Net.WebException"></exception>
        /// <exception cref="T:Amazon.S3.AmazonS3Exception"></exception>
        public void GetObjectMetadata(GetObjectMetadataRequest request)
        {
            if (request.IsSetKey())
                invokeGetObjectMetadata(request, true);
            else
                invokeGetObjectMetadata(request, false);
        }

        void invokeGetObjectMetadata(GetObjectMetadataRequest request, bool includeKey)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The GetObjectMetadataRequest specified is null!");
            }

            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The BucketName specified is null or empty!");
            }

            if (includeKey)
            {
                if (!request.IsSetKey())
                {
                    throw new ArgumentNullException(S3Constants.RequestParam, "The Key Specified is null or empty!");
                }
            }

            ConvertGetObjectMetadata(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<GetObjectMetadataResponse>(asyncResult);
        }

        #endregion

        #region PutObject

        /// <summary>
        /// Initiates the asynchronous execution of the PutObject operation.
        /// 
        /// The PutObject operation adds an object to an S3 Bucket.
        /// The response indicates that the object has been successfully stored.
        /// Amazon S3 never stores partial objects: if you receive a successful
        /// response, then you can be confident that the entire object was stored. 
        /// Hook on the <see cref="S3ResponseDelgate<object, IS3Response> S3WebResponse"/> event to get
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
        public void PutObject(PutObjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The PutObjectRequest specified is null!");
            }

            // The BucketName and one of either the Key or the FilePath needs to be set
            if (!request.IsSetBucketName())
            {
                throw new ArgumentException("An S3 Bucket must be specified for S3 PUT object.");
            }

            if (!(request.IsSetKey() || request.IsSetFilePath()))
            {
                throw new ArgumentException(
                    "Either a Key or a Filename need to be specified for S3 PUT object.");
            }

            // Either:
            // 1. A file is being transferred - so a filename or a stream needs to be provided
            // 2. The content body needs to be set
            if (!request.IsSetFilePath() &&
                !request.IsSetInputStream() &&
                !request.IsSetContentBody())
            {
                throw new ArgumentException(
                    "Please specify either a Filename, provide a FileStream or provide a ContentBody to PUT an object into S3.");
            }

            if (request.IsSetInputStream() && request.IsSetContentBody())
            {
                throw new ArgumentException(
                    "Please specify one of either an Input FileStream or the ContentBody to be PUT as an S3 object.");
            }

            if (request.IsSetInputStream() && request.IsSetFilePath())
            {
                throw new ArgumentException(
                    "Please specify one of either an Input FileStream or a Filename to be PUT as an S3 object.");
            }

            if (request.IsSetFilePath() && request.IsSetContentBody())
            {
                throw new ArgumentException(
                    "Please specify one of either a Filename or the ContentBody to be PUT as an S3 object.");
            }

            if (request.IsSetFilePath())
            {
                // Create a stream from the filename specified
                if (File.Exists(request.FilePath))
                {
                    request.InputStream = new FileStream(request.FilePath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    throw new FileNotFoundException("The specified file does not exist");
                }

                if (!request.IsSetKey())
                {
                    string name = request.FilePath;
                    // Set the key to be the name of the file sans directories
                    request.Key = name.Substring(name.LastIndexOf(@"\", StringComparison.OrdinalIgnoreCase) + 1);
                }
            }

            ConvertPutObject(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<PutObjectResponse>(asyncResult);
        }

        #endregion

        #region DeleteObject

        /// <summary>
        /// Initiates the asynchronous execution of the DeleteObject operation. 
        /// 
        /// The DeleteObject operation removes the specified object from Amazon S3.
        /// Once deleted, there is no method to restore or undelete an object. 
        /// Hook on the <see cref="S3ResponseDelgate<object, IS3Response> S3WebResponse"/> event to get
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
        public void DeleteObject(DeleteObjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The DeleteObjectRequest is null!");
            }
            if (!request.IsSetBucketName())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The S3 BucketName specified is null or empty!");
            }
            if (!request.IsSetKey())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The S3 Key Specified is null or empty!");
            }

            // MFA requests must pass over HTTPS. Switch protocol
            // to HTTPS just for this call
            if (request.IsSetMfaCodes())
            {
                config.CommunicationProtocol = Protocol.HTTPS;
            }

            ConvertDeleteObject(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<DeleteObjectResponse>(asyncResult);
        }

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
        /// destination bucket. Hook on the <see cref="S3ResponseDelgate<object, IS3Response> S3WebResponse"/> event to get
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
        public void CopyObject(CopyObjectRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The CopyObjectRequest specified is null!");
            }

            if (!request.IsSetDestinationBucket())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The Destination S3Bucket must be specified!");
            }

            if (!request.IsSetSourceBucket())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The Source S3Bucket must be specified!");
            }

            if (!request.IsSetSourceKey())
            {
                throw new ArgumentNullException(S3Constants.RequestParam, "The Source Key must be specified!");
            }

            ConvertCopyObject(request);
            S3AsyncResult asyncResult = new S3AsyncResult(request, null);
            invoke<CopyObjectResponse>(asyncResult);
        }

        #endregion

        #region Private ConvertXXX Methods

        /**
        * Convert ListBucketsRequest to key/value pairs
        */
        private void ConvertListBuckets(ListBucketsRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "ListBuckets";
            addS3QueryParameters(request, null);
        }

        /**
         * Convert GetBucketLocationRequest to key/value pairs.
         */
        private void ConvertGetBucketLocation(GetBucketLocationRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "GetBucketLocation";
            parameters[S3QueryParameter.Query] = parameters[S3QueryParameter.QueryToSign] = "?location";

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert GetBucketLoggingRequest to key/value pairs.
         */
        private void ConvertGetBucketLogging(GetBucketLoggingRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "GetBucketLogging";
            parameters[S3QueryParameter.Query] = "?logging";
            parameters[S3QueryParameter.QueryToSign] = "?logging";

            addS3QueryParameters(request, request.BucketName);

        }

        /**
         * Convert SetBucketLoggingRequest for enable logging, to key/value pairs.
         */
        private void ConvertEnableBucketLogging(EnableBucketLoggingRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.ContentBody] = request.LoggingConfig.ToString();
            parameters[S3QueryParameter.ContentType] = AWSSDKUtils.UrlEncodedContent;
            parameters[S3QueryParameter.Verb] = S3Constants.PutVerb;
            parameters[S3QueryParameter.Action] = "SetBucketLogging";
            parameters[S3QueryParameter.Query] = "?logging";
            parameters[S3QueryParameter.QueryToSign] = "?logging";

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert DisableBucketLoggingRequest for disable logging, to key/value pairs.
         */
        private void ConvertDisableBucketLogging(DisableBucketLoggingRequest request)
        {
            ConvertEnableBucketLogging(request);
        }

        /**
         * Convert ListObjectsRequest to key/value pairs
         */
        private void ConvertListObjects(ListObjectsRequest request)
        {
            Map parameters = request.parameters;

            //Create query string if any of the values are set.
            StringBuilder sb = new StringBuilder("?", 256);
            if (request.IsSetPrefix())
            {
                sb.Append(String.Concat("prefix=", AmazonS3Util.UrlEncode(request.Prefix, false), "&"));
            }
            if (request.IsSetMarker())
            {
                sb.Append(String.Concat("marker=", AmazonS3Util.UrlEncode(request.Marker, false), "&"));
            }
            if (request.IsSetDelimiter())
            {
                sb.Append(String.Concat("delimiter=", AmazonS3Util.UrlEncode(request.Delimiter, false), "&"));
            }
            if (request.IsSetMaxKeys())
            {
                sb.Append(String.Concat("max-keys=", request.MaxKeys, "&"));
            }

            string query = sb.ToString();

            // Remove trailing & character
            if (query.EndsWith("&", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Remove(query.Length - 1);
            }

            // We initialized the query with a "?". If none of
            // Prefix, Marker, Delimiter, MaxKeys is set, there
            // is no query
            if (query.Length > 1)
            {
                parameters[S3QueryParameter.Query] = query;
            }

            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "ListObjects";
            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert GetAclRequest to key/value pairs.
         */
        private void ConvertGetACL(GetACLRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "GetACL";

            string queryStr = "?acl";

            if (request.IsSetKey())
            {
                parameters[S3QueryParameter.Key] = request.Key;

                // The queryStr needs to be changed from its default value only
                // if a version-id is specified
                if (request.IsSetVersionId())
                {
                    queryStr = String.Concat(queryStr, "&versionId=", request.VersionId);
                }
            }

            parameters[S3QueryParameter.Query] = queryStr;
            parameters[S3QueryParameter.QueryToSign] = queryStr;

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         *  Convert SetACLRequest to key/value pairs.
         */
        private void ConvertSetACL(SetACLRequest request)
        {
            Map parameters = request.parameters;
            WebHeaderCollection webHeaders = request.Headers;

            if (request.IsSetACL())
            {
                parameters[S3QueryParameter.ContentBody] = request.ACL.ToString();
                parameters[S3QueryParameter.ContentType] = AWSSDKUtils.UrlEncodedContent;
            }

            if (request.IsSetCannedACL())
            {
                setCannedACLHeader(webHeaders, request.CannedACL);
            }

            parameters[S3QueryParameter.Verb] = S3Constants.PutVerb;
            parameters[S3QueryParameter.Action] = "SetACL";

            string queryStr = "?acl";

            if (request.IsSetKey())
            {
                parameters[S3QueryParameter.Key] = request.Key;

                // The queryStr needs to be changed from its default value only
                // if a version-id is specified
                if (request.IsSetVersionId())
                {
                    queryStr = String.Concat(queryStr, "&versionId=", request.VersionId);
                }
            }

            parameters[S3QueryParameter.Query] = queryStr;
            parameters[S3QueryParameter.QueryToSign] = queryStr;

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Converts the PutBucketRequest to key/value pairs
         */
        private void ConvertPutBucket(PutBucketRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.PutVerb;
            parameters[S3QueryParameter.Action] = "PutBucket";

            string regionCode;
            if (!string.IsNullOrEmpty(request.BucketRegionName))
            {
                if (string.Equals(request.BucketRegionName, S3Constants.REGION_EU_WEST_1))
                {
                    regionCode = S3Constants.LocationConstraints[(int)S3Region.EU];
                }
                else
                {
                    regionCode = request.BucketRegionName;
                }
            }
            else
            {
                regionCode = S3Constants.LocationConstraints[(int)request.BucketRegion];
            }

            if (!string.IsNullOrEmpty(regionCode) && !string.Equals(request.BucketRegionName, S3Constants.REGION_US_EAST_1))
            {
                string content = String.Format(CultureInfo.InvariantCulture,
                    "<CreateBucketConstraint><LocationConstraint>{0}</LocationConstraint></CreateBucketConstraint>",
                    regionCode
                    );
                parameters[S3QueryParameter.ContentBody] = content;
                parameters[S3QueryParameter.ContentType] = AWSSDKUtils.UrlEncodedContent;
            }


            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert DeleteBucketRequest to key/value pairs
         */
        private void ConvertDeleteBucket(DeleteBucketRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.DeleteVerb;
            parameters[S3QueryParameter.Action] = "DeleteBucket";
            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert GetBucketPolicyRequest to key/value pairs.
         */
        private void ConvertGetBucketPolicy(GetBucketPolicyRequest request)
        {
            Map parameters = request.parameters;
            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "GetBucketPolicy";
            parameters[S3QueryParameter.Query] = parameters[S3QueryParameter.QueryToSign] = "?policy";

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert PutBucketPolicyRequest to key/value pairs.
         */
        private void ConvertPutBucketPolicy(PutBucketPolicyRequest request)
        {
            Map parameters = request.parameters;
            parameters[S3QueryParameter.Verb] = S3Constants.PutVerb;
            parameters[S3QueryParameter.Action] = "PutBucketPolicy";
            parameters[S3QueryParameter.Query] = parameters[S3QueryParameter.QueryToSign] = "?policy";

            parameters[S3QueryParameter.ContentBody] = request.Policy;
            parameters[S3QueryParameter.ContentType] = AWSSDKUtils.UrlEncodedContent;

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert DeleteBucketPolicyRequest to key/value pairs.
         */
        private void ConvertDeleteBucketPolicy(DeleteBucketPolicyRequest request)
        {
            Map parameters = request.parameters;
            parameters[S3QueryParameter.Verb] = S3Constants.DeleteVerb;
            parameters[S3QueryParameter.Action] = "DeleteBucketPolicy";
            parameters[S3QueryParameter.Query] = parameters[S3QueryParameter.QueryToSign] = "?policy";

            addS3QueryParameters(request, request.BucketName);
        }

        /**
          * Convert GetObjectRequest to key/value pairs.
          */
        private void ConvertGetObject(GetObjectRequest request)
        {
            Map parameters = request.parameters;
            WebHeaderCollection webHeaders = request.Headers;

            parameters[S3QueryParameter.Verb] = S3Constants.GetVerb;
            parameters[S3QueryParameter.Action] = "GetObject";
            parameters[S3QueryParameter.Key] = request.Key;

            if (request.IsSetByteRange())
            {
                parameters[S3QueryParameter.Range] = String.Concat(
                    request.ByteRangeLong.First,
                    ":",
                    request.ByteRangeLong.Second
                    );
            }

            // Add the necessary get object specific headers to the request.Headers object
            if (request.IsSetETagToMatch())
            {
                setIfMatchHeader(webHeaders, request.ETagToMatch);
            }
            if (request.IsSetETagToNotMatch())
            {
                setIfNoneMatchHeader(webHeaders, request.ETagToNotMatch);
            }
            if (request.IsSetModifiedSinceDate())
            {
                setIfModifiedSinceHeader(webHeaders, request.ModifiedSinceDate);
            }
            if (request.IsSetUnmodifiedSinceDate())
            {
                setIfUnmodifiedSinceHeader(webHeaders, request.UnmodifiedSinceDate);
            }

            StringBuilder queryStr = new StringBuilder();

            addParameter(queryStr, "versionId", request.VersionId);
            addParameter(queryStr, ResponseHeaderOverrides.RESPONSE_CACHE_CONTROL, request.ResponseHeaderOverrides.CacheControl);
            addParameter(queryStr, ResponseHeaderOverrides.RESPONSE_CONTENT_DISPOSITION, request.ResponseHeaderOverrides.ContentDisposition);
            addParameter(queryStr, ResponseHeaderOverrides.RESPONSE_CONTENT_ENCODING, request.ResponseHeaderOverrides.ContentEncoding);
            addParameter(queryStr, ResponseHeaderOverrides.RESPONSE_CONTENT_LANGUAGE, request.ResponseHeaderOverrides.ContentLanguage);
            addParameter(queryStr, ResponseHeaderOverrides.RESPONSE_CONTENT_TYPE, request.ResponseHeaderOverrides.ContentType);
            addParameter(queryStr, ResponseHeaderOverrides.RESPONSE_EXPIRES, request.ResponseHeaderOverrides.Expires);


            if (queryStr.Length > 0)
            {
                parameters[S3QueryParameter.Query] = "?" + queryStr.ToString();
                parameters[S3QueryParameter.QueryToSign] = parameters[S3QueryParameter.Query];
            }

            // Add the Timeout parameter
            parameters[S3QueryParameter.RequestTimeout] = request.Timeout.ToString(CultureInfo.InvariantCulture);

            addS3QueryParameters(request, request.BucketName);
        }

        static void addParameter(StringBuilder queryStr, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (queryStr.Length > 0)
                    queryStr.Append("&");

                queryStr.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}", key, value);
            }
        }

        /**
         * Convert GetObjectMetadataRequest to key/value pairs.
         */
        private void ConvertGetObjectMetadata(GetObjectMetadataRequest request)
        {
            Map parameters = request.parameters;
            WebHeaderCollection webHeaders = request.Headers;

            parameters[S3QueryParameter.Verb] = S3Constants.HeadVerb;
            parameters[S3QueryParameter.Action] = "GetObjectMetadata";
            parameters[S3QueryParameter.Key] = request.Key;

            if (request.IsSetETagToNotMatch())
            {
                setIfNoneMatchHeader(webHeaders, request.ETagToNotMatch);
            }
            if (request.IsSetModifiedSinceDate())
            {
                setIfModifiedSinceHeader(webHeaders, request.ModifiedSinceDate);
            }
            if (request.IsSetUnmodifiedSinceDate())
            {
                setIfUnmodifiedSinceHeader(webHeaders, request.UnmodifiedSinceDate);
            }
            if (request.IsSetVersionId())
            {
                string queryStr = String.Concat("?versionId=", request.VersionId);
                parameters[S3QueryParameter.Query] = queryStr;
                parameters[S3QueryParameter.QueryToSign] = queryStr;
            }

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert PutObjectRequest to key/value pairs.
         */
        protected internal void ConvertPutObject(PutObjectRequest request)
        {
            Map parameters = request.parameters;
            WebHeaderCollection webHeaders = request.Headers;

            parameters[S3QueryParameter.Verb] = S3Constants.PutVerb;
            parameters[S3QueryParameter.Action] = "PutObject";
            parameters[S3QueryParameter.Key] = request.Key;

            // Add the Content Type
            if (request.IsSetContentType())
            {
                parameters[S3QueryParameter.ContentType] = request.ContentType;
            }
            else if (request.IsSetFilePath() ||
                request.IsSetKey())
            {
                // Get the extension of the file from the path.
                // Try the key as well.
                string ext = Path.GetExtension(request.FilePath);
                if (String.IsNullOrEmpty(ext) &&
                    request.IsSetKey())
                {
                    ext = Path.GetExtension(request.Key);
                }
                // Use the extension to get the mime-type
                if (!String.IsNullOrEmpty(ext))
                {
                    parameters[S3QueryParameter.ContentType] = AmazonS3Util.MimeTypeFromExtension(ext);
                }
            }

            // Set the Content Length based on whether there is a stream
            if (request.IsSetInputStream())
            {
                parameters[S3QueryParameter.ContentLength] = request.InputStream.Length.ToString(CultureInfo.InvariantCulture);
            }

            if (request.IsSetContentBody())
            {
                // The content length is determined based on the number of bytes
                // needed to represent the content string - check invoke<T>
                parameters[S3QueryParameter.ContentBody] = request.ContentBody;
                // Since a content body was set, let's determine whether a content type was set
                if (!parameters.ContainsKey(S3QueryParameter.ContentType))
                {
                    parameters[S3QueryParameter.ContentType] = AWSSDKUtils.UrlEncodedContent;
                }
            }

            // Add the Timeout parameter
            parameters[S3QueryParameter.RequestTimeout] = request.Timeout.ToString(CultureInfo.InvariantCulture);

            // Add the Put Object specific headers to the request
            // 1. The Canned ACL
            if (request.IsSetCannedACL())
            {
                setCannedACLHeader(webHeaders, request.CannedACL);
            }

            // 2. The MetaData
            if (request.IsSetMetaData())
            {
                // Add headers of type x-amz-meta-<key> to the request
                foreach (string key in request.metaData.Keys)
                {
                    string prefixedKey;
                    if (!key.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
                    {
                        prefixedKey = String.Concat("x-amz-meta-", key);
                    }
                    else
                    {
                        prefixedKey = key;
                    }

                    webHeaders[prefixedKey] = request.metaData[key];
                }
            }

            // Add the storage class header
            webHeaders[S3Constants.AmzStorageClassHeader] = S3Constants.StorageClasses[(int)request.StorageClass];

            // Finally, add the S3 specific parameters and headers
            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert GetPreSignedUrlRequest to key/value pairs.
         */
        private void ConvertGetPreSignedUrl(GetPreSignedUrlRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.Verbs[(int)request.Verb];
            parameters[S3QueryParameter.Action] = "GetPreSignedUrl";
            StringBuilder queryStr = new StringBuilder("?AWSAccessKeyId=", 512);
            queryStr.Append(this.awsAccessKeyId);

            if (request.IsSetKey())
            {
                parameters[S3QueryParameter.Key] = request.Key;
            }
            else if (request.Verb == HttpVerb.HEAD)
            {
                queryStr.Append("&max-keys=0");
            }

            if (request.IsSetContentType())
            {
                parameters[S3QueryParameter.ContentType] = request.ContentType;
            }

            if (queryStr.Length != 0)
            {
                queryStr.Append("&");
            }
            queryStr.Append("Expires=");

            string value = Convert.ToInt64((request.Expires.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture);
            queryStr.Append(value);
            parameters[S3QueryParameter.Expires] = value;

            StringBuilder queryStrToSign = new StringBuilder();
            if (request.IsSetKey() &&
                request.IsSetVersionId() &&
                request.Verb < HttpVerb.PUT)
            {
                queryStrToSign.AppendFormat(CultureInfo.InvariantCulture, "versionId={0}", request.VersionId);
            }

            addParameter(queryStrToSign, ResponseHeaderOverrides.RESPONSE_CACHE_CONTROL, request.ResponseHeaderOverrides.CacheControl);
            addParameter(queryStrToSign, ResponseHeaderOverrides.RESPONSE_CONTENT_DISPOSITION, request.ResponseHeaderOverrides.ContentDisposition);
            addParameter(queryStrToSign, ResponseHeaderOverrides.RESPONSE_CONTENT_ENCODING, request.ResponseHeaderOverrides.ContentEncoding);
            addParameter(queryStrToSign, ResponseHeaderOverrides.RESPONSE_CONTENT_LANGUAGE, request.ResponseHeaderOverrides.ContentLanguage);
            addParameter(queryStrToSign, ResponseHeaderOverrides.RESPONSE_CONTENT_TYPE, request.ResponseHeaderOverrides.ContentType);
            addParameter(queryStrToSign, ResponseHeaderOverrides.RESPONSE_EXPIRES, request.ResponseHeaderOverrides.Expires);


            if (queryStrToSign.Length > 0)
            {
                parameters[S3QueryParameter.QueryToSign] = "?" + queryStrToSign.ToString();
                queryStr.Append("&" + queryStrToSign.ToString());
            }

            parameters[S3QueryParameter.Query] = queryStr.ToString();
            addS3QueryParameters(request, request.BucketName);

            // the url needs to be modified so that:
            // 1. The right http protocol is used
            // 2. The auth string is added to the url
            string url = request.parameters[S3QueryParameter.Url];

            // the url's protocol prefix is generated using the config's
            // CommunicationProtocol property. If the request's
            // protocol differs from that set in the config, make the
            // necessary string replacements.
            if (request.Protocol != config.CommunicationProtocol)
            {
                switch (config.CommunicationProtocol)
                {
                    case Protocol.HTTP:
                        url = url.Replace("http://", "https://");
                        break;
                    case Protocol.HTTPS:
                        url = url.Replace("https://", "http://");
                        break;
                }
            }

            parameters[S3QueryParameter.Url] = String.Concat(
                url,
                "&Signature=",
                AmazonS3Util.UrlEncode(request.parameters[S3QueryParameter.Authorization], false)
                );
        }

        /**
         * Convert DeleteObjectRequest to key/value pairs.
         */
        private void ConvertDeleteObject(DeleteObjectRequest request)
        {
            Map parameters = request.parameters;

            parameters[S3QueryParameter.Verb] = S3Constants.DeleteVerb;
            parameters[S3QueryParameter.Action] = "DeleteObject";
            parameters[S3QueryParameter.Key] = request.Key;
            if (request.IsSetVersionId())
            {
                string queryStr = String.Concat("?versionId=", request.VersionId);
                parameters[S3QueryParameter.Query] = queryStr;
                parameters[S3QueryParameter.QueryToSign] = queryStr;
            }

            if (request.IsSetMfaCodes())
            {
                setMfaHeader(request.Headers, request.MfaCodes);
            }

            addS3QueryParameters(request, request.BucketName);
        }

        /**
         * Convert CopyObjectRequest to key/value pairs.
         */
        private void ConvertCopyObject(CopyObjectRequest request)
        {
            Map parameters = request.parameters;
            WebHeaderCollection webHeaders = request.Headers;

            parameters[S3QueryParameter.Verb] = S3Constants.PutVerb;
            parameters[S3QueryParameter.Action] = "CopyObject";

            // the name of the new key created in the destination bucket is the
            // DestinationKey parameter unless it isn't specified, in which case,
            // use the SourceKey.
            if (request.IsSetDestinationKey())
            {
                parameters[S3QueryParameter.Key] = request.DestinationKey;
            }
            else
            {
                parameters[S3QueryParameter.Key] = request.SourceKey;
            }

            // Add the Timeout parameter
            parameters[S3QueryParameter.RequestTimeout] = request.Timeout.ToString(CultureInfo.InvariantCulture);

            // Add the Copy Object specific headers to the request
            if (request.IsSetETagToMatch())
            {
                setIfMatchCopyHeader(webHeaders, request.ETagToMatch);
            }
            if (request.IsSetETagToNotMatch())
            {
                setIfNoneMatchCopyHeader(webHeaders, request.ETagToNotMatch);
            }
            if (request.IsSetModifiedSinceDate())
            {
                setIfModifiedSinceCopyHeader(webHeaders, request.ModifiedSinceDate);
            }
            if (request.IsSetUnmodifiedSinceDate())
            {
                setIfUnmodifiedSinceCopyHeader(webHeaders, request.UnmodifiedSinceDate);
            }

            // Add the Copy Source header which makes this a COPY request
            string sourceKey = request.SourceKey;
            if (request.IsSetSourceVersionId())
            {
                sourceKey = String.Concat(
                    sourceKey,
                    "?versionId=",
                    request.SourceVersionId
                    );
            }
            setCopySourceHeader(webHeaders, request.SourceBucket, sourceKey);

            // there is always a directive associated with the request
            setMetadataDirectiveHeader(webHeaders, request.Directive);

            // if the user has specified the REPLACE directive
            // and specified new metadata for the copied object
            // specify the metadata using the x-amz-meta header.
            // also, pass the content type header.
            if (request.Directive == S3MetadataDirective.REPLACE)
            {
                if (request.IsSetMetaData())
                {
                    // Add headers of type x-amz-meta-<key> to the request
                    foreach (string key in request.metaData.Keys)
                    {
                        string prefixedKey;
                        if (!key.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
                        {
                            prefixedKey = String.Concat("x-amz-meta-", key);
                        }
                        else
                        {
                            prefixedKey = key;
                        }

                        webHeaders[prefixedKey] = request.metaData[key];
                    }
                }

                // Add the Content Type, if it is specified
                // or determine the content type from the extension
                if (request.IsSetContentType())
                {
                    parameters[S3QueryParameter.ContentType] = request.ContentType;
                }
                else if (request.IsSetDestinationKey())
                {
                    // Get the extension of the file from the destination key.
                    // Try the key as well.
                    string ext = Path.GetExtension(request.DestinationKey);
                    if (String.IsNullOrEmpty(ext))
                    {
                        ext = Path.GetExtension(request.SourceKey);
                    }
                    // Use the extension to get the mime-type
                    if (!String.IsNullOrEmpty(ext))
                    {
                        parameters[S3QueryParameter.ContentType] = AmazonS3Util.MimeTypeFromExtension(ext);
                    }
                }
            }
            addS3QueryParameters(request, request.DestinationBucket);
        }

        #endregion

        #region Private Methods

        void invoke<T>(S3AsyncResult s3AsyncResult) where T : S3Response, new()
        {
            if (s3AsyncResult.S3Request == null)
            {
                throw new AmazonS3Exception("No request specified for the S3 operation!");
            }

            Map parameters = s3AsyncResult.S3Request.parameters;
            Stream fStream = s3AsyncResult.S3Request.InputStream;

            string actionName = parameters[S3QueryParameter.Action];
            string verb = parameters[S3QueryParameter.Verb];

            LOGGER.DebugFormat("Starting request (id {0}) for {0}", s3AsyncResult.S3Request.Id, actionName);

            // Variables that pertain to PUT requests
            byte[] requestData = Encoding.UTF8.GetBytes("");
            long reqDataLen = 0;

            if (String.IsNullOrEmpty(this.awsAccessKeyId))
            {
                throw new AmazonS3Exception("The AWS Access Key ID cannot be NULL or a Zero length string");
            }

            validateVerb(verb);

            if (verb.Equals(S3Constants.PutVerb) || verb.Equals(S3Constants.PostVerb))
            {
                if (parameters.ContainsKey(S3QueryParameter.ContentBody))
                {
                    string reqBody = parameters[S3QueryParameter.ContentBody];
                    s3AsyncResult.S3Request.BytesProcessed = reqBody.Length;
                    LOGGER.DebugFormat("Request (id {0}) body's length [{1}]", s3AsyncResult.S3Request.Id, reqBody.Length);
                    requestData = Encoding.UTF8.GetBytes(reqBody);

                    // Since there is a request body, determine the length of the
                    // data that will be sent to the server.
                    reqDataLen = requestData.Length;
                    parameters[S3QueryParameter.ContentLength] = reqDataLen.ToString(CultureInfo.InvariantCulture);
                }

                if (parameters.ContainsKey(S3QueryParameter.ContentLength))
                {
                    reqDataLen = Int64.Parse(parameters[S3QueryParameter.ContentLength], CultureInfo.InvariantCulture);
                }
            }

            if (fStream != null)
            {
                s3AsyncResult.OrignalStreamPosition = fStream.Position;
            }

            HttpWebRequest request = ConfigureWebRequest(s3AsyncResult.S3Request);

            parameters[S3QueryParameter.RequestAddress] = request.RequestUri.ToString();

            try
            {
                s3AsyncResult.RequestState = new RequestState(request, fStream, requestData, reqDataLen);

                if (reqDataLen > 0)
                {
                    request.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCallback<T>), s3AsyncResult);
                }
                else
                {
                    request.BeginGetResponse(new AsyncCallback(this.GetResponseCallback<T>), s3AsyncResult);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Error starting async http operation", e);
                throw;
            }
        }

        static void validateVerb(string verb)
        {
            // The HTTP operation specified has to be one of the operations
            // the Amazon S3 service explicitly supports
            if (!(verb.Equals(S3Constants.PutVerb) ||
                verb.Equals(S3Constants.GetVerb) ||
                verb.Equals(S3Constants.DeleteVerb) ||
                verb.Equals(S3Constants.HeadVerb) ||
                verb.Equals(S3Constants.PostVerb)))
            {
                throw new AmazonS3Exception("Invalid HTTP Operation attempted! Supported operations - GET, HEAD, PUT, DELETE, POST");
            }
        }

        [method: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void GetRequestStreamCallback<T>(IAsyncResult result) where T : S3Response, new()
        {
            S3AsyncResult s3AsyncResult = result as S3AsyncResult;

            if (null == s3AsyncResult)
                s3AsyncResult = result.AsyncState as S3AsyncResult;

            try
            {
                RequestState state = s3AsyncResult.RequestState;
                bool shouldRetry = false;
                try
                {
                    Stream requestStream;
                    requestStream = state.WebRequest.EndGetRequestStream(result);

                    using (requestStream)
                    {
                        Stream stream = state.InputStream != null ? state.InputStream : new MemoryStream(state.RequestData);
                        writeStreamToService(s3AsyncResult.S3Request, state.RequestDataLength, stream, requestStream);
                    }
                }
                catch (IOException e)
                {
                    shouldRetry = handleIOException(s3AsyncResult.S3Request, s3AsyncResult.RequestState.WebRequest, null, e, s3AsyncResult.RetriesAttempt);
                }

                if (shouldRetry)
                {
                    s3AsyncResult.RetriesAttempt++;
                    handleRetry(s3AsyncResult.S3Request, s3AsyncResult.RequestState.WebRequest, null, s3AsyncResult.OrignalStreamPosition,
                        s3AsyncResult.RetriesAttempt, HttpStatusCode.OK, null);
                    invoke<T>(s3AsyncResult);
                }
                else
                {
                    state.WebRequest.BeginGetResponse(new AsyncCallback(this.GetResponseCallback<T>), s3AsyncResult);
                }
            }
            catch (Exception e)
            {
                s3AsyncResult.RequestState.WebRequest.Abort();
                LOGGER.Error("Error for GetRequestStream", e);
                s3AsyncResult.Exception = e;
            }
        }

        [method: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void GetResponseCallback<T>(IAsyncResult result) where T : S3Response, new()
        {
            S3AsyncResult s3AsyncResult = result as S3AsyncResult;
            if (null == s3AsyncResult)
                s3AsyncResult = result.AsyncState as S3AsyncResult;

            bool shouldRetry = false;
            try
            {
                Exception cause = null;
                HttpStatusCode statusCode = HttpStatusCode.OK;
                RequestState state = s3AsyncResult.RequestState;
                HttpWebResponse httpResponse = null;
                T response = null;
                try
                {
                    httpResponse = state.WebRequest.EndGetResponse(result) as HttpWebResponse;
                    TimeSpan lengthOfRequest = DateTime.Now - state.WebRequestStart;
                    s3AsyncResult.S3Request.ResponseTime = lengthOfRequest;

                    shouldRetry = handleHttpResponse<T>(
                        s3AsyncResult.S3Request,
                        state.WebRequest,
                        httpResponse,
                        s3AsyncResult.RetriesAttempt,
                        lengthOfRequest,
                        out response, out cause, out statusCode);
                    if (!shouldRetry)
                    {
                        s3AsyncResult.FinalResponse = response;
                    }
                }
                catch (WebException we)
                {
                    shouldRetry = handleHttpWebErrorResponse(s3AsyncResult.S3Request, we, s3AsyncResult.RequestState.WebRequest, httpResponse, out cause, out statusCode);
                }
                catch (IOException e)
                {
                    shouldRetry = handleIOException(s3AsyncResult.S3Request, s3AsyncResult.RequestState.WebRequest, httpResponse, e, s3AsyncResult.RetriesAttempt);
                }

                if (shouldRetry)
                {
                    s3AsyncResult.RetriesAttempt++;
                    WebHeaderCollection respHeaders = null;
                    if (response != null)
                    {
                        respHeaders = response.Headers;
                    }

                    handleRetry(s3AsyncResult.S3Request, s3AsyncResult.RequestState.WebRequest, respHeaders, s3AsyncResult.OrignalStreamPosition,
                        s3AsyncResult.RetriesAttempt, statusCode, cause);
                    invoke<T>(s3AsyncResult);
                }
                else if (cause != null)
                {
                    s3AsyncResult.Exception = cause;
                }
            }
            catch (Exception e)
            {
                AmazonS3Exception amazonExcepion = e as AmazonS3Exception;
                if (null != amazonExcepion)
                {
                    s3AsyncResult.FinalResponse = amazonExcepion;
                    return;
                }
                else
                {
                    LOGGER.Error("Error for GetResponse", e);
                    s3AsyncResult.Exception = e;
                    shouldRetry = false;
                }
            }
            finally
            {
                if (!shouldRetry)
                {
                    //Invoke the event with response
                    if (null != OnS3Response)
                        OnS3Response.Invoke(this, new ResponseEventArgs(s3AsyncResult.FinalResponse));
                }
            }
        }

        /**
         * Add authentication related and version parameters
         */
        void addS3QueryParameters(S3Request request, string destinationBucket)
        {
            if (request == null)
            {
                return;
            }

            Map parameters = request.parameters;
            WebHeaderCollection webHeaders = request.Headers;

            if (webHeaders != null)
            {
                webHeaders[S3Constants.AmzDateHeader] = AmazonS3Util.FormattedCurrentTimestamp;
            }

            StringBuilder canonicalResource = new StringBuilder("/", 512);
            if (!String.IsNullOrEmpty(destinationBucket))
            {
                parameters[S3QueryParameter.DestinationBucket] = destinationBucket;
                if (AmazonS3Util.ValidateV2Bucket(destinationBucket))
                {
                    parameters[S3QueryParameter.BucketVersion] = S3Constants.BucketVersions[2];
                }
                else
                {
                    parameters[S3QueryParameter.BucketVersion] = S3Constants.BucketVersions[1];
                }
                canonicalResource.Append(destinationBucket);
                if (!destinationBucket.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    canonicalResource.Append("/");
                }
            }
            else
            {
                // If there is no destination bucket specified, just use V2.
                parameters[S3QueryParameter.BucketVersion] = S3Constants.BucketVersions[2];
            }

            // The canonical resource doesn't need the query because it is added
            // in the configureWebRequest function directly to the URL
            if (parameters.ContainsKey(S3QueryParameter.Key))
            {
                canonicalResource.Append(parameters[S3QueryParameter.Key]);
            }

            parameters[S3QueryParameter.CanonicalizedResource] = canonicalResource.ToString();

            // Has the user added the Content-Type header to the request?
            string value = webHeaders[AWSSDKUtils.ContentTypeHeader];
            if (!String.IsNullOrEmpty(value))
            {
                // Remove the header from the webHeaders collection
                // and add it to the parameters
                parameters[S3QueryParameter.ContentType] = value;
            }

            string toSign = BuildSigningString(parameters, webHeaders);
            string auth;

            KeyedHashAlgorithm algorithm = new HMACSHA1();
            auth = AWSSDKUtils.HMACSign(
                toSign,
                clearAwsSecretAccessKey,
                algorithm
                );
            parameters[S3QueryParameter.Authorization] = auth;

            // Insert the S3 Url into the parameters
            addUrlToParameters(request, config);
        }

        void writeStreamToService(S3Request request, long reqDataLen, Stream inputStream, Stream requestStream)
        {
            if (inputStream != null)
            {
                long current = 0;
                // Reset the file stream's position to the starting point
                inputStream.Position = 0;
                byte[] buffer = new byte[this.config.BufferSize];
                int bytesRead = 0;
                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    current += bytesRead;
                    requestStream.Write(buffer, 0, bytesRead);
                    if (request != null)
                    {
                        request.OnRaiseProgressEvent(bytesRead, current, reqDataLen);
                    }
                }
            }
        }

        void handleRetry(S3Request userRequest, HttpWebRequest request, WebHeaderCollection respHdrs, long orignalStreamPosition, int retries, HttpStatusCode statusCode, Exception cause)
        {
            string actionName = userRequest.parameters[S3QueryParameter.Action];
            string requestAddr = request.RequestUri.ToString();

            if (retries <= this.config.MaxErrorRetry)
            {
                LOGGER.InfoFormat("Retry number {0} for request {1}.", retries, actionName);
            }
            pauseOnRetry(retries, this.config.MaxErrorRetry, statusCode, requestAddr, respHdrs, cause);
            // Reset the request so that streams are recreated,
            // removed headers are added back, etc
            prepareRequestForRetry(userRequest, orignalStreamPosition);
        }

        bool handleIOException(S3Request userRequest, HttpWebRequest request, HttpWebResponse httpResponse, IOException e, int retries)
        {
            if (isInnerExceptionThreadAbort(e))
                throw e;

            string actionName = userRequest.parameters[S3QueryParameter.Action];
            LOGGER.Error(string.Format(CultureInfo.InvariantCulture, "Error making request {0}.", actionName), e);
            if (httpResponse != null)
            {
                httpResponse.Close();
                httpResponse = null;
            }
            // Abort the unsuccessful request
            request.Abort();

            return retries <= this.config.MaxErrorRetry;
        }

        bool isInnerExceptionThreadAbort(Exception e)
        {
            if (e.InnerException is ThreadAbortException)
                return true;
            if (e.InnerException != null)
                return isInnerExceptionThreadAbort(e.InnerException);
            return false;
        }

        static bool handleHttpWebErrorResponse(S3Request userRequest, WebException we, HttpWebRequest request, HttpWebResponse httpResponse, out Exception cause, out HttpStatusCode statusCode)
        {
            WebHeaderCollection respHdrs;
            string actionName = userRequest.parameters[S3QueryParameter.Action];
            string requestAddr = request.RequestUri.ToString();

            LOGGER.Debug(string.Format(CultureInfo.InvariantCulture, "Error making request {0}.", actionName), we);

            bool shouldRetry;
            using (HttpWebResponse errorResponse = we.Response as HttpWebResponse)
            {
                shouldRetry = processRequestError(actionName, request, we, errorResponse, requestAddr, out respHdrs, out cause);

                if (httpResponse != null)
                {
                    httpResponse.Close();
                    httpResponse = null;
                }
                // Abort the unsuccessful request regardless of whether we should
                // or shouldn't retry.
                request.Abort();

                if (errorResponse != null)
                {
                    statusCode = errorResponse.StatusCode;
                }
                else
                {
                    statusCode = HttpStatusCode.BadRequest;
                }
            }

            return shouldRetry;
        }

        bool handleHttpResponse<T>(S3Request userRequest, HttpWebRequest request, HttpWebResponse httpResponse,
            int retries,
            TimeSpan lengthOfRequest, out T response, out Exception cause, out HttpStatusCode statusCode)
            where T : S3Response, new()
        {
            response = null;
            cause = null;
            statusCode = httpResponse.StatusCode;
            Map parameters = userRequest.parameters;
            string actionName = parameters[S3QueryParameter.Action];
            string requestAddr = request.RequestUri.ToString();

            bool shouldRetry;
            LOGGER.InfoFormat("Received response for {0} (id {1}) with status code {2} in {3}.", actionName, userRequest.Id, httpResponse.StatusCode, lengthOfRequest);

            statusCode = httpResponse.StatusCode;
            if (!isRedirect(httpResponse))
            {
                // The request submission has completed. Retrieve the response.
                shouldRetry = processRequestResponse<T>(httpResponse, userRequest, out response, out cause);
            }
            else
            {
                shouldRetry = true;

                processRedirect(userRequest, httpResponse);
                LOGGER.InfoFormat("Request for {0} is being redirect to {1}.", actionName, userRequest.parameters[S3QueryParameter.Url]);

                pauseOnRetry(retries + 1, this.config.MaxErrorRetry, statusCode, requestAddr, httpResponse.Headers, cause);

                // The HTTPResponse object needs to be closed. Once this is done, the request
                // is gracefully terminated. Mind you, if this response object is not closed,
                // the client will start getting timeout errors.
                // P.S. This sequence of close-response followed by abort-request
                // will be repeated through the exception handlers for this try block
                httpResponse.Close();
                httpResponse = null;
                request.Abort();
            }

            return shouldRetry;
        }

        static void processRedirect(S3Request userRequest, HttpWebResponse httpResponse)
        {
            if (httpResponse == null)
            {
                throw new WebException(
                    "The Web Response for a redirected request is null!",
                    WebExceptionStatus.ProtocolError
                    );
            }

            // This is a redirect. Get the URL from the location header
            WebHeaderCollection respHeaders = httpResponse.Headers;
            string value;
            if (!String.IsNullOrEmpty(value = respHeaders["Location"]))
            {
                // This should be the new location for the request
                userRequest.parameters[S3QueryParameter.Url] = value;
            }
        }

        static bool isRedirect(HttpWebResponse httpResponse)
        {
            if (httpResponse == null)
            {
                throw new ArgumentNullException("httpResponse", "Input parameter is null");
            }

            HttpStatusCode statusCode = httpResponse.StatusCode;

            return (statusCode >= HttpStatusCode.MovedPermanently &&
                statusCode < HttpStatusCode.BadRequest);
        }

        /*
         * 1. Add removed headers back to the request's headers
         * 2. If the InputStream is not-null, reset its position to 0
         */
        static void prepareRequestForRetry(S3Request request, long orignalStreamPosition)
        {
            StringBuilder removedHeaderString = new StringBuilder();
            if (request.InputStream != null)
            {
                request.InputStream.Position = orignalStreamPosition;
            }

            if (request.removedHeaders.Count > 0)
            {
                foreach (KeyValuePair<String, String> removedHeader in request.removedHeaders)
                {
                    request.Headers[removedHeader.Key] = removedHeader.Value;
                }
            }
        }

        static bool processRequestResponse<T>(HttpWebResponse httpResponse, S3Request request, out T response, out Exception cause)
            where T : S3Response, new()
        {
            response = default(T);
            cause = null;
            IDictionary<S3QueryParameter, string> parameters = request.parameters;
            string actionName = parameters[S3QueryParameter.Action];
            bool shouldRetry = false;

            if (httpResponse == null)
            {
                throw new WebException(
                    "The Web Response for a successful request is null!",
                    WebExceptionStatus.ProtocolError
                    );
            }

            WebHeaderCollection headerCollection = httpResponse.Headers;
            HttpStatusCode statusCode = httpResponse.StatusCode;
            string responseBody = null;

            try
            {
                if (actionName.Equals("GetObject"))
                {
                    response = new T();
                    Stream respStr = httpResponse.GetResponseStream();
                    request.BytesProcessed = httpResponse.ContentLength;

                    if (parameters.ContainsKey(S3QueryParameter.VerifyChecksum))
                    {
                        try
                        {
                            // The md5Digest needs to be verified
                            string checksumFromS3 = headerCollection[AWSSDKUtils.ETagHeader];
                            checksumFromS3 = checksumFromS3.Replace("\"", String.Empty);
                            if (respStr.CanSeek)
                            {
                                response.ResponseStream = respStr;
                            }
                            else
                            {
                                response.ResponseStream = AmazonS3Util.MakeStreamSeekable(respStr);
                            }
                        }
                        catch (Exception)
                        {
                            // Handle this error gracefully by setting the response object
                            // to be null. The outer finally block will catch the exception
                            // and close the httpResponse if the response object is null
                            response = null;
                            throw;
                        }
                    }
                    else
                    {
                        response.ResponseStream = respStr;
                    }
                }
                else
                {
                    using (httpResponse)
                    {
                        DateTime streamRead = DateTime.UtcNow;

                        using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
                        {
                            responseBody = reader.ReadToEnd();
                        }

                        request.BytesProcessed = responseBody.Length;
                        responseBody = responseBody.Trim();

                        if (responseBody.EndsWith("/Error>", StringComparison.OrdinalIgnoreCase))
                        {
                            // Attempt to deserialize response into S3ErrorResponse type
                            S3Error error;
                            XmlSerializer serializer = new XmlSerializer(typeof(S3Error));
                            XDocument doc = XDocument.Parse(responseBody);
                            using (XmlReader sr = doc.CreateReader())
                            {
                                error = (S3Error)serializer.Deserialize(sr);
                            }

                            cause = new AmazonS3Exception(statusCode, responseBody, parameters[S3QueryParameter.RequestAddress],
                                headerCollection, error);
                            shouldRetry = true;
                        }

                        // Perform response transformation
                        else if (responseBody.EndsWith(">", StringComparison.OrdinalIgnoreCase))
                        {
                            DateTime streamParsed = DateTime.UtcNow;

                            // Attempt to deserialize response into <Action> Response type
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            XDocument doc = XDocument.Parse(responseBody);

                            using (XmlReader sr = doc.CreateReader())
                            {
                                response = (T)serializer.Deserialize(sr);
                            }

                            //Explicitly mark the request was successful. 
                            //This information will be used by the concrete classes which they need to override.
                            response.ProcessResponseBody(responseBody);

                            DateTime objectCreated = DateTime.UtcNow;
                            request.ResponseReadTime = streamParsed - streamRead;
                            request.ResponseProcessingTime = objectCreated - streamParsed;
                            LOGGER.InfoFormat("Done reading response stream for request (id {0}). Stream read: {1}. Object create: {2}. Length of body: {3}",
                                request.Id,
                                request.ResponseReadTime,
                                request.ResponseProcessingTime,
                                request.BytesProcessed);
                        }
                        else
                        {
                            // We can receive responses that have no response body.
                            // All responses have headers so at a future point,
                            // we "do" attach the headers to the response.
                            response = new T();
                            response.ProcessResponseBody(responseBody);

                            DateTime streamParsed = DateTime.UtcNow;
                            request.ResponseReadTime = streamParsed - streamRead;
                        }
                    }

                    // We are done with our use of the httpResponse object
                    httpResponse = null;
                }
            }
            finally
            {
                if (actionName.Equals("GetObject") &&
                    response != null)
                {
                    // Save the http response object so that it can be closed
                    // gracefully when the GetObjectResponse object is either
                    // garbage-collected or disposed
                    response.httpResponse = httpResponse;
                }
                else if (httpResponse != null)
                {
                    httpResponse.Close();
                    httpResponse = null;
                }

                // Store the headers in the response for all successful service requests
                if (response != null)
                {
                    // Add the header key/value pairs to our <Action> Response type
                    response.Headers = headerCollection;
                    response.ResponseXml = responseBody;
                }
            }

            return shouldRetry;
        }

        private static bool processRequestError(string actionName, HttpWebRequest request, WebException we, HttpWebResponse errorResponse,
            string requestAddr, out WebHeaderCollection respHdrs, out Exception cause)
        {
            bool shouldRetry = false;
            HttpStatusCode statusCode = default(HttpStatusCode);
            string responseBody = null;

            // Initialize the out parameter to null
            // in case there is no errorResponse
            respHdrs = null;

            if (errorResponse == null)
            {
                LOGGER.Error(string.Format(CultureInfo.InvariantCulture, "Error making request {0}.", actionName), we);
                throw we;
            }

            // Set the response headers for future use
            respHdrs = errorResponse.Headers;

            // Obtain the HTTP status code
            statusCode = errorResponse.StatusCode;

            using (StreamReader reader = new StreamReader(errorResponse.GetResponseStream(), Encoding.UTF8))
            {
                responseBody = reader.ReadToEnd();
            }

            if (request.Method.Equals("HEAD"))
            {
                string message = we.Message;
                string errorCode = statusCode.ToString();
                if (statusCode == HttpStatusCode.NotFound)
                {
                    message = "The specified key does not exist";
                    errorCode = "NoSuchKey";
                }

                AmazonS3Exception excep = new AmazonS3Exception(
                    message,
                    statusCode,
                    errorCode,
                    respHdrs[S3Constants.AmzRequestIdHeader],
                    "",
                    "",
                    requestAddr,
                    respHdrs
                    );

                LOGGER.Error(string.Format(CultureInfo.InvariantCulture, "Error making request {0}.", actionName), excep);
                throw excep;
            }

            if (statusCode == HttpStatusCode.InternalServerError ||
                statusCode == HttpStatusCode.ServiceUnavailable)
            {
                shouldRetry = true;
                cause = we;
            }
            else
            {
                // Attempt to deserialize response into ErrorResponse type
                XDocument doc = XDocument.Parse(responseBody);
                using (XmlReader sr = doc.CreateReader())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(S3Error));
                    S3Error error = (S3Error)serializer.Deserialize(sr);

                    // Throw formatted exception with information available from the error response                    
                    AmazonS3Exception excep = new AmazonS3Exception(
                        error.Message,
                        statusCode,
                        error.Code,
                        error.RequestId,
                        error.HostId,
                        responseBody,
                        requestAddr,
                        respHdrs
                        );

                    LOGGER.Error(string.Format(CultureInfo.InvariantCulture, "Error making request {0}.", actionName), excep);
                    throw excep;
                }
            }

            return shouldRetry;
        }

        /**
         * Build the Url from the parameters passed in.
         * Component parts are:
         * - ServiceURL from the Config
         * - Bucket
         * - Key
         * - urlPrefix
         * - Query
         */
        static void addUrlToParameters(S3Request request, AmazonS3Config config)
        {
            Map parameters = request.parameters;

            if (!config.IsSetServiceURL())
            {
                throw new AmazonS3Exception("The Amazon S3 Service URL is either null or empty");
            }

            string url = config.ServiceURL;

            if (parameters[S3QueryParameter.BucketVersion].Equals(S3Constants.BucketVersions[1]))
            {
                url = String.Concat(url, parameters[S3QueryParameter.CanonicalizedResource]);
            }
            else if (parameters.ContainsKey(S3QueryParameter.DestinationBucket))
            {
                url = String.Concat(parameters[S3QueryParameter.DestinationBucket], ".", url, "/");

                if (parameters.ContainsKey(S3QueryParameter.Key))
                {
                    url = String.Concat(url, parameters[S3QueryParameter.Key]);
                }
            }

            string urlPrefix = "https://";
            if (config.CommunicationProtocol == Protocol.HTTP)
            {
                urlPrefix = "http://";
            }
            url = String.Concat(urlPrefix, url);

            // Encode the URL
            url = AmazonS3Util.UrlEncode(url, true);

            if (parameters.ContainsKey(S3QueryParameter.Query))
            {
                url = String.Concat(url, parameters[S3QueryParameter.Query]);
            }

            // Add the Url to the parameters
            parameters[S3QueryParameter.Url] = url;
        }

        /**
         * Configure HttpClient with set of defaults as well as configuration
         * from AmazonEC2Config instance
         */
        HttpWebRequest ConfigureWebRequest(S3Request request)
        {
            WebHeaderCollection headers = request.Headers;
            Map parameters = request.parameters;

            if (!parameters.ContainsKey(S3QueryParameter.Url))
            {
                throw new AmazonS3Exception("The Amazon S3 URL is either null or empty");
            }

            string url = parameters[S3QueryParameter.Url];

            HttpWebRequest httpRequest = WebRequest.Create(url) as HttpWebRequest;

            if (request != null)
            {
                string value = string.Empty;

                headers[S3Constants.AuthorizationHeader] = String.Concat(
                    "AWS ",
                    this.awsAccessKeyId,
                    ":",
                    parameters[S3QueryParameter.Authorization]);

                httpRequest.Headers = headers;

                // The Content-Type header could have been specified using
                // the S3Request.AddHeader method. If Content-Type was specified,
                // it needs to be removed and set as an explicit property
                // of the HttpWebRequest.             
                if (parameters.ContainsKey(S3QueryParameter.Range))
                {
                    string rangeHeader = parameters[S3QueryParameter.Range];
                    char[] splitter = { ':' };
                    string[] myRange = rangeHeader.Split(splitter);
                    httpRequest.Headers[HttpRequestHeader.Range] = string.Format(CultureInfo.InvariantCulture, "bytes={0}-{1}", myRange[0], myRange[1]);
                }

                value = headers[AWSSDKUtils.IfModifiedSinceHeader];
                if (!String.IsNullOrEmpty(value))
                {
                    DateTime date = DateTime.ParseExact(value, AWSSDKUtils.GMTDateFormat, null);
                    httpRequest.Headers[HttpRequestHeader.IfModifiedSince] = date.ToString();
                    request.removedHeaders[AWSSDKUtils.IfModifiedSinceHeader] = value;
                }

                value = headers[AWSSDKUtils.ContentTypeHeader];
                if (!String.IsNullOrEmpty(value))
                {
                    httpRequest.ContentType = value;
                    request.removedHeaders[AWSSDKUtils.ContentTypeHeader] = value;
                }
                if (parameters.ContainsKey(S3QueryParameter.ContentType))
                {
                    httpRequest.ContentType = parameters[S3QueryParameter.ContentType];
                }

                // Let's enable Expect100Continue only for PutObject requests
                //httpRequest.ServicePoint.Expect100Continue = request.Expect100Continue;

                // While checking the Action, for Get, Put and Copy Object, set
                // the timeout to the value specified in the request.
                if (request.SupportTimeout)
                {
                    int timeout = 0;
                    if (Int32.TryParse(parameters[S3QueryParameter.RequestTimeout], out timeout))
                        if (timeout > 0)
                        {
                            httpRequest.Headers[HttpRequestHeader.Expires] = timeout.ToString(CultureInfo.InvariantCulture);
                        }
                }

                httpRequest.UserAgent = config.UserAgent;
                httpRequest.Method = parameters[S3QueryParameter.Verb];
                httpRequest.AllowAutoRedirect = false;
            }
            return httpRequest;
        }

        /**
         * Exponential sleep on failed request
         */
        static void pauseOnRetry(int retries, int maxRetries, HttpStatusCode status, string requestAddr, WebHeaderCollection headers, Exception cause)
        {
            if (retries <= maxRetries)
            {
                int delay = (int)Math.Pow(4, retries) * 100;
                System.Threading.Thread.Sleep(delay);
            }
            else
            {
                throw new AmazonS3Exception(
                    String.Concat("Maximum number of retry attempts reached : ", (retries - 1)),
                    status,
                    requestAddr,
                    headers,
                    cause
                    );
            }
        }

        /// <summary>
        /// Sets the header information to use a S3CannedACL.
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="acl">Enum for the type of canned acl wanted</param>
        static void setCannedACLHeader(WebHeaderCollection headers, S3CannedACL acl)
        {
            headers[S3Constants.AmzAclHeader] = S3Constants.CannedAcls[(int)acl];
        }

        /// <summary>
        /// Sets the If-Match Header in the specified header collection.
        ///
        /// Return the object only if its entity tag (ETag) is the same as the one
        /// specified, otherwise return a 412 (precondition failed).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="eTag">The ETag to match against</param>
        static void setIfMatchHeader(WebHeaderCollection headers, string eTag)
        {
            headers[AWSSDKUtils.IfMatchHeader] = eTag;
        }

        /// <summary>
        /// Set the If-None-Match Header in the specified header collection.
        ///
        /// Return the object only if its entity tag (ETag) is different from the one
        /// specified, otherwise return a 304 (not modified).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="eTag">The ETag to match against</param>
        static void setIfNoneMatchHeader(WebHeaderCollection headers, string eTag)
        {
            headers["If-None-Match"] = eTag;
        }

        /// <summary>
        /// Sets the If-Modifed-Since Header in the specified header collection.
        ///
        /// Return the object only if it has been modified since the specified time,
        /// otherwise return a 304 (not modified).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="date">DateTime Object representing the date to use</param>
        static void setIfModifiedSinceHeader(WebHeaderCollection headers, DateTime date)
        {
            headers[AWSSDKUtils.IfModifiedSinceHeader] = date.ToUniversalTime().ToString(AWSSDKUtils.GMTDateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the If-Unmodifed-Since Header in the specified header collection.
        ///
        /// Return the object only if it has not been modified since the specified time,
        /// otherwise return a 412 (precondition failed).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="date">DateTime Object representing the date to use</param>
        static void setIfUnmodifiedSinceHeader(WebHeaderCollection headers, DateTime date)
        {
            headers["If-Unmodified-Since"] = date.ToUniversalTime().ToString(AWSSDKUtils.GMTDateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the If-Match Header for the CopyObject operation in the specified header collection.
        ///
        /// Return the object only if its entity tag (ETag) is the same as the one
        /// specified, otherwise return a 412 (precondition failed).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="eTag">The ETag to match against</param>
        static void setIfMatchCopyHeader(WebHeaderCollection headers, string eTag)
        {
            headers["x-amz-copy-source-if-match"] = eTag;
        }

        /// <summary>
        /// Sets the If-None-Match Header for the CopyObject operation.
        ///
        /// Return the object only if its entity tag (ETag) is different from the one
        /// specified, otherwise return a 304 (not modified).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="eTag">The ETag to match against</param>
        static void setIfNoneMatchCopyHeader(WebHeaderCollection headers, string eTag)
        {
            headers["x-amz-copy-source-if-none-match"] = eTag;
        }

        /// <summary>
        /// Sets the If-Modifed-Since Header for the CopyObject operation.
        ///
        /// Return the object only if it has been modified since the specified time,
        /// otherwise return a 304 (not modified).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="date">DateTime Object representing the date to use</param>
        static void setIfModifiedSinceCopyHeader(WebHeaderCollection headers, DateTime date)
        {
            headers["x-amz-copy-source-if-modified-since"] = date.ToUniversalTime().ToString(AWSSDKUtils.GMTDateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the If-Unmodifed-Since Header for the CopyObject operation.
        ///
        /// Return the object only if it has not been modified since the specified time,
        /// otherwise return a 412 (precondition failed).
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="date">DateTime Object representing the date to use</param>
        static void setIfUnmodifiedSinceCopyHeader(WebHeaderCollection headers, DateTime date)
        {
            headers["x-amz-copy-source-if-unmodified-since"] = date.ToUniversalTime().ToString(AWSSDKUtils.GMTDateFormat, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Valid values: COPY | REPLACE
        /// Constraints: Values other than COPY or REPLACE result in an immediate 400-based error
        /// response. You cannot copy an object to itself unless the S3MetadataDirective header is
        /// specified and its value set to REPLACE.
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="directive">Either COPY or REPLACE</param>
        static void setMetadataDirectiveHeader(WebHeaderCollection headers, S3MetadataDirective directive)
        {
            headers[S3Constants.AmzMetadataDirectiveHeader] = S3Constants.MetaDataDirectives[(int)directive];
        }

        /// <summary>
        /// Sets the x-amz-copy-source header based on the bucket and key passed
        /// as input.
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="bucket">The source bucket</param>
        /// <param name="key">The source key</param>
        static void setCopySourceHeader(WebHeaderCollection headers, string bucket, string key)
        {
            string source = bucket;
            if (key != null)
            {
                source = String.Concat("/", bucket, "/", key);
            }
            headers["x-amz-copy-source"] = AmazonS3Util.UrlEncode(source, true);
        }

        /// <summary>
        /// Sets the x-amz-version-id header based on the versionId passed as input
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="versionId">The versionId of the desired S3 object</param>
        static void setVersionIdHeader(WebHeaderCollection headers, string versionId)
        {
            headers[S3Constants.AmzVersionIdHeader] = versionId;
        }

        /// <summary>
        /// Sets the x-amz-mfa based on the serial and token passed as input
        /// </summary>
        /// <param name="headers">The header collection to add the new header to</param>
        /// <param name="mfaCodes">The tuple of the authentication device codes</param>
        static void setMfaHeader(WebHeaderCollection headers, Tuple<string, string> mfaCodes)
        {
            headers[S3Constants.AmzMfaHeader] = String.Concat(mfaCodes.First, " ", mfaCodes.Second);
        }

        /**
         * Creates a string based on the parameters and encrypts it using
         * key. Returns the encrypted string.
         */
        static string BuildSigningString(IDictionary<S3QueryParameter, string> parameters, WebHeaderCollection webHeaders)
        {
            StringBuilder sb = new StringBuilder("", 256);
            string value = null;

            sb.Append(parameters[S3QueryParameter.Verb]);
            sb.Append("\n");

            if (webHeaders != null)
            {
                if (!String.IsNullOrEmpty(value = webHeaders[AWSSDKUtils.ContentMD5Header]))
                {
                    sb.Append(value);
                }
                sb.Append("\n");

                if (parameters.ContainsKey(S3QueryParameter.ContentType))
                {
                    sb.Append(parameters[S3QueryParameter.ContentType]);
                }
                sb.Append("\n");
            }
            else
            {
                // The headers are null, but we still need to append
                // the 2 newlines that are required by S3.
                // Without these, S3 rejects the signature.
                sb.Append("\n\n");
            }

            if (parameters.ContainsKey(S3QueryParameter.Expires))
            {
                sb.Append(parameters[S3QueryParameter.Expires]);
                sb.Append("\n");
            }
            else
            {
                sb.Append("\n");
                sb.Append(BuildCanonicalizedHeaders(webHeaders));
            }
            if (parameters.ContainsKey(S3QueryParameter.CanonicalizedResource))
            {
                sb.Append(AmazonS3Util.UrlEncode(parameters[S3QueryParameter.CanonicalizedResource], true));
            }

            if (parameters.ContainsKey(S3QueryParameter.QueryToSign))
            {
                sb.Append(parameters[S3QueryParameter.QueryToSign]);
            }

            return sb.ToString();
        }

        /**
         * Returns a string of all x-amz headers sorted by Ordinal.
         */
        static StringBuilder BuildCanonicalizedHeaders(WebHeaderCollection headers)
        {
            // Build a sorted list of headers that start with x-amz
            List<string> list = new List<string>(headers.Count);
            foreach (string key in headers.AllKeys)
            {
                string lowerKey = key.ToLowerInvariant();
                if (lowerKey.StartsWith("x-amz-", StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(lowerKey);
                }
            }
            // Using the recommendations from:
            // http://msdn.microsoft.com/en-us/library/ms973919.aspx
            list.Sort(StringComparer.Ordinal);

            // Create the canonicalized header string to return.
            StringBuilder sb = new StringBuilder(256);
            foreach (string key in list)
            {
                sb.Append(String.Concat(key, ":", headers[key], "\n"));
            }

            return sb;
        }

        #endregion

        #region Async Classes

        class S3AsyncResult
        {
            private static Logger _logger = new Logger(typeof(S3AsyncResult));

            S3Request _s3Request;
            RequestState _requestState;
            long _orignalStreamPosition;
            object _state;
            int _retiresAttempt;
            Exception _exception;
            IS3Response _finalResponse;
            Dictionary<string, object> _parameters;

            private DateTime _startTime;

            internal S3AsyncResult(S3Request s3Request, object state)
            {
                this._s3Request = s3Request;
                this._state = state;

                this._startTime = DateTime.Now;
            }

            internal S3Request S3Request
            {
                get { return this._s3Request; }
                set { this._s3Request = value; }
            }

            internal Exception Exception
            {
                get { return this._exception; }
                set { this._exception = value; }
            }

            internal long OrignalStreamPosition
            {
                get { return this._orignalStreamPosition; }
                set { this._orignalStreamPosition = value; }
            }

            internal int RetriesAttempt
            {
                get { return this._retiresAttempt; }
                set { this._retiresAttempt = value; }
            }

            internal object State
            {
                get { return this._state; }
            }

            internal RequestState RequestState
            {
                get { return this._requestState; }
                set { this._requestState = value; }
            }

            internal IS3Response FinalResponse
            {
                get { return this._finalResponse; }
                set
                {
                    this._finalResponse = value;
                    DateTime endTime = DateTime.Now;
                    TimeSpan timeToComplete = endTime - this._startTime;
                    this._s3Request.TotalRequestTime = timeToComplete;
                    _logger.InfoFormat("S3 request completed: {0}", this._s3Request);
                }
            }
            internal Dictionary<string, object> Parameters
            {
                get
                {
                    if (this._parameters == null)
                    {
                        this._parameters = new Dictionary<string, object>();
                    }

                    return this._parameters;
                }
            }
        }

        class RequestState
        {
            Stream _inputStream;
            byte[] _requestData;
            long _requestDataLength;
            HttpWebRequest _webRequest;
            DateTime _webRequestStart;

            public RequestState(HttpWebRequest webRequest, Stream inputStream, byte[] requestData, long requestDataLength)
            {
                this._webRequest = webRequest;
                this._inputStream = inputStream;
                this._requestData = requestData;
                this._requestDataLength = requestDataLength;
                this._webRequestStart = DateTime.Now;
            }

            internal HttpWebRequest WebRequest
            {
                get { return this._webRequest; }
            }

            internal Stream InputStream
            {
                get { return this._inputStream; }
            }

            internal byte[] RequestData
            {
                get { return this._requestData; }
            }

            internal long RequestDataLength
            {
                get { return this._requestDataLength; }
            }

            internal DateTime WebRequestStart
            {
                get { return this._webRequestStart; }
            }
        }

        #endregion
    }
}
