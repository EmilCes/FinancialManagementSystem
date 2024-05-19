using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.CreditApplication;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class ValidateCreditApplicationPageViewModel: ViewModelBase
{
    [ObservableProperty]
    private bool _loadHeader = true;
    [ObservableProperty] 
    private bool _loadContent = true;
    [ObservableProperty]
    private bool _modifyHeader;
    [ObservableProperty] 
    private bool _modifyContent;

    private readonly IMessenger _messenger = Message.Instance;
    
    private CreditApplication selectedAplication;

    
    public ObservableCollection<CreditApplication> CreditApplicationList { get; set; } = new();
    public List<CreditApplication> CreditApplications { get; set; } = new();
    
    private readonly ICreditApplicationService _creditApplicationService;
    public ValidateCreditApplicationPageViewModel()
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");

        Dispatcher.UIThread.InvokeAsync(() =>
            {
                LoadCommand();
            }
        );
    }

    private async Task LoadCommand()
    {
        foreach (CreditApplication aplications in CreditApplicationList)
        {
            CreditApplicationList.Remove(aplications);
        }
        try
        {
            List<CreditApplication> result = await _creditApplicationService.GetCreditAplicationsTypesAsync();
            CreditApplications = result;
            foreach (var aplication in result)
            {
                CreditApplicationList.Add(aplication);
                Console.WriteLine(aplication.CreditApplicationId);
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
    public void ValidateCommand(string ApplicationId)
    {
        foreach (CreditApplication aplications in CreditApplicationList)
        {
            if (aplications.CreditApplicationId.ToString() == ApplicationId)
            {
                selectedAplication = aplications;
                Console.WriteLine(selectedAplication.IdentificationPdf.Length);
            }
        }
        _messenger.Send(new ViewCreditAplicationMessage(selectedAplication));
    }
}