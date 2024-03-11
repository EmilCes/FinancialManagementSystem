using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FinancialManagementSystem.ViewModels;

public partial class MainMenuViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPaneOpen = true;

    [ObservableProperty] 
    private ViewModelBase _currentPage = new HomePageViewModel();

    [ObservableProperty] 
    private ListItemTemplate? _selectedListItem;

    partial void OnSelectedListItemChanged(ListItemTemplate? value)
    {
        if (value is null) return;
        var instance = Activator.CreateInstance(value.ModelType);
        if (instance is null) return;
        CurrentPage = (ViewModelBase)instance;
    }

    public ObservableCollection<ListItemTemplate> Items { get; } = new()
    {
        new ListItemTemplate(typeof(HomePageViewModel), "Menu Principal", "HomeRegular"),
        new ListItemTemplate(typeof(ClientPageViewModel), "Registrar Cliente", "PeopleCommunityRegular")
    };
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