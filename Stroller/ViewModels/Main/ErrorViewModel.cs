using System;
using Stroller.ViewModels.Common;

namespace Stroller.ViewModels.Main
{
    public class ErrorViewModel : ScreenBase
    {
        public string Message { get; }

        public ErrorViewModel(string message) : base(null)
        {
            Message = message;
        }

        public ErrorViewModel(Exception ex) : this(ex?.Message)
        {
        }
    }
}
