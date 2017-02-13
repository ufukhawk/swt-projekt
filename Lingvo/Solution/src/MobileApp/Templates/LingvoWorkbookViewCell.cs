﻿using Lingvo.Common.Entities;
using Lingvo.MobileApp.Entities;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lingvo.MobileApp.Templates
{
    class LingvoWorkbookViewCell : ViewCell
    {
        private static readonly int DownloadButtonSize = Device.OnPlatform(iOS: 55, Android: 65, WinPhone: 110);
        internal LingvoAudioProgressView ProgressView
        {
            get; private set;
        }

        private Label subtitleLabel;

        private MenuItem deleteAction;

        public LingvoWorkbookViewCell() :
            base()
        {
            Label titleLabel = new Label()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                LineBreakMode = LineBreakMode.WordWrap
            };

            titleLabel.SetBinding(Label.TextProperty, "Title");

            subtitleLabel = new Label()
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                IsVisible = false
            };

            subtitleLabel.SetBinding(Label.TextProperty, "Subtitle");

            ProgressView = new LingvoAudioProgressView()
            {
                Size = Device.OnPlatform(iOS: 80, Android: 120, WinPhone: 240),
                LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM,
                MuteEnabled = false,
                InnerProgressEnabled = false
            };

            LocalCollection.Instance.WorkbookChanged += Event_WorkbookChanged;
            LocalCollection.Instance.PageChanged += Event_PageChanged;


            deleteAction = new MenuItem
            {
                Text = ((Span)App.Current.Resources["label_delete"]).Text,
                Icon = "ic_delete.png",
                IsDestructive = true
            };

            deleteAction.Clicked += (o, e) =>
            {
                LocalCollection.Instance.DeleteWorkbook((Workbook)BindingContext);
            };

            ContextActions.Add(deleteAction);

            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = DownloadButtonSize });








            var stackLayout = new StackLayout
            {
                Padding = new Thickness(5, 5),
                HeightRequest = Device.OnPlatform(iOS: 70, Android: 72, WinPhone: 260),
                Orientation = StackOrientation.Horizontal,
                Children =
                                {
                                    ProgressView,
                                    new StackLayout
                                    {
                                        HorizontalOptions = LayoutOptions.StartAndExpand,
                                        VerticalOptions = LayoutOptions.Center,
                                        Spacing = 0,
                                        Children =
                                        {
                                            titleLabel,
                                            subtitleLabel
                                        }
                                        }

                                }

            };

            grid.Children.Add(stackLayout, 0, 0);
            View = grid;
        }

        protected virtual void Event_PageChanged(Lingvo.Common.Entities.Page p)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (p.workbookId.Equals(workbook.Id))
            {
                Workbook local = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(p.workbookId));

                BindingContext = local != null ? local : p.Workbook;
            }
        }

        protected virtual void Event_WorkbookChanged(Workbook w)
        {
            Workbook workbook = (Workbook)BindingContext;
            if (w.Id.Equals(workbook.Id))
            {
                Workbook local = LocalCollection.Instance.Workbooks.FirstOrDefault(lwb => lwb.Id.Equals(w.Id));

                BindingContext = local != null ? local : w;
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            Workbook workbook = (Workbook)BindingContext;

            int completed = 0;
            workbook.Pages.ForEach((p) => { if (p.StudentTrack != null) completed++; });
            ProgressView.OuterProgressColor = (Color)App.Current.Resources["secondaryColor"];
            ProgressView.MaxProgress = workbook.Pages.Count;
            ProgressView.Progress = completed;
            ProgressView.InnerProgressEnabled = false;
            ProgressView.LabelType = LingvoAudioProgressView.LabelTypeValue.NOfM;

            subtitleLabel.IsVisible = workbook.Subtitle?.Length > 0;
        }

    }
}
