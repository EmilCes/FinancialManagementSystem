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
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _loadHeader = true;
    [ObservableProperty] 
    private bool _loadContent = true;
    [ObservableProperty]
    private bool _modifyHeader = false;
    [ObservableProperty] 
    private bool _modifyContent = false;
    
    private int selectedCreditId;
    
    
    private bool _validPolitics;
    private CreditType selectedCreditType;
    public ObservableCollection<Politic> Politics { get; }
    public ObservableCollection<CreditType> CreditsList { get; set; } = new();
    public List<CreditType> Credits { get; set; } = new();
    
    private readonly ICreditTypeService _creditTypeService;
    private readonly IPoliticsService _politicsService;


    
    public CreditsPageViewModel()
    {
        _creditTypeService = new CreditTypeService("http://localhost:8080/api/v1/credit-type");
        _politicsService = new PoliticsService("http://localhost:8080/api/v1/politics");

        Politics = new ObservableCollection<Politic>();

        Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadCommand();
            }
        );
    }
    
    private async Task LoadCommand()
    {
        foreach (CreditType creditType in CreditsList)
        {
            CreditsList.Remove(creditType);
        }
        try
        {
            List<CreditType> result = await _creditTypeService.GetCreditsTypeAsync();
            Credits = result;
            foreach (var creditType in result)
            {
                CreditsList.Add(creditType);
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
    public void ModifyCommand(string creditId)
    {
        Dispatcher.UIThread.InvokeAsync(() =>
            {
                GetPoliticsCommand();
            }
        );
        if (int.TryParse(creditId, out int id))
        {
            LoadHeader = false;
            LoadContent = false;
            ModifyHeader = true;
            ModifyContent = true;

            foreach (var creditType in Credits)
            {
                selectedCreditId = id;
                
                if (creditType.CreditTypeId == id)
                {
                    Description = creditType.Description;
                    InterestRate = creditType.InterestRate.ToString();
                    Iva = creditType.Iva.ToString();
                    Term = creditType.Term;
                    State = 1;
                    break;
                }
            }
        }
            
    }
    
    [RelayCommand]
    public async Task ModifyCreditTypeCommand()
    {
        var politics = GetSelectedPolitics();
        
        if (!Validations.ValidateFields(this) || !_validPolitics)
        {
            DialogMessages.ShowInvalidFieldsMessage();
            return;
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
                CreditTypeId = selectedCreditId,
                Description = Description,
                InterestRate = float.Parse(InterestRate),
                State = state,
                Term = Term,
                Iva = float.Parse(Iva),
                Politics = politics
            };
            
            await _creditTypeService.ModifyCreditTypeAsync(request);

            DialogMessages.ShowMessage("Modificación Exitosa", "El Credito fue actualizado correctamente.");

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

    [RelayCommand]
    public void CancelCommand()
    {
        Politics.Clear();
        LoadHeader = true;
        LoadContent = true;
        ModifyHeader = false;
        ModifyContent = false;
    }
    


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
    private string _term;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required (ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    [RegularExpression(@"^\d+(\.\d+)?$", ErrorMessage = ErrorMessages.NUMERIC_FIELD_MESSAGE)]
    private string _iva;
}