using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;

namespace FinancialManagementSystem.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    [ObservableProperty] 
    private string _username;
    [ObservableProperty]
    private bool _isPaneOpen = true;
    [ObservableProperty] 
    private ViewModelBase _currentPage = new HomePageViewModel();
    [ObservableProperty] 
    private ListItemTemplate? _selectedListItem;
    
    public ObservableCollection<ListItemTemplate> Items { get; } = new();

    public MainMenuViewModel(IMessenger messenger)
    {
        messenger.Register<MainMenuViewModel, ViewClientMessage>(this, (_, message) =>
        {
            CurrentPage = new ClientPageViewModel(message.Value);
        });
        
        messenger.Register<MainMenuViewModel, ViewClientsMessage>(this, (_, message) =>
        {
            CurrentPage = new ClientsPageViewModel();
        });

        messenger.Register<MainMenuViewModel, ViewCreditAplicationMessage>(this, (_, message) =>
        { 
            CurrentPage = new CreditApplicationViewModel(message.Value); 
        });
        
        messenger.Register<MainMenuViewModel, ViewWorkerMessage>(this, (_, message) =>
        {
            CurrentPage = new EmployeeModificationPageViewModel(message.Value.Rfc);
        });
        
        messenger.Register<MainMenuViewModel, SearchWorkerMessage>(this, (_, message) =>
        {
            CurrentPage = new SearchWorkerPageViewModel();
        });
        
        //Employee employee = Employee.Instance;
        //SetItemsBasedOnRole(employee.Role);
        //string username = employee.FirstName + " " + employee.LastName;
        //Username = username;
        //SetItemsBasedOnRole("ANALISTA_COBRO");
        SetItemsBasedOnRole("ASESOR_CREDITO");
    }

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
    }
    

    private void SetItemsBasedOnRole(string userRole)
    {
        Items.Clear();

        switch (userRole)
        {
            case "ADMIN":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(PoliticsPageViewModel), "Politicas", ""));
                Items.Add(new ListItemTemplate(typeof(RegisterPoliticPageViewModel), "Registrar politica", ""));
                Items.Add(new ListItemTemplate(typeof(CreditTypePageViewModel), "Registrar Crédito", ""));
                Items.Add(new ListItemTemplate(typeof(EmployeeRegistrationPageViewModel), "Registrar Trabajador", ""));
                Items.Add(new ListItemTemplate(typeof(SearchWorkerPageViewModel), "Buscar Trabajador", ""));
                break;
            case "ANALISTA_COBRO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(PaymentLayoutGenerationViewModel), "Layouts de Cobro", ""));
                break;
            case "ANALISTA_CREDITO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(ValidateCreditApplicationPageViewModel), "Validación de solicitudes", ""));
                Items.Add(new ListItemTemplate(typeof(CreditsPageViewModel), "Validación", ""));

                break;
            case "ASESOR_CREDITO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(ClientPageViewModel), "Registrar Cliente", "PeopleCommunityRegular"));
                Items.Add(new ListItemTemplate(typeof(ClientsPageViewModel), "Clientes", "Search"));
                Items.Add(new ListItemTemplate(typeof(CreditApplicationViewModel), "Solicitud de crédito", ""));
                break;
            default:
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                break;
        }
    }

    [RelayCommand]
    public void LogoutCommand()
    {
        ChangeWindowToLogin();
    }

    private void ChangeWindowToLogin()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var currentWindow = desktop.MainWindow;
            
            desktop.MainWindow = new Views.Login
            {
                DataContext = new LoginViewModel(),
            };
            
            desktop.MainWindow.Show();
            currentWindow?.Close();
        }
    }
}

public class ListItemTemplate
{
    public string Label { get;  }
    public Type ModelType { get;  }
    public StreamGeometry ListItemIcon { get; }

    public ListItemTemplate(Type type, string label, string iconKey)
    {
        ModelType = type;
        Label = label;

        Application.Current!.TryGetResource(iconKey, out var res);
        ListItemIcon = (StreamGeometry)res!;
    }

}