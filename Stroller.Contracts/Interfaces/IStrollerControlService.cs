using System.Threading;
using System.Threading.Tasks;
using Stroller.Contracts.Dto;

namespace Stroller.Contracts.Interfaces
{
    public interface IStrollerControlService
    {
        Task<StrollerStatus> GetStatus(CancellationToken cancellationToken);

        Task<CapturingProgressInfo> Capture(CancellationToken cancellationToken);

        Task CancelCapturing(CancellingInfo data, CancellationToken cancellationToken);

        Task<CapturingProgressInfo> SendToRotate(SendToRotateInfo data, CancellationToken cancellationToken);
    }
}
