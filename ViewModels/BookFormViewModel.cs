using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyLibrary.Models;
using MyLibrary.Services;
using System;
using System.Threading.Tasks;

namespace MyLibrary.ViewModels;

public partial class BookFormViewModel : ObservableObject
{
    [ObservableProperty] private string? title;
    [ObservableProperty] private string? author;
    [ObservableProperty] private int? year;
    [ObservableProperty] private int? availableCopies;
    [ObservableProperty] private string? errorMessage;
    [ObservableProperty] private bool isEditMode;
    [ObservableProperty] private int bookId;

    public IRelayCommand SaveCommand { get; }
    public IRelayCommand CancelCommand { get; }

    private readonly DatabaseService _db = new();
    private readonly Action? _onSuccess;

    public BookFormViewModel(Action? onSuccess = null)
    {
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        CancelCommand = new RelayCommand(Cancel);
        _onSuccess = onSuccess;
        Year = null;
        AvailableCopies = null;
    }

    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Author) || Year is null || Year <= 0 || AvailableCopies is null || AvailableCopies < 0)
        {
            ErrorMessage = "All fields are required and must be valid.";
            return;
        }
        if (IsEditMode)
        {
            await Task.Run(() => _db.UpdateBook(new Book { BookID = BookId, Title = Title!, Author = Author!, Year = Year.Value, AvailableCopies = AvailableCopies.Value }));
        }
        else
        {
            await Task.Run(() => _db.AddBook(new Book { Title = Title!, Author = Author!, Year = Year.Value, AvailableCopies = AvailableCopies.Value }));
        }
        _onSuccess?.Invoke();
    }

    private void Cancel()
    {
        _onSuccess?.Invoke();
    }
}
