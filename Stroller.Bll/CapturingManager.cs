using System;
using System.IO;

namespace Stroller.Bll
{
    public static class CapturingManager
    {
        public static string Initialize()
        {
            var path = Path.Combine(Properties.Settings.Default.ImagesDir,
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            Directory.CreateDirectory(path);
            return path;
        }

        public static void AppendImage(byte[] imageData, string dir, int index)
        {
            using (var fs = File.Open(Path.Combine(dir, index + ".jpg"), FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(imageData, 0, imageData.Length);
            }
        }

        public static void RemoveDir(string dir)
        {
            Directory.Delete(dir, true);
        }
    }
}
