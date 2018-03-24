using System;
using System.Threading.Tasks;
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
    public class CapturingViewModel: ScreenBase
    {
        private readonly IStrollerControlService _strollerControlService = IoC.Get<IStrollerControlService>();

        public CapturingViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
        }

        public async void StartCapturing()
        {
            // TODO: Uncomment when status fetching is implemented
//                        if (!CapturingConfiguration.IsReadyToCapture)
//                        {
//                            await ShowMessage("Operation failed",
//                                "Cannot start capturing. Device is busy or camera not selected");
//                            return;
//                        }

            var imagesPath = CapturingManager.Initialize();

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
                        "Error occured during image acquisition process: " + exception.Message, progress, imagesPath);
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
                            "360 image acquisition process has been cancelled by the user", progress, imagesPath);
                        return;
                    }

                    processData = await _strollerControlService.SendToRotate(str);
                }
                catch (Exception ex)
                {
                    StopProcessAndShowError("Capturing failed",
                        "Error occured during image acquisition process: " + ex.Message, progress, imagesPath);
                    return;
                }

                progress.SetProgress(processData.Progress);
                CapturingManager.AppendImage(bytes, imagesPath, index);

                if (processData.Status == AcquisitionStatusType.Finished)
                {
                    // TODO: Save image record via CapturingManager

                    await progress.CloseAsync();

                    ShowInfo("Operation finished", "Image successfully acquired!");
                }
                else
                {
                    index++;
                    CameraManager.Capture();
                }
            });

            CameraManager.Capture();
        }

        private async void StopProcessAndShowError(string title, string message, ProgressDialogController progress, string imagesPath)
        {
            await progress.CloseAsync();

            CapturingManager.RemoveDir(imagesPath);

            ShowInfo(title,
                message);
        }

        private async void ShowInfo(string title, string message)
        {
            await ShowMessage(title, message);
        }
    }
}
