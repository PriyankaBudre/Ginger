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

namespace GingerCoreNETUnitTest.Reports
{
    [TestClass]
    public class LinuxFoldersTest
    {

        static string mOutputFolderPath;
        static string mTestResourcesPath;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            //Arrange
            mOutputFolderPath = TestResources.GetTempFolder("HTMLReports") + Path.DirectorySeparatorChar;
            mTestResourcesPath = TestResources.GetTestResourcesFolder(Path.Combine("Reports", "HTMLReports"));

        }


        [TestMethod]
        public void CreateFolderTest()
        {
            //Act
            string TestFolder = Path.Combine(mOutputFolderPath, "Test");
            Directory.CreateDirectory(TestFolder);
            Console.WriteLine("Directory Created at:" + TestFolder);
            //Assert
            Assert.IsTrue(Directory.Exists(TestFolder));
        }


    }
}
