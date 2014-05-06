using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using ServiceStack;
using QuoteMe.ServiceModel;

namespace QuoteMe
{
    public partial class QuoteMeViewController : UIViewController
    {
        JsonServiceClient client;

        public QuoteMeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            IosPclExportClient.Configure();
            //client = new JsonServiceClient("http://ec2-54-185-30-234.us-west-2.compute.amazonaws.com:1337/");
            client = new JsonServiceClient("http://localhost:1337/");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        partial void UIButton8_TouchUpInside(UIButton sender)
        {
            client.GetAsync<QuoteResponse>(new QuoteRequest { Topic = "Funny" })
                .Success(response => 
                {
                    textViewQuote.Text = response.Text;
                    textViewAuthor.Text = response.Author;
                })
                .Error(ex => 
                {
                    textViewQuote.Text = ex.ToString();
                });
        }
        #endregion
    }
}

