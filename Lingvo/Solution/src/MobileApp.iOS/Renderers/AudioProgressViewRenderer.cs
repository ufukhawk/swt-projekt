﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using CoreAnimation;
using CoreFoundation;
using UIKit;


[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoAudioProgressView), typeof(Lingvo.MobileApp.iOS.AudioProgressViewRenderer))]
namespace Lingvo.MobileApp.iOS
{
	class AudioProgressViewRenderer : ViewRenderer<LingvoAudioProgressView, UIView>
	{
		AudioProgressView progressView;

		protected override void OnElementChanged(ElementChangedEventArgs<LingvoAudioProgressView> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				progressView = new AudioProgressView(Frame);
				SetNativeControl(progressView);
			}

			if (e.OldElement != null)
			{
				e.OldElement.PropertyChanged -= updateView;
				progressView.StudentTrackMuted -= e.OldElement.OnStudentTrackMuted;
				e.OldElement.SizeChanged -= NewElementOnSizeChanged;
			}
			else if (e.NewElement != null)
			{

				e.NewElement.PropertyChanged += updateView;
				progressView.StudentTrackMuted += e.NewElement.OnStudentTrackMuted;
				e.NewElement.SizeChanged += NewElementOnSizeChanged;
			}
		}

		private void updateView(object sender, EventArgs e)
		{
			if (Control == null)
			{
				return;
			}
			LingvoAudioProgressView element = (LingvoAudioProgressView)sender;
		
			if (progressView.InnerProgressEnabled != element.InnerProgressEnabled)
				progressView.InnerProgressEnabled = element.InnerProgressEnabled;
			if (progressView.Size != element.Size)
				progressView.Size = element.Size;
			if (!progressView.OuterProgressColor.Equals(element.OuterProgressColor.ToUIColor()))
				progressView.OuterProgressColor = element.OuterProgressColor.ToUIColor();
			if (!progressView.InnerProgressColor.Equals(element.InnerProgressColor.ToUIColor()))
				progressView.InnerProgressColor = element.InnerProgressColor.ToUIColor();
			if (progressView.MaxProgress != element.MaxProgress)
				progressView.MaxProgress = element.MaxProgress;
			if (progressView.Progress != element.Progress)
				progressView.Progress = element.Progress;
			
		}
		private void NewElementOnSizeChanged(object sender, EventArgs eventArgs)
		{
			var audioProgressView = sender as LingvoAudioProgressView;

			if (audioProgressView != null)
			{
				var frame = new CGRect(audioProgressView.X, audioProgressView.X, audioProgressView.Width, audioProgressView.Height);
				progressView.Frame = frame;
				progressView.render();

			}
		}
		public override void LayoutSublayersOfLayer(CALayer layer)
		{
			base.LayoutSublayersOfLayer(layer);

			progressView.Frame = layer.Bounds;
			progressView.teacherProgressBar.Frame = layer.Bounds;
			progressView.studentProgressBar.Frame = layer.Bounds;
			progressView.teacherProgressBar.render();
			progressView.studentProgressBar.render();

		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

		}
	}
}