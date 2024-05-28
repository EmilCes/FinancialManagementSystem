using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Efficiencies;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class EfficienciesPageViewModel:  ViewModelBase
{

    private readonly IEfficienciesService _efficienciesService;
    
    public EfficienciesPageViewModel()
    {
        _efficienciesService = new EfficienciesService("http://localhost:8080/api/v1/efficiencies");
        Initialize();
    }

    private async Task Initialize()
    {
        try
        {
            DateTime now = DateTime.Now;
            int thisYear = now.Year;
            int firstCreditYear = await _efficienciesService.GetYear();

            while (thisYear <= firstCreditYear)
            {
                _years.Add(thisYear);
                thisYear++;
            }
        }
        catch (ApiException e)
        {
            Console.WriteLine(e.ToString());
            DialogMessages.ShowApiExceptionMessage();
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            DialogMessages.ShowHttpRequestExceptionMessage();
        }
        
    }

    public ObservableCollection<MonthResponse> YearList { get; set; } = new();

    
    [RelayCommand]
    public async Task GetEfficiencies()
    {
        int selectedYear = _selectedYear;

        if (selectedYear != 0)
        {
            int counter = 0;
            double totalYearExpectedAmount = 0;
            double totalYearActualAmount = 0;
            try
            {
                List<MonthResponse> result = await _efficienciesService.GetMonthlyEfficiencies(selectedYear);
                foreach (var month in result)
                {
                    if (month.ExpectedAmount != 0)
                    {
                        month.Efficiencies = (month.ActualAmount / month.ExpectedAmount) * 100;
                        totalYearActualAmount += month.ActualAmount;
                        totalYearExpectedAmount += month.ExpectedAmount;
                    }
                    else
                    {
                        month.Efficiencies = 0;
                    }

                    if (month.ActualAmount == 0)
                    {
                        counter++;
                    }

                    setLabels(totalYearActualAmount, totalYearExpectedAmount);
                    checkPayments(counter);
                    changeMonthName(month);
                }
                FillObservableCollection(YearList, result);
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.ToString());
                DialogMessages.ShowApiExceptionMessage();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                DialogMessages.ShowHttpRequestExceptionMessage();
            }
        }
        else
        {
            DialogMessages.ShowInvalidFieldsMessage();
        }
    }

    private void setLabels(double totalYearActualAmount, double totalYearExpectedAmount)
    {
        TotalPaymentAwaited = "Pago anual esperado: $" + totalYearExpectedAmount.ToString("N2");
        TotalPaymentObtained = "Pago anual recibido: $" + totalYearActualAmount.ToString("N2");

        double totalEfficiencies = (totalYearActualAmount / totalYearExpectedAmount) * 100;

        TotalEfficiencies = "Eficiencias anuales: " + totalEfficiencies.ToString("N2") + "%";

    }

    private void checkPayments(int numberOfMonthsWithoutPayments)
    {
        if (numberOfMonthsWithoutPayments == 12)
        {
            DialogMessages.ShowMessage("Sin pagos", "El año seleccionado no presenta pagos.");
        }
    }

    
    private void changeMonthName(MonthResponse month)
    {
        switch (month.Month)
        {
            case "JANUARY":
                month.Month = "Enero";
                break;
            case "FEBRUARY":
                month.Month = "Febrero";
                break;
            case "MARCH":
                month.Month = "Marzo";
                break;
            case "APRIL":
                month.Month = "Abril";
                break;
            case "MAY":
                month.Month = "Mayo";
                break;
            case "JUNE":
                month.Month = "Junio";
                break;
            case "JULY":
                month.Month = "Julio";
                break;
            case "AUGUST":
                month.Month = "Agosto";
                break;
            case "SEPTEMBER":
                month.Month = "Septiembre";
                break;
            case "OCTOBER":
                month.Month = "Octubre";
                break;
            case "NOVEMBER":
                month.Month = "Noviembre";
                break;
            case "DECEMBER":
                month.Month = "Diciembre";
                break;
        }
        
    }

    private bool ValidateFields()
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(this);
        return Validator.TryValidateObject(this, validationContext, validationResults, true);
    }
    
    private void FillObservableCollection<T>(ObservableCollection<T> observableCollection, List<T> listToCopy)
    {
        observableCollection.Clear();
        
        foreach (var item in listToCopy)
        {
            observableCollection.Add(item);
        }
    }
    
    
    [ObservableProperty] 
    private string? _monthName;
    
    [ObservableProperty] 
    private string? _actualAmount;

    [ObservableProperty] 
    private string? _expectedAmount;

    [ObservableProperty] 
    private string? _efficiencies;
    
    [ObservableProperty] 
    private string _totalPaymentAwaited;
    
    [ObservableProperty] 
    private string _totalEfficiencies;
    
    [ObservableProperty] 
    private string _totalPaymentObtained;
    
    [ObservableProperty] 
    private ObservableCollection<int> _years = new ObservableCollection<int>();
    
    [ObservableProperty] 
    [Required(ErrorMessage = ErrorMessages.REQUIRED_FIELD_MESSAGE)]
    private int _selectedYear;
}