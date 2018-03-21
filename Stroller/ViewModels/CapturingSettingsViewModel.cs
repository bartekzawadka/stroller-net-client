using System.Linq;
using Caliburn.Micro;
using Stroller.Bll;
using Stroller.Camera;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class CapturingSettingsViewModel : DetailsScreen<StrollerSettings>
    {
        private readonly ISettingsService _settingsService = IoC.Get<ISettingsService>();
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

        public CapturingSettingsViewModel() : base(IoC.Get<IMain>() as ScreenBase)
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

            var progress = await ShowProgress("Saving settings", "Sending settings to Stroller. Please wait...",
                true);
            progress.SetIndeterminate();

            _settingsService.SaveSettings(Context);
            CapturingConfiguration.Settings = Context;

            await progress.CloseAsync();
            IoC.Get<IMain>().GoBack();
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
            var progress = await ShowProgress("Loading settings", "Reading settings from Stroller. Please wait...",
                true);
            progress.SetIndeterminate();

            var settings = await _settingsService.GetSettings();
            settings.Directions = await _settingsService.GetDirections();

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

            await progress.CloseAsync();
        }
    }
}
