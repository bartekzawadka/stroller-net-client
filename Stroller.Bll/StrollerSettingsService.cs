using System.Threading;
using System.Threading.Tasks;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;

namespace Stroller.Bll
{
    public class StrollerSettingsService : StrollerService, IStrollerSettingsService
    {
        public Task<StrollerSettings> GetSettings(CancellationToken cancellationToken)
        {
            return ExecuteGetService<StrollerSettings>("config", cancellationToken);
        }

        public Task<NameValuePair<string>[]> GetDirections(CancellationToken cancellationToken)
        {
            return ExecuteGetService<NameValuePair<string>[]>("directions", cancellationToken);
        }

        public Task SaveSettings(StrollerSettings settings, CancellationToken cancellationToken)
        {
            return ExecutePostService(settings, cancellationToken, "config");
        }
    }
}
