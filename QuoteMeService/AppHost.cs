using System;
using ServiceStack;
using ServiceStack.Logging;
using System.Reflection;
using ServiceStack.Text;
using ServiceStack.Host;
using QuoteMe.ServiceInterface;
using System.Collections.Generic;
using System.Text;
using System.Net.Mime;

namespace QuoteMeService
{
    public class AppHost : AppHostHttpListenerBase
    {
        private ILog log = LogManager.GetLogger(typeof(AppHost));

        public AppHost() : base("QuoteMe Service", typeof(IQuoteMeServiceInterface).Assembly)
        {
        }

        public override void Configure(Funq.Container container)
        {
            JsConfig.EmitCamelCaseNames = true;

            var appConfig = (QuoteMeConfig)this.Container.Resolve<IQuoteMeConfig>();

            SetConfig(
                new HostConfig
            { 
                EnableFeatures = Feature.All & ~Feature.Soap,
                DefaultContentType = "application/json",
                #if DEBUG
                DebugMode = true,
                #else
                DebugMode = false,
                #endif
            });

            log.Info(appConfig.ToString());
        }
    }
}

