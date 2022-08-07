using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.FabricMC;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Minecraft.Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            MinecraftPath = new MinecraftPath();
            Launcher = new CMLauncher(MinecraftPath);
            
            Launcher.ProgressChanged += (s, e) =>
            {
                IsLoading = true;
                LoadingTitle = $"Загрузка версии игры {e.ProgressPercentage}";
            };

            this.WhenActivated( (CompositeDisposable disposables) =>
            {
                Task.Run(ActivateView);
            });

            var canExecutePlayGame = this.WhenAnyValue(
                x => x.UserName, x => x.SelectedVersion,
                (userName, version) => 
                    !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(version));
            
            PlayGameCommand = ReactiveCommand.CreateFromTask(PlayGame, canExecutePlayGame);
        }

        public MinecraftPath MinecraftPath { get; }
        public CMLauncher Launcher { get; }
        
        [Reactive]
        public ReadOnlyObservableCollection<string>? Versions { get; set; }
        
        [Reactive]
        public string? SelectedVersion { get; set; }
        
        [Reactive]
        public string UserName { get; set; }
        
        public ReactiveCommand<Unit, Unit> PlayGameCommand { get; set; }

        private async Task ActivateView()
        {
            try
            {
                IsLoading = true;
                LoadingTitle = "Загрузка списка версий";
                var fabricVersionLoader = new FabricVersionLoader();
                var versions = await Launcher.GetAllVersionsAsync();
                Versions = new ReadOnlyObservableCollection<string>(versions.Select(x => x.Name)
                    .ToObservableCollection());
            }
            finally
            {
                IsLoading = false;
                LoadingTitle = null;
            }
        }

        private async Task PlayGame()
        {
            try
            {
                IsLoading = true;
                LoadingTitle = "Запуск игры";
                if (SelectedVersion == null)
                {
                    return;
                }
            
                ServicePointManager.DefaultConnectionLimit = 256;
                var session = MSession.GetOfflineSession(UserName);
                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = 8000,
                    Session = session,
                };
                var process = await Launcher.CreateProcessAsync(SelectedVersion, launchOption);
                process.Start();
            }
            finally
            {
                IsLoading = false;
                LoadingTitle = null;
            }
        }
    }
}