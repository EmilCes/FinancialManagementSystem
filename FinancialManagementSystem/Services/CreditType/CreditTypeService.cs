using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditType;

public class CreditTypeService : ICreditTypeService
{
    private readonly ICreditTypeService _api;
    
    public CreditTypeService(string apiUrl)
    {
        _api = RestService.For<ICreditTypeService>(apiUrl);
    }
    
    public async Task<List<Models.CreditType>> GetCreditsTypeAsync()
    {
        return await _api.GetCreditsTypeAsync();
    }

    public async Task RegisterCreditTypeAsync(Models.CreditType request)
    {
        await _api.RegisterCreditTypeAsync(request);
    }

    public async Task ModifyCreditTypeAsync(Models.CreditType request)
    {
        await _api.ModifyCreditTypeAsync(request);
    }

    public async Task<List<Models.CreditType>> GetActiveCreditTypesAsync()
    {
        return await _api.GetActiveCreditTypesAsync();
    }
}