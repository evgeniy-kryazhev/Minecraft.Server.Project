using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core;
using DynamicData;
using Minecraft.Application.Modification.Dto;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace Minecraft.Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            MinecraftPath = new MinecraftPath();
            Launcher = new CMLauncher(MinecraftPath);

            this.WhenActivated( (CompositeDisposable disposables) =>
            {
                Task.Run(ActivateView);
            });
        }

        public MinecraftPath MinecraftPath { get; }
        public CMLauncher Launcher { get; }
        
        [Reactive]
        public ReadOnlyObservableCollection<string>? Versions { get; set; }
        
        [Reactive]
        public string SelectedVersion { get; set; }
        
        private async Task ActivateView()
        {
            try
            {
                IsLoading = true;
                LoadingTitle = "Загрузка списка версий";
                await Task.Delay(5000);
                var versions = await Launcher.GetAllVersionsAsync();
                Versions = new ReadOnlyObservableCollection<string>(versions.Select(x => x.Name)
                    .ToObservableCollection());
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}