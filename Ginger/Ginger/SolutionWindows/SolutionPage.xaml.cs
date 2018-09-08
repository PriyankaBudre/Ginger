#region License
/*
Copyright © 2014-2018 European Support Limited

Licensed under the Apache License, Version 2.0 (the "License")
you may not use this file except in compliance with the License.
You may obtain a copy of the License at 

http://www.apache.org/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS, 
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
See the License for the specific language governing permissions and 
limitations under the License. 
*/
#endregion

using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.Common;
using Ginger.Environments;
using Ginger.TagsLib;
using Ginger.UserControls;
using Ginger.Variables;
using GingerCore;
using GingerCore.Platforms;
using GingerCore.Variables;
using GingerCoreNET.SolutionRepositoryLib.RepositoryObjectsLib.PlatformsLib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Ginger.SolutionWindows
{
    /// <summary>
    /// Interaction logic for SolutionPage.xaml
    /// </summary>
    public partial class SolutionPage : Page
    {
        Solution mSolution;
        ucGrid ApplicationGrid;

        public SolutionPage()
        {
            InitializeComponent();
            
            App.UserProfile.PropertyChanged += UserProfile_PropertyChanged;
            Init();
        }

        private void UserProfile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(UserProfile.Solution))
            {
                Init();
            }
        }

        private void Init()
        {
            if (App.UserProfile.Solution != null)
            {
                mSolution = App.UserProfile.Solution;
            }
            else
            {
                mSolution = null;
            }

            if (mSolution != null)
            {
                xLoadSolutionlbl.Visibility = Visibility.Collapsed;
                xSolutionDetailsStack.Visibility = Visibility.Visible;
                App.ObjFieldBinding(SolutionNameTextBox, TextBox.TextProperty, mSolution, nameof(Solution.Name));
                App.ObjFieldBinding(SolutionFolderTextBox, TextBox.TextProperty, mSolution, nameof(Solution.Folder));
                App.ObjFieldBinding(AccountTextBox, TextBox.TextProperty, mSolution, nameof(Solution.Account));
            }
            else
            {
                xLoadSolutionlbl.Visibility = Visibility.Visible;
                xSolutionDetailsStack.Visibility = Visibility.Collapsed;
            }
        }


        private void xSaveButton_Click(object sender, RoutedEventArgs e)
        {
            mSolution.SaveSolutionConfigurations();
        }
    }
}
