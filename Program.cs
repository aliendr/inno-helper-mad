using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace InnoHelp.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateHostBuilder(string[] args)
		{
			var port = Environment.GetEnvironmentVariable("PORT");

			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseUrls("http://*:" + port);
			//Host.CreateDefaultBuilder(args)
			//	.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
		}
	}
}
