using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Serializable;

namespace Stroller.Bll
{
    public static class CapturingManager
    {
        public static ImageStorageInfo Initialize()
        {
            var storageInfo = new ImageStorageInfo();
            var date = DateTime.Now;
            var dirName = date.ToString("yyyy-MM-dd_HH-mm-ss");
            var path = Path.Combine(Properties.Settings.Default.ImagesDir, dirName);

            storageInfo.CreatedAt = date;
            storageInfo.DirectoryName = dirName;
            storageInfo.FullPath = path;
            
            Directory.CreateDirectory(path);

            return storageInfo;
        }

        public static void AppendImage(byte[] imageData, string dir, int index)
        {
            using (var fs = File.Open(Path.Combine(dir, index + ".jpg"), FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(imageData, 0, imageData.Length);
            }
        }

        public static void SaveImage(ImageStorageInfo storageInfo, int numberOfChunks, string thumbnail)
        {
            var recordPath = Path.Combine(Properties.Settings.Default.ImagesDir, "record.xml");

            var record = new Record();

            if (File.Exists(recordPath))
            {
                record = XmlFileSerializer.Deserialize<Record>(recordPath);
            }

            var images = record.Images?.ToList() ?? new List<Image>();
            images.Add(new Image
            {
                CreatedAt = storageInfo.CreatedAt,
                DirectoryName = storageInfo.DirectoryName,
                NumberOfChunks = numberOfChunks,
                Thumbnail = thumbnail
            });

            record.Images = images.ToArray();

            XmlFileSerializer.Serialize(record, recordPath);
        }

        public static void RemoveDir(string dir)
        {
            Directory.Delete(dir, true);
        }
    }
}
