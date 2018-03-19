using System.Linq;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Stroller.Contracts.Interfaces;
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

        private ISettingsService _settingsService;

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

        public MainViewModel()
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Camera},
                    Label = "Capturing",
                    Tag = new CapturingViewModel
                    {
                        ParentScreen = this
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.ImageGallery},
                    Label = "Browse images"
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Settings},
                    Label = "Capturing settings",
                    Tag = new CapturingSettingsViewModel
                    {
                        ParentScreen = this
                    }
                },
                new HamburgerMenuIconItem
                {
                    Icon = new PackIconModern {Kind = PackIconModernKind.Connect},
                    Label = "Connection settings",
                    Tag = new ConnectionSettingsViewModel
                    {
                        ParentScreen = this
                    }
                },
            };

            LastView = MenuItems[0].Tag as ScreenBase;
        }

        public void UpdateView()
        {
            CurrentContent = SelectedMenuItem.Tag as ScreenBase;
        }

        public void StartCapturing()
        {

        }

        public void BrowseImages()
        {
        }

        public void ShowSettings()
        {

        }

        public void GoBack()
        {
            if (LastView != null)
            {
                SelectedMenuItem =
                    MenuItems.FirstOrDefault(x => x.Tag != null && x.Tag.GetType() == LastView.GetType()) as
                        HamburgerMenuIconItem;
            }
            else
            {
                SelectedMenuItem = MenuItems.First() as HamburgerMenuIconItem;
            }

            BrowseImages();
        }
    }
}
