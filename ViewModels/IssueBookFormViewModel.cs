using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyLibrary.Models;
using MyLibrary.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyLibrary.ViewModels;

public partial class IssueBookFormViewModel : ObservableObject
{
    private readonly Action _onSave;
    private readonly DatabaseService _db = new();

    public ObservableCollection<Book> AvailableBooks { get; } = [];
    public ObservableCollection<Borrower> Borrowers { get; } = [];

    [ObservableProperty]
    private Book? selectedBook;

    [ObservableProperty]
    private Borrower? selectedBorrower;

    [ObservableProperty]
    private DateTimeOffset dueDate = DateTimeOffset.Now.AddDays(14);

    [ObservableProperty]
    private string? errorMessage;

    public IRelayCommand SaveCommand { get; }

    public IssueBookFormViewModel(Action onSave)
    {
        _onSave = onSave;
        SaveCommand = new RelayCommand(Save, CanSave);
        LoadData();
    }

    private void LoadData()
    {
        // Load available books (with copies > 0)
        var books = _db.GetAllBooks().Where(b => b.AvailableCopies > 0);
        foreach (var book in books)
            AvailableBooks.Add(book);

        // Load all borrowers
        var borrowers = _db.GetAllBorrowers();
        foreach (var borrower in borrowers)
            Borrowers.Add(borrower);
    }

    private void Save()
    {
        ErrorMessage = string.Empty;

        if (SelectedBook is null || SelectedBorrower is null)
        {
            ErrorMessage = "Please select both a book and a borrower.";
            return;
        }

        if (DueDate <= DateTimeOffset.Now)
        {
            ErrorMessage = "Due date must be in the future.";
            return;
        }

        var issuedBook = new IssuedBook
        {
            BookID = SelectedBook.BookID,
            BorrowerID = SelectedBorrower.BorrowerID,
            IssueDate = DateTime.Today,
            DueDate = DueDate.DateTime
        };

        try
        {
            _db.IssueBook(issuedBook);
            _onSave();
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to issue book: " + ex.Message;
        }
    }

    private bool CanSave() => SelectedBook is not null && SelectedBorrower is not null && DueDate > DateTimeOffset.Now;

    partial void OnSelectedBookChanged(Book? value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedBorrowerChanged(Borrower? value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnDueDateChanged(DateTimeOffset value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }
} 