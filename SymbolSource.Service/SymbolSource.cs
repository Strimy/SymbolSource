using Autofac;
using SymbolSource.Contract;
using SymbolSource.Contract.Container;
using SymbolSource.Contract.Scheduler;
using SymbolSource.Processor;
using SymbolSource.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SymbolSource.Service
{
    public partial class SymbolSource : ServiceBase
    {
        private CancellationTokenSource _stopSource;

        public SymbolSource()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            foreach (var assembly in typeof(PackageProcessor).Assembly.GetReferencedAssemblies())
                Trace.WriteLine(assembly.FullName);

            _stopSource = new CancellationTokenSource();


            var configuration = new DefaultConfigurationService();
            var builder = new ContainerBuilder();

            DefaultContainerBuilder.Register(builder, configuration);
            SupportContainerBuilder.Register(builder, SupportEnvironment.WebJob);
            PackageProcessorContainerBuilder.Register(builder);

            var container = builder.Build();

            var support = container.Resolve<ISupportConfiguration>();

            var scheduler = container.Resolve<ISchedulerService>();
            Task.Run(() =>
            {
                int i = 0;
                while (!_stopSource.IsCancellationRequested)
                {
                    try
                    {
                        scheduler.ListenAndProcess(_stopSource.Token);
                    }
                    catch
                    {
                        i++;
                        if(i > 10)
                        {
                            Stop();
                        }
                    }
                }
            });

        }

        protected override void OnStop()
        {
            _stopSource.Cancel();
        }
    }
}
