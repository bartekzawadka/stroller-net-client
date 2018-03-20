using System.Linq;
using Caliburn.Micro;
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

        public CapturingSettingsViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
            Context = new StrollerSettings();
        }

        protected override void OnViewLoaded(object view)
        {
            LoadSettings();
        }

        public async void LoadSettings()
        {
            var progress = await ShowProgress("Loading settings", "Reading settings from Stroller. Please wait...",
                true);
            progress.SetIndeterminate();

            var settings = await _settingsService.GetSettings();
            settings.Directions = await _settingsService.GetDirections();
            Context = settings;
            SelectedDirection = Context.Directions?.FirstOrDefault(x => x.Value == Context.Direction);

            await progress.CloseAsync();
        }

        public async void Save()
        {
            if (SelectedDirection != null && !string.IsNullOrEmpty(SelectedDirection.Value))
                Context.Direction = SelectedDirection.Value;

            var progress = await ShowProgress("Saving settings", "Sending settings to Stroller. Please wait...",
                true);
            progress.SetIndeterminate();

            _settingsService.SaveSettings(Context);

            await progress.CloseAsync();
            IoC.Get<IMain>().GoBack();
        }
    }
}
