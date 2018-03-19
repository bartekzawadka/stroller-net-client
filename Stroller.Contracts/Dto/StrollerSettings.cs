using System.Collections.Generic;

namespace Stroller.Contracts.Dto
{
    public class StrollerSettings
    {
        public string Direction { get; set; }

        public decimal StepAngle { get; set; }

        //public KeyValuePair<string, string>[] Directions { get; set; }

        public string[] Cameras { get; set; }

        public string Camera { get; set; }
    }
}
