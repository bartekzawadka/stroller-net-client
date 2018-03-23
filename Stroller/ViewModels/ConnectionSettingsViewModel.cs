using System;
using System.Collections.Generic;
using System.Net;
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

        public void Load()
        {
            Context = new ConnectionSettingsInfo
            {
                IpAddress = Bll.Properties.Settings.Default.IpAddress,
                Port = Bll.Properties.Settings.Default.Port.ToString()
            };
        }

        public void Save()
        {
            Save(true);
        }

        public async void TestConnection()
        {
            var lastIp = Bll.Properties.Settings.Default.IpAddress;
            var lastPort = Bll.Properties.Settings.Default.Port;

            Save(false);

            await ExecuteIntederminateProcess("Checing connectivity",
                "Tesing connection with " + Context.IpAddress + ". Please wait...",
                async () => { await _strollerControlService.GetStatus(); },
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

            return new KeyValuePair<string, string>();
        }
    }
}
