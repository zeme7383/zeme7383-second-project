using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MyLibrary.Models;
using MyLibrary.Services;
using CommunityToolkit.Mvvm.Input;

namespace MyLibrary.ViewModels;

public partial class IssuedBooksViewModel : ObservableObject
{
    public ObservableCollection<IssuedBook> IssuedBooks { get; } = [];
    private readonly DatabaseService _db = new();

    [ObservableProperty]
    private IssuedBook? selectedIssuedBook;

    public IRelayCommand ReturnBookCommand { get; }

    public IssuedBooksViewModel()
    {
        ReturnBookCommand = new RelayCommand(ReturnBook, CanReturnBook);
        ReloadIssuedBooks();
    }

    public void ReloadIssuedBooks()
    {
        IssuedBooks.Clear();
        foreach (var issuedBook in _db.GetAllIssuedBooks())
            IssuedBooks.Add(issuedBook);
        SelectedIssuedBook = null;
    }

    public void ReturnBook()
    {
        if (SelectedIssuedBook is null) return;
        _db.ReturnBook(SelectedIssuedBook.IssueID);
        ReloadIssuedBooks();
        MyLibrary.Views.IssuedBooksView.ReloadBooksAfterReturn();
    }

    private bool CanReturnBook() => SelectedIssuedBook is not null && !SelectedIssuedBook.ReturnDate.HasValue;

    partial void OnSelectedIssuedBookChanged(IssuedBook? value)
    {
        System.Diagnostics.Debug.WriteLine($"SelectedIssuedBook: {value?.IssueID}, ReturnDate: {value?.ReturnDate}");
        ReturnBookCommand.NotifyCanExecuteChanged();
    }
} 