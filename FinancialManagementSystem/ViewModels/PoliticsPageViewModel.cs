using FinancialManagementSystem.Services.CRU_Politica;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Authentication;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class PoliticsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _loadHeader = true;
    [ObservableProperty] 
    private bool _loadContent = true;
    [ObservableProperty]
    private bool _modifyHeader;
    [ObservableProperty] 
    private bool _modifyContent;

    private int selectedPoliticId;

    public ObservableCollection<Politic> PoliticsList { get; set; } = new();
    public List<Politic> Politics { get; set; } = new();


    private readonly IPoliticsService _politicsService;

    public PoliticsPageViewModel()
    {
        _politicsService = new PoliticsService("http://localhost:8080/api/v1/politics");

        Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadCommand();
            }
        );
    }

    private async Task LoadCommand()
    {
        foreach (Politic politic in PoliticsList)
        {
            PoliticsList.Remove(politic);
        }
        try
        {
            List<Politic> result = await _politicsService.GetPoliticsAsync();
            Politics = result;
            foreach (var politic in result)
            {
                PoliticsList.Add(politic);
            }
        }
        catch (ApiException)
        {
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException)
        {
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
    }
    
    [RelayCommand]
    public void ModifyCommand(string politicId)
    {
        if (int.TryParse(politicId, out int id))
        {
            LoadHeader = false;
            LoadContent = false;
            ModifyHeader = true;
            ModifyContent = true;

            foreach (var politic in PoliticsList)
            {
                selectedPoliticId = id;
                
                if (politic.politicId == id)
                {
                    Name= politic.name;
                    Description = politic.description;
                    if (politic.state == "Activo")
                    {
                        State = "0";
                    }
                    else
                    {
                        State = "1";
                    }
                    
                    break;
                }
            }
        }
            
    }
    
    [RelayCommand]
    public void CancelCommand()
    {
        LoadHeader = true;
        LoadContent = true;
        ModifyHeader = false;
        ModifyContent = false;
    }
    
    [RelayCommand]
    public async Task ConfirmModifyCommand()
    {
        Politic request = new Politic()
        {
            politicId = selectedPoliticId,
            name = Name,
            description = Description,
            state = State
        };

        if (request.state == "0")
        {
            request.state = "Activo";
        }else if (request.state == "1")
        {
            request.state = "Inactivo";
        }

        if (ValidateFields())
        {
            try
            {
                await _politicsService.ModifyAsync(request);
                DialogMessages.ShowMessage("Modificación Exitosa!", "La politica fue modificada correctamente.");
                LoadCommand();
                LoadHeader = true;
                LoadContent = true;
                ModifyHeader = false;
                ModifyContent = false;
            }
            catch (ApiException)
            {
                DialogMessages.ShowApiExceptionMessage();
            }
            catch (HttpRequestException)
            {
                DialogMessages.ShowHttpRequestExceptionMessage();
            }
        }
    }
    
    private bool ValidateFields()
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(this);
        return Validator.TryValidateObject(this, validationContext, validationResults, true);
    }
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _name;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _description;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)] 
    private string _state;
    
    
    
    
}