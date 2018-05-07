using System;
using System.Linq;
using MahApps.Metro.Controls;
using Stroller.Contracts.Dto;

namespace Stroller.ViewModels.Common
{
    public class MenuScreen : ScreenBase
    {
        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuIconItem _selectedMenuItem;
        private ScreenBase _currentContent;
        private ScreenBase _lastView;
        private HamburgerMenuIconItem _lastItem;

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

        protected MenuScreen(ScreenBase parentScreen) : base(parentScreen)
        {
        }

        protected override void OnViewLoaded(object view)
        {
            if (MenuItems != null && MenuItems.Count > 0)
            {
                LastView = ((ActivationInfo)MenuItems[0].Tag).ViewModel as ScreenBase;
                _lastItem = MenuItems[0] as HamburgerMenuIconItem;
                SelectedMenuItem = MenuItems[0] as HamburgerMenuIconItem;
            }
        }

        protected virtual void OnItemViewModelCreated(ScreenBase vm)
        {
        }

        protected virtual void OnDialogClosed(ScreenBase vm, bool? result)
        {
        }

        private void UpdateView()
        {
            if (SelectedMenuItem == null) return;

            var activationInfo = (ActivationInfo)SelectedMenuItem.Tag;

            var vm = ActivateViewModel(activationInfo);

            OnItemViewModelCreated(vm);

            if (!activationInfo.IsDialog)
            {
                CurrentContent = vm;
                _lastItem = SelectedMenuItem;
            }
        }

        public void GoHome()
        {
            SelectedMenuItem = MenuItems.FirstOrDefault() as HamburgerMenuIconItem;
        }

        public void GoBack()
        {
            SelectedMenuItem = _lastItem;
        }

        public void OpenDialogItem(ItemClickEventArgs e)
        {
            var info = ((HamburgerMenuIconItem)e.ClickedItem).Tag as ActivationInfo;
            if (info == null)
                return;

            if (!info.IsDialog)
                return;

            var vm = ActivateViewModel(info);
            var result = ShowDialog(vm);
            SelectedMenuItem = _lastItem;

            OnDialogClosed(vm, result);
        }

        private ScreenBase ActivateViewModel(ActivationInfo info)
        {
            return Activator.CreateInstance((Type)info.ViewModel, info.Params) as ScreenBase;
        }
    }
}
