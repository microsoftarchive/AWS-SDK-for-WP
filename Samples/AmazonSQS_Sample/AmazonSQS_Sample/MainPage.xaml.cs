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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Phone.Controls;
using Amazon.Runtime;

namespace AmazonSQS_Sample
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region Fields

        private AmazonSQS sqs;
        private AmazonSQSConfig _sqsConfig;
        private string _queueName = string.Empty;
        private string _queuePath = string.Empty;
        private string _queueUrl = string.Empty;
        private string _attributeValue;
        private string _messageDeletedNotification;
        private string _receivedMessage;
        private string _messageSentNotification;
        private string _messageToSend;
        private bool _haveQueueUrl; // state of the UI. Initially, app does not have a Queue Url.
        private bool _enableCreateQueueButton; // Only enable when have Queue Name field is not empty 
        private bool _enableSendMessageButton; // Only enable when have Queue Url and message text not empty 
        private string _specialCharacters = @"[<>?*\""/|%^()@#!&!$~{}|:;',.? \\]";
        private int _selectedQueueIndex = -1;
        private string _deletedMessage;

        private ObservableCollection<string> _queueNames = new ObservableCollection<string>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets a bool value indicating if the controls should be enabled for furthur actions.
        /// </summary>
        public bool HaveQueueUrl
        {
            get { return _haveQueueUrl; }
            set
            {
                _haveQueueUrl = value;
                OnPropertyChanged("HaveQueueUrl");

            }
        }

        /// <summary>
        /// Gets or sets a bool indicating if the Create Queue button should be enabled
        /// </summary>
        public bool EnableCreateQueueButton
        {
            get { return _enableCreateQueueButton; }
            set
            {
                _enableCreateQueueButton = value;
                OnPropertyChanged("EnableCreateQueueButton");
            }
        }

        /// <summary>
        /// Gets or sets a bool indicating if the Send Message button should be enabled
        /// </summary>
        public bool EnableSendMessageButton
        {
            get { return _enableSendMessageButton; }
            set
            {
                _enableSendMessageButton = value;
                OnPropertyChanged("EnableSendMessageButton");
            }
        }

        /// <summary>
        /// Gets or sets the collection of Queue-names.
        /// </summary>
        public ObservableCollection<string> QueueNames
        {
            get { return _queueNames; }
        }


        /// <summary>
        /// Gets or sets the message to send.
        /// </summary>
        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                _messageToSend = value;
                OnPropertyChanged("MessageToSend");
            }
        }

        /// <summary>
        /// Gets or sets the message-sent-notification.
        /// </summary>
        public string MessageSentNotification
        {
            get { return _messageSentNotification; }
            set
            {
                _messageSentNotification = value;
                OnPropertyChanged("MessageSentNotification");
            }
        }

        /// <summary>
        /// Gets or sets the received-message.
        /// </summary>
        public string ReceivedMessage
        {
            get { return _receivedMessage; }
            set
            {
                _receivedMessage = value;
                OnPropertyChanged("ReceivedMessage");
            }
        }

        /// <summary>
        /// Gets or sets the deleted-message.
        /// </summary>
        public string DeletedMessage
        {
            get { return _deletedMessage; }
            set
            {
                _deletedMessage = value;
                OnPropertyChanged("DeletedMessage");
            }
        }

        /// <summary>
        /// Gets or sets the message-deleted-notification value.
        /// </summary>
        public string MessageDeletedNotification
        {
            get { return _messageDeletedNotification; }
            set
            {
                _messageDeletedNotification = value;
                OnPropertyChanged("MessageDeletedNotification");
            }
        }

        /// <summary>
        /// Gets the ServiceURL of the Queue - e.g. Example Queue URL https://queue.amazonaws.com
        /// </summary>
        /// <remarks>
        /// The service URL is obtained from the AmazonSQSConfig object 
        /// </remarks>
        public string QueueServiceURL
        {
            get
            {
                if (_sqsConfig == null)
                {
                    _sqsConfig = new AmazonSQSConfig();
                }
                return _sqsConfig.ServiceURL;
            }
        }

        /// <summary>
        /// Gets or sets the Path of the Queue - the part of the Url after the ServiceURL and before the queue name.
        /// </summary>
        /// <remarks>
        /// For example if the queue URL is https://queue.amazonaws.com/421187735901/ZZZ_Queue002  
        /// then the Queue Path is 421187735901
        /// </remarks>
        public string QueuePath
        {
            get { return _queuePath; }
            set
            {
                _queuePath = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the Queue.
        /// </summary>
        /// <remarks>
        /// For example if the queue URL is https://queue.amazonaws.com/421187735901/ZZZ_Queue002  
        /// then the name of the Queue is ZZZ_Queue002
        /// </remarks>
        public string QueueName
        {
            get { return _queueName; }
            set
            {
                _queueName = value;
                OnPropertyChanged("QueueName");
            }
        }

        /// <summary>
        /// Gets or sets the Url of the Queue.
        /// </summary>
        public string QueueUrl
        {
            get { return _queueUrl; }
            set
            {
                _queueUrl = value;
                OnPropertyChanged("QueueUrl");
            }
        }

        /// <summary>
        /// Gets or sets the value of the Attribute. (As of now the attribute is 'VisibilityTimeout'.
        /// </summary>
        public string AttributeValue
        {
            get { return _attributeValue; }
            set
            {
                _attributeValue = value;
                OnPropertyChanged("AttributeValue");
            }
        }

        /// <summary>
        /// Gets or sets the index of the selected queue.
        /// </summary>
        public int SelectedQueueIndex
        {
            get { return _selectedQueueIndex; }
            set
            {
                _selectedQueueIndex = value;
                OnPropertyChanged("SelectedQueueIndex");
            }
        }

        #endregion Properties

        #region Default Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = this;

            AWSCredentials credentials = new EnvironmentAWSCredentials();
            sqs = new AmazonSQSClient(credentials);

            this.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (string.Compare(e.PropertyName, "SelectedQueueIndex", System.StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (_selectedQueueIndex > -1)
                    {
                        this.QueueName = this.QueueNames[_selectedQueueIndex];
                        this.HaveQueueUrl = true;
                    }
                }
            };
        }

        #endregion

        #region Error Handler

        void SQSErrorResponse(AmazonSQSException error)
        {
            StringBuilder errorBuilder = new StringBuilder();
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "ERROR CODE: {0}", error.ErrorCode));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "ERROR TYPE: {0}", error.ErrorType));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "INNER EXCEPTION: {0}", error.InnerException));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "MESSAGE: {0}", error.Message));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "REQUEST ID: {0}", error.RequestId));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "STATUS CODE: {0}", error.StatusCode));
            errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "\nERROR RESPONSE:\n {0}", error.XML));

            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show(errorBuilder.ToString(), "Error Occured", MessageBoxButton.OK);
            });

        }

        #endregion

        #region Create Queue

        /// <summary>
        /// Creates a Queue.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnCreateQueue_Click(object sender, RoutedEventArgs e)
        {
            CreateQueueRequest sqsRequest = new CreateQueueRequest();
            if (!string.IsNullOrEmpty(QueueNameTextBox.Text))
            {
                Regex rg = new Regex(_specialCharacters);
                if (rg.IsMatch(this.QueueName.Trim()))
                {
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            this.QueueUrl = "Error:\nA Queue name cannot have special characters.";
                        }
                    );
                    return;
                }

                sqsRequest.QueueName = this.QueueName.Trim();

                this.Dispatcher.BeginInvoke(() =>
                    {
                        this.QueueUrl = "Please wait...";
                    });
                sqs.OnSQSResponse += GetCreateQueueResponse;
                sqs.CreateQueue(sqsRequest);
            }
        }

        /// <summary>
        /// The callback for the CreateQueue action.
        /// </summary>
        /// <param name="sqsResponse">The <see cref="ISQSResponse"/> instance.</param>
        void GetCreateQueueResponse(object sender, ResponseEventArgs args)
        {
            AmazonSQSException sqsResponse = args.Response as AmazonSQSException;
            sqs.OnSQSResponse -= GetCreateQueueResponse;

            if (null != sqsResponse)
            {
                SQSErrorResponse(sqsResponse);
                return;
            }
            CreateQueueResponse response = args.Response as CreateQueueResponse;
            if (null == response) return;

            this.Dispatcher.BeginInvoke(() =>
                {
                    this.QueueUrl = "Url of Queue:\n" + response.CreateQueueResult.QueueUrl;
                    this.HaveQueueUrl = true;
                });

            if (string.IsNullOrEmpty(QueuePath))
            {
                string[] parts = response.CreateQueueResult.QueueUrl.Split('/');
                string queuePath = parts[parts.Length - 2];
                QueuePath = queuePath;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void QueueNameTextBox_TextChanged
            (object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EnableCreateQueueButton = !string.IsNullOrEmpty(QueueNameTextBox.Text);
        }

        #endregion

        #region List Queue

        /// <summary>
        /// Envent handler for the List-Queues action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnListQueue_Click(object sender, RoutedEventArgs e)
        {
            ListQueuesRequest listQueue = new ListQueuesRequest();

            this.Dispatcher.BeginInvoke(() =>
            {
                this.QueueNames.Clear();
                this.QueueNames.Add("Please wait...");
            });
            sqs.OnSQSResponse += GetListQueueResponse;
            sqs.ListQueues(listQueue);
        }

        /// <summary>
        /// The callback for the List-Queues action.
        /// </summary>
        /// <param name="sqsResponse">The <see cref="ISQSResponse"/>.</param>
        void GetListQueueResponse(object sender, ResponseEventArgs args)
        {
            AmazonSQSException sqsResponse = args.Response as AmazonSQSException;
            sqs.OnSQSResponse -= GetListQueueResponse;
            if (null != sqsResponse)
            {
                SQSErrorResponse(sqsResponse);
                this.Dispatcher.BeginInvoke(() =>
                        {
                            this.QueueNames.Clear();
                        });
                return;
            }
            ListQueuesResponse response = args.Response as ListQueuesResponse;
            if (null != response)
            {
                ListQueuesResult result = response.ListQueuesResult;
                if (null != result)
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        this.QueueNames.Clear();
                        System.Collections.Generic.List<string> queueList = result.QueueUrl.ToList();
                        foreach (string queueListEntry in queueList)
                        {
                            string[] parts = queueListEntry.Split('/');
                            string queueName = parts[parts.Length - 1];
                            QueueNames.Add(queueName);

                            if (string.IsNullOrEmpty(QueuePath))
                            {
                                string queuePath = parts[parts.Length - 2];
                                QueuePath = queuePath;
                            }
                        }
                    });
                }
            }
        }

        #endregion

        #region Send Message

        /// <summary>
        /// Envent handler for the Send-Message action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessageRequest request = new SendMessageRequest();

            request.QueueUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}",
                QueueServiceURL, QueuePath, QueueName.Trim());

            request.MessageBody = this.MessageToSend;

            this.Dispatcher.BeginInvoke(() =>
            {
                this.MessageSentNotification = "Please wait...";
            });
            sqs.OnSQSResponse += GetSendMessageResponse;
            sqs.SendMessage(request);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void SendMessageTextBox_TextChanged
            (object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            EnableSendMessageButton = !string.IsNullOrEmpty(SendMessageTextBox.Text);
        }

        /// <summary>
        /// The callback for the Send-Message action.
        /// </summary>
        /// <param name="sqsResponse">The <see cref="ISQSResponse"/> instance.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void GetSendMessageResponse(object sender, ResponseEventArgs args)
        {
            AmazonSQSException sqsResponse = args.Response as AmazonSQSException;
            sqs.OnSQSResponse -= GetSendMessageResponse;
            if (null != sqsResponse)
            {
                SQSErrorResponse(sqsResponse as AmazonSQSException);
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.MessageSentNotification = "Error sending message";
                });
                return;
            }
            SendMessageResponse response = args.Response as SendMessageResponse;
            if (response != null)
            {
                this.Dispatcher.BeginInvoke(() => this.MessageSentNotification = "Message sent successfully");
            }
        }

        #endregion

        #region Receive Message

        /// <summary>
        /// Envent handler for the Receive-Message action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnReceiveMessage_Click(object sender, RoutedEventArgs e)
        {
            ReceiveMessageRequest request = new ReceiveMessageRequest();
            request.QueueUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}",
                QueueServiceURL, QueuePath, QueueName.Trim());

            this.Dispatcher.BeginInvoke(() =>
            {
                this.ReceivedMessage = "Please wait...";
            });
            sqs.OnSQSResponse += GetReceiveMessageResponse;
            sqs.ReceiveMessage(request);
        }

        /// <summary>
        /// The callback for the Receive-Message action.
        /// </summary>
        /// <param name="sqsResponse">The <see cref="ISQSResponse"/> instance.</param>
        private void GetReceiveMessageResponse(object sender, ResponseEventArgs args)
        {
            AmazonSQSException sqsResponse = args.Response as AmazonSQSException;
            sqs.OnSQSResponse -= GetReceiveMessageResponse;
            if (null != sqsResponse)
            {
                SQSErrorResponse(sqsResponse);
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.ReceivedMessage = "Error receiving message.";
                });
                return;
            }
            ReceiveMessageResponse response = args.Response as ReceiveMessageResponse;
            if (null == response) return;

            if (response.ReceiveMessageResult.Message.Count > 0)
            {
                this.Dispatcher.BeginInvoke(() =>
                    this.ReceivedMessage = string.Format(CultureInfo.InvariantCulture, "Queue's Message: {0}", response.ReceiveMessageResult.Message[0].Body));
            }
            else
                this.Dispatcher.BeginInvoke(() =>
                    this.ReceivedMessage = string.Format(CultureInfo.InvariantCulture, "The server returned an Empty message."));
        }

        #endregion

        #region Delete Queue

        /// <summary>
        /// Envent handler for the Delete-Queue action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnDeleteQueue_Click(object sender, RoutedEventArgs e)
        {
            DeleteQueueRequest request = new DeleteQueueRequest();

            request.QueueUrl = (string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}",
                    QueueServiceURL, QueuePath, QueueName.Trim()));

            this.Dispatcher.BeginInvoke(() =>
            {
                this.MessageDeletedNotification = "Please wait...";
            });
            sqs.OnSQSResponse += GetDeleteQueueResponse;
            sqs.DeleteQueue(request);
        }

        /// <summary>
        /// The callback for the Delete-Queue action.
        /// </summary>
        /// <param name="sqsResponse">The <see cref="ISQSResponse"/> instance.</param>
        private void GetDeleteQueueResponse(object sender, ResponseEventArgs args)
        {
            AmazonSQSException sqsResponse = args.Response as AmazonSQSException;
            sqs.OnSQSResponse -= GetDeleteQueueResponse;
            if (null != sqsResponse)
            {
                SQSErrorResponse(sqsResponse);
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.MessageDeletedNotification = "Error Deleting Queue.";
                });
                return;
            }

            DeleteQueueResponse response = args.Response as DeleteQueueResponse;
            if (response != null)
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    ClearAllProperties(false);
                    this.MessageDeletedNotification = "Queue Deleted Successfully. The queue list is refreshed.";
                    //Relist the queues.
                    btnListQueue_Click(this, null);
                });
            }
        }

        #endregion

        #region Get Attribute

        /// <summary>
        /// Envent handler for the Get-Queue-Attribute action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnGetQueueAttribute_Click(object sender, RoutedEventArgs e)
        {
            GetQueueAttributesRequest request = new GetQueueAttributesRequest();
            request.AttributeName.Add("VisibilityTimeout");

            request.QueueUrl = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/{2}",
                            QueueServiceURL, QueuePath, QueueName.Trim());

            this.Dispatcher.BeginInvoke(() =>
            {
                this.AttributeValue = "Please wait...";
            });

            sqs.OnSQSResponse += GetQueueAttributeResponse;
            sqs.GetQueueAttributes(request);

        }

        /// <summary>
        /// The callback for the Get-Queue-Attribute action.
        /// </summary>
        /// <param name="result">The <see cref="ISQSResponse"/> instance.</param>
        void GetQueueAttributeResponse(object sender, ResponseEventArgs args)
        {
            AmazonSQSException sqsResponse = args.Response as AmazonSQSException;
            sqs.OnSQSResponse -= GetQueueAttributeResponse;
            if (null != sqsResponse)
            {
                SQSErrorResponse(sqsResponse);
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.AttributeValue = "Error getting attribute.";
                });
                return;
            }
            GetQueueAttributesResponse response = args.Response as GetQueueAttributesResponse;
            if (null == response) return;

            if (response.GetQueueAttributesResult.Attribute.Count > 0)
            {
                this.Dispatcher.BeginInvoke(() => this.AttributeValue =
                    response.GetQueueAttributesResult.Attribute[0].Value);
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// Event handler for the clear action.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/>.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            //Clear all the buffers.
            ClearAllProperties(true);
        }

        private void ClearAllProperties(bool isExplicit)
        {
            this.QueueName = string.Empty;
            this.QueueUrl = string.Empty;
            this.MessageSentNotification = string.Empty;
            this.MessageDeletedNotification = string.Empty;
            this.ReceivedMessage = string.Empty;
            this.MessageToSend = string.Empty;
            this.AttributeValue = string.Empty;
            if (isExplicit)
                this.QueueNames.Clear();
            this.HaveQueueUrl = false;
        }

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
    }
}