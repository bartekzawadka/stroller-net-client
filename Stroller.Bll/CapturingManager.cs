using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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

        public static string GetThumbnail(ImageStorageInfo storageInfo)
        {
            var files = Directory.GetFiles(storageInfo.FullPath, "*.jpg");
            if (files.Length == 0)
                return null;

            var index = (int) Math.Round((double) (files.Length / 2));
            if (index < 0 || index > files.Length - 1)
                index = 0;

            string result;

            using (var fs = File.Open(files[index], FileMode.Open, FileAccess.Read))
            {
                using (var img = System.Drawing.Image.FromStream(fs))
                {
                    var destinationHeight = (int) (img.Height * 1.0 / (img.Width * 1.0) * 200);
                    using (var thumb = img.GetThumbnailImage(200, destinationHeight, () => false, IntPtr.Zero))
                    {
                        byte[] buff;
                        using (var ms = new MemoryStream())
                        {
                            thumb.Save(ms, ImageFormat.Jpeg);
                            buff = ms.ToArray();
                        }

                        result = Convert.ToBase64String(buff);
                    }
                }
            }

            return result;
        }
    }
}
