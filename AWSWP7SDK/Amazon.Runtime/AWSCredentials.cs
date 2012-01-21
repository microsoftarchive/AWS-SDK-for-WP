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
using System.Security;
using System.Reflection;
using System.Xml;
using IO = System.IO;

namespace Amazon.Runtime
{
    /// <summary>
    /// Abstract class that represents a credentials object for AWS services. 
    /// </summary>
    public abstract class AWSCredentials
    {
        #region Constructor

        /// <summary>
        /// C'tor
        /// </summary>
        protected AWSCredentials()
        {
        }

        #endregion

        #region GetCredentials

        /// <summary>
        /// Returns a copy of ImmutableCredentials
        /// </summary>
        /// <returns></returns>
        public abstract ImmutableCredentials GetCredentials();

        #endregion
    }

    /// <summary>
    /// Immutable representation of AWS credentials. 
    /// </summary>
    public class ImmutableCredentials
    {
        #region Private Members

        private string accessKey;
        private string secretKey;

        #endregion

        #region Constructors

        /// <summary>
        /// C'tor
        /// </summary>
        private ImmutableCredentials()
        {
        }

        /// <summary>
        /// Constructs an ImmutableCredentials object with supplied accessKey, secretKey.
        /// </summary>
        /// <param name="awsAccessKeyId">access key for AWS services</param>
        /// <param name="awsSecretAccessKey">secret key for AWS services</param>
        public ImmutableCredentials(string awsAccessKeyId, string awsSecretAccessKey)
        {
            if (string.IsNullOrEmpty(awsAccessKeyId))
            {
                throw new ArgumentNullException("awsAccessKeyId");
            }

            if (string.IsNullOrEmpty(awsSecretAccessKey))
            {
                throw new ArgumentNullException("awsSecretAccessKey");
            }

            this.AccessKey = awsAccessKeyId;
            this.SecretKey = awsSecretAccessKey;
        }

        #endregion

        #region Copy

        /// <summary>
        /// Returns a copy of the current credentials.
        /// </summary>
        /// <returns></returns>
        public ImmutableCredentials Copy()
        {
            ImmutableCredentials credentials2 = new ImmutableCredentials();
            credentials2.AccessKey = this.AccessKey;
            credentials2.SecretKey = this.SecretKey;
            return credentials2;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AccessKey property for the current credentials. 
        /// </summary>
        public string AccessKey
        {
            get
            {
                return this.accessKey;
            }

            private set
            {
                this.accessKey = value;
            }
        }

        /// <summary>
        /// Gets the SecureSecretKey property for the current credentials.
        /// </summary>
        public string SecretKey
        {
            get
            {
                return this.secretKey;
            }

            private set
            {
                this.secretKey = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Credentials that are retrieved from ConfigurationManager.AppSettings 
    /// </summary>
    public class EnvironmentAWSCredentials : AWSCredentials
    {
        #region Private Members

        private ImmutableCredentials wrappedCredentials;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs an instance of EnvironmentAWSCredentials and attempts to load AccessKey and SecretKey from ConfigurationManager.AppSettings
        /// </summary>
        public EnvironmentAWSCredentials()
        {
            string accessKey = ConfigurationManager.GetAccessKey();
            string secretKey = ConfigurationManager.GetSecretKey();

            if (string.IsNullOrEmpty(accessKey))
            {
                throw new ArgumentException(string.Format("Access Key could not be found.  Add an appsetting to your App.config with the name {0} with a value of your access key.", "AWSAccessKey"));
            }

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentException(string.Format("Secret Key could not be found.  Add an appsetting to your App.config with the name {0} with a value of your secret key.", "AWSSecretKey"));
            }

            this.wrappedCredentials = new ImmutableCredentials(accessKey, secretKey);
        }

        #endregion

        #region GetCredentials

        /// <summary>
        /// Returns an instance of ImmutableCredentials for this instance.
        /// </summary>
        /// <returns></returns>
        public override ImmutableCredentials GetCredentials()
        {
            return this.wrappedCredentials.Copy();
        }

        #endregion
    }
}