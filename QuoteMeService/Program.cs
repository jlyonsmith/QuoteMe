using System;
using System.Reflection;
using System.Text;
using Funq;
using Mono.Unix;
using Mono.Unix.Native;
using QuoteMe.ServiceInterface;
using ServiceStack.Logging;
using ServiceStack.Logging.NLogger;
using ServiceStack.Text;

namespace QuoteMeService
{
    class Program
    {
        static void Main(string[] args)
        {
            LogManager.LogFactory = new NLogFactory();

            var appHost = new AppHost();
            var appConfig = new QuoteMeConfig(args.Length > 0 ? args[0] : null, args.Length > 1 ? args[1] : null);
            appHost.Container.Register<IQuoteMeConfig>(appConfig);
            appHost.Init();
            appHost.Start(appConfig.ServiceUrl);

            UnixSignal[] signals = new UnixSignal[] { 
                new UnixSignal(Signum.SIGINT), 
                new UnixSignal(Signum.SIGTERM), 
            };

            // Wait for a unix signal
            for (bool exit = false; !exit;)
            {
                int id = UnixSignal.WaitAny(signals);

                if (id >= 0 && id < signals.Length)
                {
                    if (signals[id].IsSet)
                        exit = true;
                }
            }
        }
    }
}
