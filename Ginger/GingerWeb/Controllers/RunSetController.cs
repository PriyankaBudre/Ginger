using amdocs.ginger.GingerCoreNET;
using Ginger.Run;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GingerWeb.Controllers
{
    [Route("api/[controller]")]
    public class RunSetController : Controller
    {
        

        [HttpGet("[action]")]
        public IEnumerable<object> RunSets()
        {        
            IEnumerable<RunSetConfig> runSets = WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<RunSetConfig>().OrderBy(x => x.Name);
            var data = runSets.Select(x =>
                                    new
                                    {
                                        name = x.Name,
                                        description = x.Description,                                        
                                    });

            return data;
        }


        public class RunRunSetResult
        {
            public string name { get; set; }
            public string Status { get; internal set; }
        }

        public class RunRunSetRequest
        {
            public string name { get; set; }
        }


        [HttpPost("[action]")]
        public RunRunSetResult RunRunSet([FromBody] RunRunSetRequest runRunSetRequest)
        {
            RunRunSetResult runBusinessFlowResult = new RunRunSetResult();

            if (string.IsNullOrEmpty(runRunSetRequest.name))
            {
                runBusinessFlowResult.Status = "Name cannot be null";
                return runBusinessFlowResult;
            }

            RunSetConfig runSet = (from x in WorkSpace.Instance.SolutionRepository.GetAllRepositoryItems<RunSetConfig>() where x.Name == runRunSetRequest.name select x).SingleOrDefault();
            if (runSet == null)
            {
                runBusinessFlowResult.Status = "Name cannot be null";
                return runBusinessFlowResult;
            }

            RunrunSet(runSet);

            runBusinessFlowResult.Status = "Executed";



            return runBusinessFlowResult;
        }

        private void RunrunSet(RunSetConfig runSetConfig)
        {
            RunsetExecutor runsetExecutor = new RunsetExecutor();
            runsetExecutor.RunSetConfig = runSetConfig;
            WorkSpace.RunsetExecutor = runsetExecutor;
            runsetExecutor.RunRunset();
        }
    }
}