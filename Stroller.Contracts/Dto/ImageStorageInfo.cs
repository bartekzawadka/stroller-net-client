using System;

namespace Stroller.Contracts.Dto
{
    public class ImageStorageInfo
    {
        public DateTime CreatedAt { get; set; }

        public string DirectoryName { get; set; }

        public string FullPath { get; set; }
    }
}
