using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
/*
"Gradient background on a UIButton / emulating UIGlassButton"
http://www.shrinkrays.net/code-snippets/csharp/monotouch-tips-and-snippets.aspx
linked to Martin Bowling's sample using UIGlassButton (ZIP)
http://bit.ly/MTGlassButton

http://www.tuaw.com/2010/02/19/iphone-devsugar-create-shiny-buttons-easily/ (Erica Sudan) mentions
Objective-C version from @schwa (Jonathan Wight) http://pastie.org/830884
*/
namespace GlassButtonGenerator
{
	public class Application
	{
		static void Main (string[] args)
		{ UIApplication.Main (args, null, "AppDelegate"); }
	}

	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		GlassButtonViewController gbView;
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);	
			window.BackgroundColor = UIColor.White;
			gbView = new GlassButtonViewController();
			gbView.View.Bounds = new RectangleF(0,20,window.Bounds.Width, window.Bounds.Height - 20);
			window.AddSubview(gbView.View);
			window.MakeKeyAndVisible ();
			return true;
		}
		// This method is required in iPhoneOS 3.0
		public override void OnActivated (UIApplication application) {}
	}
	class GlassButtonViewController : UIViewController
	{
		string text = "Buy now!";
		string filename = "buttonDigital"; // saves as filename.png and filename@2x.png 
		string yourname = "YOUR_NAME"; // eg. /Users/YOUR_NAME/Desktop/ location to save the images to

		public override void ViewDidLoad ()
		{	
			base.ViewDidLoad ();
			// Set Tint Color
			UIColor tint = UIColor.FromHSBA(0.267f, 1.000f,0.667f,1.000f); // lime green
			//tint = UIColor.Red;

			// Regular iPad/iPhone button
			UIGlassButton glassButton = new UIGlassButton(new RectangleF(20, 50, 280, 48));
			glassButton.SetTitle(text, UIControlState.Normal);
			glassButton.TitleLabel.AdjustsFontSizeToFitWidth = true;
			glassButton.TitleLabel.MinimumFontSize = 10f;
			glassButton.TitleLabel.Font = UIFont.BoldSystemFontOfSize(16);
			glassButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			glassButton.SetValueForKey(tint, "tintColor");
			
			// iPhone4 Retina display button
			UIGlassButton glassButton2x = new UIGlassButton(new RectangleF(20, 120, 560, 96));
			glassButton2x.SetTitle(text, UIControlState.Normal);
			glassButton2x.TitleLabel.AdjustsFontSizeToFitWidth = true;
			glassButton2x.TitleLabel.MinimumFontSize = 20f;
			glassButton2x.TitleLabel.Font = UIFont.BoldSystemFontOfSize(32);
			glassButton2x.SetTitleColor(UIColor.White, UIControlState.Normal);
			glassButton2x.SetValueForKey(tint, "tintColor");

			// This is our button at the bottom that does the generating
			UIGlassButton makeGlassButton = new UIGlassButton(new RectangleF(20, this.View.Bounds.Height - 68, 280, 48));
			UIColor tint2 = UIColor.Blue;
			makeGlassButton.SetTitle("Generate Glass Button", UIControlState.Normal);
			makeGlassButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			makeGlassButton.SetValueForKey(tint2, "tintColor");
			makeGlassButton.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin;
			makeGlassButton.TouchUpInside += delegate
			{
				MakeButton (glassButton, "");
				var png = MakeButton (glassButton2x, "@2x");
				var saved = "Your UIGlassButton has been saved to -> " + png;
				if (IsSimulator)
				{
					saved = "Your UIGlassButton has been saved to -> " + 
									System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, png);
					Console.WriteLine (saved);
				}
				var alert = new UIAlertView("Saved!", saved, null, "Ok");
				alert.Show();
			};
				
			this.Add(glassButton);
			this.Add(glassButton2x);
			this.Add(makeGlassButton);
		}
		string MakeButton (UIGlassButton theButton, string fileExtension)
		{
			// Grab The Context The Size of the Button
			UIGraphics.BeginImageContext (theButton.Frame.Size);
			var ctx = UIGraphics.GetCurrentContext ();
			
			// Render the button in the context
			theButton.Layer.RenderInContext(ctx);

			// Lets grab a UIImage of the current graphics context
			UIImage i = UIGraphics.GetImageFromCurrentImageContext();
			
			Console.Write("model: "+UIDevice.CurrentDevice.Model);
			string png = "../Documents/"+filename+fileExtension+".png";	
			if (IsSimulator)
			{
				png = "/Users/"+ yourname +"/Desktop/"+filename+fileExtension+".png";
			}
			
			// Get the Image as a PNG
			NSData imgData = i.AsPNG();
			NSError err = null;
			if (imgData.Save(png, false, out err))
			{
				Console.WriteLine("saved as " + png);
			} 
			else 
			{
			 	Console.WriteLine("NOT saved as" + png + 
			                    " because" + err.LocalizedDescription);
			}
			
			UIGraphics.EndImageContext ();

			return png;
		}
		public bool IsSimulator
		{
			get
			{
				return (UIDevice.CurrentDevice.Model.ToLower().IndexOf("simulator") >= 0);
			}
		}
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
}
