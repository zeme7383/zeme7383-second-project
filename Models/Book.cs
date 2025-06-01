namespace MyLibrary.Models;

public class Book
{
    public int BookID { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public int Year { get; set; }
    public int AvailableCopies { get; set; }
}
