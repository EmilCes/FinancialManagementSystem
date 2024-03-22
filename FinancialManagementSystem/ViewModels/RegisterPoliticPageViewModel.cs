using FinancialManagementSystem.Services.CRU_Politica;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.Authentication;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class RegisterPoliticPageViewModel : ViewModelBase
{
    private readonly IPoliticsService _politicsService;


    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _description;
    [ObservableProperty]
    private string _state;
    
    public RegisterPoliticPageViewModel()
    {
        _politicsService = new PoliticsService("http://localhost:8080/api/v1/politics");
    }
    
    [RelayCommand]
    public async Task RegisterCommand()
    {


        Politic request = new Politic()
        {
            name = Name,
            description = Description,
            state = State
        };

        if (request.state == "1")
        {
            request.state = "Activo";
        }else if (request.state == "2")
        {
            request.state = "Inactivo";
        }

        try
        {
            await _politicsService.RegisterAsync(request);
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}