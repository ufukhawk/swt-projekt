﻿using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Proxies;
using Lingvo.MobileApp.Services;
using Lingvo.MobileApp.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoDownloadPageViewCell : LingvoPageViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);

        private static readonly FileImageSource cancelImage = (FileImageSource)ImageSource.FromFile("ic_action_cancel.png");
        private static readonly FileImageSource downloadImage = (FileImageSource)ImageSource.FromFile("ic_action_download.png");

        private LingvoRoundImageButton downloadButton;
        private CancellationTokenSource cancellationToken;

        public LingvoDownloadPageViewCell() : base()
        {
            downloadButton = new LingvoRoundImageButton()
            {
                HorizontalOptions = LayoutOptions.End,
                Color = (Color)App.Current.Resources["primaryColor"],
                WidthRequest = DownloadButtonSize,
                HeightRequest = DownloadButtonSize,
                VerticalOptions = LayoutOptions.Center
            };

            downloadButton.OnClicked += (o, e) => DownloadButton_Clicked();

            ((StackLayout)View).Children.Add(downloadButton);
        }

        private async void DownloadButton_Clicked()
        {

            if (cancellationToken == null)
            {
                if (!await AlertHelper.DisplayWarningIfNotWifiConnected())
                {
                    return;
                }
                cancellationToken = new CancellationTokenSource();
                cancellationToken.Token.Register(() => Device.BeginInvokeOnMainThread(() =>
                {
                    downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png");
                    OnBindingContextChanged();
                }));
                downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_cancel.png");

                await ((PageProxy)BindingContext).Resolve(cancellationToken.Token);
            }
            else
            {
                cancellationToken.Cancel();
            }
            cancellationToken = null;
            Device.BeginInvokeOnMainThread(() => downloadButton.Image = (FileImageSource)ImageSource.FromFile("ic_action_download.png"));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ProgressHolder.Instance.SetPageProgressListener((IPage)BindingContext, OnProgressUpdate);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ProgressHolder.Instance.UnsetPageProgressListener((IPage)BindingContext);
        }

        private void OnProgressUpdate(double progress)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ProgressView.Progress = (int)progress;
            });
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            IPage page = (IPage)BindingContext;

            Workbook localWorkbook = LocalCollection.Instance.Workbooks.FirstOrDefault(w => w.Id.Equals(page.workbookId));
            bool downloaded = localWorkbook?.Pages.Find(p => p.Id.Equals(page.Id)) != null;

            string color = downloaded ? "secondaryColor" : "primaryColor";
            ProgressView.OuterProgressColor = (Color)App.Current.Resources[color];

            ProgressView.InnerProgressEnabled = false;
            ProgressView.MaxProgress = 100;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.Percentual;

            try
            {
                if (ContextActions.Count > 0)
                {
                    ContextActions.Clear();
                }
            }
            catch
            {
                Console.WriteLine("Context Actions null");
            }

            int progress = 0;
            if (ProgressHolder.Instance.HasPageProgress(page.Id))
            {
                progress = (int)ProgressHolder.Instance.GetPageProgress(page.Id).CurrentProgress;
            }

            if (ProgressHolder.Instance.HasPageProgress(page.Id) && cancellationToken != null)
            {
                downloadButton.Image = cancelImage;
            }
            else
            {
                downloadButton.Image = downloadImage;
            }

            ProgressView.Progress = downloaded ? 100 : progress;
            downloadButton.IsEnabled = !downloaded && (!ProgressHolder.Instance.HasPageProgress(page.Id) || cancellationToken != null);
        }

        protected override void Event_PageChanged(IPage p)
        {
            IPage page = (IPage)BindingContext;
            if (p.Id.Equals(page.Id))
            {
                OnBindingContextChanged();
            }
        }
    }
}
