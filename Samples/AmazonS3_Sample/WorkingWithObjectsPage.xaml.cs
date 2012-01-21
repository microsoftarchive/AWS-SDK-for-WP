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
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Phone.Controls;

namespace AmazonS3_Sample
{
    public partial class WorkingWithObjectsPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region Fields

        private string _specialCharacters = @"[<>?*\""/|%^()@#!&!$~{}|:;',? \\]";
        private string _specialChar_forObject = @"[<>?*\""/|%^()@#!&!$~{}|:;',?\\]";
        private ObservableCollection<string> _bucketNames = new ObservableCollection<string>();
        private ObservableCollection<S3Grant> _acls = new ObservableCollection<S3Grant>();
        private ObservableCollection<S3Object> _bucketObjects = new ObservableCollection<S3Object>();
        private ObservableCollection<string> _objectKeys = new ObservableCollection<string>();
        private string _bucketName;
        private bool _bucketNameSelected; // state of the UI. True when bucket name selected in list
        private bool _enableObjectButtons; // Only enable when user has selected a bucket
        private string _objectKeyToCreate = string.Empty;
        private string _bucketNameDestination;
        private bool _destinationBucketSelected;
        private string _bucketKey = string.Empty;
        private bool _objectInBucketSelected;
        private bool _objectKeyToCreateEntered;
        private string _bucketKeyDestination;
        private string _getObjectContent;
        private string _createObjectResult;
        private string _deleteObjectResult;
        private string _contentOfBody;
        private bool _contentToUploadEntered;
        private string _putObjectResult;
        private string _ownerDisplayName;
        private string _createBucketName;
        private string _selectedBucketName;
        private string _ownerId;
        private int _selectedIndex = -1;
        private int _objectListSelectedIndex = -1;
        private int _selectedIndexDestinationBucket = -1;
        private Visibility _aclErrorMessageVisibility = Visibility.Collapsed;
        private Visibility _bucketObjectsMessageVisibility = Visibility.Collapsed;
        private string _getACLErrorMessage;
        private string _bucketObjectsMessage;
        private string _copyObjectResult;
        private int _noOfObjects;
        private string _getObjectMetadataResult;
        private string _getBucketResult = string.Empty;
        private string _setACLResult = string.Empty;

        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets a bool indicating if the Get Object etc. buttons should be enabled
        /// </summary>
        public bool EnableObjectButtons
        {
            get { return _enableObjectButtons; }
            set
            {
                _enableObjectButtons = value;
                OnPropertyChanged("EnableObjectButtons");
            }
        }

        public string GetBucketResult
        {
            get { return _getBucketResult; }
            set
            {
                _getBucketResult = value;
                OnPropertyChanged("GetBucketResult");
            }
        }

        /// <summary>
        /// Gets or sets the message for the Get-Object-Metadata action.
        /// </summary>
        public string GetObjectMetadataResult
        {
            get { return _getObjectMetadataResult; }
            set
            {
                _getObjectMetadataResult = value;
                OnPropertyChanged("GetObjectMetadataResult");
            }
        }

        /// <summary>
        /// Gets or sets the message for the Copy-Object action.
        /// </summary>
        public string CopyObjectResult
        {
            get { return _copyObjectResult; }
            set
            {
                _copyObjectResult = value;
                OnPropertyChanged("CopyObjectResult");
            }
        }

        /// <summary>
        /// Gets or sets the message of the Get-ACL-Error-Textblock.
        /// </summary>
        public string GetACLErrorMessage
        {
            get { return _getACLErrorMessage; }
            set
            {
                _getACLErrorMessage = value;
                OnPropertyChanged("GetACLErrorMessage");
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the Get-ACL-Error-Textblock.
        /// </summary>
        public Visibility ACLErrorMessageVisibility
        {
            get { return _aclErrorMessageVisibility; }
            set
            {
                _aclErrorMessageVisibility = value;
                OnPropertyChanged("ACLErrorMessageVisibility");
            }
        }

        /// <summary>
        /// Gets or sets the message of the List-Objects action.
        /// </summary>
        public string BucketObjectsMessage
        {
            get { return _bucketObjectsMessage; }
            set
            {
                _bucketObjectsMessage = value;
                OnPropertyChanged("BucketObjectsMessage");
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the List-Objects action status. It is used to show messages like error-messages or any other information.
        /// </summary>
        public Visibility BucketObjectsMessageVisibility
        {
            get { return _bucketObjectsMessageVisibility; }
            set
            {
                _bucketObjectsMessageVisibility = value;
                OnPropertyChanged("BucketObjectsMessageVisibility");
            }
        }

        /// <summary>
        /// Gets or sets the selected index of the <see cref="S3Grant"/> listbox.
        /// </summary>
        public int BucketListSelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged("BucketListSelectedIndex");
            }
        }

        /// <summary>
        /// Gets or sets the state of whether user has selected a bucket name in the list
        /// </summary>
        public bool BucketNameSelected
        {
            get { return _bucketNameSelected; }
            set
            {
                _bucketNameSelected = value;
                OnPropertyChanged("BucketNameSelected");
            }
        }


        /// <summary>
        /// Gets or sets the selected index of the listbox that contains names for the destination bucket.
        /// </summary>
        public int SelectedIndexDestinationBucket
        {
            get { return _selectedIndexDestinationBucket; }
            set
            {
                _selectedIndexDestinationBucket = value;
                OnPropertyChanged("SelectedIndexDestinationBucket");
            }
        }

        /// <summary>
        /// Gets or sets the UI state true if user has selected a destination bucket for copy action
        /// </summary>
        public bool DestinationBucketSelected
        {
            get { return _destinationBucketSelected; }
            set
            {
                _destinationBucketSelected = value;
                OnPropertyChanged("DestinationBucketSelected");
            }
        }

        /// <summary>
        /// Gets or sets the selected index of the Key in the List-Objects listbox.
        /// </summary>
        public int ObjectListSelectedIndex
        {
            get { return _objectListSelectedIndex; }
            set
            {
                _objectListSelectedIndex = value;
                OnPropertyChanged("ObjectListSelectedIndex");
            }
        }

        /// <summary>
        /// Gets or sets the display name of the owner for the current Bucket.
        /// </summary>
        public string OwnerDisplayName
        {
            get { return _ownerDisplayName; }
            set
            {
                _ownerDisplayName = value;
                OnPropertyChanged("OwnerDisplayName");
            }
        }

        /// <summary>
        /// Gets or sets the ID of the owner for the current Bucket.
        /// </summary>
        public string OwnerId
        {
            get { return _ownerId; }
            set
            {
                _ownerId = value;
                OnPropertyChanged("OwnerId");
            }
        }

        /// <summary>
        /// Gets or sets the result of Get-Object.
        /// </summary>
        public string GetObjectContent
        {
            get { return _getObjectContent; }
            set
            {
                _getObjectContent = value;
                OnPropertyChanged("GetObjectContent");
            }
        }

        /// <summary>
        /// Gets or sets the result of Delete-Object.
        /// </summary>
        public string DeleteObjectResult
        {
            get { return _deleteObjectResult; }
            set
            {
                _deleteObjectResult = value;
                OnPropertyChanged("DeleteObjectResult");
            }
        }

        /// <summary>
        /// Gets or sets the Object Key of the object to create
        /// </summary>
        public string ObjectKeyToCreate
        {
            get { return _objectKeyToCreate; }
            set
            {
                _objectKeyToCreate = value;
                OnPropertyChanged("ObjectKeyToCreate");
            }
        }


        /// <summary>
        /// Gets or sets the UI state: true if user has entered Object Key to Create
        /// </summary>
        public bool ObjectKeyToCreateEntered
        {
            get { return _objectKeyToCreateEntered; }
            set
            {
                _objectKeyToCreateEntered = value;
                OnPropertyChanged("ObjectKeyToCreateEntered");
            }
        }

        /// <summary>
        /// Gets or sets the name of the Key.
        /// </summary>
        public string CreateObjectResult
        {
            get { return _createObjectResult; }
            set
            {
                _createObjectResult = value;
                OnPropertyChanged("CreateObjectResult");
            }
        }
        /// <summary>
        /// Gets or sets the name of the Key.
        /// </summary>
        public string BucketKey
        {
            get { return _bucketKey; }
            set
            {
                _bucketKey = value;
                OnPropertyChanged("BucketKey");
            }
        }

        /// <summary>
        /// Gets or sets the state of whether user has selected an object in the list of objects in the current bucket.
        /// </summary>
        public bool ObjectInBucketSelected
        {
            get { return _objectInBucketSelected; }
            set
            {
                _objectInBucketSelected = value;
                OnPropertyChanged("ObjectInBucketSelected");
            }
        }

        /// <summary>
        /// Gets or sets the number of objects in the Bucket.
        /// </summary>
        public int NoOfObjects
        {
            get { return _noOfObjects; }
            set
            {
                _noOfObjects = value;
                OnPropertyChanged("NoOfObjects");
            }
        }

        /// <summary>
        /// Gets or sets the name of the destination Key.
        /// </summary>
        public string BucketKeyDestination
        {
            get { return _bucketKeyDestination; }
            set
            {
                _bucketKeyDestination = value;
                OnPropertyChanged("BucketKeyDestination");
            }
        }

        /// <summary>
        /// Gets or sets the name of the bucket to create.
        /// </summary>
        public string CreateBucketName
        {
            get { return _createBucketName; }
            set
            {
                _createBucketName = value;
                OnPropertyChanged("CreateBucketName");
            }
        }

        /// <summary>
        /// Gets or sets the name of the bucket to delete.
        /// </summary>
        public string SelectedBucketName
        {
            get { return _selectedBucketName; }
            set { _selectedBucketName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BucketName
        {
            get { return _bucketName; }
            set
            {
                _bucketName = value;
                OnPropertyChanged("BucketName");
            }
        }

        /// <summary>
        /// Gets or sets the name of the destination bucket. This is used for Copy-Object action.
        /// </summary>
        public string BucketNameDestination
        {
            get { return _bucketNameDestination; }
            set
            {
                _bucketNameDestination = value;
                OnPropertyChanged("BucketNameDestination");
            }
        }

        /// <summary>
        /// Gets or sets the collection of bucket names.
        /// </summary>
        public ObservableCollection<string> BucketNames
        {
            get { return _bucketNames; }
        }

        /// <summary>
        /// Gets or sets the collection of ACLs.
        /// </summary>
        public ObservableCollection<S3Grant> ACL
        {
            get { return _acls; }
        }

        /// <summary>
        /// Gets or sets the collection of objects in a Bucket.
        /// </summary>
        public ObservableCollection<S3Object> BucketObjects
        {
            get { return _bucketObjects; }
        }

        /// <summary>
        /// Gets or sets the collection of object keys (names).
        /// </summary>
        public ObservableCollection<string> ObjectKeys
        {
            get { return _objectKeys; }
        }
        /// <summary>
        /// Gets or sets the result of the Put-Object action.
        /// </summary>
        public string PutObjectResult
        {
            get { return _putObjectResult; }
            set
            {
                _putObjectResult = value;
                OnPropertyChanged("PutObjectResult");
            }
        }

        /// <summary>
        /// Gets or sets the content to put in the bucket.
        /// </summary>
        public string ContentOfBody
        {
            get { return _contentOfBody; }
            set
            {
                _contentOfBody = value;
                OnPropertyChanged("ContentOfBody");
            }
        }

        /// <summary>
        /// Gets or sets the UI state: true if user has entered some content to upload
        /// </summary>
        public bool PutObjectButtonEnabled
        {
            get { return _contentToUploadEntered; }
            set
            {
                _contentToUploadEntered = value;
                OnPropertyChanged("PutObjectButtonEnabled");
            }
        }

        /// <summary>
        /// Gets or sets the result of teh Set-ACL action
        /// </summary>
        public string SetACLResult
        {
            get { return _setACLResult; }
            set
            {
                _setACLResult = value;
                OnPropertyChanged("SetACLResult");
            }
        }
        #endregion Properties

        #region Constructor

        public WorkingWithObjectsPage()
        {
            InitializeComponent();
            this.DataContext = this;

            ListBuckets();

            this.EnableObjectButtons = false;

            this.PropertyChanged += HandlePropertyChangedEvent;
        }

        #endregion Constructor

        #region Property Changed Event Handler
        private void HandlePropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {
            if (string.Compare(e.PropertyName, "BucketListSelectedIndex", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (BucketListSelectedIndex < 0)
                {
                    this.BucketNameSelected = false;
                    return;
                }
                // User has selected a bucket from the list of buckets
                // Update the bucket-name with the currently selected one.
                this.BucketName = _bucketNames[BucketListSelectedIndex];
                this.SelectedBucketName = _bucketNames[BucketListSelectedIndex];
                this.BucketNameSelected = true;
                // Get the list of objects that reside in this bucket and display this list
                ListObjects();
            }
            else if (string.Compare(e.PropertyName, "ObjectListSelectedIndex", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (ObjectListSelectedIndex < 0)
                {
                    this.ObjectInBucketSelected = false;
                    this.EnableObjectButtons = false;
                    this.BucketKey = string.Empty;
                }
                else
                {
                    // User has selected an object key from the list of objects.
                    // Enable the Get / Delete / etc Object buttons. Update the Bucket-Key.
                    this.BucketKey = _objectKeys[ObjectListSelectedIndex];
                    this.ObjectInBucketSelected = true;
                    this.EnableObjectButtons = true;
                }
                ClearAllResults();
            }
            else if (string.Compare(e.PropertyName, "SelectedIndexDestinationBucket", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (SelectedIndexDestinationBucket < 0) return;

                this.BucketNameDestination = _bucketNames[SelectedIndexDestinationBucket];
                this.DestinationBucketSelected = true;
            }
        }

        #endregion Property Changed Event Handler

        #region Error Handling

        void S3ErrorResponse(AmazonS3Exception error)
        {
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "ERROR CODE: {0}", error.ErrorCode));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "HOST ID: {0}", error.HostId));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "INNER EXCEPTION: {0}", error.InnerException));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "MESSAGE: {0}", error.Message));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "REQUEST ID: {0}", error.RequestId));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "STATUS CODE: {0}", error.StatusCode));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "REQUEST ADDRESS: {0}", error.S3RequestAddress));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "\nERROR RESPONSE:\n {0}", error.XML));

            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(errorBuilder.ToString(), "Error Occured", MessageBoxButton.OK);
            });
        }

        #endregion

        #region ListBuckets
        /// <summary>
        /// ListBuckets() - Get a list of all buckets in the current account
        /// </summary>
        private void ListBuckets()
        {
            SelectedBucketName = string.Empty;

            S3Common.client.OnS3Response += ListBucketWebResponse;
            this.Dispatcher.BeginInvoke(() =>
            {
                this.BucketNames.Clear();
                this.BucketNames.Add("Please wait...");
            });
            S3Common.client.ListBuckets();
        }

        void ListBucketWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= ListBucketWebResponse;
            if (null != result)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.BucketNames.Clear();
                });
                S3ErrorResponse(result as AmazonS3Exception);
            }
            else
            {
                ListBucketsResponse response = args.Response as ListBucketsResponse;
                this.Dispatcher.BeginInvoke(() =>
                {
                    BucketNames.Clear();
                    this.BucketNameSelected = false;
                    response.Buckets.ToList().ForEach(b => BucketNames.Add(b.BucketName));
                });
            }

        }
        #endregion ListBuckets

        #region ListObjects

        /// <summary>
        /// ListObjectsRequest - get the list of all objects in the selected bucket
        /// </summary>
        private void ListObjects()
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName))
                {
                    Regex rg = new Regex(_specialCharacters);
                    if (rg.IsMatch(this.BucketName.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.BucketObjects.Clear();
                            this.NoOfObjects = 0;
                            this.BucketObjectsMessageVisibility = System.Windows.Visibility.Visible;
                            this.BucketObjectsMessage = "Error:\nA Bucket name cannot have special characters.";
                        });
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.BucketObjectsMessageVisibility = System.Windows.Visibility.Visible;
                        this.BucketObjectsMessage = "Please wait...";

                        this.BucketObjects.Clear();
                    });

                    ListObjectsRequest request = new ListObjectsRequest { BucketName = this.BucketName };
                    S3Common.client.OnS3Response += Client_ListObjectsResponse;

                    S3Common.client.ListObjects(request);
                }
            }
        }

        /// <summary>
        /// Callback for the List-Object action.
        /// </summary>
        /// <param name="result">The <see cref="IS3Response"/> object.</param>
        private void Client_ListObjectsResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_ListObjectsResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            ListObjectsResponse response = args.Response as ListObjectsResponse;

            if (null == response) return;

            this.Dispatcher.BeginInvoke(() =>
            {
                this.BucketObjectsMessageVisibility = System.Windows.Visibility.Collapsed;
                this.BucketObjectsMessage = string.Empty;
                this.ObjectKeys.Clear();

                response.S3Objects.ForEach(g => ObjectKeys.Add(g.Key));
                this.NoOfObjects = this.ObjectKeys.Count;
            }
           );
        }

        #endregion ListObjects

        #region Get-Object

        /// <summary>
        /// Event handler for the Get-Object action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnGetObjectContent_Click(object sender, RoutedEventArgs e)
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName) && !string.IsNullOrEmpty(this.BucketKey))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.GetObjectContent = "Please wait...";
                    });
                    GetObjectRequest request = new GetObjectRequest();
                    request.BucketName = this.BucketName;

                    request.Key = this.BucketKey;
                    S3Common.client.OnS3Response += Client_GetObjectResponse;
                    S3Common.client.GetObject(request);
                }
            }
        }

        /// <summary>
        /// The callback for the Get-Object action.
        /// </summary>
        /// <param name="result">The <see cref="IS3Response"/> object.</param>
        void Client_GetObjectResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_GetObjectResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }

            GetObjectResponse response = args.Response as GetObjectResponse;
            if (response == null) return;


            string responseBody = string.Empty;
            using (StreamReader streamRead = new StreamReader(response.ResponseStream))
            {
                responseBody = streamRead.ReadToEnd();
            }

            if (responseBody.Length > 200)
            {
                responseBody = responseBody.Substring(0, 200);
            }

            this.Dispatcher.BeginInvoke(() =>
            {
                this.GetObjectContent = "Object Content: " + responseBody;
                this.DeleteObjectResult = string.Empty;
            });
        }

        #endregion Get-Objects

        #region Put-Object

        /// <summary>
        /// Event handler for the Put-Object action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnPutObject_Click(object sender, RoutedEventArgs e)
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName))
                {
                    Regex rg = new Regex(_specialCharacters);
                    if (rg.IsMatch(this.BucketName.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.PutObjectResult = "Error:\nA Bucket name cannot have special characters.";
                        }
                        );
                        return;
                    }
                    Regex rgForObject = new Regex(_specialChar_forObject);
                    if (rgForObject.IsMatch(this.BucketKey.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.PutObjectResult = "Error:\nA Bucket Key cannot have special characters.";
                        }
                        );
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.PutObjectResult = "Please wait...";
                    });

                    PutObjectRequest putRequest = new PutObjectRequest();
                    putRequest.BucketName = this.BucketName;
                    putRequest.Key = this.BucketKey;
                    putRequest.ContentBody = this.ContentOfBody;
                    S3Common.client.OnS3Response += Client_PutObjectResponse;

                    S3Common.client.PutObject(putRequest);
                }
            }
        }

        /// <summary>
        /// The callback for the Put-Object action.
        /// </summary>
        /// <param name="result">The <see cref="IS3Response"/> object.</param>
        private void Client_PutObjectResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_PutObjectResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            PutObjectResponse response = args.Response as PutObjectResponse;

            if (null != response)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.PutObjectResult = response.IsRequestSuccessful ? "The object was put successfully." : "Error:\nError putting object.";
                    this.ContentToUploadTextBox.Text = String.Empty;
                }
                );
            }
        }

        #endregion Put-Object

        #region Delete-Object
        /// <summary>
        /// Event handler for the Delete-Object action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnDeleteObject_Click(object sender, RoutedEventArgs e)
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName))
                {
                    Regex rg = new Regex(_specialCharacters);
                    if (rg.IsMatch(this.BucketName.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.DeleteObjectResult = "Error:\nA 'Bucket name cannot have special characters.";
                        }
                        );
                        return;
                    }
                    Regex rgForObject = new Regex(_specialChar_forObject);
                    if (rgForObject.IsMatch(this.BucketKey.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.DeleteObjectResult = "Error:\nA Bucket Key cannot have special characters.";
                        }
                        );
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.DeleteObjectResult = "Please wait...";
                    });

                    DeleteObjectRequest request = new DeleteObjectRequest { BucketName = this.BucketName, Key = this.BucketKey };
                    S3Common.client.OnS3Response += Client_DeleteObjectResponse;

                    S3Common.client.DeleteObject(request);
                }
            }
        }

        /// <summary>
        /// Event handler for the Delete-Object action.
        /// </summary>
        /// <param name="result">The <see cref="IS3Response"/> object.</param>
        private void Client_DeleteObjectResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_ListObjectsResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }

            DeleteObjectResponse response = args.Response as DeleteObjectResponse;

            if (null == response) return;

            this.Dispatcher.BeginInvoke(() =>
            {
                this.DeleteObjectResult = response.IsRequestSuccessful ? "Object deleted successfully." : "Error deleting object.";

                // Remove the deleted key from the ListObjectsListBox
                var s3Object = ObjectKeys.FirstOrDefault(x => x == this.BucketKey);
                this.ObjectKeys.Remove(s3Object);

                // Clear the "GetObjectContent" text block               
                this.GetObjectContent = string.Empty;

                // Clear the selected object in the list 
                ObjectListSelectedIndex = -1;
            }
           );
        }

        #endregion Delete-Object

        #region Create Object
        /// <summary>
        /// Event handler for the Create New Object (this uses the Put-Object API).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnCreateObject_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ObjectKeyToCreate))
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.PutObjectResult = "Please wait...";
                });

                PutObjectRequest putRequest = new PutObjectRequest();
                putRequest.BucketName = this.BucketName;
                putRequest.Key = ObjectKeyToCreate;
                putRequest.ContentBody = "Default content.";
                S3Common.client.OnS3Response += Client_CreateObjectResponse;
                S3Common.client.PutObject(putRequest);
            }
        }

        /// <summary>
        /// The callback for the Create-Object action.
        /// </summary>
        /// <param name="result">The <see cref="IS3Response"/> object.</param>
        private void Client_CreateObjectResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_CreateObjectResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            PutObjectResponse response = args.Response as PutObjectResponse;

            if (null != response)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    ClearAllResults();
                    this.CreateObjectResult = response.IsRequestSuccessful ? "The object was created successfully." : "Error:\nError creating object.";
                    this.ObjectKeyToCreate = string.Empty;
                }
                );
                ListObjects();
            }
        }

        #endregion Create Object

        #region Get-ACLs-Object

        /// <summary>
        /// Event handler for the Get-ACL action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnGetACL_Click(object sender, RoutedEventArgs e)
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName))
                {
                    Regex rg = new Regex(_specialCharacters);
                    if (rg.IsMatch(this.BucketName.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.OwnerDisplayName = string.Empty;
                            this.OwnerId = string.Empty;
                            this.ACL.Clear();

                            this.ACLErrorMessageVisibility = System.Windows.Visibility.Visible;
                            this.GetACLErrorMessage = "Error:\nA Bucket name cannot have special characters.";
                        });
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.ACLErrorMessageVisibility = System.Windows.Visibility.Collapsed;
                        this.GetACLErrorMessage = string.Empty;
                        this.OwnerDisplayName = "Please wait...";
                        this.OwnerId = "Please wait...";

                        this.ACL.Clear();
                    });
                    GetACLRequest request = null;
                    if (string.IsNullOrEmpty(_bucketKey))
                        request = new GetACLRequest { BucketName = this.BucketName };
                    else
                        request = new GetACLRequest { BucketName = this.BucketName, Key = _bucketKey };
                    S3Common.client.OnS3Response += Client_GetACLResponse;

                    S3Common.client.GetACL(request);
                }
            }
        }

        /// <summary>
        /// The callback for the Get-ACL action.
        /// </summary>
        /// <param name="result">The <see cref="IS3Response"/> object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void Client_GetACLResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_GetACLResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            if (null == result)
            {
                return;
            }

            if (result.GetType() != typeof(GetACLResponse)) return;
            GetACLResponse response = args.Response as GetACLResponse;

            this.Dispatcher.BeginInvoke(() =>
            {
                this.OwnerDisplayName = response.Owner.DisplayName;
                this.OwnerId = response.Owner.Id;

                this.ACL.Clear();
                response.AccessControlList.Grants.ForEach(g => ACL.Add(g));
            }
            );
        }

        #endregion Get-ACLs-Object

        #region Set-ACLs-Object

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnSetACL_Click(object sender, RoutedEventArgs e)
        {
            S3AccessControlList list = new S3AccessControlList();
            list.AddGrant(new S3Grantee
            {
                CanonicalUser = new Amazon.S3.Model.Tuple<string, string> { First = "86ce77da65e907bab1f0e532c4ec6f9e3a55fcaa4b0cdfc492a5efb45e0f8617", Second = "Me" }
            }, S3Permission.FULL_CONTROL);

            list.Owner = new Owner { DisplayName = "Me", Id = "86ce77da65e907bab1f0e532c4ec6f9e3a55fcaa4b0cdfc492a5efb45e0f8617" };
            SetACLRequest request = new SetACLRequest { BucketName = this.BucketName, Key = this.BucketKey, ACL = list };

            S3Common.client.SetACL(request);
            S3Common.client.OnS3Response += client_S3WebResponse;
        }

        void client_S3WebResponse(object sender, ResponseEventArgs args)
        {
            S3Common.client.OnS3Response -= client_S3WebResponse;
            SetACLResponse res = args.Response as SetACLResponse;

            if (null != res)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.SetACLResult = res.IsRequestSuccessful ? "The ACL was set successfully." : "Error:\nError setting ACL.";
                });
            }
        }

        #endregion


        #region Copy-Object

        /// <summary>
        /// Event handler for the Copy-Object event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnCopyObject_Click(object sender, RoutedEventArgs e)
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName) && !string.IsNullOrEmpty(this.BucketNameDestination) &&
                    !string.IsNullOrEmpty(this.BucketKey) && !string.IsNullOrEmpty(this.BucketKeyDestination))
                {
                    Regex rg = new Regex(_specialCharacters);
                    if (rg.IsMatch(this.BucketName.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.CopyObjectResult = "Error:\nA 'Source Bucket name cannot have special characters.";
                        }
                        );
                        return;
                    }
                    if (rg.IsMatch(this.BucketNameDestination.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.CopyObjectResult = "Error:\nA 'Destination Bucket name cannot have special characters.";
                        }
                        );
                        return;
                    }
                    Regex rgForObject = new Regex(_specialChar_forObject);
                    if (rgForObject.IsMatch(this.BucketKey.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.CopyObjectResult = "Error:\nA Source Bucket Key cannot have special characters.";
                        }
                        );
                        return;
                    }
                    if (rg.IsMatch(this.BucketKeyDestination.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.CopyObjectResult = "Error:\nThe Destination Bucket Key cannot have special characters.";
                        }
                        );
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.CopyObjectResult = "Please wait...";
                    });

                    S3Common.client.OnS3Response += Client_CopyObjectResponse;
                    S3Common.client.CopyObject(new CopyObjectRequest
                    {
                        SourceBucket = this.BucketName,
                        DestinationBucket = this.BucketNameDestination,
                        SourceKey = this.BucketKey,
                        DestinationKey = this.BucketKeyDestination
                    });
                }
            }
        }

        private void Client_CopyObjectResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= (Client_CopyObjectResponse);
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            CopyObjectResponse response = args.Response as CopyObjectResponse;

            if (null != response)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.CopyObjectResult = response.IsRequestSuccessful ? "Successfully copied the source object." : "Error copying object.";
                });
            }

        }

        #endregion Copy-Object

        #region Get-Object-Metadata

        /// <summary>
        /// Event handler for the Get-object-Metadata action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnGetObjectMetadata_Click(object sender, RoutedEventArgs e)
        {
            if (null != S3Common.client)
            {
                if (!string.IsNullOrEmpty(this.BucketName))
                {
                    Regex rg = new Regex(_specialCharacters);
                    if (rg.IsMatch(this.BucketName.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.GetObjectMetadataResult = "Error:\nThe Bucket-Name cannot have special characters.";
                        });
                        return;
                    }

                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.GetObjectMetadataResult = "Please wait...";
                    });
                    GetObjectMetadataRequest request = null;
                    if (string.IsNullOrEmpty(this.BucketKey))
                        request = new GetObjectMetadataRequest { BucketName = this.BucketName };
                    else
                    {
                        Regex rgForObject = new Regex(_specialChar_forObject);
                        if (rgForObject.IsMatch(this.BucketKey.Trim()))
                        {
                            this.Dispatcher.BeginInvoke(() =>
                            {
                                this.GetObjectMetadataResult = "Error:\nThe Bucket-Key cannot have special characters.";
                            });
                            return;
                        }
                        request = new GetObjectMetadataRequest { BucketName = this.BucketName, Key = this.BucketKey };
                    }

                    if (null != request)
                    {
                        S3ResponseEventHandler<object, ResponseEventArgs> handler = null;
                        handler = delegate(object S3Sender, ResponseEventArgs args)
                        {
                            IS3Response result = args.Response;
                            S3Common.client.OnS3Response -= handler;
                            if (null != result)
                            {
                                S3ErrorResponse(result as AmazonS3Exception);
                                return;
                            }
                            GetObjectMetadataResponse response = result as GetObjectMetadataResponse;
                            if (null != response)
                            {
                                this.Dispatcher.BeginInvoke(() =>
                                {
                                    this.GetObjectMetadataResult = "Error getting Bucket/Object-Metadata from server.";
                                    return;
                                });

                                this.Dispatcher.BeginInvoke(() =>
                                {
                                    this.GetObjectMetadataResult = string.Empty;
                                    if (response.Metadata.Count <= 0)
                                    {
                                        this.GetObjectMetadataResult = "No metadata present for the Bucket/Object";
                                        return;
                                    }
                                    foreach (var key in response.Metadata.Keys)
                                        this.GetObjectMetadataResult += key + ":" + response.Metadata[key] + "\n";
                                });
                            }
                        };
                        S3Common.client.OnS3Response += handler;
                        S3Common.client.GetObjectMetadata(request);
                    }
                }
            }
        }

        /// <summary>
        /// The callback for the Get-Object-Metadata action.
        /// </summary>
        /// <param name="result">The <see cref="S3Response"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void Client_GetObjectMetadata(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= Client_GetObjectMetadata;

            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }

            GetObjectMetadataResponse response = args.Response as GetObjectMetadataResponse;
            if (null == response)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.GetObjectMetadataResult = "Error getting Bucket/Object-Metadata from server.";
                    return;
                });
            }

            this.Dispatcher.BeginInvoke(() =>
            {
                this.GetObjectMetadataResult = string.Empty;
                if (response.Metadata.Count <= 0)
                {
                    this.GetObjectMetadataResult = "No metadata present for the Bucket/Object";
                    return;
                }
                foreach (var key in response.Metadata.Keys)
                {
                    this.GetObjectMetadataResult += key + ":" + response.Metadata[key] + "\n";
                }
            }
            );
        }

        #endregion Get-Object-Metadata

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ObjectKeyToCreateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ObjectKeyToCreateEntered = !string.IsNullOrEmpty(ObjectKeyToCreateTextBox.Text);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ContentToUploadTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((string.IsNullOrEmpty(this.ContentToUploadTextBox.Text)) ||
                (string.IsNullOrEmpty(this.BucketKey)) ||
                (string.IsNullOrEmpty(this.BucketName)))
            {
                PutObjectButtonEnabled = false;
            }
            else
            {
                PutObjectButtonEnabled = true;
            }
        }

        private void ClearAllResults()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.GetObjectContent = string.Empty;
                this.PutObjectResult = string.Empty;
                this.CreateObjectResult = string.Empty;
                this.DeleteObjectResult = string.Empty;
                this.ACL.Clear();
                this.SetACLResult = string.Empty;
            });
        }
    }
}