using System;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FinancialManagementSystem.Messages;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.Payment;
using FinancialManagementSystem.ViewModels.Helpers;
using Refit;

namespace FinancialManagementSystem.ViewModels;

public partial class ConductPaymentPageViewModel: ViewModelBase
{
    private readonly IPaymentService _paymentService;
    private PaymentRecord _paymentRecord;
    
    public ConductPaymentPageViewModel(string rfc)
    {
        _paymentService = new PaymentService("http://localhost:8080/api/v1/payment");
        _cancelButtonText = "Regresar";
        _paymentButtonVisibility = false;

        Initialize(rfc);
    }

    private async Task Initialize(string rfc)
    {
        try
        {
            PaymentResponse paymentResponse = await _paymentService.GetPaymentInfoAsync(rfc);
            
            string monthDeadlineDate = paymentResponse.monthDeadlineDate;
            string formattedDate = string.IsNullOrEmpty(monthDeadlineDate) ? "N/A" : monthDeadlineDate.Split('T')[0];

            ClientName = paymentResponse.clientName;
            AddedAmount = "$" + 0;
            PendingAmount = "$" + paymentResponse.pendingAmount.ToString("N2");;
            Deadline = formattedDate;
            RemainingMonths = paymentResponse.termType + " restantes: " + paymentResponse.remainingMonths;
            RemainingAmount = "$" + paymentResponse.amountForNoInterest.ToString("N2");

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
    
    public ConductPaymentPageViewModel(PaymentRecord paymentRecord)
    {
        _paymentService = new PaymentService("http://localhost:8080/api/v1/payment");
        _paymentRecord = paymentRecord;
        _paymentButtonVisibility = true;
        _cancelButtonText = "Cancelar pago";
        
        InitializeWithPayment();
    }

    private async Task InitializeWithPayment()
    {
        try
        {
            if (!await _paymentService.PaymentExist(_paymentRecord.folio))
            {
                PaymentResponse paymentResponse = await _paymentService.GetPaymentInfoAsync(_paymentRecord.rfc);
            
                string monthDeadlineDate = paymentResponse.monthDeadlineDate;
                string formattedDate = string.IsNullOrEmpty(monthDeadlineDate) ? "N/A" : monthDeadlineDate.Split('T')[0];

                float pendingAmount = paymentResponse.pendingAmount - (float)_paymentRecord.amount;
                float amountForNoInterest = paymentResponse.amountForNoInterest - (float)_paymentRecord.amount;
            
                ClientName = paymentResponse.clientName;
                AddedAmount = "$" + _paymentRecord.amount.ToString("N2");
                PendingAmount = "$" + pendingAmount.ToString("N2");;
                Deadline = formattedDate;
                RemainingMonths = paymentResponse.remainingMonths + " (" + paymentResponse.termType + ")";
                RemainingAmount = "$" + amountForNoInterest.ToString("N2");
            }
            else
            {
                IMessenger messenger = Message.Instance;
                messenger.Send(new PaymentUploadMessage());
                DialogMessages.ShowMessage("Folio ya existe", "El folio de este pago ya existe.");
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
    
    [RelayCommand]
    public async Task CancelPaymentCommand()
    {
        IMessenger messenger = Message.Instance;
        messenger.Send(new PaymentUploadMessage());
    }

    [RelayCommand]
    public async Task ConductPaymentCommand()
    {
        try
        {
            await _paymentService.SavePaymentAsync(_paymentRecord);
            
            IMessenger messenger = Message.Instance;
            messenger.Send(new PaymentUploadMessage());
        }catch (ApiException e)
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
    
    [ObservableProperty] 
    private string? _clientName;
    
    [ObservableProperty] 
    private string? _addedAmount;

    [ObservableProperty]
    private string? _pendingAmount;

    [ObservableProperty] 
    private string? _deadline;

    [ObservableProperty] 
    private string? _remainingMonths;

    [ObservableProperty] 
    private string? _remainingAmount;

    [ObservableProperty]
    private bool _paymentButtonVisibility;

    [ObservableProperty] 
    private string? _cancelButtonText;

}