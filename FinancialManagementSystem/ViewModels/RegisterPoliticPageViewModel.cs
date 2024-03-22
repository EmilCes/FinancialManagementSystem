using FinancialManagementSystem.Services.CRU_Politica;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Authentication;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class RegisterPoliticPageViewModel : ViewModelBase
{
    private readonly IPoliticsService _politicsService;


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

        if (ValidateFields())
        {
            try
            {
                await _politicsService.RegisterAsync(request);
                DialogMessages.ShowMessage("Registro Exitoso!", "La política fue registrada correctamente.");

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
    
    [RelayCommand]
    public void CancelCommand()
    {
        Name = "";
        Description = "";
    }
}