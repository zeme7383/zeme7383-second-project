using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using MyLibrary.ViewModels;
using System.Threading.Tasks;
using Avalonia.Layout;

namespace MyLibrary.Views;

public partial class BooksView : UserControl
{
    public BooksView()
    {
        InitializeComponent();
    }

    private async void AddBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not BooksViewModel booksVm)
            return;
        var dialog = new Window
        {
            Title = "Add New Book",
            Width = 400,
            Height = 420,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = true // for debugging
        };
        var formVm = new BookFormViewModel(() =>
        {
            dialog.Close();
            booksVm.ReloadBooks();
        });
        dialog.Content = new BookFormView { DataContext = formVm };
        // Robustly get the main window
        var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var owner = lifetime?.MainWindow;
        if (owner is not null)
            await dialog.ShowDialog(owner);
        else
            dialog.Show(); // fallback: non-modal
    }

    private async void DeleteBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not BooksViewModel booksVm)
            return;
        if (booksVm.SelectedBook is null)
            return;
        var confirm = await ShowConfirmationDialog("Delete Book", $"Are you sure you want to delete '{booksVm.SelectedBook.Title}'?");
        if (confirm)
        {
            booksVm.DeleteBook();
        }
    }

    private async void EditBook_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not BooksViewModel booksVm)
            return;
        if (booksVm.SelectedBook is null)
            return;
        var dialog = new Window
        {
            Title = "Edit Book",
            Width = 400,
            Height = 420,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInTaskbar = true
        };
        var formVm = new BookFormViewModel(() =>
        {
            dialog.Close();
            booksVm.ReloadBooks();
        })
        {
            Title = booksVm.SelectedBook.Title,
            Author = booksVm.SelectedBook.Author,
            Year = booksVm.SelectedBook.Year,
            AvailableCopies = booksVm.SelectedBook.AvailableCopies
        };
        formVm.IsEditMode = true;
        formVm.BookId = booksVm.SelectedBook.BookID;
        dialog.Content = new BookFormView { DataContext = formVm };
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