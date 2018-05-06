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
    public class SettingsViewModel : ScreenBase
    {
        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuIconItem _selectedMenuItem;
        private ScreenBase _currentContent;
        private readonly List<ScreenBase> _settingsPanels = new List<ScreenBase>();

        public ScreenBase CurrentContent
        {
            get => _currentContent;
            set
            {
                if (Equals(value, _currentContent)) return;
                _currentContent = value;
                NotifyOfPropertyChange();
            }
        }

        public HamburgerMenuItemCollection MenuItems
        {
            get => _menuItems;
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                NotifyOfPropertyChange();
            }
        }

        public HamburgerMenuIconItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                if (Equals(value, _selectedMenuItem)) return;
                _selectedMenuItem = value;
                NotifyOfPropertyChange();
                UpdateView();
            }
        }

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

        public void UpdateView()
        {
            if (SelectedMenuItem != null)
            {
                var activationInfo = (ActivationInfo)SelectedMenuItem.Tag;

                var vm =
                    Activator.CreateInstance((Type)activationInfo.ViewModel, activationInfo.Params) as ScreenBase;

                AddToPanelStorage(vm);

                if (!activationInfo.IsDialog)
                    CurrentContent = vm;
                else
                    ShowDialog(vm);
            }
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
