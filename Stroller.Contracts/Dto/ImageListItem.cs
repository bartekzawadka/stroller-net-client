using System;

namespace Stroller.Contracts.Dto
{
    public class ImageListItem
    {
        public string DirectoryName { get; set; }

        public byte[] Thumbnail { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedAtText => CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
