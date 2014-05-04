// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace QuoteMe
{
	[Register ("QuoteMeViewController")]
	partial class QuoteMeViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView textViewAuthor { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView textViewQuote { get; set; }

		[Action ("UIButton8_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton8_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (textViewAuthor != null) {
				textViewAuthor.Dispose ();
				textViewAuthor = null;
			}
			if (textViewQuote != null) {
				textViewQuote.Dispose ();
				textViewQuote = null;
			}
		}
	}
}
