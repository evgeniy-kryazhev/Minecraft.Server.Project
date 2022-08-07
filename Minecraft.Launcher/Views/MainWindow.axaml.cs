using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Minecraft.Launcher.ViewModels;
using Refit;

namespace Minecraft.Launcher.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}