namespace Stroller.Contracts.Dto
{
    public class ImageUploadsSettingsInfo
    {
        public string UploadServerHostName { get; set; }

        public string UploadServerPort { get; set; }

        public string UploadServerUsername { get; set; }

        public string UploadServerPassword { get; set; }

        public bool UploadServerTargetIsUnix { get; set; }

        public string UploadServerDestRootDir { get; set; }
    }
}
