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

namespace Amazon.SQS
{
    public class AmazonSQSConfig
    {
        private string serviceVersion = "2009-02-01";
        private string serviceURL = @"https://queue.amazonaws.com";
        private string userAgent = Amazon.Util.AWSSDKUtils.SDKUserAgent;
        private string signatureVersion = "2";
        private string signatureMethod = "HmacSHA256";
        private string proxyHost;
        private int proxyPort = -1;
        private int maxErrorRetry = 3;
        private string proxyUsername;
        private string proxyPassword;

        /// <summary>
        /// Gets Service Version
        /// </summary>
        public string ServiceVersion
        {
            get { return this.serviceVersion; }
        }
        /// <summary>
        /// Gets and sets of the signatureMethod property.
        /// </summary>
        public string SignatureMethod
        {
            get { return this.signatureMethod; }
            set { this.signatureMethod = value; }
        }

        /// <summary>
        /// Sets the SignatureMethod property
        /// </summary>
        /// <param name="signatureMethod">SignatureMethod property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithSignatureMethod(string signatureMethod)
        {
            this.signatureMethod = signatureMethod;
            return this;
        }

        /// <summary>
        /// Checks if SignatureMethod property is set
        /// </summary>
        /// <returns>true if SignatureMethod property is set</returns>
        public bool IsSetSignatureMethod()
        {
            return this.signatureMethod != null;
        }
        /// <summary>
        /// Gets and sets of the SignatureVersion property.
        /// </summary>
        public string SignatureVersion
        {
            get { return this.signatureVersion; }
            set { this.signatureVersion = value; }
        }

        /// <summary>
        /// Sets the SignatureVersion property
        /// </summary>
        /// <param name="signatureVersion">SignatureVersion property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithSignatureVersion(string signatureVersion)
        {
            this.signatureVersion = signatureVersion;
            return this;
        }

        /// <summary>
        /// Checks if SignatureVersion property is set
        /// </summary>
        /// <returns>true if SignatureVersion property is set</returns>
        public bool IsSetSignatureVersion()
        {
            return this.signatureVersion != null;
        }

        /// <summary>
        /// Gets and sets of the UserAgent property.
        /// </summary>
        public string UserAgent
        {
            get { return this.userAgent; }
            set { this.userAgent = value; }
        }

        /// <summary>
        /// Sets the UserAgent property
        /// </summary>
        /// <param name="userAgent">UserAgent property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithUserAgent(string userAgent)
        {
            this.userAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Checks if UserAgent property is set
        /// </summary>
        /// <returns>true if UserAgent property is set</returns>
        public bool IsSetUserAgent()
        {
            return this.userAgent != null;
        }

        /// <summary>
        /// Gets and sets of the ServiceURL property.
        /// This is an optional property; change it
        /// only if you want to try a different service
        /// endpoint or want to switch between https and http.
        /// </summary>
        public string ServiceURL
        {
            get { return this.serviceURL; }
            set { this.serviceURL = value; }
        }

        /// <summary>
        /// Sets the ServiceURL property
        /// </summary>
        /// <param name="serviceURL">ServiceURL property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithServiceURL(string serviceURL)
        {
            this.serviceURL = serviceURL;
            return this;
        }

        /// <summary>
        /// Sets the ServiceURL property
        /// </summary>
        /// <param name="serviceURL">ServiceURL value.</param>
        /// <returns>this instance</returns>
        //public AmazonSQSConfig WithServiceURL(Uri serviceURL)
        //{
        //    this.serviceURL = serviceURL;
        //    return this;
        //}

        /// <summary>
        /// Checks if ServiceURL property is set
        /// </summary>
        /// <returns>true if ServiceURL property is set</returns>
        public bool IsSetServiceURL()
        {
            return this.serviceURL != null;
        }

        /// <summary>
        /// Gets and sets of the ProxyHost property.
        /// </summary>
        public string ProxyHost
        {
            get { return this.proxyHost; }
            set { this.proxyHost = value; }
        }

        /// <summary>
        /// Sets the ProxyHost property
        /// </summary>
        /// <param name="proxyHost">ProxyHost property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithProxyHost(string proxyHost)
        {
            this.proxyHost = proxyHost;
            return this;
        }

        /// <summary>
        /// Checks if ProxyHost property is set
        /// </summary>
        /// <returns>true if ProxyHost property is set</returns>
        public bool IsSetProxyHost()
        {
            return this.proxyHost != null;
        }

        /// <summary>
        /// Gets and sets of the ProxyPort property.
        /// </summary>
        public int ProxyPort
        {
            get { return this.proxyPort; }
            set { this.proxyPort = value; }
        }

        /// <summary>
        /// Sets the ProxyPort property
        /// </summary>
        /// <param name="proxyPort">ProxyPort property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithProxyPort(int proxyPort)
        {
            this.proxyPort = proxyPort;
            return this;
        }

        /// <summary>
        /// Checks if ProxyPort property is set
        /// </summary>
        /// <returns>true if ProxyPort property is set</returns>
        public bool IsSetProxyPort()
        {
            return this.proxyPort >= 0;
        }

        /// <summary>
        /// Gets and sets of the MaxErrorRetry property.
        /// </summary>
        public int MaxErrorRetry
        {
            get { return this.maxErrorRetry; }
            set { this.maxErrorRetry = value; }
        }

        /// <summary>
        /// Sets the MaxErrorRetry property
        /// </summary>
        /// <param name="maxErrorRetry">MaxErrorRetry property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithMaxErrorRetry(int maxErrorRetry)
        {
            this.maxErrorRetry = maxErrorRetry;
            return this;
        }

        /// <summary>
        /// Checks if MaxErrorRetry property is set
        /// </summary>
        /// <returns>true if MaxErrorRetry property is set</returns>
        public bool IsSetMaxErrorRetry()
        {
            return this.maxErrorRetry >= 0;
        }

        /// <summary>
        /// Gets and sets the ProxyUsername property.
        /// Used in conjunction with the ProxyPassword
        /// property to authenticate requests with the
        /// specified Proxy server.
        /// </summary>
        public string ProxyUsername
        {
            get { return this.proxyUsername; }
            set { this.proxyUsername = value; }
        }

        /// <summary>
        /// Sets the ProxyUsername property
        /// </summary>
        /// <param name="username">Value for the ProxyUsername property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithProxyUsername(string username)
        {
            this.proxyUsername = username;
            return this;
        }

        /// <summary>
        /// Gets and sets the ProxyPassword property.
        /// Used in conjunction with the ProxyUsername
        /// property to authenticate requests with the
        /// specified Proxy server.
        /// </summary>
        /// <remarks>
        /// If this property isn't set, String.Empty is used as
        /// the proxy password. This property isn't
        /// used if ProxyUsername is null or empty.
        /// </remarks>
        public string ProxyPassword
        {
            get { return this.proxyPassword; }
            set { this.proxyPassword = value; }
        }

        /// <summary>
        /// Sets the ProxyPassword property.
        /// Used in conjunction with the ProxyUsername
        /// property to authenticate requests with the
        /// specified Proxy server.
        /// </summary>
        /// <remarks>
        /// If this property isn't set, String.Empty is used as
        /// the proxy password. This property isn't
        /// used if ProxyUsername is null or empty.
        /// </remarks>
        /// <param name="password">ProxyPassword property</param>
        /// <returns>this instance</returns>
        public AmazonSQSConfig WithProxyPassword(string password)
        {
            this.proxyPassword = password;
            return this;
        }
    }
}
