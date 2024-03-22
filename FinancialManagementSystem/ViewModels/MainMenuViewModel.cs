using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    public MainMenuViewModel()
    {
        Employee employee = Employee.Instance;
        //SetItemsBasedOnRole(employee.Role);
        SetItemsBasedOnRole("ASESOR_CREDITO");
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
    

    public void SetItemsBasedOnRole(string userRole)
    {
        Items.Clear();

        switch (userRole)
        {
            case "ADMIN":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(PoliticsPageViewModel), "Politicas", ""));
                Items.Add(new ListItemTemplate(typeof(RegisterPoliticPageViewModel), "Registrar politica", ""));
                Items.Add(new ListItemTemplate(typeof(CreditTypePageViewModel), "Registrar Crédito", ""));
                break;
            case "ANALISTA_COBRO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                break;
            case "ANALISTA_CREDITO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                break;
            case "ASESOR_CREDITO":
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(ClientPageViewModel), "Registrar Cliente", "PeopleCommunityRegular"));
                Items.Add(new ListItemTemplate(typeof(CreditApplicationViewModel), "Solicitud de crédito", ""));
                break;
            default:
                Items.Add(new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"));
                Items.Add(new ListItemTemplate(typeof(ClientPageViewModel), "Registrar Cliente", "PeopleCommunityRegular")); //QUITAR
                break;
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