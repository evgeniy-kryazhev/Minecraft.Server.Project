using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Minecraft.Application.Modification.Dto;
using ReactiveUI;
using Splat;

namespace Minecraft.Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            _serverApi = Locator.Current.GetService<IServerApi>();
            ListModifications = new ObservableCollection<string>();
        }

        public ObservableCollection<string> ListModifications { get; set; }

        public ReactiveCommand<Unit, Unit> LoadModsCommand =>
            ReactiveCommand.CreateFromTask(LoadModifications);

        private readonly IServerApi _serverApi;

        public async Task LoadModifications()
        {
            var mods = await _serverApi.GetModifications();
            foreach (var modificationDto in mods)
            {
                Console.WriteLine(modificationDto.Name);
                ListModifications.Add(modificationDto.Name);
            }
        }
    }
}