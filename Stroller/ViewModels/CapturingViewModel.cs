using System;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Stroller.Bll;
using Stroller.Camera;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Enums;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class CapturingViewModel : ScreenBase
    {
        private readonly IStrollerControlService _strollerControlService = IoC.Get<IStrollerControlService>();
        private readonly IStrollerImageService _strollerImageService = IoC.Get<IStrollerImageService>();
        private StrollerStatus _status = new StrollerStatus();
        private string _statusText = StrollerStatusType.Unknown;
        private Brush _statusForeground = Application.Current.Resources["GrayBrush2"] as Brush;

        public bool IsAcquisitionEnabled => CapturingConfiguration.IsReadyToCapture;

        public Brush StatusForeground
        {
            get => _statusForeground;
            set
            {
                if (Equals(value, _statusForeground)) return;
                _statusForeground = value;
                NotifyOfPropertyChange();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(IsReleaseAvailable));
            }
        }

        public bool IsReleaseAvailable => _status.Status == StrollerStatusType.Busy;

        public CapturingViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
        }

        protected override void OnViewLoaded(object view)
        {
            RefreshStatus();
        }

        public async void StartCapturing()
        {
            if (!CapturingConfiguration.IsReadyToCapture)
            {
                await ShowMessage("Operation failed",
                    "Cannot start capturing. Device is busy or camera not selected");
                return;
            }

            var imageStorageInfo = _strollerImageService.Initialize();

            var progress = await ShowProgress("Capturing", "360 image acquisition pending...");
            progress.Minimum = 0.0;
            progress.Maximum = 100.0;
            progress.SetCancelable(true);

            CapturingProgressInfo processData;

            try
            {
                processData = await _strollerControlService.Capture();
            }
            catch (Exception ex)
            {
                await ShowMessage("Operation failed", "Capturing initialization failed: " + ex.Message);
                return;
            }

            var str = new SendToRotateInfo
            {
                Token = processData.Token
            };

            var index = 0;

            CameraManager.SetCaptureCallback(async (bytes, exception) =>
            {
                if (exception != null)
                {
                    StopProcessAndShowError("Capturing failed",
                        "Error occured during image acquisition process: " + exception.Message, progress, imageStorageInfo.DirectoryName);
                    return;
                }

                try
                {
                    if (progress.IsCanceled)
                    {
                        await _strollerControlService.CancelCapturing(new CancellingInfo
                        {
                            Token = str.Token
                        });
                        StopProcessAndShowError("Operation cancelled",
                            "360 image acquisition process has been cancelled by the user", progress, imageStorageInfo.DirectoryName);
                        return;
                    }

                    processData = await _strollerControlService.SendToRotate(str);
                }
                catch (Exception ex)
                {
                    StopProcessAndShowError("Capturing failed",
                        "Error occured during image acquisition process: " + ex.Message, progress, imageStorageInfo.DirectoryName);
                    return;
                }

                progress.SetProgress(processData.Progress);
                _strollerImageService.AppendImage(bytes, imageStorageInfo.FullPath, index);

                if (processData.Status == AcquisitionStatusType.Finished)
                {
                    _strollerImageService.SaveImage(imageStorageInfo, index + 1,
                        _strollerImageService.GetThumbnail(imageStorageInfo));

                    await progress.CloseAsync();

                    await ShowMessage("Operation finished", "Image successfully acquired!");
                }
                else
                {
                    index++;
                    CameraManager.Capture();
                }
            });

            CameraManager.Capture();
        }

        public async void RefreshStatus()
        {
            await ExecuteIntederminateProcess("Reading status", "Getting Stroller status. Please wait...",
                async () =>
                {
                    _status = await _strollerControlService.GetStatus();
                    SetColorForStatus();
                    NotifyOfPropertyChange(nameof(IsAcquisitionEnabled));
                },
                () => { },
                async exception =>
                {
                    await ShowMessage("Operation failed", "Unable to read Stroller status: " + exception.Message);
                });
        }

        public async void ReleaseDevice()
        {
            var dialogResult = await ShowConfirmation("Device busy",
                "Releasing currently working device may impact result images. Do you want to continue anyway?", "Yes",
                "No");

            if (dialogResult != MessageDialogResult.Affirmative)
                return;

            await ExecuteIntederminateProcess("Releasing device", "Releasing Stroller. Please wait...", async () =>
                {
                    await _strollerControlService.CancelCapturing(new CancellingInfo
                    {
                        Force = true
                    });
                }, () => { RefreshStatus(); },
                async exception =>
                {
                    await ShowMessage("Releasing failed", "Unable to release Stroller: " + exception.Message);
                });
        }

        private async void StopProcessAndShowError(string title, string message, ProgressDialogController progress, string imagesPath)
        {
            await progress.CloseAsync();

            _strollerImageService.RemoveDir(imagesPath);

            await ShowMessage(title,
                message);
        }

        private void SetColorForStatus()
        {
            switch (_status.Status)
            {
                case StrollerStatusType.Unknown:
                    StatusForeground = Application.Current.Resources["GrayBrush2"] as Brush;
                    break;
                case StrollerStatusType.Busy:
                    StatusForeground = Brushes.Red;
                    break;
                case StrollerStatusType.Ready:
                    StatusForeground = Brushes.Green;
                    break;
            }

            StatusText = _status.Status.ToUpper();
        }
    }
}
