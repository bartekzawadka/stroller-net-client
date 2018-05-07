using System;

namespace Stroller.Contracts.Dto
{
    public class ActivationInfo
    {
        public object ViewModel { get; set; }

        public object[] Params { get; set; }

        public bool IsDialog { get; set; }
    }
}
