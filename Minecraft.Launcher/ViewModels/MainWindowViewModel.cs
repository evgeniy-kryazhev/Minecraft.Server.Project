using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer.FabricMC;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using DynamicData.Binding;
using Newtonsoft.Json;
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
            Launcher.FileChanged += args =>
            {
                Log = JsonConvert.SerializeObject(args);
            };

            this.WhenActivated( (CompositeDisposable disposables) =>
            {
                Task.Run(LoadingVersions);
            });

            this.WhenAnyValue(x => x.FilterLocalVersion)
                .Subscribe(x => Task.Run(LoadingVersions));

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
        
        [Reactive]
        public bool FilterLocalVersion { get; set; }
        
        [Reactive]
        public string? Log { get; set; }
        
        public ReactiveCommand<Unit, Unit> PlayGameCommand { get; set; }

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
            catch (Exception exception)
            {
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow("Error", exception.Message);
                await messageBoxStandardWindow.Show();
            }
            finally
            {
                IsLoading = false;
                LoadingTitle = null;
            }
        }

        private async Task LoadingVersions()
        {
            try
            {
                IsLoading = true;
                LoadingTitle = "Загрузка списка версий";
                var versions = await GetVersions();
                Versions = new ReadOnlyObservableCollection<string>(versions.Select(x => x.Name)
                    .ToObservableCollection());
            }
            catch (Exception exception)
            {
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow("Error", exception.Message);
                await messageBoxStandardWindow.Show();
            }
            finally
            {
                IsLoading = false;
                LoadingTitle = null;
            }
        }
        
        private Task<MVersionCollection> GetVersions()
        {
            if (!FilterLocalVersion)
            {
                return Launcher.GetAllVersionsAsync();
            }
            else
            {
                var local = new LocalVersionLoader(MinecraftPath);
                return local.GetVersionMetadatasAsync();
            }
        }
    }
}