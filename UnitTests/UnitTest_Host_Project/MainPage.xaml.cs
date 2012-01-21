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
using System.Reflection;
using Microsoft.Silverlight.Testing;

namespace UnitTest_Host_Project
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Constants

        const bool runUnitTests = true;

        #endregion Constants

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += delegate(object sender, RoutedEventArgs e)
            {
                if (runUnitTests)
                {
                    UnitTestSettings settings = UnitTestSystem.CreateDefaultSettings();

                    if (null != Deployment.Current)
                    {
                        var parts = Deployment.Current.Parts;
                        var assembliesToLoad = new List<string> { "UnitTest_S3", "UnitTest_SimpleDB", "UnitTest_SQS" }; //ADD THE NAME OF THE ASSEMBLY TO TEST HERE AND LOAD IT.
                        int count = 0;
                        
                        //Clear all the test-assemblies.
                        settings.TestAssemblies.Clear();

                        //Algorithm.
                        //0.NOTE: ADD THE NAME OF THE ASSEMBLIES TO TEST MANUALLY.
                        //1.Assembly.Load() cannot load assemblies as it would do in a normal .NET applications.
                        //Hence, the only way to do is to make a explicit reference to them from this main executing assembly(.exe, a Windows Phone Application).
                        //Reason, to enable loading assemblies dynamically, they need to be signed by Windows Market.

                        //2. Specify the assemblies that need to be tested explicitly to the testing runtime=> UnitTestSettings.TestAssemblies() method.
                        foreach (var part in parts)
                        {
                            var assemblyName = part.Source.Replace(".dll", string.Empty);
                            if (!assembliesToLoad.Contains(assemblyName)) continue;

                            var assembly = Assembly.Load(assemblyName);
                            count++;
                            
                            settings.TestAssemblies.Add(assembly);
                            if (count >= assembliesToLoad.Count)
                                break;
                        }

                        UserControl content = UnitTestSystem.CreateTestPage(settings) as UserControl;
                        (Application.Current.RootVisual as PhoneApplicationFrame).Content = content;
                        (Application.Current.RootVisual as PhoneApplicationFrame).Padding = new Thickness(-50);

                        IMobileTestPage imtp = (Application.Current.RootVisual as PhoneApplicationFrame).Content as IMobileTestPage;

                        if (imtp != null)
                        {
                            BackKeyPress += (x, xe) => xe.Cancel = imtp.NavigateBack();
                        }
                    }
                }
            };
        }
    }
}