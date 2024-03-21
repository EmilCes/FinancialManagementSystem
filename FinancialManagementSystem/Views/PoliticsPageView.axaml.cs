using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FinancialManagementSystem.ViewModels;

namespace FinancialManagementSystem.Views;

public partial class PoliticsPageView : UserControl
{
    public PoliticsPageView()
    {
        InitializeComponent();
        DataContext = new PoliticsPageViewModel();
    }
}