using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyLibrary.Models;

public class IssuedBook : INotifyPropertyChanged
{
    public int IssueID { get; set; }

    [Required]
    public int BookID { get; set; }

    [Required]
    public int BorrowerID { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    private DateTime? returnDate;
    public DateTime? ReturnDate
    {
        get => returnDate;
        set
        {
            if (returnDate != value)
            {
                returnDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReturnDate)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverdue)));
            }
        }
    }

    [NotMapped]
    public Book? Book { get; set; }

    [NotMapped]
    public Borrower? Borrower { get; set; }

    [NotMapped]
    public bool IsOverdue => !ReturnDate.HasValue && DueDate < DateTime.Today;

    [NotMapped]
    public string StatusText => ReturnDate.HasValue ? "Returned" : (IsOverdue ? "Overdue" : "Active");

    public event PropertyChangedEventHandler? PropertyChanged;
}
