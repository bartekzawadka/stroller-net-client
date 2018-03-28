using System.Threading;
using System.Threading.Tasks;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;

namespace Stroller.Bll
{
    public class StrollerControlService : StrollerService, IStrollerControlService
    {
        public Task<StrollerStatus> GetStatus(CancellationToken cancellationToken)
        {
            return ExecuteGetService<StrollerStatus>("status", cancellationToken,
                status => { CapturingConfiguration.Status = status.Status; });
        }

        public Task<CapturingProgressInfo> Capture(CancellationToken cancellationToken)
        {
            return ExecuteGetService<CapturingProgressInfo>("capture", cancellationToken);
        }

        public Task CancelCapturing(CancellingInfo data, CancellationToken cancellationToken)
        {
            return ExecutePostService(data, cancellationToken, "capture/cancel");
        }

        public Task<CapturingProgressInfo> SendToRotate(SendToRotateInfo data, CancellationToken cancellationToken)
        {
            return ExecutePostService<SendToRotateInfo, CapturingProgressInfo>(data, cancellationToken, "image");
        }
    }
}
