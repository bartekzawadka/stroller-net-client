using System;
using System.Windows.Forms;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Settings
{
    public class GeneralSettingsViewModel : ScreenBase, ISettings
    {
        private string _imagesDir;
        private int _outputImageSize;

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

        public int OutputImageSize
        {
            get => _outputImageSize;
            set
            {
                if (value == _outputImageSize) return;
                _outputImageSize = value;
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
            OutputImageSize = Bll.Properties.Settings.Default.OutputImageWidth;
        }

        public void Save()
        {
            if (OutputImageSize <= 0)
                throw new Exception("360 image chunk size cannot be a negative value");

            Bll.Properties.Settings.Default.ImagesDir = ImagesDir;
            Bll.Properties.Settings.Default.OutputImageWidth = OutputImageSize;

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
