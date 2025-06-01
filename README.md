# MyLibrary

A modern, cross-platform library management application built with Avalonia UI and C#.

_I used Copilot to create the initial file and folder structure, and used ChatGPT to help understand and resolve issues during development._

⚠️ _Due to an unexpected issue with my Windows PC, I had to continue development on a Linux system. As WinForms isn't available on Linux, I used Avalonia as a cross-platform alternative, and integrated SQLite with ADO.NET._

## Features

- **User Authentication:** Secure login with password visibility toggle.
- **Books Management:**
  - Add, edit, delete books
  - Filter books by author or year range
- **Borrowers Management:**
  - Add, edit, delete borrowers
- **Book Issuing:**
  - Issue books to borrowers
  - Return books
- **Reports:**
  - View overdue books (where due date < today and not returned)
- **Modern UI:**
  - Fluent design, responsive layout, and vector icons

## Getting Started

### Prerequisites

- [.NET 7.0+ SDK](https://dotnet.microsoft.com/download)
- [Avalonia UI](https://avaloniaui.net/)

### Running the App

1. Clone the repository:
   ```sh
   git clone <your-repo-url>
   cd MyLibrary
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Build and run:
   ```sh
   dotnet run
   ```

## Project Structure

- `Views/` - Avalonia XAML UI files
- `ViewModels/` - MVVM logic and data binding
- `Models/` - Data models (Book, Borrower, IssuedBook)
- `Services/` - Database and business logic
- `Helpers/` - Value converters and utilities
- `App.axaml` - Application resources and styles

## Technologies Used

- [Avalonia UI](https://avaloniaui.net/) (cross-platform XAML UI)
- C# 10 / .NET 7+
- MVVM pattern
- SQLite (via ADO.NET/Microsoft.Data.Sqlite)

---

_Built with Avalonia UI. For more info, see [Avalonia Documentation](https://docs.avaloniaui.net/)._
