using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using MyLibrary.Models;

namespace MyLibrary.Services;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=library.db";

    public List<Book> GetAllBooks()
    {
        var books = new List<Book>();
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT BookID, Title, Author, Year, AvailableCopies FROM Books";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            books.Add(new Book
            {
                BookID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                Year = reader.GetInt32(3),
                AvailableCopies = reader.GetInt32(4)
            });
        }
        return books;
    }

    public List<Borrower> GetAllBorrowers()
    {
        var borrowers = new List<Borrower>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "SELECT BorrowerID, Name, Email, Phone FROM Borrowers ORDER BY Name";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            borrowers.Add(new Borrower
            {
                BorrowerID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            });
        }
        return borrowers;
    }

    public void AddBorrower(Borrower borrower)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "INSERT INTO Borrowers (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";
        command.Parameters.AddWithValue("@Name", borrower.Name);
        command.Parameters.AddWithValue("@Email", borrower.Email);
        command.Parameters.AddWithValue("@Phone", borrower.Phone);
        command.ExecuteNonQuery();
    }

    public void UpdateBorrower(Borrower borrower)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "UPDATE Borrowers SET Name = @Name, Email = @Email, Phone = @Phone WHERE BorrowerID = @BorrowerID";
        command.Parameters.AddWithValue("@Name", borrower.Name);
        command.Parameters.AddWithValue("@Email", borrower.Email);
        command.Parameters.AddWithValue("@Phone", borrower.Phone);
        command.Parameters.AddWithValue("@BorrowerID", borrower.BorrowerID);
        command.ExecuteNonQuery();
    }

    public void DeleteBorrower(int borrowerId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "DELETE FROM Borrowers WHERE BorrowerID = @BorrowerID";
        command.Parameters.AddWithValue("@BorrowerID", borrowerId);
        command.ExecuteNonQuery();
    }

    private void InitializeDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = @"CREATE TABLE IF NOT EXISTS Books (
                BookID INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                Year INTEGER NOT NULL,
                AvailableCopies INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Borrowers (
                BorrowerID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL,
                Phone TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS IssuedBooks (
                IssueID INTEGER PRIMARY KEY AUTOINCREMENT,
                BookID INTEGER NOT NULL,
                BorrowerID INTEGER NOT NULL,
                IssueDate TEXT NOT NULL,
                DueDate TEXT NOT NULL,
                ReturnDate TEXT,
                FOREIGN KEY (BookID) REFERENCES Books(BookID),
                FOREIGN KEY (BorrowerID) REFERENCES Borrowers(BorrowerID)
            );";
        command.ExecuteNonQuery();
    }

    public List<IssuedBook> GetAllIssuedBooks()
    {
        var issuedBooks = new List<IssuedBook>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "SELECT IssueID, BookID, BorrowerID, IssueDate, DueDate, ReturnDate FROM IssuedBooks ORDER BY IssueDate DESC";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var issuedBook = new IssuedBook
            {
                IssueID = reader.GetInt32(0),
                BookID = reader.GetInt32(1),
                BorrowerID = reader.GetInt32(2),
                IssueDate = DateTime.Parse(reader.GetString(3)),
                DueDate = DateTime.Parse(reader.GetString(4)),
                ReturnDate = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5))
            };
            // Load related Book
            issuedBook.Book = GetBookById(issuedBook.BookID);
            issuedBook.Borrower = GetBorrowerById(issuedBook.BorrowerID);
            issuedBooks.Add(issuedBook);
        }
        return issuedBooks;
    }

    public Book? GetBookById(int bookId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "SELECT BookID, Title, Author, Year, AvailableCopies FROM Books WHERE BookID = @BookID";
        command.Parameters.AddWithValue("@BookID", bookId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Book
            {
                BookID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Author = reader.GetString(2),
                Year = reader.GetInt32(3),
                AvailableCopies = reader.GetInt32(4)
            };
        }
        return null;
    }

    public Borrower? GetBorrowerById(int borrowerId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var command = conn.CreateCommand();
        command.CommandText = "SELECT BorrowerID, Name, Email, Phone FROM Borrowers WHERE BorrowerID = @BorrowerID";
        command.Parameters.AddWithValue("@BorrowerID", borrowerId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Borrower
            {
                BorrowerID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            };
        }
        return null;
    }

    public void IssueBook(IssuedBook issuedBook)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            var command = conn.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = "INSERT INTO IssuedBooks (BookID, BorrowerID, IssueDate, DueDate) VALUES (@BookID, @BorrowerID, @IssueDate, @DueDate)";
            command.Parameters.AddWithValue("@BookID", issuedBook.BookID);
            command.Parameters.AddWithValue("@BorrowerID", issuedBook.BorrowerID);
            command.Parameters.AddWithValue("@IssueDate", issuedBook.IssueDate.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@DueDate", issuedBook.DueDate.ToString("yyyy-MM-dd"));
            command.ExecuteNonQuery();

            var updateBookCmd = conn.CreateCommand();
            updateBookCmd.Transaction = transaction;
            updateBookCmd.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE BookID = @BookID";
            updateBookCmd.Parameters.AddWithValue("@BookID", issuedBook.BookID);
            updateBookCmd.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void ReturnBook(int issueId)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            // Get the issued book record
            var issuedBook = GetIssuedBookById(issueId, conn, transaction);
            if (issuedBook != null)
            {
                var updateIssuedCmd = conn.CreateCommand();
                updateIssuedCmd.Transaction = transaction;
                updateIssuedCmd.CommandText = "UPDATE IssuedBooks SET ReturnDate = @ReturnDate WHERE IssueID = @IssueID";
                updateIssuedCmd.Parameters.AddWithValue("@ReturnDate", DateTime.Now.ToString("yyyy-MM-dd"));
                updateIssuedCmd.Parameters.AddWithValue("@IssueID", issueId);
                updateIssuedCmd.ExecuteNonQuery();

                var updateBookCmd = conn.CreateCommand();
                updateBookCmd.Transaction = transaction;
                updateBookCmd.CommandText = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE BookID = @BookID";
                updateBookCmd.Parameters.AddWithValue("@BookID", issuedBook.BookID);
                updateBookCmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private IssuedBook? GetIssuedBookById(int issueId, SqliteConnection conn, SqliteTransaction transaction)
    {
        var command = conn.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT IssueID, BookID, BorrowerID, IssueDate, DueDate, ReturnDate FROM IssuedBooks WHERE IssueID = @IssueID";
        command.Parameters.AddWithValue("@IssueID", issueId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new IssuedBook
            {
                IssueID = reader.GetInt32(0),
                BookID = reader.GetInt32(1),
                BorrowerID = reader.GetInt32(2),
                IssueDate = DateTime.Parse(reader.GetString(3)),
                DueDate = DateTime.Parse(reader.GetString(4)),
                ReturnDate = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5))
            };
        }
        return null;
    }

    public void AddBook(Book book)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO Books (Title, Author, Year, AvailableCopies) VALUES (@title, @author, @year, @copies)";
        command.Parameters.AddWithValue("@title", book.Title);
        command.Parameters.AddWithValue("@author", book.Author);
        command.Parameters.AddWithValue("@year", book.Year);
        command.Parameters.AddWithValue("@copies", book.AvailableCopies);
        command.ExecuteNonQuery();
    }

    public void DeleteBook(int bookId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Books WHERE BookID = @id";
        command.Parameters.AddWithValue("@id", bookId);
        command.ExecuteNonQuery();
    }

    public void UpdateBook(Book book)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = @"UPDATE Books SET Title = @title, Author = @author, Year = @year, AvailableCopies = @copies WHERE BookID = @id";
        command.Parameters.AddWithValue("@title", book.Title);
        command.Parameters.AddWithValue("@author", book.Author);
        command.Parameters.AddWithValue("@year", book.Year);
        command.Parameters.AddWithValue("@copies", book.AvailableCopies);
        command.Parameters.AddWithValue("@id", book.BookID);
        command.ExecuteNonQuery();
    }

    public List<IssuedBook> GetOverdueBooks()
    {
        var overdueBooks = new List<IssuedBook>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var command = conn.CreateCommand();
        command.CommandText = @"SELECT IssueID, BookID, BorrowerID, IssueDate, DueDate, ReturnDate FROM IssuedBooks WHERE ReturnDate IS NULL AND DueDate < @Today ORDER BY DueDate";
        command.Parameters.AddWithValue("@Today", today);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var issuedBook = new IssuedBook
            {
                IssueID = reader.GetInt32(0),
                BookID = reader.GetInt32(1),
                BorrowerID = reader.GetInt32(2),
                IssueDate = DateTime.Parse(reader.GetString(3)),
                DueDate = DateTime.Parse(reader.GetString(4)),
                ReturnDate = reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5))
            };
            issuedBook.Book = GetBookById(issuedBook.BookID);
            issuedBook.Borrower = GetBorrowerById(issuedBook.BorrowerID);
            overdueBooks.Add(issuedBook);
        }
        return overdueBooks;
    }
}
