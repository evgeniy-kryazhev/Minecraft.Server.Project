using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Minecraft.Launcher.ViewModels;
using Minecraft.Launcher.Views;
using Refit;
using Splat;

namespace Minecraft.Launcher
{
    public partial class App : Avalonia.Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Locator.CurrentMutable
                .Register<IServerApi>(() => RestService.For<IServerApi>("http://2.59.41.222"));
                

            Locator.CurrentMutable
                .Register<MainWindowViewModel>(() => new MainWindowViewModel());
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = Locator.Current.GetService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
            RegisterServices();
        }
    }
}