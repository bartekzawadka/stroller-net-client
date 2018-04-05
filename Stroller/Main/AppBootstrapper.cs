using System;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.Autofac;
using MahApps.Metro;
using Stroller.Bll;
using Stroller.Contracts.Interfaces;
using Stroller.ViewModels;

namespace Stroller.Main
{
    public class AppBootstrapper : AutofacBootstrapper<MainViewModel>
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // add custom accent and theme resource dictionaries to the ThemeManager
            // you should replace MahAppsMetroThemesSample with your application name
            // and correct place where your custom accent lives
            ThemeManager.AddAccent("CustomAccent1", new Uri("pack://application:,,,/Stroller;component/Resources/Style.xaml"));

            // get the current app style (theme and accent) from the application
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

            // now change app style to the custom accent and current theme
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent("CustomAccent1"),
                theme.Item1);

            DisplayRootViewFor<IMain>();
        }

        protected override void ConfigureContainer(ContainerBuilder builder)
        {
            base.ConfigureContainer(builder);

            builder.RegisterInstance(new CustomWindowsManager()).As<IWindowManager>();
            builder.RegisterInstance(new MainViewModel()).As<IMain>();
            builder.RegisterInstance(new StrollerSettingsService()).As<IStrollerSettingsService>();
            builder.RegisterInstance(new StrollerControlService()).As<IStrollerControlService>();
            builder.RegisterInstance(new StrollerImageService()).As<IStrollerImageService>();
        }
    }
}
