﻿using Caliburn.Micro;
using Stroller.Main;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels
{
    public class CapturingViewModel: ScreenBase
    {
        public CapturingViewModel() : base(IoC.Get<IMain>() as ScreenBase)
        {
        }
    }
}
