using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using MyLibrary.ViewModels;
using System;
using System.Threading.Tasks;

namespace MyLibrary.Views;

public partial class IssuedBooksView : UserControl
{
    public IssuedBooksView()
    {
        InitializeComponent();
    }

    private async void IssueBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not IssuedBooksViewModel issuedBooksVm)
            return;

        var dialog = new Window
        {
            Title = "Issue Book",
            Width = 400,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = true
        };

        var formVm = new IssueBookFormViewModel(() =>
        {
            dialog.Close();
            issuedBooksVm.ReloadIssuedBooks();
            if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime &&
                lifetime.MainWindow?.DataContext is MainWindowViewModel mainVm)
            {
                mainVm.BooksViewModel.ReloadBooks();
            }
        });

        dialog.Content = new IssueBookFormView { DataContext = formVm };
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var owner = lifetime?.MainWindow;
        if (owner is not null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    public static void ReloadBooksAfterReturn()
    {
        if (App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime &&
            lifetime.MainWindow?.DataContext is MainWindowViewModel mainVm)
        {
            mainVm.BooksViewModel.ReloadBooks();
        }
    }
} 