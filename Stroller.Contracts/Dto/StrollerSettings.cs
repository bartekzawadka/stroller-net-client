namespace Stroller.Contracts.Dto
{
    public class StrollerSettings
    {
        public string Direction { get; set; }

        public decimal StepAngle { get; set; }

        public string[] Cameras { get; set; }

        public string Camera { get; set; }

        public NameValuePair<string>[] Directions { get; set; }

        public bool IsLargeImagesMode { get; set; }
    }
}
