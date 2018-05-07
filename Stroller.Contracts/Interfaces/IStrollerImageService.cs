using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Serializable;

namespace Stroller.Contracts.Interfaces
{
    public interface IStrollerImageService
    {
        ImageStorageInfo Initialize();

        void AppendImage(byte[] imageData, string dir, int index);

        void SaveImage(ImageStorageInfo storageInfo, int numberOfChunks, string thumbnail);

        void RemoveDir(string dir);

        string GetThumbnail(ImageStorageInfo storageInfo);

        IEnumerable<ImageListItem> GetImages();

        StrollerImageObject GetImageJson(string dirName);

        byte[] GetImageZip(string dirName);

        IEnumerable<byte[]> GetImageBytes(string dirName);

        Task UploadImagesToServer(List<string> dirName, IProgress<double> progressHandler);
    }
}
