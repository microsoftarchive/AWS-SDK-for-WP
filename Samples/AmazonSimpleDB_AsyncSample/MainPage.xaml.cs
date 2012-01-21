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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using Amazon;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Microsoft.Phone.Controls;
using System.Windows.Controls;

namespace AmazonSimpleDB_AsyncSample
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region Fields and Constants

        private string _domainName = string.Empty;
        private string _itemName;
        private int _selectedIndex = -1;
        const int MAX_ROWS = 5;
        private ObservableCollection<Amazon.SimpleDB.Model.Attribute> _attributes = new ObservableCollection<Amazon.SimpleDB.Model.Attribute>();
        private ObservableCollection<string> _domainNames = new ObservableCollection<string>();
        private string _listDomainMessage;
        private string _domainMetadataMessage;
        private string _domainDeleteMessage;
        private string _createDomainMessage;
        private bool _isDomainNameSet;

        #endregion Fields and Constants

        #region Properties

        /// <summary>
        /// Gets or sets a bool value indicating if the domain name if set or not.
        /// </summary>
        public bool IsDomainNameSet
        {
            get { return _isDomainNameSet; }
            set
            {
                _isDomainNameSet = value;
                OnPropertyChanged("IsDomainNameSet");
            }
        }

        /// <summary>
        /// Gets or sets the collection of <see cref="Amazon.SimpleDB.Model.Attribute"/>.
        /// </summary>
        public ObservableCollection<Amazon.SimpleDB.Model.Attribute> Attributes
        {
            get { return _attributes; }
        }

        public ObservableCollection<string> DomainNameList
        {
            get { return _domainNames; }
        }

        public String DomainName
        {
            get { return _domainName; }
            set
            {
                _domainName = value;
                OnPropertyChanged("DomainName");
            }
        }

        /// <summary>
        /// Gets or sets the name of the Item.
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                OnPropertyChanged("ItemName");
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        /// <summary>
        /// Gets or sets the message for the list-domain operation.
        /// </summary>
        public string ListDomainMessage
        {
            get { return _listDomainMessage; }
            set
            {
                _listDomainMessage = value;
                OnPropertyChanged("ListDomainMessage");
            }
        }

        /// <summary>
        /// Gets or sets the message for the domain-metadata operation.
        /// </summary>
        public string DomainMetadataMessage
        {
            get { return _domainMetadataMessage; }
            set
            {
                _domainMetadataMessage = value;
                OnPropertyChanged("DomainMetadataMessage");
            }
        }

        /// <summary>
        /// Gets or sets the message for the domain-delete operation.
        /// </summary>
        public string DomainDeleteMessage
        {
            get { return _domainDeleteMessage; }
            set
            {
                _domainDeleteMessage = value;
                OnPropertyChanged("DomainDeleteMessage");
            }
        }

        /// <summary>
        /// Gets or sets the message for the create-domain operation.
        /// </summary>
        public string CreateDomainMessage
        {
            get { return _createDomainMessage; }
            set
            {
                _createDomainMessage = value;
                OnPropertyChanged("CreateDomainMessage");
            }
        }

        #endregion Properties

        #region Constructor

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;

            SimpleDB.Client = AWSClientFactory.CreateAmazonSimpleDBClient();

            PropertyChanged += ((object sender, PropertyChangedEventArgs e) =>
                {
                    if (string.Compare(e.PropertyName, "SelectedIndex", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (SelectedIndex >= 0)
                            //Update the bucket-name with the currently selected one.
                            this.DomainName = this.DomainNameList[SelectedIndex];
                    }
                    else if (string.Compare(e.PropertyName, "DomainName", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (string.IsNullOrEmpty(_domainName))
                            this.IsDomainNameSet = false;
                        else
                            this.IsDomainNameSet = true;
                    }
                });
        }

        #endregion Constructor

        #region Exception Handling

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        void SimpleDBErrorResponse(AmazonSimpleDBException error)
        {
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "ERROR CODE: {0}", error.ErrorCode));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "INNER EXCEPTION: {0}", error.InnerException));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "MESSAGE: {0}", error.Message));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "REQUEST ID: {0}", error.RequestId));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "STATUS CODE: {0}", error.StatusCode)); ;
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "\nERROR RESPONSE:\n {0}", error.XML));

            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(errorBuilder.ToString(), "Error Occured", MessageBoxButton.OK);
            });
        }

        #endregion

        #region Domain Operations

        #region Create Domain

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void CreateDomainButton_Click(object sender, RoutedEventArgs e)
        {
            SimpleDB.Client.OnSimpleDBResponse += CreateDomainWebResponse;
            this.CreateDomainMessage = "Please wait...";
            SimpleDB.Client.BeginCreateDomain(new CreateDomainRequest()
                .WithDomainName(DomainName));
        }


        void CreateDomainWebResponse(object sender, ResponseEventArgs args)
        {
            ISimpleDBResponse result = args.Response;
            SimpleDB.Client.OnSimpleDBResponse -= CreateDomainWebResponse;
            if (result is AmazonSimpleDBException)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.CreateDomainMessage = "Error: " + ((AmazonSimpleDBException)result).Message;
                });
                return;
            }
            else
            {
                this.Dispatcher.BeginInvoke(() =>
                    {
                        this.CreateDomainMessage = "Domain created successfully.";
                    });
            }

        }

        #endregion

        #region List Domain

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ListDomainButton_Click(object sender, RoutedEventArgs e)
        {
            SimpleDB.Client.OnSimpleDBResponse += ListDomainWebResponse;
            ListDomainsRequest request = new ListDomainsRequest();
            this.ListDomainMessage = "Please wait...";
            this.DomainNameList.Clear();
            SimpleDB.Client.BeginListDomains(request);
        }

        void ListDomainWebResponse(object sender, ResponseEventArgs args)
        {
            ISimpleDBResponse result = args.Response;
            SimpleDB.Client.OnSimpleDBResponse -= ListDomainWebResponse;

            if (result is AmazonSimpleDBException)
            {
                this.Dispatcher.BeginInvoke(() =>
                    {
                        this.ListDomainMessage = "Error: " + ((AmazonSimpleDBException)result).Message;
                    });
                return;
            }

            ListDomainsResponse response = (ListDomainsResponse)result;
            this.Dispatcher.BeginInvoke(() =>
                {
                    DomainNameList.Clear();
                    response.ListDomainsResult.DomainName.ForEach(b => DomainNameList.Add(b));
                    this.ListDomainMessage = "No of Domains: " + response.ListDomainsResult.DomainName.Count;
                });
        }

        #endregion

        #region Domain Metadata

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void DomainMetadataButton_Click(object sender, RoutedEventArgs e)
        {
            SimpleDB.Client.OnSimpleDBResponse += DomainMetadataWebResponse;

            DomainMetadataRequest request = new DomainMetadataRequest() { DomainName = this.DomainName };
            SimpleDB.Client.BeginDomainMetadata(request);
        }

        void DomainMetadataWebResponse(object sender, ResponseEventArgs args)
        {
            ISimpleDBResponse result = args.Response;
            SimpleDB.Client.OnSimpleDBResponse -= DomainMetadataWebResponse;
            if (result is AmazonSimpleDBException)
            {
                this.Dispatcher.BeginInvoke(() =>
                    {
                        this.DomainMetadataMessage = "Error: " + ((AmazonSimpleDBException)result).Message;
                    });
                return;
            }

            DomainMetadataResponse response = (DomainMetadataResponse)result;
            DomainMetadataResult domainResult = response.DomainMetadataResult;

            StringBuilder metadataResponse = new StringBuilder();
            metadataResponse.AppendLine(string.Format(CultureInfo.InvariantCulture, "Attribute Name Count: {0}", domainResult.AttributeNameCount));
            metadataResponse.AppendLine(string.Format(CultureInfo.InvariantCulture, "Attribute Value Count: {0}", domainResult.AttributeValueCount));
            metadataResponse.AppendLine(string.Format(CultureInfo.InvariantCulture, "Item Count: {0}", domainResult.ItemCount));
            metadataResponse.AppendLine(string.Format(CultureInfo.InvariantCulture, "TimeStamp: {0}", domainResult.Timestamp));

            this.Dispatcher.BeginInvoke(() =>
                {
                    this.DomainMetadataMessage = metadataResponse.ToString();
                });
        }

        #endregion

        #region Delete Domain

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void DeleteDomainButton_Click(object sender, RoutedEventArgs e)
        {
            SimpleDB.Client.OnSimpleDBResponse += DeleteDomainWebResponse;
            DeleteDomainRequest request = new DeleteDomainRequest() { DomainName = this.DomainName };
            this.DomainDeleteMessage = "Please wait...";
            SimpleDB.Client.BeginDeleteDomain(request);
        }

        void DeleteDomainWebResponse(object sender, ResponseEventArgs args)
        {
            ISimpleDBResponse result = args.Response;
            SimpleDB.Client.OnSimpleDBResponse -= DeleteDomainWebResponse;
            if (result is AmazonSimpleDBException)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.DomainDeleteMessage = "Error: " + ((AmazonSimpleDBException)result).Message;
                });
                return;
            }
            else
                this.Dispatcher.BeginInvoke(() =>
                    {
                        this.DomainDeleteMessage = "Domain deleted successfully.";
                    });
        }

        #endregion

        #endregion

        #region INotifyPropertyChanged Properties

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property-changed event for the specified property-name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (null != PropertyChanged)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //Enable and/or disable the create-domain , domain-metadata and delete-domain buttons.
            TextBox textbox = sender as TextBox;
            if (null != textbox)
            {
                if (textbox.Text.Length > 0)
                    this.IsDomainNameSet = true;
                else
                    this.IsDomainNameSet = false;
            }
        }
    }

    public class Results
    {
        public int Successes { get; set; }
        public int Errors { get; set; }
    }
}
