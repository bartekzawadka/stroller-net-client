using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Stroller.Contracts.Dto;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Settings
{
    public class ImageUploadsSettingsViewModel : DetailsScreen<ImageUploadsSettingsInfo>, ISettings
    {
        public ImageUploadsSettingsViewModel(ScreenBase parentScreen) : base(parentScreen)
        {
            Load();
        }

        public async void Save()
        {
            var msg = Validate();
            if (!string.IsNullOrEmpty(msg.Value))
            {
                await ShowMessage(msg.Key, msg.Value);
                return;
            }

            Bll.Properties.Settings.Default.UploadImagesTargetHost = Context.UploadServerHostName;
            Bll.Properties.Settings.Default.UploadImagesTargetPassword = Context.UploadServerPassword;
            Bll.Properties.Settings.Default.UploadImagesTargetPort = int.Parse(Context.UploadServerPort);
            Bll.Properties.Settings.Default.UploadImagesTargetUsername = Context.UploadServerUsername;
            Bll.Properties.Settings.Default.UploadImagesTargetDestRootDir = Context.UploadServerDestRootDir;
            Bll.Properties.Settings.Default.UploadImagesTargetIsUnix = Context.UploadServerTargetIsUnix;

            Bll.Properties.Settings.Default.Save();
        }

        public void OnPasswordChanged(RoutedEventArgs args)
        {
            if (Context != null)
                Context.UploadServerPassword = ((PasswordBox)args.OriginalSource).Password;
        }

        public void Load()
        {
            Context = new ImageUploadsSettingsInfo
            {
                UploadServerHostName = Bll.Properties.Settings.Default.UploadImagesTargetHost,
                UploadServerPassword = Bll.Properties.Settings.Default.UploadImagesTargetPassword,
                UploadServerPort = Bll.Properties.Settings.Default.UploadImagesTargetPort.ToString(),
                UploadServerUsername = Bll.Properties.Settings.Default.UploadImagesTargetUsername,
                UploadServerTargetIsUnix = Bll.Properties.Settings.Default.UploadImagesTargetIsUnix,
                UploadServerDestRootDir = Bll.Properties.Settings.Default.UploadImagesTargetDestRootDir
            };
        }

        private KeyValuePair<string, string> Validate()
        {
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
