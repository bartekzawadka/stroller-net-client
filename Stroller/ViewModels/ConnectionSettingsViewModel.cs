using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class ConnectionSettingsViewModel : DetailsScreen<ConnectionSettingsInfo>
    {
        private readonly IStrollerControlService _strollerControlService = IoC.Get<IStrollerControlService>();

        public ConnectionSettingsViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
            Load();
        }

        public void OnPasswordChanged(RoutedEventArgs args)
        {
            if (Context != null)
                Context.UploadServerPassword = ((PasswordBox) args.OriginalSource).Password;
        }

        public void Load()
        {
            Context = new ConnectionSettingsInfo
            {
                IpAddress = Bll.Properties.Settings.Default.IpAddress,
                Port = Bll.Properties.Settings.Default.Port.ToString(),
                UploadServerHostName = Bll.Properties.Settings.Default.UploadImagesTargetHost,
                UploadServerPassword = Bll.Properties.Settings.Default.UploadImagesTargetPassword,
                UploadServerPort = Bll.Properties.Settings.Default.UploadImagesTargetPort.ToString(),
                UploadServerUsername = Bll.Properties.Settings.Default.UploadImagesTargetUsername,
                UploadServerTargetIsUnix = Bll.Properties.Settings.Default.UploadImagesTargetIsUnix,
                UploadServerDestRootDir = Bll.Properties.Settings.Default.UploadImagesTargetDestRootDir
            };

        }

        public void Save()
        {
            Save(true);
        }

        public async void TestStrollerConnection()
        {
            var lastIp = Bll.Properties.Settings.Default.IpAddress;
            var lastPort = Bll.Properties.Settings.Default.Port;

            Save(false);

            await ExecuteIntederminateProcess("Checking connectivity",
                "Tesing connection with " + Context.IpAddress + ". Please wait...",
                async (token) => { await _strollerControlService.GetStatus(token); },
                async () =>
                {
                    await ShowMessage("Connection established", "Successfully connected to Stroller system");
                }, 
                async ex =>
                {
                    await ShowMessage("Connection failed",
                        "Unable to establish connection with Stroller on " + Context.IpAddress + "." +
                        Environment.NewLine + ex.Message);
                });

            Context.IpAddress = lastIp;
            Context.Port = lastPort.ToString();

            Save(false);

            NotifyOfPropertyChange(nameof(Context));
        }

        private async void Save(bool close)
        {
            var msg = Validate();
            if (!string.IsNullOrEmpty(msg.Value))
            {
                await ShowMessage(msg.Key, msg.Value);
                return;
            }

            Bll.Properties.Settings.Default.IpAddress = Context.IpAddress;
            Bll.Properties.Settings.Default.Port = int.Parse(Context.Port);

            Bll.Properties.Settings.Default.UploadImagesTargetHost = Context.UploadServerHostName;
            Bll.Properties.Settings.Default.UploadImagesTargetPassword = Context.UploadServerPassword;
            Bll.Properties.Settings.Default.UploadImagesTargetPort = int.Parse(Context.UploadServerPort);
            Bll.Properties.Settings.Default.UploadImagesTargetUsername = Context.UploadServerUsername;
            Bll.Properties.Settings.Default.UploadImagesTargetDestRootDir = Context.UploadServerDestRootDir;
            Bll.Properties.Settings.Default.UploadImagesTargetIsUnix = Context.UploadServerTargetIsUnix;

            Bll.Properties.Settings.Default.Save();

            if (close)
            {
                TryClose();
                IoC.Get<IMain>().GoHome();
            }
        }

        private KeyValuePair<string, string> Validate()
        {
            if (!IPAddress.TryParse(Context.IpAddress, out var _))
            {
                return new KeyValuePair<string, string>("Invalid address",
                    "Value '" + Context.IpAddress + "' is not a valid IP address");
            }

            if (!int.TryParse(Context.Port, out var _))
            {
                return new KeyValuePair<string, string>("Invalid port number",
                    "Stroller port number must be an integer value");
            }

            if (!string.IsNullOrEmpty(Context.UploadServerPort) && !int.TryParse(Context.UploadServerPort, out var _))
            {
                return new KeyValuePair<string, string>("Invalid port number",
                    "Images upload server port number must be an integer value");
            }

            if (!string.IsNullOrEmpty(Context.UploadServerHostName) && string.IsNullOrEmpty(Context.UploadServerUsername))
            {
                return new KeyValuePair<string, string>("Username required",
                    "Username is required for SFTP connection");
            }

            return new KeyValuePair<string, string>();
        }
    }
}
