using System.Collections.Generic;
using MyLibrary.Models;

namespace MyLibrary.Services;

public class ReportService
{
    private readonly DatabaseService _db = new();

    public IEnumerable<IssuedBook> GetOverdueBooks()
    {
        return _db.GetOverdueBooks();
    }
}
