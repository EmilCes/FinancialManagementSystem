using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Services.Credit.Dto;
using Refit;

namespace FinancialManagementSystem.Services.Credit;

public interface ICreditService
{
    [Get("/")]
    Task<List<GetCreditResponse>> GetCreditsAsync();
}