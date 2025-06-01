using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MyLibrary.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object? currentView;

    public IRelayCommand ShowBooksCommand { get; }
    public IRelayCommand ShowBorrowersCommand { get; }
    public IRelayCommand ShowReportsCommand { get; }

    [ObservableProperty]
    private BooksViewModel booksViewModel = new();

    [ObservableProperty]
    private BorrowersViewModel borrowersViewModel = new();

    [ObservableProperty]
    private IssuedBooksViewModel issuedBooksViewModel = new();

    public ReportsViewModel ReportsViewModel { get; } = new();

    public MainWindowViewModel()
    {
        ShowBooksCommand = new RelayCommand(ShowBooks);
        ShowBorrowersCommand = new RelayCommand(ShowBorrowers);
        ShowReportsCommand = new RelayCommand(ShowReports);
        // Set default view
        ShowBooks();
    }

    private void ShowBooks() => CurrentView = BooksViewModel;
    private void ShowBorrowers() => CurrentView = BorrowersViewModel;
    private void ShowReports() => CurrentView = ReportsViewModel;

    public string Greeting { get; } = "Welcome to Avalonia!";
}
