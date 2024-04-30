using System.Collections.Generic;
using System.Threading.Tasks;
using FinancialManagementSystem.Services.CreditApplication;
using Refit;

namespace FinancialManagementSystem.Services.CreditApplication;

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

    public async Task CreateAplicationAsync(CreditApplicationRequest request)
    {
        _api.CreateAplicationAsync(request);
    }
}