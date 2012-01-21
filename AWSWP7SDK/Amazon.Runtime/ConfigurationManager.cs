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
using System.Xml;
using System.Windows.Resources;
using System.IO;
using System.Collections.Generic;

namespace Amazon.Runtime
{
    /// <summary>
    /// Class to read application settings to retrive access/secret keys.
    /// </summary>
    public static class ConfigurationManager
    {
        #region Private Members

        private const string FILENAME = "app.config";
        private const string APPSETTINGS = "appsettings";
        private const string KEY = "key";
        private const string VALUE = "value";
        private const string ACCESSKEY = "AWSAccessKey";
        private const string SECRETKEY = "AWSSecretKey";

        static Dictionary<string, string> appSettings = null;

        #endregion

        #region Load Application Settings

        /// <summary>
        /// Load app config file
        /// </summary>
        private static void LoadConfigFile()
        {
            if (null == appSettings)
            {
                Stream xmlStream = Application.GetResourceStream(new Uri(FILENAME, UriKind.Relative)).Stream;

                if (xmlStream != null)
                {
                    XmlReader xmlReader = XmlReader.Create(xmlStream);
                    appSettings = new Dictionary<string, string>();
                    ReadApplicationSetting(xmlReader);
                }
            }
        }

        /// <summary>
        /// Read key/value pairs in appSettings section
        /// </summary>
        /// <param name="xmlReader"></param>
        private static void ReadApplicationSetting(XmlReader xmlReader)
        {

            while (!xmlReader.EOF && !(xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name.ToLower().Equals(APPSETTINGS)))
                xmlReader.Read();

            while (!xmlReader.EOF && !xmlReader.HasAttributes)
                xmlReader.Read();

            while (xmlReader.NodeType != XmlNodeType.EndElement)
            {
                string key = string.Empty;
                string value = string.Empty;

                while (xmlReader.MoveToNextAttribute())
                {
                    if (xmlReader.Name.ToLower().Equals(KEY))
                    {
                        key = xmlReader.ReadContentAsString().ToLower();
                        continue;
                    }

                    if (xmlReader.Name.ToLower().Equals(VALUE))
                    {
                        value = xmlReader.ReadContentAsString();
                    }

                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        appSettings.Add(key, value);
                    }
                }

                xmlReader.Read();
            }
        }

        #endregion

        #region GetAccessKey

        /// <summary>
        /// Get AWS access key from app settings
        /// </summary>
        /// <returns>access key</returns>
        internal static string GetAccessKey()
        {
            try
            {
                LoadConfigFile();

                if (appSettings.ContainsKey(ACCESSKEY.ToLower()))
                    return appSettings[ACCESSKEY.ToLower()];
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion

        #region GetAccessKey

        /// <summary>
        /// Get AWS secret key from app settings
        /// </summary>
        /// <returns>secret key</returns>
        internal static string GetSecretKey()
        {
            try
            {
                LoadConfigFile();
                if (appSettings.ContainsKey(SECRETKEY.ToLower()))
                    return appSettings[SECRETKEY.ToLower()];
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
