using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using MyLibrary.ViewModels;
using System.Threading.Tasks;

namespace MyLibrary.Views;

public partial class BorrowersView : UserControl
{
    public BorrowersView()
    {
        InitializeComponent();
    }

    private async void AddBorrower_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not BorrowersViewModel borrowersVm)
            return;
        var dialog = new Window
        {
            Title = "Add New Borrower",
            Width = 400,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = true
        };
        var formVm = new BorrowerFormViewModel(() =>
        {
            dialog.Close();
            borrowersVm.ReloadBorrowers();
        });
        dialog.Content = new BorrowerFormView { DataContext = formVm };
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var owner = lifetime?.MainWindow;
        if (owner is not null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    private async void DeleteBorrower_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not BorrowersViewModel borrowersVm)
            return;
        if (borrowersVm.SelectedBorrower is null)
            return;
        var confirm = await ShowConfirmationDialog("Delete Borrower", $"Are you sure you want to delete '{borrowersVm.SelectedBorrower.Name}'?");
        if (confirm)
        {
            borrowersVm.DeleteBorrower();
        }
    }

    private async void EditBorrower_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not BorrowersViewModel borrowersVm)
            return;
        if (borrowersVm.SelectedBorrower is null)
            return;
        var dialog = new Window
        {
            Title = "Edit Borrower",
            Width = 400,
            Height = 320,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = true
        };
        var formVm = new BorrowerFormViewModel(() =>
        {
            dialog.Close();
            borrowersVm.ReloadBorrowers();
        })
        {
            Name = borrowersVm.SelectedBorrower.Name,
            Email = borrowersVm.SelectedBorrower.Email,
            Phone = borrowersVm.SelectedBorrower.Phone
        };
        formVm.IsEditMode = true;
        formVm.BorrowerId = borrowersVm.SelectedBorrower.BorrowerID;
        dialog.Content = new BorrowerFormView { DataContext = formVm };
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var owner = lifetime?.MainWindow;
        if (owner is not null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
    }

    private async Task<bool> ShowConfirmationDialog(string title, string message)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 350,
            Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = false,
            CanResize = false
        };
        var result = false;
        var stack = new StackPanel { Spacing = 16, Margin = new Thickness(24) };
        stack.Children.Add(new TextBlock { Text = message, FontSize = 16, Margin = new Thickness(0,0,0,8) });
        var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right };
        var yesBtn = new Button { Content = "Yes", MinWidth = 80 };
        var noBtn = new Button { Content = "No", MinWidth = 80 };
        yesBtn.Click += (_, __) => { result = true; dialog.Close(); };
        noBtn.Click += (_, __) => { dialog.Close(); };
        buttonPanel.Children.Add(yesBtn);
        buttonPanel.Children.Add(noBtn);
        stack.Children.Add(buttonPanel);
        dialog.Content = stack;
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var owner = lifetime?.MainWindow;
        if (owner is not null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show();
        return result;
    }
}