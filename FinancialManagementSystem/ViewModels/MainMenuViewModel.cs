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
        
        messenger.Register<MainMenuViewModel, ViewClientPageMessage>(this, (_, message) =>
        {
            CurrentPage = new ClientPageViewModel();
        });
        
        messenger.Register<MainMenuViewModel, ViewClientMessageFromValidation>(this, (_, message) =>
        {
            CurrentPage = new ClientPageViewModel(message.Value.Client, message.Value.CreditApplication);
        });
        
        messenger.Register<MainMenuViewModel, ViewClientsMessage>(this, (_, message) =>
        {
            CurrentPage = new ClientsPageViewModel();
        });
        
        messenger.Register<MainMenuViewModel, PaymentUploadMessage>(this, (_, message) =>
        {
            CurrentPage = new PaymentUploadPageViewModel();
        });
        
        messenger.Register<MainMenuViewModel, ViewPaymentMessage>(this, (_, message) =>
        {
            CurrentPage = new ConductPaymentPageViewModel(message.Value);
        });
        
        messenger.Register<MainMenuViewModel, ViewPaymentMessageWithoutPayment>(this, (_, message) =>
        {
            CurrentPage = new ConductPaymentPageViewModel(message.Value);
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
        
        messenger.Register<MainMenuViewModel, CreateCreditApplication>(this, (_, message) =>
        {
            CurrentPage = new CreditApplicationViewModel(message.Value);
        });
        
        Employee employee = Employee.Instance;
        SetItemsBasedOnRole(employee.Role);
        string username = employee.FirstName + " " + employee.LastName;
        Username = username; 
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
                Items.Add(new ListItemTemplate(typeof(CreditsPageViewModel), "Creditos", ""));
                Items.Add(new ListItemTemplate(typeof(CreditTypePageViewModel), "Registrar Crédito", ""));
                Items.Add(new ListItemTemplate(typeof(EmployeeRegistrationPageViewModel), "Registrar Trabajador", ""));
                Items.Add(new ListItemTemplate(typeof(SearchWorkerPageViewModel), "Buscar Trabajador", ""));
                break;
            case "ANALISTA_COBRO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(PaymentLayoutGenerationViewModel), "Layouts de Cobro", ""));
                Items.Add(new ListItemTemplate(typeof(PaymentUploadPageViewModel), "Subir cobro", ""));
                Items.Add(new ListItemTemplate(typeof(EfficienciesPageViewModel), "Eficiencias", ""));
                break;
            case "ANALISTA_CREDITO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(ValidateCreditApplicationPageViewModel), "Validación de solicitudes", ""));

                break;
            case "ASESOR_CREDITO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(ClientsPageViewModel), "Clientes", "PeopleCommunityRegular"));
                Items.Add(new ListItemTemplate(typeof(CreditApplicationViewModel), "Solicitud de crédito", "DocumentToolboxRegular"));
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