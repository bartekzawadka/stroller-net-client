using System;
using System.Collections.Generic;
using System.Linq;
using WIA;

namespace Stroller.Camera
{
    public class CameraManager
    {
        private static readonly DeviceManager DeviceManager;
        private static string _camera;

        static CameraManager()
        {
            DeviceManager = new DeviceManagerClass();
        }

        public static IEnumerable<string> GetCameras()
        {
            return GetCameraInfos().Select(x => x.Properties[6].get_Value().ToString());
        }

        public static string GetCurrentCamera()
        {
            return _camera;
        }

        public static void SetCurrentCamera(string camera)
        {
            if (string.IsNullOrEmpty(camera))
                throw new Exception("Camera name was not specified");

            var cameras = GetCameras();
            if (!cameras.Any(x => string.Equals(camera.ToLower(), x.ToLower())))
            {
                throw new Exception("Camera '" + camera + "' is not connected");
            }

            _camera = camera;
        }

        public static byte[] Capture()
        {
            if(string.IsNullOrEmpty(_camera))
                throw new Exception("Capturing device is not set");

            var device = GetCameraInfos()
                .FirstOrDefault(x => string.Equals(x.Properties[6].get_Value().ToString().ToLower(), _camera.ToLower()));
            if (device == null)
            {
                throw new Exception(
                    "Could not connect to camera '" + _camera + "'. Please try restarting device");
            }

            var camera = device.Connect();

            var dialog = new CommonDialogClass();
            var result = dialog.ShowTransfer(camera.Items[1], FormatID.wiaFormatJPEG, true);

//            var result = //camera.Items[1].Transfer(FormatID.wiaFormatJPEG);
            if (result == null)
            {
                throw new Exception(
                    "Capturing failed. Empty data received");
            }

            if (!(result is ImageFile image) || image.FileData == null)
            {
                throw new Exception(
                    "Capturing failed. Received data is not a valid ImageFile object");
            }

            return (byte[]) image.FileData.get_BinaryData();
        }

        private static IEnumerable<DeviceInfo> GetCameraInfos()
        {
            for (var i = 1; i <= DeviceManager.DeviceInfos.Count; i++)
            {
                if (DeviceManager.DeviceInfos[i].Type == WiaDeviceType.CameraDeviceType)
                {
                    yield return DeviceManager.DeviceInfos[i];
                }
            }
        }
    }
}
