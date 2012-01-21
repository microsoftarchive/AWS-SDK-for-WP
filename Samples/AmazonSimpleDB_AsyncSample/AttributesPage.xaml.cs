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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Amazon.SimpleDB.Model;
using Amazon.SimpleDB;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AmazonSimpleDB_AsyncSample
{
    public partial class AttributesPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        #region Fields and Constants

        private ObservableCollection<Amazon.SimpleDB.Model.Attribute> _attributes = new ObservableCollection<Amazon.SimpleDB.Model.Attribute>();
        private ObservableCollection<string> _domainNames = new ObservableCollection<string>();
        private ObservableCollection<string> _itemsNameList = new ObservableCollection<string>();
        private string _domainName = string.Empty;
        private string _itemName;
        private string _fetchingAttributeMessage;
        private string _attributesForQuery;
        private bool _isDomainSelected;
        private bool _isSingleOperation;
        private string _selectQueryMessage;
        private ObservableCollection<AttributeAndValue> _selectResultAttributes = new ObservableCollection<AttributeAndValue>();
        private string _selectQuery;
        private int _selectedIndexDomain = -1;
        private int _attributeSelectedIndex = -1;
        private int _selectedIndexItem = -1;
        private string _attributesAndValuesToPut;
        private string _someMessage;
        private bool _isBatchOperation;
        private string _batchPutMessage;
        private bool _acceptAttributesAndValues;
        private string _batchDeleteMessage;
        private string _itemsNameMessage;

        #endregion Fields and Constants

        #region Properties

        /// <summary>
        /// Gets or sets the messages for fetching items names operation.
        /// </summary>
        public string ItemsNameMessage
        {
            get { return _itemsNameMessage; }
            set
            {
                _itemsNameMessage = value;
                OnPropertyChanged("ItemsNameMessage");
            }
        }

        /// <summary>
        /// Gets or sets the messages for batch delete operation.
        /// </summary>
        public string BatchDeleteMessage
        {
            get { return _batchDeleteMessage; }
            set
            {
                _batchDeleteMessage = value;
                OnPropertyChanged("BatchDeleteMessage");
            }
        }

        /// <summary>
        /// Gets or sets the messages for batch put operation.
        /// </summary>
        public string BatchPutMessage
        {
            get { return _batchPutMessage; }
            set
            {
                _batchPutMessage = value;
                OnPropertyChanged("BatchPutMessage");
            }
        }

        /// <summary>
        /// Gets or sets a bool value indicating to accept or not to accept the attributes and their values.
        /// </summary>
        public bool AcceptAttributesAndValues
        {
            get { return _acceptAttributesAndValues; }
            set
            {
                _acceptAttributesAndValues = value;
                OnPropertyChanged("AcceptAttributesAndValues");
            }
        }

        /// <summary>
        /// Gets or sets a bool value indicating if Item name is provided and is single operation. For eg; PutAttribute, DeleteAttribute, etc.
        /// </summary>
        public bool IsSingleOperation
        {
            get { return _isSingleOperation; }
            set
            {
                _isSingleOperation = value;
                OnPropertyChanged("IsSingleOperation");
            }
        }

        /// <summary>
        /// Gets or sets a bool to indicate if a domain is selected.
        /// </summary>
        public bool IsDomainSelected
        {
            get { return _isDomainSelected; }
            set
            {
                _isDomainSelected = value;
                OnPropertyChanged("IsDomainSelected");
            }
        }

        /// <summary>
        /// Gets or sets the collection of Domain names.
        /// </summary>
        public ObservableCollection<string> DomainNameList
        {
            get { return _domainNames; }
            //set
            //{
            //    _domainNames = value;
            //    OnPropertyChanged("DomainNameList");
            //}
        }

        /// <summary>
        /// Gets or sets the collection of items names.
        /// </summary>
        public ObservableCollection<string> ItemsNameList
        {
            get { return _itemsNameList; }
            //set
            //{
            //    _itemsNameList = value;
            //    OnPropertyChanged("ItemsNameList");
            //}
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
        /// Gets or sets the collection of <see cref="Amazon.SimpleDB.Model.Attribute"/>.
        /// </summary>
        public ObservableCollection<Amazon.SimpleDB.Model.Attribute> Attributes
        {
            get { return _attributes; }
        }


        /// <summary>
        /// Gets or sets the fetching attribute message.
        /// </summary>
        public string FetchingOrDeletingAttributeMessage
        {
            get { return _fetchingAttributeMessage; }
            set
            {
                _fetchingAttributeMessage = value;
                OnPropertyChanged("FetchingOrDeletingAttributeMessage");
            }
        }

        /// <summary>
        /// Gets or sets the attributes the the item.
        /// </summary>
        public string AttributesForQuery
        {
            get { return _attributesForQuery; }
            set
            {
                _attributesForQuery = value;
                OnPropertyChanged("AttributesForQueury");
            }
        }

        /// <summary>
        /// Gets or sets the message displayed for SELECT query execution.
        /// </summary>
        public string SelectQueryMessage
        {
            get { return _selectQueryMessage; }
            set
            {
                _selectQueryMessage = value;
                OnPropertyChanged("SelectQueryMessage");
            }
        }

        /// <summary>
        /// Gets or sets the collection of SELECT query results.
        /// </summary>
        public ObservableCollection<AttributeAndValue> SelectResultAttributes
        {
            get { return _selectResultAttributes; }
            //set { _selectResultAttributes = value; }
        }

        /// <summary>
        /// Gets or sets the SELECT query.
        /// </summary>
        public string SelectQuery
        {
            get { return _selectQuery; }
            set
            {
                _selectQuery = value;
                OnPropertyChanged("SelectQuery");
            }
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int SelectedIndexDomain
        {
            get { return _selectedIndexDomain; }
            set
            {
                _selectedIndexDomain = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        //
        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndexItem
        {
            get { return _selectedIndexItem; }
            set
            {
                _selectedIndexItem = value;
                OnPropertyChanged("SelectedIndexItem");
            }
        }

        /// <summary>
        /// Gets or sets the message to display for Put action.
        /// </summary>
        public string SomeMessage
        {
            get { return _someMessage; }
            set
            {
                _someMessage = value;
                OnPropertyChanged("SomeMessage");
            }
        }

        /// <summary>
        /// Gets or sets the selected index for the list of attribute.
        /// </summary>
        public int AttributeSelectedIndex
        {
            get { return _attributeSelectedIndex; }
            set
            {
                _attributeSelectedIndex = value;
                OnPropertyChanged("AttributeSelectedIndex");
            }
        }

        /// <summary>
        /// Gets or sets the attributes and values to put.
        /// </summary>
        public string AttributesAndValuesToPut
        {
            get { return _attributesAndValuesToPut; }
            set
            {
                _attributesAndValuesToPut = value;
                OnPropertyChanged("AttributesAndValuesToPut");
            }
        }

        /// <summary>
        /// Gets or sets a bool value indicating if Item-name is provided and is batch operation. For eg; PutBatchAttribute, DeleteBatchAttribute, etc.
        /// </summary>
        public bool IsBatchOperation
        {
            get { return _isBatchOperation; }
            set
            {
                _isBatchOperation = value;
                OnPropertyChanged("IsBatchOperation");
            }
        }

        #endregion Properties

        #region CTor

        public AttributesPage()
        {
            InitializeComponent();
            this.DataContext = this;

            this.ItemsNameMessage = "No of Items: ";
            this.SomeMessage = string.Empty;
            PropertyChanged += ((object sender, PropertyChangedEventArgs e) =>
            {
                #region ItemName
                if (string.Compare(e.PropertyName, "ItemName", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //Enable or disable putting and getting attributes actions.
                    if (string.IsNullOrEmpty(_itemName))
                    {
                        this.IsSingleOperation = false;

                        //Enable to accept the attributes and values.
                        this.AcceptAttributesAndValues = false;
                    }
                    else
                    {
                        //Determine if it is batch operation or single operation.
                        if (_itemName.Contains(','))
                        {
                            this.IsBatchOperation = true;
                            this.IsSingleOperation = false;
                        }
                        else
                        {
                            this.IsBatchOperation = false;
                            this.IsSingleOperation = true;
                        }
                        //Create the SQL query.
                        string itemQuery = "'";
                        foreach (var item in GetItemNames(_itemName))
                        {
                            itemQuery += item + "','";
                        }
                        //Remove trailing characters.
                        itemQuery = itemQuery.Substring(0, itemQuery.Length - 2);

                        this.SelectQuery = "SELECT *  FROM " + this.DomainName + " WHERE ItemName() IN (" + itemQuery + ")";

                        //Enable to accept the attributes and values.
                        this.AcceptAttributesAndValues = true;
                    }
                    //Update the bucket-name with the currently selected one.
                    if (string.IsNullOrEmpty(_itemName))
                        this.AttributesAndValuesToPut = string.Empty;
                    else
                        this.AttributesAndValuesToPut = "att1-val1, att2-val2, att3-val3, att4-val4";
                    this.IsDomainSelected = true;
                }
                #endregion ItemName

                #region SelectedIndex
                else if (string.Compare(e.PropertyName, "SelectedIndex", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (_selectedIndexDomain >= 0)
                        //Update the bucket-name with the currently selected one.
                        this.DomainName = this.DomainNameList[SelectedIndexDomain];
                    this.IsDomainSelected = true;
                }
                #endregion SelectedIndex

                #region SelectedIndexItem

                else if (string.Compare(e.PropertyName, "SelectedIndexItem", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (_selectedIndexItem >= 0)
                        //Update the bucket-name with the currently selected one.
                        this.ItemName = this.ItemsNameList[_selectedIndexItem];
                }

                #endregion SelectedIndexItem

                #region DomainName

                else if (string.Compare(e.PropertyName, "DomainName", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //Pupulate the list of items for this domain.
                    this.PopulateItemsNamesList(_domainName);
                }
                #endregion DomainName

                #region AttributesAndValuesToPut

                else if (string.Compare(e.PropertyName, "AttributesAndValuesToPut", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    //Extract the attributes only.
                    char[] sep = new char[] { ',' };
                    string[] nameValues = this.AttributesAndValuesToPut.Split(sep);
                    if (nameValues.Length <= 0)
                    {
                        this.AttributesForQuery = string.Empty;
                    }
                    else
                    {
                        sep = new char[] { '-' };
                        string temp = string.Empty;
                        int index = 0;
                        foreach (var item in nameValues)
                        {
                            if (index == nameValues.Length - 1)
                            {
                                temp += item.Trim();
                                break;
                            }
                            temp += item.Trim() + "-";
                            index++;
                        }
                        string[] attributesAndValues = temp.Split(sep);
                        index = 0;
                        List<string> tempAttributes = new List<string>();
                        foreach (var item in attributesAndValues)
                        {
                            if (index % 2 == 0)
                                tempAttributes.Add(item.Trim());
                            index++;
                        }

                        //Finally create the string to bind.
                        _attributesForQuery = string.Empty;
                        tempAttributes.ForEach(a => this.AttributesForQuery += a + ", ");
                        this.AttributesForQuery = this.AttributesForQuery.Substring(0, this.AttributesForQuery.Length - 2);
                    }
                }

                #endregion AttributesAndValuesToPut
            });
            //Populate the domains while booting up.
            this.PopulateDomainNames();
        }

        /// <summary>
        /// Populates the list of items for the specified domain name.
        /// </summary>
        /// <param name="domainName">The domain name.</param>
        private void PopulateItemsNamesList(string domainName)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object sender, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {

                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.ItemsNameList.Clear();
                            if (selectResult.Item.Count > 0)
                            {
                                selectResult.Item.ForEach(i => this.ItemsNameList.Add(i.Name));
                                this.ItemsNameMessage = "No of Item: " + selectResult.Item.Count;
                            }
                            else
                            {
                                this.ItemsNameMessage = "No Item";
                                this.ItemsNameList.Clear();
                            }
                        });
                    }
                }
            };
            this.ItemsNameMessage = "Updating Item names...";
            this.ItemsNameList.Clear();
            SimpleDB.Client.OnSimpleDBResponse += handler;
            SimpleDB.Client.Select(new SelectRequest { SelectExpression = "SELECT * FROM " + domainName, ConsistentRead = true });
        }

        #endregion CTor

        #region Private methods

        /// <summary>
        /// Fetches the domain names and populates them to the domain list box.
        /// </summary>
        private void PopulateDomainNames()
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            handler = delegate(object sender, ResponseEventArgs args)
            {
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                ListDomainsResponse response = args.Response as ListDomainsResponse;

                if (null != response)
                {
                    ListDomainsResult listResult = response.ListDomainsResult;
                    if (null != listResult)
                    {
                        this.Dispatcher.BeginInvoke(() =>
                            {
                                this.DomainNameList.Clear();
                                listResult.DomainName.ForEach(domain => this.DomainNameList.Add(domain));
                            });
                    }
                }
            };

            //Show a wait message.
            this.Dispatcher.BeginInvoke(() =>
            {
                this.DomainNameList.Clear();
                this.DomainNameList.Add("Please wait...");
            });
            SimpleDB.Client.OnSimpleDBResponse += handler;
            ListDomainsRequest request = new ListDomainsRequest();
            SimpleDB.Client.BeginListDomains(request);
        }
        #endregion

        #region Put Attribute

        /// <summary>
        /// Gets a list of Attribute and Values for Put action.
        /// </summary>
        /// <returns>Returns a list of <see cref="AttributeAndValue"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        static List<AttributeAndValue> GetListAttributeAndValueFromString(string stringToParse)
        {
            List<AttributeAndValue> list = new List<AttributeAndValue>();
            string[] tempArray = stringToParse.Split(new char[] { ',' });

            if (tempArray.Length > 0)
            {
                foreach (var item in tempArray)
                {
                    string[] attributeAndValue = item.Split(new char[] { '-' });
                    if (attributeAndValue.Length > 0)
                    {
                        string value = string.Empty;
                        if (attributeAndValue.Length > 0)
                            value = attributeAndValue[1];

                        list.Add(new AttributeAndValue { Attribute = attributeAndValue[0].Trim(), Value = value });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets a list of Attribute and no values.
        /// </summary>
        /// <returns>Returns a list of <see cref="AttributeAndValue"/>s.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        static List<AttributeAndValue> GetListAttributeFromString(string stringToParse)
        {
            List<AttributeAndValue> list = new List<AttributeAndValue>();
            string[] tempArray = stringToParse.Split(new char[] { ',' });

            if (tempArray.Length > 0)
            {
                foreach (var item in tempArray)
                    list.Add(new AttributeAndValue { Attribute = item.Trim() });
            }
            return list;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnPutAttributes_Click(object sender, RoutedEventArgs e)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;
            this.SomeMessage = "Please wait...";
            handler = delegate(object senderAmazon, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                PutAttributesResponse response = args.Response as PutAttributesResponse;
                this.Dispatcher.BeginInvoke(() =>
                {
                    if (null != response)
                        this.SomeMessage = "Attribute put successfully.";
                    else
                    {
                        AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;
                        if (null != exception)
                            this.SomeMessage = "Error: " + exception.Message;
                    }
                });
            };

            SimpleDB.Client.OnSimpleDBResponse += handler;

            PutAttributesRequest putAttributesRequest = new PutAttributesRequest { DomainName = this.DomainName, ItemName = this.ItemName };
            List<ReplaceableAttribute> attributesOne = putAttributesRequest.Attribute;

            //Calculate the attributes and their values to put.
            foreach (var item in GetListAttributeAndValueFromString(this.AttributesAndValuesToPut))
                attributesOne.Add(new ReplaceableAttribute().WithName(item.Attribute).WithValue(item.Value));


            //attributesOne.Add(new ReplaceableAttribute().WithName("Category").WithValue("Clothes"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Subcategory").WithValue("Sweater"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Name").WithValue("Cathair Sweater"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Color").WithValue("Siamese"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Size").WithValue("Small"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Size").WithValue("Medium"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Size").WithValue("Large"));
            SimpleDB.Client.PutAttributes(putAttributesRequest);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnGetAttributes_Click(object sender, RoutedEventArgs e)
        {
            #region User Input Validation

            this.Attributes.Clear();
            //Validate user input.
            if (string.IsNullOrEmpty(this.DomainName) || string.IsNullOrEmpty(this.ItemName))
            {
                FetchingOrDeletingAttributeMessage = "Domain-Name or Item-Name cannot be empty.";
                return;
            }
            this.FetchingOrDeletingAttributeMessage = "Please wait...";

            #endregion User Input Validation

            GetAttributesRequest request = new GetAttributesRequest { DomainName = this.DomainName, ItemName = this.ItemName };

            //Associate the attributes.
            GetListAttributeFromString(this.AttributesForQuery).ForEach(v => request.AttributeName.Add(v.Attribute));

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object senderAmazon, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                GetAttributesResponse response = args.Response as GetAttributesResponse;

                if (null != response)
                {
                    GetAttributesResult attributeResult = response.GetAttributesResult;
                    if (null != attributeResult)
                    {
                        this.Dispatcher.BeginInvoke(() =>
                        {
                            this.Attributes.Clear();
                            if (attributeResult.Attribute.Count > 0)
                            {
                                FetchingOrDeletingAttributeMessage = "Result count: " + attributeResult.Attribute.Count;
                                foreach (var item in attributeResult.Attribute)
                                    this.Attributes.Add(item);
                            }
                            else
                            {
                                FetchingOrDeletingAttributeMessage = "No results";
                            }
                        });
                    }
                }
            };

            SimpleDB.Client.OnSimpleDBResponse += handler;
            SimpleDB.Client.GetAttributes(request);
        }

        #endregion Get Attribute

        #region Batch Put Attribute

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnBatchPutAttributes_Click(object sender, RoutedEventArgs e)
        {
            SimpleDB.Client.OnSimpleDBResponse += BatchPutAttributeWebResponse;
            BatchPutAttributesRequest request = new BatchPutAttributesRequest() { DomainName = this.DomainName };
            this.BatchPutMessage = "Please wait...";
            List<ReplaceableAttribute> attributesOne = new List<ReplaceableAttribute>();
            List<ReplaceableAttribute> attributesTwo = new List<ReplaceableAttribute>();

            List<AttributeAndValue> aAndV = GetListAttributeAndValueFromString(this.AttributesAndValuesToPut);

            int index = 0;
            foreach (var item in aAndV)
            {
                if (index <= aAndV.Count / 2)
                    attributesOne.Add(new ReplaceableAttribute().WithName(item.Attribute).WithValue(item.Value));
                else
                    attributesTwo.Add(new ReplaceableAttribute().WithName(item.Attribute).WithValue(item.Value));
                index++;
            }

            //attributesOne.Add(new ReplaceableAttribute().WithName("Category").WithValue("Clothes"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Subcategory").WithValue("Sweater"));
            //attributesOne.Add(new ReplaceableAttribute().WithName("Name").WithValue("Cathair Sweater"));


            //attributesTwo.Add(new ReplaceableAttribute().WithName("Color").WithValue("Siamese"));
            //attributesTwo.Add(new ReplaceableAttribute().WithName("Size").WithValue("Small"));
            //attributesTwo.Add(new ReplaceableAttribute().WithName("Size").WithValue("Medium"));
            //attributesTwo.Add(new ReplaceableAttribute().WithName("Size").WithValue("Large"));

            List<ReplaceableItem> replacableItem = request.Items;

            //Get the item-names.
            foreach (var item in GetItemNames(this.ItemName))
            {
                ReplaceableItem repItem = new ReplaceableItem() { ItemName = item };
                attributesOne.ForEach(a => repItem.Attribute.Add(a));
                replacableItem.Add(repItem);
            }
            //replacableItem.Add(new ReplaceableItem() { Attribute = attributesOne, ItemName = "OneAttribute" });
            //replacableItem.Add(new ReplaceableItem() { Attribute = attributesTwo, ItemName = "TwoAttribute" });
            SimpleDB.Client.BatchPutAttributes(request);
        }

        void BatchPutAttributeWebResponse(object sender, ResponseEventArgs args)
        {
            ISimpleDBResponse result = args.Response;
            SimpleDB.Client.OnSimpleDBResponse -= BatchPutAttributeWebResponse;

            this.Dispatcher.BeginInvoke(() =>
                {
                    BatchPutAttributesResponse response = result as BatchPutAttributesResponse;

                    if (null != response)
                    {
                        this.BatchPutMessage = "Batch attributes put successfully";
                    }
                    else
                    {
                        AmazonSimpleDBException exception = result as AmazonSimpleDBException;
                        if (null != exception)
                            this.BatchPutMessage = "Error: " + exception.Message;
                    }
                });
        }

        /// <summary>
        /// Extracts the item-names for Batch operations.
        /// </summary>
        /// <param name="text">The string to extract the items.</param>
        /// <returns>Returns a list of names of items needed for Batch operations..</returns>
        static List<string> GetItemNames(string text)
        {
            List<string> items = new List<string>();

            if (!string.IsNullOrEmpty(text))
            {
                string[] tempItems = text.Split(new char[] { ',' });

                if (tempItems.Length > 0)
                {
                    foreach (var item in tempItems)
                        items.Add(item);
                }
            }
            return items;
        }

        #endregion

        #region Delete Attribute

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnDeletetAttributes_Click(object sender, RoutedEventArgs e)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;
            responseHandler = delegate(object senderAmazon, ResponseEventArgs args)
            {
                SimpleDB.Client.OnSimpleDBResponse -= responseHandler;
                DeleteAttributesResponse deleteResponse = args.Response as DeleteAttributesResponse;
                this.Dispatcher.BeginInvoke(() =>
                    {
                        this.Attributes.Clear();
                        string message = string.Empty;
                        if (null != deleteResponse)
                            message = "Attribute(s) deleted successfully.";
                        else
                        {
                            AmazonSimpleDBException exception = args.Response as AmazonSimpleDBException;
                            if (null != exception)
                                message = "Error: " + exception.Message;
                        }
                        this.FetchingOrDeletingAttributeMessage = message;
                    });
            };

            SimpleDB.Client.OnSimpleDBResponse += responseHandler;

            DeleteAttributesRequest deleteRequest = new DeleteAttributesRequest() { DomainName = this.DomainName, ItemName = this.ItemName };
            List<Amazon.SimpleDB.Model.Attribute> deleteItem = deleteRequest.Attribute;
            List<AttributeAndValue> aAndV = GetListAttributeAndValueFromString(this.AttributesAndValuesToPut);

            aAndV.ForEach(a => deleteItem.Add(new Amazon.SimpleDB.Model.Attribute().WithName(a.Attribute).WithValue(a.Value)));
            //deleteItem.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Category").WithValue("Clothes"));
            //deleteItem.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Subcategory").WithValue("Sweater"));

            SimpleDB.Client.DeleteAttributes(deleteRequest);
        }

        #endregion

        #region Batch Delete Attribute

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnBatchDeleteAttributes_Click(object sender, RoutedEventArgs e)
        {
            SimpleDBResponseEventHandler<object, ResponseEventArgs> responseHandler = null;

            responseHandler = delegate(object senderOriginal, ResponseEventArgs args)
            {
                ISimpleDBResponse result = args.Response;
                SimpleDB.Client.OnSimpleDBResponse -= responseHandler;
                this.Dispatcher.BeginInvoke(() =>
                {
                    BatchDeleteAttributesResponse response = result as BatchDeleteAttributesResponse;

                    if (null != response)
                    {
                        this.BatchDeleteMessage = "Batch attributes deleted successfully";
                    }
                    else
                    {
                        AmazonSimpleDBException exception = result as AmazonSimpleDBException;
                        if (null != exception)
                            this.BatchDeleteMessage = "Error: " + exception.Message;
                    }
                });
            };
            this.BatchDeleteMessage = "Please wait...";
            SimpleDB.Client.OnSimpleDBResponse += responseHandler;

            BatchDeleteAttributesRequest deleteRequest = new BatchDeleteAttributesRequest() { DomainName = this.DomainName };
            List<DeleteableItem> deleteItem = deleteRequest.Item;

            //List<Amazon.SimpleDB.Model.Attribute> attributeItem1 = new List<Amazon.SimpleDB.Model.Attribute>();
            //List<Amazon.SimpleDB.Model.Attribute> attributeItem2 = new List<Amazon.SimpleDB.Model.Attribute>();

            List<AttributeAndValue> aAndV1 = GetListAttributeAndValueFromString(this.AttributesAndValuesToPut);
            DeleteableItem item1 = new DeleteableItem { ItemName = "OneAttribute" };
            DeleteableItem item2 = new DeleteableItem { ItemName = "TwoAttribute" };

            int index = 0;
            foreach (var item in aAndV1)
            {
                if (index <= aAndV1.Count / 2)
                    item1.Attribute.Add(new Amazon.SimpleDB.Model.Attribute().WithName(item.Attribute).WithValue(item.Value));
                else
                    item2.Attribute.Add(new Amazon.SimpleDB.Model.Attribute().WithName(item.Attribute).WithValue(item.Value));
                index++;
            }

            //attributeItem1.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Category").WithValue("Clothes"));
            //attributeItem1.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Subcategory").WithValue("Sweater"));

            //attributeItem2.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Size").WithValue("Small"));
            //attributeItem2.Add(new Amazon.SimpleDB.Model.Attribute().WithName("Color").WithValue("Siamese"));

            #region Commented
            //Commented because of changes in the Attribute property definition change during resolving FxCop warnings.
            //deleteItem.Add(new DeleteableItem() { Attribute = attributeItem1, ItemName = "OneAttribute" });
            //deleteItem.Add(new DeleteableItem() { Attribute = attributeItem2, ItemName = "TwoAttribute" });

            #endregion Commented
            deleteItem.Add(item1);
            deleteItem.Add(item2);

            SimpleDB.Client.BatchDeleteAttributes(deleteRequest);
        }

        #endregion

        #region Select Query

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void btnSelectQuery_Click(object sender, RoutedEventArgs e)
        {
            #region User Input Validation

            if (string.IsNullOrEmpty(this.DomainName))
            {
                this.SelectQueryMessage = "Domain-Name cannot be empty.";
                return;
            }
            this.SelectResultAttributes.Clear();
            this.SelectQueryMessage = "Executing query for " + this.DomainName + ". Please wait...";

            #endregion User Input Validation

            SimpleDBResponseEventHandler<object, ResponseEventArgs> handler = null;

            handler = delegate(object senderOriginal, ResponseEventArgs args)
            {
                //Unhook from event.
                SimpleDB.Client.OnSimpleDBResponse -= handler;
                SelectResponse response = args.Response as SelectResponse;

                if (null != response)
                {
                    SelectResult selectResult = response.SelectResult;
                    if (null != selectResult)
                    {

                        this.Dispatcher.BeginInvoke(() =>
                        {
                            if (selectResult.Item.Count > 0)
                            {
                                this.SelectResultAttributes.Clear();

                                int itemsCount = 0;
                                int attributesCount = 0;
                                foreach (var item in selectResult.Item)
                                {
                                    item.Attribute.ForEach(attribute => this.SelectResultAttributes.Add(new AttributeAndValue { Attribute = attribute.Name, Value = attribute.Value, ItemName = item.Name }));
                                    itemsCount++;
                                    attributesCount += item.Attribute.Count;
                                }
                                //Item item = selectResult.Item[0];
                                //item.Attribute.ForEach(attribute => this.SelectResultAttributes.Add(attribute));
                                this.SelectQueryMessage = "No of Item: " + itemsCount + ", No of attributes: " + attributesCount;
                            }
                            else
                            {
                                this.SelectQueryMessage = "No results";
                                this.SelectResultAttributes.Clear();
                            }
                        });
                    }
                }
            };
            SimpleDB.Client.OnSimpleDBResponse += handler;
            SimpleDB.Client.Select(new SelectRequest { SelectExpression = this.SelectQuery, ConsistentRead = true });
        }

        #endregion Select Query

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
    /// Represents a type to hold information about Attribute and its Value.
    /// </summary>
    public class AttributeAndValue
    {
        /// <summary>
        /// Gets and sets the attribute name.
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Gets or sets the attribute value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the name of the associated item.
        /// </summary>
        public string ItemName { get; set; }
    }
}