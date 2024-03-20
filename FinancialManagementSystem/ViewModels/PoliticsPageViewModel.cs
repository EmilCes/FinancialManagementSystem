using FinancialManagementSystem.Services.CRU_Politica;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.Authentication;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class PoliticsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _description;
    [ObservableProperty]
    private string _state;

    public ObservableCollection<Politic> PoliticsList { get; set; } = new();
    public List<Politic> Politics { get; set; } = new();

    
    private readonly IPoliticsService _politicsService;

    public PoliticsPageViewModel()
    {
        _politicsService = new PoliticsService("http://localhost:8080/api/v1/politics");
        Initialize();
    }

    private async void Initialize()
    {
        await LoadCommand();
    }

    private async Task LoadCommand()
    {
        try
        {
            object objectNull = new object();
            List<Politic> result = await _politicsService.GetPoliticsAsync(objectNull);
            Politics = result;
            
            foreach (var politic in result)
            {
                PoliticsList.Add(politic);
            }
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
    
    private void ModifyCommand()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var currentWindow = desktop.MainWindow;
            
            
            desktop.MainWindow.Show();
            currentWindow?.Close();
        }
    }
}