﻿using GingerWPF.WizardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ginger.RunSetLib.CreateCLIWizardLib
{
    /// <summary>
    /// Interaction logic for CLISourceControlPage.xaml
    /// </summary>
    public partial class CLIOptionsPage : Page, IWizardPage
    {
        CreateCLIWizard mCreateCLIWizard;
        public CLIOptionsPage()
        {
            InitializeComponent();
        }

        public void WizardEvent(WizardEventArgs WizardEventArgs)
        {
            switch (WizardEventArgs.EventType)
            {
                case EventType.Init:
                    mCreateCLIWizard = (CreateCLIWizard)WizardEventArgs.Wizard;
                    break;
                case EventType.Active:
                    //
                    break;
            }

        }

        

        private void XDownloadsolutionFromSourceControlcheckBox_Checked(object sender, RoutedEventArgs e)
        {
            mCreateCLIWizard.DownloadSolutionFromSourceControl = true;
        }

        private void XDownloadsolutionFromSourceControlcheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            mCreateCLIWizard.DownloadSolutionFromSourceControl = false;
        }
    }
}