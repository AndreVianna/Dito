using CommunityToolkit.Mvvm.ComponentModel;

namespace VivaVoz.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string welcomeMessage = "Welcome to VivaVoz";
}
