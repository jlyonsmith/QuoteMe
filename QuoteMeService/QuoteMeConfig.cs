using System;
using System.Configuration;
using ServiceStack.Configuration;
using System.Collections.Generic;
using QuoteMe.ServiceInterface;

namespace QuoteMeService
{
    public class QuoteMeConfig : IQuoteMeConfig
    {
        public string ServiceUrl { get; private set; }
        public string DataFilePath { get; private set; }
        public AppSettings Settings { get; private set; }

        public QuoteMeConfig(string arg0)
        {
            Settings = new AppSettings();
            this.ServiceUrl = arg0 ?? Settings.Get("ServiceUrl", "http://*:1337/");
            this.DataFilePath = Settings.Get("DataFilePath", "./Quotes.json");
        }

        public override string ToString()
        {
            return string.Format("[QuoteMeConfig: ServiceUrl={0}, DataFilePath={1}]", ServiceUrl, DataFilePath);
        }
    }
}

