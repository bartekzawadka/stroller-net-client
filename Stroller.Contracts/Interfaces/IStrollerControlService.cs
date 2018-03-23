using System.Threading.Tasks;
using Stroller.Contracts.Dto;

namespace Stroller.Contracts.Interfaces
{
    public interface IStrollerControlService
    {
        Task<StrollerStatus> GetStatus();
    }
}
