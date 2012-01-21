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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Phone.Controls;

namespace AmazonS3_Sample
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region Fields

        private string _specialCharacters = @"[<>?*\""/|%^()@#!&!$~{}|:;',? \\]";
        private string _specialChar_forObject = @"[<>?*\""/|%^()@#!&!$~{}|:;',?\\]";

        private ObservableCollection<string> _bucketNames = new ObservableCollection<string>();
        private ObservableCollection<S3Grant> _acls = new ObservableCollection<S3Grant>();
        private ObservableCollection<S3Object> _bucketObjects = new ObservableCollection<S3Object>();
        private string _bucketName;
        private bool _bucketNameSelected; // state of the UI. True when bucket name selected in list
        private string _bucketNameDestination;
        private string _bucketKey;
        private bool _objectInBucketSelected;
        private string _bucketKeyDestination;
        private string _contentOfBody;
        private string _ownerDisplayName;
        private string _createBucketName;
        private string _selectedBucketName;
        private string _ownerId;
        private int _selectedIndex = -1;
        private int _selectedIndexOfObjects = -1;
        private int _selectedIndexDestinationBucket = -1;
        private Visibility _bucketObjectsMessageVisibility = Visibility.Collapsed;
        private string _getACLErrorMessage;
        private string _bucketObjectsMessage;
        private string _deleteObjectMessage;
        private string _copyObjectResult;
        private int _noOfObjects;
        private string _getBucketResult = string.Empty;
        private string _deleteBucketText = string.Empty;

        #endregion Fields

        #region Properties

        public string DeleteBucketText
        {
            get { return _deleteBucketText; }
            set
            {
                _deleteBucketText = value;
                OnPropertyChanged("DeleteBucketText");
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
        /// Gets or sets the message of the Delete-Object action.
        /// </summary>
        public string DeleteObjectMessage
        {
            get { return _deleteObjectMessage; }
            set
            {
                _deleteObjectMessage = value;
                OnPropertyChanged("DeleteObjectMessage");
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
        /// Gets or sets the selected index of the Key in the List-Objects listbox.
        /// </summary>
        public int SelectedIndexOfObjects
        {
            get { return _selectedIndexOfObjects; }
            set
            {
                _selectedIndexOfObjects = value;
                OnPropertyChanged("SelectedIndexOfObjects");
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

        #endregion Properties

        #region Constructor

        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;

            S3Common.client = Amazon.AWSClientFactory.CreateAmazonS3Client();

            this.PropertyChanged += new PropertyChangedEventHandler((object sender, PropertyChangedEventArgs e) =>
                {
                    if (string.Compare(e.PropertyName, "SelectedIndex", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (SelectedIndex < 0)
                        {
                            this.BucketNameSelected = false;
                            return;
                        }
                        //Update the bucket-name with the currently selected one.
                        this.BucketName = _bucketNames[SelectedIndex];
                        this.SelectedBucketName = _bucketNames[SelectedIndex];
                        this.BucketNameSelected = true;
                    }
                    else if (string.Compare(e.PropertyName, "SelectedIndexOfObjects", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (SelectedIndexOfObjects < 0)
                        {
                            this.ObjectInBucketSelected = false;
                            return;
                        }

                        //Update the Bucket-Key.
                        this.BucketKey = _bucketObjects[SelectedIndexOfObjects].Key;
                        this.ObjectInBucketSelected = true;
                    }
                    else if (string.Compare(e.PropertyName, "SelectedIndexDestinationBucket", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (SelectedIndexDestinationBucket < 0) return;

                        this.BucketNameDestination = _bucketNames[SelectedIndexDestinationBucket];
                    }
                });
        }

        #endregion Constructor

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void DisplayError(string errorMessage)
        {
            if (!string.IsNullOrEmpty(errorMessage))
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(errorMessage);
                });
            }
        }

        #endregion

        #region Bucket APIs

        #region List Bucket

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ListBuckets_Click(object sender, RoutedEventArgs e)
        {
            DeleteBucketResponseText.Text = string.Empty;
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
                S3ErrorResponse(result);
                return;
            }

            ListBucketsResponse response = args.Response as ListBucketsResponse;
            this.Dispatcher.BeginInvoke(() =>
                {
                    BucketNames.Clear();
                    this.BucketNameSelected = false;
                    response.Buckets.ToList().ForEach(b => BucketNames.Add(b.BucketName));
                });


        }

        #endregion

        #region Create Bucket

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void CreateBucket_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(CreateBucketName))
            {
                S3Common.client.OnS3Response += CreateBucketWebResponse;

                PutBucketRequest request = new PutBucketRequest();
                request.BucketName = CreateBucketName;

                S3Common.client.PutBucket(request);
                return;
            }
            DisplayError("Please Provide the Bucket Name");
        }

        void CreateBucketWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception response = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= CreateBucketWebResponse;
            if (null != response)
            {
                S3ErrorResponse(response);
                return;
            }
            PutBucketResponse putBucketResponse = args.Response as PutBucketResponse;
            if (null != putBucketResponse)
            {
                if (!string.IsNullOrEmpty(putBucketResponse.AmazonId2) &&
                    (!string.IsNullOrEmpty(putBucketResponse.RequestId)))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        GetBucketResult = string.Format(CultureInfo.InvariantCulture, "{0} bucket created Successfully", CreateBucketName);
                    });
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Unsuccessul Creation of Bucket: {0}", CreateBucketName);
                });
            }
        }

        #endregion

        #region Delete Bucket

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void DeleteBucket_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(SelectedBucketName))
            {
                S3Common.client.OnS3Response += DeleteBucketWebResponse;
                DeleteBucketRequest deleteRequest = new DeleteBucketRequest();
                deleteRequest.BucketName = SelectedBucketName;
                S3Common.client.DeleteBucket(deleteRequest);
                return;
            }
            DisplayError("Please select the Bucket Name to be deleted from the list");
        }

        void DeleteBucketWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception response = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= DeleteBucketWebResponse;
            if (null != response)
            {
                S3ErrorResponse(response);
                return;
            }

            DeleteBucketResponse deleteBucketResult = args.Response as DeleteBucketResponse;
            if (null != deleteBucketResult)
            {
                if (!string.IsNullOrEmpty(deleteBucketResult.AmazonId2) &&
                    (!string.IsNullOrEmpty(deleteBucketResult.RequestId)))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        DeleteBucketText = string.Format(CultureInfo.InvariantCulture, "{0} bucket deleted Successfully", SelectedBucketName);
                        this.BucketNameSelected = false;
                    });
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    DeleteBucketText = string.Format(CultureInfo.InvariantCulture, "Unsuccessful deletetion of Bucket: {0}", SelectedBucketName);
                    this.BucketNameSelected = false;
                });
            }

        }

        #endregion

        #region Put Bucket Policy

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void PutBucketPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3Common.client.OnS3Response += PutBucketPolicyWebResponse;
                PutBucketPolicyRequest putRequest = new PutBucketPolicyRequest();
                putRequest.BucketName = SelectedBucketName;
                putRequest.Policy = GetPolicy(SelectedBucketName);
                S3Common.client.PutBucketPolicy(putRequest);
            }
            catch (Exception ex)
            {
                S3Common.client.OnS3Response -= PutBucketPolicyWebResponse;
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });

            }
        }

        void PutBucketPolicyWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= PutBucketPolicyWebResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            PutBucketPolicyResponse policyResponse = args.Response as PutBucketPolicyResponse;
            if (null != policyResponse)
            {
                if (!string.IsNullOrEmpty(policyResponse.AmazonId2) &&
                    (!string.IsNullOrEmpty(policyResponse.RequestId)))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Policy Created Successfully for Bucket {0}", SelectedBucketName);
                    });
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Unsuccessfully creation of Policy for Bucket {0}", SelectedBucketName);
                });
            }

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        static string GetPolicy(string bucketName)
        {
            return "{\"Version\": \"2008-10-17\",\"Id\": \"aaaa-bbbb-cccc-dddd\",\"Statement\" :" +
                "[{\"Effect\": \"Allow\",\"Sid\": \"1\",\"Principal\": {\"AWS\": \"*\"}," +
                "\"Action\": [\"s3:*\"],\"Resource\": \"arn:aws:s3:::" + bucketName + "/*\"}]}";
        }

        #endregion

        #region Get Bucket Policy

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void GetBucketPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3Common.client.OnS3Response += GetBucketPolicyWebResponse;
                GetBucketPolicyRequest request = new GetBucketPolicyRequest();
                request.BucketName = SelectedBucketName;
                S3Common.client.GetBucketPolicy(request);
            }
            catch (Exception ex)
            {
                S3Common.client.OnS3Response -= GetBucketPolicyWebResponse;
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });

            }
        }

        void GetBucketPolicyWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception result = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= GetBucketPolicyWebResponse;
            if (null != result)
            {
                S3ErrorResponse(result);
                return;
            }
            GetBucketPolicyResponse policyResponse = args.Response as GetBucketPolicyResponse;
            if (null != policyResponse)
            {
                if (!string.IsNullOrEmpty(policyResponse.AmazonId2) &&
                   (!string.IsNullOrEmpty(policyResponse.RequestId)))
                {
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            GetBucketResult = policyResponse.Policy;
                        });
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Error Processing GetPolicy Request");
                });
            }
        }

        #endregion

        #region Get Bucket Location

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void GetBucketLocationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3Common.client.OnS3Response += GetBucketLocationWebResponse;
                GetBucketLocationRequest request = new GetBucketLocationRequest();
                request.BucketName = SelectedBucketName;

                S3Common.client.GetBucketLocation(request);
            }
            catch (Exception ex)
            {
                S3Common.client.OnS3Response -= GetBucketLocationWebResponse;
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });

            }
        }

        void GetBucketLocationWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception response = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= GetBucketLocationWebResponse;
            if (null != response)
            {
                S3ErrorResponse(response);
                return;
            }
            GetBucketLocationResponse locationResponse = args.Response as GetBucketLocationResponse;
            if (null != locationResponse)
            {
                if (!string.IsNullOrEmpty(locationResponse.AmazonId2) &&
                    (!string.IsNullOrEmpty(locationResponse.RequestId)))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        GetBucketResult = String.Format(CultureInfo.InvariantCulture, "The Location of {0} Bucket: {1}", SelectedBucketName,
                            locationResponse.Location);
                    });
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Error Processing GetBucketLocation Request");
                });
            }
        }

        #endregion

        #region Delete Bucket Policy

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void DeleteBucketPolicyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3Common.client.OnS3Response += DeleteBucketPolicyWebResponse;
                DeleteBucketPolicyRequest request = new DeleteBucketPolicyRequest();
                request.BucketName = SelectedBucketName;
                S3Common.client.DeleteBucketPolicy(request);
            }
            catch (Exception ex)
            {
                S3Common.client.OnS3Response -= DeleteBucketPolicyWebResponse;
                this.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });

            }
        }

        void DeleteBucketPolicyWebResponse(object sender, ResponseEventArgs args)
        {
            AmazonS3Exception response = args.Response as AmazonS3Exception;
            S3Common.client.OnS3Response -= DeleteBucketPolicyWebResponse;
            if (null != response)
            {
                S3ErrorResponse(response);
                return;
            }
            DeleteBucketPolicyResponse policyResponse = args.Response as DeleteBucketPolicyResponse;
            if (null != policyResponse)
            {
                if (!string.IsNullOrEmpty(policyResponse.AmazonId2) &&
                   (!string.IsNullOrEmpty(policyResponse.RequestId)))
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Bucket Policy Deleted Successfully for Bucket: {0}", SelectedBucketName);
                    });
                }
            }
            else
            {
                GetBucketResult = string.Format(CultureInfo.InvariantCulture, "Unsuccessful Deletion of Policy for Bucket: {0}", SelectedBucketName);
            }
        }

        #endregion

        #endregion

        #region Copy-Object

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
                    if (rg.IsMatch(this.BucketKey.Trim()))
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.CopyObjectResult = "Error:\nA Source Bucket Key cannot have special characters.";
                        }
                        );
                        return;
                    }
                    Regex rgForObject = new Regex(_specialChar_forObject);
                    if (rgForObject.IsMatch(this.BucketKeyDestination.Trim()))
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
    }

    /// <summary>
    /// S3Common is a static class to encompass the AmazonS3 client object making it accessible to other pages.
    /// </summary>
    public static class S3Common
    {
        internal static AmazonS3 client;
    }

}
