using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using FinancialManagementSystem.Models;
using FinancialManagementSystem.Services.Credit.Dto;
using Refit;

namespace FinancialManagementSystem.Services.Credit;

public interface ICreditService
{
    [Get("/")]
    Task<List<GetCreditResponse>> GetCreditsAsync();
    
    [Get("/{id}")]
    Task<List<CreditByIdResponse>> GetCreditAsync(int id);
    
    [Post("/")]
    Task ValidateCreditApplicationAsync([Body] ValidateCreditApplicationRequest request);

    public class ValidateCreditApplicationRequest
    {
        public string Comments { get; set; }
        public List<Politic> RejectedPolicies { get; set; }
        public int UserId { get; set; }
        public int CreditApplicationId { get; set; }
    }
}