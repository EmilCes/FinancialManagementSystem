using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Services.Credit.Dto;
using Refit;

namespace FinancialManagementSystem.Services.Credit;

public class CreditService : ICreditService
{
    private readonly ICreditService _api;
    
    public CreditService(string apiUrl)
    {
        _api = RestService.For<ICreditService>(apiUrl);
    }

    
    public async Task<List<GetCreditResponse>> GetCreditsAsync()
    {
        return await _api.GetCreditsAsync();
    }

    public async Task<List<CreditByIdResponse>> GetCreditAsync(int id)
    {
        return await _api.GetCreditAsync(id);
    }

    public async Task ValidateCreditApplicationAsync(ICreditService.ValidateCreditApplicationRequest request)
    {
        await _api.ValidateCreditApplicationAsync(request);
    }
}