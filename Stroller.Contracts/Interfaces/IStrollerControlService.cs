using System.Threading.Tasks;
using Stroller.Contracts.Dto;

namespace Stroller.Contracts.Interfaces
{
    public interface IStrollerControlService
    {
        Task<StrollerStatus> GetStatus();

        Task<CapturingProgressInfo> Capture();

        Task CancelCapturing(CancellingInfo data);

        Task<CapturingProgressInfo> SendToRotate(SendToRotateInfo data);
    }
}
