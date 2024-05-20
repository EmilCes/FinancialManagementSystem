using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.PaymentLayout;

public interface IPaymentLayoutService
{
    [Get("/")]
    Task<List<PaymentLayoutResponse>> GetPaymentLayouts();
}

public class PaymentLayoutResponse
{
    public int paymentLayoutId { get; set; }
    public string startDate { get; set; }
    public string clientRfc { get; set; }
    public string clientName { get; set; }
    public Models.CreditType CreditType { get; set; }
}