using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using MyLibrary.ViewModels;
using MyLibrary.Views;

namespace MyLibrary;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            
            DisableAvaloniaDataAnnotationValidation();
            // Show LoginView in a window first
            var loginWindow = new Window
            {
                Title = "Login",
                Width = 400,
                Height = 500,
                Content = new MyLibrary.Views.LoginView
                {
                    DataContext = new MyLibrary.ViewModels.LoginViewModel(desktop)
                },
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            if (loginWindow.Content is MyLibrary.Views.LoginView loginView &&
                loginView.DataContext is MyLibrary.ViewModels.LoginViewModel loginVM)
            {
                loginVM.SetLoginWindow(loginWindow);
            }
            loginWindow.Show();
            // MainWindow will be shown after successful login
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}