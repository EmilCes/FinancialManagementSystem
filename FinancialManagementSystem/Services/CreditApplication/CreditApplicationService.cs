using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace FinancialManagementSystem.Services.CreditAplication;

public class CreditApplicationService: ICreditApplicationService
{
    private readonly ICreditApplicationService _api;


    public CreditApplicationService(string apiUrl)
    {
        _api = RestService.For<ICreditApplicationService>(apiUrl);
    }
    
    public async Task<VerifyRegularClientResponse> VerifyRegularAsync(string rfc)
    {
        return await _api.VerifyRegularAsync(rfc);
    }

    public async Task<List<CreditType>> GetCreditTypesAsync()
    {
        return await _api.GetCreditTypesAsync();
    }

    public async Task CreateAplicationAsync(CreditAplicationRequest request)
    {
        _api.CreateAplicationAsync(request);
    }
}