using System.Threading.Tasks;
using Stroller.Contracts.Dto;
using Stroller.Contracts.Interfaces;

namespace Stroller.Bll
{
    public class StrollerSettingsService : StrollerService, ISettingsService
    {
        public Task<StrollerSettings> GetSettings()
        {
            return ExecuteGetService<StrollerSettings>("config");
        }

        public Task<NameValuePair<string>[]> GetDirections()
        {
            return ExecuteGetService<NameValuePair<string>[]>("directions");
        }

        public void SaveSettings(StrollerSettings settings)
        {
            ExecutePostService(settings, "config");
        }
    }
}
