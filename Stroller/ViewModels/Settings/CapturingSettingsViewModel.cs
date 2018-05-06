using System.Linq;
using Caliburn.Micro;
using Stroller.Bll;
using Stroller.Camera;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Settings
{
    public class CapturingSettingsViewModel : DetailsScreen<StrollerSettings>
    {
        private readonly IStrollerSettingsService _strollerSettingsService = IoC.Get<IStrollerSettingsService>();
        private NameValuePair<string> _selectedDirection;
        private string _camera;

        public NameValuePair<string> SelectedDirection
        {
            get => _selectedDirection;
            set
            {
                if (Equals(value, _selectedDirection)) return;
                _selectedDirection = value;
                NotifyOfPropertyChange();
            }
        }

        public string Camera
        {
            get => _camera;
            set
            {
                if (value == _camera) return;
                _camera = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(IsSaveEnabled));
            }
        }

        public bool IsSaveEnabled => !string.IsNullOrEmpty(Camera);

        public CapturingSettingsViewModel(ScreenBase parent) : base(parent)
        {
            Context = new StrollerSettings();
        }

        protected override void OnViewLoaded(object view)
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            LoadSettings(false);
        }

        public async void Save()
        {
            if (!string.IsNullOrEmpty(SelectedDirection?.Value))
                Context.Direction = SelectedDirection.Value;

            Context.Camera = Camera;
            if (Context.IsLargeImagesMode)
            {
                CameraManager.SetCurrentCamera(Camera);
            }

            await ExecuteIntederminateProcess("Saving settings", "Sending settings to Stroller. Please wait...", async token =>
            {
                await _strollerSettingsService.SaveSettings(Context, token);
                CapturingConfiguration.Settings = Context;
            }, () => { IoC.Get<IMain>().GoHome(); }, async ex => { await ShowMessage("Opration failed", ex.Message); });
        }

        public void LargeImageModeChecked()
        {
            Context.Cameras = CameraManager.GetCameras().ToArray();
            Camera = CameraManager.GetCurrentCamera();
            NotifyOfPropertyChange(nameof(Context));
        }

        public void LargeImageModeUnchecked()
        {
            LoadSettings(true);
        }

        private async void LoadSettings(bool useLocalCameras)
        {
            await ExecuteIntederminateProcess("Loading settings", "Reading settings from Stroller. Please wait...",
                async token =>
                {
                    var settings = await _strollerSettingsService.GetSettings(token);
                    settings.Directions = await _strollerSettingsService.GetDirections(token);

                    SelectedDirection = settings.Directions?.FirstOrDefault(x => x.Value == settings.Direction);

                    if (useLocalCameras)
                    {
                        settings.IsLargeImagesMode = false;
                    }

                    if (settings.IsLargeImagesMode)
                    {
                        settings.Camera = CameraManager.GetCurrentCamera();
                        settings.Cameras = CameraManager.GetCameras().ToArray();
                    }

                    Context = settings;
                    Camera = settings.Camera;

                    NotifyOfPropertyChange(nameof(Context));
                    NotifyOfPropertyChange(nameof(SelectedDirection));
                }, () => { },

                async ex =>
                {
                    await ShowMessage("Operation failed",
                        "Unable to retrieve configuration from Stroller: " + ex.Message);
                });
        }
    }
}
