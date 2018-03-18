using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Stroller.Main;

namespace Stroller.ViewModels.Common
{
    public abstract class ScreenBase : Conductor<object>
    {
        protected static CustomWindowsManager WindowManager => IoC.Get<IWindowManager>() as CustomWindowsManager;

        public ScreenBase ParentScreen { get; set; }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            WindowManager.RemoveWindow(this);
        }

        public Task<MessageDialogResult> ShowMessage(string title, string message, bool showOnParentScreen = false)
        {
            var view = GetWindowThisOrParent(showOnParentScreen);
            return view.ShowMessageAsync(title, message);
        }

        public Task<MessageDialogResult> ShowConfirmation(string title, string message,
            string affirmativeButtonText = "Tak", string negativeButtonText = "Nie", bool showOnParentScreen = false)
        {
            var view = GetWindowThisOrParent(showOnParentScreen);
            return view.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = affirmativeButtonText,
                    NegativeButtonText = negativeButtonText,
                    DefaultButtonFocus = MessageDialogResult.Affirmative
                });
        }

        public bool? ShowDialog(ScreenBase viewModel)
        {
            return WindowManager.ShowDialog(viewModel);
        }

        public Task<ProgressDialogController> ShowProgress(string title, string message, bool showOnParentScreen = false)
        {
            var view = GetWindowThisOrParent(showOnParentScreen);
            return view.ShowProgressAsync(title, message);
        }

        public virtual void SelectedContextItemDoubleClick(object context)
        {

        }

        private MetroWindow GetWindowThisOrParent(bool useParent = false)
        {
            var screen = this;
            if (useParent)
                screen = ParentScreen;
            var view = WindowManager.GetWindowForModel(screen);

            if (view == null)
                throw new Exception("Nie odnaleziono okna dla wybranego modelu");

            return view;
        }
    }
}
