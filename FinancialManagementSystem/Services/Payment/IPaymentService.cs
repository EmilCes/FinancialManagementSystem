using System;
using System.Threading.Tasks;
using FinancialManagementSystem.Models.Helpers;
using Refit;

namespace FinancialManagementSystem.Services.Payment;

public interface IPaymentService
{
    [Get("/credit")]
    Task<PaymentResponse> GetPaymentInfoAsync([Query] string rfc);

    [Get("/existance")]
    Task<bool> PaymentExist([Query] string folio);

    [Post("/savePayment")]
    Task SavePaymentAsync([Body] PaymentRecord paymentRecord);
}

public class PaymentResponse()
{
    public string clientName { get; set; }
    public float pendingAmount { get; set; }
    public long remainingMonths { get; set; }
    public float amountForNoInterest { get; set; }
    public string monthDeadlineDate { get; set; } 
    public string  termType { get; set; }
}