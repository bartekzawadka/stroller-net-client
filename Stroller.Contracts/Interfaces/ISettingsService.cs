using System.Threading.Tasks;
using Stroller.Contracts.Dto;

namespace Stroller.Contracts.Interfaces
{
    public interface ISettingsService
    {
        Task<StrollerSettings> GetSettings();

        Task<NameValuePair<string>[]> GetDirections();

        void SaveSettings(StrollerSettings settings);
    }
}
