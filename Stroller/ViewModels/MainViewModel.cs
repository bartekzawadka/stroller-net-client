using System;
using System.Linq;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Stroller.Contracts.Dto;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class MainViewModel : ScreenBase, IMain
    {
        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuIconItem _selectedMenuItem;
        private ScreenBase _currentContent;
        private ScreenBase _lastView;

        public ScreenBase CurrentContent
        {
            get => _currentContent;
            set
            {
                if (Equals(value, _currentContent)) return;
                LastView = _currentContent;
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

        public ScreenBase LastView
        {
            get => _lastView;
            set
            {
                if (Equals(value, _lastView)) return;
                _lastView = value;
                NotifyOfPropertyChange();
            }
        }

        public MainViewModel() : base(null)
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Camera},
                    Label = "Capturing",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(CapturingViewModel)
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.ImageGallery},
                    Label = "Browse images",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(BrowseImagesViewModel)
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Upload},
                    Label = "Upload images",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(BrowseImagesViewModel),
                        Params = new object[] {true}
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Settings},
                    Label = "Capturing settings",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(CapturingSettingsViewModel)
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Connect},
                    Label = "Connection settings",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(ConnectionSettingsViewModel)
                    }
                }
            };
        }

        public void UpdateView()
        {
            if (SelectedMenuItem != null)
            {
                var activationInfo = (ActivationInfo) SelectedMenuItem.Tag;
                CurrentContent =
                    Activator.CreateInstance((Type)activationInfo.ViewModel, activationInfo.Params) as ScreenBase;
            }
        }

        public void GoHome()
        {
            SelectedMenuItem = MenuItems.First() as HamburgerMenuIconItem;
        }

        protected override void OnViewLoaded(object view)
        {
            LastView = ((ActivationInfo)MenuItems[0].Tag).ViewModel as ScreenBase;
            SelectedMenuItem = MenuItems[0] as HamburgerMenuIconItem;
        }
    }
}
