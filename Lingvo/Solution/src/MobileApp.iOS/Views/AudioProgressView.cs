﻿using System;
using UIKit;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;


namespace Lingvo.MobileApp.iOS
{
	public class AudioProgressView : UIView
	{

		public CircleProgressBar teacherProgressBar;
		public CircleProgressBar studentProgressBar;

		private static UIColor studentColor = UIColor.Red;
		private static UIColor teacherColor = new UIColor(74.0f / 255.0f, 144.0f / 255.0f, 226.0f / 255.0f, 1.0f);

		private float lineWidth = 10.0f;
		public bool innerProgressEnabled = true;

		private int progress = 0;
		int maxProgress = 100;



		public delegate void StudentTrackMutedEventHandler(bool muted);
		public event StudentTrackMutedEventHandler StudentTrackMuted;

		UIButton muteBtn = new Func<UIButton>(() =>
		{
			var btn = new UIButton(new CGRect(0, 0, 50, 50));
			btn.SetTitleColor(teacherColor, UIControlState.Normal);
			var btnImage = new UIImage("ic_volume_up");
			btn.SetImage(btnImage, UIControlState.Normal);
			return btn;
		})();
		UILabel timeLabel = new UILabel()
		{
			Text = "00:00",
			TextColor = teacherColor,
			Font = UIFont.SystemFontOfSize(28, UIFontWeight.Thin)
		};
		UIStackView stackView = new UIStackView()
		{
			Axis = UILayoutConstraintAxis.Vertical,
			Alignment = UIStackViewAlignment.Center
		};

		public AudioProgressView(CGRect frame) : base(frame)
		{
			setupViews();
		}


		public void setupViews()
		{
			teacherProgressBar = new CircleProgressBar(Frame);
			teacherProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			teacherProgressBar.ProgressColor = teacherColor;
			teacherProgressBar.MaxProgress = maxProgress;
			teacherProgressBar.Progress = progress;
			teacherProgressBar.LineWidth = lineWidth;
			AddSubview(teacherProgressBar);

			studentProgressBar = new CircleProgressBar(Frame);
			studentProgressBar.TranslatesAutoresizingMaskIntoConstraints = false;
			studentProgressBar.ProgressColor = studentColor;
			studentProgressBar.NestingLevel = 1;
			studentProgressBar.backgroundLayer.SetNeedsDisplay();
			studentProgressBar.MaxProgress = maxProgress;
			studentProgressBar.Progress = progress;
			studentProgressBar.LineWidth = lineWidth;
			studentProgressBar.Hidden = !innerProgressEnabled;
			AddSubview(studentProgressBar);

			//layout views programatically
			//create Autolayout constraints
			var viewNames = NSDictionary.FromObjectsAndKeys(new NSObject[] {
				teacherProgressBar,
				studentProgressBar
			}, new NSObject[] {
				new NSString("teacher_bar"),
				new NSString("student_bar")
			});
			var emptyDict = new NSDictionary();

			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[teacher_bar]|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[teacher_bar]|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[student_bar]|", 0, emptyDict, viewNames));
			AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[student_bar]|", 0, emptyDict, viewNames));
			studentProgressBar.CenterXAnchor.ConstraintEqualTo(teacherProgressBar.CenterXAnchor).Active = true;
			studentProgressBar.CenterYAnchor.ConstraintEqualTo(teacherProgressBar.CenterYAnchor).Active = true;


			//Wrap label in vertical stack view to avoid repositioning the label programatically
		
			AddSubview(stackView);
			stackView.AddArrangedSubview(timeLabel);
			stackView.AddArrangedSubview(muteBtn);

			//subscribe to click events of button in the middle
			muteBtn.AddTarget(OnMuteButtonClicked, UIControlEvent.TouchUpInside);

			updateMuteButtonImage();


			//layout stack view using Autolayout
			stackView.TranslatesAutoresizingMaskIntoConstraints = false;
			stackView.CenterXAnchor.ConstraintEqualTo(CenterXAnchor).Active = true;
			stackView.CenterYAnchor.ConstraintEqualTo(CenterYAnchor).Active = true;

			if (!innerProgressEnabled)
			{
				studentProgressBar.Muted = true;
			}
		}
		private void OnMuteButtonClicked(object sender, EventArgs e)
		{
			innerProgressEnabled = !innerProgressEnabled;
			StudentTrackMuted?.Invoke(!innerProgressEnabled);

			updateMuteButtonImage();

			studentProgressBar.Muted = !innerProgressEnabled;
			studentProgressBar.render();

		}
		private void updateMuteButtonImage()
		{
			var imageName = innerProgressEnabled ? "ic_volume_up" : "ic_volume_off";
			var image = new UIImage(imageName);
			muteBtn.SetImage(image, UIControlState.Normal);
		}
		public bool MuteEnabled
		{
			get { 
				return !muteBtn.Hidden;
			}
			set 
			{ 
				muteBtn.Hidden = !value;
			}
		}
		public int Progress
		{
			get
			{
				return progress;
			}
			set
			{
				var modValue = Math.Min(MaxProgress, value);
				progress = modValue;
				teacherProgressBar.Progress = modValue;

				if (innerProgressEnabled)
				{
					studentProgressBar.Progress = modValue;
					studentProgressBar.strokeLayer.SetNeedsDisplay();
					studentProgressBar.SetNeedsDisplay();

				}
			}
		}

		public int TextSize
		{
			get
			{
				return (int)timeLabel.Font.PointSize;
			}
			set
			{
				timeLabel.Font = UIFont.SystemFontOfSize(value, UIFontWeight.Thin);
			}
		}

        public string Text
        {
            get { return timeLabel.Text; }
            set
            {
                timeLabel.Text = value;
            }
        }

		protected void runOnMainThread(Action action)
		{
			//updates on UI only work on the main thread
			DispatchQueue.MainQueue.DispatchAsync(action);
		}

		public int MaxProgress
		{
			get
			{
				return maxProgress;
			}
			set
			{
				maxProgress = value;
				studentProgressBar.MaxProgress = value;
				teacherProgressBar.MaxProgress = value;
			}
		}
		public float LineWidth
		{
			get
			{
				return lineWidth;
			}
			set
			{
				lineWidth = value;


				teacherProgressBar.LineWidth = value;
				studentProgressBar.LineWidth = value;
			}
		}
		public bool InnerProgressEnabled
		{
			get
			{
				var readValue = innerProgressEnabled;
				return readValue;
			}
			set
			{
				innerProgressEnabled = value;
				render();
			}
		}

		public int Size
		{
			get
			{
				return (int)Frame.Width;
			}
			set
			{
				Frame = new CGRect(Frame.X, Frame.Y, value, value);
				studentProgressBar.Size = value;
				studentProgressBar.Size = value;
			}
		}

		public UIColor InnerProgressColor
		{
			get
			{
				return studentProgressBar.ProgressColor;
			}
			set
			{
				studentColor = value;
				studentProgressBar.ProgressColor = value;
				SetNeedsDisplay();
			}
		}
		public UIColor OuterProgressColor
		{
			get { return teacherProgressBar?.ProgressColor; }
			set
			{
				teacherProgressBar.ProgressColor = value;
				teacherColor = value;
				SetNeedsDisplay();
				timeLabel.TextColor = value;
				muteBtn.SetTitleColor(value, UIControlState.Normal);
			}
		}

		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);
			if (layer == Layer)
			{

				if (studentProgressBar != null)
				{
					studentProgressBar.Frame = Frame;
					studentProgressBar.render();
					studentProgressBar.SetNeedsDisplay();
				}

				if (teacherProgressBar != null)
				{
					teacherProgressBar.Frame = Frame;
					teacherProgressBar.render();

				}
			}
		}
		public void render()
		{
			stackView.RemoveFromSuperview();
			stackView.RemoveArrangedSubview(muteBtn);
			stackView.RemoveArrangedSubview(timeLabel);
			muteBtn.RemoveTarget(OnMuteButtonClicked, UIControlEvent.TouchUpInside);
			teacherProgressBar?.RemoveFromSuperview();
			studentProgressBar?.RemoveFromSuperview();



			setupViews();
		}

	}
}
