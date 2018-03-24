using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WIA;

namespace Stroller.Camera
{
    public class CameraManager
    {
        private static readonly DeviceManager DeviceManager;
        private static string _camera;
        private static Device _cameraDevice;
        private static Action<byte[], Exception> _callbackAction;

        private static SynchronizationContext _synchronizationContext;

        static CameraManager()
        {
            DeviceManager = new DeviceManagerClass();
        }

        public static IEnumerable<string> GetCameras()
        {
            var cameras = GetCameraInfos().Select(x => x.Properties[6].get_Value().ToString());
            var enumerable = cameras as string[] ?? cameras.ToArray();
            if (!enumerable.Contains(_camera))
            {
                _camera = null;
            }

            return enumerable;
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

        public static void Capture()
        {
            if (string.IsNullOrEmpty(_camera))
                throw new Exception("Capturing device is not set");

            var device = GetCameraInfos()
                .FirstOrDefault(x => string.Equals(x.Properties[6].get_Value().ToString().ToLower(), _camera.ToLower()));
            if (device == null)
            {
                throw new Exception(
                    "Could not connect to camera '" + _camera + "'. Please try restarting device");
            }

            _cameraDevice = device.Connect();

            var commandId = string.Empty;

            foreach (IDeviceCommand cameraCommand in _cameraDevice.Commands)
            {
                if (cameraCommand.Name.ToLower() == "take picture")
                {
                    commandId = cameraCommand.CommandID;
                    break;
                }
            }

            if (string.IsNullOrEmpty(commandId))
            {
                throw new Exception("Device '" + _camera + "' does not provide 'Take Picture' command");
            }

            _synchronizationContext = SynchronizationContext.Current;

            DeviceManager.RegisterEvent(EventID.wiaEventItemCreated, _cameraDevice.DeviceID);
            DeviceManager.RegisterEvent(EventID.wiaEventDeviceDisconnected, _cameraDevice.DeviceID);
            DeviceManager.OnEvent += DeviceManager_OnEvent;
            _cameraDevice.ExecuteCommand(commandId);
        }

        public static void SetCaptureCallback(Action<byte[], Exception> callback)
        {
            _callbackAction = callback;
        }

        private static async void DeviceManager_OnEvent(string eventId, string deviceId, string itemId)
        {
            if (_cameraDevice != null && eventId == EventID.wiaEventItemCreated && _callbackAction != null)
            {
                for (var i = 1; i <= _cameraDevice.Items.Count; i++)
                {
                    if (_cameraDevice.Items[i].ItemID == itemId)
                    {
                        var obj = _cameraDevice.Items[i].Transfer(FormatID.wiaFormatJPEG);

                        if (!(obj is ImageFile))
                        {
                            if (_callbackAction != null)
                            {
                                ExecuteSynchedWithContext(() =>
                                {
                                    _callbackAction(null,
                                        new Exception("Data received from camera, but format is not valid"));
                                });
                            }

                            break;
                        }

                        var image = (ImageFile)obj;
                        var buff = (byte[])image.FileData.get_BinaryData();

                        _cameraDevice.Items.Remove(i);

                        if (_callbackAction != null)
                        {
                            ExecuteSynchedWithContext(() => { _callbackAction(buff, null); });
                        }

                        break;
                    }
                }
            }

            DeviceManager.UnregisterEvent(eventId, deviceId);
            DeviceManager.OnEvent -= DeviceManager_OnEvent;
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

        private static void ExecuteSynchedWithContext(Action invoke)
        {
            if (_synchronizationContext != null)
            {
                _synchronizationContext.Post(delegate { invoke.Invoke(); }, null);
            }
            else
            {
                invoke.Invoke();
            }
        }
    }
}
