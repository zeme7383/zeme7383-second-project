using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MyLibrary.Models;
using MyLibrary.Services;
using System;

namespace MyLibrary.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    public ObservableCollection<IssuedBook> OverdueBooks { get; } = new();
    private readonly ReportService _reportService = new();

    public ReportsViewModel()
    {
        LoadOverdueBooks();
    }

    public void LoadOverdueBooks()
    {
        OverdueBooks.Clear();
        foreach (var issuedBook in _reportService.GetOverdueBooks())
            OverdueBooks.Add(issuedBook);
    }
}
