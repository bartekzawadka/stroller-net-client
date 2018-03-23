using System.Threading.Tasks;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;

namespace Stroller.Bll
{
    public class StrollerControlService : StrollerService, IStrollerControlService
    {
        public Task<StrollerStatus> GetStatus()
        {
            return ExecuteGetService<StrollerStatus>("status");
        } 
    }
}
