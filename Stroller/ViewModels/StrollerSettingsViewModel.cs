using Caliburn.Micro;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class StrollerSettingsViewModel : ScreenBase
    {
        public StrollerSettingsViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
        }
    }
}
