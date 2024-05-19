using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Authentication;
using FinancialManagementSystem.Services.CreditType;
using FinancialManagementSystem.Services.CRU_Politica;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditTypePageViewModel : ViewModelBase
{
    private readonly ICreditTypeService _creditTypeService;
    private readonly IPoliticsService _politicsService;
    public ObservableCollection<Politic> Politics { get; }
    private bool _validPolitics;
    private bool _validTermType;
    
    public CreditTypePageViewModel()
    {
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        _politicsService = new PoliticsService("http://localhost:8080/api/v1/politics");

        Politics = new ObservableCollection<Politic>();

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            GetPoliticsCommand();
        });
    }

    [RelayCommand]
    public async Task RegisterCreditTypeCommand()
    {
        var politics = GetSelectedPolitics();
        var termType = GetSelectedTermType();
        
        if (!Validations.ValidateFields(this) || !_validTermType)
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
        }
        else
        {
            if (!_validPolitics)
            {
                DialogMessages.ShowMessage("Politicas no Seleccionadas", "Debes seleccionar al menos una política.");
                return;
            }
        }
        
        try
        {
            string state = State switch
            {
                1 => "Activo",
                2 => "Inactivo",
                _ => ""
            };

            var request = new CreditType()
            {
                Description = Description,
                InterestRate = float.Parse(InterestRate),
                State = state,
                Term = int.Parse(Term),
                Iva = float.Parse(Iva),
                Amount = float.Parse(Amount),
                Politics = politics,
                TermType = termType
            };
            
            await _creditTypeService.RegisterCreditTypeAsync(request);

            DialogMessages.ShowMessage("Registro Exitoso", "El Crédito fue registrado correctamente.");

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
    
    private async Task GetPoliticsCommand()
    {
        try
        {
            var response = await _politicsService.GetPoliticsAsync();

            foreach (var politic in response)
            {
                Politics.Add(politic);
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

    private List<Politic> GetSelectedPolitics()
    {
        List<Politic> politics = Politics.Where(politic => politic.cbPoliticState).ToList();
        
        _validPolitics = (politics.Count != 0);

        return politics;
    }

    private string GetSelectedTermType()
    {
        _validTermType = true;
        
        if (WeeklyTermType) return "Semanal";
        if (BiweeklyTermType) return "Quincenal";
        if (MonthlyTermType) return "Mensual";

        _validTermType = false;
        return string.Empty;
    }
    
    [ObservableProperty]
    private bool _weeklyTermType;
    [ObservableProperty]
    private bool _biweeklyTermType;
    [ObservableProperty]
    private bool _monthlyTermType;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private string _description;
    
    [ObservableProperty] 
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _interestRate;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [Range(1, 2, ErrorMessage = ErrorMessages.SELECT_VALUE_MESSAGE)]
    private int _state;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _term;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _iva;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _amount;
}