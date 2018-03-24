using System.Threading.Tasks;
using Stroller.Contracts.Dto;

namespace Stroller.Contracts.Interfaces
{
    public interface IStrollerSettingsService
    {
        Task<StrollerSettings> GetSettings();

        Task<NameValuePair<string>[]> GetDirections();

        Task SaveSettings(StrollerSettings settings);
    }
}
