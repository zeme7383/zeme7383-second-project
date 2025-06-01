using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MyLibrary.Models;
using MyLibrary.Services;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace MyLibrary.ViewModels;

public partial class BorrowersViewModel : ObservableObject
{
    public ObservableCollection<Borrower> Borrowers { get; } = [];
    private readonly DatabaseService _db = new();

    [ObservableProperty]
    private Borrower? selectedBorrower;

    [ObservableProperty]
    private string? errorMessage;

    public IRelayCommand DeleteBorrowerCommand { get; }

    public BorrowersViewModel()
    {
        DeleteBorrowerCommand = new RelayCommand(DeleteBorrower, CanDeleteBorrower);
        ReloadBorrowers();
    }

    public void ReloadBorrowers()
    {
        Borrowers.Clear();
        foreach (var borrower in _db.GetAllBorrowers())
            Borrowers.Add(borrower);
    }

    public void DeleteBorrower()
    {
        ErrorMessage = string.Empty;
        if (SelectedBorrower is null) return;
        var dbService = new DatabaseService();
        var issuedBooks = dbService.GetAllIssuedBooks();
        if (issuedBooks.Any(b => b.BorrowerID == SelectedBorrower.BorrowerID))
        {
            ErrorMessage = "Cannot delete borrower: they have issued books in the system.";
            return;
        }
        _db.DeleteBorrower(SelectedBorrower.BorrowerID);
        ReloadBorrowers();
    }

    private bool CanDeleteBorrower() => SelectedBorrower is not null;

    partial void OnSelectedBorrowerChanged(Borrower? value)
    {
        DeleteBorrowerCommand.NotifyCanExecuteChanged();
    }
}
