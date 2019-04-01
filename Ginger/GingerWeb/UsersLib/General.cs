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
using Amdocs.Ginger.Common;
using Amdocs.Ginger.GingerConsole;
using Amdocs.Ginger.GingerConsole.ReporterLib;
using Amdocs.Ginger.Repository;
using GingerCore;
using System;
using System.Collections.Generic;
using System.IO;

namespace GingerWeb.UsersLib
{


    public class General
    {
        
        public static void init()
        {
            InitClassTypesDictionary();

            Reporter.WorkSpaceReporter = new GingerConsoleWorkspaceReporter();            
            GingerConsoleWorkSpace ws = new GingerConsoleWorkSpace();
            WorkSpace.Init(ws);
            
            var servicesGrid = WorkSpace.Instance.LocalGingerGrid;
            var nodes = servicesGrid.NodeList;            
        }

        
        // COMBINE to one for all !!!!!!!!!!!!!!
        public static void InitClassTypesDictionary()
        {
            //TODO: cleanup after all RIs moved to GingerCoreCommon

            //if (bDone) return;
            //bDone = true;

            // TODO: remove after we don't need old serializer to load old repo items
            // NewRepositorySerializer.NewRepositorySerializerEvent += RepositorySerializer.NewRepositorySerializer_NewRepositorySerializerEvent;

            // Add all RI classes from GingerCoreCommon
            NewRepositorySerializer.AddClassesFromAssembly(typeof(RepositoryItemBase).Assembly);
            NewRepositorySerializer.AddClassesFromAssembly(typeof(Ginger.SolutionGeneral.Solution).Assembly);

            // corenet
            // NewRepositorySerializer.AddClassesFromAssembly(typeof(Agent).Assembly);

            // Add all RI classes from GingerCore
            // NewRepositorySerializer.AddClassesFromAssembly(typeof(GingerCore.Actions.ActButton).Assembly); // GingerCore.dll

            // add  old Plugins - TODO: remove later when we change to new plugins
            // NewRepositorySerializer.AddClassesFromAssembly(typeof(GingerPlugIns.ActionsLib.PlugInActionsBase).Assembly);


            // add from Ginger - items like RunSetConfig
            // NewRepositorySerializer.AddClassesFromAssembly(typeof(Ginger.App).Assembly);

            // Each class which moved from GingerCore to GingerCoreCommon needed to be added here, so it will auto translate
            // For backward compatibility of loading old object name in xml
            Dictionary<string, Type> list = new Dictionary<string, Type>();
            list.Add("GingerCore.Actions.ActInputValue", typeof(ActInputValue));
            list.Add("GingerCore.Actions.ActReturnValue", typeof(ActReturnValue));
            list.Add("GingerCore.Actions.EnhancedActInputValue", typeof(EnhancedActInputValue));
            list.Add("GingerCore.Environments.GeneralParam", typeof(GeneralParam));

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


        public static string GetLocalGingerDirectory(string subfolder = null)
        {

            string localFolder;
            if (GingerUtils.OperatingSystem.IsWindows())
            {
                //envHome = "HOMEPATH";
                //gingerHome = @"C:\GingerSourceControl";
                localFolder = @"c:\Ginger";
            }
            else if (GingerUtils.OperatingSystem.IsLinux())
            {                
                string homePath = Environment.GetEnvironmentVariable("HOME");
                localFolder = System.IO.Path.Combine(homePath, "Ginger");
            }
            else
            {
                throw new Exception("Unknown OS for get ginger Home");
            }
            if (!string.IsNullOrEmpty(subfolder))
            {
                localFolder = Path.Combine(localFolder, subfolder);
            }
            return localFolder;



        }



    }
}
