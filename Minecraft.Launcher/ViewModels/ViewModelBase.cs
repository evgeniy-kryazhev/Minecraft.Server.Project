using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Minecraft.Launcher.ViewModels
{
    public class ViewModelBase : ReactiveObject, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

        [Reactive] public bool IsLoading { get; set; } = true;
        [Reactive] public string? LoadingTitle { get; set; }
    }
}