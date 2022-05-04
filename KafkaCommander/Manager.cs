using KafkaSpace;
using Microsoft.Extensions.Configuration;
using Model;
using IO;
namespace KafkaCommander
{
   
    internal static class Manager
    {
        internal static void Run() {

            var cfg = GetConfig();
            var k = new KafkaExchanges(cfg);         
           
            Task t = new Task(() => {
                if(!cfg.isConsole)
                    Console.WriteLine("Wait request...");
                k.Consumer();
            });

            if (cfg.isConsole)
            {
                var w = new IOWork();
                Console.WriteLine(w.GetApplicationHelp());                
            }

            Task tp = new Task(() => {
                k.Producer();
            });
                       
            t.Start();
            tp.Start();
            tp.Wait();
            t.Wait();
        }

        private static Settings GetConfig() {
            IConfiguration config = new ConfigurationBuilder()
                                          .AddJsonFile("appsettings.json")
                                          .AddEnvironmentVariables()
                                          .Build();
            Settings settings = config.GetRequiredSection("Settings").Get<Settings>();
            return settings;
        }
    }
}
