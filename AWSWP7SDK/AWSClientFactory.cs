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
 *  AWS SDK for .NET
 *
 */
using Amazon.SQS;
using Amazon.S3;
using Amazon.SimpleDB;

namespace Amazon
{
    /// <summary>
    /// The Amazon Web Services SDK provides devlopers with a coherent and unified interface to the
    /// suite of Amazon Web Services. The intent is to facilitate the rapid building of
    /// applications that leverage multiple Amazon Web Services.
    /// <para>
    /// To get started, request an instance of the AWSClientFactory via this class's static Instance
    /// member. Use the factory instance to create clients for all the Web Services needed by
    /// the application.</para>
    /// </summary>
    public static class AWSClientFactory
    {
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
        /// <returns>An Amazon S3 client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonSimpleDB CreateAmazonSimpleDBClient()
        {
            return new AmazonSimpleDBClient();
        }

        /// <summary>
        /// Create a client for the Amazon SimpleDB Service with the default configuration
        /// </summary>
        /// <param name="awsAccessKey">The AWS Access Key associated with the account</param>
        /// <param name="awsSecretAccessKey">The AWS Secret Access Key associated with the account</param>
        /// <returns>An Amazon SimpleDB client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonSimpleDB CreateAmazonSimpleDBClient(
            string awsAccessKey,
            string awsSecretAccessKey
            )
        {
            return new AmazonSimpleDBClient(awsAccessKey, awsSecretAccessKey);
        }

        /// <summary>
        /// Create a client for the Amazon SimpleDB Service with the specified configuration
        /// </summary>
        /// <param name="awsAccessKey">The AWS Access Key associated with the account</param>
        /// <param name="awsSecretAccessKey">The AWS Secret Access Key associated with the account</param>
        /// <param name="config">Configuration options for the service like HTTP Proxy, # of connections, etc
        /// </param>
        /// <returns>An Amazon SimpleDB client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonSimpleDB CreateAmazonSimpleDBClient(
            string awsAccessKey,
            string awsSecretAccessKey,
            AmazonSimpleDBConfig config
            )
        {
            return new AmazonSimpleDBClient(awsAccessKey, awsSecretAccessKey, config);
        }

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
        /// <returns>An Amazon S3 client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonSQS CreateAmazonSQSClient()
        {
            return new AmazonSQSClient();
        }

        /// <summary>
        /// Create a client for the Amazon SQS service with the default configuration
        /// </summary>
        /// <param name="awsAccessKey">The AWS Access Key associated with the account</param>
        /// <param name="awsSecretAccessKey">The AWS Secret Access Key associated with the account</param>
        /// <returns>An Amazon SQS client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonSQS CreateAmazonSQSClient(
           string awsAccessKey,
           string awsSecretAccessKey)
        {
            return new AmazonSQSClient(awsAccessKey, awsSecretAccessKey);
        }

        /// <summary>
        /// Create a client for the Amazon SQS service with the specified configuration
        /// </summary>
        /// <param name="awsAccessKey">The AWS Access Key associated with the account</param>
        /// <param name="awsSecretAccessKey">The AWS Secret Access Key associated with the account</param>
        /// <param name="config">Configuration options for the service like HTTP Proxy, # of connections, etc
        /// </param>
        /// <returns>An Amazon SQS client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonSQS CreateAmazonSQSClient(
            string awsAccessKey,
            string awsSecretAccessKey,
            AmazonSQSConfig config)
        {
            return new AmazonSQSClient(awsAccessKey, awsSecretAccessKey, config);
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
        /// <returns>An Amazon S3 client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonS3 CreateAmazonS3Client()
        {
            return new AmazonS3Client();
        }

        /// <summary>
        /// Create a client for the Amazon S3 service with the default configuration
        /// </summary>
        /// <param name="awsAccessKey">The AWS Access Key associated with the account</param>
        /// <param name="awsSecretAccessKey">The AWS Secret Access Key associated with the account</param>
        /// <returns>An Amazon S3 client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonS3 CreateAmazonS3Client(
            string awsAccessKey,
            string awsSecretAccessKey
            )
        {
            return new AmazonS3Client(awsAccessKey, awsSecretAccessKey);
        }

        /// <summary>
        /// Create a client for the Amazon S3 service with the specified configuration
        /// </summary>
        /// <param name="awsAccessKey">The AWS Access Key associated with the account</param>
        /// <param name="awsSecretAccessKey">The AWS Secret Access Key associated with the account</param>
        /// <param name="config">Configuration options for the service like HTTP Proxy, # of connections, etc
        /// </param>
        /// <returns>An Amazon S3 client</returns>
        /// <remarks>
        /// </remarks>
        public static AmazonS3 CreateAmazonS3Client(
            string awsAccessKey,
            string awsSecretAccessKey,
            AmazonS3Config config
            )
        {
            return new AmazonS3Client(awsAccessKey, awsSecretAccessKey, config);
        }
    }
}