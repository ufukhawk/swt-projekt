using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Lingvo.MobileApp.LingvoAudioProgressView), typeof(Lingvo.MobileApp.Droid.LingvoAudioProgressViewRenderer))]
namespace Lingvo.MobileApp.Droid
{
    class LingvoAudioProgressViewRenderer : ViewRenderer<LingvoAudioProgressView, Android.Views.View>
    {
        AndroidLingvoAudioProgressView progressView;

        protected override void OnElementChanged(ElementChangedEventArgs<LingvoAudioProgressView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                progressView = new AndroidLingvoAudioProgressView(Context);
                SetNativeControl(progressView); 
            }

            if (e.OldElement != null && e.NewElement == null)
            {
                e.OldElement.PropertyChanged -= updateView;
                progressView.StudentTrackMuted -= e.OldElement.OnStudentTrackMuted;
            }
            else if (e.NewElement != null)
            {
                e.NewElement.PropertyChanged += updateView;
                progressView.StudentTrackMuted += e.NewElement.OnStudentTrackMuted;
            }
        }

        private void updateView(object sender, EventArgs e)
        {
            LingvoAudioProgressView element = (LingvoAudioProgressView)sender;

            if (progressView.InnerProgressEnabled != element.InnerProgressEnabled)
                progressView.InnerProgressEnabled = element.InnerProgressEnabled;
            if (progressView.Size != element.Size)
                progressView.Size = element.Size;
            if (!progressView.OuterProgressColor.Equals(element.OuterProgressColor.ToAndroid()))
                progressView.OuterProgressColor = element.OuterProgressColor.ToAndroid();
            if (!progressView.InnerProgressColor.Equals(element.InnerProgressColor.ToAndroid()))
                progressView.InnerProgressColor = element.InnerProgressColor.ToAndroid();
            if (progressView.Max != element.MaxProgress)
                progressView.Max = element.MaxProgress;
            if (progressView.Progress != element.Progress)
                progressView.Progress = element.Progress;
            if (progressView.InnerMuteButtonVisible != element.InnerProgressEnabled)
                progressView.InnerMuteButtonVisible = element.InnerProgressEnabled;
        }
    }
}