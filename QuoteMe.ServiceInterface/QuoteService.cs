using System;
using ServiceStack;
using QuoteMe.ServiceModel;
using ServiceStack.Text;
using System.IO;
using QuoteMe.DataModel;

namespace QuoteMe.ServiceInterface
{
    public class QuoteService : Service
    {
        public IQuoteMeConfig Config { get; set; }

        public QuoteResponse Get(QuoteRequest request)
        {
            QuoteData quoteData = null;

            using (StreamReader reader = new StreamReader(Config.DataFilePath))
            {
                quoteData = JsonSerializer.DeserializeFromReader<QuoteData>(reader);
            }

            return quoteData.Quotes[new Random().Next(quoteData.Quotes.Count - 1)]
                .ConvertTo<ServiceModel.QuoteResponse>();
        }
    }
}

