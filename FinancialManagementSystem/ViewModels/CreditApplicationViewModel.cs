using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialManagementSystem.Services.CreditAplication;

namespace FinancialManagementSystem.ViewModels;

public partial class CreditApplicationViewModel: ViewModelBase
{
    private readonly ICreditApplicationService _creditApplicationService;

    
    [NotifyDataErrorInfo]
    [Required]
    [ObservableProperty]
    private string? _rfc = string.Empty;

    [ObservableProperty] 
    private SolidColorBrush _rfcBrush;

    public CreditApplicationViewModel()
    {
        _creditApplicationService = new CreditApplicationService("http://localhost:8080/api/v1/creditApplication");
    }

    [RelayCommand]
    public async Task SearchClientCommand()
    {
        string rfc = Rfc;
        try
        {
            VerifyRegularClientResponse response = await _creditApplicationService.VerifyRegularAsync(rfc);
            
            if (response.clientIsRegular)
            {
                RfcBrush = new SolidColorBrush(Colors.Aqua);
            }
            else
            {
                RfcBrush = new SolidColorBrush(Colors.MediumOrchid);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.StackTrace);
            RfcBrush = new SolidColorBrush(Colors.Red);
        }
    }

    [RelayCommand]
    public void AddIdentificationDocumentCommand()
    {
        
    }

    [RelayCommand]
    public void AddProofOfIncomeCommand()
    {
        
    }

    [RelayCommand]
    public void AddProofOfAddressCommand()
    {
        
    }
    
    
}