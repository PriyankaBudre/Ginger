#region License
/*
Copyright © 2014-2019 European Support Limited

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
using Amdocs.Ginger;
using Amdocs.Ginger.Common;
using Amdocs.Ginger.Common.InterfacesLib;
using Amdocs.Ginger.CoreNET.Repository;
using Amdocs.Ginger.CoreNET.RunLib;
using Amdocs.Ginger.IO;
using Amdocs.Ginger.Repository;
using Ginger.BusinessFlowWindows;
using Ginger.ReporterLib;
using Ginger.Reports;
using Ginger.Repository;
using Ginger.Run;
using Ginger.Run.RunSetActions;
using Ginger.SolutionGeneral;
using Ginger.SolutionWindows;
using Ginger.SourceControl;
using GingerCore;
using GingerCore.Actions;
using GingerCore.Actions.Common;
using GingerCore.GeneralLib;
using GingerCore.Platforms;
using GingerCore.Repository;
using GingerCore.Repository.UpgradeLib;
using GingerCore.SourceControl;
using GingerCore.Variables;
using GingerCoreNET.SourceControl;
using GingerWPF;
using GingerWPF.WorkSpaceLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Ginger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //DO NOT REMOVE- Needed for Log to work
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger
                                       (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly string ENCRYPTION_KEY = "D3^hdfr7%ws4Kb56=Qt";//?????

        public static System.Diagnostics.FileVersionInfo ApplicationInfo
        {
            get
            {
                return System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            }
        }
        public static string AppName = ApplicationInfo.FileDescription;//"Ginger"
        public static string AppFullProductName = ApplicationInfo.ProductName;//"Ginger by Amdocs"

        private static string mAppVersion = String.Empty;
        public static string AppVersion
        {
            get
            {
                if (mAppVersion == string.Empty)
                {
                    if (ApplicationInfo.ProductPrivatePart != 0)//Alpha
                    {
                        mAppVersion = string.Format("{0}.{1}.{2}.{3}", ApplicationInfo.ProductMajorPart, ApplicationInfo.ProductMinorPart, ApplicationInfo.ProductBuildPart, ApplicationInfo.ProductPrivatePart);
                        mAppVersion += "(Alpha, Build Time: " + App.AppBuildTime.ToString("dd-MMM-yyyy hh:mm tt") + ")";
                    }
                    else if (ApplicationInfo.ProductBuildPart != 0)//Beta
                    {
                        mAppVersion = string.Format("{0}.{1}.{2}", ApplicationInfo.ProductMajorPart, ApplicationInfo.ProductMinorPart, ApplicationInfo.ProductBuildPart);
                        mAppVersion += "(Beta, Build Date: " + App.AppBuildTime.ToString("dd-MMM-yyyy") + ")";
                    }
                    else//Official Release
                    {
                        mAppVersion = string.Format("{0}.{1}", ApplicationInfo.ProductMajorPart, ApplicationInfo.ProductMinorPart);
                    }
                }

                return mAppVersion;
            }
        }

        private static string mAppShortVersion = String.Empty;
        public static string AppShortVersion
        {
            get
            {
                if (mAppShortVersion == string.Empty)
                {
                    if (ApplicationInfo.ProductPrivatePart != 0)//Alpha
                    {
                        mAppShortVersion = string.Format("{0}.{1}.{2}.{3}", ApplicationInfo.ProductMajorPart, ApplicationInfo.ProductMinorPart, ApplicationInfo.ProductBuildPart, ApplicationInfo.ProductPrivatePart);
                    }
                    else if (ApplicationInfo.ProductBuildPart != 0)//Beta
                    {
                        mAppShortVersion = string.Format("{0}.{1}.{2}", ApplicationInfo.ProductMajorPart, ApplicationInfo.ProductMinorPart, ApplicationInfo.ProductBuildPart);
                    }
                    else//Official Release
                    {
                        mAppShortVersion = string.Format("{0}.{1}", ApplicationInfo.ProductMajorPart, ApplicationInfo.ProductMinorPart);
                    }
                }

                return mAppShortVersion;
            }
        }

        public static Ginger.Functionalties.SolutionAutoSave AppSolutionAutoSave = new Ginger.Functionalties.SolutionAutoSave();//To move it workspace
        public static Ginger.Functionalties.SolutionRecover AppSolutionRecover = new Ginger.Functionalties.SolutionRecover();//To move it workspace
        public static string RecoverFolderPath = null; //???

        static bool mIsReady = false;//???
        public bool IsReady { get { return mIsReady; } }//???

        private static bool mAppBuildTimeCalculated = false;
        private static DateTime mAppBuildTime;
        public static DateTime AppBuildTime
        {
            get
            {
                if (mAppBuildTimeCalculated == false)
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
                    mAppBuildTime = fileInfo.LastWriteTime;
                    mAppBuildTimeCalculated = true;
                }

                return mAppBuildTime;
            }
        }

        public new static MainWindow MainWindow { get; set; }
        
        private Dictionary<string, Int32> mExceptionsDic = new Dictionary<string, int>();

        public static event PropertyChangedEventHandler PropertyChanged;
        protected static void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {                
                handler(null, new PropertyChangedEventArgs(name));
            }
        }
        
       

        public static bool RunningFromUnitTest = false;

        public static void LoadApplicationDictionaries(Amdocs.Ginger.Core.eSkinDicsType SkinDicType = Amdocs.Ginger.Core.eSkinDicsType.Default, GingerCore.eTerminologyType TerminologyType = GingerCore.eTerminologyType.Default)
        {
            //Clear all Dictionaries
            Application.Current.Resources.MergedDictionaries.Clear();

            //Load only relevant dictionaries for the application to use
            //Skins
            switch (SkinDicType)
            {
                case Amdocs.Ginger.Core.eSkinDicsType.Default:
                    Application.Current.Resources.MergedDictionaries.Add(
                            new ResourceDictionary()
                            {
                                Source = new Uri("pack://application:,,,/Ginger;component/Dictionaries/Skins/GingerDefaultSkinDictionary.xaml")
                            });
                    break;

                default:
                    Application.Current.Resources.MergedDictionaries.Add(
                            new ResourceDictionary()
                            {
                                Source = new Uri("pack://application:,,,/Ginger;component/Dictionaries/Skins/GingerDefaultSkinDictionary.xaml")
                            });
                    break;
            }

            // set terminology type
            GingerTerminology.TERMINOLOGY_TYPE = TerminologyType;
        }

        public static void InitApp()
        {
            // Add event handler for handling non-UI thread exceptions.
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(StandAloneThreadExceptionHandler);

            Reporter.WorkSpaceReporter = new GingerWorkSpaceReporter();

            WorkSpaceEventHandler WSEH = new WorkSpaceEventHandler();
            WorkSpace.Init(WSEH);

            string phase = string.Empty;

            RepositoryItemHelper.RepositoryItemFactory = new RepositoryItemFactory();

            WorkSpace.Instance.BetaFeatures = BetaFeatures.LoadUserPref();
            WorkSpace.Instance.BetaFeatures.PropertyChanged += BetaFeatureChanged;            

            if (WorkSpace.Instance.BetaFeatures.ShowDebugConsole)
            {
                DebugConsoleWindow debugConsole = new DebugConsoleWindow();
                debugConsole.ShowAsWindow();
                WorkSpace.Instance.BetaFeatures.DisplayStatus();
            }

            Reporter.ToLog(eLogLevel.INFO, "######################## Application version " + App.AppVersion + " Started ! ########################");

            SetLoadingInfo("Init Application");
            WorkSpace.AppVersion = App.AppShortVersion;
            // We init the classes dictionary for the Repository Serializer only once
            InitClassTypesDictionary();

            // TODO: need to add a switch what we get from old ginger based on magic key

            phase = "Loading User Profile";
            Reporter.ToLog(eLogLevel.DEBUG, phase);
            SetLoadingInfo(phase);
            WorkSpace.Instance.UserProfile = UserProfile.LoadUserProfile();

            phase = "Configuring User Type";
            Reporter.ToLog(eLogLevel.DEBUG, phase);
            SetLoadingInfo(phase);
            WorkSpace.Instance.UserProfile.LoadUserTypeHelper();


            phase = "Loading User Selected Resource Dictionaries";
            Reporter.ToLog(eLogLevel.DEBUG, phase);
            SetLoadingInfo(phase);
            if (WorkSpace.Instance.UserProfile != null)
                LoadApplicationDictionaries(Amdocs.Ginger.Core.eSkinDicsType.Default, WorkSpace.Instance.UserProfile.TerminologyDictionaryType);
            else
                LoadApplicationDictionaries(Amdocs.Ginger.Core.eSkinDicsType.Default, GingerCore.eTerminologyType.Default);

            Reporter.ToLog(eLogLevel.DEBUG, "Loading user messages pool");
            UserMsgsPool.LoadUserMsgsPool();
            StatusMsgsPool.LoadStatusMsgsPool();

            Reporter.ToLog(eLogLevel.DEBUG, "Init the Centralized Auto Log");
            AutoLogProxy.Init(App.AppVersion);

            Reporter.ToLog(eLogLevel.DEBUG, "Initializing the Source control");
            SetLoadingInfo(phase);

            phase = "Loading the Main Window";
            Reporter.ToLog(eLogLevel.DEBUG, phase);
            SetLoadingInfo(phase);            
            

            AutoLogProxy.LogAppOpened();
            

            // Register our own Ginger tool tip handler
            //--Canceling customize tooltip for now due to many issues and no real added value            

            phase = "Application was loaded and ready";
            Reporter.ToLog(eLogLevel.INFO, phase);
            mIsReady = true;
       
        }

        private static void SetLoadingInfo(string text)
        {
            if (MainWindow != null)
            {
                MainWindow.LoadingInfo(text);
            }
            
            Console.WriteLine("Loading Info: " + text);            
        }

        private static void StandAloneThreadExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (RunningFromUnitTest)
            {
                // happen when we close Ginger from unit tests
                if (e.ExceptionObject is System.Runtime.InteropServices.InvalidComObjectException || e.ExceptionObject is System.Threading.Tasks.TaskCanceledException)
                {
                    Console.WriteLine("StandAloneThreadExceptionHandler: Running from unit test ignoring error on ginger close");
                    return;
                }
            }
            Reporter.ToLog(eLogLevel.FATAL, ">>>>>>>>>>>>>> Error occurred on stand alone thread(non UI) - " + e.ExceptionObject);
            //Reporter.ToUser(eUserMsgKey.ThreadError, "Error occurred on stand alone thread - " + e.ExceptionObject.ToString());

            if ( WorkSpace.Instance.RunningInExecutionMode == false)
            {
                App.AppSolutionAutoSave.DoAutoSave();
            }

            /// if (e.IsTerminating)...
            /// 
            //TODO: show exception
            // save work to temp folder
            // enable user to save work
            // ask if to restart/close
            // when loading check restore and restore
        }

        static bool bDone = false;
        public static void InitClassTypesDictionary()
        {
            //TODO: cleanup after all RIs moved to GingerCoreCommon

            if (bDone) return;
            bDone = true;

            // TODO: remove after we don't need old serializer to load old repo items
            NewRepositorySerializer.NewRepositorySerializerEvent += RepositorySerializer.NewRepositorySerializer_NewRepositorySerializerEvent;

            // Add all RI classes from GingerCoreNET
            // NewRepositorySerializer.AddClassesFromAssembly(typeof(RunSetConfig).Assembly);

            // Add all RI classes from GingerCoreCommon
            NewRepositorySerializer.AddClassesFromAssembly(typeof(RepositoryItemBase).Assembly);
            
            // Add all RI classes from GingerCore
            NewRepositorySerializer.AddClassesFromAssembly(typeof(GingerCore.Actions.ActButton).Assembly); // GingerCore.dll
            
            // add from Ginger
            NewRepositorySerializer.AddClassesFromAssembly(typeof(Ginger.App).Assembly);

            // Each class which moved from GingerCore to GingerCoreCommon needed to be added here, so it will auto translate
            // For backward compatibility of loading old object name in xml
            Dictionary<string, Type> list = new Dictionary<string, Type>();
            list.Add("GingerCore.Actions.ActInputValue", typeof(ActInputValue));
            list.Add("GingerCore.Actions.ActReturnValue", typeof(ActReturnValue));
            list.Add("GingerCore.Actions.EnhancedActInputValue", typeof(EnhancedActInputValue));
            list.Add("GingerCore.Environments.GeneralParam", typeof(GeneralParam));
            
            // TODO: remove after it moved to common
            AddClass(list, typeof(RunSetConfig));
            AddClass(list, typeof(RunSetActionSendEmail));
            AddClass(list, typeof(BusinessFlowReport));
            AddClass(list, typeof(HTMLReportConfiguration));
            AddClass(list, typeof(HTMLReportConfigFieldToSelect));
            AddClass(list, typeof(Agent));
            AddClass(list, typeof(DriverConfigParam));
            AddClass(list, typeof(GingerRunner));
            AddClass(list, typeof(ApplicationAgent));

            AddClass(list, typeof(RunSetActionHTMLReportSendEmail));
            AddClass(list, typeof(EmailHtmlReportAttachment));
            AddClass(list, typeof(RunSetActionAutomatedALMDefects));
            AddClass(list, typeof(RunSetActionGenerateTestNGReport));
            AddClass(list, typeof(RunSetActionHTMLReport));
            AddClass(list, typeof(RunSetActionSaveResults));
            AddClass(list, typeof(RunSetActionSendFreeEmail));
            AddClass(list, typeof(RunSetActionSendSMS));
            AddClass(list, typeof(RunSetActionPublishToQC));
            AddClass(list, typeof(ActSetVariableValue));
            AddClass(list, typeof(ActClearAllVariables));
            AddClass(list, typeof(ActAgentManipulation));
            AddClass(list, typeof(ActUIElement));
            AddClass(list, typeof(UserProfile));
            AddClass(list, typeof(Solution));
            AddClass(list, typeof(Email));
            AddClass(list, typeof(EmailAttachment));
            AddClass(list, typeof(RunSetActionScript));
            // Put back for Lazy load of BF.Acitvities
            NewRepositorySerializer.AddLazyLoadAttr(nameof(BusinessFlow.Activities)); // TODO: add RI type, and use attr on field


            // Verify the old name used in XML
            //list.Add("GingerCore.Actions.RepositoryItemTag", typeof(RepositoryItemTag));
            //list.Add("GingerCore.Actions.EnhancedActInputValue", typeof(EnhancedActInputValue));

            // TODO: change to SR2  if we want the files to be loaded convert and save with the new SR2

            //if (WorkSpace.Instance.BetaFeatures.UseNewRepositorySerializer)
            //{
            //RepositorySerializer2 RS2 = new RepositorySerializer2();

            //SolutionRepository.mRepositorySerializer = RS2;
            //RepositoryFolderBase.mRepositorySerializer = RS2;
            //    ObservableListSerializer.RepositorySerializer = RS2;

            //}
            //else
            //{
            //        SolutionRepository.mRepositorySerializer = new RepositorySerializer();
            //        RepositoryFolderBase.mRepositorySerializer = new RepositorySerializer();
            //}

            NewRepositorySerializer.AddClasses(list);

        }

        private static void AddClass(Dictionary<string, Type> list, Type t)
        {
            list.Add((t).FullName, t);
            list.Add((t).Name, t);
        }

        private static async void HandleAutoRunMode()
        {
            string phase = "Running in Automatic Execution Mode";
            Reporter.ToLog(eLogLevel.INFO, phase);
            
            AutoLogProxy.LogAppOpened();
            SetLoadingInfo(phase);

            var result = await WorkSpace.Instance.RunsetExecutor.RunRunSetFromCommandLine();

            Reporter.ToLog(eLogLevel.INFO, "Closing Ginger automatically...");
            

            //setting the exit code based on execution status
            if (result == 0)
            {
                Reporter.ToLog(eLogLevel.DEBUG, ">> Run Set executed and passed, exit code: 0");
                Environment.ExitCode = 0;//success                    
            }
            else
            {
                Reporter.ToLog(eLogLevel.DEBUG, ">> No indication found for successful execution, exit code: 1");
                Environment.ExitCode = 1;//failure
            }

            AutoLogProxy.LogAppClosed();
            Environment.Exit(Environment.ExitCode);
        }

        public static void DownloadSolution(string SolutionFolder)
        {
            SourceControlBase mSourceControl;
            if ( WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.GIT)
                mSourceControl = new GITSourceControl();
            else if ( WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.SVN)
                mSourceControl = new SVNSourceControl();
            else
                mSourceControl = new SVNSourceControl();

            if (mSourceControl != null)
            {
                 WorkSpace.Instance.UserProfile.SourceControlType = mSourceControl.GetSourceControlType;
                mSourceControl.SourceControlURL =  WorkSpace.Instance.UserProfile.SourceControlURL;
                mSourceControl.SourceControlUser =  WorkSpace.Instance.UserProfile.SourceControlUser;
                mSourceControl.SourceControlPass =  WorkSpace.Instance.UserProfile.SourceControlPass;
                mSourceControl.SourceControlLocalFolder =  WorkSpace.Instance.UserProfile.SourceControlLocalFolder;
                mSourceControl.SolutionFolder = SolutionFolder;

                mSourceControl.SourceControlConfigureProxy =  WorkSpace.Instance.UserProfile.SolutionSourceControlConfigureProxy;
                mSourceControl.SourceControlProxyAddress =  WorkSpace.Instance.UserProfile.SolutionSourceControlProxyAddress;
                mSourceControl.SourceControlProxyPort =  WorkSpace.Instance.UserProfile.SolutionSourceControlProxyPort;
                mSourceControl.SourceControlTimeout =  WorkSpace.Instance.UserProfile.SolutionSourceControlTimeout;
                mSourceControl.supressMessage = true;
            }

            if ( WorkSpace.Instance.UserProfile.SourceControlLocalFolder == string.Empty)
            {
                Reporter.ToUser(eUserMsgKey.SourceControlConnMissingLocalFolderInput);
            }
            if (SolutionFolder.EndsWith("\\"))
                SolutionFolder = SolutionFolder.Substring(0, SolutionFolder.Length - 1);
            SolutionInfo sol = new SolutionInfo();
            sol.LocalFolder = SolutionFolder;
            if ( WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.SVN && Directory.Exists(PathHelper.GetLongPath(sol.LocalFolder)))
                sol.ExistInLocaly = true;
            else if ( WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.GIT && Directory.Exists(PathHelper.GetLongPath(sol.LocalFolder + @"\.git")))
                sol.ExistInLocaly = true;
            else
                sol.ExistInLocaly = false;
            sol.SourceControlLocation = SolutionFolder.Substring(SolutionFolder.LastIndexOf("\\") + 1);

            if (sol == null)
            {
                Reporter.ToUser(eUserMsgKey.AskToSelectSolution);
                return;
            }

            string ProjectURI = string.Empty;
            if ( WorkSpace.Instance.UserProfile.SourceControlType == SourceControlBase.eSourceControlType.SVN)
            {
                ProjectURI =  WorkSpace.Instance.UserProfile.SourceControlURL.StartsWith("SVN", StringComparison.CurrentCultureIgnoreCase) ?
                sol.SourceControlLocation :  WorkSpace.Instance.UserProfile.SourceControlURL + sol.SourceControlLocation;
            }
            else
            {
                ProjectURI =  WorkSpace.Instance.UserProfile.SourceControlURL;
            }
            bool getProjectResult = true;
            getProjectResult = SourceControlIntegration.CreateConfigFile(mSourceControl);
            if (getProjectResult != true)
                return;
            if (sol.ExistInLocaly == true)
            {
                mSourceControl.RepositoryRootFolder = sol.LocalFolder;
                SourceControlIntegration.GetLatest(sol.LocalFolder, mSourceControl);
            }
            else
                getProjectResult = SourceControlIntegration.GetProject(mSourceControl, sol.LocalFolder, ProjectURI);
        }

        static bool mLoadingSolution;
        public static bool LoadingSolution
        {
            get
            {
                return mLoadingSolution;
            }
        }

        private static void SolutionCleanup()
        {
            if (WorkSpace.Instance.SolutionRepository != null)
            {
                WorkSpace.Instance.PlugInsManager.CloseAllRunningPluginProcesses();
            }

            if (! WorkSpace.Instance.RunningInExecutionMode)
            {
                AppSolutionAutoSave.SolutionAutoSaveEnd();
            }

            WorkSpace.Instance.Solution = null;
            
            CloseAllRunningAgents();
            App.OnAutomateBusinessFlowEvent(AutomateEventArgs.eEventType.ClearAutomate, null);
            AutoLogProxy.SetAccount("");
            WorkSpace.Instance.SolutionRepository = null;
            WorkSpace.Instance.SourceControl = null;
        }

        public static void CloseAllRunningAgents()
        {
            if (WorkSpace.Instance.SolutionRepository != null)
            {
                List<Agent> runningAgents = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<Agent>().Where(x => x.Status == Agent.eStatus.Running).ToList();
                if (runningAgents != null && runningAgents.Count > 0)
                {
                    foreach (Agent agent in runningAgents)
                    {
                        try
                        {
                            agent.Close();
                        }
                        catch (Exception ex)
                        {
                            if (agent.Name != null)
                                Reporter.ToLog(eLogLevel.ERROR, string.Format("Failed to Close the '{0}' Agent", agent.Name), ex);
                            else
                                Reporter.ToLog(eLogLevel.ERROR, "Failed to Close the Agent", ex);
                        }
                        agent.IsFailedToStart = false;
                    }
                }
            }
        }

        public static bool SetSolution(string SolutionFolder)
        {            
            try
            {
                Reporter.ToLog(eLogLevel.INFO, string.Format("Loading the Solution '{0}'", SolutionFolder));
                mLoadingSolution = true;
                OnPropertyChanged(nameof(LoadingSolution));

                //Cleanup
                SolutionCleanup();

                //Load new Solution
                string SolFile = System.IO.Path.Combine(SolutionFolder, @"Ginger.Solution.xml");
                if (File.Exists(Amdocs.Ginger.IO.PathHelper.GetLongPath(SolFile)))
                {
                    //get Solution files
                    IEnumerable<string> solutionFiles = Solution.SolutionFiles(SolutionFolder);
                    ConcurrentBag<Tuple<SolutionUpgrade.eGingerVersionComparisonResult, string>> solutionFilesWithVersion = null; 

                    //check if Ginger Upgrade is needed for loading this Solution
                    try
                    {
                        Reporter.ToLog(eLogLevel.DEBUG, "Checking if Ginger upgrade is needed for loading the Solution");
                        if (solutionFilesWithVersion == null)
                        {
                            solutionFilesWithVersion = SolutionUpgrade.GetSolutionFilesWithVersion(solutionFiles);
                        }
                        ConcurrentBag<string> higherVersionFiles = SolutionUpgrade.GetSolutionFilesCreatedWithRequiredGingerVersion(solutionFilesWithVersion, SolutionUpgrade.eGingerVersionComparisonResult.HigherVersion);
                        if (higherVersionFiles.Count > 0)
                        {
                            if ( WorkSpace.Instance.RunningInExecutionMode == false && RunningFromUnitTest == false)
                            {
                                MainWindow.HideSplash();
                                UpgradePage gingerUpgradePage = new UpgradePage(SolutionUpgradePageViewMode.UpgradeGinger, SolutionFolder, string.Empty, higherVersionFiles.ToList());
                                gingerUpgradePage.ShowAsWindow();
                            }
                            Reporter.ToLog(eLogLevel.WARN, "Ginger upgrade is needed for loading the Solution, aborting Solution load.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        Reporter.ToLog(eLogLevel.ERROR, "Error occurred while checking if Solution requires Ginger Upgrade", ex);
                    }

                    Solution sol = Solution.LoadSolution(SolFile);

                    if (sol != null)
                    {
                        WorkSpace.Instance.SolutionRepository = GingerSolutionRepository.CreateGingerSolutionRepository();
                        WorkSpace.Instance.SolutionRepository.Open(SolutionFolder);

                        WorkSpace.Instance.PlugInsManager.SolutionChanged(WorkSpace.Instance.SolutionRepository);

                        HandleSolutionLoadSourceControl(sol);                        

                        ValueExpression.SolutionFolder = SolutionFolder;
                        BusinessFlow.SolutionVariables = sol.Variables;
                        sol.SetReportsConfigurations();

                        WorkSpace.Instance.Solution = sol;
                                                
                        WorkSpace.Instance.UserProfile.LoadRecentAppAgentMapping();
                        AutoLogProxy.SetAccount(sol.Account);

                        //SetDefaultBusinessFlow();

                        if (! WorkSpace.Instance.RunningInExecutionMode)
                        {
                            DoSolutionAutoSaveAndRecover();
                        }

                        //Offer to upgrade Solution items to current version
                        try
                        {
                            if (WorkSpace.Instance.UserProfile.DoNotAskToUpgradeSolutions == false &&  WorkSpace.Instance.RunningInExecutionMode == false && RunningFromUnitTest == false)
                            {
                                if (solutionFilesWithVersion == null)
                                {
                                    solutionFilesWithVersion = SolutionUpgrade.GetSolutionFilesWithVersion(solutionFiles);
                                }
                                ConcurrentBag<string> lowerVersionFiles = SolutionUpgrade.GetSolutionFilesCreatedWithRequiredGingerVersion(solutionFilesWithVersion, SolutionUpgrade.eGingerVersionComparisonResult.LowerVersion);
                                if (lowerVersionFiles != null && lowerVersionFiles.Count > 0)
                                {
                                    MainWindow.HideSplash();
                                    UpgradePage solutionUpgradePage = new UpgradePage(SolutionUpgradePageViewMode.UpgradeSolution, sol.Folder, sol.Name, lowerVersionFiles.ToList());
                                    solutionUpgradePage.ShowAsWindow();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Reporter.ToLog(eLogLevel.ERROR, "Error occurred while checking if Solution files should be Upgraded", ex);
                        }

                        // No need to add solution to recent if running from CLI
                        if (!WorkSpace.Instance.RunningInExecutionMode && !RunningFromUnitTest)
                        {
                            WorkSpace.Instance.UserProfile.AddSolutionToRecent(sol);
                        }
                    }
                    else
                    {
                        Reporter.ToUser(eUserMsgKey.SolutionLoadError, "Load solution from file failed.");
                        return false;
                    }
                }
                else
                {
                    Reporter.ToUser(eUserMsgKey.BeginWithNoSelectSolution);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Reporter.ToLog(eLogLevel.ERROR, "Error occurred while loading the solution", ex);
                SolutionCleanup();
                throw ex;
            }
            finally
            {
                mLoadingSolution = false;
                OnPropertyChanged(nameof(LoadingSolution));
                Reporter.ToLog(eLogLevel.INFO, string.Format("Finished Loading the Solution '{0}'", SolutionFolder));
                Mouse.OverrideCursor = null;
            }
        }

        private static void HandleSolutionLoadSourceControl(Solution solution)
        {
            string RepositoryRootFolder = string.Empty;
            SourceControlBase.eSourceControlType type = SourceControlIntegration.CheckForSolutionSourceControlType(solution.Folder, ref RepositoryRootFolder);
            if (type == SourceControlBase.eSourceControlType.GIT)
            {
                solution.SourceControl = new GITSourceControl();
            }
            else if (type == SourceControlBase.eSourceControlType.SVN)
            {
                solution.SourceControl = new SVNSourceControl();
            }

            if (solution.SourceControl != null)
            {
                if (string.IsNullOrEmpty( WorkSpace.Instance.UserProfile.SolutionSourceControlUser) || string.IsNullOrEmpty( WorkSpace.Instance.UserProfile.SolutionSourceControlPass))
                {
                    if ( WorkSpace.Instance.UserProfile.SourceControlUser != null &&  WorkSpace.Instance.UserProfile.SourceControlPass != null)
                    {
                        solution.SourceControl.SourceControlUser =  WorkSpace.Instance.UserProfile.SourceControlUser;
                        solution.SourceControl.SourceControlPass =  WorkSpace.Instance.UserProfile.SourceControlPass;
                        solution.SourceControl.SolutionSourceControlAuthorEmail =  WorkSpace.Instance.UserProfile.SolutionSourceControlAuthorEmail;
                        solution.SourceControl.SolutionSourceControlAuthorName =  WorkSpace.Instance.UserProfile.SolutionSourceControlAuthorName;
                    }
                }
                else
                {
                    solution.SourceControl.SourceControlUser =  WorkSpace.Instance.UserProfile.SolutionSourceControlUser;
                    solution.SourceControl.SourceControlPass =  WorkSpace.Instance.UserProfile.SolutionSourceControlPass;
                    solution.SourceControl.SolutionSourceControlAuthorEmail =  WorkSpace.Instance.UserProfile.SolutionSourceControlAuthorEmail;
                    solution.SourceControl.SolutionSourceControlAuthorName =  WorkSpace.Instance.UserProfile.SolutionSourceControlAuthorName;
                }

                string error = string.Empty;
                solution.SourceControl.SolutionFolder = solution.Folder;
                solution.SourceControl.RepositoryRootFolder = RepositoryRootFolder;
                solution.SourceControl.SourceControlURL = solution.SourceControl.GetRepositoryURL(ref error);
                solution.SourceControl.SourceControlLocalFolder =  WorkSpace.Instance.UserProfile.SourceControlLocalFolder;
                solution.SourceControl.SourceControlProxyAddress =  WorkSpace.Instance.UserProfile.SolutionSourceControlProxyAddress;
                solution.SourceControl.SourceControlProxyPort =  WorkSpace.Instance.UserProfile.SolutionSourceControlProxyPort;
                solution.SourceControl.SourceControlTimeout =  WorkSpace.Instance.UserProfile.SolutionSourceControlTimeout;

                WorkSpace.Instance.SourceControl = solution.SourceControl;
                RepositoryItemBase.SetSourceControl(solution.SourceControl);
                RepositoryFolderBase.SetSourceControl(solution.SourceControl);
            }
        }

        private static void DoSolutionAutoSaveAndRecover()
        {
            //Init
            AppSolutionAutoSave.SolutionInit( WorkSpace.Instance.Solution.Folder);
            AppSolutionRecover.SolutionInit( WorkSpace.Instance.Solution.Folder);

            //start Auto Save
            AppSolutionAutoSave.SolutionAutoSaveStart();

            //check if Recover is needed
            if (! WorkSpace.Instance.UserProfile.DoNotAskToRecoverSolutions)
                AppSolutionRecover.SolutionRecoverStart();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            //Exceptions to avoid because it source is in some .NET issue
            if (ex.Message == "Value cannot be null.\r\nParameter name: element" && ex.Source == "PresentationCore")//Seems like WPF Bug 
            {
                e.Handled = true;
                return;
            }

            //log it
            Reporter.ToLog(eLogLevel.ERROR, ex.ToString(), ex);

            //add to dictionary to make sure same exception won't show more than 3 times
            if (mExceptionsDic.ContainsKey(ex.Message))
                mExceptionsDic[ex.Message]++;
            else
                mExceptionsDic.Add(ex.Message, 1);

            if (mExceptionsDic[ex.Message] <= 3)
            {
                Ginger.GeneralLib.ExceptionDetailsPage.ShowError(ex);
            }

            // Clear the err so it will not crash
            e.Handled = true;
        }

        public static BusinessFlow GetNewBusinessFlow(string Name, bool setTargetApp=false)
        {
            BusinessFlow biz = new BusinessFlow();
            biz.Name = Name;
            biz.Activities = new ObservableList<Activity>();
            biz.Variables = new ObservableList<VariableBase>();
            Activity a = new Activity() { Active = true };
            a.ActivityName = GingerDicser.GetTermResValue(eTermResKey.Activity) + " 1";
            a.Acts = new ObservableList<IAct>();
            biz.Activities.Add(a);
            biz.Activities.CurrentItem = a;
            biz.CurrentActivity = a;

            if (setTargetApp == true && WorkSpace.Instance.Solution.ApplicationPlatforms.Count > 0)
            {
                biz.TargetApplications.Add(new TargetApplication() {AppName = WorkSpace.Instance.Solution.MainApplication});
                biz.CurrentActivity.TargetApplication = biz.TargetApplications[0].Name;
            }

            return biz;
        }

        internal static void CheckIn(string Path)
        {
            CheckInPage CIW = new CheckInPage(Path);
            CIW.ShowAsWindow();
        }
        

        private static void BetaFeatureChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public Dispatcher GetMainWindowDispatcher()
        {
            return MainWindow.Dispatcher;
        }

        internal static Style GetStyle(string key)
        {
            foreach (ResourceDictionary RD in Application.Current.Resources.MergedDictionaries)
            {
                var s = (Style)RD[key];
                if (s != null)
                {
                    return s;
                }
            }
            return null;
        }


        public static void CloseSolution()///????
        {
             WorkSpace.Instance.Solution = null;
        }


        public static event AutomateBusinessFlowEventHandler AutomateBusinessFlowEvent;
        public delegate void AutomateBusinessFlowEventHandler(AutomateEventArgs args);
        public static void OnAutomateBusinessFlowEvent(AutomateEventArgs.eEventType eventType, object obj)
        {
            AutomateBusinessFlowEventHandler handler = AutomateBusinessFlowEvent;
            if (handler != null)
            {
                handler(new AutomateEventArgs(eventType, obj));
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Console.WriteLine("Starting Ginger");

            if (e.Args.Length == 0)
            {
                // start regular Ginger UI
                StartGingerUI();                
            }
            else
            {
                // handle CLI
                if (e.Args[0].StartsWith("ConfigFile"))
                {
                    // This Ginger is running with run set config will do the run and close GingerInitApp();                                
                    StartGingerExecutor();
                }
                else
                {
                    InitClassTypesDictionary();
                    Reporter.WorkSpaceReporter = new GingerWorkSpaceReporter();                    
                    CLIProcessor.ExecuteArgs(e.Args);
                    // do proper close !!!         
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }

        private void StartGingerExecutor()
        {            
            InitApp();
            WorkSpace.Instance.RunningInExecutionMode = true;
            Reporter.ReportAllAlsoToConsole = true;  //needed so all reportering will be added to Console                             
            HandleAutoRunMode();
        }

        public void StartGingerUI()
        {            
            if (RunningFromUnitTest)
            {
                LoadApplicationDictionaries();
            }

            MainWindow = new MainWindow();
            MainWindow.Show();
            GingerCore.General.DoEvents();

            InitApp();

            MainWindow.Init();
            MainWindow.HideSplash();
        }

    }
}
