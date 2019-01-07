﻿using System;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

// Source: https://github.com/MitchMilam/Drawit
namespace Moments.iOS
{
	// Original Source: http://stackoverflow.com/questions/21029440/xamarin-ios-drawing-onto-image-after-scaling-it
	public class DrawView : UIView
	{
		public DrawView (CGRect frame) : base (frame)
		{
			DrawPath = new CGPath ();
			CurrentLineColor = UIColor.Yellow;
			PenWidth = 5.0f;
			Lines = new List<VESLine> ();
		}

		private CGPoint PreviousPoint;
		private CGPath DrawPath;
		private byte IndexCount;
		private UIBezierPath CurrentPath;
		private List<VESLine> Lines;

		public UIColor CurrentLineColor { get; set; }
		public float PenWidth { get; set; }

		public void Clear ()
		{
			DrawPath.Dispose ();
			DrawPath = new CGPath ();
			SetNeedsDisplay ();
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			IndexCount++;

			var path = new UIBezierPath
			{
				LineWidth = PenWidth
			};

			var touch = (UITouch)touches.AnyObject;
			PreviousPoint = touch.PreviousLocationInView (this);

			var newPoint = touch.LocationInView (this);
			path.MoveTo (newPoint);

			InvokeOnMainThread (SetNeedsDisplay);

			CurrentPath = path;

			var line = new VESLine
			{
				Path = CurrentPath, 
				Color = CurrentLineColor,
				Index = IndexCount 
			};

			Lines.Add (line);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			var touch = (UITouch)touches.AnyObject;
			var currentPoint = touch.LocationInView (this);

			if (Math.Abs (currentPoint.X - PreviousPoint.X) >= 4 ||
				Math.Abs (currentPoint.Y - PreviousPoint.Y) >= 4) {

				var newPoint = new CGPoint ((currentPoint.X + PreviousPoint.X) / 2, (currentPoint.Y + PreviousPoint.Y) / 2);

				CurrentPath.AddQuadCurveToPoint (newPoint, PreviousPoint);
				PreviousPoint = currentPoint;
			} else {
				CurrentPath.AddLineTo (currentPoint);
			}

			InvokeOnMainThread (SetNeedsDisplay);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			InvokeOnMainThread (SetNeedsDisplay);
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			InvokeOnMainThread (SetNeedsDisplay);
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			foreach (var line in Lines) {
				line.Color.SetStroke ();
				line.Path.Stroke ();
			}
		}
	}

	public class VESLine
	{
		public UIBezierPath Path { get; set; }
		public UIColor Color { get; set; }
		public byte Index { get; set; }
	}
}