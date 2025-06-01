using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MyLibrary.Models;
using MyLibrary.Services;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Collections.Generic;
using System;

namespace MyLibrary.ViewModels;

public partial class BooksViewModel : ObservableObject
{
    public ObservableCollection<Book> Books { get; } = [];
    private readonly DatabaseService _db = new();

    [ObservableProperty]
    private Book? selectedBook;

    [ObservableProperty]
    private string? authorFilter;

    [ObservableProperty]
    private int? yearFromFilter;

    [ObservableProperty]
    private int? yearToFilter;

    [ObservableProperty]
    private string yearFromFilterText = string.Empty;

    [ObservableProperty]
    private string yearToFilterText = string.Empty;

    public IRelayCommand DeleteBookCommand { get; }
    public IRelayCommand ClearFiltersCommand { get; }

    public BooksViewModel()
    {
        DeleteBookCommand = new RelayCommand(DeleteBook, CanDeleteBook);
        ClearFiltersCommand = new RelayCommand(ClearFilters);
        ReloadBooks();
    }

    public void ReloadBooks()
    {
        Books.Clear();
        _allBooks = _db.GetAllBooks();
        ApplyFilters();
    }

    private List<Book> _allBooks = new();

    private void ApplyFilters()
    {
        Books.Clear();
        var filtered = _allBooks.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(AuthorFilter))
            filtered = filtered.Where(b => b.Author.Contains(AuthorFilter, StringComparison.OrdinalIgnoreCase));
        if (YearFromFilter.HasValue)
            filtered = filtered.Where(b => b.Year >= YearFromFilter.Value);
        if (YearToFilter.HasValue)
            filtered = filtered.Where(b => b.Year <= YearToFilter.Value);
        foreach (var book in filtered)
            Books.Add(book);
    }

    public void DeleteBook()
    {
        if (SelectedBook is null) return;
        _db.DeleteBook(SelectedBook.BookID);
        ReloadBooks();
    }

    private bool CanDeleteBook() => SelectedBook is not null;

    partial void OnSelectedBookChanged(Book? value)
    {
        DeleteBookCommand.NotifyCanExecuteChanged();
    }

    partial void OnAuthorFilterChanged(string? value)
    {
        ApplyFilters();
    }

    partial void OnYearFromFilterChanged(int? value)
    {
        ApplyFilters();
    }

    partial void OnYearToFilterChanged(int? value)
    {
        ApplyFilters();
    }

    partial void OnYearFromFilterTextChanged(string value)
    {
        if (int.TryParse(value, out var year))
            YearFromFilter = year;
        else if (string.IsNullOrWhiteSpace(value))
            YearFromFilter = null;
        // Do not set YearFromFilter if invalid input
    }

    partial void OnYearToFilterTextChanged(string value)
    {
        if (int.TryParse(value, out var year))
            YearToFilter = year;
        else if (string.IsNullOrWhiteSpace(value))
            YearToFilter = null;
        // Do not set YearToFilter if invalid input
    }

    private void ClearFilters()
    {
        AuthorFilter = string.Empty;
        YearFromFilterText = string.Empty;
        YearToFilterText = string.Empty;
        YearFromFilter = null;
        YearToFilter = null;
        ApplyFilters();
    }
}
