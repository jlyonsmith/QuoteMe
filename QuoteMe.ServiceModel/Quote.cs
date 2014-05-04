using System;
using ServiceStack;

namespace QuoteMe.ServiceModel
{
    [Route("/quote", "GET")]
    public class QuoteRequest
    {
        public string Topic { get; set; }
    }

    public class QuoteResponse
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}

