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

public partial class ModifyPoliticPageViewModel : ViewModelBase
{
    private readonly IPoliticsService _politicsService;


    [ObservableProperty]
    private string _name;
    [ObservableProperty]
    private string _description;
    [ObservableProperty]
    private string _state;
    
    public ModifyPoliticPageViewModel(Politic politic)
    {
        _politicsService = new PoliticsService("http://localhost:8080/api/v1/politics");
    }
    
    [RelayCommand]
    public async Task ModifyCommand()
    {


        Politic modifyRequest = new Politic()
        {
            name = Name,
            description = Description,
            state = State
        };

        try
        {
            await _politicsService.ModifyAsync(modifyRequest);
        }
        catch (ApiException ex)
        {
            Console.WriteLine(ex.StatusCode);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}