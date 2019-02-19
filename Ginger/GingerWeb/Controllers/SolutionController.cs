using amdocs.ginger.GingerCoreNET;
using Amdocs.Ginger.CoreNET.Repository;
using Amdocs.Ginger.Repository;
using Ginger.SolutionGeneral;
using GingerWeb.UsersLib;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace GingerWeb.Controllers
{
    [Route("api/[controller]")]
    public class SolutionController : Controller
    {

        

        [HttpGet("[action]")]
        public IEnumerable<object> Solutions()
        {
            List<SolutionJSON> solutions = new List<SolutionJSON>();

            // Scan all folder which contains Ginger.Solution.xml
            // string solutionsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ginger" , "Solutions" );            
            

            string solutionsFolder = General.GetLocalGingerDirectory("Solutions");
                
                
            string[] solutionFolders = Directory.GetDirectories(solutionsFolder);            
            foreach(string solution in solutionFolders)
            {                
                
                string solutionName = solution.Replace(Path.GetDirectoryName(solution) + "\\","");
                solutions.Add(new SolutionJSON() { name = solutionName, path = solution });
            }
            

            return solutions;
        }


        [HttpPost("[action]")]
        public string OpenSolution([FromBody] SolutionJSON sol)
        {
            OpenSolution(sol.name);            
            return "OK";
        }

        private static void OpenSolution(string sFolder)
        {            
            if (Directory.Exists(sFolder))
            {
                Console.WriteLine("Opening Solution at folder: " + sFolder);                 
                SolutionRepository SR = GingerSolutionRepository.CreateGingerSolutionRepository();                
                SR.Open(sFolder);
                WorkSpace.Instance.SolutionRepository = SR;

                //string txt = System.IO.File.ReadAllText(solution);
                //Ginger.SolutionGeneral.Solution sol = (Ginger.SolutionGeneral.Solution)NewRepositorySerializer.DeserializeFromText(txt);
                //WorkSpace.Instance.Solution = sol;


                WorkSpace.Instance.Solution = (Solution)(ISolution)SR.RepositorySerializer.DeserializeFromFile(Path.Combine(SR.SolutionFolder, "Ginger.Solution.xml"));
            }
            else
            {
                Console.WriteLine("Directory not found: " + sFolder);
            }
        }

    }

    public class SolutionJSON
    {
        public string name { get; set; }
        public string path { get; set; }
    }


}