using System;
using System.IO;
using System.Windows.Forms;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Settings
{
    public class GeneralSettingsViewModel : ScreenBase, ISettings
    {
        private string _imagesDir;

        public string ImagesDir
        {
            get => _imagesDir;
            set
            {
                if (value == _imagesDir) return;
                _imagesDir = value;
                NotifyOfPropertyChange();
            }
        }

        public GeneralSettingsViewModel(ScreenBase parentScreen) : base(parentScreen)
        {
            Load();
        }

        public void Load()
        {
            ImagesDir = Bll.Properties.Settings.Default.ImagesDir;
        }

        public void Save()
        {
            Bll.Properties.Settings.Default.ImagesDir = ImagesDir;
            Bll.Properties.Settings.Default.Save();
        }

        public void BrowseDir()
        {
            var fDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyComputer
            };
            if (fDialog.ShowDialog() == DialogResult.OK)
            {
                ImagesDir = fDialog.SelectedPath;
            }
        }
    }
}
