using System.Threading.Tasks;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;

namespace Stroller.Bll
{
    public class StrollerControlService : StrollerService, IStrollerControlService
    {
        public Task<StrollerStatus> GetStatus()
        {
            return ExecuteGetService<StrollerStatus>("status",
                status => { CapturingConfiguration.Status = status.Status; });
        }

        public Task<CapturingProgressInfo> Capture()
        {
            return ExecuteGetService<CapturingProgressInfo>("capture");
        }

        public Task CancelCapturing(CancellingInfo data)
        {
            return ExecutePostService(data, "capture/cancel");
        }

        public Task<CapturingProgressInfo> SendToRotate(SendToRotateInfo data)
        {
            return ExecutePostService<SendToRotateInfo, CapturingProgressInfo>(data, "image");
        }
    }
}
