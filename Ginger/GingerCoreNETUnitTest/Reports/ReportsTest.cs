using Amdocs.Ginger.CoreNET.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using GingerTestHelper;
using System.Globalization;
using Ginger.Reports.GingerExecutionReport;
using System.IO;
using System.Xml;
using Amdocs.Ginger.Common;

namespace Ginger.Reports.Tests
{
    [TestClass]
    public class ReportsTest
    {
        [TestMethod]
        [Timeout(60000)]
        public void ActivityReportTest()
        {

            string ActivityReportFile = GingerTestHelper.TestResources.GetTestResourcesFile(@"Reports" + Path.DirectorySeparatorChar + "Activity.txt");
            try
            {

                ActivityReport AR = (ActivityReport)JsonLib.LoadObjFromJSonFile(ActivityReportFile, typeof(ActivityReport));
                Assert.AreEqual("Passed", AR.RunStatus);
                Assert.AreEqual(2044, AR.Elapsed);
            }

            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }
        }

        [TestMethod]
        [Timeout(60000)]
        public void BusinessflowReportTest()
        {
            //Arrange
            string BusinessFlowReportFile = GingerTestHelper.TestResources.GetTestResourcesFile(@"Reports" + Path.DirectorySeparatorChar + "BusinessFlow.txt");
            try
            {

                BusinessFlowReport BFR = (BusinessFlowReport)JsonLib.LoadObjFromJSonFile(BusinessFlowReportFile, typeof(BusinessFlowReport));
                Assert.AreEqual("Failed", BFR.RunStatus);
                Assert.AreEqual(float.Parse("36.279", CultureInfo.InvariantCulture), BFR.ElapsedSecs.Value);
            }

            catch (Exception Ex)
            {
                Assert.Fail(Ex.Message);
            }
        }

        static string mOutputFolderPath;
        static string mTestResourcesPath;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            //Arrange
            string BusinessFlowReportFolder = GingerTestHelper.TestResources.GetTestResourcesFolder(@"Reports" + Path.DirectorySeparatorChar + "AutomationTab_LastExecution" + Path.DirectorySeparatorChar);
            ReportInfo RI = new ReportInfo(BusinessFlowReportFolder);
            string templatesFolder = Path.Combine(Amdocs.Ginger.Common.GeneralLib.General.GetExecutingDirectory(), "Reports", "GingerExecutionReport");
            HTMLReportConfiguration selectedHTMLReportConfiguration = HTMLReportConfiguration.SetHTMLReportConfigurationWithDefaultValues("DefaultTemplate", true);

            mOutputFolderPath = TestResources.GetTempFolder("HTMLReports") + Path.DirectorySeparatorChar;
            mTestResourcesPath = TestResources.GetTestResourcesFolder(Path.Combine("Reports", "HTMLReports"));
            //Act
            string report = ExtensionMethods.CreateGingerExecutionReportByReportInfoLevel(RI, selectedHTMLReportConfiguration, templatesFolder, mOutputFolderPath);

        }



        [TestMethod]
        public void GenrateLastExecutionHTMLBusinessFlowReportTest()
        {
            //Assert
            string ExecutionFile = GetReportWithoutCrteationDate(mOutputFolderPath + "BusinessFlowReport.html");
            string TestResourcesFIle = GetReportWithoutCrteationDate(Path.Combine(mTestResourcesPath , "BusinessFlowReport.html"));

            //Assert.AreEqual(ExecutionFile, TestResourcesFIle);
        }

        [TestMethod]
        public void GenrateLastExecutionHTMLActivityReportTest()
        {
            //Assert
            //string ActivityReportFullPath = "1 Goto SCM URL" + Path.DirectorySeparatorChar + "ActivityReport.html";
            //string ExecutionFile = GetReportWithoutCrteationDate(mOutputFolderPath + Path.DirectorySeparatorChar + ActivityReportFullPath);
            //string TestResourcesFIle = GetReportWithoutCrteationDate(mTestResourcesPath + Path.DirectorySeparatorChar + ActivityReportFullPath);

            //Assert.AreEqual(ExecutionFile, TestResourcesFIle);
        }

        [TestMethod]
        public void GenrateLastExecutionHTMLActionReportTest()
        {
            //Assert
            string ActivityReportFullPath = Path.Combine( "1 Goto SCM URL" , "1 Goto App URL - httpcmitechin" , "ActionReport.html");
            string ExecutionFile = GetReportWithoutCrteationDate(Path.Combine(mOutputFolderPath, ActivityReportFullPath));
            string TestResourcesFIle = GetReportWithoutCrteationDate(Path.Combine(mTestResourcesPath, ActivityReportFullPath));

            try
            {
                Assert.AreEqual(ExecutionFile, TestResourcesFIle);
            }
            catch (Exception ex)
            {
                Reporter.ToLog(eLogLevel.ERROR, ex.Message);
                string logoString = "<img alt='Embedded Image' width='203' height='75' src='./../../assets/img/CustomerLogo.png' />";
                if (TestResourcesFIle.Contains(logoString) && !ExecutionFile.Contains(logoString))
                {
                    return;
                }
                else
                {
                    throw ex;
                }

            }
        }


        private string GetReportWithoutCrteationDate(string filePath)
        {
            string ExecutionFile = File.ReadAllText(filePath);
            return RemoveReportCreationDate(ExecutionFile);
        }


        private string RemoveReportCreationDate(string FileContent)
        {
            int creationDatePosition = FileContent.IndexOf("Report Creation Time");
            int EndOfLineOfCreationDatePosition = FileContent.IndexOf("</div>", creationDatePosition);
            string NewFileContent = FileContent.Substring(0, creationDatePosition) + FileContent.Substring(EndOfLineOfCreationDatePosition);
            return NewFileContent;
        }

    }
}
