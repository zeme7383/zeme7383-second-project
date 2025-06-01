using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using MyLibrary.Services;

namespace MyLibrary.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string? username;

    [ObservableProperty]
    private string? password;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool showPassword;

    public IRelayCommand LoginCommand { get; }
    public IRelayCommand ToggleShowPasswordCommand { get; }

    private readonly IClassicDesktopStyleApplicationLifetime? _desktop;
    private Window? _loginWindow;
    private readonly AuthenticationService _authService = new();

    public LoginViewModel() { 
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        ToggleShowPasswordCommand = new RelayCommand(ToggleShowPassword);
    }
    public LoginViewModel(IClassicDesktopStyleApplicationLifetime desktop)
    {
        _desktop = desktop;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        ToggleShowPasswordCommand = new RelayCommand(ToggleShowPassword);
    }
    public void SetLoginWindow(Window window) => _loginWindow = window;

    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        await Task.Delay(200); // Simulate async work
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter both username and password.";
            return;
        }
        if (_authService.Authenticate(Username, Password))
        {
            if (_desktop != null)
            {
                _desktop.MainWindow = new Views.MainWindow();
                _desktop.MainWindow.Show();
                _loginWindow?.Close();
            }
        }
        else
        {
            ErrorMessage = "Invalid username or password.";
        }
    }

    private void ToggleShowPassword()
    {
        ShowPassword = !ShowPassword;
    }
}
