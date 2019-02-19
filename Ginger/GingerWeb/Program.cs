using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GingerWeb.UsersLib;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GingerWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            General.init();

            CreateWebHostBuilder(args).Build().Run();
            //CreateWebHostBuilderAllIPs15000(args).Build().Run();

            
        }

        private static IWebHostBuilder CreateWebHostBuilderAllIPs15000(string[] args)
        {

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                    //!!!!!!!!!!!!!!!!!!!!! Config which web host to use and port
                    options.ListenAnyIP(15000);  // Listen on any IP + removed HTTPS redirect at StartUp class
                    
                });


            //var config = new ConfigurationBuilder()
            //    .AddCommandLine(args)
            //    .Build();

            //var host = new WebHostBuilder()
            //    .UseConfiguration(config)
            //    .UseContentRoot(Directory.GetCurrentDirectory())
            //    .UseKestrel()
            //    .UseIISIntegration()
            //    .UseStartup<Startup>();

            // var host = new WebHostBuilder().UseKestrel().UseStartup<Startup>().UseUrls("http://localhost:5055");
            //var host = new WebHostBuilder().UseKestrel().UseStartup<Startup>().UseUrls("http://192.168.1.148:5077");

            // 192.168.1.148

            //.UseApplicationBasePath(Directory.GetCurrentDirectory())
            //.UseDefaultConfiguration(args)
            //.UseIISPlatformHandlerUrl()
            //.UseStartup<Startup>()
            //.UseKestrel()
            //.UseUrls("http://localhost:5055");
            // .Build();


            return host;
            
        }

       

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
