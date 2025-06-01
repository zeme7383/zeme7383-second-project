using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyLibrary.Models;
using MyLibrary.Services;
using System.ComponentModel.DataAnnotations;

namespace MyLibrary.ViewModels;

public partial class BorrowerFormViewModel : ObservableObject
{
    private readonly Action _onSave;
    private readonly DatabaseService _db = new();

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? email;

    [ObservableProperty]
    private string? phone;

    [ObservableProperty]
    private string? errorMessage;

    public bool IsEditMode { get; set; }
    public int BorrowerId { get; set; }

    public IRelayCommand SaveCommand { get; }

    public BorrowerFormViewModel(Action onSave)
    {
        _onSave = onSave;
        SaveCommand = new RelayCommand(Save);
    }

    private void Save()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Phone))
        {
            ErrorMessage = "Please fill in all fields.";
            return;
        }

        if (!new EmailAddressAttribute().IsValid(Email))
        {
            ErrorMessage = "Please enter a valid email address.";
            return;
        }

        if (!new PhoneAttribute().IsValid(Phone))
        {
            ErrorMessage = "Please enter a valid phone number.";
            return;
        }

        var borrower = new Borrower
        {
            Name = Name,
            Email = Email,
            Phone = Phone
        };

        if (IsEditMode)
        {
            borrower.BorrowerID = BorrowerId;
            _db.UpdateBorrower(borrower);
        }
        else
        {
            _db.AddBorrower(borrower);
        }

        _onSave();
    }
}
