using System;
using System.Collections.Generic;
using System.Net;
using Caliburn.Micro;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Settings
{
    public class ConnectionSettingsViewModel : DetailsScreen<ConnectionSettingsInfo>, ISettings
    {
        private readonly IStrollerControlService _strollerControlService = IoC.Get<IStrollerControlService>();

        public ConnectionSettingsViewModel(ScreenBase parent) : base(parent)
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

        public async void Save()
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
        }

        public async void TestStrollerConnection()
        {
            var lastIp = Bll.Properties.Settings.Default.IpAddress;
            var lastPort = Bll.Properties.Settings.Default.Port;

            Save();

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

            Save();

            NotifyOfPropertyChange(nameof(Context));
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
