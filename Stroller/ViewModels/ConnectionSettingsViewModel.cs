using System.Collections.Generic;
using System.Net;
using Caliburn.Micro;
using Stroller.Contracts.Dto;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class ConnectionSettingsViewModel : DetailsScreen<ConnectionSettingsInfo>
    {
        public ConnectionSettingsViewModel()
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
                await ShowMessage(msg.Key, msg.Value, true);
                return;
            }

            Bll.Properties.Settings.Default.IpAddress = Context.IpAddress;
            Bll.Properties.Settings.Default.Port = int.Parse(Context.Port);
            Bll.Properties.Settings.Default.Save();

            TryClose();
            IoC.Get<IMain>().GoBack();
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
