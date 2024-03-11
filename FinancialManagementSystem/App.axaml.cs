using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using FinancialManagementSystem.ViewModels;
using FinancialManagementSystem.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialManagementSystem;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new Views.Login()
            {
                DataContext = new LoginViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}