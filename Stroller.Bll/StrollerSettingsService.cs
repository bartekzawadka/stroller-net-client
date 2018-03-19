using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
