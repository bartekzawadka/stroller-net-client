using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Renci.SshNet;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;
using Stroller.Contracts.Serializable;

namespace Stroller.Bll
{
    public class StrollerImageService : IStrollerImageService
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ImageStorageInfo Initialize()
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

        public void AppendImage(byte[] imageData, string dir, int index)
        {
            using (var fs = File.Open(Path.Combine(dir, index + ".jpg"), FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(imageData, 0, imageData.Length);
            }
        }

        public void SaveImage(ImageStorageInfo storageInfo, int numberOfChunks, string thumbnail)
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

        public void RemoveDir(string dir)
        {
            var path = Path.Combine(Properties.Settings.Default.ImagesDir, dir);
            Directory.Delete(path, true);

            var recordPath = Path.Combine(Properties.Settings.Default.ImagesDir, "record.xml");
            var record = XmlFileSerializer.Deserialize<Record>(recordPath);

            var images = record.Images.Where(x => x.DirectoryName != dir).ToArray();
            record.Images = images;

            XmlFileSerializer.Serialize(record, recordPath);
        }

        public string GetThumbnail(ImageStorageInfo storageInfo)
        {
            var files = Directory.GetFiles(storageInfo.FullPath, "*.jpg");
            if (files.Length == 0)
                return null;

            var index = (int)Math.Round((double)(files.Length / 2));
            if (index < 0 || index > files.Length - 1)
                index = 0;

            string result;

            using (var fs = File.Open(files[index], FileMode.Open, FileAccess.Read))
            {
                using (var img = System.Drawing.Image.FromStream(fs))
                {
                    var destinationHeight = (int)(img.Height * 1.0 / (img.Width * 1.0) * 200);
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

        public IEnumerable<ImageListItem> GetImages()
        {
            var recordPath = Path.Combine(Properties.Settings.Default.ImagesDir, "record.xml");
            var record = XmlFileSerializer.Deserialize<Record>(recordPath);

            return record.Images.OrderByDescending(x => x.CreatedAt).Select(x => new ImageListItem
            {
                CreatedAt = x.CreatedAt,
                DirectoryName = x.DirectoryName,
                Thumbnail = !string.IsNullOrEmpty(x.Thumbnail) ? Convert.FromBase64String(x.Thumbnail) : null
            });
        }

        public StrollerImageObject GetImageJson(string dirName)
        {
            var path = Path.Combine(Properties.Settings.Default.ImagesDir, dirName);
            var recordPath = Path.Combine(Properties.Settings.Default.ImagesDir, "record.xml");

            var record = XmlFileSerializer.Deserialize<Record>(recordPath);

            var imageInfo = record.Images.First(x => x.DirectoryName == dirName);

            var obj = new StrollerImageObject
            {
                CreatedAt = imageInfo.CreatedAt,
                Thumbnail = "data:image/jpeg;base64," + imageInfo.Thumbnail
            };

            var files = Directory.GetFiles(path, "*.jpg");
            var chunks = new List<StrollerChunkItem>();

            foreach (var file in files)
            {
                var index = int.Parse(Path.GetFileNameWithoutExtension(file));
                string chunkString;

                using (var fs = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    var buff = new byte[fs.Length];
                    fs.Read(buff, 0, buff.Length);
                    chunkString = Convert.ToBase64String(buff);
                }

                chunks.Add(new StrollerChunkItem
                {
                    Image = "data:image/jpeg;base64," + chunkString,
                    Index = index
                });
            }

            obj.Chunks = chunks.ToArray();

            return obj;
        }

        public byte[] GetImageZip(string dirName)
        {
            var path = Path.Combine(Properties.Settings.Default.ImagesDir, dirName);
            var files = Directory.GetFiles(path, "*.jpg");

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            Directory.CreateDirectory(tempPath);

            foreach (var t in files)
            {
                var fName = Path.GetFileNameWithoutExtension(t);
                var index = int.Parse(fName) + 1;
                File.Copy(t, Path.Combine(tempPath, index + ".jpg"));
            }

            using (ZipArchive newFile = ZipFile.Open(tempPath + ".zip", ZipArchiveMode.Create))
            {
                foreach (var file in Directory.GetFiles(tempPath))
                {
                    newFile.CreateEntryFromFile(file, Path.GetFileName(file));
                }
            }

            Directory.Delete(tempPath, true);

            byte[] buff;
            using (var fs = File.Open(tempPath + ".zip", FileMode.Open, FileAccess.Read))
            {
                buff = new byte[fs.Length];
                fs.Read(buff, 0, buff.Length);
            }

            File.Delete(tempPath+".zip");

            return buff;
        }

        public IEnumerable<byte[]> GetImageBytes(string dirName)
        {
            var path = Path.Combine(Properties.Settings.Default.ImagesDir, dirName);
            var files = Directory.GetFiles(path, "*.jpg");

            foreach (var file in files)
            {
                using (var fs = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    var buff = new byte[fs.Length];
                    fs.Read(buff, 0, buff.Length);
                    yield return buff;
                }
            }
        }

        public Task UploadImagesToServer(List<string> dirName, IProgress<double> progressHandler)
        {
            var errorList = new List<string>();

            return Task.Factory.StartNew(() =>
            {

                using (var sftp = new SftpClient(Properties.Settings.Default.UploadImagesTargetHost,
                    Properties.Settings.Default.UploadImagesTargetPort,
                    Properties.Settings.Default.UploadImagesTargetUsername,
                    Properties.Settings.Default.UploadImagesTargetPassword))
                {
                    sftp.Connect();
                    sftp.ConnectionInfo.Encoding = Encoding.Unicode;

                    var numberOfProcessedFiles = 0;
                    var totalFiles = 0;

                    foreach (var dir in dirName)
                    {
                        totalFiles += Directory.GetFiles(Path.Combine(Properties.Settings.Default.ImagesDir, dir), "*.jpg").Length;
                    }

                    foreach (var dir in dirName)
                    {
                        try
                        {
                            var directoryName = Path.GetFileName(dir);

                            if (CheckIfRemoteDirectoryExists(sftp, Properties.Settings.Default.UploadImagesTargetDestRootDir, directoryName))
                            {
                                errorList.Add("Unable to upload dir " + directoryName +
                                              " to remote resource. Destination directory already exists");
                                break;
                            }

                            sftp.ChangeDirectory(Properties.Settings.Default.UploadImagesTargetDestRootDir);
                            sftp.CreateDirectory(directoryName);

                            foreach (var file in Directory.GetFiles(Path.Combine(Properties.Settings.Default.ImagesDir, dir), "*.jpg"))
                            {
                                using (var fs = File.Open(file, FileMode.Open, FileAccess.Read))
                                {
                                    var path = Path.Combine(directoryName, Path.GetFileName(file));
                                    if (Properties.Settings.Default.UploadImagesTargetIsUnix)
                                    {
                                        path = path.Replace("\\", "/");
                                    }

                                    sftp.UploadFile(fs, path);
                                }

                                numberOfProcessedFiles++;
                                progressHandler.Report(numberOfProcessedFiles * 100.0 / totalFiles);
                            }
                        }
                        catch (Exception ex)
                        {
                            var exType = ex.GetType();
                            _logger.Error(ex);
                            errorList.Add(ex.Message);
                        }
                    }

                    sftp.Disconnect();
                }

                if (!errorList.Any()) return;
                var msg = "";
                var lastError = errorList.Last();
                foreach (var error in errorList)
                {
                    msg += error;
                    if (error != lastError)
                    {
                        msg += Environment.NewLine;
                    }
                }

                throw new Exception(msg);
            });
        }

        /// <summary>
        /// Checks if Remote folder contains the given file name
        /// </summary>
        private bool CheckIfRemoteDirectoryExists(SftpClient sftpClient, string remoteRootFolderName, string remoteFolderName)
        {
            var isFileExists = sftpClient
                .ListDirectory(remoteRootFolderName)
                .Any(
                    f => f.IsDirectory &&
                         string.Equals(f.Name, remoteFolderName, StringComparison.CurrentCultureIgnoreCase)
                );
            return isFileExists;
        }
    }
}
