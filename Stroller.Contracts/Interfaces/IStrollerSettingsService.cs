using System.Threading;
using System.Threading.Tasks;
using Stroller.Contracts.Dto;

namespace Stroller.Contracts.Interfaces
{
    public interface IStrollerSettingsService
    {
        Task<StrollerSettings> GetSettings(CancellationToken cancellationToken);

        Task<NameValuePair<string>[]> GetDirections(CancellationToken cancellationToken);

        Task SaveSettings(StrollerSettings settings, CancellationToken cancellationToken);
    }
}
