using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Stroller.Main;
using Action = System.Action;

namespace Stroller.ViewModels.Common
{
    public abstract class ScreenBase : Conductor<object>
    {
        protected static CustomWindowsManager WindowManager => IoC.Get<IWindowManager>() as CustomWindowsManager;

        public ScreenBase ParentScreen { get; set; }

        protected ScreenBase(ScreenBase parentScreen)
        {
            ParentScreen = parentScreen;
        }

        public override void TryClose(bool? dialogResult = null)
        {
            base.TryClose(dialogResult);
            WindowManager.RemoveWindow(this);
        }

        public Task<MessageDialogResult> ShowMessage(string title, string message, bool showOnParentScreen = true)
        {
            var view = GetWindowThisOrParent(showOnParentScreen);
            return view.ShowMessageAsync(title, message);
        }

        public Task<MessageDialogResult> ShowConfirmation(string title, string message,
            string affirmativeButtonText = "Tak", string negativeButtonText = "Nie", bool showOnParentScreen = true)
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

        public Task<ProgressDialogController> ShowProgress(string title, string message, bool showOnParentScreen = true)
        {
            var view = GetWindowThisOrParent(showOnParentScreen);
            return view.ShowProgressAsync(title, message);
        }

        public async Task ExecuteIntederminateProcess(string title, string message, Func<Task> invoke,
            Func<Task> onSuccess = null, Func<Exception, Task> onFail = null, bool showOnParentScreen = true)
        {
            var progress = await ShowProgress(title, message);
            progress.SetIndeterminate();

            try
            {
                await invoke();
                await progress.CloseAsync();

                if (onSuccess != null)
                    await onSuccess.Invoke();
            }
            catch (Exception e)
            {
                await progress.CloseAsync();
                if (onFail != null)
                    await onFail.Invoke(e);
            }
        }

        public async Task ExecuteIntederminateProcess(string title, string message, Func<Task> invoke,
            Action onSuccess = null, Func<Exception, Task> onFail = null, bool showOnParentScreen = true)
        {
            var progress = await ShowProgress(title, message);
            progress.SetIndeterminate();

            try
            {
                await invoke();
                await progress.CloseAsync();

                onSuccess?.Invoke();
            }
            catch (Exception e)
            {
                await progress.CloseAsync();
                if (onFail != null)
                    await onFail.Invoke(e);
            }
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
