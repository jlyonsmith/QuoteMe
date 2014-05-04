using System;
using ServiceStack;
using ServiceStack.Logging;
using System.Reflection;
using ServiceStack.Text;
using ServiceStack.Host;
using System.Collections.Generic;
using System.Text;
using System.Net.Mime;

namespace QuoteMe.ServiceInterface
{
	public interface IQuoteMeConfig
	{
        string ServiceUrl { get; }
        string DataFilePath { get; }
	}
}

