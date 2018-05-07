using System;
using System.Collections.Generic;
using System.Linq;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Stroller.Contracts.Dto;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Settings
{
    public class SettingsViewModel : MenuScreen
    {
        private readonly List<ScreenBase> _settingsPanels = new List<ScreenBase>();

        public SettingsViewModel() : base(null)
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Settings},
                    Label = "General",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(GeneralSettingsViewModel),
                        Params = new object[]{this}
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Camera},
                    Label = "Capturing",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(CapturingSettingsViewModel),
                        Params = new object[] {this}
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Connect},
                    Label = "Stroller connection",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(ConnectionSettingsViewModel),
                        Params = new object[] {this}
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.CloudUpload},
                    Label = "Image uploads",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(ImageUploadsSettingsViewModel),
                        Params = new object[] {this}
                    }
                },
            };
        }

        protected override void OnItemViewModelCreated(ScreenBase vm)
        {
            AddToPanelStorage(vm);
        }

        public void SaveAndClose()
        {
            foreach (var settingsPanel in _settingsPanels)
            {
                if (settingsPanel is ISettings settings)
                    settings.Save();
            }

            TryClose();
        }

        private void AddToPanelStorage(ScreenBase item)
        {
            if (item == null)
                return;

            if (_settingsPanels.Any(x => item.GetType() == x.GetType())) return;
            _settingsPanels.Add(item);
        }
    }
}
