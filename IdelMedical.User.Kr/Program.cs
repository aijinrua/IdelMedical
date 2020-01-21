using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdelMedical.User.Kr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
#if DEBUG
                        .UseKestrel(kestrelServerOption =>
                        {
                            kestrelServerOption.ListenAnyIP(80);
                            kestrelServerOption.ListenAnyIP(443, listenOption =>
                            {
                                listenOption.UseHttps(@"D:\Projects\SSL\certificate.pfx", "Rhkdwls2@");
                            });
                        })
#endif
                        .UseStartup<Startup>();
                });
    }
}
