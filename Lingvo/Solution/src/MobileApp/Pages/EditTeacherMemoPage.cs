﻿using Lingvo.Common.Enums;
using Lingvo.MobileApp.Controllers;
using Lingvo.MobileApp.Forms;
using Lingvo.MobileApp.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;

namespace Lingvo.MobileApp.Pages
{
    class EditTeacherMemoPage : ContentPage
    {

        private static readonly int ControlButtonSize = Device.OnPlatform(iOS: 75, Android: 86, WinPhone: 150);
        private static readonly int EditButtonSize = Device.OnPlatform(iOS: 25, Android: 35, WinPhone: 50);

        private LingvoRoundImageButton RecordButton
        {
            get; set;
        }

        private Label Label
        {
            get; set;
        }

        private Entry Name
        {
            get; set;
        }
        private Label NameLabel
        {
            get; set;
        }

        private LingvoRoundImageButton EditButton
        {
            get; set;
        }
        private ToolbarItem SaveItem
        {
            get; set;
        }

        public EditTeacherMemoPage(TeacherMemo memo) : this()
        {
            NameLabel.IsVisible = true;
            Name.Text = memo.Name;
            Name.IsVisible = false;
            EditButton.IsVisible = true;
            NameLabel.Text = memo.Name;
            ToolbarItems.Clear();
            RecordButton.IsEnabled = false;
            Progress_Update(memo.TeacherTrack.Duration);
            SaveItem.Clicked -= SaveItem_Clicked;
            SaveItem.Clicked += async (o, e) =>
            {
                if (Name.Text.Length > 0 && !await checkNameExists(Name.Text))
                {
                    //TODO: SIMPLY CREATE A FUCKIN' WORKING DATABASE ACCESS ARCHITECTURE.
                    memo.Name = Name.Text;
                    App.Database.Save(memo);
                    await Navigation.PopAsync();
                }
            };
        }

        public EditTeacherMemoPage() : base()
        {
            Title = ((Span)App.Current.Resources["page_title_recordTeacherMemo"]).Text;

            TeacherMemoController.Instance.Reset();

            SaveItem = new ToolbarItem
            {
                Text = ((Span)App.Current.Resources["label_save"]).Text,
                Icon = "ic_action_tick.png"
            };

            SaveItem.Clicked += SaveItem_Clicked;

            ToolbarItems.Add(SaveItem);

            RecordButton = new LingvoRoundImageButton()
            {
                Image = LingvoRoundImageButton.RecordImage,
                Color = Color.Red,
                Border = true,
                WidthRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                HeightRequest = Device.OnPlatform(iOS: ControlButtonSize, Android: ControlButtonSize, WinPhone: 2 * ControlButtonSize),
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            RecordButton.OnClicked += RecordButton_OnClicked;

            Label = new Label()
            {
                Text = "00:00",
                FontSize = Device.OnPlatform(iOS: 80, Android: 100, WinPhone: 200),
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            TeacherMemoController.Instance.Update += Progress_Update;

            Name = new Entry()
            {
                Placeholder = "Name der Lehrerspur",
                PlaceholderColor = Color.Gray,
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry))
            };

            NameLabel = new Label()
            {
                TextColor = (Color)App.Current.Resources["primaryColor"],
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Entry)),
                IsVisible = false
            };

            EditButton = new LingvoRoundImageButton()
            {
                Border = false,
                Image = "ic_edit.png",
                Color = (Color)App.Current.Resources["primaryColor"],
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = EditButtonSize,
                HeightRequest = EditButtonSize,
                IsVisible = false
            };

            EditButton.OnClicked += EditButton_OnClicked;

            Content = new StackLayout()
            {
                Padding = new Thickness(15, 40),
                Children =
                {
                    new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.StartAndExpand,
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            Name,
                            NameLabel,
                            EditButton
                        }
                    },
                    Label,
                    RecordButton
                }
            };
        }
        private void EditButton_OnClicked(object sender, EventArgs e)
        {
            NameLabel.IsVisible = false;
            Name.IsVisible = true;
            EditButton.IsEnabled = false;
            ToolbarItems.Add(SaveItem);
        }

        private void Progress_Update(int progress)
        {
            int minutes = progress / 60000;
            int seconds = Math.Abs(progress / 1000) % 60;
            string minuteString = (Math.Abs(minutes) < 10 ? "0" + minutes : "" + minutes);
            string secondString = (seconds < 10 ? "0" + seconds : "" + seconds);
            Device.BeginInvokeOnMainThread(() => Label.Text = minuteString + ":" + secondString);
        }

        private async Task<bool> checkNameExists(string name)
        {
            if (LocalCollection.Instance.TeacherMemos.FirstOrDefault(m => m.Name.Equals(Name.Text)) != null)
            {
                string title = ((Span)App.Current.Resources["label_nameAlreadyExists"]).Text;
                string desc = ((Span)App.Current.Resources["desc_teacherTrackNameAlreadyExists"]).Text;
                string ok = ((Span)App.Current.Resources["label_ok"]).Text;
                await DisplayAlert(title, desc, ok);
                return true;
            }
            return false;
        }

        private async void SaveItem_Clicked(object sender, EventArgs e)
        {
            if (Name.Text.Length > 0 && !await checkNameExists(Name.Text))
            {
                if (TeacherMemoController.Instance.CurrentMemo != null)
                {
                    TeacherMemoController.Instance.SaveTeacherMemo(Name.Text);
                    await Navigation.PopAsync();
                }
            }
        }

        private async void RecordButton_OnClicked(object sender, EventArgs e)
        {
            RecorderState currentState = TeacherMemoController.Instance.State;
            if (currentState != RecorderState.RECORDING)
            {
                if (TeacherMemoController.Instance.CurrentMemo != null)
                {
                    string title = ((Span)App.Current.Resources["label_warning"]).Text;
                    string desc = ((Span)App.Current.Resources["desc_teacherMemoAlreadyExists"]).Text;
                    string accept = ((Span)App.Current.Resources["label_overwrite"]).Text;
                    string cancel = ((Span)App.Current.Resources["label_cancel"]).Text;

                    if (!await DisplayAlert(title, desc, accept, cancel))
                    {
                        return;
                    }
                }
                TeacherMemoController.Instance.StartTeacherMemo();
                RecordButton.Image = LingvoRoundImageButton.StopImage;
            }
            else
            {
                TeacherMemoController.Instance.EndTeacherMemo();
                Progress_Update(TeacherMemoController.Instance.CurrentMemo.TeacherTrack.Duration);
                RecordButton.Image = LingvoRoundImageButton.RecordImage;
                return;
            }
        }
    }
}
