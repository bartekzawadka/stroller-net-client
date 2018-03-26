using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using Stroller.Contracts.Interfaces;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class ImageViewModel : ScreenBase
    {
        public string DirName { get; }
        private readonly IStrollerImageService _strollerImageService = IoC.Get<IStrollerImageService>();
        private List<byte[]> _chunksSource;

        public List<byte[]> ChunksSource
        {
            get => _chunksSource;
            set
            {
                if (Equals(value, _chunksSource)) return;
                _chunksSource = value;
                NotifyOfPropertyChange();
            }
        }

        public ImageViewModel(string dirName) : base(IoC.Get<IMain>() as ScreenBase)
        {
            DirName = dirName;
        }

        protected override void OnViewLoaded(object view)
        {
            LoadImages(DirName);
        }

        private void LoadImages(string dirName)
        {
            ChunksSource = _strollerImageService.GetImageBytes(dirName).ToList();
        }
    }
}
