using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using Stroller.ViewModels.Common;

namespace Stroller.Main
{
    public class CustomWindowsManager : WindowManager
    {
        readonly Dictionary<object, MetroWindow> _modelToWindows = new Dictionary<object, MetroWindow>();

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = base.EnsureWindow(model, view, isDialog);
            AddWindow((MetroWindow) window, model);
            return window;
        }

        public MetroWindow GetWindowForModel(ScreenBase model)
        {
            if (model != null && _modelToWindows.ContainsKey(model))
                return _modelToWindows[model];
            return null;
        }

        public void RemoveWindow(ScreenBase model)
        {
            _modelToWindows.Remove(model);
        }

        private void AddWindow(MetroWindow window, object model)
        {
            if (!_modelToWindows.ContainsKey(model))
            {
                _modelToWindows.Add(model, window);
            }
            else
            {
                _modelToWindows[model] = window;
            }
        }
    }
}
