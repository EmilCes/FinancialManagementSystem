using System.Threading.Tasks;
using FinancialManagementSystem.Models.Helpers;
using FinancialManagementSystem.Services.CreditApplication;
using Refit;

namespace FinancialManagementSystem.Services.Payment;

public class PaymentService: IPaymentService
{

    private readonly IPaymentService _api;

    public PaymentService(string apiUrl)
    {
        _api = RestService.For<IPaymentService>(apiUrl);
    }
        
    public async Task<PaymentResponse> GetPaymentInfoAsync(string rfc)
    {
        return await _api.GetPaymentInfoAsync(rfc);
    }

    public async Task<bool> PaymentExist(string folio)
    {
        return await _api.PaymentExist(folio);
    }

    public async Task SavePaymentAsync(PaymentRecord paymentRecord)
    {
        _api.SavePaymentAsync(paymentRecord);
    }
}