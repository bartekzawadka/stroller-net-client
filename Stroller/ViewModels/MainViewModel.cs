using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Stroller.Contracts.Dto;
using Stroller.Main;
using Stroller.ViewModels.Common;
using Stroller.ViewModels.Settings;

namespace Stroller.ViewModels
{
    public class MainViewModel : MenuScreen, IMain
    {
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
                        ViewModel = typeof(BrowseImagesViewModel),
                        Params = new object[] {false}
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
                    Label = "Settings",
                    Tag = new ActivationInfo
                    {
                        ViewModel = typeof(SettingsViewModel),
                        IsDialog = true
                    }
                }
            };
        }
    }
}
