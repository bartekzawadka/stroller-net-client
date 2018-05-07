using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public bool IsImagesUploadMode { get; }
        private readonly IStrollerImageService _strollerImageService = IoC.Get<IStrollerImageService>();
        private IEnumerable<ImageListItem> _imagesList;
        private IEnumerable<ImageListItem> _selectedImages;

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

        public bool CanUpload => _selectedImages?.Count() > 0;

        public BrowseImagesViewModel(bool isImagesUploadMode = false) : base(IoC.Get<IMain>() as ScreenBase)
        {
            IsImagesUploadMode = isImagesUploadMode;
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
            if (!IsImagesUploadMode)
                ShowDialog(new ImageViewModel(e.ImageListItem.DirectoryName));
        }

        public void SelectedImagesChanged(SelectedImagesChangedEventArgs e)
        {
            _selectedImages = e.SelectedItems;
            NotifyOfPropertyChange(nameof(CanUpload));
        }

        public async void UploadImages()
        {
            var progress = await ShowProgress("Uploading images", "Selected images upload to server in progress...");
            progress.Minimum = 0.0;
            progress.Maximum = 100.0;

            var reporter = new Progress<double>();

            reporter.ProgressChanged += (sender, d) => { progress.SetProgress(d); };

            try
            {
                await _strollerImageService.UploadImagesToServer(_selectedImages.Select(x => x.DirectoryName).ToList(),
                    reporter);
                await progress.CloseAsync();
                await ShowMessage("Upload completed", "Images successfully uploaded");
            }
            catch (Exception ex)
            {
                await progress.CloseAsync();
                await ShowMessage("Upload failed",
                    "Images upload to server could not be completed" + Environment.NewLine + ex.Message);
            }

            LoadData();
        }
    }
}
