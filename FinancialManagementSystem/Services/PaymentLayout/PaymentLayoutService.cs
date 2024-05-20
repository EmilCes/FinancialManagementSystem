using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.PaymentLayout;

public class PaymentLayoutService : IPaymentLayoutService
{
    private readonly IPaymentLayoutService _api;

    public PaymentLayoutService(string apiUrl)
    {
        _api = RestService.For<IPaymentLayoutService>(apiUrl);
    }
    
    public async Task<List<PaymentLayoutResponse>> GetPaymentLayouts()
    {
        return await _api.GetPaymentLayouts();
    }
}