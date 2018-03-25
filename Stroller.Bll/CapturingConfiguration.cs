using System;
using Stroller.Contracts.Dto;

namespace Stroller.Bll
{
    public static class CapturingConfiguration
    {
        public static StrollerSettings Settings { get; set; } = new StrollerSettings();

        public static string Status { get; set; } = StrollerStatusType.Unknown;

        public static bool IsReadyToCapture => !string.IsNullOrEmpty(Settings.Camera) && Status == StrollerStatusType.Ready;
    }
}
