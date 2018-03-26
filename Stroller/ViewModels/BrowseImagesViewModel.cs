using System.Collections.Generic;
using System.IO;
using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Newtonsoft.Json;
using Stroller.Common.Events;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class BrowseImagesViewModel : ListScreen<ImageListItem>
    {
        private readonly IStrollerImageService _strollerImageService = IoC.Get<IStrollerImageService>();
        private IEnumerable<ImageListItem> _imagesList;

        public IEnumerable<ImageListItem> ImagesList
        {
            get => _imagesList;
            set
            {
                if (Equals(value, _imagesList)) return;
                _imagesList = value;
                NotifyOfPropertyChange();
            }
        }

        public BrowseImagesViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
        }

        protected override void OnViewLoaded(object view)
        {
            LoadData();
        }

        public override void LoadData()
        {
            ImagesList = _strollerImageService.GetImages();
        }

        public async void DeleteItem(ImageListItemEventArgs e)
        {
            var confirmation = await ShowConfirmation("Are you sure?",
                "Are you sure you want to remove this image? This operation cannot be undone.");

            if(confirmation != MessageDialogResult.Affirmative)
                return;
            
            _strollerImageService.RemoveDir(e.ImageListItem.DirectoryName);
            LoadData();
        }

        public void DownloadJson(ImageListItemEventArgs e)
        {
            var fDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddExtension = true,
                Filter = "JSON (*.json)|*.json",
                FilterIndex = 1,
                CheckFileExists = false
            };

            if(fDialog.ShowDialog() != true) 
                return;

            var imageObject = _strollerImageService.GetImageJson(e.ImageListItem.DirectoryName);
            var json = JsonConvert.SerializeObject(imageObject);

            using (var fs = File.Open(fDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                }
            }
        }

        public void DownloadZip(ImageListItemEventArgs e)
        {
            var fDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddExtension = true,
                Filter = "ZIP Archive (*.zip)|*.zip",
                FilterIndex = 1,
                CheckFileExists = false
            };

            if (fDialog.ShowDialog() != true)
                return;

            var buff = _strollerImageService.GetImageZip(e.ImageListItem.DirectoryName);
            using (var fs = File.Open(fDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(buff, 0, buff.Length);
            }
        }

        public void OpenItem(ImageListItemEventArgs e)
        {
            ShowDialog(new ImageViewModel(e.ImageListItem.DirectoryName));
        }
    }
}
