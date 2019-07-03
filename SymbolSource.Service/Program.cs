using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SymbolSource.Service
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new SymbolSource()
            };

            if (Environment.UserInteractive && System.Diagnostics.Debugger.IsAttached)
            {
                // Simule l'exécution des services
                RunInteractiveServices(ServicesToRun);
            }
            else
            {
                // Exécute les services normalement
                ServiceBase.Run(ServicesToRun);
            }
        }

        static void RunInteractiveServices(ServiceBase[] servicesToRun)
        {
            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);

            // Boucle de démarrage des services
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Starting {0} ... ", service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.WriteLine("Done");
            }

            Console.WriteLine();
            Console.WriteLine("Press key to stop...");
            Console.ReadKey();
            Console.WriteLine();


            MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Stopping {0} ... ", service.ServiceName);
                onStopMethod.Invoke(service, null);
                Console.WriteLine("OK");
            }
        }
    }
}
