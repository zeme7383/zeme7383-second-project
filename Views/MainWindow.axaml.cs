using Avalonia.Controls;

namespace MyLibrary.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MyLibrary.ViewModels.MainWindowViewModel();
    }
}