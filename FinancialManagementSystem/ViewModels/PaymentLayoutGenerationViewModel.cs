using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Credit;
using FinancialManagementSystem.Services.Credit.Dto;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class PaymentLayoutGenerationViewModel : ViewModelBase
{
    [ObservableProperty] 
    private ObservableCollection<string>? _layoutTypes;
    [ObservableProperty] 
    private string? _selectedItem;
    [ObservableProperty]
    private string _rfc = null!;
    
    private readonly ICreditService _creditService;
    private List<GetCreditResponse> CreditsListCopy { get; set; } = new();
    
    public ObservableCollection<GetCreditResponse> CreditsList { get; set; } = new();


    public PaymentLayoutGenerationViewModel()
    {
        _creditService = new CreditService("http://localhost:8080/api/v1/credit");

        LoadData();
        Initialize();
    }
    
    private async void Initialize()
    {
        await LoadCommand();
    }

    private void LoadData()
    {
        LayoutTypes = ["Mes", "Año", "Periodo Completo"];
    }
    
    private async Task LoadCommand()
    {
        try
        {
            var result = await _creditService.GetCreditsAsync();
            CreditsListCopy = result;
            
            FillObservableCollection(CreditsList, result);
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
    public void FilterCreditsByRfc()
    {
        if (string.IsNullOrEmpty(Rfc))
        {
            FillObservableCollection(CreditsList, CreditsListCopy);
            return;
        }

        var filteredCredits = CreditsListCopy.Where(credit => credit.ClientRfc.Contains(Rfc, StringComparison.OrdinalIgnoreCase));

        var enumerable = filteredCredits.ToList();
        CreditsList.Clear();

        if (enumerable.Count == 0)
        {
            DialogMessages.ShowMessage("", "Creditos no encontrados. Verifica el RFC ingresado.");
        }
        else
        {
            foreach (var client in enumerable.ToList())
            {
                CreditsList.Add(client);
            }
        }
    }

    [RelayCommand]
    public async void DownloadPaymentLayout()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);

            // Start async operation to open the dialog.
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Guardar Layout de Cobro"
            });

            if (file != null)
            {
                Console.WriteLine(file.Path);
            }
        }
    }
    
    private void FillObservableCollection<T>(ObservableCollection<T> observableCollection, List<T> listToCopy)
    {
        observableCollection.Clear();
        
        foreach (var item in listToCopy)
        {
            observableCollection.Add(item);
        }
    }
    
}