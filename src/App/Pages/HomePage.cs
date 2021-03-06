﻿using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Bit.App.Abstractions;
using Bit.App.Resources;
using Xamarin.Forms;
using XLabs.Ioc;
using Plugin.Settings.Abstractions;
using Bit.App.Controls;
using FFImageLoading.Forms;

namespace Bit.App.Pages
{
    public class HomePage : ExtendedContentPage
    {
        private readonly IAuthService _authService;
        private readonly IUserDialogs _userDialogs;
        private readonly ISettings _settings;
        private DateTime? _lastAction;

        public HomePage()
            : base(updateActivity: false)
        {
            _authService = Resolver.Resolve<IAuthService>();
            _userDialogs = Resolver.Resolve<IUserDialogs>();
            _settings = Resolver.Resolve<ISettings>();

            Init();
        }

        public void Init()
        {
            MessagingCenter.Send(Application.Current, "ShowStatusBar", false);

            var logo = new CachedImage
            {
                Source = "logo",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 282,
                HeightRequest = 44
            };

            var message = new Label
            {
                Text = AppResources.LoginOrCreateNewAccount,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.FromHex("333333")
            };

            var createAccountButton = new ExtendedButton
            {
                Text = AppResources.CreateAccount,
                Command = new Command(async () => await RegisterAsync()),
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.Fill,
                Style = (Style)Application.Current.Resources["btn-primary"],
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button))
            };

            var loginButton = new ExtendedButton
            {
                Text = AppResources.LogIn,
                Command = new Command(async () => await LoginAsync()),
                VerticalOptions = LayoutOptions.End,
                Style = (Style)Application.Current.Resources["btn-primaryAccent"],
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button))
            };

            var buttonStackLayout = new StackLayout
            {
                Padding = new Thickness(30, 40),
                Spacing = 10,
                Children = { logo, message, createAccountButton, loginButton }
            };

            Title = AppResources.Bitwarden;
            NavigationPage.SetHasNavigationBar(this, false);
            Content = new ScrollView { Content = buttonStackLayout };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send(Application.Current, "ShowStatusBar", false);
        }

        public async Task LoginAsync()
        {
            if(_lastAction.LastActionWasRecent())
            {
                return;
            }
            _lastAction = DateTime.UtcNow;

            await Navigation.PushForDeviceAsync(new LoginPage());
        }

        public async Task RegisterAsync()
        {
            if(_lastAction.LastActionWasRecent())
            {
                return;
            }
            _lastAction = DateTime.UtcNow;

            await Navigation.PushForDeviceAsync(new RegisterPage(this));
        }

        public async Task DismissRegisterAndLoginAsync(string email)
        {
            await Navigation.PopForDeviceAsync();
            await Navigation.PushForDeviceAsync(new LoginPage(email));
            _userDialogs.Toast(AppResources.AccountCreated);
        }
    }
}
